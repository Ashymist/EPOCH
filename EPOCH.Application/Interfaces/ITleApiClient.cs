using EPOCH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Application.Interfaces
{
    public interface ITleApiClient
    {
        Task<IEnumerable<SatelliteDto>> GetAllSatellites();
    }
}
