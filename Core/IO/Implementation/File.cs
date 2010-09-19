using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.IO.Implementation {
    internal class File : IFile {
        private static IDictionary<FileLockMode, FileShare> fileShare = new Dictionary<FileLockMode, FileShare> {
            { FileLockMode.None,        FileShare.ReadWrite },
            { FileLockMode.Read,        FileShare.Write },
            { FileLockMode.Write,       FileShare.Read },
            { FileLockMode.ReadWrite,   FileShare.None },
        };

        private readonly string path;

        public File(string path) {
            this.path = path;
        }
        
        public bool IsHidden() {
            return System.IO.File.GetAttributes(this.path).HasFlag(FileAttributes.Hidden);
        }

        public void SetHidden(bool value) {
            if (!value)
                throw new NotImplementedException();

            System.IO.File.SetAttributes(
                this.path,
                System.IO.File.GetAttributes(this.path) | FileAttributes.Hidden
            );
        }

        public Stream Read(FileLockMode lockMode) {
            return System.IO.File.Open(this.path, FileMode.Open, FileAccess.Read, fileShare[lockMode]);
        }

        public Stream Open(FileLockMode lockMode, bool overwrite) {
            return System.IO.File.Open(
                this.path,
                overwrite ? FileMode.Create : FileMode.OpenOrCreate,
                overwrite ? FileAccess.Write : FileAccess.ReadWrite,
                fileShare[lockMode]
            );
        }

        public DateTimeOffset GetLastWriteTime() {
            return System.IO.File.GetLastWriteTimeUtc(this.path);
        }

        public string Name {
            get { return System.IO.Path.GetFileName(this.path); }
        }

        public string Path {
            get { return this.path; }
        }
    }
}
