using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaFaceProvider : IFaceProvider {
        private readonly IFile contactsXml;
        private readonly PicasaIniParser parser;

        public PicasaFaceProvider(
            IFile contactsXml,
            PicasaIniParser parser
        ) {
            this.contactsXml = contactsXml;
            this.parser = parser;
        }

        public IEnumerable<Face> GetFaces(Album album) {
            throw new NotImplementedException();
        }
    }
}
