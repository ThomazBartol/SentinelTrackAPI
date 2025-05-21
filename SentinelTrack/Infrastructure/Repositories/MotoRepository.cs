using SentinelTrack.Domain.Entities;
using System.Xml.Linq;

namespace SentinelTrack.Infrastructure.Repositories
{
    public class MotoRepository
    {
        private static readonly List<Moto> _motos = new();

        public List<Moto> GetAll()
        {
            return _motos;
        }

        public Moto? GetById(Guid id)
        {
            return _motos.FirstOrDefault(m => m.Id == id);
        }

        public Moto Add(Moto moto)
        {
            moto.Id = Guid.NewGuid();
            _motos.Add(moto);
            return moto;
        }

        public bool Remove(Guid id)
        {
            var moto = GetById(id);
            if (moto == null) return false;

            return _motos.Remove(moto);
        }

        public bool Update(Moto updatedMoto)
        {
            var existing = GetById(updatedMoto.Id);
            if (existing == null) return false;

            existing.Plate = updatedMoto.Plate;
            existing.Model = updatedMoto.Model;
            existing.Color = updatedMoto.Color;
            existing.YardId = updatedMoto.YardId;

            return true;
        }
    }
}
