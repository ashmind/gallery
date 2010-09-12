using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace AshMind.Web.Gallery.Core.ImageProcessing {
    internal static class ImageProcessor {
        private const int ExifOrientationId = 274;

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

        public static Image ToThumbnail(Image image, int desiredSize) {
            var orientation = image.GetOrientation();
            var ratio = ((double)desiredSize / image.Width);

            var targetWidth = desiredSize;
            var targetHeight = (int)(image.Height * ratio);

            var resized = new Bitmap(targetWidth, targetHeight, image.PixelFormat);
            resized.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, targetWidth, targetHeight);
            }

            ImageProcessor.CorrectOrientation(resized, orientation);

            return resized;
        }

        private static void CorrectOrientation(Bitmap bitmap, RotateFlipType? orientation)
        {
            if (orientation == null || orientation.Value == RotateFlipType.RotateNoneFlipNone)
                return;

            bitmap.RotateFlip(orientation.Value);
        }

        public static RotateFlipType? GetOrientation(this Image image)
        {
            if (!image.PropertyIdList.Contains(ExifOrientationId))
                return null;

            var exifOrientation = image.GetPropertyItem(ExifOrientationId).Value[0];
            if (exifOrientation == 0)
                return null;

            return ms_orientations[exifOrientation];
        }
    }
}
