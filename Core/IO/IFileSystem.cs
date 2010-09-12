﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.IO
{
    public interface IFileSystem
    {
        IEnumerable<string> GetLocations(string root);
        IEnumerable<string> GetFileNames(string location);

        bool IsFileName(string pathOrFileName);
        string GetFileName(string path);
        string BuildPath(params string[] parts);

        DateTimeOffset GetCreationTime(string location);

        bool FileExists(string path);        
    }
}
