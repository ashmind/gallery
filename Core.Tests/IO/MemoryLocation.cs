using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.Tests.IO {
    public class MemoryLocation : ILocation, IEnumerable {
        private readonly IDictionary<string, IFile> files = new Dictionary<string, IFile>();

        public MemoryLocation() {
            this.Name = string.Empty;
            this.Path = string.Empty;
        }

        public void Add(string fileName, IFile file) {
            this.files.Add(fileName, file);
        }

        public void Add(IFile file) {
            this.files.Add(file.Name, file);
        }

        public IFile GetFile(string name, ActionIfMissing actionIfMissing) {
            var file = this.files.GetValueOrDefault(name);
            if (actionIfMissing != ActionIfMissing.ReturnNull)
                file = file ?? new MemoryFile(this, name, false);

            return file;
        }

        public ILocation GetLocation(string path, ActionIfMissing actionIfMissing = ActionIfMissing.ReturnNull) {
            return new MemoryLocation { Path = this.Path + "/" + path };
        }

        public IEnumerable<IFile> GetFiles() {
            return this.files.Values;
        }

        public string Name { get; set; }
        public string Path { get; set; }

        public bool IsHidden() {
            throw new NotImplementedException();
        }

        public void SetHidden(bool value) {
            throw new NotImplementedException();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return this.files.GetEnumerator();
        }
        
        public IEnumerable<ILocation> GetLocations(bool recursive) {
            throw new NotImplementedException();
        }
    }
}
