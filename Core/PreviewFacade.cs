using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using AshMind.Web.Gallery.Core.ImageProcessing;
using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core {
    public class PreviewFacade {
        private readonly ImageCache cache;

        public PreviewFacade(ImageCache cache) {
            this.cache = cache;
        }

        public string GetPreviewPath(string originalPath, int size) {
            return this.cache.GetTransformPath(originalPath, size, ImageProcessor.ReduceSize);
        }

        public ImageMetadata GetPreviewMetadata(IFile originalFile, int size) {
            return this.cache.GetMetadata(
                originalFile,
                () => {
                    using (var stream = originalFile.Read(FileLockMode.Write)) {
                        return ImageMetadataExtractor.ReadMetadata(stream);
                    }
                },
                metadata => new ImageMetadata(ImageProcessor.EstimateSize(metadata, size))
            );
        }
        
        public string ImageMimeType {
            get { return this.cache.Format.MimeType; }
        }
    }
}
