using CourierMax.Domain.Interfaces;
using CourierMax.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using CourierMax.Domain.Entities;

namespace CourierMax.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly CourierMaxContext _context;

        public VehicleRepository(CourierMaxContext context)
        {
            _context = context;
        }

        public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
        {
            const string query = @"
                SELECT TOP 1 VehicleId, LicensePlate, MaxWeightKg, MaxVolumeM3, IsActive 
                FROM dbo.Vehicles 
                WHERE LicensePlate = @LicensePlate";

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = query;

            var parameter = new SqlParameter("@LicensePlate", SqlDbType.NVarChar) { Value = licensePlate };
            command.Parameters.Add(parameter);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Vehicle
                {
                    VehicleId = reader.GetInt32(0),
                    LicensePlate = reader.GetString(1),
                    MaxWeightKg = reader.GetDecimal(2),
                    MaxVolumeM3 = reader.GetDecimal(3),
                    IsActive = reader.GetBoolean(4)
                };
            }
            return null;
        }
        
        public async Task<Vehicle?> GetByIdAsync(int vehicleId)
        {
            const string query = @"
                SELECT TOP 1 VehicleId, LicensePlate, MaxWeightKg, MaxVolumeM3, IsActive 
                FROM dbo.Vehicles 
                WHERE VehicleId = @VehicleId";

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = query;

            var parameter = new SqlParameter("@VehicleId", SqlDbType.Int) { Value = vehicleId };
            command.Parameters.Add(parameter);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Vehicle
                {
                    VehicleId = reader.GetInt32(0),
                    LicensePlate = reader.GetString(1),
                    MaxWeightKg = reader.GetDecimal(2),
                    MaxVolumeM3 = reader.GetDecimal(3),
                    IsActive = reader.GetBoolean(4)
                };
            }
            return null;
        }

        public async Task<IEnumerable<Vehicle>> GetByDriverOrPlateAsync(string driverOrPlate)
        {
            return await _context.Vehicles
                .Where(v => v.DriverName == driverOrPlate || v.LicensePlate == driverOrPlate)
                .ToListAsync();
        }

    }
}
