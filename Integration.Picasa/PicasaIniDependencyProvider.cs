using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.IO.Abstraction;

using AshMind.Gallery.Core.ImageProcessing;

namespace AshMind.Gallery.Integration.Picasa {
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
