using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Imaging.GdiPlus {
    public class ImageLoader : IImageLoader {
        public ImageLoader(IEnumerable<ImageCodecAdapter> codecs) {
            this.FileExtensions = codecs.SelectMany(c => c.FileExtensions).ToList().AsReadOnly();
        }

        public IImage Load(IFile file) {
            //using (var stream = file.Read(FileLockMode.Write)) {
            //    return new ImageAdapter(Image.FromStream(stream));
            //}

            // TEMPHACK: for some reason above code does not load exif
            return new ImageAdapter(Image.FromFile(file.Path));
        }

        public ReadOnlyCollection<string> FileExtensions { get; private set; }
    }
}
