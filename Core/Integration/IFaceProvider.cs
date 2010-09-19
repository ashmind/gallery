using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Integration {
    public interface IFaceProvider {
        IEnumerable<Face> GetFaces(Album album);
    }
}
