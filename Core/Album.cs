using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using AshMind.Extensions;

using AshMind.Gallery.Core.AlbumSupport;

namespace AshMind.Gallery.Core {
    public class Album : IReadOnlySupport<Album> {
        public static Album Empty { get; private set; }

        private bool readOnly;
        private Lazy<ReadOnlyCollection<AlbumItem>> lazyItems;
        private readonly Lazy<DateTimeOffset> lazyDate;

        static Album() {
            Empty = new Album(new AlbumDescriptor("", ""), "", null, () => new AlbumItem[0]);
        }

        public Album(AlbumDescriptor descriptor, string name, object providerData, IList<AlbumItem> items)
            : this(descriptor, name, providerData, () => items)
        {
        }

        public Album(AlbumDescriptor descriptor, string name, object providerData, Func<IList<AlbumItem>> getItems)
            : this(descriptor, name, providerData, new Lazy<ReadOnlyCollection<AlbumItem>>(() => getItems().AsReadOnly(), true))
        {
        }

        private Album(AlbumDescriptor descriptor, string name, object providerData, Lazy<ReadOnlyCollection<AlbumItem>> items) {
            this.Descriptor = descriptor;
            this.Name = name;
            this.ProviderData = providerData;
            this.lazyItems = items;
            this.lazyDate = new Lazy<DateTimeOffset>(
                () => Items.Min(i => (DateTimeOffset?)i.Date) ?? DateTimeOffset.Now,
                true
            );
        }

        public AlbumDescriptor Descriptor          { get; private set; }
        public string Name                         { get; private set; }
        public object ProviderData                 { get; private set; }

        public DateTimeOffset Date {
            get { return this.lazyDate.Value; }
        }

        public ReadOnlyCollection<AlbumItem> Items {
            get { return this.lazyItems.Value; }
        }

        #region IReadOnlySupport<Album> Members

        public void MakeReadOnly() {
            if (this.readOnly)
                return;

            this.readOnly = true;
            this.lazyItems = this.lazyItems.Apply(items => MakeReadOnly(items));
        }

        private void MakeReadOnly(IEnumerable<AlbumItem> items) {
            foreach (var item in items) {
                item.MakeReadOnly();
            }
        }
        
        public bool IsReadOnly {
            get { return this.readOnly; }
        }

        public Album AsWritable() {
            if (!this.readOnly)
                return this;

            return new Album(
                this.Descriptor, this.Name, this.ProviderData,
                () => this.lazyItems.Value.Select(item => item.AsWritable()).ToArray()
            );
        }

        #endregion
    }
}
