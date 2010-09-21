﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration {
    public interface IFaceProvider {
        IEnumerable<Face> GetFaces(ILocation location);
    }
}