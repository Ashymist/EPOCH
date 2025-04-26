using EPOCH.Domain.Enums;
using EPOCH.Domain.Services;
using EPOCH.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Infrastructure.Services
{
    public class SatelliteClassifier : ISatelliteClassifier
    {
        
        public SatelliteGroup GetSatelliteGroup(Tle tle) {
            int.TryParse(tle.TleLine2.Substring(2, 5), out int noradId);
            double.TryParse(tle.TleLine2.Substring(52, 11).Trim(),CultureInfo.InvariantCulture, out double meanMotion);
            double.TryParse(tle.TleLine2.Substring(8, 8).Trim(), CultureInfo.InvariantCulture, out double inclination);
            double.TryParse("0." + tle.TleLine2.Substring(26, 7).Trim(), CultureInfo.InvariantCulture, out double eccentricity);

            if (noradId == 25544 || noradId == 48274) return SatelliteGroup.SpaceStation;

            if (meanMotion >= 11 && meanMotion <= 13 && inclination >= 50 && inclination <= 65) return SatelliteGroup.Navigation;
            
            if (meanMotion >= 14.8 && meanMotion <= 15.2 && inclination >= 96 && inclination <= 99) return SatelliteGroup.Weather;
            
            if (meanMotion >= 14 && meanMotion <= 16 && inclination >= 30 && inclination <= 60) return SatelliteGroup.Satellite;

            return SatelliteGroup.Other;
        }
    }
}
