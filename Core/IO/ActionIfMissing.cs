using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.IO {
    public enum ActionIfMissing {
        ReturnNull,
        ReturnAsIs,
        ThrowException,
        CreateNew
    }
}
