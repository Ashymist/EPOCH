using EPOCH.Application.DTOs;
using EPOCH.Application.Interfaces;
using EPOCH.Application.UseCases;
using EPOCH.Domain.Entities;
using EPOCH.Domain.Interfaces;
using EPOCH.Domain.Services;
using EPOCH.Domain.ValueObjects;
using EPOCH.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Application.Tests.UseCases
{
    public class FetchAndStoreSatellitesIfDbEmptyTest
    {
        private readonly Mock<ISatelliteRepository> _repositoryMock = new();
        private readonly Mock<ITleApiClient> _apiClientMock = new();
        private readonly Mock<ISatelliteFactory> _factoryMock = new();
        private readonly Mock<ISatelliteClassifier> _classifierMock = new();

        private readonly FetchAndStoreSatellitesIfDbEmptyUseCase _useCase;

        public FetchAndStoreSatellitesIfDbEmptyTest() {
            _useCase = new FetchAndStoreSatellitesIfDbEmptyUseCase(_classifierMock.Object, _repositoryMock.Object, _apiClientMock.Object, _factoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenDbIsNotEmpty_FetchesAndStoresSatellites() {
            //Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Satellite>());


            var dto = new SatelliteDto
            {
                Name = "Sat-1",
                TleLine1 = "1 00001U 00001A   00000.00000000  .00000000  00000-0  00000-0 0  9991",
                TleLine2 = "2 00001  98.0000 000.0000 0000001 000.0000 000.0000 14.00000000    01"
            };

            _apiClientMock.Setup(a => a.GetAllSatellites()).ReturnsAsync(new[] { dto });

            var tle = new Tle(dto.TleLine1, dto.TleLine2);
            var satellite = new Satellite { Name = dto.Name, TleData = tle, NoradId = 00001, Group = SatelliteGroup.Satellite };

            _factoryMock.Setup(f => f.Create(dto.Name, tle)).Returns(satellite);
            //Act
            await _useCase.ExecuteAsync();
            //Assert
            _repositoryMock.Verify(r =>
            r.ReplaceAllAsync(It.Is<IEnumerable<Satellite>>(s => s.Contains(satellite))),
            Times.Once);
        }
    }
}
