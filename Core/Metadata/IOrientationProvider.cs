using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Gallery.Imaging;
using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.Metadata {
    public interface IOrientationProvider {
        ImageOrientation GetOrientation(IImage image, IFile imageFile);

        int Priority { get; }
    }
}
