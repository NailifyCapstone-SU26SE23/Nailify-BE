using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Common
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; } = new();
        public MetaData MetaData { get; set; }

        public PagedList()
        {
        }

        public PagedList(IEnumerable<T> items, long totalItems, int pageNumber, int pageSize)
        {
            MetaData = new MetaData
            {
                TotalItems = totalItems,
                PageSize = pageSize,
                CurrentPage = pageNumber,
            };
            Items.AddRange(items);
        }

        public MetaData GetMetaData()
        {
            return MetaData;
        }
    }
}
