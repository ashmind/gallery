using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.AlbumSupport {
    public interface IAlbumNameTransform {
        string Transform(string albumName, AlbumDescriptor descriptor);
    }
}
