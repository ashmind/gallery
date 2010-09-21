using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.AlbumSupport
{
    internal interface IAlbumIDProvider {
        string GetAlbumID(string name, AlbumDescriptor descriptor);
        AlbumDescriptor GetAlbumDescriptor(string albumID);
    }
}