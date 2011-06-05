using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AshMind.IO.Abstraction {
    public interface IFile : IFileSystemElement {
        DateTimeOffset GetLastWriteTime();

        Stream Read(FileLockMode lockMode);
        Stream Open(FileLockMode lockMode, FileOpenMode openMode);

        bool Exists { get; }

        ILocation Location { get; }
    }
}
