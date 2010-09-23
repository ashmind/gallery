using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.Integration {
    public interface IFaceProvider {
        IEnumerable<Face> GetFaces(ILocation location);
    }
}
