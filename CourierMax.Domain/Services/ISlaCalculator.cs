using CourierMax.Domain.Enums;

namespace CourierMax.Domain.Services
{
    public interface ISlaCalculator
    {
        DateTime CalculateEstimatedDeliveryDate(DateTime startDate, ServiceType serviceType);
    }
}
