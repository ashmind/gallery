using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using AshMind.Extensions;

using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Core.Internal;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Values;

namespace AshMind.Gallery.Core {
    public class Album : IReadOnlySupport<Album> {
        public static Album Empty { get; private set; }

        private bool readOnly;
        private IValue<IList<AlbumItem>> items;
        private readonly IValue<DateTimeOffset> date;

        static Album() {
            Empty = new Album(new AlbumDescriptor("", ""), "", To.Just(new AlbumItem[0]));
        }

        protected Album(AlbumDescriptor descriptor, string name, IValue<IEnumerable<AlbumItem>> items)
            : this(descriptor, name, items.Get(v => v as IList<AlbumItem> ?? v.ToList()))
        {
        }

        protected Album(AlbumDescriptor descriptor, string name, IValue<IList<AlbumItem>> items) {
            this.Descriptor = descriptor;
            this.Name = name;
            this.items = items;
            this.date = items.Get(v => v.Min(i => (DateTimeOffset?)i.Date) ?? DateTimeOffset.Now);
            this.ViewedBy = new HashSet<IUser>();
        }

        public AlbumDescriptor Descriptor          { get; private set; }
        public string Name                         { get; private set; }

        public DateTimeOffset Date {
            get { return this.date.Value; }
        }

        public IValue<IList<AlbumItem>> Items {
            get { return this.items; }
            set {
                if (this.IsReadOnly)
                    throw new InvalidOperationException("This object is read-only.");

                this.items = value;
            }
        }

        public ISet<IUser> ViewedBy { get; private set; }

        #region IReadOnlySupport<Album> Members

        public void MakeReadOnly() {
            if (this.readOnly)
                return;

            this.readOnly = true;
            this.items = this.items.Get(MakeReadOnly);
            this.ViewedBy = new ReadOnlySet<IUser>(this.ViewedBy);
        }

        private ReadOnlyCollection<AlbumItem> MakeReadOnly(IList<AlbumItem> items) {
            foreach (var item in items) {
                item.MakeReadOnly();
            }
            return items.AsReadOnly();
        }
        
        public bool IsReadOnly {
            get { return this.readOnly; }
        }

        public Album AsWritable() {
            if (!this.readOnly)
                return this;

            var album = this.Recreate(
                this.Items.Get(v => v.Select(i => i.AsWritable()).ToList())
            );
            album.ViewedBy.AddRange(this.ViewedBy);

            return album;
        }

        protected virtual Album Recreate(IValue<IList<AlbumItem>> items) {
            return new Album(this.Descriptor, this.Name, items);
        }

        #endregion
    }
}
