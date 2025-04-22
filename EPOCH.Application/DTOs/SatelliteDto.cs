using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Application.DTOs
{
    public class SatelliteDto
    {
        public required string Name { get; set; } = null!;
        public required string TleLine1 { get; set; } = null!;
        public required string TleLine2 { get; set; } = null!;
    }
}
