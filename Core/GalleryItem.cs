using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.ImageProcessing;

namespace AshMind.Web.Gallery.Core {
    public class GalleryItem {
        public GalleryItem(string name, GalleryItemType type, DateTimeOffset date, Func<int, ImageMetadata> getMetadata) {
            this.Name = name;
            this.Type = type;
            this.Date = date;
            this.GetMetadata = getMetadata;
        }

        public string Name { get; private set; }
        public GalleryItemType Type { get; private set; }
        public DateTimeOffset Date { get; private set; }

        public Func<int, ImageMetadata> GetMetadata { get; private set; }
    }
}
