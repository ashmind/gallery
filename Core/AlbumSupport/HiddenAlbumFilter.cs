using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class HiddenAlbumFilter : IAlbumFilter {
        public bool ShouldSkip(ILocation location) {
            return location.IsHidden();
        }
    }
}
