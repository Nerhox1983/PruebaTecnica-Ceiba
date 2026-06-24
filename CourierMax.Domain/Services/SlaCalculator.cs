using CourierMax.Domain.Enums;

namespace CourierMax.Domain.Services
{
    public class SlaCalculator : ISlaCalculator
    {
        private static readonly HashSet<DateTime> Holidays2026 = new HashSet<DateTime>
        {
            new DateTime(2026, 1, 1),   // 1 Ene
            new DateTime(2026, 1, 26),  // 26 Ene
            new DateTime(2026, 1, 30),  // 30 Ene
            new DateTime(2026, 3, 24),  // 24 Mar
            new DateTime(2026, 5, 1),   // 1 May
            new DateTime(2026, 6, 1),   // 1 Jun
            new DateTime(2026, 6, 29),  // 29 Jun
            new DateTime(2026, 7, 20),  // 20 Jul
            new DateTime(2026, 8, 17),  // 17 Ago
            new DateTime(2026, 10, 20), // 20 Oct
            new DateTime(2026, 11, 9),  // 9 Nov
            new DateTime(2026, 12, 8)   // 8 Dic
        };

        public DateTime CalculateEstimatedDeliveryDate(DateTime startDate, ServiceType serviceType)
        {
            int businessDaysToId = serviceType switch
            {
                ServiceType.MismoDia => 0,
                ServiceType.Express => 1,
                ServiceType.Estandar => 3,
                _ => 3
            };

            if (businessDaysToId == 0)
            {
                return startDate;
            }

            DateTime currentDate = startDate;
            int addedDays = 0;

            while (addedDays < businessDaysToId)
            {
                currentDate = currentDate.AddDays(1);

                if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                    currentDate.DayOfWeek != DayOfWeek.Sunday &&
                    !Holidays2026.Contains(currentDate.Date))
                {
                    addedDays++;
                }
            }

            return currentDate;
        }
    }
}
