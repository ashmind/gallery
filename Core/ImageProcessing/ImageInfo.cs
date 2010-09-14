using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.ImageProcessing {
    public class ImageInfo {
        public ImageInfo(string path, ImageMetadata metadata) {
            this.Path = path;
            this.Metadata = metadata;
        }

        public string Path { get; private set; }
        public ImageMetadata Metadata { get; private set; }
    }
}
