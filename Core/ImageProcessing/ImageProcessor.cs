using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Drawing.Imaging;
using System.Collections.Generic;

using AshMind.Web.Gallery.Core.Metadata;

namespace AshMind.Web.Gallery.Core.ImageProcessing {
    using OrientationTuple = Tuple<int, ImageMirroring>;

    internal static class ImageProcessor {
        private const int ExifOrientationId = 274;

        private static HashSet<RotateFlipType> flipsWidthHeight = new HashSet<RotateFlipType> {
            RotateFlipType.Rotate90FlipNone,
            RotateFlipType.Rotate90FlipX,
            RotateFlipType.Rotate90FlipY,
            RotateFlipType.Rotate270FlipNone,
            RotateFlipType.Rotate270FlipX,
            RotateFlipType.Rotate270FlipY
        };

        private static IDictionary<OrientationTuple, RotateFlipType> rotateFlip = new Dictionary<OrientationTuple, RotateFlipType> {
            { new OrientationTuple(0, ImageMirroring.None),   RotateFlipType.RotateNoneFlipNone },
            { new OrientationTuple(90, ImageMirroring.None),  RotateFlipType.Rotate270FlipNone },
            { new OrientationTuple(180, ImageMirroring.None), RotateFlipType.Rotate180FlipNone },
            { new OrientationTuple(270, ImageMirroring.None), RotateFlipType.Rotate90FlipNone }
        };

        public static Image ReduceSize(Image image, int desiredSize) {
            var targetSize = EstimateSize(image.Size, desiredSize);

            var resized = new Bitmap(targetSize.Width, targetSize.Height, image.PixelFormat);
            resized.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var g = Graphics.FromImage(resized)) {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, targetSize.Width, targetSize.Height);
            }            

            return resized;
        }

        public static Image CorrectOrientation(Image image, ImageOrientation orientation) {
            if (orientation.Angle == 0 && orientation.Mirroring == ImageMirroring.None)
                return image;
            
            image.RotateFlip(rotateFlip[new OrientationTuple(orientation.Angle, orientation.Mirroring)]);
            return image;
        }

        public static Size EstimateSize(ImageMetadata metadata, int desiredSize) {
            var size = EstimateSize(new Size(metadata.Width, metadata.Height), desiredSize);
            if (metadata.Orientation != null && flipsWidthHeight.Contains(metadata.Orientation.Value))
                size = new Size(size.Height, size.Width);

            return size;
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
