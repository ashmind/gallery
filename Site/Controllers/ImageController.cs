using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mime;

using AshMind.Gallery.Core;
using AshMind.Gallery.Site.Models;
using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Controllers {
    [Authorize]
    public class ImageController : Controller {
        private IDictionary<string, string> knownMimeTypes = new Dictionary<string, string> {
            {".jpg",    MediaTypeNames.Image.Jpeg},
            {".jpeg",   MediaTypeNames.Image.Jpeg},
            {".png",    "image/png"}
        };

        private readonly AlbumFacade gallery;
        private readonly PreviewFacade preview;
        private readonly IRepository<User> userRepository;

        public ImageController(AlbumFacade gallery, PreviewFacade preview, IRepository<User> userRepository) {
            this.gallery = gallery;
            this.preview = preview;
            this.userRepository = userRepository;
        }

        //[OutputCache(Duration = 60, VaryByParam = "*")]
        public ActionResult Get(string album, string item, string size) {
            var imageSize = ImageSize.Parse(size);

            var file = this.gallery.GetItem(album, item, this.userRepository.FindByEmail(User.Identity.Name)).File;
            if (imageSize != ImageSize.Original)
                file = this.preview.GetPreview(file, imageSize.Size);

            var ifModifiedSince = Request.Headers["If-Modified-Since"];
            var lastModifiedDate = file.GetLastWriteTime();
            if (!string.IsNullOrEmpty(ifModifiedSince)) {
                var ifModifiedSinceDate = DateTimeOffset.Parse(ifModifiedSince);

                if ((lastModifiedDate - ifModifiedSinceDate).TotalSeconds < 1) {
                    Response.StatusCode = 304;
                    return new EmptyResult();
                }
            }

            Response.AddFileDependency(file.Path);
            Response.Cache.SetCacheability(HttpCacheability.Private);
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetLastModifiedFromFileDependencies();

            var maxAge = TimeSpan.Zero;
            if ((DateTimeOffset.Now - lastModifiedDate).TotalDays > 30) { // I think I will not edit these
                maxAge = TimeSpan.FromDays(90);
            }
            else {
                maxAge = TimeSpan.FromHours(1);
            }

            Response.Cache.SetExpires((DateTimeOffset.Now + maxAge).UtcDateTime);
            Response.Cache.SetMaxAge(maxAge);

            return File(
                file.Path,
                knownMimeTypes[Path.GetExtension(file.Name).ToLower()],
                file.Name
            );
        }
    }
}
