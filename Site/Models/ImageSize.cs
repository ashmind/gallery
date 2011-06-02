using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Site.Models {
    public sealed class ImageSize {
        public static ImageSize Thumbnail   { get; private set; }
        public static ImageSize Small       { get; private set; }
        public static ImageSize Medium      { get; private set; }
        public static ImageSize Large       { get; private set; }
        public static ImageSize Original    { get; private set; }

        private static readonly IDictionary<string, ImageSize> sizesByName;

        static ImageSize() {
            Thumbnail = new ImageSize("Thumbnail", 250);
            Small     = new ImageSize("Small", 500);
            Medium    = new ImageSize("Medium", 1000);
            Large     = new ImageSize("Large", 3000);
            Original  = new ImageSize("Original", 10000);

            sizesByName = new[] { Thumbnail, Small, Medium, Large, Original }.ToDictionary(s => s.Name, StringComparer.InvariantCultureIgnoreCase);
        }

        public ImageSize(string name, int size) {
            this.Name = name;
            this.Size = size;
        }
        
        public static ImageSize Parse(string name) {
            return sizesByName[name];
        }

        public string Name  { get; private set; }
        public int Size     { get; set; }

        public override string ToString() {
            return this.Name;
        }
    }
}
