using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.IO.Abstraction {
    public interface IFileSystem {
        bool IsFileName(string pathOrFileName);
        
        string BuildPath(params string[] parts);

        IFile GetFile(string path, ActionIfMissing actionIfMissing = ActionIfMissing.ReturnNull);

        bool IsLocation(string path);
        ILocation GetLocation(string path, ActionIfMissing actionIfMissing = ActionIfMissing.ThrowException);
    }
}
