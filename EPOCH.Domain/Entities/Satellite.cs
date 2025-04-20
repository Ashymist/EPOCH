using EPOCH.Domain.Enums;
using EPOCH.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Domain.Entities
{
    public class Satellite
    {

        private string _name = null!;
        private int _noradId;
        private Tle _tleData = null!;
        private SatelliteGroup _group;

        public required string Name {
            get => _name;
            init{ 
                ValidateName(value);
                _name = value;
            } 
        }
        public required int NoradId {
            get => _noradId;
            init { 
                ValidateNoradId(value);
                _noradId = value;
            } 
        }

        public required Tle TleData {
            get => _tleData;
            init {
                ValidateTle(value);
                _tleData = value;
            } 
        }
        public required SatelliteGroup Group {
            get => _group;
            init {
                ValidateGroup(value);
                _group = value;
            }
        }

        private void ValidateName(string name) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentException("Invalid satellite name");
            }
        }

        private void ValidateNoradId(int id) {
            if (id < 1) { // id = 1 is the the first ever satellite
                throw new ArgumentException("Invalid NORAD ID"); 
            }
        }

        private void ValidateTle(Tle tle) {
            if (tle is null) {
                throw new ArgumentException("TLE data cannot be null");
            }
        }

        private void ValidateGroup(SatelliteGroup group) {
            if (group == SatelliteGroup.Uncategorized) {
                throw new ArgumentException("Invalid satellite group");
            }
        }
    }
}
