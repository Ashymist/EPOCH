using EPOCH.Domain.Entities;
using EPOCH.Domain.ValueObjects;
using EPOCH.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace EPOCH.Infrastructure.Tests.Services
{
    public class SgpSatelliteTrackerTest
    {
        private readonly ITestOutputHelper _output;

        public SgpSatelliteTrackerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SgpSatelliteTracker_ShouldReturnValidGeodeticPostion_WithValidData() {
            Satellite sat = new Satellite
            {
                Name = "ISS (ZARYA)",
                NoradId = 25544,
                Group = Domain.Enums.SatelliteGroup.SpaceStation,
                TleData = new Tle(
                    @"1 25544U 98067A   25113.77221326  .00022023  00000+0  39391-3 0  9998", 
                    @"2 25544  51.6359 216.5553 0005513  67.1671 292.9899 15.49908296506721"
                )
            };

            DateTime targetTime = DateTime.UtcNow;

            SgpSatelliteTracker tracker = new();

            var position = tracker.GetGeodeticPosition(sat, targetTime);
            _output.WriteLine($"{position.Latitude}, {position.Longitude}, {position.Altitude}");


        }
    }
}
