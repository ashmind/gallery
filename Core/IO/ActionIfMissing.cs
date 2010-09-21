using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.IO {
    public enum ActionIfMissing {
        ReturnNull,
        ReturnAsIs,
        ThrowException,
        CreateNew
    }
}
