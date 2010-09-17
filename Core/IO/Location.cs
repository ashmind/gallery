using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AshMind.Web.Gallery.Core.IO {
    public class Location : ILocation {
        private readonly string path;

        public Location(string path) {
            this.path = path;
        }

        public IEnumerable<IFile> GetFiles() {
            return Directory.EnumerateFiles(this.path)
                            .Select(name => new File(name));
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
