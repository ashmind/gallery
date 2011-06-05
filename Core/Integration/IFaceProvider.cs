﻿using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.Integration {
    public interface IFaceProvider {
        IEnumerable<Face> GetFaces(ILocation location);
    }
}
