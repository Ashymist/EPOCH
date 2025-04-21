using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPOCH.Domain.ValueObjects;

namespace EPOCH.Domain.Tests.ValueObjects
{
    public class GeodeticPositionTest
    {
        [Fact]
        public void Should_Throw_When_Latitude_Is_Invalid()
        {
            Assert.Throws<ArgumentException>(() => new GeodeticPosition(100, 0, 100));
        }

        [Fact]
        public void Should_Throw_When_Longitude_Is_Invalid()
        {
            Assert.Throws<ArgumentException>(() => new GeodeticPosition(0, 190, 100));
        }

        [Fact]
        public void Should_Throw_When_Altitude_Is_Negative()
        {
            Assert.Throws<ArgumentException>(() => new GeodeticPosition(0, 0, -1));
        }

        [Fact]
        public void Should_Create_When_All_Values_Are_Valid()
        {
            var exception = Record.Exception(() => new GeodeticPosition(0, 0, 100));
            Assert.Null(exception);

        }
    }
}
