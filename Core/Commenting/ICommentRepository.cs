using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Commenting {
    public interface ICommentRepository {
        IList<Comment> LoadCommentsOf(string itemPath);
        void SaveComment(string itemPath, Comment comment);
    }
}
