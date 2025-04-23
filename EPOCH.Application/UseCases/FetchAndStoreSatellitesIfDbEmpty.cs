using EPOCH.Application.DTOs;
using EPOCH.Application.Interfaces;
using EPOCH.Application.Mappers;
using EPOCH.Domain.Entities;
using EPOCH.Domain.Interfaces;
using EPOCH.Domain.Services;
using EPOCH.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Application.UseCases
{
    public class FetchAndStoreSatellitesIfDbEmptyUseCase
    {
        private readonly ISatelliteClassifier _classifier;
        private readonly ISatelliteRepository _repository;
        private readonly ITleApiClient _tleApiClient;
        private readonly ISatelliteFactory _satelliteFactory;

        public FetchAndStoreSatellitesIfDbEmptyUseCase(ISatelliteClassifier classifier, ISatelliteRepository repository, ITleApiClient tleApiClient, ISatelliteFactory satelliteFactory)
        {
            _classifier = classifier;
            _repository = repository;
            _tleApiClient = tleApiClient;
            _satelliteFactory = satelliteFactory;
        }

        public async Task ExecuteAsync() {
            var satList = await _repository.GetAllAsync();
            if (!satList.Any()) {
                IEnumerable<SatelliteDto> satelliteDtoList = await _tleApiClient.GetAllSatellites();
                List<Satellite> satelliteList = new List<Satellite>();
                foreach (SatelliteDto dto in satelliteDtoList)
                {
                    Tle tleData = new Tle(dto.TleLine1, dto.TleLine2);
                    Satellite sat = _satelliteFactory.Create(dto.Name, tleData);
                    satelliteList.Add(sat);
                    
                }
                await _repository.ReplaceAllAsync(satelliteList);
            }
        }
    }
}
