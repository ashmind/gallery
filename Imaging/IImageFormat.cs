﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Imaging {
    public interface IImageFormat : IImageLoader {
        void Save(IImage image, IFile file);
        ReadOnlyCollection<string> MediaTypeNames { get; }
    }
}
