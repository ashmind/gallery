using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Metadata {
    public interface IOrientationProvider {
        ImageOrientation GetOrientation(Image image, IFile imageFile);

        int Priority { get; }
    }
}
