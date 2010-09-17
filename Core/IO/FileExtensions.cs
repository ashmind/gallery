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
    }
}
