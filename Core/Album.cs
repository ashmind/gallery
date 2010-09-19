using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using AshMind.Extensions;
using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core {
    public class Album {
        public static Album Empty { get; private set; }

        static Album() {
            Empty = new Album(null, new AlbumItem[0]);
        }

        internal Album(ILocation location, IList<AlbumItem> items) {
            this.Location = location;
            this.Items = items.AsReadOnly();
        }

        public string ID                           { get; internal set; }
        public string Name                         { get; internal set; }
        public ILocation Location                  { get; private set; }
        public DateTimeOffset Date                 { get; internal set; }
        public ReadOnlyCollection<AlbumItem> Items { get; private set; }
    }
}
