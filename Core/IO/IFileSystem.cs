using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.IO {
    public interface IFileSystem {
        IEnumerable<Location> GetLocations(string root);        

        bool IsFileName(string pathOrFileName);
        
        string BuildPath(params string[] parts);

        File GetFile(string path, bool nullUnlessExists = true);

        bool IsLocation(string path);
        Location GetLocation(string path);
    }
}
