using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using AshMind.Extensions;

using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core {
    public class Album : IReadOnlySupport<Album> {
        public static Album Empty { get; private set; }

        private bool readOnly = false;
        private Lazy<ReadOnlyCollection<AlbumItem>> lazyItems;
        private readonly Lazy<DateTimeOffset> lazyDate;

        static Album() {
            Empty = new Album(new AlbumDescriptor("", ""), "", () => new AlbumItem[0], null);
        }

        public Album(AlbumDescriptor descriptor, string name, IList<AlbumItem> items, object securableToken) 
            : this(descriptor, name, () => items, securableToken)
        {
        }

        public Album(AlbumDescriptor descriptor, string name, Func<IList<AlbumItem>> getItems, object securableToken)
            : this(descriptor, name, new Lazy<ReadOnlyCollection<AlbumItem>>(() => getItems().AsReadOnly(), true), securableToken)
        {
        }

        private Album(AlbumDescriptor descriptor, string name, Lazy<ReadOnlyCollection<AlbumItem>> items, object securableToken) {
            this.Descriptor = descriptor;
            this.Name = name;
            this.lazyItems = items;
            this.SecurableToken = securableToken;
            this.lazyDate = new Lazy<DateTimeOffset>(
                () => Items.Min(i => (DateTimeOffset?)i.Date) ?? DateTimeOffset.Now,
                true
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
                this.Descriptor, this.Name,
                () => this.lazyItems.Value.Select(item => item.AsWritable()).ToArray(),
                this.SecurableToken
            );
        }

        #endregion
    }
}
