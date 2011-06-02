using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core {
    public class AlbumItem : IReadOnlySupport<AlbumItem> {
        private bool readOnly;
        private Lazy<IList<Comment>> lazyComments;
        private Lazy<Album> lazyPrimaryAlbum;

        public AlbumItem(
            IFile file,
            string name,            
            AlbumItemType type,
            DateTimeOffset date,
            Func<IList<Comment>> getComments
        ) {
            this.File = file;
            this.Name = name;
            this.Type = type;
            this.Date = date;
            this.lazyComments = new Lazy<IList<Comment>>(getComments);
            this.DeleteProposals = new HashSet<User>();
        }

        public IFile File { get; private set; }
        public string Name { get; private set; }
        public AlbumItemType Type { get; private set; }
        public DateTimeOffset Date { get; private set; }

        public Album PrimaryAlbum {
            get { return this.LazyPrimaryAlbum != null ? this.LazyPrimaryAlbum.Value : null; }
        }

        public ICollection<User> DeleteProposals { get; private set; }
        internal Lazy<Album> LazyPrimaryAlbum {
            get { return this.lazyPrimaryAlbum; }
            set {
                if (this.readOnly)
                    throw new InvalidOperationException("The AlbumItem is read-only.");

                this.lazyPrimaryAlbum = value;
            }
        }

        public IList<Comment> Comments {
            get { return this.lazyComments.Value; }
        }

        public bool IsProposedToBeDeleted {
            get { return this.DeleteProposals.Count > 0; }
        }

        #region IReadOnlySupport<AlbumItem> Members

        public void MakeReadOnly() {
            if (this.readOnly)
                return;

            this.readOnly = true;
            this.lazyPrimaryAlbum = this.lazyPrimaryAlbum.Apply(album => album.MakeReadOnly());
            this.lazyComments = this.lazyComments.Apply(comments => comments.AsReadOnly());
            this.DeleteProposals = this.DeleteProposals.ToArray().AsReadOnly();
        }

        public bool IsReadOnly {
            get { return this.readOnly; }
        }

        public AlbumItem AsWritable() {
            if (!this.IsReadOnly)
                return this;

            var item = new AlbumItem(
                this.File, this.Name, this.Type, this.Date,
                () => this.Comments
            );
            item.DeleteProposals.AddRange(this.DeleteProposals);
            return item;
        }

        #endregion
    }
}
