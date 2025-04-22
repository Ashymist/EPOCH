using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPOCH.Domain.Entities;
using EPOCH.Domain.ValueObjects;

namespace EPOCH.Application.Interfaces
{
    public interface ISatelliteTrackingService
    {
        Task<GeodeticPosition> TrackSatellitePosition(Satellite satellite);
    }
}
