using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Service.Common;
using System.Xml;
using Shangpin.Ocs.Entity.Extenstion.ProductFlat;
using System.Xml.Serialization;
using System.Collections;


namespace Shangpin.Ocs.Service.Shangpin.ProductSort
{
    public class ProductSortService
    {
        //价格区间
        public static readonly string[] priceRegion = AppSettingManager.AppSettings["priceRegionNew"].Split(',');

        #region 搜索列表页
        //按商品编号查询以保存的商品 判断是否加入排序
        public SWfsSortProduct GetSortProductByProductNo(string productNo, string ocsCategoryNo)
        {
            return DapperUtil.Query<SWfsSortProduct>("ComBeziWfs_SWfsSortProduct_GetSorProductByProductNo", new
            {
                ProductNo = productNo,
                OcsCategoryNo = ocsCategoryNo
            }).FirstOrDefault();
        }
        //通过ocs分类和查询加入商品排序池的 商品编号
        public IEnumerable<SWfsSortProduct> GetSortProductList(string ocsCategoryNo)
        {
            IEnumerable<SWfsSortProduct> list = DapperUtil.Query<SWfsSortProduct>("ComBeziWfs_SWfsSortProduct_GetSorProductList", new
            {
                OcsCategoryNo = ocsCategoryNo,
            });
            if (list.Count() > 0)
            {
                list = list.OrderBy(p => p.Sort);
            }
            return list;
        }

        //流浪搜索页面
        public SearchProductInfo SearchProductList(Parameters p)
        {
            SearchProductInfo objss = new SearchProductInfo();
            List<SortProduct> xmlProductList = new List<SortProduct>();//保存搜索接口返回的商品
            string xmlText = GetResponse(GetSearchListUrl(p));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.InnerXml = xmlText;
            XmlNodeList nodeList = xmlDoc.SelectNodes("/result/facets/facet");
            if (nodeList != null && nodeList.Count > 0)
            {
                short ruleType = 0;
                foreach (XmlNode item1 in nodeList)
                {
                    SortRule obj = new SortRule();
                    switch (item1.Attributes[0].InnerText)
                    {
                        case "CLv4":
                            ruleType = 1;
                            for (int i = 0; i < item1.ChildNodes.Count; i++)
                            {
                                if (i == item1.ChildNodes.Count - 1)
                                {
                                    obj.RuleObjectNo += item1.ChildNodes[i].Attributes[0].InnerText.Split('|')[0];
                                    obj.RuleObjectName = item1.ChildNodes[i].Attributes[0].InnerText.Split('|')[1];
                                    obj.RuleType = 1;
                                }
                                else
                                {
                                    obj.RuleObjectNo += item1.ChildNodes[i].Attributes[0].InnerText.Split('|')[0] + ",";
                                    obj.RuleObjectName = item1.ChildNodes[i].Attributes[0].InnerText.Split('|')[1] + ",";
                                }
                            }
                            break;
                        case "Brand":
                            ruleType = 2;
                            break;
                        case "ProductPrimaryColors":
                            ruleType = 3;
                            break;
                    }

                }
            }
            return objss;
        }
        #endregion

