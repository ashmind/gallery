using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using AshMind.Gallery.Core.Metadata;
using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.ImageProcessing {
    public class ExifOrientationProvider : IOrientationProvider {
        private const int ExifOrientationId = 274;

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

        public ImageOrientation GetOrientation(Image image, IFile imageFile) {
            if (!image.PropertyIdList.Contains(ExifOrientationId))
                return null;

            var exifOrientation = image.GetPropertyItem(ExifOrientationId).Value[0];
            if (exifOrientation == 0)
                return null;

            return orientations[exifOrientation];
        }

        public int Priority {
            get { return 0; }
        }
    }
}
