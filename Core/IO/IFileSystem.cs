using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.IO {
    public interface IFileSystem {
        bool IsFileName(string pathOrFileName);
        
        string BuildPath(params string[] parts);

        IFile GetFile(string path, bool nullUnlessExists = true);

        bool IsLocation(string path);
        ILocation GetLocation(string path);
    }
}
