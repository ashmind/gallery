using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Collections.ObjectModel;

using AshMind.Extensions;

namespace AshMind.Gallery.Core.Fixes {
    // This is a dummy class that only exists to fix the
    // https://connect.microsoft.com/VisualStudio/feedback/details/565313/breaking-problem-with-datetimeoffset-that-affects-multiple-framework-classes
    // It does not do anything by itself, but alternative cache can gather paths and set up its own monitoring.
    internal class FixedFileChangeMonitor : FileChangeMonitor {
        private readonly ReadOnlyCollection<string> filePaths;

        public FixedFileChangeMonitor(string[] paths) {
            this.filePaths = paths.AsReadOnly();
        }

        public override ReadOnlyCollection<string> FilePaths {
            get { return this.filePaths; }
        }

        public override DateTimeOffset LastModified {
            get { throw new NotSupportedException(); }
        }

        protected override void Dispose(bool disposing) {
            throw new NotSupportedException();
        }

        public override string UniqueId {
            get { throw new NotSupportedException(); }
        }
    }
}
