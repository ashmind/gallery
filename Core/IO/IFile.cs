using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AshMind.Web.Gallery.Core.IO {
    public interface IFile : IFileSystemElement {
        DateTimeOffset GetLastWriteTime();

        Stream Read(FileLockMode lockMode);
        Stream Open(FileLockMode lockMode, FileOpenMode openMode);

        bool Exists { get; }

        ILocation Location { get; }
    }
}
