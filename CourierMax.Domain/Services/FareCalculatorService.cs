using CourierMax.Domain.Enums;

namespace CourierMax.Domain.Services
{
    public class FareCalculatorService
    {
        public (decimal total, decimal baseTariff, decimal weightSurcharge, decimal packageSurcharge) CalculateFare(
            ServiceType serviceType,
            PackageType packageType,
            decimal weightKg,
            decimal distanceSurcharge)
        {            
            decimal baseTariff = serviceType switch
            {
                ServiceType.Estandar => 8000m,
                ServiceType.Express => 15000m,
                ServiceType.MismoDia => 25000m,
                _ => throw new ArgumentOutOfRangeException(nameof(serviceType), "Tipo de servicio no soportado")
            };
         
            decimal weightSurcharge = 0m;
            if (weightKg > 2m)
            {
                weightSurcharge = (weightKg - 2m) * 1500m;
            }
            
            decimal subtotal = baseTariff + weightSurcharge + distanceSurcharge;
            
            decimal packagePercentage = packageType switch
            {
                PackageType.Fragil => 0.30m,
                PackageType.Perecedero => 0.25m,
                PackageType.Paquete => 0.0m,
                _ => 0.0m
            };

            decimal packageSurcharge = subtotal * packagePercentage;
            
            decimal total = subtotal + packageSurcharge;

            return (total, baseTariff, weightSurcharge, packageSurcharge);
        }
    }
}