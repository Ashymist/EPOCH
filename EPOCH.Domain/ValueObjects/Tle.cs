using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Domain.ValueObjects
{
    public sealed record Tle
    {
        public string TleLine1 { get; init; }
        public string TleLine2 { get; init; }

        public Tle(string tle1, string tle2) {
            if (String.IsNullOrEmpty(tle1) || String.IsNullOrEmpty(tle2) || tle1.Length != 69 || tle2.Length != 69) {
                throw new ArgumentException("Invalid satellite TLE data");
            }
            this.TleLine1 = tle1;
            this.TleLine2 = tle2;
        }
    }
}
