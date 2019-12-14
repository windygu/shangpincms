using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SwfsFlagShipNewArrivalProductService
    {
        #region 最新上架管理页面
        /// <summary>
        /// 查询当前品牌下最新上架
        /// </summary>
        /// <param name="brandNo"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<NewShelfListModel> SelectNewShelfList(string brandNo, string startDate, string endDate, int pageIndex, int pageSize, out int count)
        {
            endDate = endDate != null && endDate != "" ? Convert.ToDateTime(endDate).AddDays(1).AddSeconds(-1).ToString() : "";
            List<NewShelfListModel> ListModel = new List<NewShelfListModel>();
            var dic = new Dictionary<string, object>();
            dic.Add("startDate", startDate == null ? "" : startDate);
            dic.Add("endDate",endDate);
            IEnumerable<SwfsFlagShipNewArrival> list = DapperUtil.Query<SwfsFlagShipNewArrival>("ComBeziWfs_SwfsFlagShipNewArrival_SelectBrandNo", dic, new { BrandNo = brandNo, startDate = startDate, endDate = endDate, pageIndex = pageIndex, pageSize = pageSize });
            foreach (SwfsFlagShipNewArrival item in list)
            {
                NewShelfListModel model = new NewShelfListModel();
                model.Arrival = item;
                model.OneRowProductList = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_SelectNewArrivalId", new { IsOneRow = 1, NewArrivalId = item.NewArrivalId, pageIndex = 1, pageSize = 6 }).ToList();
                model.TwoRowProductList = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_SelectNewArrivalId", new { IsOneRow = 0, NewArrivalId = item.NewArrivalId, pageIndex = 1, pageSize = 6 }).ToList();
                ListModel.Add(model);
            }
            count = DapperUtil.Query<int>("ComBeziWfs_SwfsFlagShipNewArrival_SelectBrandNoCount", dic, new { BrandNo = brandNo, startDate = startDate, endDate = endDate }).FirstOrDefault();
            return ListModel;
        }

        public SwfsFlagShipNewArrival SelectBrandNoNewShelfDate(string BrandNo, string NewShelfDate)
        {
            return DapperUtil.Query<SwfsFlagShipNewArrival>("ComBeziWfs_SwfsFlagShipNewArrival_BrandNoNewShelfDate", new { BrandNo = BrandNo, NewShelfDate = NewShelfDate }).FirstOrDefault();
        }

        /// <summary>
        /// 操作最新上架
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int OperationNewShelf(SwfsFlagShipNewArrival model, int NewArrivalId, string UserId)
        {
            //判断ID如果为空，那就是添加
            if (NewArrivalId == 0)
            {
                model.OperateUserId = UserId;
                model.NewArrivalTitle = "新品上架";
                model.PageNo = "index";
                model.PagePositionNo = "top";
                model.PagePositionName = "新品上架";
                model.CreateDate = System.DateTime.Now;
                model.SortValue = 0;
                model.Status = 1;
                model.DataState = 1;
                model.UpdateDate = System.DateTime.Now;
                return DapperUtil.Insert<SwfsFlagShipNewArrival>(model);
            }
            else//否则就是修改 
            {
                model.UpdateDate = System.DateTime.Now;
                model.NewArrivalId = NewArrivalId;
                string str = "update SwfsFlagShipNewArrival set NewShelfDate='"+model.NewShelfDate+"',UpdateOperateUserId='"+model.UpdateOperateUserId+"',UpdateDate='"+model.UpdateDate+"' where NewArrivalId="+model.NewArrivalId;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("sqlstring", str);
                return DapperUtil.Query<int>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_InsertsProduct", dic).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据NewArrivalId删除数据
        /// </summary>
        /// <param name="NewArrivalId"></param>
        /// <returns></returns>
        public int DeleteNewShelfListNewArrivalId(string NewArrivalId)
        {
            StringBuilder str = new StringBuilder();
            str.Append("delete from SwfsFlagShipNewArrivalProductList where NewArrivalId=" + NewArrivalId + ";");
            str.Append("delete from SwfsFlagShipNewArrival where NewArrivalId=" + NewArrivalId + ";");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", str);
            return DapperUtil.Query<int>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_InsertsProduct", dic).FirstOrDefault();
        }

        #endregion


        #region  上新时间设置页面方法
        /// <summary>
        /// 查询当前品牌下的上新时间配置
        /// </summary>
        /// <param name="BrandNo">品牌编号</param>
        /// <returns></returns>
        public SwfsFlagShipGloalConfig NewDateManageBrandNo(string brandNo)
        {
            return DapperUtil.Query<SwfsFlagShipGloalConfig>("ComBeziWfs_SwfsFlagShipGloalConfig_BrandNo", new { BrandNo = brandNo }).FirstOrDefault();
        }

        /// <summary>
        /// 配置上新时间设置方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ConfigNewDate(SwfsFlagShipGloalConfig model)
        {
            SwfsFlagShipGloalConfig Config = this.NewDateManageBrandNo(model.BrandNo);
            if (Config == null)//如果为空就是添加数据
            {
                try
                {
                    model.DataCreate = System.DateTime.Now;
                    DapperUtil.Insert<SwfsFlagShipGloalConfig>(model);
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                    throw;
                }
            }
            else //修改操作
            {
                Config.ConfigName = model.ConfigName;
                Config.ConfigValue = model.ConfigValue;
                Config.ConfigTime = model.ConfigTime;
                return DapperUtil.Update<SwfsFlagShipGloalConfig>(Config) == true ? 1 : 0;
            }
        }
        #endregion

        #region 商品管理页面
        //商品管理列表
        public List<ProductInfoNew> SelectNewArrivalId(string newArrivalId, string isOneRow)
        {
            List<ProductInfoNew> list = new List<ProductInfoNew>();
            list=DapperUtil.Query<ProductInfoNew>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_SelectNewArrivalId", new { IsOneRow = isOneRow, NewArrivalId = newArrivalId, pageIndex = 1, pageSize = 30 }).ToList();
            for (int i = 0; i < list.Count(); i++)//循环去取商品价格的信息
            {
                list[i] = SelectSpfSkuPrice(list[i],list[i].ProductNo);
            }
            return list;
        }

        public ProductInfoNew SelectSpfSkuPrice(ProductInfoNew Product,string ProductNo)//商品价格的信息
        {
            //取最小价格
            List<ProductInfoNew> Mininfo = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_SpfSkuPriceAll", new { ProductNo = ProductNo }).ToList();
            if (Mininfo != null && Mininfo.Count() != 0)
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
        //修改排序值
        public int UpdateProductSort(string idStr, string sortStr)
        {
            idStr = idStr.TrimEnd(',');
            sortStr = sortStr.TrimEnd(',');
            string[] idStrAttr = idStr.Split(',');
            string[] sortStrAttr = sortStr.Split(',');
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < idStrAttr.Count(); i++)
            {
                str.Append("update SwfsFlagShipNewArrivalProductList set SortValue=" + sortStrAttr[i] + " where ProductListId=" + idStrAttr[i] + ";");
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", str);
            return DapperUtil.Query<int>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_InsertsProduct", dic).FirstOrDefault();

        }
        //删除所选择的商品
        public int DeleteProduct(string idStr)
        {
            idStr = idStr.TrimEnd(',');
            StringBuilder str = new StringBuilder();
            str.Append("delete from SwfsFlagShipNewArrivalProductList where ProductListId in(" + idStr + ");");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", str);
            return DapperUtil.Query<int>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_InsertsProduct", dic).FirstOrDefault();
        }
        #endregion

        #region 添加商品关联页面方法

        //某品牌30天之内的上新商品集合
        public List<ProductInfoNew> BrandNewShelfProductList(string brandNo, int pageIndex, int pageSize, string dateShelf, string keyword, string categoryNo, string gender, out int count)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            DateTime NewDateShelf = Convert.ToDateTime(dateShelf);
            string startDate = System.DateTime.Now.AddDays(-30).ToString();
            string endDate = System.DateTime.Now.ToString();
            count = DapperUtil.Query<int>("ComBeziWfs_WfsProduct_FirstNewShelfProductCount", dic, new { BrandNo = brandNo, startDate = startDate, endDate = endDate, DateShelf = dateShelf, KeyWord = keyword, Gender = gender, CategoryNo = categoryNo }).FirstOrDefault();
            List<ProductInfoNew> list = new List<ProductInfoNew>();
            list=DapperUtil.Query<ProductInfoNew>("ComBeziWfs_WfsProduct_FirstNewShelfProduct", dic, new { BrandNo = brandNo, startDate = startDate, endDate = endDate, DateShelf = dateShelf, pageIndex = pageIndex, pageSize = pageSize, KeyWord = keyword, Gender = gender, CategoryNo = categoryNo }).ToList();
            for (int i = 0; i < list.Count(); i++)//循环去取商品价格的信息
            {
                list[i] = SelectSpfSkuPrice(list[i], list[i].ProductNo);
            }
            return list;
        }

        //添加商品
        public int AddShelfProduct(string productNoStr, string UserID, string NewArrivalId, string IsOneRow)
        {
            productNoStr = productNoStr.TrimEnd(',');
            string[] ProductNoList = productNoStr.Split(',');
            StringBuilder str = new StringBuilder();
            //找出当前品牌的当前上架时间排序最大值的商品
            int MaxSort = MaxSortProduct(NewArrivalId, IsOneRow);
            int count = MaxSort;
            foreach (string item in ProductNoList)
            {
                count++;
                str.Append("insert into SwfsFlagShipNewArrivalProductList values(" + NewArrivalId + ",'" + item + "','',''," + IsOneRow + "," + count + ",'" + System.DateTime.Now + "',1,1,'" + UserID + "','" + UserID + "','" + System.DateTime.Now + "');");
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", str);
            try
            {
                return DapperUtil.Query<int>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_InsertsProduct", dic).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //查询出最大的排序值
        public int MaxSortProduct(string NewArrivalId, string IsOneRow)
        {
            try
            {
                return DapperUtil.Query<int>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_MaxSort", new { NewArrivalId = NewArrivalId, IsOneRow = IsOneRow }).FirstOrDefault();
            }
            catch (Exception e)
            {

                return 0;
            }

        }
        #endregion

        #region 编辑商品展示图片页面方法
        /// <summary>
        /// 根据ProductListId查询商品信息
        /// </summary>
        /// <param name="ProductListId"></param>
        /// <returns></returns>
        public ProductInfoNew SelectProductInfoNewProductListId(string ProductListId)
        {
            return DapperUtil.Query<ProductInfoNew>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_ProductListIdProductInfo", new { ProductListId = ProductListId }).FirstOrDefault();
        }

        /// <summary>
        /// 查询SwfsFlagShipNewArrivalProductList表商品的信息
        /// </summary>
        /// <param name="ProductListId"></param>
        /// <returns></returns>
        public SwfsFlagShipNewArrivalProductList SelectProductListId(string ProductListId)
        {
            return DapperUtil.Query<SwfsFlagShipNewArrivalProductList>("ComBeziWfs_SwfsFlagShipNewArrivalProductList_ProductListId", new { ProductListId = ProductListId }).FirstOrDefault();
        }
        public int SelectProductListId(SwfsFlagShipNewArrivalProductList ArrivalProduct)
        {
            return DapperUtil.Update<SwfsFlagShipNewArrivalProductList>(ArrivalProduct) == true ? 1 : 0;
        }
        #endregion
    }
}
