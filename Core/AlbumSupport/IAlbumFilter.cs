using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.AlbumSupport {
    public interface IAlbumFilter {
        bool ShouldSkip(string location);
    }
}
