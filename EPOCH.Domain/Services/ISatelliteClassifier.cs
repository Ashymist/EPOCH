﻿using EPOCH.Domain.Entities;
using EPOCH.Domain.Enums;
using EPOCH.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOCH.Domain.Services
{
    public interface ISatelliteClassifier
    {
        SatelliteGroup GetSatelliteGroup(Tle tleData);
    }
}
