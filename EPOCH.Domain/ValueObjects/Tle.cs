using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EPOCH.Domain.ValueObjects
{
    [Owned]
    public sealed record Tle
    {
        public string TleLine1 { get; init; }
        public string TleLine2 { get; init; }

        public Tle(string tleLine1, string tleLine2) {
            if (String.IsNullOrEmpty(tleLine1) || String.IsNullOrEmpty(tleLine2) || tleLine1.Length != 69 || tleLine2.Length != 69) {
                throw new ArgumentException("Invalid satellite TLE data");
            }
            this.TleLine1 = tleLine1;
            this.TleLine2 = tleLine2;
        }
    }
}
