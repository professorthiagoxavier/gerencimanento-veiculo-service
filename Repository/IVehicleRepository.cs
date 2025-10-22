using Domain;

namespace Repository
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<int> AddVehicleAsync(Vehicle vehicle);
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task DeleteVehicleAsync(int id);
    }
}
