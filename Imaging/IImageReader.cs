using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Imaging {
    public interface IImageReader {
        IImage Read(IFile file);
    }
}
