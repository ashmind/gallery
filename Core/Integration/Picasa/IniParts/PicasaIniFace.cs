using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Integration.Picasa.IniParts {
    public class PicasaIniFace {
        public PicasaIniFace(string contactHash) {
            this.ContactHash = contactHash;
        }

        public string ContactHash { get; private set; }
    }
}
