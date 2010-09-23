using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.ImageProcessing;
using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core {
    public class AlbumItem {
        private Lazy<IList<Comment>> lazyComments;

        internal AlbumItem(
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
        }

        public IFile File { get; private set; }
        public string Name { get; private set; }
        public AlbumItemType Type { get; private set; }
        public DateTimeOffset Date { get; private set; }

        public Album PrimaryAlbum {
            get { return this.LazyPrimaryAlbum != null ? this.LazyPrimaryAlbum.Value : null; }
        }

        internal Lazy<Album> LazyPrimaryAlbum { get; set; }

        public IList<Comment> Comments {
            get { return lazyComments.Value; }
        }
    }
}
