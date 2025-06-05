using EPOCH.Domain.Entities;
using EPOCH.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPOCH.Application.UseCases
{
    public class GetAllSatellitesUseCase
    {
        private readonly ISatelliteRepository _repository;

        public GetAllSatellitesUseCase(ISatelliteRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Satellite>> ExecuteAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}