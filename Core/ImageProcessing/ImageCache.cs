using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using AshMind.IO.Abstraction;

using AshMind.Gallery.Core.Internal;
using AshMind.Gallery.Imaging;

namespace AshMind.Gallery.Core.ImageProcessing {
    public class ImageCache {
        public ILocation CacheRoot { get; private set; }
        public IImageFormat CacheFormat { get; private set; }

        private readonly IDictionary<string, IImageLoader> imageLoaders;
        private readonly ICacheDependencyProvider[] dependencyProviders;
        
        public ImageCache(
            ILocation cacheRoot,
            IImageFormat cacheFormat,
            IImageLoader[] imageLoaders,
            ICacheDependencyProvider[] dependencyProviders
        ) {
            this.CacheRoot = cacheRoot;

            this.CacheFormat = cacheFormat;
            this.imageLoaders = imageLoaders.SelectMany(
                                    loader => loader.FileExtensions,
                                    (loader, extension) => new { extension, loader }
                                ).ToDictionary(
                                    x => x.extension,
                                    x => x.loader,
                                    StringComparer.InvariantCultureIgnoreCase
                                );
            this.dependencyProviders = dependencyProviders;
        }

        public IFile GetTransform(IFile imageFile, int size, Func<IImage, int, IImage> transform) {
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
                
        private IFile CacheTransform(IFile imageFile, IFile cacheFile, Converter<IImage, IImage> transform) {
            using (var original = LoadOriginalImage(imageFile))
            using (var result = transform(original)) {
                this.CacheFormat.Save(result, cacheFile);
            }

            return cacheFile;
        }

        private IImage LoadOriginalImage(IFile imageFile) {
            return this.imageLoaders[imageFile.Extension].Load(imageFile);
        }

        private IFile GetCacheFile(IFile imageFile, int size) {
            var cacheKey = this.GetCacheKey(imageFile.Path, size);
            return this.CacheRoot.GetFile(cacheKey, ActionIfMissing.ReturnAsIs);
        }
        
        private string GetCacheKey(string imagePath, int size) {
            return string.Format(
                "{0}-x{1}.{2}",
                GetCacheKey(imagePath), size,
                this.CacheFormat.FileExtensions[0]
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
