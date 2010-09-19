using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core.Metadata;
using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaOrientationProvider : IOrientationProvider {
        private readonly PicasaIniLoader picasaIniLoader;

        public PicasaOrientationProvider(PicasaIniLoader picasaIniLoader) {
            this.picasaIniLoader = picasaIniLoader;
        }

        public ImageOrientation GetOrientation(Image image, IFile imageFile) {
            var picasaIni = this.picasaIniLoader.LoadFrom(imageFile.Location);
            if (picasaIni == null)
                return null;

            var item = picasaIni.Items.GetValueOrDefault(imageFile.Name);
            if (item == null || item.Rotate == null)
                return null;

            if (item.Rotate == 0)
                return new ImageOrientation(0);

            return new ImageOrientation((4 - item.Rotate.Value) * 90);
        }

        public int Priority {
            get { return 1000; }
        }
    }
}
