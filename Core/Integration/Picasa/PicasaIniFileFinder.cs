using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaIniFileFinder {
        private static readonly string[] fileNames = { ".picasa.ini", "picasa.ini" };

        public IFile FindIn(ILocation location) {
            return fileNames
                       .Select(name => location.GetFile(name))
                       .FirstOrDefault(f => f != null);
        }
    }
}
