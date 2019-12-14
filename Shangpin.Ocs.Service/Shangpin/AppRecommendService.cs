using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ProductFlat;
using Shangpin.Ocs.Service.Common;
using System.Xml;
using System.Reflection;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class AppRecommendService
    {

        public XMLReturnClassLists GetLists(Parameters p)
        {
            string url = GetListUrl(p);
            string xmlstr = GetResponse(url);
            return XmlParsing(xmlstr);
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private string GetResponse(string url, string encoding = "")
        {
            string result;
            try
            {
                //创建
                var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                httpWebRequest.KeepAlive = false;
                //访问
                var httpWebResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                Encoding coding = string.IsNullOrEmpty(encoding) ? Encoding.UTF8 : Encoding.GetEncoding(encoding);
                System.IO.StreamReader sr;
                using (sr = new System.IO.StreamReader(httpWebResponse.GetResponseStream(), coding))
                {
                    result = sr.ReadToEnd();
                }
                //关闭
                httpWebResponse.Close();
                httpWebRequest.Abort();
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception("搜索接口地址" + url + "返回数据错误");
            }
            //返回
            return result;
        }
        /// <summary>
        /// //根据条件拼接url地址
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string GetListUrl(Parameters p)
        {
            System.Text.StringBuilder str = new StringBuilder();
            str.Append(AppSettingManager.AppSettings["SearchInterFaceUrl"]);
            str.Append("/spcms/AppRecommendList?userID=a23j3121112&userIP=127.0.0.1&encode=UTF-8");
            if (p.productNO != null && p.productNO != "")
            {
                str.Append("&productNO=" + p.productNO);
            }
            if (p.productName != null && p.productName != "")
            {
                str.Append("&productName=" + p.productName);
            }
            if (p.categoryNO != null && p.categoryNO != "")
            {
                str.Append("&categoryNO=" + p.categoryNO);
            }
            if (p.brandNO != null && p.brandNO != "")
            {
                str.Append("&brandNO=" + p.brandNO);
            }
            if (p.colorId != null && p.colorId != "")
            {
                str.Append("&colorId=" + p.colorId);
            }
            if (p.shelfDate != null && p.shelfDate != "")
            {
                str.Append("&shelfDate=" + p.shelfDate);
            }
            if (p.postArea != null && p.postArea != "")
            {
                str.Append("&postArea=" + p.postArea);
            }
            if (p.price != null && p.price != "")
            {
                str.Append("&price=" + p.price);
            }
            if (p.stock != null && p.stock != "")
            {
                str.Append("&stock=" + p.stock);
            }
            if (p.discountRate != null && p.discountRate != "")
            {
                str.Append("&discountRate=" + p.discountRate);
            }
            if (p.hot != null && p.hot != "")
            {
                str.Append("&hot=" + p.hot);
            }
            if (p.sevenHot != null && p.sevenHot != "")
            {
                str.Append("&sevenHot=" + p.sevenHot);
            }
            if (p.start != null && p.start != "")
            {
                str.Append("&start=" + p.start);
            }
            else
            {//为空的话默认1
                str.Append("&start=1");
            }
            if (p.end != null && p.end != "")
            {
                str.Append("&end=" + p.end);
            }
            else
            {//为空的话默认10
                str.Append("&end=10");
            }
            return str.ToString();
        }

        /// <summary>
        /// 查询条件的描述
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string SelectDirection(Parameters p)
        {
            System.Text.StringBuilder str = new StringBuilder();
            if (p.productNO != null && p.productNO != "")
            {
                str.Append(p.productNO + ">");
            }
            if (p.productName != null && p.productName != "")
            {
                str.Append(System.Web.HttpUtility.UrlDecode(p.productName) + ">");
            }
            if (p.BrandName != null && p.BrandName != "")
            {
                str.Append(p.BrandName + ">");
            }
            if (p.OCSCategoryName != null && p.OCSCategoryName != "")
            {
                str.Append(p.OCSCategoryName + ">");
            }
            if (p.ColorName != null && p.ColorName != "")
            {
                str.Append(p.ColorName + ">");
            }
            if (p.shelfType != null && p.shelfType != "")
            {
                if (p.shelfType == "0")
                {
                    str.Append("新上架>");
                }
                else
                {
                    str.Append(p.StartShelfDate + "-" + p.EndShelfDate);
                }
            }
            if (p.postArea != null && p.postArea != "")
            {
                if (p.postArea == "1")
                {
                    str.Append("大陆商品>");
                }
                else if (p.postArea == "2")
                {
                    str.Append("海外商品>");
                }
            }
            if (p.StartPrice != null && p.StartPrice != "")
            {
                str.Append("价格" + p.StartPrice + "-" + p.EndPrice + ">");
            }
            if (p.StartStock != null && p.StartStock != "")
            {
                str.Append("库存" + p.StartStock + "-" + p.EndStock + ">");
            }
            if (p.StartDiscountRate != null && p.StartDiscountRate != "")
            {
                str.Append("折扣" + p.StartDiscountRate + "-" + p.EndDiscountRate + ">");
            }

            return str.ToString().TrimEnd('>');
        }

        /// <summary>
        /// 解析分类、色系节点
        /// </summary>
        /// <param name="xn">分类、色系节点</param>
        /// <returns></returns>
        private string PraseCategoryOrColor(XmlNode xn)
        {
            string ID = "";
            XmlNodeList xnl = xn.ChildNodes;
            StringBuilder sb = new StringBuilder();
            foreach (var key in xnl)
            {
                string val = ((XmlElement)key).InnerText;//获取节点值
                string[] arr = val.Split('|');//分隔参数
                if (arr.Length > 0)
                {
                    sb.Append("," + arr[0]);//加入编号
                }
            }
            ID = sb.ToString();
            if (!string.IsNullOrEmpty(ID))
            {
                ID = ID.Substring(1);//去掉第一个逗号
            }
            return ID;
        }

        /// <summary>
        /// 解析xml
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <param name="docCount">返回总条数</param>
        /// <returns>返回所有集合对象</returns>
        private XMLReturnClassLists XmlParsing(string xmlStr)
        {
            XMLReturnClassLists xmlclass = new XMLReturnClassLists();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.InnerXml = xmlStr;
            int Status = int.Parse(xmlDoc.SelectSingleNode("/result/Total").InnerText);//获取总查询条数
            xmlclass.docCount = Status;
            if (Status == 0)//没有查询到数据
            {
                return xmlclass;
            }
            List<SearchResultBrands> listBrand = new List<SearchResultBrands>();
            List<SearchResultCategorys> listCategory = new List<SearchResultCategorys>();
            List<ProductPrimaryColors> listColor = new List<ProductPrimaryColors>();
            List<InterfaceProductInfo> listprod = new List<InterfaceProductInfo>();
            XmlNodeList docs_nodeList = xmlDoc.SelectNodes("/result/docs/doc");
            foreach (XmlNode item_doc in docs_nodeList)
            {
                InterfaceProductInfo prod = new InterfaceProductInfo();
                foreach (XmlNode item_field in item_doc.ChildNodes)
                {
                    switch (item_field.Attributes["name"].Value)
                    {
                        case "ProductNo":
                            prod.ProductNo = item_field.InnerText;
                            break;
                        case "ProductName":
                            prod.ProductName = item_field.InnerText;
                            break;
                        case "MarketPrice":
                            prod.MarketPrice = decimal.Parse(string.IsNullOrEmpty(item_field.InnerText) ? "0" : item_field.InnerText);
                            break;
                        case "LimitedPrice":
                            prod.LimitedPrice = decimal.Parse(string.IsNullOrEmpty(item_field.InnerText) ? "0" : item_field.InnerText);
                            break;
                        case "PromotionPrice":
                            prod.PromotionPrice = decimal.Parse(string.IsNullOrEmpty(item_field.InnerText) ? "0" : item_field.InnerText);
                            break;
                        case "ProductPicFile":
                            prod.ProductPicFile = item_field.InnerText;
                            break;
                        case "GoodsNo":
                            prod.GoodsNo = item_field.InnerText;
                            break;
                        case "DiscountRate":
                            prod.DiscountRate = item_field.InnerText;
                            break;
                        case "BrandNo":
                            prod.BrandNo = item_field.InnerText;
                            break;
                        case "BrandCnName":
                            if (item_field.InnerText != null && item_field.InnerText != "")
                            {
                                prod.BrandCnName = item_field.InnerText;
                            }
                            break;
                        case "BrandEnName":
                            prod.BrandEnName = item_field.InnerText;
                            break;
                        case "DateShelf":
                            prod.DateShelf = item_field.InnerText;
                            break;
                        case "Stock":
                            prod.stock = int.Parse(item_field.InnerText);
                            break;
                        case "Category":
                            for (int i = 0; i < item_field.ChildNodes.Count; i++)
                            {
                                if (i == item_field.ChildNodes.Count - 1)
                                {
                                    prod.Category = item_field.ChildNodes[i].InnerText.Split('|')[1];
                                }
                                else
                                {
                                    prod.Category = item_field.ChildNodes[i].InnerText.Split('|')[1] + "|";
                                }
                            }
                            break;
                        case "ProductPrimaryColor":
                            for (int i = 0; i < item_field.ChildNodes.Count; i++)
                            {
                                if (i == item_field.ChildNodes.Count - 1)
                                {
                                    prod.ProductPrimaryColor = prod.ProductPrimaryColor + item_field.ChildNodes[i].InnerText.Split('|')[0];
                                    prod.ProductPrimaryColorName = prod.ProductPrimaryColorName + item_field.ChildNodes[i].InnerText.Split('|')[1];
                                }
                                else
                                {
                                    prod.ProductPrimaryColor = prod.ProductPrimaryColor + item_field.ChildNodes[i].InnerText.Split('|')[0] + "|";
                                    prod.ProductPrimaryColorName = prod.ProductPrimaryColorName + item_field.ChildNodes[i].InnerText.Split('|')[1] + "|";
                                }
                            }
                            break;
                        case "Hot":
                            prod.Hot = !string.IsNullOrEmpty(item_field.InnerText)?long.Parse(item_field.InnerText):0;
                            break;
                        case "SevenHot":
                            prod.SevenHot = !string.IsNullOrEmpty(item_field.InnerText) ? long.Parse(item_field.InnerText) : 0;
                            break;
                    }

                }
                listprod.Add(prod);
            }

            XmlNodeList facets_nodeList = xmlDoc.SelectNodes("/result/facets");
            if (facets_nodeList != null && facets_nodeList.Count > 0)
            {
                foreach (XmlNode item_facets in facets_nodeList)
                {
                    foreach (XmlNode fList in item_facets.ChildNodes)//节点facet
                    {
                        if (fList.Attributes["name"].Value == "Brand") //取name=brand的子节点
                        {
                            XmlNodeList brandsXMl = fList.ChildNodes;//获取item节点
                            string brandString = "";
                            string[] attribute = null;
                            foreach (XmlNode item_brand in brandsXMl)
                            {
                                SearchResultBrands brand = new SearchResultBrands();
                                brandString = item_brand.Attributes["name"].Value;
                                brand.ProductCount = int.Parse(item_brand.InnerText);
                                attribute = brandString.Split('|');
                                brand.BrandNO = attribute[0];
                                brand.BrandEnName = attribute[1];
                                brand.BrandChName = attribute[2];
                                listBrand.Add(brand);
                            }
                        }
                        if (fList.Attributes["name"].Value == "ProductPrimaryColors")
                        {
                            XmlNodeList ColorsXMl = fList.ChildNodes;//获取item节点
                            string colorString = "";
                            string[] attribute = null;
                            foreach (XmlNode item_color in ColorsXMl)
                            {
                                ProductPrimaryColors color = new ProductPrimaryColors();
                                colorString = item_color.Attributes["name"].Value;
                                color.ColorProductCount = int.Parse(item_color.InnerText);
                                attribute = colorString.Split('|');
                                color.ColorNO = attribute[0];
                                color.ColorName = attribute[1];
                                color.ColorPicFile = attribute[2];
                                listColor.Add(color);
                            }
                        }
                        if (fList.Attributes["name"].Value == "CLv2")//二级分类
                        {
                            XmlNodeList CategoryXMl = fList.ChildNodes;//获取item节点
                            string categoryString = "";
                            string[] attribute = null;
                            foreach (XmlNode item_category in CategoryXMl)
                            {
                                SearchResultCategorys cagegory = new SearchResultCategorys();
                                categoryString = item_category.Attributes["name"].Value;
                                cagegory.CategoryProductCount = int.Parse(item_category.InnerText);
                                attribute = categoryString.Split('|');
                                cagegory.CategoryNo = attribute[0];
                                cagegory.CateGoryName = attribute[1];
                                cagegory.PrentNo = attribute[2];
                                cagegory.State = int.Parse(attribute[3]);
                                cagegory.CategorySort = int.Parse(attribute[4]);
                                cagegory.CateGoryLevel = 2;
                                listCategory.Add(cagegory);

                            }
                        }
                        if (fList.Attributes["name"].Value == "CLv3")//二级分类
                        {
                            XmlNodeList CategoryXMl = fList.ChildNodes;//获取item节点
                            string categoryString = "";
                            string[] attribute = null;
                            foreach (XmlNode item_category in CategoryXMl)
                            {
                                SearchResultCategorys cagegory = new SearchResultCategorys();
                                categoryString = item_category.Attributes["name"].Value;
                                cagegory.CategoryProductCount = int.Parse(item_category.InnerText);
                                attribute = categoryString.Split('|');
                                cagegory.CategoryNo = attribute[0];
                                cagegory.CateGoryName = attribute[1];
                                cagegory.PrentNo = attribute[2];
                                cagegory.State = int.Parse(attribute[3]);
                                cagegory.CategorySort = int.Parse(attribute[4]);
                                cagegory.CateGoryLevel = 3;
                                listCategory.Add(cagegory);

                            }
                        }
                        if (fList.Attributes["name"].Value == "CLv4")//二级分类
                        {
                            XmlNodeList CategoryXMl = fList.ChildNodes;//获取item节点
                            string categoryString = "";
                            string[] attribute = null;
                            foreach (XmlNode item_category in CategoryXMl)
                            {
                                SearchResultCategorys cagegory = new SearchResultCategorys();
                                categoryString = item_category.Attributes["name"].Value;
                                cagegory.CategoryProductCount = int.Parse(item_category.InnerText);
                                attribute = categoryString.Split('|');
                                cagegory.CategoryNo = attribute[0];
                                cagegory.CateGoryName = attribute[1];
                                cagegory.PrentNo = attribute[2];
                                cagegory.State = int.Parse(attribute[3]);
                                cagegory.CategorySort = int.Parse(attribute[4]);
                                cagegory.CateGoryLevel = 4;
                                listCategory.Add(cagegory);

                            }
                        }

                    }
                }
            }

            xmlclass.ListBrands = listBrand.OrderBy(p => p.BrandEnName).ToList();
            xmlclass.ListCategorys = listCategory;
            xmlclass.ListProducts = listprod;
            xmlclass.ListColors = listColor;
            return xmlclass;
        }


        /// <summary>
        /// 将LIST对象转换成JSON各式字符窜
        /// </summary>
        /// <param name="List">LIST对象</param>
        /// <returns></returns>
        public string ListToJsonString<T>(List<T> List)
        {
            if (List.Count < 1)
            {
                return "[]";
            }
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] piList = List[0].GetType().GetProperties();
            foreach (var item in List)
            {
                sb.Append(",{");
                StringBuilder sbTmp = new StringBuilder();
                //遍历元素并拼接元素属性及属性值
                foreach (var Propery in piList)
                {
                    sbTmp.Append(",\"" + Propery.Name + "\":\"" + Propery.GetValue(item, null) + "\"");
                }
                sb.Append(sbTmp.ToString().Substring(1) + "}");
            }
            return "[" + sb.ToString().Substring(1) + "]";
        }

        /// <summary>
        /// 查询App推荐产品
        /// </summary>
        /// <returns></returns>
        public List<AppRecommendProductModle> GetAppRecommendProductList()
        {
            List<AppRecommendProductModle> list = new List<AppRecommendProductModle>();
            list = DapperUtil.Query<AppRecommendProductModle>("ComBeziWfs_SWfsAppRecommendProduct_SelectAllInfo").ToList();
            return list;
        }

        /// <summary>
        /// 添加App推荐产品
        /// </summary>
        /// <param name="DTO"></param>
        /// <returns></returns>
        public int AddAppRecommendProduct(AppRecommendProductModle DTO)
        {
            int result = 0;
            var IsExit = DapperUtil.Query<AppRecommendProductModle>("ComBeziWfs_SWfsAppRecommendProduct_SelectInfoByProductNo", new { ProductNo = DTO.ProductNo }).ToList();
            if (IsExit.Count() == 0)
            {
                result = DapperUtil.Execute("ComBeziWfs_SWfsAppRecommendProduct_InsertInfo", new { ProductNo = DTO.ProductNo, SortId = "0", CreateTime = DateTime.Now, Creator = DTO.Creator });

            }
            return result;
        }

        public int DelAppRecommendProductById(string Id)
        {
            int result = 0;
            DapperUtil.Execute("ComBeziWfs_SWfsAppRecommendProduct_deleteInfoById", new { ID = Id });
            return result;
        }
    }
}
