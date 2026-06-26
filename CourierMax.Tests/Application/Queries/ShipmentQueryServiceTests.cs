using Moq;
using CourierMax.Infrastructure.Services;
using CourierMax.Domain.Interfaces;
using CourierMax.Domain.Entities;

public class ShipmentQueryServiceTests
{
    private readonly Mock<IShipmentRepository> _mockShipmentRepo;
    private readonly Mock<IVehicleRepository> _mockVehicleRepo;
    private readonly ShipmentQueryService _service;

    public ShipmentQueryServiceTests()
    {
        _mockShipmentRepo = new Mock<IShipmentRepository>();
        _mockVehicleRepo = new Mock<IVehicleRepository>();       
        _service = new ShipmentQueryService(_mockShipmentRepo.Object, _mockVehicleRepo.Object);
    }

    [Fact]
    public async Task GetDriverEfficiencyReportAsync_ShouldReturnZeros_WhenNoShipmentsFound()
    { 
        var driverId = "Juan Pérez";
        _mockVehicleRepo.Setup(r => r.GetByDriverOrPlateAsync(driverId))
        .ReturnsAsync(new List<Vehicle>());

        var result = await _service.GetDriverEfficiencyReportAsync(driverId, DateTime.Now.AddDays(-30), DateTime.Now);

        Assert.Equal(0, result.TotalAssigned);
        Assert.Equal(0, result.TotalWeightTransported);
    }
}
