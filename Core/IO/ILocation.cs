using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.IO {
    public interface ILocation : IFileSystemElement {
        IFile GetFile(string name, bool nullUnlessExists = true);
        IEnumerable<IFile> GetFiles();
    }
}
