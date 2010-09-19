using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Tests.IO.Resources {
    public class Resource : IFile {
        private string resourcePath;
        private Assembly assembly;

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

        public Stream Open(FileLockMode lockMode, bool overwrite) {
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
    }
}
