using Moq;
using Xunit;
using CourierMax.Application.Shipments.Queries;
using CourierMax.Application.Shipments.DTOs;
using CourierMax.Domain.Entities;
using CourierMax.Domain.Interfaces;
using CourierMax.Domain.Enums;

namespace CourierMax.Tests.Application.Queries
{
    public class GetDelayedShipmentsQueryHandlerTests
    {
        private readonly Mock<IShipmentRepository> _repositoryMock;
        private readonly GetDelayedShipmentsQueryHandler _handler;

        public GetDelayedShipmentsQueryHandlerTests()
        {
            _repositoryMock = new Mock<IShipmentRepository>();
            _handler = new GetDelayedShipmentsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnMappedDelayedShipments_WhenTheyExistInRepository()
        {
            var startDate = new DateTime(2026, 06, 01);
            var endDate = new DateTime(2026, 06, 25);
            var query = new GetDelayedShipmentsQuery(startDate, endDate);

            var testShipment = CreateValidTestShipment(
                trackingCode: "CM-7CC",
                senderName: "Sergio Cáceres",
                receiverName: "Patricia Cifuentes"
            );

            var fakeShipments = new List<Shipment> { testShipment };

            _repositoryMock
                .Setup(repo => repo.GetDelayedShipmentsAsync(startDate, endDate))
                .ReturnsAsync(fakeShipments);

            var result = await _handler.HandleAsync(query);

            Assert.NotNull(result);
            var resultList = Assert.IsAssignableFrom<IEnumerable<ShipmentDto>>(result);
            var firstItem = resultList.FirstOrDefault();

            Assert.NotNull(firstItem);
            Assert.Equal("CM-7CC", firstItem.TrackingCode);
            Assert.Equal("Sergio Cáceres", firstItem.SenderName);
            Assert.Equal("Patricia Cifuentes", firstItem.ReceiverName);

            _repositoryMock.Verify(repo => repo.GetDelayedShipmentsAsync(startDate, endDate), Times.Once);
        }

        /// <summary>
        /// Método Helper para abstraer el constructor gigante de Shipment en los tests.
        /// Llena los 15 parámetros obligatorios y te permite cambiar solo los que necesitas probar.
        /// </summary>
        private Shipment CreateValidTestShipment(string trackingCode, string senderName, string receiverName)
        {
            var shipment = new Shipment(
                senderName: senderName,
                senderPhone: "3001234567",
                senderAddress: "Calle 123",
                receiverName: receiverName,
                receiverPhone: "3159876543",
                receiverAddress: "Carrera 45",
                originCityId: 1,
                destinationCityId: 2,
                weightKg: 1.5m,
                lengthCm: 20m,
                widthCm: 20m,
                heightCm: 20m,
                packageType: PackageType.Paquete,
                serviceType: ServiceType.MismoDia,
                totalCost: 8000m
            );

            typeof(Shipment)
                .GetProperty(nameof(Shipment.TrackingCode))?
                .SetValue(shipment, trackingCode);

            typeof(Shipment)
                .GetProperty(nameof(Shipment.Id))?
                .SetValue(shipment, 16);

            return shipment;
        }
    }
}