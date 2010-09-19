using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Integration.Picasa.IniParts {
    public class PicasaIniMetadata {
        public PicasaIniMetadata() {
            this.Faces = new List<PicasaIniFace>();
        }

        public int? Rotate { get; set; }
        public bool Starred { get; set; }
        public IList<PicasaIniFace> Faces { get; private set; }
    }
}