        #region 排序规则生成商品顺序
        //查询规则
        private IEnumerable<SortRule> GetRuleListByCategoryNo(string ocsCategoryNo)
        {
            return DapperUtil.Query<SortRule>("ComBeziWfs_SWfsSortRule_GetSortRuleByCategoryNo", new
            {
                OcsCategoryNo = ocsCategoryNo,
            });
        }
        //对用户保存的排序规则 进行排序
        private List<SortRule> GetOrderSorRule(IEnumerable<SortRule> ruleList)
        {
            List<SortRule> newRuleList = new List<SortRule>();
            if (ruleList.Count() > 0)
            {
                List<SortRule> firstRule = ruleList.Where(p => p.ParentId == 0).OrderByDescending(p => p.Sort).ToList();//一级规则排序
                for (int i = 0; i < firstRule.Count(); i++)//一级规则基础上 对二级规则排序
                {
                    if (ruleList.Count(p => p.ParentId == firstRule.ElementAt(i).RuleId) > 0)//是否有二级规则
                    {
                        newRuleList.AddRange(ruleList.Where(p => p.ParentId == firstRule.ElementAt(i).RuleId).OrderByDescending(p => p.Sort));
                    }
                    else
                    {
                        newRuleList.Add(firstRule.ElementAt(i));
                    }

                }
            }
            return newRuleList;
        }
        //获取一级规则下的所有产品
        private List<SortProduct> GetFirstRuleProduct(SortRule firstRule, List<SortProduct> allProduct, string[] PriceRegion)
        {
            List<SortProduct> templist = new List<SortProduct>();
            switch (firstRule.RuleType)
            {
                case 1:
                    templist.AddRange(allProduct.Where(p => p.CategoryNo == firstRule.RuleObjectNo).ToList());
                    break;
                case 2:
                    templist.AddRange(allProduct.Where(p => p.BrandNo == firstRule.RuleObjectNo).ToList());
                    break;
                case 3:
                    templist.AddRange(allProduct.Where(p => p.ProductPrimaryColorNO == firstRule.RuleObjectNo).ToList());
                    break;
                case 4:
                    templist.AddRange(allProduct.Where(p => p.PriceNo == firstRule.RuleObjectNo).ToList());
                    //switch (firstRule.RuleObjectNo)
                    //{
                    //    case "1":
                    //        templist.AddRange(allProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[0].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[0].Split('-')[1])).ToList());

                    //        break;
                    //    case "2":
                    //        templist.AddRange(allProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[1].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[1].Split('-')[1])).ToList());

                    //        break;
                    //    case "3":
                    //        templist.AddRange(allProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[2].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[2].Split('-')[1])).ToList());

                    //        break;
                    //    case "4":
                    //        templist.AddRange(allProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[3].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[3].Split('-')[1])).ToList());

                    //        break;
                    //}
                    break;
                case 6://一级规则的其他商品
                    templist.AddRange(allProduct.Where(p => p.RuleId == firstRule.RuleId).ToList());
                    break;
                default:
                    templist.AddRange(allProduct);
                    break;
            }
            return templist;
        }
        private List<SortProduct> CreateProductListByFirstRule(SortRule firstRule, List<SortProduct> allProduct, string[] PriceRegion)
        {
            List<SortProduct> templist = new List<SortProduct>();
            switch (firstRule.RuleType)
            {
                case 1:
                    templist = allProduct.Where(p => p.CategoryNo == firstRule.RuleObjectNo).ToList();
                    break;
                case 2:
                    templist = allProduct.Where(p => p.BrandNo == firstRule.RuleObjectNo).ToList();
                    break;
                case 3:
                    templist = allProduct.Where(p => p.ProductPrimaryColorNO == firstRule.RuleObjectNo).ToList();
                    break;
                case 4:
                    templist = allProduct.Where(p => p.PriceNo == firstRule.RuleObjectNo).ToList();
                    //switch (firstRule.RuleObjectNo)
                    //{
                    //    case "1":
                    //        templist = allProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[0].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[0].Split('-')[1])).ToList();

                    //        break;
                    //    case "2":
                    //        templist = allProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[1].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[1].Split('-')[1])).ToList();

                    //        break;
                    //    case "3":
                    //        templist = allProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[2].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[2].Split('-')[1])).ToList();

                    //        break;
                    //    case "4":
                    //        templist = allProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[3].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[3].Split('-')[1])).ToList();

                    //        break;
                    //}
                    break;
                case 6://一级规则的其他商品
                    templist = allProduct.Where(p => p.RuleId == firstRule.RuleId).ToList();
                    break;
                default:
                    templist = allProduct;
                    break;
            }
            templist.ForEach(p =>
            {
                if (p.RuleId != firstRule.RuleId)//判断erp是否修改过分类
                {
                    p.sort = -1;
                }
                p.RuleId = firstRule.RuleId;
                p.RuleType = firstRule.RuleType;
                p.RuleNo = firstRule.RuleObjectNo;
                p.RuleName = firstRule.RuleObjectName;
            });
            firstRule.ProductCount = templist.Count();
            return templist.OrderByDescending(p => p.sort).ToList();
        }
        //按二级规则 对 一级规则下的商品 进行二次 排序
        private List<SortProduct> CreateProductListBySecondRule(List<SortRule> SecondRule, List<SortProduct> firstOrderRuleProduct, string[] PriceRegion)
        {
            List<SortProduct> resultList = new List<SortProduct>();
            List<SortProduct> tempOrderProduct = new List<SortProduct>();
            //通过二级规则 然后对商品 firstOrderRuleProduct 进行二次排序
            for (int j = 0; j < SecondRule.Count; j++)
            {
                switch (SecondRule[j].RuleType)
                {
                    case 1://分类
                        tempOrderProduct = firstOrderRuleProduct.Where(p => p.CategoryNo == SecondRule[j].RuleObjectNo).ToList();
                        break;
                    case 2://品牌
                        tempOrderProduct = firstOrderRuleProduct.Where(p => p.BrandNo == SecondRule[j].RuleObjectNo).ToList();
                        break;
                    case 3://色系
                        tempOrderProduct = firstOrderRuleProduct.Where(p => p.ProductPrimaryColorNO == SecondRule[j].RuleObjectNo).ToList();
                        break;
                    case 4://价格
                        tempOrderProduct = firstOrderRuleProduct.Where(p => p.PriceNo == SecondRule[j].RuleObjectNo).ToList();
                        //switch (SecondRule[j].RuleObjectNo)
                        //{
                        //    case "1":
                        //        tempOrderProduct = firstOrderRuleProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[0].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[0].Split('-')[1])).ToList();
                        //        break;
                        //    case "2":
                        //        tempOrderProduct = firstOrderRuleProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[1].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[1].Split('-')[1])).ToList();
                        //        break;
                        //    case "3":
                        //        tempOrderProduct = firstOrderRuleProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[2].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[2].Split('-')[1])).ToList();
                        //        break;
                        //    case "4":
                        //        tempOrderProduct = firstOrderRuleProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[3].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[3].Split('-')[1])).ToList();
                        //        break;
                        //}
                        break;
                }
                tempOrderProduct.ForEach(p =>
                {
                    if (p.RuleId != SecondRule[j].RuleId)//判断erp是否修改过分类
                    {
                        p.sort = -1;
                    }
                    p.RuleId = SecondRule[j].RuleId;
                    p.RuleType = SecondRule[j].RuleType;
                    p.RuleNo = SecondRule[j].RuleObjectNo;
                    p.RuleName = SecondRule[j].RuleObjectName;
                });
                SecondRule[j].ProductCount = tempOrderProduct.Count();
                resultList.AddRange(tempOrderProduct.OrderByDescending(p => p.sort).ToList());
                firstOrderRuleProduct.OrderByDescending(p => p.sort).ToList();

            }
            if (firstOrderRuleProduct.Count > resultList.Count)//二级规则还有其他商品
            {
                for (int k = 0; k < firstOrderRuleProduct.Count; k++)
                {
                    if (resultList.Count(p => p.ProductNo == firstOrderRuleProduct.ElementAt(k).ProductNo) == 0)
                    {
                        resultList.Add(firstOrderRuleProduct.ElementAt(k));
                    }
                }
            }
            return resultList;
        }
        //按照规则生成产品排序
        private List<SortProduct> CreateProductOrder(List<SortProduct> searchProduct, List<SortRule> saveRule)
        {
            if (searchProduct.Count() == 0)//接口查不到商品直接返回
            {
                return searchProduct;
            }
            if (saveRule.Count == 0)//如果不存在规则
            {
                List<SortProduct> list = searchProduct.OrderByDescending(p => p.sort).ToList();
                return list;
            }
            List<SortProduct> result = new List<SortProduct>();
            string[] PriceRegion = AppSettingManager.AppSettings["PriceRegion"].Split(',');
            //先找一级规
            List<SortRule> firstRule = saveRule.Where(p => p.ParentId == 0).OrderByDescending(p => p.Sort).ToList();
            for (int i = 0; i < firstRule.Count; i++)
            {
                List<SortProduct> templist = new List<SortProduct>();
                if (saveRule.Count(p => p.ParentId == firstRule[i].RuleId) > 0)
                {
                    //一级规则下的所有商品
                    List<SortProduct> firstOrderRuleProduct = new List<SortProduct>();
                    List<SortRule> SecondRule = saveRule.Where(p => p.ParentId == firstRule[i].RuleId).OrderByDescending(p => p.Sort).ToList();
                    if (firstRule[i].RuleObjectNo == "OneOther")//一级规则的其他规则 特殊处理
                    {
                        firstOrderRuleProduct.AddRange(searchProduct.Where(p => p.RuleId == firstRule[i].RuleId));//加入一级规则商品
                        //加入二级规则下的商品
                        for (int j = 0; j < SecondRule.Count; j++)
                        {
                            firstOrderRuleProduct.AddRange(searchProduct.Where(p => p.RuleId == SecondRule[j].RuleId));
                        }
                    }
                    else
                    {
                        firstOrderRuleProduct = GetFirstRuleProduct(firstRule[i], searchProduct, PriceRegion);
                    }
                    //通过一级规则找到所有的 二级规则 然后对商品 firstOrderRuleProduct 进行二次排序
                    templist = CreateProductListBySecondRule(SecondRule, firstOrderRuleProduct, PriceRegion);
                    firstRule[i].ProductCount = templist.Count;
                }
                else//一级规则
                {
                    templist = CreateProductListByFirstRule(firstRule[i], searchProduct, PriceRegion);
                }
                if (templist.Count > 0)
                {
                    result.AddRange(templist);
                }
            }
            for (int i = 0; i < searchProduct.Count; i++)//二次检查不在规则内的商品 放入其他 可能是erp修改分类
            {
                if (result.Count(p=>p.ProductNo==searchProduct[i].ProductNo)==0)
                {
                    searchProduct[i].sort = -1;
                    result.Add(searchProduct[i]);
                    if (firstRule.Count(p=>p.RuleObjectNo=="OneOther")==1)
                    {
                        firstRule.Single(p => p.RuleObjectNo == "OneOther").ProductCount++;
                    }
                }
            }
            for (int i = 0; i < result.Count; i++)//重新计算生成排序
            {
                result.ElementAt(i).sort = i + 1;
                result.ElementAt(i).BrandContent = result.ElementAt(i).BrandEnName + "(" + result.ElementAt(i).BrandCnName + ")";
            }
            return result;
        }
        //拼装排序页面产品
        public SearchProductInfo GetSortResult(string category, string categoryType, int autoLastFlag = 0, int groupCount = 10)
        {
            SearchProductInfo obj = new SearchProductInfo();
            //传入全部商品编号  按每次10条调用接口数据
            obj.SaveRule = GetRuleListByCategoryNo(category).ToList();//获取用户保存规则
            List<SortProduct> searchProduct = GetSearchProductList(category, categoryType, autoLastFlag, groupCount);//获取接口搜索商品信息 
            //按规则 生成排序
            obj.ProductList = CreateProductOrder(searchProduct, obj.SaveRule);
            obj.SaveRule.Where(p => p.ProductCount > 0).ToList();//去除规则下面没有商品的
            #region 反推

            foreach (var item in obj.ProductList)
            {

                //反推 分类 品牌 色系 价格 规则
                if (!string.IsNullOrEmpty(item.CategoryNo))
                {
                    if (obj.RefRule.Count(p => p.RuleObjectNo == item.CategoryNo && p.RuleType == 1) == 0)//分类
                    {
                        obj.RefRule.Add(new SortRule { RuleObjectNo = item.CategoryNo, RuleObjectName = item.CategoryName, RuleType = 1, ProductCount = obj.ProductList.Count(p => p.CategoryNo == item.CategoryNo) });
                    }
                }
                if (obj.RefRule.Count(p => p.RuleObjectNo == item.BrandNo && p.RuleType == 2) == 0)//品牌
                {
                    obj.RefRule.Add(new SortRule { RuleObjectNo = item.BrandNo, RuleObjectName = item.BrandEnName, RuleType = 2, ProductCount = obj.ProductList.Count(p => p.BrandNo == item.BrandNo) });
                }
                if (!string.IsNullOrEmpty(item.ProductPrimaryColorNO))
                {
                    if (obj.RefRule.Count(p => p.RuleObjectNo == item.ProductPrimaryColorNO && p.RuleType == 3) == 0)//色系
                    {
                        obj.RefRule.Add(new SortRule { RuleObjectNo = item.ProductPrimaryColorNO, RuleObjectName = item.ProductPrimaryColorName, RuleType = 3, ProductCount = obj.ProductList.Count(p => p.ProductPrimaryColorNO == item.ProductPrimaryColorNO) });
                    }
                }
                if (priceRegion.Length > 0)//价格区间反推
                {
                    if (obj.RefRule.Count(p => p.RuleObjectNo == item.PriceNo) == 0)
                    {
                        obj.RefRule.Add(new SortRule { RuleObjectNo = item.PriceNo, RuleObjectName = GetPriceNo(item.LimitedPrice)[1], RuleType = 4, ProductCount = obj.ProductList.Count(p => p.PriceNo == item.PriceNo) });
                    }
                }
            }
            obj.RefRule.OrderBy(p => p.RuleObjectNo).ToList();
            #endregion
            obj.SaveRule= obj.SaveRule.Where(p=>p.ProductCount>0).ToList();
            obj.RefRule=obj.RefRule.OrderBy(p => p.RuleObjectNo).ToList();
            return obj;
        }
        //价格区间获取
        private string[] GetPriceNo(decimal productPrice)
        {
            string[] result = new string[2]; ;
            for (int i = 0; i < priceRegion.Length; i++)
            {
                if (productPrice >= decimal.Parse(priceRegion[i].Split('-')[1]) && productPrice <= decimal.Parse(priceRegion[i].Split('-')[2]))
                {
                    result[0] = priceRegion[i].Split('-')[0];
                    result[1] = priceRegion[i].Split('-')[1] + "-" + priceRegion[i].Split('-')[2];
                }
            }
            return result;
        }
        #endregion

