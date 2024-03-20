using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Linq
{
    public static class LinqExtension
    {
        public static ObservableCollection<TSource> ToObservableCollection<TSource>(this IEnumerable<TSource> source)
        {
            ObservableCollection<TSource> collection = new ObservableCollection<TSource>();
            foreach (TSource item in source)
            {
                collection.Add(item);
            }
            return collection;
        }
    }
}
