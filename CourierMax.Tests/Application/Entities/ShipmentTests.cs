using CourierMax.Domain.Entities;
using CourierMax.Domain.Enums;
using CourierMax.Domain.Exceptions;

namespace CourierMax.Tests.Domain.Entities
{
    public class ShipmentTests
    {
        private Shipment CreateValidTestShipment()
        {
            return new Shipment(
                senderName: "Sergio Cáceres",
                senderPhone: "3101234567",
                senderAddress: "Calle 26 # 68-90, Bogotá",
                receiverName: "Lina Novoa",
                receiverPhone: "3159876543",
                receiverAddress: "Carrera 7 # 45-10, Bogotá",
                originCityId: 2,
                destinationCityId: 3,
                weightKg: 5.0m,
                lengthCm: 20,
                widthCm: 20,
                heightCm: 20,
                packageType: PackageType.Paquete,
                serviceType: ServiceType.Estandar,
                totalCost: 14600m
            );
        }

        #region Pruebas para AssignVehicle

        [Fact]
        public void AssignVehicle_ShouldSetLicensePlateAndChangeStatusToAsignado_WhenLicensePlateIsValid()
        {
            var shipment = CreateValidTestShipment();
            var validPlate = "ABC-123";
            var testUserId = "user-test-123";

            shipment.AssignVehicle(validPlate, testUserId);

            Assert.Equal(validPlate, shipment.VehicleId);
            Assert.Equal(ShipmentStatus.ASIGNADO, shipment.CurrentStatus);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void AssignVehicle_ShouldThrowBusinessException_WhenLicensePlateIsNullOrEmpty(string invalidPlate)
        {
            var shipment = CreateValidTestShipment();
            var testUserId = "user-test-123";

            var exception = Assert.Throws<BusinessException>(() => shipment.AssignVehicle(invalidPlate, testUserId));
            Assert.Equal("La placa del vehículo no es válida.", exception.Message);
        }

        #endregion

        #region Pruebas para Transit

        [Fact]
        public void Transit_ShouldChangeStatusToEnTransito_WhenCurrentStatusIsAsignado()
        {
            var shipment = CreateValidTestShipment();
            var testUserId = "user-test-123";
            var reason = "Inicio de ruta troncal nacional";
            shipment.AssignVehicle("XYZ789", testUserId);

            shipment.Transit(testUserId, reason);

            Assert.Equal(ShipmentStatus.EN_TRANSITO, shipment.CurrentStatus);
            Assert.Equal(reason, shipment.StatusHistory.Last().ChangeReason);
        }

        [Fact]
        public void Transit_ShouldThrowInvalidOperationException_WhenCurrentStatusIsNotAsignado()
        {
            var shipment = CreateValidTestShipment();
            var testUserId = "user-test-123";
            var reason = "Inicio de ruta";

            var exception = Assert.Throws<InvalidOperationException>(() => shipment.Transit(testUserId, reason));
            Assert.Equal("El envío debe estar asignado a un vehículo antes de iniciar tránsito.", exception.Message);
        }

        #endregion

        #region Pruebas para Deliver

        [Fact]
        public void Deliver_ShouldChangeStatusToEntregado_WhenCurrentStatusIsEnTransito()
        {
            var shipment = CreateValidTestShipment();
            var testUserId = "user-test-123";
            var reason = "Entregado en portería con firma";
            shipment.AssignVehicle("XYZ-789", testUserId);
            shipment.Transit(testUserId, "En ruta");

            shipment.Deliver(testUserId, reason);

            Assert.Equal(ShipmentStatus.ENTREGADO, shipment.CurrentStatus);
            Assert.Equal(reason, shipment.StatusHistory.Last().ChangeReason);
        }

        [Fact]
        public void Deliver_ShouldThrowInvalidOperationException_WhenCurrentStatusIsNotEnTransito()
        {
            var shipment = CreateValidTestShipment();
            var testUserId = "user-test-123";
            var reason = "Entrega";

            var exception = Assert.Throws<InvalidOperationException>(() => shipment.Deliver(testUserId, reason));
            Assert.Equal("No se puede entregar un envío que no esté en tránsito.", exception.Message);
        }

        #endregion

        #region Pruebas de Validación del Constructor (ValidateData)

        [Theory]
        [InlineData("", "Dirección Destino")]
        [InlineData("Dirección Origen", "")]
        [InlineData(null, "Dirección Destino")]
        public void Constructor_ShouldThrowBusinessException_WhenAddressesAreNullOrWhiteSpace(string senderAddress, string receiverAddress)
        {
            var exception = Assert.Throws<BusinessException>(() => new Shipment(
                senderName: "Sergio Cáceres",
                senderPhone: "3101234567",
                senderAddress: senderAddress,
                receiverName: "Lina Novoa",
                receiverPhone: "3159876543",
                receiverAddress: receiverAddress,
                originCityId: 2,
                destinationCityId: 3,
                weightKg: 5.0m,
                lengthCm: 20,
                widthCm: 20,
                heightCm: 20,
                packageType: PackageType.Paquete,
                serviceType: ServiceType.Estandar,
                totalCost: 14600m
            ));

            Assert.Equal("RN-04: Las direcciones no pueden estar vacías.", exception.Message);
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("31012345678")]
        [InlineData("2101234567")]
        public void Constructor_ShouldThrowBusinessException_WhenPhonesAreInvalid(string invalidPhone)
        {
            var exception = Assert.Throws<BusinessException>(() => new Shipment(
                senderName: "Sergio Cáceres",
                senderPhone: invalidPhone,
                senderAddress: "Calle 26 # 68-90, Bogotá",
                receiverName: "Lina Novoa",
                receiverPhone: "3159876543",
                receiverAddress: "Carrera 7 # 45-10, Bogotá",
                originCityId: 2,
                destinationCityId: 3,
                weightKg: 5.0m,
                lengthCm: 20,
                widthCm: 20,
                heightCm: 20,
                packageType: PackageType.Paquete,
                serviceType: ServiceType.Estandar,
                totalCost: 14600m
            ));

            Assert.Equal("RN-04: El teléfono debe tener 10 dígitos y comenzar con 3 o 6.", exception.Message);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(100.1)]
        public void Constructor_ShouldThrowBusinessException_WhenWeightIsOutOfRange(decimal invalidWeight)
        {
            var exception = Assert.Throws<BusinessException>(() => new Shipment(
                senderName: "Sergio Cáceres",
                senderPhone: "3101234567",
                senderAddress: "Calle 26 # 68-90, Bogotá",
                receiverName: "Lina Novoa",
                receiverPhone: "3159876543",
                receiverAddress: "Carrera 7 # 45-10, Bogotá",
                originCityId: 2,
                destinationCityId: 3,
                weightKg: invalidWeight,
                lengthCm: 20,
                widthCm: 20,
                heightCm: 20,
                packageType: PackageType.Paquete,
                serviceType: ServiceType.Estandar,
                totalCost: 14600m
            ));

            Assert.Equal("RN-04: El peso por envío debe estar entre 0.1 kg y 100 kg.", exception.Message);
        }

        [Theory]
        [InlineData(0, 20, 20)]
        [InlineData(201, 20, 20)]
        [InlineData(20, 0, 20)]
        [InlineData(20, 20, 201)]
        public void Constructor_ShouldThrowBusinessException_WhenDimensionsAreOutOfRange(int length, int width, int height)
        {
            var exception = Assert.Throws<BusinessException>(() => new Shipment(
                senderName: "Sergio Cáceres",
                senderPhone: "3101234567",
                senderAddress: "Calle 26 # 68-90, Bogotá",
                receiverName: "Lina Novoa",
                receiverPhone: "3159876543",
                receiverAddress: "Carrera 7 # 45-10, Bogotá",
                originCityId: 2,
                destinationCityId: 3,
                weightKg: 5.0m,
                lengthCm: length,
                widthCm: width,
                heightCm: height,
                packageType: PackageType.Paquete,
                serviceType: ServiceType.Estandar,
                totalCost: 14600m
            ));

            Assert.Equal("RN-04: Las dimensiones deben estar entre 1 cm y 200 cm por cada lado.", exception.Message);
        }

        #endregion
    }
}