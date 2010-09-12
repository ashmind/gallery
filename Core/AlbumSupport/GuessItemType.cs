using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.AlbumSupport
{
    internal static class GuessItemType
    {
        private static class FileExtensions
        {
            public static readonly HashSet<string> OfVideo = new HashSet<string> { ".mov", ".avi", ".mpg" };
            public static readonly HashSet<string> OfImage = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif" };
        }

        public static GalleryItemType Of(string path)
        {
            string extension = Path.GetExtension(path);

            if (FileExtensions.OfVideo.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
                return GalleryItemType.Video;

            if (FileExtensions.OfImage.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
                return GalleryItemType.Image;

            return GalleryItemType.Unknown;
        }
    }
}
