using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using AshMind.Web.Gallery.Core.Internal;

using Encoder = System.Drawing.Imaging.Encoder;

namespace AshMind.Web.Gallery.Core.ImageProcessing 
{
    internal class ImageCache
    {
        private readonly object DiskAccessLock = new object();

        public string CacheRoot         { get; private set; }
        public ImageCacheFormat Format  { get; private set; }

        private readonly MD5 md5 = MD5.Create();
        private readonly ImageCodecInfo m_imageEncoder;
        private readonly EncoderParameters m_imageEncoderParameters;
        
        public ImageCache(string cacheRoot, ImageCacheFormat format) {
            this.CacheRoot = cacheRoot;
            this.Format = format;

            m_imageEncoder = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == format.MimeType);
            m_imageEncoderParameters = new EncoderParameters {
                Param = new[] { new EncoderParameter(Encoder.Quality, 100L) }
            };
        }

        public string GetResultPath(string imagePath, int size, Func<Image, int, Image> transform) {
            var cacheKey = this.GetCacheKey(imagePath, size);
            var cachePath = Path.Combine(this.CacheRoot, cacheKey);

            if (!Directory.Exists(this.CacheRoot))
                Directory.CreateDirectory(this.CacheRoot);

            var cached = File.Exists(cachePath);
            if (!cached)
                this.SaveTransform(imagePath, cachePath, image => transform(image, size));

            return cachePath;
        }

        private void SaveTransform(string imagePath, string cachePath, Converter<Image, Image> transform) {
            lock (DiskAccessLock) {
                using (var original = Image.FromFile(imagePath))
                using (var result = transform(original)) {
                    result.Save(cachePath, m_imageEncoder, m_imageEncoderParameters);
                }
            }
        }

        private string GetCacheKey(string imagePath, int size) {
            var pathBytes = Encoding.UTF8.GetBytes(imagePath);
            var key = md5.ComputeHashAsString(pathBytes);
            return string.Format(
                "{0}-x{1}.{2}",
                key, size, this.Format.FileExtension
            );
        }
    }
}
