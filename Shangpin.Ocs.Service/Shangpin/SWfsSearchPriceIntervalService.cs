using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsSearchPriceIntervalService
    {

        public int DeleteSWfsSearchPriceInterval(string categoryNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSearchPriceInterval_Delete", new { CategoryNo = categoryNo });
        }

        public int AddSWfsSearchPriceInterval(SWfsSearchPriceInterval sWfsSearchPriceInterval)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSearchPriceInterval_Add", new
            {
                @CategoryNo = sWfsSearchPriceInterval.CategoryNo,
                @MinPrice = sWfsSearchPriceInterval.MinPrice,
                @MaxPrice = sWfsSearchPriceInterval.MaxPrice,
                @Status = 0,
                @OperatorUserId = sWfsSearchPriceInterval.OperatorUserId

            });
        }

        /// <summary>
        /// 查找分类下对应的时间段
        /// </summary>
        /// <param name="CategoryNo"></param>
        /// <returns></returns>
        public List<SWfsSearchPriceInterval> GetPriceListByCategoryNo(string CategoryNo)
        {
            return DapperUtil.Query<SWfsSearchPriceInterval>("ComBeziWfs_SWfsSearchPriceInterval_GetListByCategoryNo", new { CategoryNo = CategoryNo }).ToList();
        }
    }
}
