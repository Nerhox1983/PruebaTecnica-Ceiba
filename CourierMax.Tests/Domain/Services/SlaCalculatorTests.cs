using CourierMax.Domain.Enums;
using CourierMax.Domain.Services;

namespace CourierMax.Tests.Domain.Services
{
    public class SlaCalculatorTests
    {
        private readonly SlaCalculator _calculator;

        public SlaCalculatorTests()
        {            
            _calculator = new SlaCalculator();
        }

        [Fact]
        public void CalculateEstimatedDeliveryDate_ShouldSumDaysCorrectly_WhenWithinSameWeek()
        {         
            var tuesday = new DateTime(2026, 6, 23);         
            var expectedFriday = new DateTime(2026, 6, 26);            
            var result = _calculator.CalculateEstimatedDeliveryDate(tuesday, ServiceType.Estandar);
            
            Assert.Equal(expectedFriday.Date, result.Date);
        }

        [Fact]
        public void CalculateEstimatedDeliveryDate_ShouldReturnMonday_WhenCreatedOnFridayWithOneBusinessDay()
        {            
            var friday = new DateTime(2026, 6, 26);
            var expectedDate = new DateTime(2026, 6, 30);
            
            var result = _calculator.CalculateEstimatedDeliveryDate(friday, ServiceType.Express);
            
            Assert.Equal(expectedDate.Date, result.Date);
        }

        [Fact]
        public void CalculateEstimatedDeliveryDate_ShouldSkipWeekend_WhenSlaCrossesSaturdayAndSunday()
        {
            
            var thursday = new DateTime(2026, 6, 25);
            
            var expectedWednesday = new DateTime(2026, 7, 1);

            
            var result = _calculator.CalculateEstimatedDeliveryDate(thursday, ServiceType.Estandar);
            
            Assert.Equal(expectedWednesday.Date, result.Date);
        }

        [Fact]
        public void CalculateEstimatedDeliveryDate_ShouldSkipHoliday_WhenSlaCrossesRegisteredHoliday()
        {            
            var monday = new DateTime(2026, 8, 11);
            var fridayBeforeHoliday = new DateTime(2026, 8, 14);
            var expectedTuesday = new DateTime(2026, 8, 18);            
            var result = _calculator.CalculateEstimatedDeliveryDate(fridayBeforeHoliday, ServiceType.Express);
            
            Assert.Equal(expectedTuesday.Date, result.Date);
        }

        [Fact]
        public void CalculateEstimatedDeliveryDate_ShouldDefaultToThreeDays_WhenServiceTypeIsUnknown()
        {            
            var tuesday = new DateTime(2026, 6, 23);
            
            var unknownService = (ServiceType)99;

            var expectedFriday = new DateTime(2026, 6, 26);
            
            var result = _calculator.CalculateEstimatedDeliveryDate(tuesday, unknownService);

            Assert.Equal(expectedFriday.Date, result.Date);
        }

        [Fact]
        public void CalculateEstimatedDeliveryDate_ShouldReturnSameDate_WhenServiceTypeIsMismoDia()
        {            
            var wednesday = new DateTime(2026, 6, 24);
             
            var result = _calculator.CalculateEstimatedDeliveryDate(wednesday, ServiceType.MismoDia);
         
            Assert.Equal(wednesday.Date, result.Date);
        }
    }
}
