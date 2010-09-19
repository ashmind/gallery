using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.Integration.Picasa.IniParts;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaIni {
        public PicasaIni() {
            this.Contacts = new List<PicasaIniContact>();
            this.Items = new List<PicasaIniItem>();
        }

        public IList<PicasaIniContact> Contacts { get; private set; }
        public IList<PicasaIniItem> Items { get; private set; }
    }
}
