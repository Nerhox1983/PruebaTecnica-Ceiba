using CourierMax.Domain.Services;
using CourierMax.Domain.Enums;

namespace CourierMax.Tests.Domain.Services
{
    public class FareCalculatorServiceTests
    {
        private readonly FareCalculatorService _fareCalculatorService;

        public FareCalculatorServiceTests()
        {
            _fareCalculatorService = new FareCalculatorService();
        }

        [Fact]
        public void CalculateFare_ShouldCalculateBasePrice_WhenPackageIsStandardAndNoWeightOrDistanceSurcharge()
        {         
            var serviceType = ServiceType.Estandar;
            var packageType = PackageType.Paquete;
            decimal weightKg = 1.5m;
            decimal distanceSurcharge = 0m;

            var result = _fareCalculatorService.CalculateFare(serviceType, packageType, weightKg, distanceSurcharge);

            // Assert
            Assert.Equal(8000m, result.total);
            Assert.Equal(8000m, result.baseTariff);
            Assert.Equal(0m, result.weightSurcharge);
            Assert.Equal(0m, result.packageSurcharge);
        }

        [Fact]
        public void CalculateFare_ShouldApplyDistanceAndWeightSurchargesCorrectly()
        {
            var serviceType = ServiceType.Express;
            var packageType = PackageType.Paquete;
            decimal weightKg = 4.0m;
            decimal distanceSurcharge = 5000m;

            var result = _fareCalculatorService.CalculateFare(serviceType, packageType, weightKg, distanceSurcharge);

            // Assert
            Assert.Equal(23000m, result.total);
            Assert.Equal(15000m, result.baseTariff);
            Assert.Equal(3000m, result.weightSurcharge);
            Assert.Equal(0m, result.packageSurcharge);
        }

        [Fact]
        public void CalculateFare_ShouldApplyPercentageSurcharge_WhenPackageIsFragilOrPerecedero()
        {

            var serviceType = ServiceType.MismoDia;
            var packageType = PackageType.Fragil;
            decimal weightKg = 2.0m;
            decimal distanceSurcharge = 5000m;

            var result = _fareCalculatorService.CalculateFare(serviceType, packageType, weightKg, distanceSurcharge);

            Assert.Equal(39000m, result.total);
            Assert.Equal(25000m, result.baseTariff);
            Assert.Equal(0m, result.weightSurcharge);
            Assert.Equal(9000m, result.packageSurcharge);
        }

        [Fact]
        public void CalculateFare_ShouldThrowArgumentOutOfRangeException_WhenServiceTypeIsInvalid()
        {
            // Arrange
            var invalidServiceType = (ServiceType)999; // Forzamos un enum inexistente

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _fareCalculatorService.CalculateFare(invalidServiceType, PackageType.Paquete, 1.0m, 0m));
        }
    }
}