        #region 调用搜索接口
        //拼接按品牌编号查询的请求地址
        private string GetListUrl(Parameters p)
        {
            System.Text.StringBuilder str = new StringBuilder();
            str.Append(AppSettingManager.AppSettings["SearchInterFaceUrl"]);
            str.Append("/shangpin/Product?userID=a23j3121112&userIP=127.0.0.1&encode=UTF-8");
            if (p.productNO != null && p.productNO != "")
            {
                str.Append("&productNO=" + p.productNO);
            }
            return str.ToString();
        }
        private string GetSearchListUrl(Parameters p)
        {
            System.Text.StringBuilder str = new StringBuilder();
            str.Append(AppSettingManager.AppSettings["ProductSortList"]);
            str.Append("userID=a23j3121112&userIP=127.0.0.1&encode=UTF-8");
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
        //根据条件参数把xml解析
        public List<SortProduct> GetXMLProductByProductNo(Parameters p)
        {
            List<SortProduct> list = new List<SortProduct>();
            string xmlText = GetResponse(GetListUrl(p));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.InnerXml = xmlText;
            XmlNodeList nodeList = null;
            nodeList = xmlDoc.SelectNodes("/result/docs/doc");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode item1 in nodeList)//docs节点doc
                {
                    SortProduct obj = new SortProduct();
                    foreach (XmlNode item2 in item1.ChildNodes)//docs节点doc
                    {
                        switch (item2.Attributes[0].Value)
                        {
                            case "ProductNo":
                                obj.ProductNo = item2.InnerText;
                                break;
                            case "ProductName":
                                obj.ProductName = item2.InnerText;
                                break;
                            case "BrandNo":
                                obj.BrandNo = item2.InnerText;
                                break;
                            case "MarketPrice":
                                obj.MarketPrice = decimal.Parse(item2.InnerText);
                                break;
                            case "LimitedPrice":
                                obj.LimitedPrice = decimal.Parse(item2.InnerText);
                                obj.PriceNo = GetPriceNo(obj.LimitedPrice)[0];
                                break;
                            case "PromotionPrice":
                                obj.PromotionPrice = decimal.Parse(item2.InnerText);
                                break;
                            case "ProductPicFile":
                                obj.ProductPicFile = item2.InnerText;
                                break;
                            case "BrandCnName":
                                obj.BrandCnName = item2.InnerText;
                                break;
                            case "BrandEnName":
                                obj.BrandEnName = item2.InnerText;
                                break;
                            case "HasStock":
                                obj.Stock = string.IsNullOrEmpty(item2.InnerText) ? 0 : int.Parse(item2.InnerText);
                                break;
                            case "CategoryNo":
                                if (item2.ChildNodes.Count > 1)
                                {
                                    for (int i = 0; i < item2.ChildNodes.Count; i++)
                                    {
                                        obj.CategoryNo += item2.ChildNodes[i].InnerText + ",";
                                    }
                                }
                                else
                                {
                                    obj.CategoryNo = item2.SelectSingleNode("value").InnerText;
                                }
                                break;
                            case "CategoryName":
                                if (item2.ChildNodes.Count > 1)
                                {
                                    for (int i = 0; i < item2.ChildNodes.Count; i++)
                                    {
                                        obj.CategoryName += item2.ChildNodes[i].InnerText + ",";
                                    }
                                }
                                else
                                {
                                    obj.CategoryName = item2.SelectSingleNode("value").InnerText;
                                }
                                break;
                            case "ProductPrimaryColor":
                                if (item2.ChildNodes.Count > 1)
                                {
                                    for (int i = 0; i < item2.ChildNodes.Count; i++)
                                    {
                                        obj.ProductPrimaryColorNO = item2.ChildNodes[i].InnerText.Split('|')[0] + ","; ;
                                        obj.ProductPrimaryColorName = item2.ChildNodes[i].InnerText.Split('|')[1] + ","; ;
                                    }
                                }
                                else
                                {
                                    obj.ProductPrimaryColorNO = item2.SelectSingleNode("value").InnerText.Split('|')[0];
                                    obj.ProductPrimaryColorName = item2.SelectSingleNode("value").InnerText.Split('|')[1];
                                }
                                break;
                        }
                    }
                    obj.BrandContent = obj.BrandEnName + "(" + obj.BrandCnName + ")";
                    list.Add(obj);
                }
            }
            else//只有单条商品
            {
                XmlNode productNode = xmlDoc.SelectSingleNode("/result/doc");//doc节点
                SortProduct obj = new SortProduct();
                if (productNode != null)
                {
                    foreach (XmlNode item2 in productNode.ChildNodes)//doc节点中的商品属性
                    {
                        switch (item2.Attributes[0].Value)
                        {
                            case "ProductNo":
                                obj.ProductNo = item2.InnerText;
                                break;
                            case "ProductName":
                                obj.ProductName = item2.InnerText;
                                break;
                            case "BrandNo":
                                obj.BrandNo = item2.InnerText;
                                break;
                            case "MarketPrice":
                                obj.MarketPrice = decimal.Parse(item2.InnerText);
                                break;
                            case "LimitedPrice":
                                obj.LimitedPrice = decimal.Parse(item2.InnerText);
                                obj.PriceNo = GetPriceNo(obj.LimitedPrice)[0];
                                break;
                            case "PromotionPrice":
                                obj.PromotionPrice = decimal.Parse(item2.InnerText);
                                break;
                            case "ProductPicFile":
                                obj.ProductPicFile = item2.InnerText;
                                break;
                            case "BrandCnName":
                                obj.BrandCnName = item2.InnerText;
                                break;
                            case "BrandEnName":
                                obj.BrandEnName = item2.InnerText;
                                break;
                            case "HasStock":
                                obj.Stock = item2.InnerText != null ? 0 : int.Parse(item2.InnerText);
                                break;
                            case "CategoryNo":
                                if (item2.ChildNodes.Count > 1)
                                {
                                    for (int i = 0; i < item2.ChildNodes.Count; i++)
                                    {
                                        obj.CategoryNo += item2.ChildNodes[i].InnerText + ",";
                                    }
                                }
                                else
                                {
                                    obj.CategoryNo = item2.SelectSingleNode("value").InnerText;
                                }
                                break;
                            case "CategoryName":
                                if (item2.ChildNodes.Count > 1)
                                {
                                    for (int i = 0; i < item2.ChildNodes.Count; i++)
                                    {
                                        obj.CategoryName += item2.ChildNodes[i].InnerText + ",";
                                    }
                                }
                                else
                                {
                                    obj.CategoryName = item2.SelectSingleNode("value").InnerText;
                                }
                                break;
                            case "ProductPrimaryColor":
                                if (item2.ChildNodes.Count > 1)
                                {
                                    for (int i = 0; i < item2.ChildNodes.Count; i++)
                                    {
                                        obj.ProductPrimaryColorNO = item2.ChildNodes[i].InnerText.Split('|')[0] + ","; ;
                                        obj.ProductPrimaryColorName = item2.ChildNodes[i].InnerText.Split('|')[1] + ","; ;
                                    }
                                }
                                else
                                {
                                    obj.ProductPrimaryColorNO = item2.SelectSingleNode("value").InnerText.Split('|')[0];
                                    obj.ProductPrimaryColorName = item2.SelectSingleNode("value").InnerText.Split('|')[1];
                                }
                                break;
                        }
                    }

                    list.Add(obj);
                }
            }
            return list;
        }
        //分批次获取商品 每次获取默认10条
        private List<SortProduct> GetSearchProductList(string categoryNo, string categoryType, int isLast, int pageSize)
        {
            List<SortProduct> resultList = new List<SortProduct>();//保存最终运算结果排序
            IEnumerable<SWfsSortProduct> saveProductList = GetSortProductList(categoryNo);//查询保存排序的商品
            if (saveProductList.Count() > 0)
            {
                StringBuilder sb = new StringBuilder();
                List<SortProduct> xmlProductList = new List<SortProduct>();//保存搜索接口返回的商品
                int PageCount = saveProductList.Count() % pageSize == 0 ? saveProductList.Count() / pageSize : saveProductList.Count() / pageSize + 1;
                PageCount = PageCount == 0 ? 1 : PageCount;
                #region 按pageSize分批次 从搜索接口获取商品
                for (int i = 0; i < PageCount; i++)
                {
                    IEnumerable<string> groupPNo = saveProductList.Take((i + 1) * pageSize).Skip(pageSize * i).Select(p => p.ProductNo);//分批次获取产品编号
                    Parameters param = new Parameters();
                    for (int j = 0; j < groupPNo.Count(); j++)
                    {
                        if (j == groupPNo.Count() - 1)
                        {
                            sb.Append(groupPNo.ElementAt(j));
                        }
                        else
                        {
                            sb.Append(groupPNo.ElementAt(j) + ",");
                        }
                    }

                    param.productNO = sb.ToString();
                    List<SortProduct> groupList = GetXMLProductByProductNo(param);
                    if (groupList.Count > 0)
                    {
                        xmlProductList.AddRange(groupList);
                    }
                    sb.Length = 0;
                }
                #endregion
                #region 分类过滤 和 //售罄商品是否沉底过滤
                if (categoryType == "1")
                {
                    xmlProductList = xmlProductList.Where(p => p.CategoryNo!=null && p.CategoryNo.Contains(categoryNo) && p.Stock >= isLast).ToList();
                }
                else
                {
                    xmlProductList = xmlProductList.Where(p => p.CategoryNo != null && p.Stock >= isLast).ToList();
                }
                #endregion
                #region 对保存的商品 和 搜索接口返回的商品  两个集合的数据拼接成完整的商品信息
                for (int i = 0; i < saveProductList.Count(); i++)
                {
                    if (xmlProductList.Count(p => p.ProductNo == saveProductList.ElementAt(i).ProductNo) == 1)
                    {
                        SortProduct obj = xmlProductList.Single(p => p.ProductNo == saveProductList.ElementAt(i).ProductNo);
                        obj.RuleId = saveProductList.ElementAt(i).RuleId;
                        obj.sort = saveProductList.ElementAt(i).Sort;
                        //存在两个分类 或者  两种色系的 重置为空
                        if (obj.CategoryNo != null)
                        {
                            if (obj.CategoryNo.Contains(','))
                            {
                                obj.CategoryNo = "";
                                obj.CategoryName = "";
                            }
                        }
                        if (obj.ProductPrimaryColorNO != null)
                        {
                            if (obj.ProductPrimaryColorNO.Contains(','))
                            {
                                obj.ProductPrimaryColorNO = "";
                                obj.ProductPrimaryColorName = "";
                            }
                        }
                        resultList.Add(obj);
                    }
                }
                #endregion
            }
            return resultList;
        }
        #endregion

