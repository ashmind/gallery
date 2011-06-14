using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AshMind.Gallery.Imaging.Wpf {
    public class BitmapSourceAdapter : IImage {
        public BitmapSource Source { get; private set; }
        public BitmapMetadataAdapter Metadata { get; private set; }

        public BitmapSourceAdapter(BitmapSource source, BitmapMetadataAdapter metadata) {
            this.Source = source;
            this.Metadata = metadata;
        }
        
        public Size Size {
            get { return new Size(this.Source.PixelWidth, this.Source.PixelHeight); }
        }

        public IImage Resize(Size targetSize) {
            //var widthScale = (double)targetSize.Width / this.Source.PixelWidth;
            //var heightScale = (double)targetSize.Width / this.Source.PixelWidth;

            //return new BitmapSourceAdapter(new TransformedBitmap(
            //    this.Source, new ScaleTransform(widthScale, heightScale)
            //), this.Metadata);
            return this;
        }

        public void ReorientInPlace(ImageOrientation orientation) {
            if (orientation.Mirroring != ImageMirroring.None)
                throw new NotImplementedException();

            this.Source = new TransformedBitmap(
                this.Source, new RotateTransform(orientation.Angle)
            );
        }

        #region IDisposable Members

        public void Dispose() {
            var disposable = this.Source as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        #endregion

        #region IImage Members
        
        IImageMetadata IImage.Metadata {
            get { return this.Metadata; }
        }

        #endregion
    }
}
