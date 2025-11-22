using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Search
{
    public class SearchNeighbour<TNeighbour, TCost> 
        where TNeighbour : ISearchElement<TCost>
    {
        public TNeighbour Element { get; init; }
        public TCost Cost { get; init; }
    }
}
