using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Integration.Picasa {
    public class PicasaDatabase {
        public PicasaDatabase(IFile contactsXml) {
            this.ContactsXml = contactsXml;
        }

        public IFile ContactsXml { get; private set; }
    }
}
