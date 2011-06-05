using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core {
    public class AlbumItem : IReadOnlySupport<AlbumItem> {
        private bool readOnly;
        private Lazy<Album> lazyPrimaryAlbum;
        private KnownUser proposedToBeDeletedBy;

        public AlbumItem(
            IFile file,
            string name,            
            AlbumItemType type,
            DateTimeOffset date
        ) {
            this.File = file;
            this.Name = name;
            this.Type = type;
            this.Date = date;
        }

        public IFile File { get; private set; }
        public string Name { get; private set; }
        public AlbumItemType Type { get; private set; }
        public DateTimeOffset Date { get; private set; }

        public Album PrimaryAlbum {
            get { return this.LazyPrimaryAlbum != null ? this.LazyPrimaryAlbum.Value : null; }
        }

        public KnownUser ProposedToBeDeletedBy {
            get { return this.proposedToBeDeletedBy; }
            set {
                if (this.readOnly)
                    throw new InvalidOperationException("The AlbumItem is read-only."); 
                
                this.proposedToBeDeletedBy = value;
            }
        }

        internal Lazy<Album> LazyPrimaryAlbum {
            get { return this.lazyPrimaryAlbum; }
            set {
                if (this.readOnly)
                    throw new InvalidOperationException("The AlbumItem is read-only.");

                this.lazyPrimaryAlbum = value;
            }
        }

        public bool IsProposedToBeDeleted {
            get { return this.ProposedToBeDeletedBy != null; }
        }

        #region IReadOnlySupport<AlbumItem> Members

        public void MakeReadOnly() {
            if (this.readOnly)
                return;

            this.readOnly = true;
            this.lazyPrimaryAlbum = this.lazyPrimaryAlbum.Apply(album => album.MakeReadOnly());
        }

        public bool IsReadOnly {
            get { return this.readOnly; }
        }

        public AlbumItem AsWritable() {
            if (!this.IsReadOnly)
                return this;

            return new AlbumItem(this.File, this.Name, this.Type, this.Date) {
                ProposedToBeDeletedBy =  this.proposedToBeDeletedBy
            };
        }

        #endregion
    }
}
