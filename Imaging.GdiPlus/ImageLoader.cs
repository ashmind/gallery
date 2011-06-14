using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Imaging.GdiPlus {
    public class ImageLoader : IImageLoader {
        public IImage Load(IFile file) {
            //using (var stream = file.Read(FileLockMode.Write)) {
            //    return new ImageAdapter(Image.FromStream(stream));
            //}

            // TEMPHACK: for some reason above code does not load exif
            return new ImageAdapter(Image.FromFile(file.Path));
        }
    }
}
