using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AshMind.Web.Gallery.Core.IO.Implementation {
    internal class FileSystem : IFileSystem {
        public IEnumerable<ILocation> GetLocations(string root) {
            return Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories)
                            .Select(GetLocation);
        }

        public IFile GetFile(string path, bool nullUnlessExists = true) {
            if (nullUnlessExists && !System.IO.File.Exists(path))
                return null;

            return new File(path);
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
            return new Location(this, path);
        }
    }
}