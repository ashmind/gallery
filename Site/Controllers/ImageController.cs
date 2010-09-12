using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mime;

using AshMind.Web.Gallery.Core;
using AshMind.Web.Gallery.Site.Models;
using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Site.Controllers {
    [Authorize]
    public class ImageController : Controller {
        private IDictionary<ImageSize, int> imageSizes = new Dictionary<ImageSize, int> {
            { ImageSize.Small,      250 },
            { ImageSize.Preview,    1280 }
        };

        private IDictionary<string, string> knownMimeTypes = new Dictionary<string, string> {
            {".jpg",    MediaTypeNames.Image.Jpeg},
            {".jpeg",   MediaTypeNames.Image.Jpeg},
            {".png",    "image/png"}
        };

        private readonly AlbumFacade gallery;
        private readonly PreviewFacade preview;

        public ImageController(AlbumFacade gallery, PreviewFacade preview) {
            this.gallery = gallery;
            this.preview = preview;
        }

        //[OutputCache(Duration = 60, VaryByParam = "*")]
        public ActionResult Get(string album, string item, ImageSize size = ImageSize.Original) {
            var path = this.gallery.GetFullPath(album, item);
            if (size != ImageSize.Original)
                path = this.preview.GetPreviewPath(path, imageSizes[size]);

            var ifModifiedSince = Request.Headers["If-Modified-Since"];
            if (!string.IsNullOrEmpty(ifModifiedSince)) {
                var ifModifiedSinceDate = DateTimeOffset.Parse(ifModifiedSince);
                var lastModifiedDate = (DateTimeOffset)System.IO.File.GetLastWriteTimeUtc(path);

                if ((lastModifiedDate - ifModifiedSinceDate).TotalSeconds < 1) {
                    Response.StatusCode = 304;
                    return new EmptyResult();
                }
            }

            Response.AddFileDependency(path);
            Response.Cache.SetCacheability(HttpCacheability.Private);
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetLastModifiedFromFileDependencies();
            Response.Cache.SetExpires(DateTime.Now.AddDays(5));
            return File(
                path,
                knownMimeTypes[Path.GetExtension(path).ToLower()],
                Path.GetFileName(path)
            );
        }
    }
}
