using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaDatabase {
        public PicasaDatabase(ILocation localAppData) {
            var root = localAppData.GetLocation("Google").GetLocation("Picasa2");
            this.ContactsXml = root.GetLocation("contacts").GetFile("contacts.xml");
        }

        public IFile ContactsXml { get; private set; }
    }
}
