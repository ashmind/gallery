using System;
using System.Linq;
using System.Collections.Generic;
using AshMind.Gallery.Imaging;

namespace AshMind.Gallery.Core.ImageProcessing {
    using OrientationTuple = Tuple<int, ImageMirroring>;

    internal static class ImageProcessor {
        //private static readonly IDictionary<OrientationTuple, RotateFlipType> rotateFlip = new Dictionary<OrientationTuple, RotateFlipType> {
        //    { new OrientationTuple(0, ImageMirroring.None),   RotateFlipType.RotateNoneFlipNone },
        //    { new OrientationTuple(90, ImageMirroring.None),  RotateFlipType.Rotate270FlipNone },
        //    { new OrientationTuple(180, ImageMirroring.None), RotateFlipType.Rotate180FlipNone },
        //    { new OrientationTuple(270, ImageMirroring.None), RotateFlipType.Rotate90FlipNone }
        //};

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
            var ratio = ((double)desiredSize / size.Width);
            return new Size(
                desiredSize,
                (int)(size.Height * ratio)
            );
        }
    }
}
