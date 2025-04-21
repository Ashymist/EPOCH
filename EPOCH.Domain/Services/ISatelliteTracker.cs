using EPOCH.Domain.Entities;
using EPOCH.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Domain.Services
{
    public interface ISatelliteTracker
    {
        GeodeticPosition GetGeodeticPosition(Satellite satellite, DateTime targetTime);
    }
}
