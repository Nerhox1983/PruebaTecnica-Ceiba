namespace CourierMax.Domain.Entities
{
    public class CityDistance
    {        
        public int OriginCityId { get; private set; }
        public int DestinationCityId { get; private set; }
        public int DistanceKm { get; private set; }
        public decimal DistanceTariff { get; private set; }
        
        private CityDistance() { }

        public CityDistance(int originCityId, int destinationCityId, int distanceKm, decimal distanceTariff)
        {
            OriginCityId = originCityId;
            DestinationCityId = destinationCityId;
            DistanceKm = distanceKm;
            DistanceTariff = distanceTariff;
        }
    }
}
