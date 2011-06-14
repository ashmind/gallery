using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Imaging {
    public interface IImage : IDisposable {
        Size Size { get; }
        IImageMetadata Metadata { get; }

        IImage Resize(Size targetSize);
        void ReorientInPlace(ImageOrientation orientation);
    }
}
