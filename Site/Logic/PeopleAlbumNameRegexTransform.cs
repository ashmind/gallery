using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.AlbumSupport;

namespace AshMind.Gallery.Site.Logic {
    public class PeopleAlbumNameRegexTransform : IAlbumNameTransform {
        private readonly Regex regex;
        private readonly string replacement;

        public PeopleAlbumNameRegexTransform(Regex regex, string replacement) {
            this.regex = regex;
            this.replacement = replacement;
        }

        public string Transform(string albumName, AlbumDescriptor descriptor) {
            if (descriptor.ProviderKey != AlbumProviderKeys.People)
                return albumName;

            return regex.Replace(albumName, this.replacement);
        }
    }
}