﻿using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Values;
using AshMind.Gallery.Integration;
using AshMind.Gallery.Integration.Faces;

namespace AshMind.Gallery.Core.Albums {
    public class PersonAlbum : Album, IReadOnlySupport<PersonAlbum> {
        public PersonAlbum(AlbumDescriptor descriptor, string name, Person person, IValue<IEnumerable<AlbumItem>> items) : base(descriptor, name, items) {
            this.Person = person;
        }

        public Person Person { get; private set; }

        public bool IsOf(KnownUser realUser) {
            return this.Person.Emails.Contains(realUser.Email);
        }

        protected override Album Recreate(IValue<IList<AlbumItem>> items) {
            return new PersonAlbum(this.Descriptor, this.Name, this.Person, items);
        }

        public new PersonAlbum AsWritable() {
            return (PersonAlbum)base.AsWritable();
        }
    }
}
