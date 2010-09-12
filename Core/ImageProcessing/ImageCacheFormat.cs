using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;

namespace AshMind.Web.Gallery.Core.ImageProcessing {
    internal class ImageCacheFormat {
        public string MimeType      { get; private set; }
        public string FileExtension { get; private set; }
        
        public static ImageCacheFormat Jpeg { get; private set; }
        public static ImageCacheFormat Png  { get; private set; }

        private ImageCacheFormat() {
        }

        static ImageCacheFormat() {
            Jpeg = new ImageCacheFormat {
                MimeType = MediaTypeNames.Image.Jpeg,
                FileExtension = "jpg"
            };

            Png = new ImageCacheFormat {
                MimeType = "image/png",
                FileExtension = "png"
            };
        }
    }
}
