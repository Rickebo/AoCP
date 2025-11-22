using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Search
{
    public interface ISearchSource<TElement, TCost> where TElement : ISearchElement<TCost>
    {
        public IEnumerable<SearchNeighbour<TElement, TCost>> GetNeighbours(TElement element);
    }
}
