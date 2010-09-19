using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Metadata {
    public class ImageOrientation {
        public ImageOrientation(int angle) : this(angle, ImageMirroring.None) {
        }

        public ImageOrientation(int angle, ImageMirroring mirroring) {
            this.Angle = angle;
            this.Mirroring = mirroring;
        }

        public int Angle { get; private set; }
        public ImageMirroring Mirroring { get; private set; }
    }
}
