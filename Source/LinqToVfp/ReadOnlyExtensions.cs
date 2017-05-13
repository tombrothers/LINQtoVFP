/*
 * LINQ to VFP 
 * https://github.com/tombrothers/LINQtoVFP
 * http://www.randomdevnotes.com/tag/linq-to-vfp/
 * 
 * Written by Tom Brothers (TomBrothers@Outlook.com)
 * 
 * Released to the public domain, use at your own risk!
 */
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LinqToVfp {
    internal static class ReadOnlyExtensions {
        internal static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> collection) {
            ReadOnlyCollection<T> roc = collection as ReadOnlyCollection<T>;
            if (roc == null) {
                if (collection == null) {
                    roc = EmptyReadOnlyCollection<T>.Empty;
                }
                else {
                    roc = new List<T>(collection).AsReadOnly();
                }
            }

            return roc;
        }

        private class EmptyReadOnlyCollection<T> {
            internal static readonly ReadOnlyCollection<T> Empty = new List<T>().AsReadOnly();
        }
    }
}