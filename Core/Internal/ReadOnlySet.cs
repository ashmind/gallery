using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Internal {
    public class ReadOnlySet<T> : ISet<T> {
        private readonly ISet<T> inner;

        public ReadOnlySet(ISet<T> inner) {
            this.inner = inner;
        }

        public IEnumerator<T> GetEnumerator() {
            return inner.GetEnumerator();
        }

        public bool Contains(T item) {
            return inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            inner.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return inner.Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        public bool IsSubsetOf(IEnumerable<T> other) {
            return inner.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            return inner.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return inner.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            return inner.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other) {
            return inner.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other) {
            return inner.SetEquals(other);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private Exception SetIsReadOnlyException() {
            throw new InvalidOperationException("Set is read only.");
        }

        #region ICollection<T> members

        void ICollection<T>.Add(T item) {
            throw SetIsReadOnlyException();
        }

        void ICollection<T>.Clear() {
            throw SetIsReadOnlyException();
        }

        bool ICollection<T>.Remove(T item) {
            throw SetIsReadOnlyException();
        }

        #endregion

        #region ISet<T> methods

        bool ISet<T>.Add(T item) {
            throw SetIsReadOnlyException();
        }

        void ISet<T>.UnionWith(IEnumerable<T> other) {
            throw SetIsReadOnlyException();
        }

        void ISet<T>.IntersectWith(IEnumerable<T> other) {
            throw SetIsReadOnlyException();
        }

        void ISet<T>.ExceptWith(IEnumerable<T> other) {
            throw SetIsReadOnlyException();
        }

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) {
            throw SetIsReadOnlyException();
        }

        #endregion
    }
}
