using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class NewCommingProductService
    {
        #region 产品列表
        //按组ID获取组内产品列表
        public IEnumerable<ProductInfo> GetSWfsProductList(string gender, string brandNO, string categoryNo, string keyword, string starttime, string endtime, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNO", brandNO == null ? "" : brandNO);
            //dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            //dic.Add("TemplateName", templateName == null ? "" : templateName);
            //dic.Add("IsPublish", isPublish == null ? "" : isPublish);
            dic.Add("StartDateShelf", starttime == null ? "" : starttime);
            dic.Add("EndDateShelf", endtime == null ? "" : endtime);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsProduct_NewSelectSWfsProductCount", dic, new { KeyWord = keyword, BrandNO = brandNO, Gender = gender, CategoryNo = categoryNo, StartDateShelf = starttime, EndDateShelf = endtime }).FirstOrDefault();
            return DapperUtil.Query<ProductInfo>("ComBeziWfs_SWfsProduct_NewSelectSWfsProductList", dic, new { KeyWord = keyword, BrandNO = brandNO, Gender = gender, CategoryNo = categoryNo, StartDateShelf = starttime, EndDateShelf = endtime, pageIndex = pageIndex, pageSize = pageSize });
        }
        #endregion

        /// <summary>
        /// 已添加的上新列表
        /// </summary>
        /// <param name="newarrivalid"></param>
        /// <returns></returns>
        public IEnumerable<ProductInfo> GetUpdateGoods(int newarrivalid)
        {
            return DapperUtil.Query<ProductInfo>("ComBeziWfs_SWfsIndexNewArrival_SelUpdateGoods", new { NewArrivalId = newarrivalid });
        }
        
        public IEnumerable<SpfSkuExtendInfo> GetSkuListByProductNo(string productNo)
        {
            return DapperUtil.Query<SpfSkuExtendInfo>("ComBeziWfs_SkuList_GetSkuListByProductNo", new { ProductNo = productNo });
        }

        /// <summary>
        /// 上新时间设置的新标题
        /// </summary>
        /// <param name="newtitle">新标题</param>
        /// <returns></returns>
        public int UpdateSWfsGlobalConfigByFunctionNo(string newtitle)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsGlobalConfig_UpdateTitle", new { ConfigValue = newtitle });
        }
    }
}
