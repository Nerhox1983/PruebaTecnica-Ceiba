using CourierMax.Application.Shipments.Commands;
using CourierMax.Domain.Entities;
using CourierMax.Domain.Enums;
using CourierMax.Domain.Exceptions;
using CourierMax.Domain.Interfaces;
using Moq;

namespace CourierMax.Tests.Application.Commands
{
    public class AssignVehicleCommandHandlerTests
    {
        private readonly Mock<IShipmentRepository> _shipmentRepositoryMock;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly AssignVehicleCommandHandler _handler;

        public AssignVehicleCommandHandlerTests()
        {
            _shipmentRepositoryMock = new Mock<IShipmentRepository>();
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();

            _handler = new AssignVehicleCommandHandler(
                _shipmentRepositoryMock.Object,
                _vehicleRepositoryMock.Object
            );
        }

        private Shipment CreateTestShipment(decimal weightKg, int length, int width, int height)
        {
            return new Shipment(
                senderName: "Carlos Mendoza",
                senderPhone: "3154433221",
                senderAddress: "Carrera 7 # 32-10",
                receiverName: "Ana María Silva",
                receiverPhone: "3107654321",
                receiverAddress: "Calle 100 # 15-22",
                originCityId: 1,
                destinationCityId: 3,
                weightKg: weightKg,
                lengthCm: length,
                widthCm: width,
                heightCm: height,
                packageType: PackageType.Paquete,
                serviceType: ServiceType.Estandar,
                totalCost: 22000m
            );
        }

        [Fact]
        public async Task HandleAsync_ShouldAssignVehicleSuccessfully_WhenConstraintsAreMet()
        {            
            var command = new AssignVehicleCommand { ShipmentId = 20, VehicleId = 1, UserId = "1" };
            var shipment = CreateTestShipment(5.0m, 40, 30, 20);
            var vehicle = new VehicleData
            {
                VehicleId = 1,
                LicensePlate = "ABC-123",
                MaxWeightKg = 500.0m,
                MaxVolumeM3 = 10.0m,
                IsActive = true
            };

            _shipmentRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(shipment);
            _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vehicle);

            var result = await _handler.HandleAsync(command);
            
            Assert.NotNull(result);
            Assert.Equal("ASIGNADO", result.Status);
            Assert.Equal("ABC-123", result.AssignedVehiclePlate);
            _shipmentRepositoryMock.Verify(r => r.UpdateAsync(shipment), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowBusinessException_WhenVehicleIsInactive()
        {
            var command = new AssignVehicleCommand { ShipmentId = 20, VehicleId = 2, UserId = "1" };
            var shipment = CreateTestShipment(5.0m, 40, 30, 20);
            var vehicle = new VehicleData { VehicleId = 2, IsActive = false };

            _shipmentRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(shipment);
            _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(vehicle);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _handler.HandleAsync(command));
            Assert.Contains("conductor activo", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowBusinessException_WhenWeightExceedsCapacity()
        {
            var command = new AssignVehicleCommand { ShipmentId = 17, VehicleId = 4, UserId = "1" };
            var shipment = CreateTestShipment(12.50m, 30, 20, 15);
            var vehicle = new VehicleData
            {
                VehicleId = 4,
                MaxWeightKg = 5.0m,
                MaxVolumeM3 = 1.0m,
                IsActive = true
            };

            _shipmentRepositoryMock.Setup(r => r.GetByIdAsync(17)).ReturnsAsync(shipment);
            _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(vehicle);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _handler.HandleAsync(command));
            Assert.Contains("capacidad máxima de peso", ex.Message);
        }
    }
}
