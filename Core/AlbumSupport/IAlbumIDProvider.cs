using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.AlbumSupport
{
    internal interface IAlbumIDProvider {
        string GetAlbumID(string location);
        string GetAlbumLocation(string albumID);
    }
}
