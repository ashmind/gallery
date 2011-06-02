using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Models {
    public class DeleteProposalGroupModel {
        public DeleteProposalGroupModel(ISet<User> by, ISet<AlbumItemModel> items) {
            this.By = by;
            this.Items = items;
        }

        public ISet<User> By { get; private set; }
        public ISet<AlbumItemModel> Items { get; private set; }
    }
}