using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AshMind.Gallery.Site.Models {
    public sealed class ImageSize {
        public static ImageSize Small       { get; private set; }
        public static ImageSize Medium      { get; private set; }
        public static ImageSize Large       { get; private set; }
        public static ImageSize Original    { get; private set; }

        private static IDictionary<string, ImageSize> sizesByName;

        static ImageSize() {
            Small    = new ImageSize("Small", 250);
            Medium   = new ImageSize("Medium", 500);
            Large    = new ImageSize("Large", 1000);
            Original = new ImageSize("Original", 10000);

            sizesByName = new[] { Small, Medium, Large, Original }.ToDictionary(s => s.Name, StringComparer.InvariantCultureIgnoreCase);
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
