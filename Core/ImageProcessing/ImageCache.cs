using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using AshMind.Web.Gallery.Core.Internal;
using AshMind.Web.Gallery.Core.IO;

using Encoder = System.Drawing.Imaging.Encoder;
using Newtonsoft.Json;

namespace AshMind.Web.Gallery.Core.ImageProcessing {
    public class ImageCache {
        private readonly object DiskAccessLock = new object();

        public string CacheRoot { get; private set; }
        public ImageCacheFormat Format { get; private set; }

        private readonly ImageCodecInfo imageEncoder;
        private readonly EncoderParameters imageEncoderParameters;

        public ImageCache(string cacheRoot, ImageCacheFormat format) {
            this.CacheRoot = cacheRoot;
            this.Format = format;

            this.imageEncoder = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == format.MimeType);
            this.imageEncoderParameters = new EncoderParameters {
                Param = new[] { new EncoderParameter(Encoder.Quality, 100L) }
            };
        }

        public string GetTransformPath(string imagePath, int size, Func<Image, int, Image> transform) {
            var cachePath = GetCachePath(imagePath, size);
            
            var cached = System.IO.File.Exists(cachePath);
            if (!cached)
                return this.CacheTransform(imagePath, cachePath, image => transform(image, size));

            return cachePath;
        }

        public ImageMetadata GetMetadata(IFile image, Func<ImageMetadata> getMetadata, Func<ImageMetadata, ImageMetadata> adjustMetadata) {
            var metadataPath = GetMetadataPath(image.Path);
            var metadata = (ImageMetadata)null;

            if (System.IO.File.Exists(metadataPath))
                metadata = LoadMetadata(metadataPath);

            if (metadata == null) {
                metadata = getMetadata();
                SaveMetadata(metadataPath, metadata);
            }

            return adjustMetadata(metadata);
        }
        
        private string CacheTransform(string imagePath, string cachePath, Converter<Image, Image> transform) {
            using (var original = LoadOriginalImage(imagePath))
            using (var result = transform(original)) {
                result.Save(cachePath, this.imageEncoder, this.imageEncoderParameters);
                return cachePath;
            }
        }

        private void SaveMetadata(string metadataPath, ImageMetadata metadata) {
            System.IO.File.WriteAllText(
                metadataPath,
                JsonConvert.SerializeObject(metadata)
            );
        }

        private ImageMetadata LoadMetadata(string metadataPath) {
            return JsonConvert.DeserializeObject<ImageMetadata>(
                System.IO.File.ReadAllText(metadataPath)
            );
        }

        private Image LoadOriginalImage(string imagePath) {
            lock (DiskAccessLock) {
                return Image.FromFile(imagePath);
            }
        }

        private string GetCachePath(string imagePath, int size) {
            if (!Directory.Exists(this.CacheRoot))
                Directory.CreateDirectory(this.CacheRoot);

            var cacheKey = this.GetCacheKey(imagePath, size);
            return Path.Combine(this.CacheRoot, cacheKey);
        }

        private string GetMetadataPath(string imagePath) {
            if (!Directory.Exists(this.CacheRoot))
                Directory.CreateDirectory(this.CacheRoot);

            return Path.Combine(
                this.CacheRoot,
                GetCacheKey(imagePath) + ".metadata"
            );
        }

        private string GetCacheKey(string imagePath, int size) {
            return string.Format(
                "{0}-x{1}.{2}",
                GetCacheKey(imagePath), size,
                this.Format.FileExtension
            );
        }

        private string GetCacheKey(string imagePath) {
            using (var md5 = MD5.Create()) {
                var pathBytes = Encoding.UTF8.GetBytes(imagePath);
                var key = md5.ComputeHashAsString(pathBytes);
                return key;
            }
        }
    }
}
