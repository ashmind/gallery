using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Commenting;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumItemFactory {
        private readonly ICommentRepository commentRepository;

        public AlbumItemFactory(ICommentRepository commentRepository) {
            this.commentRepository = commentRepository;
        }

        public AlbumItem CreateFrom(IFile file, AlbumItemType itemType) {
            return new AlbumItem(
                file,
                file.Name,
                itemType,
                file.GetLastWriteTime(),
                () => this.commentRepository.LoadCommentsOf(file.Path)
            );
        }
    }
}