        //#region 获取品牌
        //public Dictionary<string, List<string>> getBrandControlDate(string selectType, string categoryno)
        //{

        //    //IEnumerable<ErpBrand> brandlist = DapperUtil.Query<ErpBrand>("ComBeziErp_ErpBrand_GetErpBrandList");
        //    Dictionary<string, List<string>> BrandDic = new Dictionary<string, List<string>>();
        //    List<ErpBrand> brandList = GetSearchProductList(categoryno, 1, 1, 10);
        //    foreach (ErpBrand brand in brandlist)
        //    {
        //        if (brand.BrandEnName.Length > 0)
        //        {
        //            try
        //            {
        //                string pinyinStr = CharactersConvertTopinyin.Convert(brand.BrandEnName.Substring(0, 1));
        //                pinyinStr = pinyinStr.Substring(0, 1).ToLower();
        //                if (!BrandDic.ContainsKey(brand.BrandNo))
        //                {
        //                    List<string> L = new List<string>();
        //                    L.Add(pinyinStr);
        //                    L.Add(brand.BrandCnName);
        //                    L.Add(brand.BrandEnName);
        //                    BrandDic.Add(brand.BrandNo, L);
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                string empty = string.Empty;
        //            }
        //        }
        //    }
        //    return BrandDic;
        //}
        //public List<ErpBrand> getBrandGroup(string selectType, string categoryno)
        //{
        //    List<ErpBrand> brandinfolist = new List<ErpBrand>();
        //    Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        //    dic = getBrandControlDate(selectType, categoryno); //需要建立缓存数据
        //    foreach (KeyValuePair<string, List<string>> p in dic)
        //    {
        //        ErpBrand brand = new ErpBrand();
        //        brand.BrandNo = p.Key;
        //        brand.BrandCnName = p.Value[1];
        //        brand.BrandEnName = p.Value[2];
        //        brand.FristLetter = p.Value[0].ToUpper();
        //        brandinfolist.Add(brand);
        //    }

