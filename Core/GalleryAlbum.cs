using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core {
    public class GalleryAlbum
    {
        public GalleryAlbum(IList<GalleryItem> items) {
            this.Items = items;
        }

        public string ID                { get; internal set; }
        public string Name              { get; internal set; }
        public DateTimeOffset Date      { get; internal set; }
        public IList<GalleryItem> Items { get; private set; }
    }
}
