using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaFaceProvider : IFaceProvider {
        private readonly IFile contactsXml;
        private readonly PicasaIniLoader loader;

        public PicasaFaceProvider(
            IFile contactsXml,
            PicasaIniLoader loader
        ) {
            this.contactsXml = contactsXml;
            this.loader = loader;
        }

        public IEnumerable<Face> GetFaces(Album album) {
            var picasaIni = this.loader.LoadFrom(album.Location);
            if (picasaIni == null)
                yield break;
        }
    }
}
