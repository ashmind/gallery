using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;

using AshMind.Extensions;
using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Imaging.Wpf {
    public class BitmapEncoderAdapter : IImageWriter {
        private readonly BitmapCodecInfo codec;
        private readonly Func<BitmapEncoder> encoderFactory;

        public BitmapEncoderAdapter(Func<BitmapEncoder> encoderFactory) {
            this.codec = encoderFactory().CodecInfo;
            this.encoderFactory = encoderFactory;

            this.FileExtensions = this.codec.FileExtensions.Split(',')
                                            .Select(e => e.TrimStart('.'))
                                            .ToArray()
                                            .AsReadOnly();

            this.MediaTypeNames = this.codec.MimeTypes.Split(',').AsReadOnly();
        }

        public void Write(IFile file, IImage image) {
            var bitmapSource = ((BitmapSourceAdapter)image).Source;
            var encoder = this.encoderFactory();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var stream = file.Open(FileLockMode.ReadWrite, FileOpenMode.ReadOrWrite)) {
                encoder.Save(stream);
            }
        }

        public ReadOnlyCollection<string> FileExtensions { get; private set; }
        public ReadOnlyCollection<string> MediaTypeNames { get; private set; }
    }
}
