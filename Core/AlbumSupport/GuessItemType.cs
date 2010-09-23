using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.AlbumSupport {
    internal static class GuessItemType {
        private static class FileExtensions {
            public static readonly HashSet<string> OfVideo = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { ".mov", ".avi", ".mpg" };
            public static readonly HashSet<string> OfImage = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif" };
        }

        public static AlbumItemType Of(string path) {
            var extension = Path.GetExtension(path);

            if (FileExtensions.OfVideo.Contains(extension))
                return AlbumItemType.Video;

            if (FileExtensions.OfImage.Contains(extension))
                return AlbumItemType.Image;

            return AlbumItemType.Unknown;
        }
    }
}
