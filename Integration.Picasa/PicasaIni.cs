using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Integration.Picasa.IniParts;

namespace AshMind.Gallery.Integration.Picasa {
    public class PicasaIni {
        public PicasaIni() {
            this.Contacts = new List<PicasaIniContact>();
            this.Items = new Dictionary<string, PicasaIniMetadata>();
        }

        public IList<PicasaIniContact> Contacts { get; private set; }
        public IDictionary<string, PicasaIniMetadata> Items { get; private set; }
    }
}
