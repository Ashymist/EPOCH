using EPOCH.Domain.Entities;
using EPOCH.Domain.Services;
using EPOCH.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Infrastructure.Services
{
    public class SgpSatelliteTracker : ISatelliteTracker
    {
        public GeodeticPosition GetGeodeticPosition(Satellite satellite, DateTime targetTime) {
            SGPdotNET.Observation.Satellite sat = new(satellite.TleData.TleLine1, satellite.TleData.TleLine2);
            var eci = sat.Predict(targetTime);
            double latitude = eci.ToGeodetic().Latitude.Degrees;
            double longitude = eci.ToGeodetic().Longitude.Degrees;
            double altitude = eci.ToGeodetic().Altitude;

            return new GeodeticPosition(latitude, longitude, altitude);

        }
    }
}
