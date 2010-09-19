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
        private readonly PicasaIniFileFinder picasaIniFileFinder;
        private readonly PicasaIniParser picasaIniParser;

        public PicasaOrientationProvider(PicasaIniFileFinder picasaIniFileFinder, PicasaIniParser picasaIniParser) {
            this.picasaIniFileFinder = picasaIniFileFinder;
            this.picasaIniParser = picasaIniParser;
        }

        public ImageOrientation GetOrientation(Image image, IFile imageFile) {
            var picasaIniFile = this.picasaIniFileFinder.FindIn(imageFile.Location);
            if (picasaIniFile == null)
                return null;

            var picasaIni = this.picasaIniParser.Parse(picasaIniFile);
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
