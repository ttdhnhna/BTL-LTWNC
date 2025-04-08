using BTL_LTWNC.Models;

namespace BTL_LTWNC.Repository
{
    public interface IVehicleRepository
    {
        void AddVehicle(Vehicle vehicle);
        void UpdateVehicle(Vehicle vehicle);
    }
}
