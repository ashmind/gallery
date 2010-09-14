using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaAlbumFilter : IAlbumFilter {
        private readonly IFileSystem fileSystem;

        public PicasaAlbumFilter(IFileSystem fileSystem) {
            this.fileSystem = fileSystem;
        }

        public bool ShouldSkip(string location) {
            var name = this.fileSystem.GetLocationName(location);
            return name == ".picasaoriginals";
        }
    }
}
