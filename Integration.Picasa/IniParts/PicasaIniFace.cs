using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Integration.Picasa.IniParts {
    public class PicasaIniFace {
        public PicasaIniFace(string contactHash) {
            this.ContactHash = contactHash;
        }

        public string ContactHash { get; private set; }
    }
}
