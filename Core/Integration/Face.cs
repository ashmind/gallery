using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration {
    public class Face {
        public Face(Person person, AlbumItem item) {
            this.Person = person;
            this.Item = item;
        }

        public Person Person    { get; private set; }
        public AlbumItem Item   { get; private set; }
    }
}
