using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Common
{
    public class PagingEntityBase
    {
        public int RowNum { get; set; }
        public int TotalCount { get; set; }
    }

    public class PageConvertor
    {
        public static RecordPage<T> Convert<T>(int pageIndex, int pageSize, IEnumerable<T> items) where T : PagingEntityBase
        {
            var page = new RecordPage<T> {CurrentPage = pageIndex, ItemsPerPage = pageSize};
            var firstOrDefault = items.FirstOrDefault();
            if (firstOrDefault != null)
            {
                page.TotalItems = firstOrDefault.TotalCount;
                page.RowNum = firstOrDefault.RowNum;
            }
            page.Items = items;

            page.TotalPages = page.TotalItems / pageSize;
            if ((page.TotalItems % pageSize) != 0)
                page.TotalPages++;


            return page;

        }

    }

    public class RecordPage<T>
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// 每页数目
        /// </summary>
        public int ItemsPerPage { get; set; }
        /// <summary>
        /// 集合
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// 当前序号
        /// </summary>
        public int RowNum { get; set; }
    }
}
