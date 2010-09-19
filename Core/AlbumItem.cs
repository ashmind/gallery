using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Web.Gallery.Core.ImageProcessing;

namespace AshMind.Web.Gallery.Core {
    public class AlbumItem {
        private Lazy<IList<Comment>> lazyComments;

        internal AlbumItem(
            string name,
            GalleryItemType type,
            DateTimeOffset date,
            Func<int, ImageMetadata> getMetadata,
            Func<IList<Comment>> getComments
        ) {
            this.Name = name;
            this.Type = type;
            this.Date = date;
            this.GetMetadata = getMetadata;
            this.lazyComments = new Lazy<IList<Comment>>(getComments);
        }

        public string Name { get; private set; }
        public GalleryItemType Type { get; private set; }
        public DateTimeOffset Date { get; private set; }

        public IList<Comment> Comments {
            get { return lazyComments.Value; }
        }

        public Func<int, ImageMetadata> GetMetadata { get; private set; }
    }
}
