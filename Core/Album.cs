using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Core {
    public class Album {
        public static Album Empty { get; private set; }

        static Album() {
            Empty = new Album(new AlbumDescriptor("", ""), "", new AlbumItem[0]);
        }

        public Album(AlbumDescriptor descriptor, string name, IList<AlbumItem> items) {
            this.Descriptor = descriptor;
            this.Name = name;
            this.Items = items.AsReadOnly();
            this.Date = items.Min(i => (DateTimeOffset?)i.Date) ?? DateTime.Now;
        }

        public AlbumDescriptor Descriptor          { get; private set; }
        public string Name                         { get; private set; }
        public DateTimeOffset Date                 { get; private set; }
        public ReadOnlyCollection<AlbumItem> Items { get; private set; }
    }
}
