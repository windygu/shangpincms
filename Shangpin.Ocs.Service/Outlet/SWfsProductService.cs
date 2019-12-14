using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.Xml;
using Com.ShangPin.WebService.Stock;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Entity.Item;
using Shangpin.Framework.Common.Cache;
using Shangpin.Framework.WebMvc;

namespace Shangpin.Ocs.Service.Outlet
{
    public class SWfsProductService
    {
        private static string _key;
        private static WebServiceAgent _agent;
        public SWfsProductService()
        {
            if (_agent == null)
                _agent = new WebServiceAgent(AppSettingManager.AppSettings["ErpInventoryService"]);

            if (string.IsNullOrEmpty(_key))
                _key = _agent.Invoke("GetSkuNoCacheKeyFormatString").ToString();


        }
        /// <summary>
        /// 批量查询商品库存接口----未完成，因为批量接口返回中不包含锁定库存，待定···
        /// </summary>
        /// <param name="productNos"></param>
        /// <returns></returns>
        public List<ProductInventory> GetInventoryByProductNos(List<string> productNolist)
        {
            StringBuilder str = new StringBuilder();
            foreach (var item in productNolist)
            {
                str.Append(item + ",");
            }
            string productNos = str.ToString().TrimEnd(',');
            List<ProductInventory> list = new List<ProductInventory>();

            string xml = string.Empty;
            try
            {
                StockWebService.StockWebService stock = new StockWebService.StockWebService();
                if (AppSettingManager.AppSettings["InventoryFlag"].ToLower() == "true")
                {
                    stock.Url = AppSettingManager.AppSettings["ErpInventoryService"];
                    xml = stock.GetInventorySumByProductNos(productNos); //1,正品；2，残次品；3，赠品
                }
                else
                {
                    //xml = StockBusiness.GetInventorySumByProductNo(productNo, 1);//1,正品；2，残次品；3，赠品
                    xml = stock.GetInventorySumByProductNos(productNos);//1,正品；2，残次品；3，赠品
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
                            ProductInventory pinventory = new ProductInventory();
                            pinventory.ProductNo = inventoryNode.ChildNodes[1].InnerText;
                            pinventory.SumQuantity = sumQuantity;
                            pinventory.SumLockQuantity = sumLockQuantity;
                            pinventory.Inventory = inventory;
                            list.Add(pinventory);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return list;
        }


        /// <summary>
        ///  根据商品编号获取所有SKU库存 by lijibo 
        /// </summary>
        /// <param name="productNos"></param>
        /// <param name="NoSize"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetProductsInventoryNew(string productNos)
        {
            var reslut = new Dictionary<string, int>();
            string[] productNoArray = productNos.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var q = GetWfsSkuCollectionByProductNosNew(productNoArray);// DapperUtil.Query<WfsSku>("ComBeziWfs_WfsSku_SelectByProductNos", new { ProductNos = productNoArray }).ToList();
            var lst = new List<ProductInventoryInfo>();
            foreach (var wfsSku in q)
            {
                var product = new ProductInventoryInfo();
                product.SkuNo = wfsSku.SkuNo;
                product.ProductNo = wfsSku.ProductNo;
                lst.Add(product);
            }
            var skus =
                lst.Select(
                    p =>
                    new SkuDto { SkuNo = p.SkuNo, SkuType = p.SkuType, StockFlag = StockFlag.Normal, ProductNo = p.ProductNo }).
                    ToList();
            StockWebService.StockWebService stock = new StockWebService.StockWebService();
            var inv = GetSkuInventoryQuantity(skus);
            //  按商品编号进行分组
            var q2 = from p in inv
                     group p by p.ProductNo into g
                     select g;
            foreach (var gg in q2)
            {
                var productNo = gg.Select(x => x.ProductNo).FirstOrDefault();
                var inventory = gg.Sum(x => x.Quantity);
                if (productNo != null) { reslut.Add(productNo, inventory); }
            }
            return reslut;
        }

        /// <summary>
        /// 根据商品编号获取所有SKU
        /// </summary>
        /// <param name="productNoCollection">商品编号集合</param>
        /// <returns></returns>
        private IEnumerable<SpfSkuExtend> GetWfsSkuCollectionByProductNosNew(IEnumerable<string> productNoCollection)
        {
            string _skuCachePrefixKey = "ComBeziWfs_SpfSkuExtend_SelecSkuInfotByProductNo_Change";
            string _skulistId = "ComBeziWfs_SpfSkuExtend_ProductNoMappingSku_MissNo_Change";
            var q = new List<SpfSkuExtend>();

            var cacheKeys = productNoCollection.Select(item => string.Format("{0}{1}", _skuCachePrefixKey, item)).ToList();

            var cached = EnyimMemcachedClient.Instance.GetMultiple(cacheKeys);

            if (cached.Count(x => x.Value != null) == 0)
            {
                var q5 = DapperUtil.Query<SpfSkuExtend>("ComBeziWfs_SpfSkuExtend_GetProductSkuOutlet", new { ProductNo = productNoCollection.ToArray() }).ToList();
                //RedisCacheProvider.Instance.PushItemToList(_skulistId, string.Join(",", productNoCollection.ToArray()));
                return q5;
            }
            // 标明cache中部分命中
            // 1.找出未命中的部分，进行数据库查询，并把编号push到队列中
            q = new List<SpfSkuExtend>();

            foreach (var v in cached)
            {
                q.AddRange(v.Value as List<SpfSkuExtend>);
#if DEBUG
                DapperUtil.Insert(string.Format("sku缓存命中 [{0}]", v.Value));
#endif
            }

            if (cached.Count(x => x.Value != null) != productNoCollection.Count()) //部分命中
            {
                var missKeys = cacheKeys.Where(x => !cached.Keys.Contains(x)).Select(x => x.Replace(_skuCachePrefixKey, string.Empty));
                var q1 = DapperUtil.Query<SpfSkuExtend>("ComBeziWfs_SpfSkuExtend_GetProductSkuOutlet", new { ProductNo = missKeys.ToArray() }).ToList();
                //RedisCacheProvider.Instance.PushItemToList(_skulistId, string.Join(",", missKeys.ToArray()));
                q.AddRange(q1);
            }
            return q;
        }


        public IEnumerable<SkuDto> GetSkuInventoryQuantity(IEnumerable<SkuDto> skus, bool validSkuOnly = false)
        {
            if (skus == null)
                return null;

            if (!skus.Any())
                return skus;
            // clone

            foreach (var sku in skus)
            {
                string stockCacheKey = string.Format(_key, sku.SkuNo);

                sku.CacheKey = stockCacheKey;

            }

            var values = MemcachedProvider.Instance.GetMultiple(skus.Select(x => x.CacheKey).ToArray());
            //
            if (values != null && values.Count != 0)
            {
                skus.ToList().ForEach(x =>
                {
                    if (values[x.CacheKey] != null)
                    {
                        x.Quantity = ((StockCache)values[x.CacheKey]).Quantity;
                        x.IsCached = true;
                    }
                });
            }

            StringBuilder sb = new StringBuilder("["); //：["02236778001","02236778002"]
            foreach (var sku in skus)
            {
                if (sku.IsCached)
                    continue;
                sb.AppendFormat("\"{0}\",", sku.SkuNo);
            }
            string skusV = sb.ToString().TrimEnd(',');
            skusV += "]";

            if (skusV.Equals("[]"))
                return skus;

            object[] args2 = new object[] { skusV };

            string quanArray = _agent.Invoke("GetSkuInventoryBySkuNos", args2).ToString();


            quanArray = quanArray.Trim(new[] { '{', '}' });
            var array1 = quanArray.Split(',');
            foreach (var sku in skus)
            {
                if (sku.IsCached)
                    continue;
                var coll = array1.FirstOrDefault(x => x.IndexOf(sku.SkuNo, System.StringComparison.Ordinal) > 0);
                if (coll != null)
                {
                    var q = int.Parse(coll.Substring(coll.IndexOf(":", System.StringComparison.Ordinal) + 1));
                    sku.Quantity = q;
                }


            }

            return skus;
        }


        public ProductInventory GetInventoryByProductNo(string productNo)
        {
            ProductInventory pinventory = new ProductInventory();
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
        /// 根据商品编号获得其所在活动
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public IList<SWfsSubjectCategory> GetSubjectCategoryByProductNo(string productNo, string status)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Status", status == null ? "" : status);
            return DapperUtil.Query<SWfsSubjectCategory>("ComBeziWfs_SWfsSubjectCategory_SelectSubjectCategoryListByProductNo", dic, new { ProductNo = productNo }).ToList();
        }

        /// <summary>
        /// 批量查询产品所属有效的活动
        /// </summary>
        /// <param name="productNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Dictionary<string, IList<SWfsSubjectCategoryII>> GetSubjectCategoryByProductNoII(string[] productNo, string status)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Status", status == null ? "" : status);
            IList<SWfsSubjectCategoryII> list = DapperUtil.Query<SWfsSubjectCategoryII>("ComBeziWfs_SWfsSubjectCategory_SelectSubjectCategoryListByProductNoII", dic, new { ProductNo = productNo }).ToList();

            Dictionary<string, IList<SWfsSubjectCategoryII>> dicRes = new Dictionary<string, IList<SWfsSubjectCategoryII>>();
            foreach (var item in productNo)
            {
                if (!dicRes.ContainsKey(item))
                {
                    IList<SWfsSubjectCategoryII> tempList = (from l in list where l.ProductNo.Trim().Equals(item) select l).ToList();
                    dicRes.Add(item, tempList);
                }
            }
            return dicRes;
        }

        /// <summary>
        /// 根据商品编号所得其所在专题
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public IList<SWfsTopicProductRef> GetTopicProductByProductNo(string productNo)
        {
            return DapperUtil.Query<SWfsTopicProductRef>("ComBeziWfs_SWfsTopicProductRef_SelectTopic", new { ProductNo = productNo }).ToList();
        }

        /// <summary>
        /// 根据商品编号和子类编号获得信息
        /// </summary>
        /// <param name="productNo"></param>
        /// <param name="categoryNo"></param>
        /// <returns></returns>
        public SWfsSubjectProductRef GetSubjectProduct(int refId)
        {
            return DapperUtil.QueryByIdentity<SWfsSubjectProductRef>(refId);
            //return DapperUtil.Query<SWfsSubjectProductRef>("ComBeziWfs_SWfsSubjectProductRef_SelectByProNoAndCategoryNo", new { ProductNo = productNo, CategoryNo = categoryNo }).FirstOrDefault();
        }

        /// <summary>
        /// 查询商品的购买价
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public decimal GetProductBuyPriceByProductNo(string productNo)
        {
            com.shangpin.erpws02.ToWfsWebService ws = new com.shangpin.erpws02.ToWfsWebService();
            ws.Url = AppSettingManager.AppSettings["ErpWSProductBuyPriceService"];
            string xml = ws.GetProductBuyPriceByProductNo(productNo);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string bid = doc.SelectSingleNode("ErpForWfsWebServiceResult/BuyPrice").InnerText;
            return Convert.ToDecimal(bid);

        }

        #region 20131010根据商品编号数组 批量获取所在活动，专题信息
        /// <summary>
        /// 根据商品编号获得其所在活动
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public Dictionary<string, IList<SWfsSubjectCategory>> GetSubjectCategoryByProductNoList(Array productNoList, string status)
        {
            var dic = new Dictionary<string, IList<SWfsSubjectCategory>>();
            var list = DapperUtil.Query<SWfsSubjectCategory>("ComBeziWfs_SWfsSubjectCategory_SelectSubjectCategoryListByProductNoList", new { ProductNoList = productNoList }).ToList();
            return dic;
        }

        /// <summary>
        /// 根据商品编号所得其所在专题
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public Dictionary<string, IList<SWfsTopicProductRef>> GetTopicProductByProductNoList(Array productNolist)
        {
            var dic = new Dictionary<string, IList<SWfsTopicProductRef>>();
            var list = DapperUtil.Query<SWfsTopicProductRef>("ComBeziWfs_SWfsTopicProductRef_SelectTopic", new { ProductNoList = productNolist }).ToList();
            return dic;
        }
        #endregion


    }
}