        //    return brandinfolist;
        //}
        //public IList<ErpBrand> getBrandGroupFristLetter(string selectType, string categoryno)
        //{
        //    Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        //    IList<ErpBrand> brandFL = new List<ErpBrand>();
        //    dic = getBrandControlDate(selectType, categoryno); //建立缓存数据
        //    ArrayList list = new ArrayList();
        //    foreach (KeyValuePair<string, List<string>> p in dic)
        //    {
        //        if (!list.Contains(p.Value[0].ToUpper()))
        //        {
        //            list.Add(p.Value[0].ToUpper());
        //        }
        //    }
        //    list.Sort();
        //    foreach (object L in list)
        //    {
        //        ErpBrand brand = new ErpBrand();
        //        brand.FristLetter = L.ToString();
        //        brandFL.Add(brand);
        //    }
        //    return brandFL;
        //}
        //#endregion

        //#region 集成测试

        ////测试排序池商品
        //public IEnumerable<SWfsSortProduct> SWfsSortProductTest(string category)
        //{
        //    return GetSortProductList(category);
        //}
        ////测试接口返回商品
        //public List<SortProduct> SearchProductTest(string categoryNo, string categoryType, int isLast, int pageSize)
        //{
        //    return GetSearchProductList(categoryNo, categoryType, isLast, pageSize);
        //}
        ////测试用户保存的规则
        //public List<SortRule> RuleTest(string categoryNo)
        //{
        //    return GetOrderSorRule(GetRuleListByCategoryNo(categoryNo));
        //}
        ////获取所有分类
        //public List<SWfsCategory> GetCagegoryList()
        //{
        //    return DapperUtil.Query<SWfsCategory>("ComBeziWfs_SwfsCategory_getCategoryAll").ToList();
        //}
        ////按商品编号获取色系
        //public WfsProductAttr GetProductAttrByProductNo(string productNo)
        //{
        //    return DapperUtil.Query<WfsProductAttr>("", new { }).FirstOrDefault();
        //}
        ////读取商品品牌
        //public string GetBrandNo(string productNo)
        //{
        //    return DapperUtil.Query<string>("ComBeziWfs_WfsProduct_GetBrandNoByProductNo", new
        //    {
        //        productNo = productNo
        //    }).FirstOrDefault();
        //}
        //#endregion
    }

    #region 实体类
    public class SearchProductInfo
    {
        //产品
        public List<SortProduct> ProductList = new List<SortProduct>();
        //反推的规则
        public List<SortRule> RefRule = new List<SortRule>();
        //保存的规则
        public List<SortRule> SaveRule = new List<SortRule>();

    }

    //商品结构
    public class SortProduct
    {
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public decimal MarketPrice { get; set; }                //市场价
        public decimal LimitedPrice { get; set; }               //尚品价
        public decimal PromotionPrice { get; set; }             //促销价
        public string ProductPicFile { get; set; }              //商品图片编号
        public int Stock { get; set; }                          //库存
        public int sort { get; set; }                           //排序
        public string BrandContent { get; set; }                //英文名加中文名 组合品牌名称

        public string BrandNo { get; set; }                     //品牌编号
        public string BrandCnName { get; set; }                 //品牌英文名称
        public string BrandEnName { get; set; }                 //品牌中文名称
        public string ProductPrimaryColorNO { get; set; }       //色系编号
        public string ProductPrimaryColorName { get; set; }     //色系名称
        public string CategoryNo { get; set; }                  //分类编号
        public string CategoryName { get; set; }                //分类名称

        public string PriceNo { get; set; }                    //价格区间ID

        public int RuleId { get; set; }                         //排序ID
        public string RuleNo { get; set; }                      //规则编号
        public string RuleName { get; set; }                    //规则名称
        public short RuleType { get; set; }                    //规则类型

    }

    public class SortRule
    {
        public DateTime DateCreate { get; set; }
        public string OcsCategoryNo { get; set; }
        public string OperatorUserId { get; set; }
        public int ParentId { get; set; }
        public int RuleId { get; set; }
        public string RuleObjectName { get; set; }
        public string RuleObjectNo { get; set; }
        public short RuleType { get; set; }
        public int Sort { get; set; }
        public int ProductCount { get; set; }
    }
    public class ErpBrand
    {
        public string BrandNo { get; set; }
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }
        public string FristLetter { get; set; }
    }
    #endregion
    

}


