using Dapper;
using Domain;
using MySqlConnector;

namespace Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly MySqlConnection _connection;
        public VehicleRepository(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            await _connection.OpenAsync();
            string sql = "SELECT id, brand, model, year, plate, color FROM vehicle;";
            var vehicles = await _connection.QueryAsync<Vehicle>(sql);
            await _connection.CloseAsync();
            return vehicles;
        }

        public async Task<int> AddVehicleAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle), "Veículo inválido.");
            await _connection.OpenAsync();
            string sql = @"
                INSERT INTO vehicle (brand, model, year, plate, color)
                VALUES (@Brand, @Model, @Year, @Plate, @Color);
                SELECT LAST_INSERT_ID();
            ";
            var id = await _connection.ExecuteScalarAsync<int>(sql, vehicle);
            await _connection.CloseAsync();
            return id;
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            if (vehicle == null || vehicle.Id <= 0)
                throw new ArgumentException("Veículo inválido.", nameof(vehicle));
            await _connection.OpenAsync();
            string sql = @"
                UPDATE vehicle
                SET brand = @Brand, model = @Model, year = @Year, plate = @Plate, color = @Color
                WHERE id = @Id;
            ";
            await _connection.ExecuteAsync(sql, vehicle);
            await _connection.CloseAsync();
        }

        public async Task DeleteVehicleAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID inválido.", nameof(id));
            await _connection.OpenAsync();
            string sql = "DELETE FROM vehicle WHERE id = @Id;";
            await _connection.ExecuteAsync(sql, new { Id = id });
            await _connection.CloseAsync();
        }
    }
}
