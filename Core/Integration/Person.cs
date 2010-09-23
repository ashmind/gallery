using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.Integration {
    public class Person {
        public Person(string name, string email) {
            this.Name = name;
            this.Email = email;
        }

        public string Name  { get; private set; }
        public string Email { get; set; }
    }
}
