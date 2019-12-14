using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Framework.Common;
using Shangpin.Framework.Common.Dapper;
using Shangpin.Ocs.Service.Common;
using System.Xml;
using Shangpin.Framework.Common.Cache;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class NewsChannelsSservice
    {
        #region 最新上架 by zhangman 20140903
        public Dictionary<string, List<ProductInfoShoes>> GetNewAlterPictureList(string titleName, string pageNo, string pagePositionNo, string startTime, string endTime, int pageIndex, int pageSize, out int count)
        {
            Dictionary<string, List<ProductInfoShoes>> diclist = new Dictionary<string, List<ProductInfoShoes>>();
            var dic = new Dictionary<string, object>();
            dic.Add("TitleName", (string.IsNullOrEmpty(titleName) || titleName == "标题") ? "" : titleName);
            dic.Add("PagePositionNo", string.IsNullOrEmpty(pagePositionNo) ? "" : pagePositionNo);
            dic.Add("StartTime", string.IsNullOrEmpty(startTime) ? "" : startTime);
            dic.Add("EndTime", string.IsNullOrEmpty(endTime) ? "" : endTime);
            DynamicParameters dp = new DynamicParameters();
            dp.Add("TitleName", titleName, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            dp.Add("PageNo", pageNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            if (!string.IsNullOrEmpty(pagePositionNo))
            {
                dp.Add("PagePositionNo", pagePositionNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                dp.Add("StartTime", startTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                dp.Add("EndTime", endTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            }

            //获取所有的商品列表信息
            IList<SWfsIndexNewArrival> list = DapperUtil.Query<SWfsIndexNewArrival>("ComBeziWfs_SWfsIndexNewArrival_SelectAllList", dic, dp).ToList();
            //分页显示
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            foreach (SWfsIndexNewArrival item in list)
            {
                DynamicParameters dp1 = new DynamicParameters();
                dp1.Add("NewArrivalId", item.NewArrivalId, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                List<ProductInfoShoes> slist = GetShoesNewProudecList(item.NewArrivalId.ToString());
                if (slist != null && slist.Count() > 0)
                {
                    slist[0].NewArrivalId = item.NewArrivalId;
                    slist[0].NewArrivalTitle = item.NewArrivalTitle;
                    slist[0].StartDate = item.StartDate;
                    diclist.Add(item.NewArrivalId + "_" + item.PagePositionNo, slist);
                }
            }
            count = diclist.Count();
            return diclist;
        }
        /// <summary>
        /// 根据上新编号获取上新信息
        /// </summary>
        /// <param name="arrivalid"></param>
        /// <returns></returns>
        public SWfsIndexNewArrival SelectNewArrivalByNid(int arrivalid)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("NewArrivalId", arrivalid, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            return DapperUtil.Query<SWfsIndexNewArrival>("ComBeziWfs_SWfsIndexNewArrival_SelectNewArrivalByNid", dp).FirstOrDefault();
        }
        /// <summary>
        /// 获取商品列表信息根据上新id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ProductInfoShoes> GetShoesNewProudecList(string id)
        {
            List<ProductInfoShoes> list = new List<ProductInfoShoes>();
            DynamicParameters dp = new DynamicParameters();
            dp.Add("NewArrivalId", id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            list = DapperUtil.Query<ProductInfoShoes>("ComBeziWfs_SWfsIndexNewArrival_GetShoesNewProductList", dp).ToList();
            if (list != null && list.Count() > 0)
            {
                foreach (var item in list)
                {
                    //查询该商品下的sku信息
                    List<ProductInfoShoes> skulist = GetSkuPriceListByProductNo(item.ProductNo);
                    foreach (var sku in skulist)
                    {
                        item.IsShelf = sku.IsShelf;
                        item.DateShelf = sku.DateShelf;
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 获取某频道的上新最大排序值
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pagePositionNo"></param>
        /// <returns></returns>
        public int GetNewArrivalMaxSort(string pageNo, string pagePositionNo)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("PageNo", pageNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("PagePositionNo", pagePositionNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            List<SWfsIndexNewArrival> list = DapperUtil.Query<SWfsIndexNewArrival>("ComBeziWfs_SWfsIndexNewArrival_SelectMaxSort", dp).ToList();
            if (null == list || list.Count() <= 0)
            {
                return 0;
            }
            else
            {
                return list.Select(a => a.SortValue).Max();
            }
        }
        /// <summary>
        /// 判断添加的上新是否有重复时间的
        /// </summary>
        /// <param name="startdate">上新时间</param>
        /// <returns></returns>
        public int SelectNewArrivalDate(DateTime startdate, int newArrivalId, string pageNo, string pagePositionNo)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("StartDate", startdate, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            dp.Add("NewArrivalId", newArrivalId, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            dp.Add("PageNo", pageNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("PagePositionNo", pagePositionNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            return DapperUtil.Query<int>("ComBeziWfs_SWfsIndexNewArrival_SelectNewDate", dp).FirstOrDefault();
        }
        /// <summary>
        /// 修改上新信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateNewArrivalInfo(SWfsIndexNewArrival model)
        {
            return DapperUtil.Update<SWfsIndexNewArrival>(model);
        }
        /// <summary>
        /// 执行添加上新信息
        /// </summary>
        /// <param name="sWfsIndexNewArrival">SWfsIndexNewArrival实体</param>
        /// <returns>返回主键id</returns>
        public int InsertNewArrivalInfo(SWfsIndexNewArrival model)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("NewArrivalTitle", model.NewArrivalTitle, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("SortValue", model.SortValue, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            dp.Add("WebSiteNo", model.WebSiteNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("PagePositionNo", model.PagePositionNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("PagePositionName", model.PagePositionName, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("PageNo", model.PageNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("CreateDate", model.CreateDate, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            dp.Add("StartDate", model.StartDate, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            dp.Add("Status", model.Status, System.Data.DbType.Int16, System.Data.ParameterDirection.Input);
            dp.Add("DataState", model.DataState, System.Data.DbType.Int16, System.Data.ParameterDirection.Input);
            dp.Add("OperateUserId", model.OperateUserId, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            var result = DapperUtil.Query<object>("ComBeziWfs_SWfsIndexNewArrival_InsertNewArrival", dp).FirstOrDefault();
            return int.Parse(result.ToString().Split('\'')[1]);
        }
        public ProductInventoryShoesNew GetInventoryByProductNo(string productNo)
        {
            ProductInventoryShoesNew pinventory = new ProductInventoryShoesNew();
            string xml = string.Empty;
            try
            {
                StockWebService.StockWebService stock = new StockWebService.StockWebService();
                if (AppSettingManager.AppSettings["InventoryFlag"].ToLower() == "true")
                {
                    stock.Url = AppSettingManager.AppSettings["ErpInventoryService"];
                    xml = stock.GetInventorySumByProductNo(productNo, 1); //1,正品；2，残次品；3，赠品
                }
                else
                {
                    xml = stock.GetInventorySumByProductNo(productNo, 1);//1,正品；2，残次品；3，赠品
                }
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                int sumQuantity = 0;  //库存总数
                int sumLockQuantity = 0; //锁定的总库存
                int inventory = 0;  //库存总数 减去 锁定的总库存

                if (doc != null)
                {
                    XmlNodeList inventoryNodeList = doc.SelectNodes("/ErpInventorySums/ErpInventorySum");
                    if (inventoryNodeList != null && inventoryNodeList.Count > 0)
                    {
                        string nodeName = string.Empty;
                        int inventoryQuantity = 0;
                        int lockQuantity = 0;
                        foreach (XmlNode inventoryNode in inventoryNodeList)
                        {
                            if (inventoryNode.ChildNodes.Count > 0)
                            {
                                inventoryQuantity = Convert.ToInt32(inventoryNode.ChildNodes[2].InnerText);
                                lockQuantity = Convert.ToInt32(inventoryNode.ChildNodes[3].InnerText);
                            }
                            sumQuantity += inventoryQuantity;
                            sumLockQuantity += lockQuantity;
                            inventory = inventory + inventoryQuantity - lockQuantity;
                        }
                    }
                }
                pinventory.ProductNo = productNo;
                pinventory.SumQuantity = sumQuantity;
                pinventory.SumLockQuantity = sumLockQuantity;
                pinventory.Inventory = inventory;

            }
            catch (Exception ex)
            {

            }
            return pinventory;
        }
        /// <summary>
        /// 商品排序操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortValue"></param>
        /// <returns></returns>
        public bool EditeBrandSortValue(int id, int sortValue)
        {
            return DapperUtil.UpdatePartialColumns<SWfsIndexNewArrivalProductList>(new
            {
                NewArrivalProductListId = id,
                SortValue = sortValue
            });
        }

        /// <summary>
        /// 删除商品信息
        /// </summary>
        /// <param name="idstr"></param>
        /// <returns></returns>
        public void DelShoesNewProduct(string idstr)
        {
            foreach (string id in idstr.Split(','))
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("NewArrivalProductListId", id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                DapperUtil.Execute("ComBeziWfs_SWfsIndexNewArrivalProductList_DelShoesNewProduct", dp);
            }
        }
        /// <summary>
        /// 删除上新信息
        /// </summary>
        /// <param name="id"></param>
        public void Del(string id)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsIndexNewArrival_DelShoesNewById", new { NewArrivalId = id });
        }
        /// <summary>
        /// 删除上新下的商品信息
        /// </summary>
        /// <param name="newarrivalid">上新id</param>
        public void DelNewProductsByNewArrivalId(string newarrivalid)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("NewArrivalId", newarrivalid, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            DapperUtil.Execute("ComBeziWfs_SWfsIndexNewArrivalProductList_DelNewProductsByNid", dp);
        }
        /// <summary>
        /// 获取上新时间信息根据编号
        /// </summary>
        /// <param name="functionNo">编号</param>
        /// <returns></returns>
        public SWfsGlobalConfig GetSWfsGlobalConfigByFunctionNo(string functionNo)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("FunctionNo", functionNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            return DapperUtil.Query<SWfsGlobalConfig>("ComBeziWfs_SWfsGlobalConfig_SelectConfigValueByNo", dp).FirstOrDefault();
        }

        /// <summary>
        /// 添加上新时间信息
        /// </summary>
        /// <param name="model">SWfsGlobalConfig</param>
        /// <returns></returns>
        public void Insert(SWfsGlobalConfig model)
        {
            DapperUtil.Insert<SWfsGlobalConfig>(model);
        }

        /// <summary>
        /// 上新时间设置的新标题
        /// </summary>
        /// <param name="newtitle">新标题</param>
        /// <returns></returns>
        public int UpdateShoesNewDateByFunctionNo(string newtitle)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsGlobalConfig_UpdateConfigValueByNo", new { ConfigValue = newtitle });
        }

        /// <summary>
        /// 获取最新预告信息
        /// </summary>
        /// <param name="pageno">频道编号</param>
        /// <returns></returns>
        public string GetNewNoticeNameByNowTime(string pageno)
        {
            string noticeTitle = "";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("PageNo", pageno, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            SWfsNewArrivalNotice model = DapperUtil.Query<SWfsNewArrivalNotice>("ComBeziWfs_SWfsNewArrivalNotice_SelectNewNoticeByPageNo", dp).FirstOrDefault();
            if (model != null)
            {
                noticeTitle = model.NewArrivalNoticName + "_" + model.NewArrivalNoticDate;
            }
            return noticeTitle;
        }

        /// <summary>
        /// 获取某上新商品下的商品
        /// </summary>
        /// <param name="gender">上新商品id</param>
        /// <param name="brandNO"></param>
        /// <param name="categoryNo"></param>
        /// <param name="keyword"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<ProductInfoShoes> GetSWfsProductList(string gender, string brandNO, string categoryNo, string keyword, string starttime, string endtime, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNO", brandNO == null ? "" : brandNO);
            dic.Add("StartDateShelf", starttime == null ? "" : starttime);
            dic.Add("EndDateShelf", endtime == null ? "" : endtime);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsProduct_SelectProductCount", dic, new { KeyWord = keyword, BrandNO = brandNO, Gender = gender, CategoryNo = categoryNo, StartDateShelf = starttime, EndDateShelf = endtime }).FirstOrDefault();
            return DapperUtil.Query<ProductInfoShoes>("ComBeziWfs_SWfsProduct_SelectAllProductList", dic, new { KeyWord = keyword, BrandNO = brandNO, Gender = gender, CategoryNo = categoryNo, StartDateShelf = starttime, EndDateShelf = endtime, pageIndex = pageIndex, pageSize = pageSize });
        }

        /// <summary>
        /// 根据商品编号获取sku价格信息
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public List<ProductInfoShoes> GetSkuPriceListByProductNo(string productNo)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("ProductNo", productNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            List<ProductInfoShoes> list = DapperUtil.Query<ProductInfoShoes>("ComBeziWfs_SWfsIndexHotProductListTemp_GetIndexHotSkutListByProduct", dp).ToList();
            if (list == null)
            {
                list = new List<ProductInfoShoes>();
            }
            return list;
        }
        /// <summary>
        /// 添加某一上新的商品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productNoStr"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int AddNewProduct(string id, string productNoStr, string userid, string flag)
        {
            string[] pNoList = productNoStr.Trim().TrimEnd(',').Split(',');
            StringBuilder str = new StringBuilder();
            int count = 0;
            if (flag != "0")//表示该上新下已存在商品
            {
                count = NewProductMaxSort(id);//找出最大排序值
            }
            for (int i = 0; i < pNoList.Length; i++)
            {
                int sort = count + pNoList.Length - i;//当前添加的商品的排序值，倒叙
                str.Append("insert into SWfsIndexNewArrivalProductList values('" + pNoList[i] + "'," + id + "," + sort + ",'" + System.DateTime.Now + "',1,1,'" + userid + "','" + userid + "','" + System.DateTime.Now + "');");
            }
            return InsertNewProduct(str.ToString());
        }
        public int InsertNewProduct(string sqlstrs)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sqlstring", sqlstrs);
            try
            {
                return DapperUtil.Query<int>("ComBeziWfs_SWfsIndexNewArrivalProductList_InsertProducts", dic).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取某上新商品下的最大排序值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int NewProductMaxSort(string id)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("NewArrivalId", id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            return DapperUtil.Query<int>("ComBeziWfs_SWfsIndexNewArrivalProductList_SelectMaxSort", dp).FirstOrDefault();
        }

        /// <summary>
        /// 根据产品id获取产品信息
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public Product GetProductByProductNo(string no)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("ProductNo", no, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            return DapperUtil.Query<Product>("ComBeziWfs_WfsProduct_SelectProductInfoByNo", dp).FirstOrDefault();
        }

        /// <summary>
        /// 獲取最近的上新信息
        /// </summary>
        /// <param name="pageno"></param>
        /// <param name="pagepositionno"></param>
        /// <returns></returns>
        public SWfsIndexNewArrival SelectNewArrivalInfo(string pageno, string pagepositionno)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("PageNo", pageno, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("PagePositionNo", pagepositionno, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            return DapperUtil.Query<SWfsIndexNewArrival>("ComBeziWfs_SWfsIndexNewArrival_SelectNewArrivalInfo", dp).FirstOrDefault();
        }
        /// <summary>
        /// 获取最新上架商品信息
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pagePositionNo"></param>
        /// <returns></returns>
        public Dictionary<string, List<ProductInfoShoes>> GetNewArrivalProductListInfo(string pageNo = "", string pagePositionNo = "")
        {
            Dictionary<string, List<ProductInfoShoes>> diclist = new Dictionary<string, List<ProductInfoShoes>>();

            //获取所有的商品列表信息
            SWfsIndexNewArrival item = SelectNewArrivalInfo(pageNo, pagePositionNo);
            if (item != null)
            {
                DynamicParameters dp1 = new DynamicParameters();
                dp1.Add("NewArrivalId", item.NewArrivalId, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                List<ProductInfoShoes> slist = GetShoesNewProudecList(item.NewArrivalId.ToString());
                if (slist.Count()>0)
                {
                    slist[0].NewArrivalId = item.NewArrivalId;
                    slist[0].NewArrivalTitle = item.NewArrivalTitle;
                    diclist.Add(item.NewArrivalId + "_" + item.PagePositionNo, slist);
                }
            }
            return diclist;
        }
        #endregion

        #region 楼层相关-hbq
        public Dictionary<string, string> picSizeAndTip { get; set; }
        public const string WomenShoes = "CHANNEL_NV003";
        public const string Website = "shangpin.com";
        public DateTime MinTime { get; set; }
        public NewsChannelsSservice()
        {
            this.MinTime = Convert.ToDateTime("1900/01/01");
            this.picSizeAndTip = GetPicSizeAndTip();
        }
        /// <summary>
        /// 获取楼层模型
        /// </summary>
        /// <returns></returns>
        public SWfsIndexModule GetEmptySWfsIndexModuleModel()
        {
            SWfsIndexModule ClassfiedFloor = new SWfsIndexModule();
            DateTime now = DateTime.Now;
            ClassfiedFloor.Stutas = 1;
            ClassfiedFloor.UpdateDate = MinTime;
            ClassfiedFloor.DateCreate = DateTime.Now;
            ClassfiedFloor.OperateUserId = PresentationHelper.GetPassport().UserName;
            ClassfiedFloor.ModuleName = "";
            ClassfiedFloor.DataState = 1;
            ClassfiedFloor.UpdateDate = MinTime;
            ClassfiedFloor.ADShowTypeChangeDate = MinTime;
            return ClassfiedFloor;
        }
        /// <summary>
        /// 获取图片模型
        /// </summary>
        /// <returns></returns>
        public SWfsOperationPicture GetEmptySWfsOperationPictureModel()
        {
            SWfsOperationPicture pic = new SWfsOperationPicture();
            pic.PictureName = "";
            pic.LinkAddress = "";
            pic.DataState = (short)1;
            pic.Status = 1;
            pic.OperatorUserId = PresentationHelper.GetPassport().UserName;
            pic.DateCreate = DateTime.Now;
            pic.LinkAddress = "";
            pic.PagePositionName = "";
            pic.DateBegin = MinTime;
            pic.DateEnd = MinTime;
            return pic;
        }
        /// <summary>
        /// 获取链接模型
        /// </summary>
        /// <returns></returns>
        public SWfsIndexModuleLink GetEmptySWfsIndexModuleLinkModel()
        {
            SWfsIndexModuleLink link = new SWfsIndexModuleLink();
            link.CategoryNo = string.Empty;
            link.DateCreate = DateTime.Now;
            link.UpdateDate = MinTime;
            link.OperateUserId = PresentationHelper.GetPassport().UserName;

            link.UpdateDate = MinTime;
            link.UpdateDate = Convert.ToDateTime("1991/02/10 01:01:00");
            link.UpdateOperateUserId = PresentationHelper.GetPassport().UserName;
            link.Stutas = 1;
            link.DataState = 1;
            return link;
        }
        /// <summary>
        /// 配置楼层大小,名称
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetPicSizeAndTip()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["pic1"] = "width:230px;height:150px;";
            dic["pic1Name"] = "分类一";
            dic["pic1Check"] = "width:230,Height:150,Length:200";
            dic["pic1Tip"] = "尺寸：230*150;格式：jpg ;大小：200K以下";
            ////
            dic["pic2"] = "width:230px;height:310px;";
            dic["pic2Name"] = "分类二";
            dic["pic2Check"] = "width:230,Height:310,Length:200";
            dic["pic2Tip"] = "尺寸：230*310;格式：jpg ;大小：200K以下";
            ///////
            dic["pic3"] = "width:290px;height:470px;";
            dic["pic3Name"] = "分类三";
            dic["pic3Check"] = "width:290,Height:470,Length:200";
            dic["pic3Tip"] = "尺寸：290*470;格式：jpg ;大小：200K以下";
            //////
            dic["pic4"] = "width:230px;height:310px;";
            dic["pic4Name"] = "分类四";
            dic["pic4Tip"] = "尺寸：230*310;格式：jpg ;大小：200K以下";
            dic["pic4Check"] = "width:230,Height:310,Length:200";
            //////
            dic["pic5"] = "width:230px;height:150px;";
            dic["pic5Name"] = "分类五";
            dic["pic5Tip"] = "尺寸：230*150;格式：jpg ;大小：200K以下";
            dic["pic5Check"] = "width:230,Height:150,Length:200";
            /////
            dic["picSingleSize"] = "width:310px;height:380px;";
            dic["picSingleName"] = "单图";
            dic["picSingleCheck"] = "width:310,Height:380,Length:500";
            dic["picSingleTip"] = "尺寸：310*380;格式：jpg ;大小：500K以下";
            return dic;
        }
        /// <summary>
        /// 获取楼层广告图分页
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<SWfsOperationPicture> GetSWfsindexModuleRefPic(Dictionary<string, object> dic, int pageIndex, int pageSize, out int count)
        {
            IEnumerable<SWfsOperationPicture> query = DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPicturePage", dic, dic["object"]);
            //查询总条数
            int totalCount = DapperUtil.Query<int>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureCount", dic, dic["object"]).First<int>();
            count = totalCount;
            return query ?? new List<SWfsOperationPicture>();
        }
        /// <summary>
        /// 通过PageNo获取频道页楼层
        /// </summary>
        /// <returns></returns>
        public List<SWfsIndexModule> GetChanelFloors(string PageNo, string WebSiteNo = "shangpin.com")
        {
            return DapperUtil.Query<SWfsIndexModule>("ComBeziWfs_SWfsIndexModule_GetChanelFloors", new { PageNo = PageNo, WebSiteNo = WebSiteNo }).FilterList() ?? new List<SWfsIndexModule>();
        }
        /// <summary>
        /// 批量更新楼层
        /// </summary>
        /// <param name="moduls"></param>
        public int UpdateSWfsIndexModuleSort(List<SWfsIndexModule> moduls)
        {
            try
            {
                foreach (SWfsIndexModule item in moduls)
                {
                    DapperUtil.UpdatePartialColumns<SWfsIndexModule>(new { item.ModuleId, item.Sort, item.Stutas });
                }
            }
            catch (Exception)
            {

                return 0;
            }
            return 1;

        }

        public int CreateClassifiedFloor(string PageNo)
        {
            try
            {
                List<SWfsIndexModule> floors = GetChanelFloors(PageNo);
                if (!floors.Any(a => a.ADShowType == 5) && (new SWfsProductCommentService().GetGlobalConfigByFNo(PageNo) != null))
                {
                    SWfsIndexModule floor = GetEmptySWfsIndexModuleModel();
                    floor.ADShowType = 5;
                    floor.ModuleName = "分类展示楼层";
                    floor.ADShowType = 5;
                    floor.WebSiteNo = NewsChannelsSservice.Website;
                    floor.PageNo = PageNo;
                    floor.ADShowTypeChangeDate = Convert.ToDateTime("2099-01-01");
                    new SWfsIndexModuleService().InsertOrUpdateSWfsIndexModule(floor);
                    return 1;
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 创建楼层
        /// </summary>
        /// <param name="AdshowType"></param>
        /// <param name="PageNo"></param>
        /// <param name="ModuleName"></param>
        /// <param name="Sort"></param>
        /// <returns></returns>
        public int CreateFloor(short AdshowType, string PageNo, string ModuleName, int Sort)
        {
            try
            {

                List<SWfsIndexModule> floors = GetChanelFloors(PageNo);
                if (!floors.Any(a => a.Sort == Sort) && floors.Count() <= 8 && (new SWfsProductCommentService().GetGlobalConfigByFNo(PageNo) != null))//包含分类层一共9个
                {
                    SWfsIndexModule floor = GetEmptySWfsIndexModuleModel();
                    floor.ADShowType = AdshowType;
                    floor.ModuleName = ModuleName;
                    floor.WebSiteNo = NewsChannelsSservice.Website;
                    floor.Sort = Sort;
                    floor.PageNo = PageNo;
                    floor.ADShowTypeChangeDate = Convert.ToDateTime("2099-01-01");
                    new SWfsIndexModuleService().InsertOrUpdateSWfsIndexModule(floor);
                    return 1;
                }
                return -1;
            }
            catch (Exception)
            {

                return -1;
            }
        }
        /// <summary>
        /// 开始时间小于现在时间的广告图 
        /// </summary>
        /// <param name="module"></param>
        public List<SWfsOperationPicture> GetChannelSWfsOperationPicture(int ModuleId, int ADShowType, int top = 1)
        {
            if (ModuleId == 0)
                return new List<SWfsOperationPicture>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["ADShowType"] = ADShowType;
            return DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetChannelSWfsOperationPicture", dic,
                 new
                 {
                     ModuleId = ModuleId.ToString(),
                     ADShowType = ADShowType,
                     tops = top
                 }
                 ).FilterList() ?? new List<SWfsOperationPicture>();

        }
        #endregion

        #region 最新上架预告 by zhaohw 20140903
        /// <summary>
        /// 获取最新预报列表
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="WebsiteNo"></param>
        /// <returns></returns>
        public List<SWfsNewArrivalNotice> GetNewArrivalNoticeList(string PageNo, string KeyWord, string BeginTime, string EndTime, string WebsiteNo = "shangpin.com")
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            List<SWfsNewArrivalNotice> list = new List<SWfsNewArrivalNotice>();
            list = DapperUtil.Query<SWfsNewArrivalNotice>("ComBeziWfs_SWfsNewArrivalNotice_GetNewArrivalNoticeList", dic, new { PageNo = PageNo, WebsiteNo = WebsiteNo, KeyWord = KeyWord, BeginTime = BeginTime, EndTime = EndTime }).FilterList() ?? new List<SWfsNewArrivalNotice>();
            return list;
        }

        /// <summary>   
        /// 修改最新预报列表
        /// </summary>
        /// <param name="NewArrivalNoticeId"></param>
        /// <returns></returns>
        public int ModifyNewArrivalNoticeById(string Id, string name, string Date, string UserID)
        {
            int result = 0;
            var resultint = DapperUtil.UpdatePartialColumns<SWfsNewArrivalNotice>(new { NewArrivalNoticeId = Id, NewArrivalNoticName = name, NewArrivalNoticDate = Date, UpdateDate = DateTime.Now, UpdateOperateUserId = UserID });
            return result = resultint == false ? 0 : 1;
        }

        /// <summary>
        /// 修改最新预报列表
        /// </summary>
        /// <param name="NewArrivalNoticeId"></param>
        /// <returns></returns>
        public int DeLNewArrivalNoticeById(string Id, string UserID)
        {
            int result = 0;
            var resultint = DapperUtil.UpdatePartialColumns<SWfsNewArrivalNotice>(new { NewArrivalNoticeId = Id, DataState = 255, UpdateDate = DateTime.Now, UpdateOperateUserId = UserID });
            return result = resultint == false ? 0 : 1;
        }

        /// <summary>
        /// 添加最新预报列表
        /// </summary>
        /// <param name="NewArrivalNoticeDTO"></param>
        /// <returns></returns>
        public int AddNewArrivalNotice(SWfsNewArrivalNotice NewArrivalNoticeDTO)
        {
            int result = 0;
            result = DapperUtil.Insert<SWfsNewArrivalNotice>(NewArrivalNoticeDTO);
            return result;
        }

        /// <summary>
        /// 判断时间是否重复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageNo"></param>
        /// <param name="DateValue"></param>
        /// <returns></returns>
        public string IsExitNewArrivalNotice(string id, string pageNo, string DateValue)
        {
            string result = "0";
            List<SWfsNewArrivalNotice> list = new List<SWfsNewArrivalNotice>();
            string datestart = Convert.ToDateTime(DateValue).ToShortDateString() + " 00:00:00";
            string dateend = Convert.ToDateTime(DateValue).ToShortDateString() + " 23:59:59";
            list = DapperUtil.Query<SWfsNewArrivalNotice>("ComBeziWfs_SWfsNewArrivalNotice_IsExitNewArrivalNoticeList", new { PageNo = pageNo, NewArrivalNoticDateStart = datestart, NewArrivalNoticDateEnd = dateend }).ToList();

            if (list.Count > 0)
            {
                if (list[0].NewArrivalNoticeId + "" != id)
                {
                    result = "1";
                }
            }
            return result;
        }
        /// <summary>
        /// 获取上新预告是否关闭
        /// </summary>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public string GetStatusNewsNotice(string pageNo)
        {
            string result = "0";
            List<SWfsNewArrivalNotice> list = new List<SWfsNewArrivalNotice>();
            list = DapperUtil.Query<SWfsNewArrivalNotice>("ComBeziWfs_SWfsNewArrivalNotice_GetStatusNewsNotice", new { PageNo = pageNo, WebSiteNo = "shangpin.com" }).ToList();
            if (list != null && list.Count > 0)
            {
                result = list[0].Status + "";
            }

            return result;
        }
        /// <summary>
        /// 关闭上新预告
        /// </summary>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        public string ChannelNewsNotice(string PageNo, string DataState, string WebSiteNo = "shangpin.com")
        {
            string result = "";
            if (DataState == "0")
            {
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsNewArrivalNotice_GetSWfsNewArrivalNotice_GetSWfsNewArrivalNoticeCHANNEL_NV003");
            }
            result = DapperUtil.Execute("ComBeziWfs_SWfsNewArrivalNotice_UpdateStatusNewsNotice", new { DataState = DataState, PageNo = PageNo, WebSiteNo = WebSiteNo }) + "";
            return result;
        }
        #endregion

        #region 横幅广告列表
        /// <summary>
        /// 获取横幅广告状态
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="WebSiteNo"></param>
        /// <param name="PagePositionNo"></param>
        /// <returns></returns>
        public string GetStatusBananaPicture(string PageNo, string WebSiteNo = "shangpin.com", string PagePositionNo = "BannerADHeight70")
        {
            string result = "0";
            List<SWfsNewArrivalNotice> list = new List<SWfsNewArrivalNotice>();
            list = DapperUtil.Query<SWfsNewArrivalNotice>("ComBeziWfs_SWfsOperationPicture_GetStatusBananaPicture", new { PageNo = PageNo, WebSiteNo = "shangpin.com", PagePositionNo = PagePositionNo }).ToList();
            if (list != null && list.Count > 0)
            {
                result = list[0].Status + "";
            }
            return result;
        }

        /// <summary>
        /// 关闭横幅广告
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="Status"></param>
        /// <param name="WebSiteNo"></param>
        /// <param name="PagePositionNo"></param>
        /// <returns></returns>
        public string ChannelBananaPicture(string PageNo, string Status, string WebSiteNo = "shangpin.com", string PagePositionNo = "BannerADHeight70")
        {
            string result = "";
            if (Status == "0")
            {
                EnyimMemcachedClient.Instance.Remove("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureByPageNo_GetIndexOperationPicture_CHANNEL_NV003_BannerADHeight70");
            }
            result = DapperUtil.Execute("ComBeziWfs_SWfsOperationPicture_UpdateStatusBananaPicture", new { Status = Status, PageNo = PageNo, WebSiteNo = WebSiteNo, PagePositionNo = PagePositionNo }) + "";
            return result;
        }
        #endregion
    }
}
