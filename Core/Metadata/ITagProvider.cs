﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Metadata {
    public interface ITagProvider {
        IEnumerable<string> GetTags(string path);
    }
}