using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System.Xml;
using Shangpin.Ocs.Service.Shangpin;

namespace Shangpin.Ocs.Service.Outlet
{
    public class OutLetExtendService
    {

        /// <summary>
        /// 获取商品库龄
        /// </summary>
        /// <param name="productNo">多个用，号隔开</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetErpProductAgeingMulter(List<string> productNoList)
        {
            Dictionary<string, string> dicResult = new Dictionary<string, string>();
            StringBuilder str = new StringBuilder();
            foreach (var item in productNoList)
            {
                str.Append(item + ",");
            }

            ProductAgeing pageSingle = new ProductAgeing();
            string xml = string.Empty;
            string daysShow = string.Empty;
            try
            {
                string tmpProductNo = str.ToString().TrimEnd(',');
                com.shangpin.erpws02.ToWfsWebService ws = new com.shangpin.erpws02.ToWfsWebService();
                xml = ws.GetInventoryAgeByProductNos(tmpProductNo);
                if (!string.IsNullOrEmpty(xml))
                {
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
                                    pageSingle.ProductNo = inventoryNode.ChildNodes[0].InnerText;
                                    pageSingle.DateReceiving = DateTime.Parse(inventoryNode.ChildNodes[1].InnerText);
                                    daysShow = SWfsNewSubjectService.GetShowDays(pageSingle.DateReceiving, DateTime.Now);
                                    dicResult.Add(inventoryNode.ChildNodes[0].InnerText, daysShow);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
            return dicResult;
        }

    }
}
