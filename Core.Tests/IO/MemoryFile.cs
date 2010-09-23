using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.Tests.IO {
    public class MemoryFile : IFile {
        private MemoryStream stream = new MemoryStream();
        
        public MemoryFile() {
            this.Exists = true;
        }

        public MemoryFile(MemoryLocation location, string name, bool exists) {
            this.Location = location;
            this.Name = name;
            this.Exists = exists;
        }
                
        public DateTimeOffset GetLastWriteTime() {
            throw new NotImplementedException();
        }

        private void ReopenStream() {
            var bytes = this.stream.ToArray();
            this.stream = new MemoryStream();
            this.stream.Write(bytes, 0, bytes.Length);
        }

        public Stream Read(FileLockMode lockMode) {
            if (!this.stream.CanRead) // disposed
                this.ReopenStream();

            this.stream.Seek(0, SeekOrigin.Begin);
            return this.stream;
        } 

        public Stream Open(FileLockMode lockMode, FileOpenMode openMode) {
            if (!this.stream.CanWrite) // disposed
                this.ReopenStream();

            if (openMode == FileOpenMode.Append) {
                this.stream.Seek(0, SeekOrigin.End);
                return this.stream;
            }

            if (openMode == FileOpenMode.ReadOrWrite) {
                this.stream.Seek(0, SeekOrigin.Begin);
                return this.stream;
            }

            if (openMode == FileOpenMode.Recreate) {
                this.stream = new MemoryStream();
                return this.stream;
            }

            throw new ArgumentOutOfRangeException("openMode");
        }

        public string Name { get; set; }

        public string Path {
            get { throw new NotImplementedException(); }
        }

        public bool Exists { get; set; }

        public bool IsHidden() {
            throw new NotImplementedException();
        }

        public void SetHidden(bool value) {
            throw new NotImplementedException();
        }

        public byte[] GetBytes() {
            return this.stream.ToArray();
        }

        public ILocation Location { get; set; }
    }
}
