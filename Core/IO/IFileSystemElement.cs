using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.IO {
    public interface IFileSystemElement {
        string Name { get; }
        string Path { get; }

        bool IsHidden();
        void SetHidden(bool value);
    }
}
