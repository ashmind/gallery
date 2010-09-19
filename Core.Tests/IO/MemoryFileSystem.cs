using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core.Tests.IO {
    public class MemoryFileSystem : IFileSystem {
        public IEnumerable<ILocation> GetLocations(string root) {
            throw new NotImplementedException();
        }

        public bool IsFileName(string pathOrFileName) {
            throw new NotImplementedException();
        }

        public string BuildPath(params string[] parts) {
            throw new NotImplementedException();
        }

        public IFile GetFile(string path, bool nullUnlessExists = true) {
            throw new NotImplementedException();
        }

        public bool IsLocation(string path) {
            throw new NotImplementedException();
        }

        public ILocation GetLocation(string path) {
            return new MemoryLocation { Path = path };
        }
    }
}
