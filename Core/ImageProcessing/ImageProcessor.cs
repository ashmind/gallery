using System;
using System.Linq;
using System.Collections.Generic;
using AshMind.Gallery.Imaging;

namespace AshMind.Gallery.Core.ImageProcessing {
    using OrientationTuple = Tuple<int, ImageMirroring>;

    internal static class ImageProcessor {
        public static IImage ReduceSize(IImage image, int desiredSize) {
            var targetSize = EstimateSize(image.Size, desiredSize);
            return image.Resize(targetSize);
        }

        public static IImage CorrectOrientation(IImage image, ImageOrientation orientation) {
            if (orientation.Angle == 0 && orientation.Mirroring == ImageMirroring.None)
                return image;

            image.ReorientInPlace(orientation.Invert());
            return image;
        }

        private static Size EstimateSize(Size size, int desiredSize) {
            var ratio = ((double)desiredSize / Math.Max(size.Width, size.Height));
            return new Size(
                (int)(size.Width * ratio),
                (int)(size.Height * ratio)
            );
        }
    }
}
