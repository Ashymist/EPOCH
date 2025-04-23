using EPOCH.Domain.Entities;
using EPOCH.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Application.Interfaces
{
    public interface ISatelliteFactory
    {
        Satellite Create(string name, Tle tle);
    }
}
