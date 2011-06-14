using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Imaging.Wpf {
    public class BitmapDecoderAdapter : IImageReader {
        public IImage Read(IFile file) {
            using (var stream = file.Read(FileLockMode.Write)) {
                var decoder = BitmapDecoder.Create(
                    stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.None
                );

                var bitmapMetadata = decoder.Frames[0].Metadata as BitmapMetadata;
                return new BitmapSourceAdapter(
                    decoder.Frames[0],
                    bitmapMetadata != null
                        ? new BitmapMetadataAdapter(bitmapMetadata)
                        : null
                );
            }
        }
    }
}
