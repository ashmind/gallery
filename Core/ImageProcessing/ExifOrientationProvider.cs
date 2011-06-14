using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Imaging;
using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.ImageProcessing {
    public class ExifOrientationProvider : IOrientationProvider {
        private static readonly IDictionary<int, ImageOrientation> orientations = new Dictionary<int, ImageOrientation> {
            { 1, new ImageOrientation(0) },
            { 2, new ImageOrientation(0, ImageMirroring.Horizontal) },
            { 3, new ImageOrientation(180) },
            { 4, new ImageOrientation(0, ImageMirroring.Vertical) },
            { 5, new ImageOrientation(90, ImageMirroring.Vertical) },
            { 6, new ImageOrientation(270) },
            { 7, new ImageOrientation(90, ImageMirroring.Horizontal) },
            { 8, new ImageOrientation(90) },
        };

        public ImageOrientation GetOrientation(IImage image, IFile imageFile) {
            if (image.Metadata == null)
                return null;

            var exifOrientation = image.Metadata.GetValue<byte>("exif/orientation");
            if (exifOrientation == 0)
                return null;

            return orientations[exifOrientation];
        }

        public int Priority {
            get { return 0; }
        }
    }
}
