using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPOCH.Domain.Entities;
using EPOCH.Domain.ValueObjects;
using EPOCH.Domain.Enums;

namespace EPOCH.Domain.Tests.Entities
{
    public class SatelliteTest
    {
        [Fact]
        public void Satellite_Should_Throw_When_Name_Is_Null() {
            Assert.Throws<ArgumentException>(() => new Satellite
            {
                Name = null!,
                NoradId = 25554,
                TleData = new Tle(new string('A',69),new string('B',69)),
                Group = SatelliteGroup.Navigation
            });
        }

        [Fact]
        public void Satellite_Should_Throw_When_Name_Is_Empty()
        {
            Assert.Throws<ArgumentException>(() => new Satellite
            {
                Name = String.Empty,
                NoradId = 25554,
                TleData = new Tle(new string('A', 69), new string('B', 69)),
                Group = SatelliteGroup.Navigation
            });
        }

        [Fact]
        public void Satellite_Should_Throw_When_NoradId_Is_Less_Than_1()
        {
            Assert.Throws<ArgumentException>(() => new Satellite
            {
                Name = "123",
                NoradId = 0,
                TleData = new Tle(new string('A', 69), new string('B', 69)),
                Group = SatelliteGroup.Navigation
            });
        }

        [Fact]
        public void Satellite_Should_Throw_When_Tle_Is_Null()
        {
            Assert.Throws<ArgumentException>(() => new Satellite
            {
                Name = "123",
                NoradId = 25564,
                TleData = null!,
                Group = SatelliteGroup.Navigation
            });
        }

        [Fact]
        public void Satellite_Should_Throw_When_Group_Is_Uncategorized()
        {
            Assert.Throws<ArgumentException>(() => new Satellite
            {
                Name = "123",
                NoradId = 25544,
                TleData = new Tle(new string('A', 69), new string('B', 69)),
                Group = SatelliteGroup.Uncategorized
            });
        }

        [Fact]
        public void Satellite_Should_Not_Throw_When_Valid_Data()
        {
            var exception = Record.Exception(() => new Satellite
            {
                Name = "123",
                NoradId = 25554,
                TleData = new Tle(new string('A', 69), new string('B', 69)),
                Group = SatelliteGroup.Navigation
            });
            Assert.Null(exception);
        }
    }
}
