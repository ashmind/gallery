using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Media.Imaging;

namespace AshMind.Gallery.Imaging.Wpf {
    public class BitmapMetadataAdapter : IImageMetadata {
        public BitmapMetadata Metadata { get; private set; }

        public BitmapMetadataAdapter(BitmapMetadata metadata) {
            this.Metadata = metadata;
        }

        public T GetValue<T>(string key) {
            if (!this.Metadata.ContainsQuery(key))
                return default(T);

            return (T)this.Metadata.GetQuery(key);
        }
    }
}
