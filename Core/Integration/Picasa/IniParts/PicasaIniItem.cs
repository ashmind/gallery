using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Integration.Picasa.IniParts {
    public class PicasaIniItem {
        public PicasaIniItem(string fileName) {
            this.FileName = fileName;
            this.Faces = new List<PicasaIniFace>();
        }

        public string FileName { get; private set; }
        public bool Starred { get; set; }
        public IList<PicasaIniFace> Faces { get; private set; }
    }
}
