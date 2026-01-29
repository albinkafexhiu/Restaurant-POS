using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Repository.Interfaces;
using RestaurantPOS.Service.Interfaces;

namespace RestaurantPOS.Service.Implementation
{
    public class WaiterService : IWaiterService
    {
        private readonly IRepository<Waiter> _waiterRepository;

        public WaiterService(IRepository<Waiter> waiterRepository)
        {
            _waiterRepository = waiterRepository;
        }

        public IEnumerable<Waiter> GetAll()
        {
            return _waiterRepository.GetAll();
        }

        public Waiter? GetById(Guid id)
        {
            return _waiterRepository.Get(id);
        }

        public void Create(Waiter entity)
        {
            _waiterRepository.Insert(entity);
        }

        public void Update(Waiter entity)
        {
            _waiterRepository.Update(entity);
        }

        public void Delete(Guid id)
        {
            var waiter = _waiterRepository.Get(id);
            if (waiter != null)
            {
                _waiterRepository.Delete(waiter);
            }
        }

        public Waiter? LoginWithPin(string pinCode)
        {
            return _waiterRepository
                .GetAll()
                .FirstOrDefault(w => w.PinCode == pinCode && w.IsActive && !w.IsManager);
        }

        public Waiter? LoginManagerWithPin(string pinCode)
        {
            return _waiterRepository
                .GetAll()
                .FirstOrDefault(w => w.PinCode == pinCode && w.IsActive && w.IsManager);
        }
    }
}