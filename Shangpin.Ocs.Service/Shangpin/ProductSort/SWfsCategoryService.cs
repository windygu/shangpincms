using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Service.Common;
using System.Xml;
using System.IO;
using Shangpin.Ocs.Entity.Extenstion.ProductFlat;
using Shangpin.Ocs.Service.ProductSort;

namespace Shangpin.Ocs.Service.Shangpin.ProductSort
{
    public class SWfsCategoryService
    {
        #region 树形结构
        /// <summary>
        /// 查找子类别
        /// </summary>
        /// <param name="parentNo">父级NO</param>
        /// <returns></returns>
        public IList<OCSInfo> SelectCategoryByParentNo(string parentNo)
        {
            IList<OCSInfo> CategoryList = DapperUtil.Query<OCSInfo>("ComBeziWfs_WfsCategory_CategoryByParentNO", new { ParentNo = parentNo }).ToList();
            foreach (OCSInfo item in CategoryList)
            {
                //int ChildCount = DapperUtil.Query<int>("ComBeziWfs_WfsCategory_CategoryByIsParent", new { ParentNo = item.CategoryNo }).First();
                item.isParent = true;
                ProductRulesService prs = new ProductRulesService();
                SWfsSortOcsCategory ocsCategory = prs.IsRuleCategory(item.CategoryNo);
                item.AutoLastFlag = ocsCategory!=null?ocsCategory.AutoLastFlag:0;
                if (ocsCategory != null && ocsCategory.DateUpdate.ToString("yyyy-MM-dd")!="1900-01-01")
                {
                    item.SortUpdateDate = ocsCategory.DateUpdate.ToString("yyyy-MM-dd");
                    item.IsUpdateDateOne = IsOne(ocsCategory.DateUpdate,System.DateTime.Now);
                }
            }
            return CategoryList;
        }

        /// <summary>
        /// 判断两个日期是否相差一个月
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool IsOne(DateTime start, DateTime end)
        {
            TimeSpan timespan = end.Subtract(start);
            return timespan.Days > 7 ? true : false;
            //int endMonth = (end.Year * 12) + end.Month;
            //int startMonth = (start.Year * 12) + start.Month;
            //if (endMonth - startMonth > 1)
            //{
            //    return true;
            //}
            //else if (endMonth - startMonth == 1)
            //{
            //    int endDay = end.Day;
            //    int startDay = start.Day;
            //    if (endDay > startDay)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //else
            //{
            //    return false;
            //}
        }
        #endregion
    }
}
