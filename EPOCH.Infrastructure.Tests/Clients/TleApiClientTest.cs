using EPOCH.Application.DTOs;
using EPOCH.Infrastructure.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace EPOCH.Infrastructure.Tests.Clients
{
    public class TleApiClientTest
    {
        private readonly ITestOutputHelper _output;

        public TleApiClientTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetAllSatellites_Should_Return_Valid_Satellite_List() {
            TleApiClient apiClient = new();
            List<SatelliteDto> dtoList = (List<SatelliteDto>)await apiClient.GetAllSatellites();
            Assert.True(dtoList.Count > 0);
        }
    }
}
