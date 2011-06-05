using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.Tests.IO {
    public class Resource : IFile {
        private readonly string resourcePath;
        private readonly Assembly assembly;

        public Resource(string resourcePath) {
            this.resourcePath = resourcePath;
            this.assembly = Assembly.GetExecutingAssembly();
        }

        public DateTimeOffset GetLastWriteTime() {
            throw new NotImplementedException();
        }

        public Stream Read(FileLockMode lockMode) {
            return this.assembly.GetManifestResourceStream(this.resourcePath);
        }

        public Stream Open(FileLockMode lockMode, FileOpenMode openMode) {
            throw new NotImplementedException();
        }
        
        public string Name {
            get { throw new NotImplementedException(); }
        }

        public string Path {
            get { return this.resourcePath; }
        }

        public bool IsHidden() {
            throw new NotImplementedException();
        }

        public void SetHidden(bool value) {
            throw new NotImplementedException();
        }

        public bool Exists {
            get { return true; }
        }

        public ILocation Location {
            get { throw new NotImplementedException(); }
        }
    }
}
