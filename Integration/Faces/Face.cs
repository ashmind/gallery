using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Integration.Faces {
    public class Face {
        public Face(Person person, IFile file) {
            Argument.VerifyNotNull("person", person);
            Argument.VerifyNotNull("file", file);

            this.Person = person;
            this.File = file;
        }

        public Person Person { get; private set; }
        public IFile File    { get; private set; }
    }
}
