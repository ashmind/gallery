using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AshMind.Web.Gallery.Core.IO {
    internal class FileSystem : IFileSystem {
        public IEnumerable<string> GetLocations(string root) {
            return Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
        }

        public DateTimeOffset GetCreationTime(string directory) {
            return Directory.GetCreationTimeUtc(directory);
        }

        public bool FileExists(string file) {
            return File.Exists(file);
        }

        public IEnumerable<string> GetFileNames(string location) {
            return Directory.GetFiles(location);
        }

        public string GetFileName(string path) {
            return Path.GetFileName(path);
        }

        public string GetLocationName(string location) {
            return Path.GetFileName(location);
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

        public string ReadAllText(string path) {
            return File.ReadAllText(path);
        }
    }
}