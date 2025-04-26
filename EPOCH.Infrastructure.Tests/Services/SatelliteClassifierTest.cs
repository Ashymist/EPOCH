using EPOCH.Domain.Enums;
using EPOCH.Domain.ValueObjects;
using EPOCH.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Infrastructure.Tests.Services
{
    public class SatelliteClassifierTest
    {
        [Fact]
        public void GetSatelliteGroup_ShouldReturnValidGroup_With_ValidTle() {
            Tle tle = new Tle(@"1 59398U 24064A   25111.94610567  .00000486  00000+0  27197-4 0  9995", @"2 59398  42.9994  72.8313 0001238 174.7387 185.3477 15.27579901 59968");
            SatelliteClassifier classifier = new();
            Assert.Equal(SatelliteGroup.Satellite, classifier.GetSatelliteGroup(tle));
        }
    }
}
