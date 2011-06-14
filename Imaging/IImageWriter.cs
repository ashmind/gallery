using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Imaging {
    public interface IImageWriter {
        void Write(IFile file, IImage image);

        ReadOnlyCollection<string> FileExtensions { get; }
        ReadOnlyCollection<string> MediaTypeNames { get; }
    }
}
