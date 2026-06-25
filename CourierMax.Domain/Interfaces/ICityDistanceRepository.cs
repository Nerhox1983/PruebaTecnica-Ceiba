using System.Text;

namespace CourierMax.Domain.Interfaces
{
    public interface ICityDistanceRepository
    {
        /// <summary>
        /// Obtiene el costo de recargo por distancia configurado entre dos ciudades.
        /// </summary>
        Task<decimal> GetDistanceSurchargeAsync(int originCityId, int destinationCityId);
    }
}
