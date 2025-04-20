using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPOCH.Domain.ValueObjects;

namespace EPOCH.Domain.Tests.ValueObjects
{
    public class GeodeticPositionTest
    {
        [Fact]
        public void GeodeticPosition_Should_Throw_When_Latitude_Is_More_Than_90() {
            Assert.Throws<ArgumentException>(() => new GeodeticPosition(100, 0, 0));
        }

        [Fact]
        public void GeodeticPosition_Should_Throw_When_Latitude_Is_Less_Than_Negative90()
        {
            Assert.Throws<ArgumentException>(() => new GeodeticPosition(-100, 0, 0));
        }

        [Fact]
        public void GeodeticPosition_Should_Throw_When_Longitude_Is_More_Than_180()
        {
            Assert.Throws<ArgumentException>(() => new GeodeticPosition(0, 190, 0));
        }

        [Fact]
        public void GeodeticPosition_Should_Throw_When_Longitude_Is_Less_Than_Negative180()
        {
            Assert.Throws<ArgumentException>(() => new GeodeticPosition(0, -190, 0));
        }

        [Fact]
        public void GeodeticPosition_Should_Throw_When_Altitude_Is_less_Than_0()
        {
            Assert.Throws<ArgumentException>(() => new GeodeticPosition(0, 0, -1));
        }
    }
}
