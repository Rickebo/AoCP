using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Search
{
    public interface ISearchAlgorithm<TSource, TElement, TCost> 
        where TElement : ISearchElement<TCost> 
        where TSource : ISearchSource<TElement, TCost>
    {
        TSource Dataset { get; }
        ISearchResult Find(TElement start, TCost initialCost, TElement end);

    }
}
