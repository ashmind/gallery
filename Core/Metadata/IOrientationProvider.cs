using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.Metadata {
    public interface IOrientationProvider {
        ImageOrientation GetOrientation(Image image, IFile imageFile);

        int Priority { get; }
    }
}
