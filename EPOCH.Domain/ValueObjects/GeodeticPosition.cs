using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Domain.ValueObjects
{
    public sealed record GeodeticPosition {

        public double Latitude { get; }
        public double Longitude { get; }
        public double Altitude { get; }

        public GeodeticPosition(double latitude, double longitude, double altitude)
        {
            if (latitude is < -90 or > 90)
                throw new ArgumentException("Latitude must be between -90 and 90 degrees.");

            if (longitude is < -180 or > 180)
                throw new ArgumentException("Longitude must be between -180 and 180 degrees.");

            if (altitude < 0)
                throw new ArgumentException("Altitude must be non-negative.");

            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }
    }
}
