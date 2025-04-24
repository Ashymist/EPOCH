using EPOCH.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using EPOCH.Application.DTOs;

namespace EPOCH.Infrastructure.Clients
{
    public class TleApiClient : ITleApiClient
    {
        private readonly HttpClient _httpClient;
        public TleApiClient() {
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<SatelliteDto>> GetAllSatellites() {
            var response = _httpClient.GetAsync("https://celestrak.org/NORAD/elements/gp.php?CATNR=25118&FORMAT=tle");
            string content = await response.Result.Content.ReadAsStringAsync();
            
            List<SatelliteDto> dtoList = new();

            string[] contentLines = content.Split('\n').Where(line => !String.IsNullOrWhiteSpace(line)).ToArray();

            for (int i = 0; i < contentLines.Length; i += 3) {
                dtoList.Add(new SatelliteDto { 
                    Name = contentLines[i], 
                    TleLine1 = contentLines[i + 1], 
                    TleLine2 = contentLines[i + 2] 
                });
            }

            return dtoList;
        }

    }
}
