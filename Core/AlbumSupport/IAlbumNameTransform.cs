using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.AlbumSupport {
    public interface IAlbumNameTransform {
        string Transform(string albumName, AlbumDescriptor descriptor);
    }
}
