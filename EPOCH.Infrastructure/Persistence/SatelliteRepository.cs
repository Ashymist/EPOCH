using EPOCH.Domain.Entities;
using EPOCH.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Infrastructure.Persistence
{
    public class SatelliteRepository : ISatelliteRepository
    {
        private readonly AppDbContext _context;

        public SatelliteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Satellite?> GetByNoradIdAsync(int noradId) {
            return await _context.Satellites.FindAsync(noradId);
        }

        public async Task<IEnumerable<Satellite>> GetAllAsync() {
            return await _context.Satellites.ToListAsync();
        }

        public async Task DeleteAllAsync() {
            await _context.Satellites.ExecuteDeleteAsync();
        }

        public async Task ReplaceAllAsync(IEnumerable<Satellite> satellites) {
            await DeleteAllAsync();
            _context.Satellites.AddRange(satellites);
            await _context.SaveChangesAsync();
        }
         
    }
}
