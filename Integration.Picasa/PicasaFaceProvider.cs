using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using AshMind.Extensions;
using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Integration.Picasa {
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
                    from attribute in element.Attributes()
                    where attribute.Name.LocalName.StartsWith("email")
                    select attribute.Value
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
                           select new { FileName = pair.Key, face.ContactHash };

            var contactsIndex = parsed.Contacts.ToDictionary(c => c.Hash);

            foreach (var rawFace in rawFaces) {
                var contact = contactsIndex.GetValueOrDefault(rawFace.ContactHash);
                if (contact == null)
                    continue;

                var file = location.GetFile(rawFace.FileName);
                if (file == null)
                    continue;

                var key = new ContactKey(contact.UserCode, contact.ID);
                var person = this.contacts.GetValueOrDefault(key);
                if (person == null)
                    continue;

                yield return new Face(person, file);
            }
        }
    }
}
