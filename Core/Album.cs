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

        private readonly Lazy<ReadOnlyCollection<AlbumItem>> lazyItems;
        private readonly Lazy<DateTimeOffset> lazyDate;

        static Album() {
            Empty = new Album(new AlbumDescriptor("", ""), "", () => new AlbumItem[0], null);
        }

        public Album(AlbumDescriptor descriptor, string name, IList<AlbumItem> items, object securableToken) 
            : this(descriptor, name, () => items, securableToken)
        {
        }

        public Album(AlbumDescriptor descriptor, string name, Func<IList<AlbumItem>> getItems, object securableToken) {
            this.Descriptor = descriptor;
            this.Name = name;
            this.lazyItems = new Lazy<ReadOnlyCollection<AlbumItem>>(() => getItems().AsReadOnly());
            this.SecurableToken = securableToken;
            this.lazyDate = new Lazy<DateTimeOffset>(
                () => Items.Min(i => (DateTimeOffset?)i.Date) ?? DateTimeOffset.Now
            );
        }

        public AlbumDescriptor Descriptor          { get; private set; }
        public object SecurableToken               { get; private set; }
        public string Name                         { get; private set; }

        public DateTimeOffset Date {
            get { return this.lazyDate.Value; }
        }

        public ReadOnlyCollection<AlbumItem> Items {
            get { return this.lazyItems.Value; }
        }
    }
}
