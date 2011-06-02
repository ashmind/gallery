using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core {
    public interface IReadOnlySupport<out TReadOnlySupport>
        where TReadOnlySupport : IReadOnlySupport<TReadOnlySupport>
    {
        void MakeReadOnly();
        bool IsReadOnly { get; }

        TReadOnlySupport AsWritable();
    }
}
