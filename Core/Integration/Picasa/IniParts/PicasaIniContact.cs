using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Integration.Picasa.IniParts {
    public class PicasaIniContact {
        public PicasaIniContact(string hash, string userCode, string id) {
            this.Hash = hash;
            this.UserCode = userCode;
            this.ID = id;
        }
                
        public string Hash { get; private set; }
        public string UserCode { get; private set; }
        public string ID { get; private set; }
    }
}
