using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.IO {
    public static class FileExtensions {
        public static string ReadAllText(this IFile file) {
            using (var stream = file.Read(FileLockMode.Write))
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }

        public static IEnumerable<string> ReadAllLines(this IFile file) {
            using (var stream = file.Read(FileLockMode.Write))
            using (var reader = new StreamReader(stream)) {
                var line = reader.ReadLine();
                while (line != null) {
                    yield return line;
                    line = reader.ReadLine();
                }
            }
        }
    }
}
