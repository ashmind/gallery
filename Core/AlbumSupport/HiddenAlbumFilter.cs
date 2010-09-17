using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.AlbumSupport {
    public class HiddenAlbumFilter : IAlbumFilter {
        public bool ShouldSkip(ILocation location) {
            return location.IsHidden();
        }
    }
}
