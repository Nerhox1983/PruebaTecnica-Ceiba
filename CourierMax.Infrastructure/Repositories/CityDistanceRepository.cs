using CourierMax.Domain.Interfaces;
using CourierMax.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourierMax.Infrastructure.Repositories
{
    public class CityDistanceRepository : ICityDistanceRepository
    {
        private readonly CourierMaxContext _context;

        public CityDistanceRepository(CourierMaxContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetDistanceSurchargeAsync(int originCityId, int destinationCityId)
        {            
            var distanceConfig = await _context.Set<Domain.Entities.CityDistance>()
                .FirstOrDefaultAsync(cd => cd.OriginCityId == originCityId && cd.DestinationCityId == destinationCityId);

            return distanceConfig?.DistanceTariff ?? 0m;
        }
    }
}