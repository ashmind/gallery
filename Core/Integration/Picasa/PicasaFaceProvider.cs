﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Integration.Picasa {
    using ContactKey = Tuple<string, string>;    

    public class PicasaFaceProvider : IFaceProvider {
        private readonly IDictionary<ContactKey, Person> contacts;
        private readonly PicasaIniFileFinder iniFileFinder;
        private readonly PicasaIniParser parser;

        public PicasaFaceProvider(
            PicasaDatabase database,
            PicasaIniFileFinder iniFileFinder,
            PicasaIniParser parser
        ) {
            this.contacts = LoadContacts(database.ContactsXml);
            this.iniFileFinder = iniFileFinder;
            this.parser = parser;
        }

        private IDictionary<ContactKey, Person> LoadContacts(IFile contactsFile) {
            if (contactsFile == null)
                return new Dictionary<ContactKey, Person>();

            var contactsContent = contactsFile.ReadAllText();
            var contactPairs = (
                from element in XDocument.Parse(contactsContent).Descendants("contact")
                from subject in element.Elements("subject")
                let key = new ContactKey(subject.Attribute("user").Value, subject.Attribute("id").Value)
                let person = new Person(
                    element.Attribute("name").Value,
                    element.Attributes("email0").Select(x => x.Value).SingleOrDefault()
                )
                select new { key, person }
            );

            return contactPairs.ToDictionary(
                x => x.key,
                x => x.person
            );
        }

        public IEnumerable<Face> GetFaces(ILocation location) {
            if (this.contacts.Count == 0) // contacts.xml not loaded for some reason
                yield break;

            var ini = this.iniFileFinder.FindIn(location);
            if (ini == null)
                yield break;

            var parsed = this.parser.Parse(ini);
            var rawFaces = from pair in parsed.Items
                           from face in pair.Value.Faces
                           select new { FileName = pair.Key, ContactHash = face.ContactHash };

            var contactsIndex = parsed.Contacts.ToDictionary(c => c.Hash);

            foreach (var rawFace in rawFaces) {
                var contact = contactsIndex.GetValueOrDefault(rawFace.ContactHash);
                if (contact == null)
                    continue;

                var file = location.GetFile(rawFace.FileName);
                if (file == null)
                    continue;

                var key = new ContactKey(contact.UserCode, contact.ID);

                yield return new Face(this.contacts[key], file);
            }
        }
    }
}