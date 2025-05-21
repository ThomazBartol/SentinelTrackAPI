using SentinelTrack.Domain.Entities;

namespace SentinelTrack.Infrastructure.Repositories
{
    public class YardRepository
    {

        private static readonly List<Yard> _yards = new();

        public List<Yard> GetAll()
        {
            return _yards;
        }

        public Yard? GetById(Guid id)
        {
            return _yards.FirstOrDefault(y => y.Id == id);
        }

        public Yard Add(Yard yard)
        {
            yard.Id = Guid.NewGuid();
            _yards.Add(yard);
            return yard;
        }

        public bool Remove(Guid id)
        {
            var yard = GetById(id);
            if (yard == null) return false;

            return _yards.Remove(yard);
        }

        public bool Update(Yard updatedYard)
        {
            var existing = GetById(updatedYard.Id);
            if (existing == null) return false;

            existing.Name = updatedYard.Name;
            existing.Address = updatedYard.Address;
            existing.PhoneNumber = updatedYard.PhoneNumber;
            existing.Capacity = updatedYard.Capacity;
            existing.Motos = updatedYard.Motos;

            return true;
        }
    }

}
