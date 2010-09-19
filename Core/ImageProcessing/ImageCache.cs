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

        public ILocation CacheRoot { get; private set; }
        public ImageCacheFormat Format { get; private set; }

        private readonly ICacheDependencyProvider[] dependencyProviders;

        private readonly ImageCodecInfo imageEncoder;
        private readonly EncoderParameters imageEncoderParameters;

        public ImageCache(ILocation cacheRoot, ImageCacheFormat format, params ICacheDependencyProvider[] dependencyProviders) {
            this.CacheRoot = cacheRoot;
            this.Format = format;

            this.dependencyProviders = dependencyProviders;

            this.imageEncoder = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == format.MimeType);
            this.imageEncoderParameters = new EncoderParameters {
                Param = new[] { new EncoderParameter(Encoder.Quality, 100L) }
            };
        }

        public IFile GetTransform(IFile imageFile, int size, Func<Image, int, Image> transform) {
            var cacheFile = GetCacheFile(imageFile, size);
            if (!IsCachedAndUpToDate(imageFile, cacheFile))
                return this.CacheTransform(imageFile, cacheFile, image => transform(image, size));

            return cacheFile;
        }

        private bool IsCachedAndUpToDate(IFile imageFile, IFile cacheFile) {
            if (!cacheFile.Exists)
                return false;

            var cacheLastWriteTime = cacheFile.GetLastWriteTime();
            if (imageFile.GetLastWriteTime() > cacheLastWriteTime)
                return false;

            var relatedChanges = this.dependencyProviders.SelectMany(p => p.GetRelatedChanges(imageFile));
            return relatedChanges.All(change => change < cacheLastWriteTime);
        }

        public ImageMetadata GetMetadata(IFile image, Func<ImageMetadata> getMetadata, Func<ImageMetadata, ImageMetadata> adjustMetadata) {
            var metadataFile = GetMetadataFile(image.Path);
            var metadata = (ImageMetadata)null;

            if (metadataFile.Exists)
                metadata = LoadMetadata(metadataFile);

            if (metadata == null) {
                metadata = getMetadata();
                SaveMetadata(metadataFile, metadata);
            }

            return adjustMetadata(metadata);
        }
        
        private IFile CacheTransform(IFile imageFile, IFile cacheFile, Converter<Image, Image> transform) {
            using (var original = LoadOriginalImage(imageFile))
            using (var result = transform(original)) 
            using (var stream = cacheFile.Open(FileLockMode.ReadWrite, FileOpenMode.Recreate))
            {
                result.Save(stream, this.imageEncoder, this.imageEncoderParameters);                
            }

            return cacheFile;
        }

        private void SaveMetadata(IFile metadataFile, ImageMetadata metadata) {
            metadataFile.WriteAllText(
                JsonConvert.SerializeObject(metadata)
            );
        }

        private ImageMetadata LoadMetadata(IFile metadataFile) {
            return JsonConvert.DeserializeObject<ImageMetadata>(
                metadataFile.ReadAllText()
            );
        }

        private Image LoadOriginalImage(IFile imageFile) {
            lock (DiskAccessLock) {
                // using (var stream = imageFile.Read(FileLockMode.Write)) {
                //     return Image.FromStream(stream);
                // }
                // TEMPHACK: for some reason above code does not load exif
                return Image.FromFile(imageFile.Path);
            }
        }

        private IFile GetCacheFile(IFile imageFile, int size) {
            var cacheKey = this.GetCacheKey(imageFile.Path, size);
            return this.CacheRoot.GetFile(cacheKey, false);
        }

        private IFile GetMetadataFile(string imagePath) {
            return this.CacheRoot.GetFile(GetCacheKey(imagePath) + ".metadata", false);
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
