using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin.ProductFlat;
using Shangpin.Ocs.Service.ProductSort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin.ProductSort
{
    public class WfsBrandService
    {
        //查询所有
        public IList<WfsBrandSort> SelectWfsBrandAll() 
        {
            IList<WfsBrandSort> listWfsBrand = DapperUtil.Query<WfsBrandSort>("ComBeziWfs_Brand_Sort_List").ToList();
            ProductRulesService prs = new ProductRulesService();
            IList<SWfsSortOcsCategory> listocs = prs.IsRuleCategoryAll();
            foreach (WfsBrandSort item in listWfsBrand)
            {
                //IList<SWfsSortOcsCategory> list=listocs.Where(t=>t.CategoryNo==item.BrandNo).ToList();
                if (listocs.Count(p => p.CategoryNo == item.BrandNo && p.DateUpdate.ToString("yyyy-MM-dd") != "1900-01-01") == 1)
                {
                    item.AutoLastFlag = listocs.Single(p => p.CategoryNo == item.BrandNo).AutoLastFlag;
                    item.SortUpdateDate = listocs.Single(p => p.CategoryNo == item.BrandNo).DateUpdate.ToString("yyyy-MM-dd");
                    item.IsUpdateDateOne = IsOne(listocs.Single(p => p.CategoryNo == item.BrandNo).DateUpdate,System.DateTime.Now);
                }
                //SWfsSortOcsCategory ocsCategory = prs.IsRuleCategoryAll(item.BrandNo);
                //item.AutoLastFlag = list.Count != 0 ? list[0].AutoLastFlag : 0;
                //if (list.Count != 0 != null && ocsCategory.DateUpdate.ToString("yyyy-MM-dd") != "1900-01-01")
                //{
                //    item.SortUpdateDate = ocsCategory.DateUpdate.ToString("yyyy-MM-dd");
                //    item.IsUpdateDateOne = IsOne(ocsCategory.DateUpdate, System.DateTime.Now);
                //}
            }
            return listWfsBrand;
        }

        /// <summary>
        /// 判断两个日期是否相差一个星期
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool IsOne(DateTime start, DateTime end)
        {
            TimeSpan timespan=end.Subtract(start);
            return timespan.Days > 7 ? true : false;
        }
    }
}
