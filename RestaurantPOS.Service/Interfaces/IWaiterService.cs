using RestaurantPOS.Domain.Entities;

namespace RestaurantPOS.Service.Interfaces
{
    public interface IWaiterService
    {
        IEnumerable<Waiter> GetAll();
        Waiter? GetById(Guid id);

        void Create(Waiter entity);
        void Update(Waiter entity);
        void Delete(Guid id);

        Waiter? LoginWithPin(string pinCode);
        
        Waiter? LoginManagerWithPin(string pinCode);
    }
}