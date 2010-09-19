using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AshMind.Web.Gallery.Core.IO.Implementation {
    internal class Location : ILocation {
        private readonly FileSystem fileSystem;
        private readonly string path;

        public Location(FileSystem fileSystem, string path) {
            this.fileSystem = fileSystem;
            this.path = path;            
        }

        public IEnumerable<IFile> GetFiles() {
            return Directory.EnumerateFiles(this.path)
                            .Select(name => new File(name));
        }

        public IFile GetFile(string name, bool nullUnlessExists = true) {
            var path = System.IO.Path.Combine(this.Path, name);
            return this.fileSystem.GetFile(name, nullUnlessExists);
        }

        public bool IsHidden() {
            return new File(this.path).IsHidden();
        }

        public void SetHidden(bool value) {
            new File(this.path).SetHidden(value);
        }

        public string Name {
            get { return System.IO.Path.GetFileName(this.path); }
        }

        public string Path {
            get { return this.path; }
        }
    }
}
