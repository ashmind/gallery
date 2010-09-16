using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AshMind.Web.Gallery.Core.IO {
    internal class FileSystem : IFileSystem {
        private static IDictionary<FileLockMode, FileShare> fileShare = new Dictionary<FileLockMode, FileShare> {
            { FileLockMode.None,        FileShare.ReadWrite },
            { FileLockMode.Read,        FileShare.Write },
            { FileLockMode.Write,       FileShare.Read },
            { FileLockMode.ReadWrite,   FileShare.None },
        };

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

        public string GetLocation(string path) {
            return Path.GetDirectoryName(path);
        }
        
        public Stream ReadFile(string path, FileLockMode lockMode) {
            return File.Open(path, FileMode.Open, FileAccess.Read, fileShare[lockMode]);
        }
        
        public Stream OpenFile(string path, FileLockMode lockMode, bool overwrite) {
            return File.Open(
                path,
                overwrite ? FileMode.Create : FileMode.OpenOrCreate,
                overwrite ? FileAccess.Write : FileAccess.ReadWrite,
                fileShare[lockMode]
            );
        }
    }
}