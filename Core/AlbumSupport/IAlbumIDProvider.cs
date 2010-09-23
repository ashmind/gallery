using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.AlbumSupport
{
    public interface IAlbumIDProvider {
        string GetAlbumID(string name, AlbumDescriptor descriptor);
        AlbumDescriptor GetAlbumDescriptor(string albumID);
    }
}