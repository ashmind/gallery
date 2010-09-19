using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.ImageProcessing;
using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaIniDependencyProvider : ICacheDependencyProvider {
        public IEnumerable<DateTimeOffset> GetRelatedChanges(IFile primary) {
            // TODO: picasa.ini (old file name)
            var picasaIni = primary.Location.GetFile(".picasa.ini");
            if (picasaIni == null)
                yield break;

            yield return picasaIni.GetLastWriteTime();
        }
    }
}
