using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AshMind.Web.Gallery.Core.ImageProcessing {
    public class ImageMetadata {
        public ImageMetadata(int width, int height) {
            this.Width = width;
            this.Height = height;
        }

        internal ImageMetadata(Size size) : this(size.Width, size.Height) {            
        }

        internal ImageMetadata(Size size, RotateFlipType? orientation)
            : this(size.Width, size.Height)
        {
            this.Orientation = orientation;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        internal RotateFlipType? Orientation { get; private set; }
    }
}
