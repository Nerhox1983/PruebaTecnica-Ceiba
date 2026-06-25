using Moq;
using CourierMax.Application.Shipments.Queries;
using CourierMax.Domain.Entities;
using CourierMax.Domain.Enums;
using CourierMax.Domain.Interfaces;

namespace CourierMax.Tests.Application.Queries
{
    public class GetShipmentHistoryQueryHandlerTests
    {
        private readonly Mock<IShipmentRepository> _shipmentRepositoryMock;
        private readonly GetShipmentHistoryQueryHandler _handler;

        public GetShipmentHistoryQueryHandlerTests()
        {
            _shipmentRepositoryMock = new Mock<IShipmentRepository>();
            _handler = new GetShipmentHistoryQueryHandler(_shipmentRepositoryMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnMappedDtoList_WhenHistoryExists()
        {
            int shipmentId = 16;
            var mockLogs = new List<ShipmentStatusHistory>
            {
                new ShipmentStatusHistory
                {
                    Id = 1,
                    ShipmentId = shipmentId,
                    PreviousStatus = ShipmentStatus.CREADO,
                    NewStatus = ShipmentStatus.ASIGNADO,
                    ChangedAt = new DateTime(2026, 6, 24, 10, 0, 0, DateTimeKind.Utc),
                    UserId = "user-1",
                    ChangeReason = "Asignación de vehículo"
                }
            };

            _shipmentRepositoryMock
                .Setup(repo => repo.GetStatusHistoryAsync(shipmentId))
                .ReturnsAsync(mockLogs);

            var result = await _handler.HandleAsync(shipmentId);

            Assert.NotNull(result);
            Assert.Single(result);
            var dto = result.First();
            Assert.Equal(1, dto.Id);
            Assert.Equal("CREADO", dto.PreviousStatus);
            Assert.Equal("ASIGNADO", dto.NewStatus);
            Assert.Equal("user-1", dto.UserId);
            Assert.Equal("Asignación de vehículo", dto.ChangeReason);
            _shipmentRepositoryMock.Verify(repo => repo.GetStatusHistoryAsync(shipmentId), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnEmptyList_WhenNoHistoryExists()
        {
            int shipmentId = 99;
            _shipmentRepositoryMock
                .Setup(repo => repo.GetStatusHistoryAsync(shipmentId))
                .ReturnsAsync(new List<ShipmentStatusHistory>());

            var result = await _handler.HandleAsync(shipmentId);

            Assert.NotNull(result);
            Assert.Empty(result);
            _shipmentRepositoryMock.Verify(repo => repo.GetStatusHistoryAsync(shipmentId), Times.Once);
        }
    }
}
