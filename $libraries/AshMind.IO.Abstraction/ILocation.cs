﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.IO.Abstraction {
    public interface ILocation : IFileSystemElement {
        IFile GetFile(string name, bool nullUnlessExists = true);

        ILocation GetLocation(string path, ActionIfMissing actionIfMissing = ActionIfMissing.ThrowException);
        IEnumerable<ILocation> GetLocations(bool recursive);

        IEnumerable<IFile> GetFiles();
    }
}
