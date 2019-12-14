using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class NewShelfBrandProductService
    {

        #region //已经生成上新商品列表的页面方法
        /// <summary>
        /// 查找当前品牌下所有15天内上新的商品
        /// </summary>
        /// <param name="brandNo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<ProductInfoNew> BrandNewShelfProductList(string brandNo, int pageIndex, int pageSize, DateTime DateShelf, string keyword, string categoryNo, string gender, string startDate, string endDate, out int count)
        {
            var dic = new Dictionary<string, object>();
            string dayStartDate = DateShelf.ToString("yyyy-MM-dd");//当天开始时间
            string dayEndtDate = DateTime.Parse(dayStartDate).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"); //当结束时间
            startDate = startDate == null || startDate == "" || Convert.ToDateTime(startDate) < DateShelf.AddDays(-15) ? DateShelf.AddDays(-15).ToString() : startDate;
            endDate = endDate == null || endDate == "" || Convert.ToDateTime(endDate) > DateShelf ? DateShelf.ToString() : endDate;
            //startDate = startDate == null || startDate == "" || Convert.ToDateTime(startDate) < System.DateTime.Now.AddDays(-15) ? System.DateTime.Now.AddDays(-15).ToString() : startDate;
            //endDate = endDate == null || endDate == "" || Convert.ToDateTime(endDate) > System.DateTime.Now ? System.DateTime.Now.ToString() : endDate;
            //string startDate = DateShelf.AddDays(-15).ToString("yyyy-MM-dd");//15天前的时间
            //DateTime endDate = DateShelf;//当前时间
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            count = DapperUtil.Query<int>("ComBeziWfs_WfsProduct_BrandTop15DayNewShelfProductCount", dic, new { BrandNo = brandNo, startDate = startDate, endDate = endDate, dayStartDate = dayStartDate, dayEndtDate = dayEndtDate, KeyWord = keyword, Gender = gender, CategoryNo = categoryNo }).FirstOrDefault();
            List<ProductInfoNew> list = new List<ProductInfoNew>();
            list = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_WfsProduct_BrandTop15DayNewShelfProduct", dic, new { BrandNo = brandNo, startDate = startDate, endDate = endDate, pageIndex = pageIndex, pageSize = pageSize, dayStartDate = dayStartDate, dayEndtDate = dayEndtDate, KeyWord = keyword, Gender = gender, CategoryNo = categoryNo }).ToList();
            for (int i = 0; i < list.Count(); i++)//循环去取商品价格的信息
            {
                list[i] = SelectSpfSkuPrice(list[i], list[i].ProductNo);
            }
            return list;
        }
        public ProductInfoNew SelectSpfSkuPrice(ProductInfoNew Product, string ProductNo)//商品价格的信息
        {
            //取最小价格
            List<ProductInfoNew> Mininfo = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_SpfSkuPriceAll", new { ProductNo = ProductNo }).ToList();
            if (Mininfo != null && Mininfo.Count()!=0)
            {
                Product.MinSellPrice = Mininfo.Min(p => p.SellPrice);//最小黄金价
                Product.MinPlatinumPrice = Mininfo.Min(p => p.PlatinumPrice);//最小白金价
                Product.MinDiamondPrice = Mininfo.Min(p => p.DiamondPrice);//最小钻石价
                Product.MinLimitedPrice = Mininfo.Min(p => p.LimitedPrice);//最小尚品价格
                Product.MinMarketPrice = Mininfo.Min(p => p.MarketPrice);//最小钻石价
                Product.MinPromotionPrice = Mininfo.Min(p => p.PromotionPrice);//最小尚品价格

                Product.MaxSellPrice = Mininfo.Max(p => p.SellPrice);//最大黄金价
                Product.MaxPlatinumPrice = Mininfo.Max(p => p.PlatinumPrice);//最大白金价
                Product.MaxDiamondPrice = Mininfo.Max(p => p.DiamondPrice);//最大钻石价
                Product.MaxLimitedPrice = Mininfo.Max(p => p.LimitedPrice);//最大尚品价格
                Product.MaxMarketPrice = Mininfo.Max(p => p.MarketPrice);//最大钻石价
                Product.MaxPromotionPrice = Mininfo.Max(p => p.PromotionPrice);//最大尚品价格
            }
           
                
            
            return Product;
        }

        /// <summary>
        /// 返回当前品牌下所有15天内上新的商品总数
        /// </summary>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        //public int BrandNewShelfProductListCont(string brandNo, DateTime DateShelf) 
        //{
        //    string startDate = DateShelf.AddDays(-30).ToString("yyyy-MM-dd");//30天前的时间
        //    DateTime endDate = DateShelf;//当前时间
        //    string dayStartDate = DateShelf.ToString("yyyy-MM-dd");//当天开始时间
        //    string dayEndtDate = DateTime.Parse(dayStartDate).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"); //当天结束时间
        //    return DapperUtil.Query<int>("ComBeziWfs_WfsProduct_BrandTop15DayNewShelfProductCount", new { BrandNo = brandNo, startDate = startDate, endDate = endDate,dayStartDate = dayStartDate, dayEndtDate = dayEndtDate }).FirstOrDefault();
        //}

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="brandNo">品牌ID</param>
        /// <param name="productNoStr">商品字符串（可能多个用‘,’）</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="OperateType">操作状态（0,代表自动生成的，1代表手动添加的）</param>
        /// <param name="DateShelf">上架时间（指的是频道页上架时间）</param>
        /// <param name="Sort">排序号</param>
        /// <param name="WeekDays">星期号</param>
        /// <returns></returns>
        public int AddShelfProduct(string brandNo, string productNoStr, string UserID, int OperateType, DateTime DateShelf, int WeekDays)
        {
            string[] ProductNoList = productNoStr.Split(',');
            StringBuilder str = new StringBuilder();
            //找出当前品牌的当前上架时间排序最大值的商品
            SWfsNewProductManage MaxSortP = ShelfProductMaxSort(brandNo, DateShelf);
            int count =MaxSortP==null?0:MaxSortP.Sort;
            int BrandAmout = MaxSortP == null ? 0 : MaxSortP.BrandAmout;//获取原来的总数
            BrandAmout = BrandAmout + ProductNoList.Count();//变为现在的总数
            string startDate = DateShelf.ToString("yyyy-MM-dd");//当天开始时间
            string EndtDate = DateTime.Parse(startDate).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"); //当结束时间
            foreach (string item in ProductNoList)
            {
                count++;
                str.Append("insert into SWfsNewProductManage values('" + item + "','" + brandNo + "',1,1,'" + UserID + "','" + System.DateTime.Now + "'," + OperateType + "," + count + ",'" + DateShelf + "'," + WeekDays + ",1," + BrandAmout + "," + MaxSortP.floor + ",'" + MaxSortP.DateShow + "');");
            }
            //修改SWfsNewProductManage的BrandAmout数量
            //str.Append("update SWfsNewProductManage set BrandAmout=(select  count(*) from SWfsNewProductManage where brandNo='" + brandNo + "' and DateShelf between '" + startDate + "'  and '" + EndtDate + "')");
            //str.Append(" where brandNo='" + brandNo + "' and DateShelf between '" + startDate + "'  and '" + EndtDate + "'");
            //修改SWfsNewBrandProductManage的数量
            //str.Append("update SWfsNewBrandProductManage set NewShelfAmount=(select  count(*) from SWfsNewProductManage where brandNo='" + brandNo + "' and DateShelf between '" + startDate + "'  and '" + EndtDate + "')");
            //str.Append(" where brandNo='" + brandNo + "' and DateShelf between '" + startDate + "'  and '" + EndtDate + "'");
            return InsertNewProduct(str.ToString());
        }



        /// <summary>
        /// 添加上新商品
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public int InsertNewProduct(string sql)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", sql);
            try
            {
                return DapperUtil.Query<int>("ComBeziWfs_SWfsNewProductManage_Inserts", dic).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 找出当前品牌下当前时间的上新的商品的最大排序值的商品
        /// </summary>
        /// <param name="brandNo"></param>
        /// <param name="productNo"></param>
        /// <param name="dateShelf"></param>
        /// <returns></returns>
        public SWfsNewProductManage ShelfProductMaxSort(string brandNo, DateTime dateShelf)
        {
            string startDate = dateShelf.ToString("yyyy-MM-dd");
            string endDate = DateTime.Parse(startDate).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            return DapperUtil.Query<SWfsNewProductManage>("ComBeziWfs_SWfsNewProductManage_MaxSort", new { BrandNo = brandNo, startDate = startDate, endDate = endDate }).FirstOrDefault();
        }

        #endregion


        #region 上新商品管理页面方法


        /// <summary>
        /// 找出已经生成好的某天的某个品牌的上架商品
        /// </summary>
        /// <param name="brandNo">品牌ID</param>
        /// <param name="dateShelf">上架时间</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<ProductInfoNew> NewProductList(string brandNo, DateTime DateShelf, string keyword, string categoryNo, string gender)
        {
            string startDate = Convert.ToDateTime(DateShelf).ToString("yyyy-MM-dd");
            string endDate = DateTime.Parse(startDate).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            List<ProductInfoNew> list = new List<ProductInfoNew>();
            list=DapperUtil.Query<ProductInfoNew>("ComBeziWfs_SWfsNewProductManage_NewProductList", dic, new { BrandNo = brandNo, startDate = startDate, endDate = endDate, KeyWord = keyword, Gender = gender, CategoryNo = categoryNo }).ToList();
            return list;
        
        }

        /// <summary>
        /// 找出已经生成好的某天的某个品牌的上架商品总数
        /// </summary>
        /// <param name="brandNo"></param>
        /// <param name="dateShelf"></param>
        /// <returns></returns>
        public int NewProductListCount(string brandNo, DateTime dateShelf)
        {
            string startDate = dateShelf.ToString("yyyy-MM-dd");
            string endDate = DateTime.Parse(startDate).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            return DapperUtil.Query<int>("ComBeziWfs_SWfsNewProductManage_NewProductListCount", new { BrandNo = brandNo, startDate = startDate, endDate = endDate }).FirstOrDefault();
        }

        //根据上新商品管理表中的id删除操作
        public int DeleteNewProductList(string idStr, string brandNo, DateTime DateShelf, int IsCount10)
        {
            StringBuilder str = new StringBuilder();
            str.Append("Delete from SWfsNewProductManage where Id in(");
            str.Append("" + idStr + ");");
            if (IsCount10 == 0)//等于0代表数据已经不足十条
            {
                str.Append("Delete from SWfsNewBrandProductManage where DateShelf='" + DateShelf + "' and brandNo='" + brandNo + "';");
            }
            //string startDate = DateShelf.ToString("yyyy-MM-dd");//当天开始时间
            //string EndtDate = DateTime.Parse(startDate).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"); //当结束时间
            //修改SWfsNewProductManage的BrandAmout数量
            //str.Append("update SWfsNewProductManage set BrandAmout=(select  count(*) from SWfsNewProductManage where brandNo='" + brandNo + "' and DateShelf between '" + startDate + "'  and '" + EndtDate + "')");
            //str.Append(" where brandNo='" + brandNo + "' and DateShelf between '" + startDate + "'  and '" + EndtDate + "'");
            //修改SWfsNewBrandProductManage的BrandAmout数量
            //str.Append("update SWfsNewBrandProductManage set NewShelfAmount=(select  count(*) from SWfsNewProductManage where brandNo='" + brandNo + "' and DateShelf between '" + startDate + "'  and '" + EndtDate + "')");
            //str.Append(" where brandNo='" + brandNo + "' and DateShelf between '" + startDate + "'  and '" + EndtDate + "'");
            string sql = str.ToString();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", sql);
            try
            {
                return DapperUtil.Query<int>("ComBeziWfs_SWfsNewProductManage_Deletes", dic).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 修改上新商品排序值
        /// </summary>
        /// <param name="idStr">id字符串</param>
        /// <param name="sortStr">排序值字符串</param>
        /// <returns></returns>
        public int UpdateShelfProductSort(string idStr, string sortStr)
        {
            StringBuilder str = new StringBuilder();
            string[] idAttr = idStr.Split(',');
            string[] sortAttr = sortStr.Split(',');
            for (int i = 0; i < idAttr.Count(); i++)
            {
                str.Append("update SWfsNewProductManage set sort=" + sortAttr[i] + " where id=" + idAttr[i] + ";");
            }
            string sql = str.ToString();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", sql);
            try
            {
                return DapperUtil.Query<int>("ComBeziWfs_SWfsNewProductManage_UpdateSort", dic).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 排序置顶操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int TopOneSortShelfProduct(string id, string brandNo, DateTime dateShelf)
        {
            string startDate = dateShelf.ToString("yyyy-MM-dd");
            string endDate = DateTime.Parse(startDate).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            //把当前品牌下，的当前上新的所有商品排序值加1
            string sql = "update SWfsNewProductManage set sort=sort+1 where BrandNo='" + brandNo + "' and DateShelf between '" + startDate + "'  and '" + endDate + "'";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", sql);
            DapperUtil.Query<int>("ComBeziWfs_SWfsNewProductManage_UpdateSort", dic).FirstOrDefault();//执行排序加1操作
            int minSort = DapperUtil.Query<int>("ComBeziWfs_SWfsNewBrandManage_SelectMinSort", new { BrandNo = brandNo, startDate = startDate, endDate = endDate }).FirstOrDefault();
            return UpdateShelfProductSort(id, (minSort - 1).ToString());
        }
        #endregion

        #region 上新商品列表页面方法
        /// <summary>
        /// 找出上新商品大于或等于10的上新品牌，并排序
        /// </summary>
        /// <param name="weekDays">星期</param>
        /// <param name="dateShelf">上架时间</param>
        /// <returns></returns>
        public List<SWfsNewProductManage> SelectNewBrandManageWeekDays(int pageIndex, int pageSize, string brandNo, string startDate, string endDate)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("StartDate", startDate == null ? "" : startDate);
            dic.Add("EndDate", endDate == null ? "" : endDate);
            return DapperUtil.Query<SWfsNewProductManage>("ComBeziWfs_SWfsNewProductManage_GrderByBrandNoDateShelf", dic, new { pageIndex = pageIndex, pageSize = pageSize, BrandNo = brandNo, StartDate = startDate, EndDate = endDate }).ToList();
        }

        /// <summary>
        /// 查询星期几下的上新品牌下的上新商品
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<ProductInfoNew>> SelectNewBrandWeekDaysProduct(int pageIndex, int pageSize, string brandNo, string startDate, string endDate, out int cout)
        {
            Dictionary<string, List<ProductInfoNew>> dicModel = new Dictionary<string, List<ProductInfoNew>>();
            List<SWfsNewProductManage> blist = SelectNewBrandManageWeekDays(pageIndex, pageSize, brandNo, startDate, endDate);
            var dic = new Dictionary<string, object>();
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("StartDate", startDate == null ? "" : startDate);
            dic.Add("EndDate", endDate == null ? "" : endDate);
            cout = DapperUtil.Query<int>("ComBeziWfs_SWfsNewProductManage_GrderByBrandNoDateShelfCount", dic, new { BrandNo = brandNo, StartDate = startDate, EndDate = endDate }).FirstOrDefault();

            foreach (SWfsNewProductManage item in blist)
            {
                List<ProductInfoNew> plist = NewProductList(item.BrandNo, item.DateShelf, null, null, null);
                dicModel.Add(item.BrandNo + "_" + item.DateShelf, plist);
            }
            return dicModel;
        }
        #endregion
    }
}
