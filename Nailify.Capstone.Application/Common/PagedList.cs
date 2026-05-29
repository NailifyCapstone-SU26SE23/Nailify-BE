using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Common
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, long totalItems, int pageNumber, int pageSize)
        {
            _metaData = new MetaData
            {
                TotalItems = totalItems,
                PageSize = pageSize,
                CurrentPage = pageNumber,
            };
            AddRange(items);
        }

        private MetaData _metaData { get; }

        public MetaData GetMetaData()
        {
            return _metaData;
        }
    }
}
