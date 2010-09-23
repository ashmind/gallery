using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.Tests.IO {
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

        public ILocation GetLocation(string path, ActionIfMissing actionIfMissing) {
            return new MemoryLocation { Path = path };
        }
    }
}
