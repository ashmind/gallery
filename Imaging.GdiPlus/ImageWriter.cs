using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Linq;
using AshMind.Extensions;
using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Imaging.GdiPlus {
    public class ImageWriter : IImageWriter {
        private readonly ImageCodecInfo codec;
        private readonly EncoderParameters parameters;

        public ImageWriter(ImageCodecInfo codec) {
            this.codec = codec;
            this.parameters = new EncoderParameters {
                Param = new[] { new EncoderParameter(Encoder.Quality, 100L) }
            };

            this.FileExtensions = codec.FilenameExtension.Split(';')
                                                         .Select(e => e.TrimStart('*', '.').ToLowerInvariant())
                                                         .ToArray()
                                                         .AsReadOnly();
            this.MediaTypeNames = new[] { codec.MimeType }.AsReadOnly();
        }

        public void Write(IFile file, IImage image) {
            var actual = ((ImageAdapter)image).Image;
            using (var stream = file.Open(FileLockMode.ReadWrite, FileOpenMode.Recreate)) {
                actual.Save(stream, this.codec, this.parameters);
            }
        }

        public ReadOnlyCollection<string> FileExtensions { get; private set; }
        public ReadOnlyCollection<string> MediaTypeNames { get; private set; }
    }
}
