using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using System.IO;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System.Text.RegularExpressions;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.Xml;
using Shangpin.Ocs.Service.Common;
using Shangpin.Framework.Common.Dapper;


namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsChannelService
    {

        #region 会场
        //获取频道会场列表
        public IEnumerable<SWfsSpChannel> GetChannelList(int pageIndex, int pageSize, string channelNO, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ChannelNO", string.IsNullOrEmpty(channelNO) ? "" : channelNO);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannel_SelectSWfsSpChannelTotalCount", dic, new { ChannelNO = channelNO, PageIndex = pageIndex, PageSize = pageSize }).FirstOrDefault();
            return DapperUtil.Query<SWfsSpChannel>("ComBeziWfs_SWfsSpChannel_SelectSWfsSpChannelList", dic, new { channelNO = channelNO, PageIndex = pageIndex, PageSize = pageSize });
        }
        //按ID获取会场
        public SWfsSpChannel GetSwfsChannelObjByID(int id)
        {
            return DapperUtil.Query<SWfsSpChannel>("ComBeziWfs_SWfsSpChannel_SelectSWfsSpChannelByID", new { ChannelID = id }).FirstOrDefault();
        }
        //编辑频道会场
        public int EditeChannel(SWfsSpChannel obj, string templateNO)
        {
            //数据验证
            if (string.IsNullOrEmpty(obj.ChannelNO))
            {
                return 0;
            }
            if (!string.IsNullOrEmpty(templateNO))
            {
                SWfsSpChannelTemplate templateObj = GetSwfsChannelTemplateObjByNO(templateNO);
                SWfsSpChannelDetail channelDetailObj = null;
                if (templateObj != null)
                {
                    channelDetailObj = GetSWfsSpChannelDetailByChannelNO(obj.ChannelNO);
                    if (channelDetailObj == null)
                    {
                        channelDetailObj = new SWfsSpChannelDetail();
                    }
                    channelDetailObj.ChannelNO = obj.ChannelNO;
                    channelDetailObj.WebTemplateNO = templateNO;
                    channelDetailObj.CssPath = templateObj.CssPath;
                    channelDetailObj.JsPath = templateObj.JsPath;
                    channelDetailObj.CreateDate = DateTime.Now;
                    EditeChannelDetail(channelDetailObj);
                }
            }
            else
            {
                if (obj.ChannelID != 0)
                {
                    SWfsSpChannelDetail channelDetailObj = GetSWfsSpChannelDetailByChannelNO(obj.ChannelNO);
                    if (channelDetailObj != null)
                    {
                        channelDetailObj.WebTemplateNO = "";
                        channelDetailObj.CssPath = "";
                        channelDetailObj.JsPath = "";
                        EditeChannelDetail(channelDetailObj);
                    }
                }
            }

            obj.CreateDate = DateTime.Now;
            if (obj.ChannelID == 0)
            {
                if (IsExistChannelNO(obj.ChannelNO) > 0)
                {
                    return -1;
                }
                return DapperUtil.Insert<SWfsSpChannel>(obj, true);
            }
            else
            {
                if (obj.ChannelID <= 0)
                {
                    return 0;
                }
                return DapperUtil.Update<SWfsSpChannel>(obj) ? 1 : 0;
            }
        }
        //编辑会场模板
        public int EditeChannelDetail(SWfsSpChannelDetail obj)
        {
            if (obj.DetailID == 0)
            {
                return DapperUtil.Insert<SWfsSpChannelDetail>(obj, true);
            }
            else
            {
                return DapperUtil.UpdatePartialColumns<SWfsSpChannelDetail>(new
                {
                    DetailID = obj.DetailID,
                    ChannelNO = obj.ChannelNO,
                    WebTemplateNO = obj.WebTemplateNO,
                    CssPath = obj.CssPath,
                    JsPath = obj.JsPath,
                    CreateDate = obj.CreateDate
                }) ? 1 : 0;
            }
        }
        //查询重复频道编号
        public int IsExistChannelNO(string channelNO)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsChannelContent_IsExistsChannelNO", new { ChannelNO = channelNO }).FirstOrDefault();
        }
        //按频道编号查询频道详情
        public SWfsSpChannelDetail GetSWfsSpChannelDetailByChannelNO(string channelNO)
        {
            return DapperUtil.Query<SWfsSpChannelDetail>("ComBeziWfs_SWfsSpChannelDetail_SelectSWfsSpChannelDetailByChannelNO", new { ChannelNO = channelNO }).FirstOrDefault();
        }
        #endregion

        #region 模板
        //获取模板列表
        public IEnumerable<SWfsSpChannelTemplate> GetChannelTemplateList(int pageIndex, int pageSize, string templateNO, string templateName, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("TemplateName", string.IsNullOrEmpty(templateName) ? "" : templateName);
            dic.Add("TemplateNO", string.IsNullOrEmpty(templateNO) ? "" : templateNO);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelTemplate_SelectSWfsSpChannelTemplateTotalCount", dic, new { TemplateName = templateName, TemplateNO = templateNO, PageIndex = pageIndex, PageSize = pageSize }).FirstOrDefault();
            return DapperUtil.Query<SWfsSpChannelTemplate>("ComBeziWfs_SWfsSpChannelTemplate_SelectSWfsSpChannelTemplateList", dic, new { TemplateName = templateName, TemplateNO = templateNO, PageIndex = pageIndex, PageSize = pageSize });
        }
        //按ID获取模板
        public SWfsSpChannelTemplate GetSwfsChannelTemplateObjByID(int id)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsSpChannelTemplate>(id);
        }
        //按模板编号获取模板的CSS和JS路径
        public SWfsSpChannelTemplate GetSwfsChannelTemplateObjByNO(string tempNO)
        {
            return DapperUtil.Query<SWfsSpChannelTemplate>("ComBeziWfs_SWfsSpChannelTemplate_SelectSWfsSpChannelTemplateByTempNO", new
            {
                TemplateNO = tempNO
            }).FirstOrDefault();
        }
        //编辑模板
        public int EditeSwfsChannelTemplate(SWfsSpChannelTemplate obj)
        {
            //数据验证
            if (string.IsNullOrEmpty(obj.TemplateName))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(obj.TemplateNO))
            {
                return 0;
            }
            if (obj.TemplatePath == null)
            {
                obj.TemplatePath = "";
            }
            if (obj.TemplateDirection == null)
            {
                obj.TemplateDirection = "";
            }
            if (obj.JsPath == null)
            {
                obj.JsPath = "";
            }
            if (obj.CssPath == null)
            {
                obj.CssPath = "";
            }
            if (obj.OcsCssPath == null)
            {
                obj.OcsCssPath = "";
            }
            if (obj.OcsJsPath == null)
            {
                obj.OcsJsPath = "";
            }
            obj.CreateDate = DateTime.Now;
            if (obj.TemplateID == 0)
            {
                if (IsExistsTemplateNO(obj.TemplateNO) > 0)
                {
                    return -1;
                }
                return DapperUtil.Insert<SWfsSpChannelTemplate>(obj, true);
            }
            else
            {
                if (obj.TemplateID == 0)
                {
                    return 0;
                }
                return DapperUtil.Update<SWfsSpChannelTemplate>(obj) ? 1 : 0;
            }
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
        //删除模板
        public int DeleteTemplateByID(int tempID)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelTemplate_DeleteSWfsSpChannelTemplateByID", new { TemplateID = tempID });
        }
        //查询模板编号是否存在
        public int IsExistsTemplateNO(string tempNO)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelTemplate_IsExistTempalateNO", new { TemplateNO = tempNO }).FirstOrDefault();
        }
        #endregion

        #region 会场区块编辑
        //查询会场区块内容列表查询
        public IEnumerable<SWfsSpChannelTemplateRegion> GetRegionRelationInfoByCondition(int relationID, string channelNO,
             string regionID, string relationType, string templateNO)
        {
            if (relationID == 0)
            {
                if (templateNO == null || channelNO == null)
                {
                    return new List<SWfsSpChannelTemplateRegion>();
                }
            }
            var dic = new Dictionary<string, object>();
            dic.Add("RelationID", relationID == 0 ? "" : relationID + "");
            dic.Add("ChannelNO", string.IsNullOrEmpty(channelNO) ? "" : channelNO);
            dic.Add("TemplateNO", string.IsNullOrEmpty(templateNO) ? "" : templateNO);
            dic.Add("RegionID", string.IsNullOrEmpty(regionID) ? "" : regionID);
            dic.Add("RelationType", string.IsNullOrEmpty(relationType) ? "" : relationType);
            return DapperUtil.Query<SWfsSpChannelTemplateRegion>("ComBeziWfs_SWfsSpChannelTemplateRegion_SelectMeetingRelationList", dic, new
            {
                RelationID = relationID,
                ChannelNO = channelNO,
                RegionID = regionID,
                RelationType = relationType,
                TemplateNO = templateNO
            });
        }
        //保存区块
        public bool SaveRegionRelationInfo(SWfsSpChannelTemplateRegion obj)
        {
            if (obj.RegionID <= 0 || obj.ChannelNO == null || obj.RelationType <= 0 || string.IsNullOrEmpty(obj.TemplateNO))
            {
                return false;
            }
            if (obj.ImgNO == null)
            {
                obj.ImgNO = "";
            }
            if (obj.Description == null)
            {
                obj.Description = "";
            }
            if (obj.RelationContent == null)
            {
                obj.RelationContent = "";
            }
            obj.CreateDate = DateTime.Now;
            if (obj.RelationID == 0)
            {
                //判断是否已近存在该区块的数据
                if (GetRegionRelationInfoByCondition(0, obj.ChannelNO, obj.RegionID + "", "", obj.TemplateNO).Count() > 0)
                {
                    return false;
                }
                return DapperUtil.Insert<SWfsSpChannelTemplateRegion>(obj, true) > 0 ? true : false;
            }
            else
            {
                return DapperUtil.Update<SWfsSpChannelTemplateRegion>(obj);
            }
        }
        //获取活动专题分页列表
        public IList<ActiveAndSpecial> GetActiveAndSpecialByPage(int pageIndex, int pageSize, string activeNOAndName,
            string webSource, string activeType, string activeStatus, string activeStartDate, string activeEndDate, out int total)
        {
            Regex reg = new Regex(@"^\d+$");
            var dic = new Dictionary<string, object>();
            dic.Add("ActiveNOAndName", string.IsNullOrEmpty(activeNOAndName) ? "" : activeNOAndName);
            dic.Add("WebSource", string.IsNullOrEmpty(webSource) ? "" : webSource);
            dic.Add("ActiveType", string.IsNullOrEmpty(activeType) ? "" : activeType);
            dic.Add("Status", string.IsNullOrEmpty(activeStatus) ? "" : activeStatus);
            dic.Add("StartTime", string.IsNullOrEmpty(activeStartDate) ? "" : activeStartDate);
            dic.Add("EndTime", string.IsNullOrEmpty(activeEndDate) ? "" : activeEndDate);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsSubjectAndSWfsTopics_SelectChannelActiveANDSpecialCount", dic, new
            {
                ActiveNOAndName = activeNOAndName,
                WebSource = reg.IsMatch(webSource + "") ? int.Parse(webSource) : -1,
                ActiveType = reg.IsMatch(activeType + "") ? int.Parse(activeType) : -1,//活动/专题
                Status = reg.IsMatch(activeStatus + "") ? int.Parse(activeStatus) : -1,
                StartTime = string.IsNullOrEmpty(activeStartDate) ? DateTime.Now : DateTime.Parse(activeStartDate),
                EndTime = string.IsNullOrEmpty(activeEndDate) ? DateTime.Now : DateTime.Parse(activeEndDate).AddDays(1)
            }).FirstOrDefault();
            return DapperUtil.Query<ActiveAndSpecial>("ComBeziWfs_SWfsSubjectAndSWfsTopics_SelectChannelActiveANDSpecialListByPage", dic, new
            {
                pageIndex = pageIndex,
                pageSize = pageSize,
                ActiveNOAndName = activeNOAndName,
                WebSource = reg.IsMatch(webSource + "") ? int.Parse(webSource) : -1,
                ActiveType = reg.IsMatch(activeType + "") ? int.Parse(activeType) : -1,//活动/专题
                Status = reg.IsMatch(activeStatus + "") ? int.Parse(activeStatus) : -1,
                StartTime = string.IsNullOrEmpty(activeStartDate) ? DateTime.Now : DateTime.Parse(activeStartDate),
                EndTime = string.IsNullOrEmpty(activeEndDate) ? DateTime.Now : DateTime.Parse(activeEndDate).AddDays(1)
            }).ToList();
        }
        //发布会场
        public bool SaveVenueHtml(string DetailID, string htmlCode)
        {
            if (DetailID == null || string.IsNullOrEmpty(htmlCode))
            {
                return false;
            }
            return DapperUtil.UpdatePartialColumns<SWfsSpChannelDetail>(new
            {
                DetailID = DetailID,
                WebCode = htmlCode
            });
        }
        #endregion

        #region 促销商品组
        //获取促销商品组列表
        public IEnumerable<SWfsSpChannelProductGroup> GetChannelProductGroupList(string groupName, string status, string startTime, string endTime, string channelNO, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("GroupName", string.IsNullOrEmpty(groupName) ? "" : groupName);
            dic.Add("Status", string.IsNullOrEmpty(status) ? "" : status);
            dic.Add("StartTime", string.IsNullOrEmpty(startTime) ? "" : startTime);
            dic.Add("EndTime", string.IsNullOrEmpty(endTime) ? "" : endTime);
            if (string.IsNullOrEmpty(startTime))
            {
                startTime = DateTime.Now.ToString();
            }
            if (string.IsNullOrEmpty(endTime))
            {
                endTime = DateTime.Now.ToString();
            }
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelProductGroup_SelectSWfsSpChannelProductCount", dic, new { GroupName = groupName, Status = status, StartTime = DateTime.Parse(startTime), EndTime = DateTime.Parse(endTime).AddDays(1), ChannelNO = channelNO }).FirstOrDefault();
            return DapperUtil.Query<SWfsSpChannelProductGroup>("ComBeziWfs_SWfsSpChannelProductGroup_SelectSWfsSpChannelProductGroupList", dic, new { GroupName = groupName, Status = status, StartTime = DateTime.Parse(startTime), EndTime = DateTime.Parse(endTime).AddDays(1), ChannelNO = channelNO, pageIndex = pageIndex, pageSize = pageSize });
        }
        //删除促销商品组
        public int DeleteSWfsSpChannelProductGroupByID(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelProductGroup_DeleteSWfsSpChannelProductGroupByID", new { GroupID = id });
        }
        //按ID获取
        public SWfsSpChannelProductGroup GetSWfsSpChannelProductGroupObjByID(int groupID)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsSpChannelProductGroup>(groupID);
        }
        //增加促销商品组
        public int AddSWfsSpChannelProductGroup(SWfsSpChannelProductGroup obj)
        {
            if (obj.GroupID == 0)
            {
                return DapperUtil.Insert<SWfsSpChannelProductGroup>(obj, true);
            }
            else
            {
                return DapperUtil.Update<SWfsSpChannelProductGroup>(obj) ? 1 : 0;
            }

        }
        //按组ID获取组内产品列表
        public IEnumerable<SkillProductExtends> GetSWfsSpChannelProductList(string isShelf, string gender, string brandNO, string categoryNo, string keyword, int groupID, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNO", brandNO == null ? "" : brandNO);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelProduct_SelectSWfsSpChannelProductCount", dic, new { IsShelf = isShelf, KeyWord = keyword, BrandNO = brandNO, Gender = gender, CategoryNo = categoryNo, GroupID = groupID }).FirstOrDefault();
            return DapperUtil.Query<SkillProductExtends>("ComBeziWfs_SWfsSpChannelProduct_SelectSWfsSpChannelProductList", dic, new { IsShelf = isShelf, KeyWord = keyword, BrandNO = brandNO, Gender = gender, CategoryNo = categoryNo, GroupID = groupID, pageIndex = pageIndex, pageSize = pageSize });
        }
        //按ID删除产品
        public int DeleteSWfsSpChannelProductByID(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelProduct_DeleteSWfsSpChannelProductByID", new { ProductID = id });
        }
        //按ID查询商品
        public SWfsSpChannelProduct GetSWfsSpChannelProductByID(int id)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsSpChannelProduct>(id);
        }
        //上传产品图片
        public int AddProductImg(int id, string imgNo)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSpChannelProduct>(new
            {
                ProductID = id,
                ProductImgNO = imgNo
            }) ? 1 : 0;
        }
        //加载商品列表
        public IList<ProductInfo> GetProductList(string isShelf, string gender, string brandNO, string categoryNo, string keyword, int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNO", brandNO == null ? "" : brandNO);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            IList<ProductInfo> productList = DapperUtil.Query<ProductInfo>("ComBeziWfs_WfsProduct_SelectChannelProductList", dic, new { Gender = gender, BrandNO = brandNO, IsShelf = isShelf, KeyWord = keyword, pageIndex = pageIndex, pageSize = pageSize, CategoryNo = categoryNo }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_WfsProduct_SelectChannelProductListTotalCount", dic, new { Gender = gender, BrandNO = brandNO, IsShelf = isShelf, KeyWord = keyword, pageIndex = pageIndex, pageSize = pageSize, CategoryNo = categoryNo }).First();
            return productList;
        }
        //查询改组内已经添加过的商品编号
        public IEnumerable<string> GetProductNO(int groupID)
        {
            return DapperUtil.Query<string>("ComBeziWfs_SWfsSpChannelProduct_SelectSWfsSpChannelProductNOByGroupID", new { GroupID = groupID });
        }
        //批量添加频道商品
        public int AddProduct(int groupid, string productNO)
        {
            if (groupid == 0)
            {
                return 0;
            }
            string[] newidlist = productNO.Split(',');
            System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            for (int i = 0; i < newidlist.Length; i++)//过滤不合法商品编号
            {
                if (!string.IsNullOrEmpty(newidlist[i].Trim().Replace("\n", null)) && _regex.IsMatch(newidlist[i].Trim().Replace("\n", null)))
                {
                    newidlist[i] = newidlist[i].Trim().Replace("\n", null);
                }
                else
                {
                    newidlist[i] = "0";
                }
            }
            newidlist = newidlist.Where(p => p != "0").ToArray();
            if (newidlist.Length <= 0)
            {
                return 0;
            }
            for (int i = 0; i < newidlist.Length; i++)
            {
                DapperUtil.Insert<SWfsSpChannelProduct>(new SWfsSpChannelProduct()
                {
                    GroupID = groupid,
                    ProductNO = newidlist[i],
                    CreateDate = DateTime.Now,
                    SortValue = 999
                }, true);
            }
            return 1;
        }
        //保存排序
        public int SaveSortProduct(int id, int sortValue)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSpChannelProduct>(new { ProductID = id, SortValue = sortValue }) ? 1 : 0;
        }
        //批量删除
        public int DeleteProductByIdList(string idList)
        {
            if (string.IsNullOrEmpty(idList))
            {
                return 0;
            }
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelProduct_DeleteSWfsSpChannelProductByIDList", new { idList = idList.Split(',').ToArray() });
        }
        #endregion

        #region 品牌组
        //获取频道品牌组
        public IEnumerable<SWfsSpChannelBrandGroup> GetBrandGroupList(string channelNO, int pageIndex, int pageSize, out int total)
        {
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelBrandGroup_GetSWfsSpChannelBrandCount", new { ChannelNO = channelNO }).FirstOrDefault();
            return DapperUtil.Query<SWfsSpChannelBrandGroup>("ComBeziWfs_SWfsSpChannelBrandGroup_GetSWfsSpChannelBrandGroupList", new { ChannelNO = channelNO, pageIndex = pageIndex, pageSize = pageSize });
        }
        //删除品牌分组
        public int DeleteBrandGroupByID(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelBrandGroup_DeleteSWfsSpChannelBrandGroup", new { GroupID = id });
        }
        //修改品牌分组状态(不用啦)
        public int EditeBrandStatus(int id, int status)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSpChannelBrandGroup>(new
            {
                GroupID = id,
                Status = status
            }) ? 1 : 0;
        }
        //按ID获取品牌分组
        public SWfsSpChannelBrandGroup GetSWfsSpChannelBrandGroupObjByID(int id)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsSpChannelBrandGroup>(id);
        }
        //添加品牌分组
        public int EditeBrandGroup(SWfsSpChannelBrandGroup obj)
        {
            if (obj.GroupID == 0)
            {
                if (IsExistsBrandGroupName(obj.GroupName, obj.ChannelNO).Count() > 0)
                {
                    return -1;
                }
                return DapperUtil.Insert<SWfsSpChannelBrandGroup>(obj, true);
            }
            else
            {
                IEnumerable<int> groupid = IsExistsBrandGroupName(obj.GroupName, obj.ChannelNO);
                if (groupid.Count() > 0)
                {
                    if (groupid.Count() > 1)
                    {
                        return -1;
                    }
                    else
                    {
                        if (groupid.ElementAt(0) != obj.GroupID)
                        {
                            return -1;
                        }
                    }
                }
                return DapperUtil.Update<SWfsSpChannelBrandGroup>(obj) ? 1 : 0;
            }
        }
        //查询是否存在重复的品牌组名称
        public IEnumerable<int> IsExistsBrandGroupName(string groupName, string channelNO)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelBrandGroup_IsExistSWfsSpChannelBrandGroupName", new { GroupName = groupName, ChannelNO = channelNO });
        }
        //获取分组内的品牌
        public IEnumerable<SWfsSpChannelBrand> GetBrandByGroupID(int groupid)
        {
            return DapperUtil.Query<SWfsSpChannelBrand>("ComBeziWfs_SWfsSpChannelBrand_GetSWfsSpChannelBrandImgNO", new { GroupID = groupid });
        }
        //按ID获取品牌
        public SWfsSpChannelBrand GetBrandByID(int id)
        {
            if (id == 0)
            {
                return new SWfsSpChannelBrand();
            }
            return DapperUtil.QueryByIdentityWithNoLock<SWfsSpChannelBrand>(id);
        }
        //编辑品牌
        public decimal EditeBrand(SWfsSpChannelBrand obj)
        {
            if (obj.BrandID == 0)
            {
                return DapperUtil.Query<decimal>("ComBeziWfs_SWfsSpChannelBrand_GetSWfsSpChannelBrandID", new
                {
                    GroupID = obj.GroupID,
                    BrandName = obj.BrandName,
                    ImgLink = obj.ImgLink,
                    ImgNO = obj.ImgNO,
                    SortValue = obj.SortValue
                }).FirstOrDefault();
            }
            else
            {
                obj.CreateDate = DateTime.Now;
                DapperUtil.Update<SWfsSpChannelBrand>(obj);
                return obj.BrandID;
            }
        }
        //查询分组下的品牌个数
        public int GetBrandImgCountByGroupID(int groupID)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelBrand_GetSWfsSpChannelBrandImgCount", new { GroupID = groupID }).FirstOrDefault();
        }

        /// <summary>
        /// 修改品牌分组状态
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="satus"></param>
        /// <returns></returns>
        public int UpdateBrandGroupStatus(int groupId, int status, string channelno)
        {
            if (status == 1)
            {
                if (GetBrandImgCountByGroupID(groupId) < 10)
                {
                    return -1;
                }
            }
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelBrandGroup_UpdatStatus", new { ChannelNo = channelno, GroupId = groupId, Status = status });
        }

        #endregion

        #region 频道品牌轮播图
        //按ID获取轮播图分组
        public SWfsListAlterGroup GetBrandAlterGroupObj(int groupID)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsListAlterGroup>(groupID);
        }
        //增加品牌轮播分组
        public int EditeBrandAlerPicGroupObj(SWfsListAlterGroup obj)
        {
            if (obj.GroupId == 0)
            {
                if (obj.Category.IndexOf("A01") >= 0)
                {
                    obj.Gender = 0;
                }
                else
                {
                    obj.Gender = 1;
                }
                return DapperUtil.Insert<SWfsListAlterGroup>(obj, true);
            }
            else
            {
                return DapperUtil.Execute("ComBeziWfs_SWfsListAlterGroup_OpenBrandAlterPicGroup", new 
                {
                    GroupId = obj.GroupId,
                    GroupName = obj.GroupName,
                    Status = obj.Status,
                    ChannelNO=obj.Category
                });
            }

        }
        //删除品牌轮播分组
        public int DeleteBrandAlterPicGroup(int id)
        {
            if (id == 0)
            {
                return 0;
            }
            return DapperUtil.Execute("ComBeziWfs_SWfsListAlterGroup_DeleteBrandAlterPicGroupByID", new
            {
                GroupId = id
            });
        }
        //获取频道轮播分组列表
        public IEnumerable<SWfsListAlterGroup> GetBrandAlterGroup(string channelno,string groupName, string status, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("GroupName", string.IsNullOrEmpty(groupName) ? "" : groupName);
            dic.Add("Status", string.IsNullOrEmpty(status) ? "" : status);
            dic.Add("Category", string.IsNullOrEmpty(channelno) ? "" : channelno);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsListAlterGroup_GetBrandAlterPicGroupCount", dic, new { Category=channelno, GroupType = 2, GroupName = groupName, Status = status }).FirstOrDefault();
            return DapperUtil.Query<SWfsListAlterGroup>("ComBeziWfs_SWfsListAlterGroup_GetBrandAlterPicGroupList", dic, new { Category = channelno, GroupType = 2, GroupName = groupName, Status = status, pageIndex = pageIndex, pageSize = pageSize });
        }
        //按ID获取品牌轮播图片
        public SWfsListAlterPic GetBrandAlterPicObj(int id)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsListAlterPic>(id);
        }
        //编辑品牌轮播图片
        public int EditeBrandAlterObj(SWfsListAlterPic obj)
        {
            if (obj.AlterPicId == 0)
            {
                return DapperUtil.Insert<SWfsListAlterPic>(obj, true);
            }
            else
            {
                return DapperUtil.UpdatePartialColumns<SWfsListAlterPic>(new
                {
                    AlterPicId = obj.AlterPicId,
                    LargePicture = obj.LargePicture,
                    LinkName = obj.LinkName,
                    AlterAddress = obj.AlterAddress
                }) ? 1 : 0;
            }
        }
        //获取频道轮播分组内图片列表
        public IEnumerable<SWfsListAlterPic> GetBrandAlterPicList(int groupID)
        {
            return DapperUtil.Query<SWfsListAlterPic>("ComBeziWfs_SWfsListAlterGroup_GetBrandAlterPicList", new { GroupId = groupID });
        }
        //删除品牌轮播图片
        public int DelteBrandAlterPic(int id)
        {
            if (id == 0)
            {
                return 0;
            }
            return DapperUtil.Execute("ComBeziWfs_SWfsListAlterPic_DeleteBrandAlterPicByID", new
            {
                AlterPicId = id
            });
        }
        #endregion

        #region 频道头图管理

        /// <summary>
        /// 添加头图
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int ChannelAdverCreate(SWfsSpChannelAdver obj)
        {
            return DapperUtil.Insert<SWfsSpChannelAdver>(obj, true);
        }

        /// <summary>
        /// 修改头图
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ChannelAdverUpdate(SWfsSpChannelAdver obj)
        {
            return DapperUtil.Update<SWfsSpChannelAdver>(obj);
        }

        /// <summary>
        /// 根据ID查询一条数据
        /// </summary>
        /// <param name="adverId"></param>
        /// <returns></returns>
        public SWfsSpChannelAdver ChannelAdver_BYID(int adverId)
        {
            return DapperUtil.Query<SWfsSpChannelAdver>("ComBeziWfs_SWfsSpChannelAdver_BYID", new { AdverID = adverId }).FirstOrDefault();
        }

        /// <summary>
        /// 删除一条头图
        /// </summary>
        /// <param name="adverId"></param>
        /// <returns></returns>
        public int ChannelAdverDelete(int adverId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelAdver_Delete", new { AdverID = adverId });
        }

        /// <summary>
        /// 修改头图状态
        /// </summary>
        /// <param name="adverId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int ChannelAdverStatus(int adverId, int status, string channelno)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelAdver_UpdateStatus", new { AdverID = adverId, Status = status, ChannelNo = channelno });
        }
        #endregion

        #region 频道轮播
        /// <summary>
        /// 排重品类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IEnumerable<SWfsListAlterGroup> GoupByCategory(string category)
        {
            return DapperUtil.Query<SWfsListAlterGroup>("ComBeziWfs_SWfsListAlterGroup_BYCategory", new { Category = category });
        }
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="satus"></param>
        /// <returns></returns>
        public int AlterStatus(int groupId, int satus, string category)
        {
            //if (satus == 0)
            //{
            //    //RedisCacheProvider.Instance.Remove("SWfsListAlterPicListByCategory" + category);
            //}
            return DapperUtil.Execute("ComBeziWfs_SWfsListAlterGroup_UpdateStatus", new { Category = category, GroupId = groupId, Status = satus });
        }
        #endregion

        #region 频道OCS分类推荐
        //根据频道编号获取推荐的OCS分类
        public IEnumerable<SWfsCategoryExtends> GetRecommendCategory(string channelNO)
        {
            //获取频道编号下的所有OCS分类
            IEnumerable<SWfsCategoryExtends> recommendCategory = DapperUtil.Query<SWfsCategoryExtends>("ComBeziWfs_SWfsSpChannelRecommendCategory_GetCategoryByChannelNO", new
            {
                ChannelNO = channelNO
            });
            return recommendCategory;
        }
        //查询已经添加推荐的分类
        public IEnumerable<SWfsCategoryExtends> GetInsertCategory(string channelNO)
        {
            return DapperUtil.Query<SWfsCategoryExtends>("ComBeziWfs_SWfsSpChannelRecommendCategory_GetRecommentCategoryByChannelNO", new
            {
                ChannelNO = channelNO
            });
        }
        //添加推荐分类
        public int AddRecommendCategory(string categoryNo, string channelNO)
        {
            int total = 0;
            if (!string.IsNullOrEmpty(categoryNo))
            {
                string[] addCategoryNOList = categoryNo.Split(',');
                IEnumerable<SWfsCategoryExtends> recommendCategorylist = GetInsertCategory(channelNO);
                for (int i = 0; i < addCategoryNOList.Length; i++)
                {
                    if (recommendCategorylist.Count(p=>p.CategoryNo==addCategoryNOList[i])>0)
                    {
                        continue;
                    }
                    total += DapperUtil.Insert<SWfsSpChannelRecommendCategory>(new SWfsSpChannelRecommendCategory
                    {
                        ChannelNO = channelNO,
                        CategoryNO = addCategoryNOList[i],
                        Status = 1,
                        SortValue = 999,
                        CreateDate = DateTime.Now
                    }, true);
                }
            }
            return total;
        }
        //取消推荐分类
        public int CancelRecommendCategory(string idList)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelRecommendCategory_CancelRecommentCategory", new
            {
                RecommendCategoryID = idList.Split(',')
            });
        }
        //保存分类的排序
        public bool SaveCategorySortValue(int id, int sortValue) 
        {
            return DapperUtil.UpdatePartialColumns<SWfsSpChannelRecommendCategory>(new 
            {
                RecommendCategoryID=id,
                SortValue=sortValue
            });
        }
        #endregion

        #region 频道页搜索接口
        /// <summary>
        /// 男女列表页，品牌商品列表页搜索接口
        /// </summary>
        /// <param name="parm">查询参数</param>
        /// <param name="encode">编码</param>
        /// <returns></returns>
        private string GetListUrl(SearchOCSParm parm)
        {
            StringBuilder str = new StringBuilder();
            str.Append(AppSettingManager.AppSettings["SearchInterFaceUrl"] + "/shangpin/ListFilters?categoryNO=" + parm.OCSNO);
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
        //根据频道编号查找三级和四级OCS分类
        public IList<OCSInfo> GetOCSList(string channelNO)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.InnerXml = GetResponse(GetListUrl(new SearchOCSParm { OCSNO = channelNO }));
            IList<OCSInfo> list = new List<OCSInfo>();
            OCSInfo obj = null;
            string[] attribute = null;
            XmlNodeList nodeList = xmlDoc.SelectNodes("result/facets/facet[@name='CLv3']/item");
            if (nodeList != null)
            {
                foreach (XmlNode item in nodeList)
                {
                    attribute = item.Attributes[0].InnerText.Split('|');
                    if (attribute.Length >= 4)
                    {
                        if (attribute[3] != "0")
                        {
                            obj = new OCSInfo();
                            obj.OCSNO = attribute[0];
                            obj.OCSName = attribute[1];
                            obj.OCSParentID = attribute[2];
                            obj.ChildCount = int.Parse(item.InnerText);
                            obj.OCSLevel = 3;
                            list.Add(obj);
                        }
                    }
                }
            }
            nodeList = xmlDoc.SelectNodes("result/facets/facet[@name='CLv4']/item");
            if (nodeList != null)
            {
                foreach (XmlNode item in nodeList)
                {
                    attribute = item.Attributes[0].InnerText.Split('|');
                    if (attribute.Length >= 4)
                    {
                        if (attribute[3] != "0")
                        {
                            obj = new OCSInfo();
                            obj.OCSNO = attribute[0];
                            obj.OCSName = attribute[1];
                            obj.OCSParentID = attribute[2];
                            obj.ChildCount = int.Parse(item.InnerText);
                            obj.OCSLevel = 3;
                            list.Add(obj);
                        }
                    }
                }
            }
            return list;
        }
        //根据频道编号获取品牌
        public IList<ChannelRecommendBrandExtends> GetBrandByChannelNO(string channelNO)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.InnerXml = GetResponse(GetListUrl(new SearchOCSParm { OCSNO = channelNO }));
            IList<ChannelRecommendBrandExtends> list = new List<ChannelRecommendBrandExtends>();
            ChannelRecommendBrandExtends obj = null;
            string[] attribute = null;
            XmlNodeList nodeList = xmlDoc.SelectNodes("result/facets/facet[@name='Brand']/item");
            if (nodeList != null)
            {
                foreach (XmlNode item in nodeList)
                {
                    attribute = item.Attributes[0].InnerText.Split('|');
                    if (attribute.Length > 2)
                    {
                        obj = new ChannelRecommendBrandExtends();
                        obj.BrandNO = attribute[0];
                        obj.BrandEnName = attribute[1];
                        obj.BrandCnName = attribute[2];
                        obj.ChannelNO = channelNO;
                        list.Add(obj);
                    }
                }
            }
            return list;
        }
        #endregion

        #region 品牌分类推荐
        //获取已经推荐的品牌
        public IList<ChannelRecommendBrandExtends> GetRecommendBrand(string channelNO)
        {
            IList<ChannelRecommendBrandExtends> searchBrandList = GetBrandByChannelNO(channelNO);
            ChannelRecommendBrandExtends obj = null;
            IList<ChannelRecommendBrandExtends> recommendBrandList = DapperUtil.Query<ChannelRecommendBrandExtends>("ComBeziWfs_SWfsSpChannelRecommendBrand_GetRecommentBrand", new
            {
                ChannelNO = channelNO
            }).ToList();
            for (int i = 0; i < recommendBrandList.Count; i++)
            {
                if (searchBrandList.Count(p => p.BrandNO == recommendBrandList[i].BrandNO) == 1)
                {
                    obj = searchBrandList.Single(p => p.BrandNO == recommendBrandList[i].BrandNO);
                    obj.RecommendBrandID = recommendBrandList[i].RecommendBrandID;
                    obj.SortValue = recommendBrandList[i].SortValue;
                }
                else
                {
                    if (searchBrandList.Count(p=>p.BrandNO==recommendBrandList[i].BrandNO) == 0)
                    {
                        obj = recommendBrandList[i];
                        obj.SortValue = -1;
                        searchBrandList.Add(obj);
                    }
                }
            }
            searchBrandList = searchBrandList.OrderByDescending(p => p.SortValue).ToList();
            return searchBrandList;
        }
        //添加频道品牌
        public int AddRecommendBrand(SWfsSpChannelRecommendBrand obj)
        {
            return DapperUtil.Insert<SWfsSpChannelRecommendBrand>(obj, true);
        }
        //获取已经添加的推荐品牌
        public IList<ChannelRecommendBrandExtends> GetAddRecommendBrand(string channelNo)
        {
            return  DapperUtil.Query<ChannelRecommendBrandExtends>("ComBeziWfs_SWfsSpChannelRecommendBrand_GetRecommentBrand", new
            {
                ChannelNO = channelNo
            }).ToList();
        }
        //取消推荐品牌
        public int CancelRecommendBrand(string idList)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpChannelRecommendBrand_CancelRecommendBrand", new
            {
                RecommendBrandID = idList.Split(',')
            });
        }
        //修改排序
        public bool SaveBrandSortValue(int id, int sortValue)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSpChannelRecommendBrand>(new
            {
                RecommendBrandID = id,
                SortValue = sortValue
            });
        }
        #endregion

        #region 频道推荐标题
        /// <summary>
        /// 频道推荐标题
        /// </summary>
        /// <param name="channelNo">频道编号</param>
        /// <returns></returns>
        public IList<SWfsSpChannelRecommendLink> GetSWfsSpChannelRecommendLinkList(string channelNo)
        {
             IList <SWfsSpChannelRecommendLink > recommendChannelLinkList = DapperUtil.Query<SWfsSpChannelRecommendLink>("ComBeziWfs_SWfsSpChannelRecommendLink_SelectByChannelNoList", new
            {
                ChannelNO = channelNo
            }).ToList();
              return recommendChannelLinkList;
        }

        /// <summary>
        /// 频道推荐标题
        /// </summary>
        /// <param name="channelNo">频道编号</param>
        /// <returns></returns>
        public SWfsSpChannelRecommendLink GetSWfsSpChannelRecommendLinkSingle(string recommendLinkID)
        {
            SWfsSpChannelRecommendLink recommendChannelLinkSingle = DapperUtil.Query<SWfsSpChannelRecommendLink>("ComBeziWfs_SWfsSpChannelRecommendLink_SelectByRecommendLinkID", new
            {
                RecommendLinkID = recommendLinkID
            }).FirstOrDefault();
            return recommendChannelLinkSingle;
        }
        /// <summary>
        /// 频道推荐标题编辑
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int EditeSWfsSpChannelRecommendLink(SWfsSpChannelRecommendLink obj)
        {
            if (obj.RecommendLinkID == 0)
            {
                if (!string.IsNullOrEmpty(obj.LinkName))
                {
                 return DapperUtil.Insert<SWfsSpChannelRecommendLink>(obj, true); 
                }
            }
            else
            {
                return DapperUtil.UpdatePartialColumns<SWfsSpChannelRecommendLink>(obj) ? 1 : 0;
            }
            return 0;
        }
        /// <summary>
        /// 频道推荐标题
        /// </summary>
        /// <param name="channelNo">频道编号</param>
        /// <returns></returns>
        public IList<SWfsSpChannelRecommendLink> GetSWfsSpChannelRecommendLinkByHomePageList(string parentID ,string channelNO)
        {
            return DapperUtil.Query<SWfsSpChannelRecommendLink>("ComBeziWfs_SWfsSpChannelRecommendLink_SelectByRecommendLinkIDList", new { ParentID = parentID, ChannelNO = channelNO }).ToList();
        }
        #endregion

        #region 上新活动图管理
        /// <summary>
        /// 获取上新活动图信息
        /// </summary>
        /// <param name="picName">图片名称</param>
        /// <param name="picPosition">轮播位</param>
        /// <param name="status">活动状态</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">当前显示页</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="count">总数</param>
        /// <returns></returns>
        public IList<SWfsNewAlterPicInfo> GetNewAlterPictureList(string picName, string picPosition, string status, string startTime, string endTime, int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("PicName", (string.IsNullOrEmpty(picName) || picName == "名称") ? "" : picName);
            dic.Add("PicPos", string.IsNullOrEmpty(picPosition) ? "" : picPosition);
            dic.Add("Status", string.IsNullOrEmpty(status) ? "" : status);
            dic.Add("DateBegin", string.IsNullOrEmpty(startTime) ? "" : startTime);
            dic.Add("DateEnd", string.IsNullOrEmpty(endTime) ? "" : endTime);
            IList<SWfsNewAlterPicInfo> list = DapperUtil.Query<SWfsNewAlterPicInfo>("ComBeziWfs_SWfsNewAlterPicture_SelectAllList", dic, new
            {
                PicName = picName,
                PicPos = picPosition,
                Status = status,
                DateBegin = string.IsNullOrEmpty(startTime) ? "" : startTime,
                DateEnd = string.IsNullOrEmpty(endTime) ? "" : endTime
            }).ToList();
            count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }

        /// <summary>
        /// 根据id获取品牌信息
        /// </summary>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        public BrandInfo GetBrandInfoById(string brandNo)
        {
            return DapperUtil.Query<BrandInfo>("ComBeziWfs_WfsBrand_SelectBrandByNo", new { BrandNo = brandNo }).FirstOrDefault();
        }

        /// <summary>
        /// 根据id获取上新活动图信息
        /// </summary>
        /// <param name="id">活动图id</param>
        /// <returns></returns>
        public SWfsNewAlterPicInfo GetNewAlterPicInfoById(int id)
        {
            return DapperUtil.Query<SWfsNewAlterPicInfo>("ComBeziWfs_SWfsNewAlterPicture_SelectById", new { PictureId = id }).FirstOrDefault();
        }

        /// <summary>
        /// 添加上新活动图信息
        /// </summary>
        /// <param name="model"></param>
        public void Insert(SWfsNewAlterPicture model) {
            DapperUtil.Insert<SWfsNewAlterPicture>(model);
        }

        /// <summary>
        /// 编辑上新活动图信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(SWfsNewAlterPicInfo model) {
            SWfsNewAlterPicture picInfo = new SWfsNewAlterPicture();
            picInfo.PictureId = model.PictureId;
            picInfo.PictureName = model.PictureName;
            picInfo.Position = model.Position;
            picInfo.BrandNo = model.BrandNo;
            picInfo.PcPictureNo = model.PcPictureNo;
            picInfo.PcPictureLinkUrl = model.PcPictureLinkUrl;
            picInfo.MobilePictureNo = model.MobilePictureNo;
            picInfo.MobilePictureLinkUrl = model.MobilePictureLinkUrl;
            picInfo.BeginDate = model.BeginDate;
            picInfo.MobilePictureType = model.MobilePictureType;
            picInfo.Status = model.Status;
            picInfo.DataStatus = model.DataStatus;
            picInfo.OperatorUserId = model.OperatorUserId;
            picInfo.DateCreate = model.DateCreate;
            return DapperUtil.Update<SWfsNewAlterPicture>(picInfo);
        }

        /// <summary>
        /// 开启或关闭操作
        /// </summary>
        /// <param name="id">上新活动图id</param>
        /// <param name="value">状态:0关闭;1开启</param>
        public void Update(string id, int value) {
            DapperUtil.Execute("ComBeziWfs_SWfsNewAlterPicture_UpdateWfsNewAlterPicById", new { PictureId = id, Status = value });
        }

        /// <summary>
        /// 删除某一上新活动图信息
        /// </summary>
        /// <param name="id"></param>
        public void Del(string id) {
            DapperUtil.Execute("ComBeziWfs_SWfsNewAlterPicture_DelWfsNewAlterPicById", new { PictureId = id }); 
        }

        /// <summary>
        /// 根据品牌编号和时间获取活动图信息
        /// </summary>
        /// <param name="brandno"></param>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public List<SWfsNewAlterPicInfo> GetNewAlterPicInfoByNo(string brandno, string starttime)
        { 
            DynamicParameters dp = new DynamicParameters();
            dp.Add("BrandNo", brandno, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            dp.Add("BeginDate", starttime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            return DapperUtil.Query<SWfsNewAlterPicInfo>("ComBeziWfs_SWfsNewAlterPicture_SelectWfsNewAlterPicByNo", dp).ToList();
        }
        
        #endregion

        #region 上新品牌管理
        public IList<SWfsNewBrandInfo> GetAllNewBrandsList()
        {
            try
            {
                //删除上新品牌表中不符合显示条件的品牌  新加，by liulang 2014-10-13   
                DapperUtil.Query<int>("ComBeziWfs_DeleteNOAccordingConditionsBrand").FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
            return DapperUtil.Query<SWfsNewBrandInfo>("ComBeziWfs_SWfsNewBrandManage_SelectAllNewBrands").ToList();
        }
        public SWfsNewBrandManage GetNewBrandInfoByBrandNo(string brandNo)
        {
            return DapperUtil.Query<SWfsNewBrandManage>("ComBeziWfs_SWfsNewBrandManage_SelectNewBrandInfoByBrandNo", new { BrandNo = brandNo }).SingleOrDefault();
        }
        public int GetIsBrandNOAccordingConditions(string brandNo) 
        {
           return DapperUtil.Query<int>("ComBeziWfs_IsBrandNOAccordingConditions", new { BrandNo = brandNo }).SingleOrDefault();        
        }
        /// <summary>
        /// 获取某日期下的上新品牌信息
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public IList<SWfsNewBrandInfo> GetNewBrandListByWeekDays(string week,string keyWord, string startTime, string endTime)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            DateTime dt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            string preTime = dt.AddDays(-15).ToString("yyyy-MM-dd"); ;//当前时间往前推15天
            string nowTime = dt.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");//当天时间
            dic.Add("KeyWord", (string.IsNullOrEmpty(keyWord) || keyWord == "编号/名称") ? "" : keyWord);
            dic.Add("DateBegin", string.IsNullOrEmpty(startTime) ? "" : startTime);
            dic.Add("DateEnd", string.IsNullOrEmpty(endTime) ? "" : endTime);
            return DapperUtil.Query<SWfsNewBrandInfo>("ComBeziWfs_SWfsNewBrandManage_SelectNewBrandListByWeek", dic, new
            {
                KeyWord = keyWord,
                DateBegin = startTime,
                DateEnd = endTime,
                WeekDays = week,
                NowTime = nowTime,
                PreTime = preTime
            }).ToList();
        }
        /// <summary>
        /// 添加上新品牌信息
        /// </summary>
        /// <param name="model"></param>
        public void AddNewBrand(SWfsNewBrandManage model)
        {
            DapperUtil.Insert<SWfsNewBrandManage>(model); 
        }
        //编辑排序
        public bool UpdateSort(SWfsNewBrandManage model)
        {
            return DapperUtil.Update<SWfsNewBrandManage>(model);
        }
        /// <summary>
        /// 删除某日期下的所有上新品牌
        /// </summary>
        /// <param name="week"></param>
        public void DelAllBrandByWeek(string week) 
        {
            DapperUtil.Execute("ComBeziWfs_SWfsNewBrandManage_DelWfsNewBrandByWeek", new { WeekDays = week }); 
        }
        /// <summary>
        /// 删除某日期下的某一上新品牌
        /// </summary>
        /// <param name="id"></param>
        public void DelBrandById(string id)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsNewBrandManage_DelWfsNewBrandById", new { NewBrandId = id });
        }
        /// <summary>
        /// 获取某日期下的上新商品数量大于10的商品信息
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public IList<SWfsNewProductInfo> GetNewProductList(string week)
        {
            return DapperUtil.Query<SWfsNewProductInfo>("ComBeziWfs_SWfsNewProductManage_SelectAllByWeekDays", new { WeekDays = week }).ToList();
        }
        /// <summary>
        /// 根据日期和品牌编号查询上新商品信息列表
        /// </summary>
        /// <param name="week"></param>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        public IList<SWfsNewProductManage> GetNewProductListByBrandNoAndWeek(string week, string brandNo)
        {
            return DapperUtil.Query<SWfsNewProductManage>("ComBeziWfs_SWfsNewProductManage_SelectAllByBrandNoAndWeek", new { BrandNo = brandNo, WeekDays = week }).ToList();
        }

        /// <summary>
        /// 按主键修改上新品牌排序值
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="sortValue">排序值</param>
        /// <returns></returns>
        public bool EditeBrandSortValue(int id, int sortValue)
        {
            return DapperUtil.UpdatePartialColumns<SWfsNewBrandManage>(new
            {
                NewBrandId = id,
                Sort = sortValue
            });
        }

        public SWfsNewBrandManage GetNewBrandManageByNewId(string id)
        {
            return DapperUtil.Query<SWfsNewBrandManage>("ComBeziWfs_SWfsNewBrandManage_SelectWfsNewBrandById", new { NewBrandId = id }).SingleOrDefault();
        }

        public bool SetTopOneSortNewBrand(string id,string week) {
            int minSort = DapperUtil.Query<int>("ComBeziWfs_SWfsNewBrandManage_GetMinSortByWeekDays", new { WeekDays = week }).FirstOrDefault();
            SWfsNewBrandManage model = DapperUtil.Query<SWfsNewBrandManage>("ComBeziWfs_SWfsNewBrandManage_SelectWfsNewBrandById", new { NewBrandId=id}).SingleOrDefault();
            model.Sort = minSort-1;
            return UpdateSort(model);
        }
        #endregion 

    }
}
