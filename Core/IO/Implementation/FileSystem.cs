using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AshMind.Web.Gallery.Core.IO.Implementation {
    internal class FileSystem : IFileSystem {
        public IFile GetFile(string path, bool nullUnlessExists = true) {
            return this.GetLocation(Path.GetDirectoryName(path))
                       .GetFile(Path.GetFileName(path), nullUnlessExists);
        }
       
        public string BuildPath(params string[] parts) {
            return Path.Combine(parts);
        }

        public bool IsFileName(string pathOrFileName) {
            return pathOrFileName.IndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) < 0;
        }

        public bool IsLocation(string path) {
            return Directory.Exists(path);
        }

        public ILocation GetLocation(string path) {
            return new Location(path);
        }
    }
}