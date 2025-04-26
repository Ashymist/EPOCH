using EPOCH.Application.Interfaces;
using EPOCH.Domain.Entities;
using EPOCH.Domain.Enums;
using EPOCH.Domain.Services;
using EPOCH.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Application.Factories
{
    public class SatelliteFactory : ISatelliteFactory
    {
        private ISatelliteClassifier _classifier;
        public SatelliteFactory(ISatelliteClassifier classifier) {
            _classifier = classifier;
        }

        public Satellite Create(string name, Tle tle) {

            if (!int.TryParse(tle.TleLine1.Substring(2, 5), out int noradid))
            {
                throw new ArgumentException("Invalid NORAD ID in TLE line 1");
            }

            SatelliteGroup group = _classifier.GetSatelliteGroup(tle);

            return new Satellite
            {
                Name = name,
                NoradId = noradid,
                TleData = tle,
                Group = group
            };
        }
    }
}
