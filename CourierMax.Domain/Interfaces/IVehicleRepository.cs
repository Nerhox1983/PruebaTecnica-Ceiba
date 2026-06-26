using CourierMax.Domain.Entities;

namespace CourierMax.Domain.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
        Task<Vehicle?> GetByIdAsync(int vehicleId);
        Task<IEnumerable<Vehicle>> GetByDriverOrPlateAsync(string driverOrPlate);
    }

/*    public class VehicleData
    {

    }*/
}