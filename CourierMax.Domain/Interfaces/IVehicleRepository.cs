namespace CourierMax.Domain.Interfaces
{
    public interface IVehicleRepository
    {
        Task<VehicleData?> GetByLicensePlateAsync(string licensePlate);
        Task<VehicleData?> GetByIdAsync(int vehicleId);
    }

    public class VehicleData
    {
        public int VehicleId { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public decimal MaxWeightKg { get; set; }
        public decimal MaxVolumeM3 { get; set; }
        public bool IsActive { get; set; }
    }
}