//private List<SortProduct> CreateProductOrderNew(List<SortProduct> searchProduct, List<SortRule> orderRule)
//        {
//            if (searchProduct.Count() == 0)//接口查不到商品直接返回
//            {
//                return searchProduct;
//            }
//            //按规则 剔除不在规则内的商品 生成新的排序
//            if (orderRule.Count() > 0)
//            {
//                List<SortProduct> ResultsortList = new List<SortProduct>();
//                List<SortProduct> templist;
//                string[] PriceRegion = AppSettingManager.AppSettings["PriceRegion"].Split(',');
//                for (int i = 0; i < orderRule.Count; i++)
//                {
//                    switch (orderRule[i].RuleType)
//                    {
//                        case 1://分类
//                            templist = searchProduct.Where(p => p.CategoryNo == orderRule[i].RuleObjectNo).ToList();
//                            templist.ForEach(p =>
//                            {
//                                if (p.RuleId != orderRule[i].RuleId)//判断erp是否修改过分类
//                                {
//                                    p.sort = -1;
//                                }
//                                p.RuleId = orderRule[i].RuleId;
//                                p.RuleType = orderRule[i].RuleType;
//                                p.RuleNo = orderRule[i].RuleObjectNo;
//                                p.RuleName = orderRule[i].RuleObjectName;
//                            });
//                            ResultsortList.AddRange(templist.OrderByDescending(p => p.sort));
//                            orderRule[i].ProductCount = templist.Count();
//                            break;
//                        case 2://品牌
//                            templist = searchProduct.Where(p => p.BrandNo == orderRule[i].RuleObjectNo).ToList();
//                            templist.ForEach(p =>
//                            {
//                                if (p.RuleId != orderRule[i].RuleId)//判断erp是否修改过品牌
//                                {
//                                    p.sort = -1;
//                                }
//                                p.RuleId = orderRule[i].RuleId;
//                                p.RuleType = orderRule[i].RuleType;
//                                p.RuleNo = orderRule[i].RuleObjectNo;
//                                p.RuleName = orderRule[i].RuleObjectName;
//                            });
//                            ResultsortList.AddRange(templist.OrderByDescending(p => p.sort));
//                            orderRule[i].ProductCount = templist.Count();
//                            break;
//                        case 3://色系
//                            templist = searchProduct.Where(p => p.ProductPrimaryColorNO == orderRule[i].RuleObjectNo).ToList();
//                            templist.ForEach(p =>
//                            {
//                                if (p.RuleId != orderRule[i].RuleId)
//                                {
//                                    p.sort = -1;
//                                }
//                                p.RuleId = orderRule[i].RuleId;
//                                p.RuleType = orderRule[i].RuleType;
//                                p.RuleNo = orderRule[i].RuleObjectNo;
//                                p.RuleName = orderRule[i].RuleObjectName;
//                            });
//                            ResultsortList.AddRange(templist.OrderByDescending(p => p.sort));
//                            orderRule[i].ProductCount = templist.Count();
//                            break;
//                        case 4://价格
//                            switch (orderRule[i].RuleObjectNo)
//                            {
//                                case "1":
//                                    templist = searchProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[0].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[0].Split('-')[1])).ToList();
//                                    templist.ForEach(p =>
//                                    {
//                                        if (p.RuleId != orderRule[i].RuleId)
//                                        {
//                                            p.sort = -1;
//                                        }
//                                        p.RuleId = orderRule[i].RuleId;
//                                        p.RuleType = orderRule[i].RuleType;
//                                        p.RuleNo = orderRule[i].RuleObjectNo;
//                                        p.RuleName = orderRule[i].RuleObjectName;
//                                    });
//                                    ResultsortList.AddRange(templist.OrderByDescending(p => p.sort));
//                                    orderRule[i].ProductCount = templist.Count();
//                                    break;
//                                case "2":
//                                    templist = searchProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[1].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[1].Split('-')[1])).ToList();
//                                    templist.ForEach(p =>
//                                    {
//                                        if (p.RuleId != orderRule[i].RuleId)
//                                        {
//                                            p.sort = -1;
//                                        }
//                                        p.RuleId = orderRule[i].RuleId;
//                                        p.RuleType = orderRule[i].RuleType;
//                                        p.RuleNo = orderRule[i].RuleObjectNo;
//                                        p.RuleName = orderRule[i].RuleObjectName;
//                                    });
//                                    ResultsortList.AddRange(templist.OrderByDescending(p => p.sort));
//                                    orderRule[i].ProductCount = templist.Count();
//                                    break;
//                                case "3":
//                                    templist = searchProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[2].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[2].Split('-')[1])).ToList();
//                                    templist.ForEach(p =>
//                                    {
//                                        if (p.RuleId != orderRule[i].RuleId)
//                                        {
//                                            p.sort = -1;
//                                        }
//                                        p.RuleId = orderRule[i].RuleId;
//                                        p.RuleType = orderRule[i].RuleType;
//                                        p.RuleNo = orderRule[i].RuleObjectNo;
//                                        p.RuleName = orderRule[i].RuleObjectName;
//                                    });
//                                    ResultsortList.AddRange(templist.OrderByDescending(p => p.sort));
//                                    orderRule[i].ProductCount = templist.Count();
//                                    break;
//                                case "4":
//                                    templist = searchProduct.Where(p => p.LimitedPrice >= decimal.Parse(PriceRegion[3].Split('-')[0]) && p.LimitedPrice < decimal.Parse(PriceRegion[3].Split('-')[1])).ToList();
//                                    templist.ForEach(p =>
//                                    {
//                                        if (p.RuleId != orderRule[i].RuleId)
//                                        {
//                                            p.sort = -1;
//                                        }
//                                        p.RuleId = orderRule[i].RuleId;
//                                        p.RuleType = orderRule[i].RuleType;
//                                        p.RuleNo = orderRule[i].RuleObjectNo;
//                                        p.RuleName = orderRule[i].RuleObjectName;
//                                    });
//                                    ResultsortList.AddRange(templist.OrderByDescending(p => p.sort));
//                                    orderRule[i].ProductCount = templist.Count();
//                                    break;
//                            }
//                            break;
//                    }
//                }
//                for (int i = 0; i < searchProduct.Count; i++)//其他规则
//                {
//                    if (ResultsortList.Count(p => p.ProductNo == searchProduct[i].ProductNo) == 0)
//                    {
//                        searchProduct.ElementAt(i).RuleId = -1;
//                        searchProduct.ElementAt(i).RuleType = -1;
//                        searchProduct.ElementAt(i).RuleNo = "";
//                        searchProduct.ElementAt(i).RuleName = "";
//                        ResultsortList.Add(searchProduct[i]);
//                    }
//                }
//                //重新统计规则下的商品个数
//                orderRule.Where(p => p.ParentId == 0).ToList().ForEach(p =>
//                {
//                    p.ProductCount = orderRule.Where(q => q.ParentId == p.RuleId).Sum(q => q.ProductCount);
//                });
//                //增加一级规则 全部 和其他
//                //orderRule.Add(new SortRule { RuleType = 5, RuleObjectNo = "OneAll", RuleObjectName = "全部", ProductCount = ResultsortList.Count });
//                //orderRule.Add(new SortRule { RuleType = 6, RuleObjectNo = "OneOther", RuleObjectName = "其他", ProductCount = ResultsortList.Count(p=>p.RuleId==-1) });

