using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using AshMind.Extensions;

namespace AshMind.Web.Gallery.Core {
    public class GalleryAlbum {
        public static GalleryAlbum Empty { get; private set; }

        static GalleryAlbum() {
            Empty = new GalleryAlbum(new GalleryItem[0], new string[0]);
        }

        internal GalleryAlbum(IList<GalleryItem> items, IList<string> tags) {
            this.Items = items.AsReadOnly();
            this.Tags = tags.AsReadOnly();
        }

        public string ID                             { get; internal set; }
        public string Name                           { get; internal set; }
        public DateTimeOffset Date                   { get; internal set; }
        public ReadOnlyCollection<GalleryItem> Items { get; private set; }
        public ReadOnlyCollection<string> Tags       { get; private set; }
    }
}
