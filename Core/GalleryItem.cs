using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core
{
    public class GalleryItem
    {
        public string Name          { get; internal set; }
        public GalleryItemType Type { get; internal set; }
        public DateTimeOffset Date  { get; internal set; }
    }
}
