using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using AshMind.Web.Gallery.Core.ImageProcessing;

namespace AshMind.Web.Gallery.Core {
    public class PreviewFacade {
        private readonly ImageCache cache;

        public PreviewFacade(ImageCache cache) {
            this.cache = cache;
        }

        public string GetPreviewPath(string originalPath, int size) {
            return this.cache.GetResultPath(originalPath, size, ImageProcessor.ToThumbnail);
        }
        
        public string ImageMimeType {
            get { return this.cache.Format.MimeType; }
        }
    }
}
