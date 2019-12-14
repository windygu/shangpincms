using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Service.Common;
using System.Xml;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System.IO;
using Shangpin.Framework.Common.Dapper;
using Shangpin.Entity.Common;



namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsBrandService
    {
        #region Navigation
        //根据品牌编号获取导航列表
        public IEnumerable<SWfsBrandNavigation> GetNavigateList(string brandNo)
        {
            var result = DapperUtil.Query<SWfsBrandNavigation>("ComBeziWfs_SWfsBrandNavigation_Read", new { BrandNo = brandNo });
            return result;
        }
        public IList<SWfsBrandNavigation> GetNavigateList(string brandNo, string branName)
        {
            var result = DapperUtil.Query<SWfsBrandNavigation>("ComBeziWfs_SWfsBrandNavigation_Read", new { BrandNo = brandNo }).ToList<SWfsBrandNavigation>();
            //result.Insert(0, new SWfsBrandNavigation { NavigationId=0,BrandNo=brandNo,NavigationName=branName,ParentId=0});
            //if (result.Count(p => p.ParentId == 0 && p.BrandNo == brandNo) <= 0)
            //{
            //    //添加导航一级品牌数据
            //    int row = InsertNavigate(new SWfsBrandNavigation() { NavigationName = branName, BrandNo = brandNo, ParentId = 0, DateCreate = DateTime.Now, RefContent = "", VisibleStatus = 1 });
            //    if (row > 0)
            //    {
            //        //读取添加的数据
            //        result = DapperUtil.Query<SWfsBrandNavigation>("ComBeziWfs_SWfsBrandNavigation_Read", new { BrandNo = brandNo });
            //    }
            //}
            return result;
        }
        //根据品牌编号和parentID查询最大的排序值
        public int GetMaxSortByBrandNoAndPID(string brandNo, int parentID)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsBrandNavigation_SelectMaxSort", new { ParentId = parentID, BrandNo = brandNo }).First();
        }
        //递归导航生成HTMl
        public void CreateTree(IEnumerable<SWfsBrandNavigation> list, int pid, StringBuilder sb)
        {
            var childList = list.Where(p => p.ParentId == pid);
            if (childList.Count() > 0)
            {
                sb.Append("<ul>");
                for (int i = 0; i < childList.Count(); i++)
                {
                    if (childList.ElementAt(i).ParentId != 0)
                    {
                        sb.Append("<li class='curr'><p class='tree_link'>" + childList.ElementAt(i).NavigationName + "</p>");
                        sb.Append("<span class='trer_button'><a class='sumit_btn' href=\"javascript:addChild('" + childList.ElementAt(i).NavigationId +
                                        "')\">新建子类</a><a class='sumit_btn' href=\"javascript:EditeName('" + childList.ElementAt(i).NavigationId +
                                         "','" + childList.ElementAt(i).NavigationName + "')\">修改名称</a><a class='sumit_btn' onclick=\"hiddenNav('"
                                         + childList.ElementAt(i).NavigationId
                                         + "',this)\">" + (childList.ElementAt(i).VisibleStatus == 1 ? "隐藏" : "显示") +
                                         "</a><a class='sumit_btn' href=\"javascript:delNav('"
                                         + childList.ElementAt(i).NavigationId + "')\">删除</a><a class='sumit_btn' href=\"" +
                                         (i == 0 ? "javascript:;" : "javascript:movePosition('" + childList.ElementAt(i).NavigationId +
                                         "','" + (childList.ElementAt(i).Sort - 1) + "')") +
                                         "\">上移</a><a class='sumit_btn' href=\"" + (i == (childList.Count() - 1) ? "javascript:;" : "javascript:('"
                                         + childList.ElementAt(i).NavigationId + "','" + (childList.ElementAt(i).Sort + 1) + "')") +
                                         "\">下移</a><a class='sumit_btn' href='/Shangpin/Brand/AboutRelationShip.html?&navNo=" + childList.ElementAt(i).NavigationId + "'>设置关联</a><a class='sumit_btn' href=\"/ShangPin/Brand/ChangeRelationShip.html?id=" + childList.ElementAt(i).NavigationId + "&brandNo=" + childList.ElementAt(i).BrandNo + "\">更改从属</a></span>");
                        CreateTree(list, childList.ElementAt(i).NavigationId, sb);
                        sb.Append("</li>");
                    }
                    else
                    {
                        sb.Append("<li class='curr'><p class='tree_link'>" + childList.ElementAt(i).NavigationName + "</p>");
                        sb.Append("<span class='trer_button'><a class='sumit_btn' href=\"javascript:addChild('" + childList.ElementAt(i).NavigationId +
                                        "')\">新建子类</a><a class='sumit_btn' href='javascript:;'>修改名称</a><a class='sumit_btn' >隐藏</a><a class='sumit_btn' href='javascript:;'>删除</a><a class='sumit_btn' href='javascript:;'>上移</a><a class='sumit_btn' href='javascript:;'>下移</a><a class='sumit_btn' href='javascript:;'>设置关联</a><a class='sumit_btn' href='javascript:;'>更改从属</a></span>");
                        CreateTree(list, childList.ElementAt(i).NavigationId, sb);
                        sb.Append("</li>");
                    }
                }
                sb.Append("</ul>");
            }
        }
        //递归生成层级列表
        public void CreateDropDownTree(IEnumerable<SWfsBrandNavigation> oldList, IList<SWfsBrandNavigation> newList, int pId, int step = 0)
        {
            step++;
            string stemp = "";
            for (int i = 0; i < step; i++)
            {
                stemp += "|—";
            }
            foreach (var item in oldList.Where(p => p.ParentId == pId))
            {
                item.NavigationName = stemp + item.NavigationName;
                newList.Add(item);
                CreateDropDownTree(oldList, newList, item.NavigationId, step);
            }
            step--;
        }
        //增加导航
        public int InsertNavigate(SWfsBrandNavigation obj)
        {
            return DapperUtil.Insert<SWfsBrandNavigation>(obj, true);
        }
        //修改导航名称
        public bool UpdateNavigateName(SWfsBrandNavigation obj)
        {
            return DapperUtil.UpdatePartialColumns<SWfsBrandNavigation>(new { NavigationId = obj.NavigationId, NavigationName = obj.NavigationName });
        }
        //隐藏导航
        public bool HiddenNavigate(SWfsBrandNavigation obj)
        {
            return DapperUtil.UpdatePartialColumns<SWfsBrandNavigation>(new { NavigationId = obj.NavigationId, VisibleStatus = obj.VisibleStatus });
        }
        //上下移动导航
        public bool UpdateNavigatePosition(SWfsBrandNavigation obj)
        {
            return DapperUtil.UpdatePartialColumns<SWfsBrandNavigation>(new { NavigationId = obj.NavigationId, Sort = obj.Sort });
        }
        //删除导航
        public int DeleteNavigate(int NavNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsBrandNavigation_DeleteByNavNo", new { NavigationId = NavNo });
        }
        //更改从属
        public bool UpdateNavigateParent(SWfsBrandNavigation obj)
        {
            return DapperUtil.UpdatePartialColumns<SWfsBrandNavigation>(new { NavigationId = obj.NavigationId, ParentId = obj.ParentId, Sort = obj.Sort });
        }
        //关联设置
        public bool UpdateNavigateRelationShip(SWfsBrandNavigation obj)
        {
            return DapperUtil.UpdatePartialColumns<SWfsBrandNavigation>(new { NavigationId = obj.NavigationId, RefType = obj.RefType, RefContent = obj.RefContent });
        }
        //按ID获取单条导航信息
        public SWfsBrandNavigation GetNavObj(int navNo)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsBrandNavigation>(navNo);
        }
        #endregion

        #region FlagShipStore
        //根据品牌编号获取旗舰店
        public SWfsBrandFlagShipStoreSave GetFlagShipByBrandNO(string brandNO)
        {
            IEnumerable<SWfsBrandFlagShipStoreSave> list = DapperUtil.Query<SWfsBrandFlagShipStoreSave>("ComBeziWfs_SWfsBrandFlagShipStoreSave_ReadByBrandNo", new { BrandNo = brandNO });
            if (list.Count() > 0)
            {
                return list.First();
            }
            else
            {
                return null;
            }
        }
        //根据旗舰店ID查询单个旗舰店
        public SWfsBrandFlagShipStoreSave GetFlagShipObj(int FlagNo)
        {
            var result = DapperUtil.QueryByIdentity<SWfsBrandFlagShipStoreSave>(FlagNo);
            //var result = DapperUtil.Query<SWfsBrandFlagShipStoreSave>("ComBeziWfs_SWfsBrandFlagShipStoreSave_ReadByFlagNo", new { FlagShipStoreId = FlagNo }).FirstOrDefault();
            return result;
        }
        //增加旗舰店信息
        public int InsertFlagShipStore(SWfsBrandFlagShipStoreSave obj)
        {
            if (DapperUtil.Query<int>("ComBeziWfs_SWfsBrandSpecialityStore_IsExistFlagBrandNo", new { BrandNo = obj.BrandNo }).FirstOrDefault() > 0)
            {
                return 1;
            }
            return DapperUtil.Insert<SWfsBrandFlagShipStoreSave>(obj, true);
        }
        //修改旗舰店
        public bool UpdateFlagShipStore(SWfsBrandFlagShipStoreSave obj)
        {
            return DapperUtil.Update<SWfsBrandFlagShipStoreSave>(obj);
        }
        //修改旗舰店发布状态
        public bool UpdateFlagShipStatus(int flagID)
        {
            return DapperUtil.UpdatePartialColumns<SWfsBrandFlagShipStoreSave>(new { FlagShipStoreId = flagID, Status = 1 });
        }
        //发布旗舰店信息
        public int PublishFlagShipStoreInfo(SWfsBrandFlagShipStore obj)
        {
            //查询发布的旗舰店是否存在
            var result = DapperUtil.Query<SWfsBrandFlagShipStoreSave>("ComBeziWfs_SWfsBrandFlagShipStore_ReadByBrandNo", new { BrandNo = obj.BrandNo }).FirstOrDefault();
            if (result != null)
            {
                //修改
                obj.FlagShipStoreId = result.FlagShipStoreId;
                return DapperUtil.Update<SWfsBrandFlagShipStore>(obj) ? 1 : 0;//修改发布数据
            }
            else
            {
                return DapperUtil.Insert<SWfsBrandFlagShipStore>(obj, true);//增加发布数据
            }

        }
        //添加所选商品
        public bool AddAboutProduct(int flagNo, string pIdList, int order)
        {
            if (order == 1)
            {
                return DapperUtil.UpdatePartialColumns<SWfsBrandFlagShipStoreSave>(new { FlagShipStoreId = flagNo, ProductNos1 = pIdList });

            }
            else
            {
                return DapperUtil.UpdatePartialColumns<SWfsBrandFlagShipStoreSave>(new { FlagShipStoreId = flagNo, ProductNos2 = pIdList });

            }
        }
        #endregion

        #region OCS分类接口
        /// <summary>
        /// 男女列表页，品牌商品列表页搜索接口
        /// </summary>
        /// <param name="parm">查询参数</param>
        /// <param name="encode">编码</param>
        /// <returns></returns>
        private string GetListUrl(SearchOCSParm parm)
        {
            StringBuilder str = new StringBuilder();
            str.Append(AppSettingManager.AppSettings["CategoryList"] + "brandNO=" + parm.OCSBrandNO);
            return str.ToString();
        }
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        /// Author:wangtao
        /// Date:2012/8/8
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
        //解析XML封装数据集合
        private IList<OCSInfo> GetOCSList(string xmlStr)
        {
            IList<OCSInfo> list = new List<OCSInfo>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.InnerXml = xmlStr;
            string[] attribute = null;
            XmlNodeList nodeList = xmlDoc.SelectNodes("/result/facets");
            if (nodeList != null && nodeList.Count > 0)
            {
                OCSInfo obj = null;
                foreach (XmlNode OCSitem in nodeList)//OCSList节点facets
                {
                    foreach (XmlNode OCSList in OCSitem.ChildNodes)//OCSList节点facet
                    {
                        foreach (XmlNode item in OCSList.ChildNodes)//OCSList obj
                        {
                            attribute = item.Attributes[0].InnerText.Split('|');
                            if (attribute.Length >= 4)
                            {
                                if (attribute[3] != "0")//不显示
                                {
                                    obj = new OCSInfo();
                                    obj.OCSNO = attribute[0];
                                    obj.OCSName = attribute[1];
                                    obj.OCSParentID = attribute[2];
                                    obj.ChildCount = int.Parse(item.InnerText);
                                    list.Add(obj);
                                }
                                //list.Add(obj);
                            }
                        }
                    }
                }
            }
            return list;
        }
        //根据品牌编号获取列表数据
        public IList<OCSInfo> GetOCSListByBrandNO(string brandNO, string addOCSNO, StringBuilder sb)
        {
            string XmlStr = GetResponse(GetListUrl(new SearchOCSParm() { OCSBrandNO = brandNO }));
            IList<OCSInfo> list = GetOCSList(XmlStr);
            if (!string.IsNullOrEmpty(addOCSNO))
            {
                string[] addOCSNOList = addOCSNO.Split(',');
                foreach (var item in addOCSNOList)
                {
                    if (list.Count(p => p.OCSNO == item) > 0)
                    {
                        sb.Append("<tr><td><input type='hidden' value='" + list.Single(p => p.OCSNO == item).OCSNO + "' name='refidlist'>" + list.Single(p => p.OCSNO == item).OCSName + "</td><td style='text-align: center;'><a onclick='removeObj(this)' href='javascript:;'>删 除</a></td></tr>");
                        list.Single(p => p.OCSNO == item).isAdd = true;
                    }
                    //找出该类的所有父类做标记Isparent
                    foreach (var pitem in list.Where(p => item.IndexOf(p.OCSNO) >= 0))
                    {
                        pitem.isParent = true;
                    }
                }
            }
            return list;
        }
        //递归OCS分类
        public void GetOCSTree(IList<OCSInfo> list, string pId, StringBuilder sb)
        {
            var child = list.Where(p => p.OCSParentID == pId).ToList();

            if (child.Count > 0)
            {
                if (pId == "ROOT")
                {
                    sb.Append("<ul><li class='head'>OCS分类</li>");
                }
                else
                {
                    sb.Append("<ul style='display:none;'>");
                }
                for (int i = 0; i < child.Count; i++)
                {
                    if (list.Count(p => p.OCSParentID == child[i].OCSNO) > 0)//是否有子类
                    {
                        sb.Append("<li><lable><input type='checkbox' value='" + child[i].OCSNO + "' name='" + child[i].OCSNO + "' " + (child[i].isAdd ? "checked='checked'" : null) + "/>" +
                        child[i].OCSName + "</lable><a>" + child[i].OCSNO + "</a>");
                        if (child[i].isAdd || child[i].isParent)
                        {
                            sb.Append("<p>已关联</p>");
                        }
                        GetOCSTree(list, child[i].OCSNO, sb);
                        sb.Append("</li>");
                    }
                    else
                    {
                        sb.Append("<li class='arrow'><lable><input type='checkbox' value='" + child[i].OCSNO + "' name='" + child[i].OCSNO + "' " + (child[i].isAdd ? "checked='checked'" : null) + "/>" +
                        child[i].OCSName + "</lable><a>" + child[i].OCSNO + "</a>");
                        if (child[i].isAdd || child[i].isParent)
                        {
                            sb.Append("<p>已关联</p>");
                        }
                        GetOCSTree(list, child[i].OCSNO, sb);
                        sb.Append("</li>");
                    }

                }
                sb.Append("</ul>");
            }

        }

        #endregion

        #region 商品列表
        /// <summary>
        /// 查询一、二、三、四级分类。2014年10月1日 14:24:56  by liu
        /// </summary>
        public IList<ErpCategory> SelectErpCategoryByParentNo(string parentNo)
        {
            //IList<WfsErpCategory> categorylist = DapperUtil.Query<WfsErpCategory>("ComBeziWfs_WfsErpCategory_ReadFirstClassCategory", new { ParentNo = parentNo }).ToList();
            IList<ErpCategory> categorylist = DapperUtil.Query<ErpCategory>("ComBeziWfs_WfsErpCategory_ReadCategoryByParentNO", new { ParentNo = parentNo }).ToList();
            if (categorylist == null)
            {
                return new List<ErpCategory>();
            }
            return categorylist;
        }
        /// <summary>
        /// 商品列表
        /// </summary>
        /// <param name="gender">性别</param>
        /// <param name="categoryNo">Erp品类编号</param>
        /// <param name="brandNO">品牌编号</param>
        /// <param name="pNOList">产品编号</param>
        /// <param name="keyword">关键字</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public IList<ProductInfo> GetProductList(string gender, string categoryNo, string brandNO, IList<string> pNOList, string keyword, int pageIndex, int pageSize, out int count)
        {

            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            if (pNOList.Count > 0)
            {
                dic.Add("PNOList", pNOList);
            }
            else
            {
                dic.Add("PNOList", "");
            }
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("pageIndex", pageIndex);
            dic.Add("pageSize", pageSize);
            dic.Add("brandNO", !string.IsNullOrEmpty(brandNO) ? brandNO : "");
            IList<ProductInfo> productList = DapperUtil.Query<ProductInfo>("ComBeziWfs_WfsProduct_SelectRelationProductList", dic, new { KeyWord = keyword, pageIndex = pageIndex, pageSize = pageSize, PNOList = pNOList, brandNO = brandNO, CategoryNo = categoryNo, Gender = gender }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_WfsProduct_SelectRelationProductListTotalCount", dic, new { KeyWord = keyword, pageIndex = pageIndex, pageSize = pageSize, PNOList = pNOList, brandNO = brandNO, CategoryNo = categoryNo, Gender = gender }).First();
            return productList;
        }
        #endregion

        #region 旗舰店模板
        //模板列表查询
        public IEnumerable<SWfsBrandFlagShipTemplate> GetTemplateList(int pageIndex, int pageSize, string templateNameORTemplateNO, string templateType, string starttime, string endtime, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("TemplateNameORTemplateNO", string.IsNullOrEmpty(templateNameORTemplateNO) ? "" : templateNameORTemplateNO);
            dic.Add("TemplateType", string.IsNullOrEmpty(templateType) ? "" : templateType);
            dic.Add("StartTime", string.IsNullOrEmpty(starttime) ? "" : starttime);
            dic.Add("EndTime", string.IsNullOrEmpty(endtime) ? "" : endtime);
            if (!string.IsNullOrEmpty(endtime))
            {
                endtime = DateTime.Parse(endtime).AddDays(1).ToShortDateString();
            }
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsBrandFlagShipTemplate_GetFlagShipStoreTemplateCount", dic, new { TemplateNameORTemplateNO = templateNameORTemplateNO, TemplateType = templateType, StartTime = starttime, EndTime = endtime, PageIndex = pageIndex, PageSize = pageSize }).FirstOrDefault();
            return DapperUtil.Query<SWfsBrandFlagShipTemplate>("ComBeziWfs_SWfsBrandFlagShipTemplate_GetFlagShipStoreTemplateList", dic, new { TemplateNameORTemplateNO = templateNameORTemplateNO, TemplateType = templateType, StartTime = starttime, EndTime = endtime, PageIndex = pageIndex, PageSize = pageSize });
        }
        //按ID获取模板
        public SWfsBrandFlagShipTemplate GetTemplateObjByID(int tempID)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsBrandFlagShipTemplate>(tempID);
        }
        //编辑模板
        public int EditeTemplate(SWfsBrandFlagShipTemplate obj)
        {
            if (obj.TemplateName == null)
            {
                return 0;
            }
            if (obj.TemplateNO == null)
            {
                return 0;
            }
            if (obj.TemplateType == 0)
            {
                return 0;
            }
            if (obj.TemplateDirection == null)
            {
                obj.TemplateDirection = "";
            }
            if (obj.TemplatePath == null)
            {
                obj.TemplatePath = "";
            }
            if (obj.CSSPath == null)
            {
                obj.CSSPath = "";
            }
            if (obj.JSPath == null)
            {
                obj.JSPath = "";
            }
            if (obj.TemplateDirection == null)
            {
                obj.TemplateDirection = "";
            }
            obj.CreateDate = DateTime.Now;
            if (obj.TemplateID == 0)//添加
            {
                //判断是否存在该编号
                if (IsExists(obj.TemplateNO) > 0)
                {
                    return -1;
                }
                return DapperUtil.Insert<SWfsBrandFlagShipTemplate>(obj, true);
            }
            else
            {
                return DapperUtil.Update<SWfsBrandFlagShipTemplate>(obj) ? 1 : 0;
            }

        }
        //删除模板
        public int DeleteTemplateByID(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsBrandFlagShipTemplate_DeleteFlagShipStoreTemplateByID", new { TemplateID = id });
        }
        //根据模板路径读取模板文件内容
        public string GetTemplateContent(string tempPath)
        {
            try
            {
                if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(tempPath)))
                {
                    using (FileStream fs = new FileStream(System.Web.HttpContext.Current.Server.MapPath(tempPath), FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }
        //保存模板文件
        public int SaveTemplateContent(string tempPath, string text)
        {
            try
            {
                if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(tempPath)))
                {
                    using (FileStream fs = new FileStream(System.Web.HttpContext.Current.Server.MapPath(tempPath), FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                        {
                            sw.WriteLine(text);
                        }
                    }
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        //查询最大模板编号
        public string GetMaxTemplateNO()
        {
            return DapperUtil.Query<string>("ComBeziWfs_SWfsBrandFlagShipTemplate_GetFlagShipStoreTemplateMaxNO", null).FirstOrDefault();
        }
        //查询是否存在模板编号
        private int IsExists(string templateNo)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsBrandFlagShipTemplate_IsExistsFlagShipStoreTemplateNO", new { TemplateNo = templateNo }).FirstOrDefault();
        }
        //按模板编号查询模板
        public SWfsBrandFlagShipTemplate GetTemplateObjByTempNO(string tempNO)
        {
            return DapperUtil.Query<SWfsBrandFlagShipTemplate>("ComBeziWfs_SWfsBrandFlagShipTemplate_GetFlagShipStoreTemplateByTempNO", new { TemplateNO = tempNO }).FirstOrDefault();
        }
        #endregion

        #region 旗舰店区块编辑
        //按品牌编号获取查询旗舰店
        public SWfsBrandFlagShipStoreMobile GetMobileFlagShipStoreObjByBrandNO(string brandNO)
        {
            return DapperUtil.Query<SWfsBrandFlagShipStoreMobile>("ComBeziWfs_SWfsBrandFlagShipStoreMobile_GetFlagShipStoreMobileByBrandNO", new { BrandNO = brandNO }).FirstOrDefault();
        }
        //修改旗舰店
        public int EditeMobileFlagShipStoreByID(SWfsBrandFlagShipStoreMobile obj)
        {
            if (obj.BrandNO == null)
            {
                return 0;
            }
            if (obj.HtmlCode == null)
            {
                obj.HtmlCode = "";
            }
            if (obj.TemplateNo == null)
            {
                obj.TemplateNo = "";
            }
            obj.CreateDate = DateTime.Now;
            if (obj.FlagShipSotreID == 0)
            {
                if (GetMobileFlagShipStoreObjByBrandNO(obj.BrandNO) != null)//判断是否存在
                {
                    return -1;
                }
                return DapperUtil.Insert<SWfsBrandFlagShipStoreMobile>(obj, true);
            }
            else
            {
                return DapperUtil.Update<SWfsBrandFlagShipStoreMobile>(obj) ? 1 : 0;

            }
        }
        //修改旗舰店模板编号
        public int EditeMobileFlagShipStoreTempNO(int flagID, string tempNO)
        {
            return DapperUtil.UpdatePartialColumns<SWfsBrandFlagShipStoreMobile>(new
            {
                FlagShipSotreID = flagID,
                TemplateNo = tempNO
            }) ? 1 : 0;
        }
        #endregion

        #region 区块编辑
        //发布旗舰店
        public int SaveVenueHtml(int flagID, string htmlcode)
        {
            if (flagID == 0)
            {
                return 0;
            }
            return DapperUtil.UpdatePartialColumns<SWfsBrandFlagShipStoreMobile>(new
            {
                FlagShipSotreID = flagID,
                HtmlCode = htmlcode,
                CreateDate = DateTime.Now,
            }) ? 1 : 0;
        }
        //查询区块列表
        public IEnumerable<SWfsBrandFlagShipStoreRegion> GetRegionRelationInfoByCondition(string brandNO, string templateNO, int regionID)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("RegionID", regionID == 0 ? "" : regionID + "");
            return DapperUtil.Query<SWfsBrandFlagShipStoreRegion>("ComBeziWfs_SWfsBrandFlagShipStoreRegion_GetFlagShipStoreRegionByTemplateNOANDBrandNO", dic, new
            {
                BrandNO = brandNO,
                TemplateNo = templateNO,
                RegionID = regionID
            });
        }
        //检查活动编号
        public string CheckActiveNO(string activeNO)
        {
            return DapperUtil.Query<string>("ComBeziWfs_SWfsNewSubject_CheckSWfsNewSubjectNO", new { SubjectNo = activeNO }).FirstOrDefault();
        }
        //保存区块关联
        public bool SaveRegionRelationInfo(SWfsBrandFlagShipStoreRegion obj)
        {
            if (obj.RegionID <= 0 || obj.BrandNO == null || obj.RelationType <= 0 || string.IsNullOrEmpty(obj.TemplateNo))
            {
                return false;
            }
            if (obj.ImgNO == null)
            {
                obj.ImgNO = "";
            }
            if (obj.RelationContent == null)
            {
                obj.RelationContent = "";
            }
            obj.CreateDate = DateTime.Now;
            //查询关联数据描述
            switch (obj.RelationType)
            {
                case 1://查询名称活动
                    if (!string.IsNullOrEmpty(obj.RelationContent))
                    {
                        SWfsNewSubject subobj = GetNewSubjectByNO(obj.RelationContent);
                        obj.Direction = subobj == null ? "" : subobj.SubjectName;
                    }
                    break;
                case 2:
                    obj.Direction = "";
                    break;
                case 3:
                    if (!string.IsNullOrEmpty(obj.RelationContent))
                    {
                        SWfsBrandNavigation navobj = GetBrandNavigationNO(obj.RelationContent);
                        obj.Direction = navobj == null ? "" : navobj.NavigationName;
                    }
                    break;
                case 4:
                    obj.Direction = "";
                    break;
            }
            if (obj.FlagReigionID == 0)
            {
                //判断是否已近存在该区块的数据
                if (GetRegionRelationInfoByCondition(obj.BrandNO, obj.TemplateNo, obj.RegionID).Count() > 0)
                {
                    return false;
                }
                return DapperUtil.Insert<SWfsBrandFlagShipStoreRegion>(obj, true) > 0 ? true : false;
            }
            else
            {
                return DapperUtil.Update<SWfsBrandFlagShipStoreRegion>(obj);
            }

        }
        //根据编号查询尚品活动
        private SWfsNewSubject GetNewSubjectByNO(string subjectNO)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsNewSubject>(subjectNO);
        }
        //按ID查询导航
        private SWfsBrandNavigation GetBrandNavigationNO(string navNO)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsBrandNavigation>(navNO);
        }
        //查询轮播图
        public IEnumerable<SWfsBrandFlagShipStoreRegion> GetMobileAlterPicList(string brandNO, int regionID)
        {
            return DapperUtil.Query<SWfsBrandFlagShipStoreRegion>("ComBeziWfs_SWfsBrandFlagShipStoreRegion_GetFlagAlterPicListByBranNO", new { BrandNO = brandNO, RegionID = regionID });
        }
        //清空轮播图
        public int ClearMobileAlterPic(string brandNO, int regionID)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsBrandFlagShipStoreRegion_ClearFlagAlterPicByBranNO", new { BrandNO = brandNO, RegionID = regionID });
        }
        #endregion


        /// <summary>
        /// 品牌首页管理——热门品牌管理EP 20141003 by lijia
        /// </summary>
        /// <param name="showName"></param>
        /// <param name="typeId"></param>
        /// <param name="status"></param>
        /// <param name="brandView"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public RecordPage<SWfsBrandIndexInfo> GetHotBrandList(string showName, string typeId, string status, string brandView, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("BrandShowName", showName);
            dic.Add("TypeId", typeId);
            dic.Add("Status", status);
            DynamicParameters param = new DynamicParameters();
            param.Add("BrandView", brandView, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            param.Add("BrandShowName", showName, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            param.Add("TypeId", typeId, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            param.Add("Status", status, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            IEnumerable<SWfsBrandIndexInfo> query = DapperUtil.QueryPaging<SWfsBrandIndexInfo>("ComBeziWfs_SpBrand_SWfsBrandIndex_ListOutlet", pageIndex, pageSize, "SWfsBrandIndex.Sort asc,SWfsBrandIndex.DateCreate ASC", dic, param);
            List<SWfsBrandIndexInfo> list = new List<SWfsBrandIndexInfo>();
            if (query != null && query.Count() > 0)
            {
                list = query.ToList();
            }
            list = (list == null ? new List<SWfsBrandIndexInfo>() : list);
            return PageConvertor.Convert(pageIndex, pageSize, list);
        }

        //修改是否手动
        public int UpdateIsAuto(string BNo, string isAuto)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsBrandExtend_UpdateIsAuto", new { BrandNo = BNo, NaviTypeId = isAuto });
        }

        //修改店铺类型
        public int UpdateShop(string BNo1, string isAuto1)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsBrandExtend_UpdateShop", new { BrandNo = BNo1, BrandTypeId = isAuto1 });
        }

        //修改全部品牌首页管理状态
        public bool UpdateStatus(int indexId, int status)
        {
            return DapperUtil.UpdatePartialColumns<SWfsBrandIndex>(new { IndexId = indexId, Status = status });
            //return DapperUtil.Execute("ComBeziWfs_SWfsBrandIndex_UpdateStatus", new { IndexId = indexId, Status = status });
        }

        //修改轮播图状态
        public bool UpdateASWfsAlterPicStatus(int alterPicId, int status)
        {
            return DapperUtil.UpdatePartialColumns<SWfsAlterPic>(new { AlterPicId = alterPicId, Status = status });
        }

        //修改排序值
        public bool SortUpdate(int indexid, int sort)
        {
            //return DapperUtil.Execute("ComBeziWfs_SWfsBrandIndex_UpdateSort", new { IndexId = indexid, Sort = sort });
            return DapperUtil.UpdatePartialColumns<SWfsBrandIndex>(new { IndexId = indexid, Sort = sort });
        }
        //旗舰店列表
        public IList<SWfsBrandExtendList> GetBrandFlagshipstoreList(int PageIndex, int PageSize, out int count)
        {
            IList<SWfsBrandExtendList> list = DapperUtil.Query<SWfsBrandExtendList>("ComBeziWfs_SWfsBrandExtend_Flagshipstore_List", null).ToList();
            count = list.Count();
            list = list.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
            return list;
        }
        //添加
        public int BrandSpecialityStoreInsert(SWfsBrandSpecialityStore obj)
        {
            //判断是否存在重复
            int count = DapperUtil.Query<int>("ComBeziWfs_SWfsBrandSpecialityStore_IsExistBrandNo", new
            {
                BrandNo = obj.BrandNo
            }).FirstOrDefault();
            if (count == 0)
            {
                return DapperUtil.Insert<SWfsBrandSpecialityStore>(obj, true);
            }
            return 1;
        }

        /// <summary>
        /// 查询热门品牌是否存在 by lijibo
        /// </summary>
        /// <param name="brandNo"></param>
        /// <param name="brandView"></param>
        /// <returns></returns>
        public int GetIsExistBrandNoByBrandNoAndBrandView(string brandNo, int typeId)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsBrandIndex_IsExistBrandNo", new
            {
                TypeId = typeId,
                BrandNo = brandNo
            }).FirstOrDefault();
        }

        /// <summary>
        /// 查询热门品牌是否存在(批量) by lijibo
        /// </summary>
        /// <param name="brandNo"></param>
        /// <param name="brandView"></param>
        /// <param name="batch"></param>
        /// <returns></returns>
        public int GetIsExistBrandNoByBrandNoAndBrandView(string brandNo, int typeId, string batch)
        {
            string[] brandNoS = brandNo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            return DapperUtil.Query<int>("ComBeziWfs_SWfsBrandIndex_IsExistBrandNos", new
            {
                TypeId = typeId,
                BrandNo = brandNoS
            }).FirstOrDefault();

        }

        /// <summary>
        /// 查询热门品牌是否存在(批量) by lijibo
        /// </summary>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        public int GetIsExistBrandNoByBrandNo(string brandNo)
        {
            string[] brandNoS = brandNo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            return DapperUtil.Query<int>("ComBeziWfs_WfsBrand_IsExistBrandNo", new
            {
                BrandNo = brandNoS
            }).FirstOrDefault();

        }

        /// <summary>
        /// 获取热门，旗舰店品牌判断显示个数 by lijibo
        /// </summary>
        /// <param name="brandView"></param>
        /// <returns></returns>
        public int GetCountShowStatusByBrandView(int typeId)
        {
            //判断是否存在重复
            return DapperUtil.Query<int>("ComBeziWfs_SWfsBrandIndex_IsCountShowStatus", new
            {
                TypeId = typeId
            }).FirstOrDefault();
        }

        public int BrandSpecialityStoreEdits(string brandNo, int templateId, string specialityStoreType, string specialityStorePic, string brandIntroduce, DateTime dateCreate, int order)
        {
            if (order == 1)
            {
                return DapperUtil.Insert<SWfsBrandSpecialityStore>(new SWfsBrandSpecialityStore { TemplateId = templateId, SpecialityStoreType = specialityStoreType, SpecialityStorePic = specialityStorePic, BrandIntroduce = brandIntroduce, DateCreate = dateCreate });
            }
            else
            {
                return Convert.ToInt32(DapperUtil.Update<SWfsBrandSpecialityStore>(new SWfsBrandSpecialityStore { TemplateId = templateId, SpecialityStoreType = specialityStoreType, SpecialityStorePic = specialityStorePic, BrandIntroduce = brandIntroduce, DateCreate = dateCreate }));
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        public SWfsBrandSpecialityStore GetSWfsBrandSpecialityStoreByBrandNo(string brandNo)
        {
            return DapperUtil.Query<SWfsBrandSpecialityStore>("ComBeziWfs_SWfsBrandSpecialityStore_Update", new { BrandNo = brandNo }).FirstOrDefault();
        }

        public SWfsBrandSpecialityStore SWfsBrandSpecialityStoreInsert(string brandNo)
        {
            return DapperUtil.Query<SWfsBrandSpecialityStore>("ComBeziWfs_SWfsBrandSpecialityStore_Insert", new { BrandNo = brandNo }).FirstOrDefault();
        }

        public bool BrandSpecialityStoreUpdate(SWfsBrandSpecialityStore obj)
        {
            return DapperUtil.Update<SWfsBrandSpecialityStore>(obj);
        }
        #region 品牌索引
        /// <summary>
        /// 查询所有模块 
        /// </summary>
        /// <returns></returns>
        public List<SWfsBrandModule> GetBrandModules(int brandView = 0)
        {
            return DapperUtil.Query<SWfsBrandModule>("ComBeziWfs_SWfsBrandModule_Select", new { BrandView = brandView }).ToList();
        }

        /// <summary>
        /// 创建品牌索引
        /// </summary>
        /// <param name="brandIndex"></param>
        /// <returns></returns>
        public int SaveBrandIndex(SWfsBrandIndex brandIndex)
        {
            if (brandIndex != null && brandIndex.IndexId == 0)
            {
                return DapperUtil.Insert<SWfsBrandIndex>(brandIndex);
            }
            else
            {
                return DapperUtil.Update<SWfsBrandIndex>(brandIndex) ? 1 : -1;
            }
        }

        public SWfsBrandIndexInfo GetSwfsBrandIndexByIndexId(string brandIndexId)
        {
            if (!string.IsNullOrEmpty(brandIndexId))
            {
                return DapperUtil.Query<SWfsBrandIndexInfo>("ComBeziWfs_SWfsBrandIndex_SelectByBrandIndexId", new { BrandIndexId = brandIndexId }).FirstOrDefault();
            }
            return null;
        }
        #endregion

        //全部品牌的列表
        public IList<SWfsBrandExtendList> AIIBrandsList(int pageIndex, int pageSize, out int count)
        {
            IList<SWfsBrandExtendList> list = DapperUtil.Query<SWfsBrandExtendList>("ComBeziWfs_SWfsBrandExtend_AllBrands_Select", null).ToList();
            count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }
        //品牌预览
        public IList<SWfsBrandExtendList> BrandPreview(string brandNo)
        {
            return DapperUtil.Query<SWfsBrandExtendList>("ComBeziWfs_Brand_SWfsBrandIndex_Preview", new { BrandNo = brandNo }).FilterList<SWfsBrandExtendList>();
        }
        //品牌索引删除
        public int BrandIndexDelete(string indexId)
        {
            return DapperUtil.Execute("ComBeziWfs_Brand_SWfsBrandModule_Delete", new { IndexId = indexId });
        }

        public IList<SWfsBrandExtendList> DigitalAccess(string barndName)
        {
            return DapperUtil.Query<SWfsBrandExtendList>("ComBeziWfs_SWfsBrandExtend_AllBrands_Inquiry", null).ToList();
        }
        //初始化品牌数据
        public int InitialData()
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsBrandExtend_Initializationdata", null);
        }

        #region 轮播图

        //轮播图删除
        public int AlterPicDelete(int alterPicId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsAlterPic_delete", new { AlterPicId = alterPicId });
        }


        //添加轮播图
        public int AlterPicInsert(SWfsAlterPic obj)
        {
            return DapperUtil.Insert<SWfsAlterPic>(obj, true);
        }

        //按ID查询轮播图
        public SWfsAlterPic GetSWfsAlterPicByID(int id)
        {
            return DapperUtil.QueryByIdentity<SWfsAlterPic>(id);
        }

        //修改
        public bool AlterPicUpdate(SWfsAlterPic obj)
        {
            return DapperUtil.Update<SWfsAlterPic>(obj);
        }
        #endregion
        #region 国际高端品牌添加
        //添加
        public int BrandGuoJiCreate(SWfsBrandIndexInfo obj)
        {
            return DapperUtil.Insert<SWfsBrandIndexInfo>(obj);
        }
        public int BrandGuoJiIndex(SWfsBrandIndex brandIndex)
        {
            if (brandIndex != null && brandIndex.IndexId == 0)
            {
                return DapperUtil.Insert<SWfsBrandIndex>(brandIndex);
            }
            else
            {
                return DapperUtil.Update<SWfsBrandIndex>(brandIndex) ? 1 : -1;
            }
        }
        //按ID查询
        public SWfsBrandIndexInfo BrandGuoJiId(int id)
        {
            return DapperUtil.QueryByIdentity<SWfsBrandIndexInfo>(id);
        }
        #endregion
        #region 列表轮播图
        //按ID查询
        public SWfsListAlterGroup GetGroupID(int id)
        {
            return DapperUtil.QueryByIdentity<SWfsListAlterGroup>(id);
        }
        public SWfsListAlterGroup GetGroupbyID(int groupid)
        {
            return DapperUtil.Query<SWfsListAlterGroup>("ComBeziWfs_Brand_SWfsListAlterGroup_ByID", new { GroupId = groupid }).FirstOrDefault();
        }
        //添加
        public int GroupCreate(SWfsListAlterGroup obj)
        {
            return DapperUtil.Insert<SWfsListAlterGroup>(obj, true);
        }
        //修改
        public bool GroupUpdate(SWfsListAlterGroup obj)
        {
            return DapperUtil.Update<SWfsListAlterGroup>(obj);
        }
        //图片添加
        public int Alterinsert(SWfsListAlterPic obj)
        {
            return DapperUtil.Insert<SWfsListAlterPic>(obj, true);
        }
        //图片修改
        public bool AlterUpdate(SWfsListAlterPic obj)
        {
            return DapperUtil.Update<SWfsListAlterPic>(obj);
        }
        //按ID查询
        public SWfsListAlterPic GetAlterID(int id)
        {
            return DapperUtil.QueryByIdentity<SWfsListAlterPic>(id);
        }

        public IEnumerable<SWfsListAlterPic> getSWfsListAlterGroupListID(int groupId)
        {
            return DapperUtil.Query<SWfsListAlterPic>("ComBeziWfs_Brand_SWfsListAlterGroup_ID", new { GroupId = groupId });
        }
        //排重
        public IEnumerable<SWfsListAlterPic> HeavyRow(string linkname, int groupId)
        {
            return DapperUtil.Query<SWfsListAlterPic>("ComBeziWfs_Brand_SWfsListAlterPic_HeavyRow", new { LinkName = linkname, GroupId = groupId });
        }

        //删除
        public int AlterDelete(int groupId)
        {
            return DapperUtil.Execute("ComBeziWfs_Brand_SWfsListAlterGroup_Delete", new { GroupId = groupId });
        }

        //修改状态（分组表）
        public int AlterStatus(int groupId, int satus)
        {
            if (satus == 0)
            {
                //RedisCacheProvider.Instance.Remove("SWfsListAlterPicListByCategory" + category);
            }
            return DapperUtil.Execute("ComBeziWfs_Brand_SWfsListAlterPic_Status", new { GroupId = groupId, Status = satus });
        }

        public IEnumerable<SWfsListAlterGroup> GetSWfsListAlterGroupList(int pageIndex, int pageSize, out int count)
        {
            IEnumerable<SWfsListAlterGroup> list = DapperUtil.Query<SWfsListAlterGroup>("ComBeziWfs_SWfsListAlterGroupt_GetAlterList", null).ToList();
            count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }
        #endregion

        #region 促销提示
        //修改状态
        public bool UpdatePromotionsStatus(int promotioninfoId, int status)
        {
            return DapperUtil.UpdatePartialColumns<SWfsProductPromotionTip>(new { PromotionInfoId = promotioninfoId, Status = status });
        }
        //删除一条数据
        public int PromotionsDelete(int promotioninfoId)
        {
            return DapperUtil.Execute("ComBeziWfs_Brand_SWfsProductPromotionTip_Delete", new { PromotionInfoId = promotioninfoId });
        }

        //添加
        public int PromotionsCreate(SWfsProductPromotionTip obj)
        {
            return DapperUtil.Insert<SWfsProductPromotionTip>(obj, true);
        }

        //修改
        public bool PromotionsUpdate(SWfsProductPromotionTip obj)
        {
            return DapperUtil.Update<SWfsProductPromotionTip>(obj);
        }
        //按ID查询
        public SWfsProductPromotionTip PromotionsListId(int promotioninfoId)
        {
            return DapperUtil.Query<SWfsProductPromotionTip>("ComBeziWfs_Brand_SWfsProductPromotionTip_ById", new { PromotionInfoId = promotioninfoId }).FirstOrDefault();
        }
        //添加编号
        public int PromotionTipRefCreate(SWfsProductPromotionTipRef obj)
        {
            return DapperUtil.Insert<SWfsProductPromotionTipRef>(obj, true);
        }
        //删除编号
        public int PromotionTipRefDelete(int promotionInfoId)
        {
            return DapperUtil.Execute("ComBeziWfs_Brand_SWfsProductPromotionTipRef_Delete", new { PromotionInfoId = promotionInfoId });
        }
        //按ID查询关联表
        public IEnumerable<SWfsProductPromotionTipRef> PromotionTipRefID(int promotionInfoId)
        {
            return DapperUtil.Query<SWfsProductPromotionTipRef>("ComBeziWfs_Brand_SWfsProductPromotionTipRef_ByID", new { PromotionInfoId = promotionInfoId });
        }
        //修改关联表
        public bool PromotionTipRefUpdate(SWfsProductPromotionTipRef obj)
        {
            return DapperUtil.Update<SWfsProductPromotionTipRef>(obj);
        }

        public SWfsProductPromotionTipRef PromotionTipRefNo(int promotionInfoId)
        {
            return DapperUtil.Query<SWfsProductPromotionTipRef>("ComBeziWfs_Brand_SWfsProductPromotionTipRef_Search", new { PromotionInfoId = promotionInfoId }).FirstOrDefault();
        }
        #endregion
        #region -hbq获取品牌信息
        /// <summary>
        /// 获取品牌信息 根据品牌编号
        /// </summary>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        public SWfsBrandExtendList GetSWfsBrandExtendListByBrandNo(string brandNo)
        {
            return DapperUtil.Query<SWfsBrandExtendList>("ComBeziWfs_SWfsBrandExtend_GetSWfsBrandExtendListByBrandNo", new { BrandNo = brandNo }).FirstOrDefault();
        }
        #endregion

        /// <summary>
        /// Ep改版，根据品牌编号获得品牌信息
        /// </summary>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        public SpBrand GetModel(string brandNo)
        {
            return DapperUtil.Query<SpBrand>("ComBeziWfs_SpBrand_GetByNoOutlet", new { BrandNo = brandNo }).FirstOrDefault();
        }
    }

}
