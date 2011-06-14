using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace AshMind.Gallery.Imaging.GdiPlus {
    public class ImageAdapter : IImage {
        private static readonly IDictionary<ImageOrientation, RotateFlipType> rotateFlip = new Dictionary<ImageOrientation, RotateFlipType> {
            { new ImageOrientation(0, ImageMirroring.None),   RotateFlipType.RotateNoneFlipNone },
            { new ImageOrientation(90, ImageMirroring.None),  RotateFlipType.Rotate90FlipNone },
            { new ImageOrientation(180, ImageMirroring.None), RotateFlipType.Rotate180FlipNone },
            { new ImageOrientation(270, ImageMirroring.None), RotateFlipType.Rotate270FlipNone }
        };

        public Image Image { get; private set; }
        public IImageMetadata Metadata { get; private set; }

        public ImageAdapter(Image image) {
            this.Image = image;
            this.Metadata = new ImageMetadataAdapter(image);
        }
        
        public Size Size {
            get { return new Size(this.Image.Size.Width, this.Image.Size.Height); }
        }

        public IImage Resize(Size targetSize) {
            var resized = new Bitmap(targetSize.Width, targetSize.Height, this.Image.PixelFormat);
            resized.SetResolution(this.Image.HorizontalResolution, this.Image.VerticalResolution);

            using (var g = Graphics.FromImage(resized)) {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(this.Image, 0, 0, targetSize.Width, targetSize.Height);
            }

            return new ImageAdapter(resized);
        }

        public void ReorientInPlace(ImageOrientation orientation) {
            if (orientation.Angle == 0 && orientation.Mirroring == ImageMirroring.None)
                return;

            this.Image.RotateFlip(rotateFlip[orientation]);
        }

        #region IDisposable Members

        public void Dispose() {
            this.Image.Dispose();
        }

        #endregion
    }
}
