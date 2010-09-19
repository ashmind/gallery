using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.ImageProcessing;
using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaIniDependencyProvider : ICacheDependencyProvider {
        private readonly PicasaIniFileFinder iniFileFinder;

        public PicasaIniDependencyProvider(PicasaIniFileFinder iniFileFinder) {
            this.iniFileFinder = iniFileFinder;
        }

        public IEnumerable<DateTimeOffset> GetRelatedChanges(IFile primary) {
            var picasaIni = this.iniFileFinder.FindIn(primary.Location);
            if (picasaIni == null)
                yield break;

            yield return picasaIni.GetLastWriteTime();
        }
    }
}
