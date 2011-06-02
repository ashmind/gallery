using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.AlbumSupport {
    public interface IAlbumIDProvider {
        string GetAlbumID(string name, AlbumDescriptor descriptor);
        AlbumDescriptor GetAlbumDescriptor(string albumID);
    }
}