using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    public class PicasaDatabase {
        public PicasaDatabase(IFile contactsXml) {
            this.ContactsXml = contactsXml;
        }

        public IFile ContactsXml { get; private set; }
    }
}
