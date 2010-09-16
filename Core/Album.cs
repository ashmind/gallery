using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using AshMind.Extensions;

namespace AshMind.Web.Gallery.Core {
    public class Album {
        public static Album Empty { get; private set; }

        static Album() {
            Empty = new Album(new GalleryItem[0]);
        }

        internal Album(IList<GalleryItem> items) {
            this.Items = items.AsReadOnly();
        }

        public string ID                             { get; internal set; }
        public string Name                           { get; internal set; }
        public DateTimeOffset Date                   { get; internal set; }
        public ReadOnlyCollection<GalleryItem> Items { get; private set; }
    }
}
