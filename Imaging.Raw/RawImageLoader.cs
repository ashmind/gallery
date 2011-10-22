using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using LibRawNet;

using AshMind.Extensions;

using AshMind.Gallery.Imaging.GdiPlus;
using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Imaging.Raw {
    public class RawImageLoader : IImageLoader {
        private static readonly ReadOnlyCollection<string> allFileExtensions = new[] { "raw", "arw", "dng" }.AsReadOnly();

        public IImage Load(IFile file) {
            using (var raw = RawImage.FromFile(file.Path)) {
                return new ImageAdapter(raw.ToBitmap());
            }
        }

        public ReadOnlyCollection<string> FileExtensions {
            get { return allFileExtensions; }
        }
    }
}
