using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Service.Common;
using Com.ShangPin.WebService.Stock;
using System.Xml;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;

namespace Shangpin.Ocs.Service.Shangpin
{
   public class SWfsNewProductService
    {
        public ProductInventoryNew GetInventoryByProductNo(string productNo)
        {
            ProductInventoryNew pinventory = new ProductInventoryNew();
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

                                sumQuantity += inventoryQuantity;
                                sumLockQuantity += lockQuantity;
                            }
                        }
                        //sumQuantity += inventoryQuantity;
                        //sumLockQuantity += lockQuantity;
                        inventory = inventory + inventoryQuantity - lockQuantity;
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


       ///// <summary>
       ///// 测试
       ///// </summary>
       ///// <param name="productNos"></param>
       ///// <returns></returns>
       //public string GetInventoryProductNosByProductNos(string productNos)
       //{
       //    string newProductNos = string.Empty;
       //     newProductNos = "13237769,12237374,12237350 ";
       //    StockWebService.StockWebService stock = new StockWebService.StockWebService();
       //    stock.Url = AppSettingManager.AppSettings["ErpInventoryService"];
       //    string aa = stock.GetInventorySumByProductNos(newProductNos);


       //    return newProductNos;
       //}




       /// <summary>
        /// 根据商品编号获得其所在活动
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public IList<SWfsNewSubjectCategory> GetSubjectCategoryByProductNo(string productNo, string status)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Status", status == null ? "" : status);
            return DapperUtil.Query<SWfsNewSubjectCategory>("ComBeziWfs_SWfsNewSubjectCategory_SelectSubjectCategoryListByProductNo", dic, new { ProductNo = productNo }).ToList();
        }
        /// <summary>
        /// 根据商品编号获得商品列表
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public IList<SWfsNewSubjectProductRef> GetSWfsNewSubjectProductRefByProductNo(string productNo)
        {
            return DapperUtil.Query<SWfsNewSubjectProductRef>("ComBeziWfs_SWfsNewSubjectProductRef_SelectSubjectProductRefListByProductNo", new { ProductNo = productNo }).ToList();
        }
        /// <summary>
        /// 根据商品编号获得其所在活动
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public IList<SWfsNewSubject> GetSubjectBySubjectProductRefId(int subjectProductRefId)
        {
            return DapperUtil.Query<SWfsNewSubject>("ComBeziWfs_SWfsNewSubject_SelectSubjectListBySubjectProductRefId", new { SubjectProductRefId = subjectProductRefId }).ToList();
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
        public SWfsNewSubjectProductRef GetSubjectProduct(int refId)
        {
            return DapperUtil.QueryByIdentity<SWfsNewSubjectProductRef>(refId);
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
            string xml = ws.GetProductBuyPriceByProductNo(productNo);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string bid = doc.SelectSingleNode("ErpForWfsWebServiceResult/BuyPrice").InnerText;
            return Convert.ToDecimal(bid);

        }

       /// <summary>
       /// 获取商品库龄
       /// </summary>
       /// <param name="productNo">多个用，号隔开</param>
       /// <returns></returns>
        public IList<ProductAgeing> GetErpProductAgeingList(string productNo)
        {
            List<ProductAgeing> pAgeingList = new List<ProductAgeing>();
            string xml = string.Empty;
            try
            {
                com.shangpin.erpws02.ToWfsWebService ws = new com.shangpin.erpws02.ToWfsWebService();
                xml = ws.GetInventoryAgeByProductNos(productNo);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml); 
                if (doc != null)
                {
                    XmlNodeList inventoryNodeList = doc.SelectNodes("/Inventorys/Inventory");
                    if (inventoryNodeList != null && inventoryNodeList.Count > 0)
                    { 
                        foreach (XmlNode inventoryNode in inventoryNodeList)
                        {
                            if (inventoryNode.ChildNodes.Count > 0)
                            {
                                ProductAgeing pageSingle = new ProductAgeing();
                                pageSingle.ProductNo = inventoryNode.ChildNodes[0].InnerText;
                                pageSingle.DateReceiving =DateTime.Parse(inventoryNode.ChildNodes[1].InnerText);
                                pAgeingList.Add(pageSingle);
                            }
                        } 
                    }
                }  

            }
            catch (Exception ex)
            {

            }
            return pAgeingList;
        }


        /// <summary>
        /// 获取商品库龄
        /// </summary>
        /// <param name="productNo">多个用，号隔开</param>
        /// <returns></returns>
        public static string GetErpProductAgeingSingle(string productNo)
        {
            ProductAgeing pageSingle = new ProductAgeing();
            string xml = string.Empty;
            string daysShow = string.Empty;
            try
            {
                com.shangpin.erpws02.ToWfsWebService ws = new com.shangpin.erpws02.ToWfsWebService();
                xml = ws.GetInventoryAgeByProductNos(productNo);
                if (!string.IsNullOrEmpty(xml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    if (doc != null)
                    {
                        XmlNodeList inventoryNodeList = doc.SelectNodes("/Inventorys/Inventory");
                        if (inventoryNodeList != null && inventoryNodeList.Count > 0)
                        {
                            XmlNode inventoryNode = inventoryNodeList[0];
                            if (inventoryNode.ChildNodes.Count > 0)
                            {
                                pageSingle.ProductNo = inventoryNode.ChildNodes[0].InnerText;
                                pageSingle.DateReceiving = DateTime.Parse(inventoryNode.ChildNodes[1].InnerText);
                                daysShow = SWfsNewSubjectService.GetShowDays(pageSingle.DateReceiving, DateTime.Now);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               

            }
            return daysShow;
        }
    }
}
