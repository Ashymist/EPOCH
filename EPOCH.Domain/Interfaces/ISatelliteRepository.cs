using EPOCH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Domain.Interfaces
{
    public interface ISatelliteRepository
    {
        Task<Satellite?> GetByNoradIdAsync(int noradId);
        Task<IEnumerable<Satellite>> GetAllAsync();
        Task RefreshAllAsync(IEnumerable<Satellite> satellites);
        Task DeleteAllAsync();

    }
}
