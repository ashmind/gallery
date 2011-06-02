using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.Integration.Picasa {
    public class PicasaDatabase {
        public PicasaDatabase(IFile contactsXml) {
            this.ContactsXml = contactsXml;
        }

        public IFile ContactsXml { get; private set; }
    }
}
