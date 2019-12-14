using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Ocs.Entity.Extenstion.ProductFlat;
using Shangpin.Ocs.Service.Shangpin.ProductSort;
using System.Xml;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class ProductLabelService
    {
        #region 分类 关联 标签 管理
        //查询所有标签   
        public IEnumerable<SWfsProductLabel> GetLabelList()
        {
            return DapperUtil.Query<SWfsProductLabel>("ComBeziWfs_SWfsProductLabel_GetLabelAll", null);
        }

        /// <summary>
        /// 删除指定标签编号的标签,将关联的删除下级标签
        /// </summary>
        /// <param name="labelNo">标签编号</param>
        /// <returns></returns>
        public int DeleteLabelBtLabelNo(string labelNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsLabel_DeleteLabel", new { LabelNo = labelNo });
        }

        //添加标签  obj 赋值 字段  ParentNo LabelName LabelType 即可
        public int AddProductLabel(SWfsProductLabel obj)
        {
            //查询同级标签类  中最大的 标签编号
            obj.LabelNo = CreateLabelNo(obj.ParentNo);
            return DapperUtil.Insert<SWfsProductLabel>(obj, true);
        }
        //按ID获取标签
        public SWfsProductLabel GetLabelById(int id)
        {
            return DapperUtil.Query<SWfsProductLabel>("ComBeziWfs_SWfsLabel_GetProductLabelByID", new { LabelId = id }).FirstOrDefault();
        }
        //修改标签
        public int EditeLabel(SWfsProductLabel obj)
        {
            //验证是否存在重复标签
            return DapperUtil.UpdatePartialColumns<SWfsProductLabel>(new
            {
                LabelId = obj.LabelId,
                LabelName = obj.LabelName,
                LabelType = obj.LabelType,
                LabelNickName = obj.LabelNickName
            }) ? 1 : 0;
        }
        //根据父类编号 生成标签的LabelNo编号
        private string CreateLabelNo(string parentNo)
        {
            string LabelNO = string.Empty;
            //查询 同级标签 中的数量
            int MaxLabelNo = DapperUtil.Query<int>("ComBeziWfs_SWfsLabel_GetMaxProductLabelNo", new { ParentNo = parentNo }).FirstOrDefault();
            if (parentNo == "Root")//添加父类
            {
                if (MaxLabelNo <= 9)
                {
                    LabelNO = "A00" + (MaxLabelNo + 1);
                }
                else if (MaxLabelNo <= 99)
                {
                    LabelNO = "A0" + (MaxLabelNo + 1);
                }
                else
                {
                    LabelNO = "A" + (MaxLabelNo + 1);
                }
            }
            else//添加子类
            {
                switch (parentNo.Length)
                {
                    case 4:
                        if (MaxLabelNo <= 9)
                        {
                            LabelNO = parentNo + "B00" + (MaxLabelNo + 1);
                        }
                        else if (MaxLabelNo <= 99)
                        {
                            LabelNO = parentNo + "B0" + (MaxLabelNo + 1);
                        }
                        else
                        {
                            LabelNO = parentNo + "B" + (MaxLabelNo + 1);
                        }
                        break;
                    case 8:
                        if (MaxLabelNo <= 9)
                        {
                            LabelNO = parentNo + "C00" + (MaxLabelNo + 1);
                        }
                        else if (MaxLabelNo <= 99)
                        {
                            LabelNO = parentNo + "C0" + (MaxLabelNo + 1);
                        }
                        else
                        {
                            LabelNO = parentNo + "C" + (MaxLabelNo + 1);
                        }
                        break;
                    case 12:
                        if (MaxLabelNo <= 9)
                        {
                            LabelNO = parentNo + "D00" + (MaxLabelNo + 1);
                        }
                        else if (MaxLabelNo <= 99)
                        {
                            LabelNO = parentNo + "D0" + (MaxLabelNo + 1);
                        }
                        else
                        {
                            LabelNO = parentNo + "D" + (MaxLabelNo + 1);
                        }
                        break;
                    default:
                        break;
                }
            }
            return LabelNO;
        }
        //验证同级下 是否存在重复标签
        public int IsExistSameLabel(string parentNo, string LabelName)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsLabel_IsExistsProductLabel", new { ParentNo = parentNo, LabelName = LabelName }).FirstOrDefault();
        }

        /// <summary>
        /// 验证同级下是否存在重复标签, 用于页面修改标签时验证重复名称
        /// </summary>
        /// <param name="parentNo">父级编号</param>
        /// <param name="LabelName">标签名称</param>
        /// <param name="LabelId">标签自动编号</param>
        /// <returns></returns>
        public int IsExistSameLabel(string parentNo, string LabelName, int LabelId)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsLabel_IsExistsProductLabelEx", new
            {
                ParentNo = parentNo,
                LabelName = LabelName,
                LabelId = LabelId
            }).FirstOrDefault();
        }
        #endregion

        #region 商品 关联 标签管理
        //按组ID获取组内产品列表   2014-10-01 14:46:07 by liu
        public IEnumerable<ProductListForLabelSelect> GetSWfsProductList(string StartDateShelf, string EndDateShelf, string isShelf, string gender, string brandNO, string categoryNo, string keyword, string isPublish, string templateName, int pageIndex, int pageSize,string isout, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNO", brandNO == null ? "" : brandNO);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("TemplateName", templateName == null ? "" : templateName);
            dic.Add("IsPublish", isPublish == null ? "" : isPublish);
            dic.Add("StartDateShelf", StartDateShelf == null ? "" : StartDateShelf);
            dic.Add("EndDateShelf", EndDateShelf == null ? "" : EndDateShelf);
            dic.Add("IsOutSide", string.IsNullOrEmpty(isout)? "" : isout);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsProduct_SelectSWfsProductCount2", dic, new 
            {
                StartDateShelf,
                EndDateShelf,
                IsShelf = isShelf,
                KeyWord = keyword,
                BrandNO = brandNO,
                Gender = gender, 
                CategoryNo = categoryNo,
                IsPublish = isPublish,
                TemplateName = templateName,
                IsOutSide=string.IsNullOrEmpty(isout)?-1:int.Parse(isout)
            }).FirstOrDefault();
            return DapperUtil.Query<ProductListForLabelSelect>("ComBeziWfs_SWfsProduct_SelectSWfsProductList2", dic, new { StartDateShelf, EndDateShelf, IsShelf = isShelf, KeyWord = keyword, BrandNO = brandNO, Gender = gender, CategoryNo = categoryNo, IsPublish = isPublish, TemplateName = templateName, pageIndex = pageIndex, pageSize = pageSize,IsOutSide = string.IsNullOrEmpty(isout) ? -1 : int.Parse(isout) });
        }
        /// <summary>
        /// 获取商品关联标签
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public IEnumerable<SWfsProductRefLabel> GetProductLabel(string productNo)
        {
            return DapperUtil.Query<SWfsProductRefLabel>("ComBeziWfs_SwfsProductRefLabel_GetProductLabel", new { ProductNo = productNo });
        }
        /// <summary>
        /// 删除商品关联标签
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public int DeleteProductLabel(string productNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SwfsProductRefLabel_DeleteProductLabel", new { ProductNo = productNo });
        }
        /// <summary>
        /// 插入商标关联标签
        /// </summary>
        /// <param name="sWfsProductRefLabel"></param>
        /// <returns></returns>
        public int InsertProductLabel(SWfsProductRefLabel sWfsProductRefLabel)
        {
            return DapperUtil.Insert<SWfsProductRefLabel>(sWfsProductRefLabel, true);
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <param name="SWfsProductLabel"></param>
        /// <returns></returns>
        public IEnumerable<SWfsProductLabel> GetAllLabel()
        {
            return DapperUtil.Query<SWfsProductLabel>("ComBeziWfs_SWfsProductLabel_GetAllLabels");
        }
        /// <summary>
        /// 获取商品标签详情
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public IEnumerable<ProductRefLabelDetail> GetProductRefLabelDetail(string productNo)
        {
            return DapperUtil.Query<ProductRefLabelDetail>("ComBeziWfs_SwfsProductRefLabel_GetProductRefLabelDetail", new { ProductNo = productNo });
        }

        //历史记录查询 放入cookie
        //public string[] GetSearchHistory()
        //{
        //    if (PresentationHelper.GetPassport() != null)
        //    {
        //        string aa = PresentationHelper.GetCookie("searchHistory");
        //        return PresentationHelper.GetCookie("searchHistory").Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
        //    }
        //    return new string[0];
        //}
        //public void AddSearchHistory(string searchUrl, int count = 10)
        //{
        //    if (PresentationHelper.GetPassport() != null && PresentationHelper.GetPassport().UserName != null)
        //    {
        //        List<string> urlList = new List<string>();
        //        string[] history = PresentationHelper.GetCookie("searchHistory").Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
        //        urlList.Add(searchUrl);
        //        if (history.Length >= count)
        //        {
        //            if (history.Length > count)
        //            {
        //                history = history.Take(10).ToArray();
        //            }
        //            for (int i = 0; i < history.Length; i++)
        //            {
        //                urlList.Add(history[i]);
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < history.Length; i++)
        //            {
        //                urlList.Add(history[i]);
        //            }
        //        }
        //        if (urlList.Count>0)
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            for (int i = 0; i < urlList.Count; i++)
        //            {
        //                sb.Append(urlList[i] + "###");
        //            }
        //            PresentationHelper.SetBrowserCookie("searchHistory", System.Web.HttpUtility.UrlEncode(sb.ToString()));
        //        }

        //    }
        //}
        #endregion

        #region 查询历史记录
        //获取历史记录
        public IEnumerable<SWfsProductLabelSearchHistory> GetSearchHistory(int count)
        {
            IEnumerable<SWfsProductLabelSearchHistory> list = new List<SWfsProductLabelSearchHistory>();
            Passport passport = PresentationHelper.GetPassport();
            if (passport != null)
            {
                if (!string.IsNullOrEmpty(passport.UserName))
                {
                    //查询历史记录
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("getCount", count);
                    list = DapperUtil.Query<SWfsProductLabelSearchHistory>("ComBeziWfs_SWfsProductLabelSearchHistory_GetSearchHistory", dic, new { SearchUser = passport.UserName });
                }
            }
            return list;
        }
        //添加历史记录
        public int AddSearchHistory(SWfsProductLabelSearchHistory obj)
        {
            Passport passport = PresentationHelper.GetPassport();
            if (passport != null && !string.IsNullOrEmpty(passport.UserName))
            {
                obj.CreateDate = DateTime.Now;
                obj.SearchUser = passport.UserName;
                return DapperUtil.Insert<SWfsProductLabelSearchHistory>(obj, true);
            }
            return 0;
        }
        //删除历史记录
        public int ClearLabelHistory(string userId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsProductLabelSearchHistory_DeleteLabelHistory", new
            {
                SearchUser = userId
            });
        }
        #endregion

        //根据产品编号获取商品不同角度的图片
        public IEnumerable<string> GetProductPicList(string productNo)
        {
            return DapperUtil.Query<string>("ComBeziWfs_WfsPrductAttr_GetProductPicList", new { ProductNo = productNo });
        }

        /// <summary>
        /// 根据父级分类No获得标签分类列表
        /// </summary>
        /// <param name="parentNo"></param>
        /// <returns></returns>
        public List<ProductLabel> GetListProductLabel(string parentNo, string channelNo)
        {
            List<ProductLabel> resultlist = new List<ProductLabel>();
            List<ProductLabel> ltlist = new List<ProductLabel>();
            string url = GetListUrl(channelNo);
            string xml = GetResponse(url);
            ltlist = GetListFilters(xml);
            IEnumerable<string> parentNoList = ltlist.Select(p => p.ParentNo).Distinct();
            for (int i = 0; i < parentNoList.Count(); i++)
            {
                resultlist.Add(ltlist.Where(p => p.ParentNo == parentNoList.ElementAt(i)).ElementAt(0));
            }
            return resultlist;
        }
        #region  查询标签

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

        private string GetListUrl(string CategoryNo)
        {
            System.Text.StringBuilder str = new StringBuilder();
            str.Append(AppSettingManager.AppSettings["ProductLabelList"]);
            str.Append("/shangpin/ListFilters?");
            str.Append("categoryNO=" + CategoryNo);
            str.Append("&userID=a23j3121112&userIP=127.0.0.1&encode=UTF-8");
           
            return str.ToString();
        }

        public XmlNodeList GetNodeList(XmlDocument XMLDoc, string xmlPath)
        {
            XmlNodeList nodeList = XMLDoc.SelectNodes(xmlPath);//默认读取第一个
            return nodeList;
        }

        internal List<ProductLabel> GetListFilters(string xml)
        {
            List<ProductLabel> tmplabelList = null;
            var xmlobj = new XmlDocument();
            if (xml != "")
            {
                xmlobj.LoadXml(xml);
            }
            XmlNodeList nodeList = GetNodeList(xmlobj, "/result/facets/facet");
            if (nodeList != null && nodeList.Count > 0)
            {
                string nodeName = string.Empty;
                foreach (XmlNode item in nodeList)
                {
                    //string[] facet = new string[] { "Brand", "CLv2", "CLv3", "CLv4", "ProductSize", "ProductLabels", "ProductPrimaryColors", parm.PriceZone };//切面数据项
                    nodeName = item.Attributes["name"].Value;
                    switch (nodeName)
                    {
                        case "ProductLabels":
                            //解析ProductLabels的逻辑输出
                            tmplabelList = GetLabelListNew(xmlobj, nodeName);
                            break;
                    }

                }
            }
            return tmplabelList;
        }

        /// <summary>
        /// 新标签列表【2014-3-19新建的】
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private List<ProductLabel> GetLabelListNew(XmlDocument xmlDoc, string nodeName)
        {
            List<ProductLabel> labList = new List<ProductLabel>();
            XmlNodeList nodeList = GetNodeList(xmlDoc, "result/facets/facet[@name='" + nodeName + "']/item");
            if (nodeList != null && nodeList.Count > 0)
            {
                ProductLabel lab;
                foreach (XmlNode node in nodeList)
                {
                    string[] columns = node.Attributes["name"].Value.Split('|');
                    if (columns != null && columns.Length > 0)
                    {
                        lab = new ProductLabel
                        {
                            ParentNo = columns[0],
                            ParentName = columns[1],
                            LabelNo = columns[2],
                            LabelName = columns[3]
                        };
                        labList.Add(lab);
                    }
                }
            }

            return labList;
        }

        #endregion
        /// <summary>
        /// 根据父级分类No获得标签关联产品总数
        /// </summary>
        /// <param name="parentNo"></param>
        /// <returns></returns>
        public int GetLabelCountByParentNo(string parentNo)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsProductRefLabel_GetProductList", new
            {
                RefLabelNo = parentNo
            }).FirstOrDefault();
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="labelNo"></param>
        /// <param name="categroyNo"></param>
        /// <returns></returns>
        public SWfsPopElements ExistsLabelNo(string labelNo, string categroyNo)
        {
            return DapperUtil.Query<SWfsPopElements>("ComBeziWfs_SWfsPopElements_ExistsPopElements", new
            {
                CategoryNo = categroyNo,
                LabelNo = labelNo
            }).FirstOrDefault();
        }

        /// <summary>
        /// 添加流行元素
        /// </summary>
        /// <param name="pop"></param>
        public void AddPopElements(SWfsPopElements pop)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsPopElements_AddPopElements", new
            {
                CategoryNo = pop.CategoryNo,
                LabelNo = pop.LabelNo,
                Status = pop.Status,
                SortId = pop.SortId,
                OperatorUserId = pop.OperatorUserId,
                DateCreate = pop.DateCreate
            });
        }

        public int GetPopElementsCount()
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsPopElements_PopElementsCount").FirstOrDefault();
        }
    }
}
