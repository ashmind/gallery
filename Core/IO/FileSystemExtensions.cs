using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.IO {
    public static class FileSystemExtensions {
        public static string ReadAllText(this IFileSystem fileSystem, string path) {
            using (var stream = fileSystem.ReadFile(path, FileLockMode.Write))
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }
    }
}
