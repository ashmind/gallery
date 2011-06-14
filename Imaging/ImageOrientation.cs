using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Imaging {
    public class ImageOrientation {
        public ImageOrientation(int angle) : this(angle, ImageMirroring.None) {
        }

        public ImageOrientation(int angle, ImageMirroring mirroring) {
            this.Angle = angle;
            this.Mirroring = mirroring;
        }

        public int Angle { get; private set; }
        public ImageMirroring Mirroring { get; private set; }

        public ImageOrientation Invert() {
            if (this.Mirroring != ImageMirroring.None)
                throw new NotImplementedException();

            return new ImageOrientation(360 - this.Angle, this.Mirroring);
        }

        public bool Equals(ImageOrientation other) {
            if (other == null)
                return false;
            
            if (other == this)
                return true;

            return other.Angle == this.Angle
                && other.Mirroring == this.Mirroring;
        }

        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(ImageOrientation))
                return false;

            return this.Equals((ImageOrientation)obj);
        }

        public override int GetHashCode() {
            return this.Angle.GetHashCode() ^ this.Mirroring.GetHashCode();
        }
    }
}
