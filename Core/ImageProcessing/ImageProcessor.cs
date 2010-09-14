using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace AshMind.Web.Gallery.Core.ImageProcessing {
    internal static class ImageProcessor {
        private const int ExifOrientationId = 274;

        private static HashSet<RotateFlipType> flipsWidthHeight = new HashSet<RotateFlipType> {
            RotateFlipType.Rotate90FlipNone,
            RotateFlipType.Rotate90FlipX,
            RotateFlipType.Rotate90FlipXY,
            RotateFlipType.Rotate90FlipY,
            RotateFlipType.Rotate270FlipNone,
            RotateFlipType.Rotate270FlipX,
            RotateFlipType.Rotate270FlipXY,
            RotateFlipType.Rotate270FlipY
        };

        private static IDictionary<int, RotateFlipType> ms_orientations = new Dictionary<int, RotateFlipType> {
            { 1, RotateFlipType.RotateNoneFlipNone },
            { 2, RotateFlipType.RotateNoneFlipX },
            { 3, RotateFlipType.RotateNoneFlipXY },
            { 4, RotateFlipType.RotateNoneFlipY },
            { 5, RotateFlipType.Rotate270FlipY },
            { 6, RotateFlipType.Rotate270FlipXY },
            { 7, RotateFlipType.Rotate270FlipX },
            { 8, RotateFlipType.Rotate270FlipNone },
        };

        public static Image ReduceSize(Image image, int desiredSize) {
            var orientation = image.GetOrientation();
            var targetSize = EstimateSize(image.Size, desiredSize);

            var resized = new Bitmap(targetSize.Width, targetSize.Height, image.PixelFormat);
            resized.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var g = Graphics.FromImage(resized)) {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, targetSize.Width, targetSize.Height);
            }

            ImageProcessor.CorrectOrientation(resized, orientation);

            return resized;
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

        private static void CorrectOrientation(Bitmap bitmap, RotateFlipType? orientation) {
            if (orientation == null || orientation.Value == RotateFlipType.RotateNoneFlipNone)
                return;

            bitmap.RotateFlip(orientation.Value);
        }

        public static RotateFlipType? GetOrientation(this Image image) {
            if (!image.PropertyIdList.Contains(ExifOrientationId))
                return null;

            var exifOrientation = image.GetPropertyItem(ExifOrientationId).Value[0];
            if (exifOrientation == 0)
                return null;

            return ms_orientations[exifOrientation];
        }
    }
}
