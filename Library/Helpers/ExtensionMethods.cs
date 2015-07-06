using System.Collections.Generic;
using System.Linq;

namespace NameProvider
{
    public static class ExtensionMethods
    {
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> @this)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] {Enumerable.Empty<T>()};
            return @this.Aggregate(emptyProduct, (accumulator, sequence) =>
                accumulator.SelectMany(accseq =>
                {
                    var enumerable = sequence as T[] ?? sequence as IList<T> ?? sequence.ToList();
                    return enumerable;
                }, (accseq, item) => accseq.Concat(new[] {item})));
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> @this, T item)
        {
            foreach (var t in @this) yield return t;
            yield return item;
        }
    }
}