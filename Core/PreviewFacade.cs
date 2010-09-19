using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using AshMind.Web.Gallery.Core.ImageProcessing;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Metadata;

namespace AshMind.Web.Gallery.Core {
    public class PreviewFacade {
        private readonly ImageCache cache;
        private readonly IOrientationProvider[] orientationProviders;

        public PreviewFacade(
            ImageCache cache,
            IOrientationProvider[] orientationProviders
        ) {
            this.cache = cache;
            this.orientationProviders = orientationProviders;
        }

        public IFile GetPreview(IFile originalFile, int size) {
            return this.cache.GetTransform(
                originalFile, size,
                (image, desiredSize) => {
                    var orientation = this.orientationProviders
                                          .OrderByDescending(p => p.Priority)
                                          .Select(p => p.GetOrientation(image, originalFile))
                                          .Where(o => o != null)
                                          .FirstOrDefault();
                    image = ImageProcessor.ReduceSize(image, desiredSize);
                    if (orientation != null)
                        image = ImageProcessor.CorrectOrientation(image, orientation);

                    return image;
                }
            );
        }
        
        public string ImageMimeType {
            get { return this.cache.Format.MimeType; }
        }
    }
}
