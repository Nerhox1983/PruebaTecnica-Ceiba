using CourierMax.Domain.Enums;
using CourierMax.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CourierMax.Domain.Entities
{
    public class Shipment
    {
        private static readonly Regex ColombiaPhoneRegex = new Regex(@"^(3[0-9]{9}|6[0-9]{9})$", RegexOptions.Compiled);
        private readonly List<ShipmentStatusHistory> _statusHistory = new();

        public int Id { get; private set; }
        public string TrackingCode { get; set; } = string.Empty;
        public string SenderName { get; private set; } = string.Empty;
        public string SenderPhone { get; private set; } = string.Empty;
        public string SenderAddress { get; private set; } = string.Empty;
        public string ReceiverName { get; private set; } = string.Empty;
        public string ReceiverPhone { get; private set; } = string.Empty;
        public string ReceiverAddress { get; private set; } = string.Empty;
        public int OriginCityId { get; private set; }
        public int DestinationCityId { get; private set; }
        public decimal WeightKg { get; private set; }
        public decimal LengthCm { get; private set; }
        public decimal WidthCm { get; private set; }
        public decimal HeightCm { get; private set; }
        public PackageType PackageType { get; private set; }
        public ServiceType ServiceType { get; private set; }
        public decimal TotalCost { get; private set; }
        public ShipmentStatus CurrentStatus { get; private set; }
        public int? VehicleId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime EstimatedDeliveryDate { get; set; }

        public IReadOnlyCollection<ShipmentStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

        [ExcludeFromCodeCoverage]
        private Shipment()
        {
        }

        public Shipment(
            string senderName, string senderPhone, string senderAddress,
            string receiverName, string receiverPhone, string receiverAddress,
            int originCityId, int destinationCityId,
            decimal weightKg, decimal lengthCm, decimal widthCm, decimal heightCm,
            PackageType packageType, ServiceType serviceType, decimal totalCost)
        {
            ValidateData(senderPhone, receiverPhone, senderAddress, receiverAddress, weightKg, lengthCm, widthCm, heightCm);

            TrackingCode = GenerateUniqueTrackingCode();
            SenderName = senderName;
            SenderPhone = senderPhone;
            SenderAddress = senderAddress;
            ReceiverName = receiverName;
            ReceiverPhone = receiverPhone;
            ReceiverAddress = receiverAddress;
            OriginCityId = originCityId;
            DestinationCityId = destinationCityId;
            WeightKg = weightKg;
            LengthCm = lengthCm;
            WidthCm = widthCm;
            HeightCm = heightCm;
            PackageType = packageType;
            ServiceType = serviceType;
            TotalCost = totalCost;
            CurrentStatus = ShipmentStatus.CREADO;
            CreatedAt = DateTime.UtcNow;

            _statusHistory.Add(new ShipmentStatusHistory(ShipmentStatus.CREADO, ShipmentStatus.CREADO, "System", "Registro inicial del envío."));
        }

        private void ValidateData(
            string senderPhone, string receiverPhone, string senderAddress, string receiverAddress,
            decimal weight, decimal length, decimal width, decimal height)
        {
            if (string.IsNullOrWhiteSpace(senderAddress) || string.IsNullOrWhiteSpace(receiverAddress))
                throw new BusinessException("RN-04: Las direcciones no pueden estar vacías.");

            if (!ColombiaPhoneRegex.IsMatch(senderPhone) || !ColombiaPhoneRegex.IsMatch(receiverPhone))
                throw new BusinessException("RN-04: El teléfono debe tener 10 dígitos y comenzar con 3 o 6.");

            if (weight < 0.1m || weight > 100m)
                throw new BusinessException("RN-04: El peso por envío debe estar entre 0.1 kg y 100 kg.");

            if (length < 1 || length > 200 || width < 1 || width > 200 || height < 1 || height > 200)
                throw new BusinessException("RN-04: Las dimensiones deben estar entre 1 cm y 200 cm por cada lado.");
        }

        private string GenerateUniqueTrackingCode()
        {
            var random = new Random();
            return $"CM-{random.Next(10000000, 99999999)}";
        }

        public void RegenerateTrackingCode()
        {
            TrackingCode = GenerateUniqueTrackingCode();
        }

        public void AssignVehicle(int vehicleId, decimal maxWeightKg, decimal maxVolumeM3, bool isDriverActive, string userId)
        {            
            if (!isDriverActive)
                throw new BusinessException("El envío solo puede asignarse a un conductor activo.");
            
            if (WeightKg > maxWeightKg)
                throw new BusinessException($"La asignación excede la capacidad máxima de peso del vehículo ({maxWeightKg} kg).");
            
            decimal shipmentVolumeM3 = (LengthCm * WidthCm * HeightCm) / 1000000m;
            if (shipmentVolumeM3 > maxVolumeM3)
                throw new BusinessException($"La asignación excede la capacidad máxima de volumen del vehículo ({maxVolumeM3} m³).");
            
            var oldStatus = CurrentStatus;
            VehicleId = vehicleId;
            CurrentStatus = ShipmentStatus.ASIGNADO;

            _statusHistory.Add(new ShipmentStatusHistory(oldStatus, ShipmentStatus.ASIGNADO, "System"));
        }

        public void Transit(string userId, string reason)
        {
            if (CurrentStatus != ShipmentStatus.ASIGNADO)
                throw new InvalidOperationException("El envío debe estar asignado a un vehículo antes de iniciar tránsito.");

            var oldStatus = CurrentStatus;
            CurrentStatus = ShipmentStatus.EN_TRANSITO;
            
            _statusHistory.Add(new ShipmentStatusHistory(oldStatus, ShipmentStatus.EN_TRANSITO, userId, reason));
        }

        public void Deliver(string userId, string reason)
        {
            if (CurrentStatus != ShipmentStatus.EN_TRANSITO)
                throw new InvalidOperationException("No se puede entregar un envío que no esté en tránsito.");

            var oldStatus = CurrentStatus;
            CurrentStatus = ShipmentStatus.ENTREGADO;
            
            _statusHistory.Add(new ShipmentStatusHistory(oldStatus, ShipmentStatus.ENTREGADO, userId, reason));
        }
    }
}