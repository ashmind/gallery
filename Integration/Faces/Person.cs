using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AshMind.Gallery.Integration.Faces {
    public class Person {
        public Person(string name, IEnumerable<string> emails) {
            this.Name = name;
            this.Emails = emails.ToList().AsReadOnly();
        }

        public string Name  { get; private set; }
        public ReadOnlyCollection<string> Emails { get; private set; }
    }
}
