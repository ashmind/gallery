using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AshMind.Gallery.Imaging.GdiPlus {
    public class ImageMetadataAdapter : IImageMetadata {
        private static readonly IDictionary<string, int> KeyMapping = new Dictionary<string,int> {
            { "exif/orientation", 274 }
        };
        private static readonly IDictionary<Type, Delegate> Converters = new Dictionary<Type, Delegate>();

        private readonly Image image;

        static ImageMetadataAdapter() {
            AddConverter(bytes => bytes[0]);
        }

        public ImageMetadataAdapter(Image image) {
            this.image = image;
        }

        public T GetValue<T>(string key) {
            var id = KeyMapping[key];
            if (!this.image.PropertyIdList.Contains(id))
                return default(T);

            var value = this.image.GetPropertyItem(id).Value;
            return ((Func<byte[], T>)Converters[typeof(T)]).Invoke(value);
        }

        private static void AddConverter<T>(Func<byte[], T> func) {
            Converters.Add(typeof(T), func);
        }
    }
}