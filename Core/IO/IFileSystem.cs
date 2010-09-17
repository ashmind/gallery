using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.IO {
    public interface IFileSystem {
        IEnumerable<string> GetLocations(string root);
        IEnumerable<string> GetFileNames(string location);

        bool IsFileName(string pathOrFileName);

        string GetFileName(string path);
        string GetLocationName(string location);

        string BuildPath(params string[] parts);

        DateTimeOffset GetLastWriteTime(string path);

        bool FileExists(string path);

        bool IsLocation(string path);
        string GetLocation(string path);

        Stream ReadFile(string path, FileLockMode lockMode);
        Stream OpenFile(string path, FileLockMode lockMode, bool overwrite);
    }
}
