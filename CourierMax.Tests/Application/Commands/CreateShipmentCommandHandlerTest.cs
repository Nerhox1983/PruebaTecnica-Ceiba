using Moq;
using CourierMax.Application.Shipments.Commands;
using CourierMax.Domain.Entities;
using CourierMax.Domain.Interfaces;
using CourierMax.Domain.Services;
using CourierMax.Domain.Exceptions;
using CourierMax.Domain.Enums;

namespace CourierMax.Tests.Application.Commands
{
    public class CreateShipmentCommandHandlerTests
    {
        private readonly Mock<IShipmentRepository> _repositoryMock;
        private readonly Mock<ISlaCalculator> _slaCalculatorMock;
        private readonly Mock<ICityDistanceRepository> _cityDistanceRepositoryMock;
        private readonly CreateShipmentCommandHandler _handler;

        public CreateShipmentCommandHandlerTests()
        {
            _repositoryMock = new Mock<IShipmentRepository>();
            _slaCalculatorMock = new Mock<ISlaCalculator>();
            _cityDistanceRepositoryMock = new Mock<ICityDistanceRepository>();
            
            _handler = new CreateShipmentCommandHandler(
                _repositoryMock.Object,
                _slaCalculatorMock.Object,
                _cityDistanceRepositoryMock.Object
            );
        }

        [Fact]
        public async Task HandleAsync_ShouldCreateShipmentSuccessfully_WhenDataIsValid()
        {
            var command = CreateValidCommand();
            var expectedDeliveryDate = DateTime.UtcNow.AddDays(3);

            _repositoryMock
                .Setup(repo => repo.ExistsByTrackingCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _slaCalculatorMock
                .Setup(calc => calc.CalculateEstimatedDeliveryDate(It.IsAny<DateTime>(), It.IsAny<ServiceType>()))
                .Returns(expectedDeliveryDate);

            _cityDistanceRepositoryMock
                .Setup(repo => repo.GetDistanceSurchargeAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(8000m);

            var result = await _handler.HandleAsync(command);

            Assert.NotNull(result);
            Assert.StartsWith("CM-", result.TrackingCode);
            Assert.Equal("CREADO", result.Status);

            _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Shipment>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowBusinessException_WhenPackageTypeIsInvalid()
        {
            var command = new CreateShipmentCommand(
                "Sergio Cáceres", "3154433221", "Calle 100 # 15-22",
                "Tatiana Bernal", "3107654321", "Carrera 7 # 32-10",
                1, 2, 5.5m, 30, 20, 15,
                "TIPO_FALSO_INVALIDO", "Estandar", 15000
            );

            var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.HandleAsync(command));
            Assert.Contains("El tipo de paquete", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowBusinessException_WhenServiceTypeIsInvalid()
        {
            var command = new CreateShipmentCommand(
                "Sergio Cáceres", "3154433221", "Calle 100 # 15-22",
                "Tatiana Bernal", "3107654321", "Carrera 7 # 32-10",
                1, 2, 5.5m, 30, 20, 15,
                "Paquete", "SERVICIO_FALSO_INVALIDO", 15000
            );

            var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.HandleAsync(command));
            Assert.Contains("El tipo de servicio", exception.Message);
        }

        [Fact]
        public async Task HandleAsync_ShouldRegenerateTrackingCode_WhenFirstCodeAlreadyExists()
        {
            var command = CreateValidCommand();

            _repositoryMock
                .SetupSequence(repo => repo.ExistsByTrackingCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _slaCalculatorMock
                .Setup(calc => calc.CalculateEstimatedDeliveryDate(It.IsAny<DateTime>(), It.IsAny<ServiceType>()))
                .Returns(DateTime.UtcNow.AddDays(1));

            _cityDistanceRepositoryMock
                .Setup(repo => repo.GetDistanceSurchargeAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(8000m);

            var result = await _handler.HandleAsync(command);

            Assert.NotNull(result);

            _repositoryMock.Verify(repo => repo.ExistsByTrackingCodeAsync(It.IsAny<string>()), Times.Exactly(2));
            _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Shipment>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowException_WhenTrackingCodeCollidesMoreThanFiveTimes()
        {
            var command = CreateValidCommand();

            _repositoryMock
                .Setup(repo => repo.ExistsByTrackingCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.HandleAsync(command));
            Assert.Contains("RN-05: No se pudo generar un código de rastreo único disponible", exception.Message);
        }

        private CreateShipmentCommand CreateValidCommand()
        {
            return new CreateShipmentCommand(
                "Sergio Cáceres",
                "3154433221",
                "Calle 100 # 15-22",
                "Tatiana Bernal",
                "3107654321",
                "Carrera 7 # 32-10",
                1,
                2,
                5.5m,
                30,
                20,
                15,
                "Paquete",
                "Estandar",
                15000
            );
        }
    }
}