//                //增加二级规则的全部 和其他 
//                //List<SortRule> tempRule = orderRule.Where(p=>p.ParentId==0).ToList();
//                //for (int i = 0; i < tempRule.Count; i++)
//                //{
//                //    if (orderRule.Count(p=>p.ParentId==tempRule[i].RuleId)>0)
//                //    {
//                //        orderRule.Add(new SortRule { RuleType = 5, RuleObjectNo = "TwoAll", RuleObjectName = "全部", ProductCount = tempRule[i].ProductCount });
//                //        orderRule.Add(new SortRule { RuleType = 6, RuleObjectNo = "TwoOther", RuleObjectName = "其他", ProductCount = tempRule[i].ProductCount - orderRule.Where(p => p.ParentId == tempRule[i].RuleId).Sum(p=>p.ProductCount) });
//                //    }
//                //}

//                for (int i = 0; i < ResultsortList.Count; i++)//重新计算生成排序
//                {
//                    ResultsortList.ElementAt(i).sort = i + 1;
//                    ResultsortList.ElementAt(i).BrandContent = ResultsortList.ElementAt(i).BrandEnName + "(" + ResultsortList.ElementAt(i).BrandCnName + ")";
//                }
//                return ResultsortList.OrderBy(p=>p.sort).ToList();
//            }
//            else
//            {
//                searchProduct.ToList().ForEach(p =>
//                {
//                    p.RuleId = -1;
//                    p.RuleType = -1;
//                    p.RuleNo = "";
//                    p.RuleName = "";
//                });
//                return searchProduct.OrderByDescending(p => p.sort).ToList();
//            }
//        }