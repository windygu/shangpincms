using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;
using System.Text.RegularExpressions;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using System.IO;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Entity.Extenstion.Login;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SwfsVenueService
    {
        #region 模板管理
        //添加模板
        public bool AddTemplate(SWfsMeetingTemplateInfo obj)
        {
            obj.CreateTime = DateTime.Now;
            if (string.IsNullOrEmpty(obj.jsFileName))
            {
                obj.jsFileName = "";
            }
            if (string.IsNullOrEmpty(obj.CssFileName))
            {
                obj.CssFileName = "";
            }
            if (DapperUtil.Insert<SWfsMeetingTemplateInfo>(obj, true) > 0)
            {
                return true;
            }
            return false;
        }
        //修改模板
        public bool EditeTemplate(SWfsMeetingTemplateInfo obj)
        {
            if (string.IsNullOrEmpty(obj.jsFileName))
            {
                obj.jsFileName = "";
            }
            if (string.IsNullOrEmpty(obj.CssFileName))
            {
                obj.CssFileName = "";
            }
            return DapperUtil.Update(obj);
        }
        //删除模板
        public bool DeleteTemplateByID(int tempID)
        {
            if (tempID == 0)
            {
                return false;
            }
            if (DapperUtil.Execute("ComBeziWfs_SWfsMeetingTemplateInfo_DeleteMeetingTemplateInfoByID", new { TemplateID = tempID }) > 0)
            {
                return true;
            }
            return false;
        }
        //查询模板列表
        public IEnumerable<SWfsMeetingTemplateInfo> GetTemplateList(string pageIndex, int pageSize, string tempNO,
            string tempName, string isPreView, string isMobile, out int totalCount)
        {
            Regex reg = new Regex(@"^\d+$");
            var dic = new Dictionary<string, object>();
            dic.Add("tempNO", string.IsNullOrEmpty(tempNO) ? "" : tempNO);
            dic.Add("tempName", string.IsNullOrEmpty(tempName) ? "" : tempName);
            dic.Add("isPreView", string.IsNullOrEmpty(isPreView) ? "" : isPreView);
            dic.Add("isMobile", string.IsNullOrEmpty(isMobile) ? "" : isMobile);
            IEnumerable<SWfsMeetingTemplateInfo> list = DapperUtil.Query<SWfsMeetingTemplateInfo>("ComBeziWfs_SWfsMeetingTemplateInfo_SelectTemplateByPage", dic, new
            {
                pageIndex = reg.IsMatch(pageIndex + "") ? int.Parse(pageIndex) : 1,
                pageSize = pageSize,
                tempNO = tempNO,
                tempName = tempName,
                isPreView = reg.IsMatch(isPreView + "") ? int.Parse(isPreView) : -1,
                isMobile = reg.IsMatch(isMobile + "") ? int.Parse(isMobile) : -1
            });
            totalCount = DapperUtil.Query<int>("ComBeziWfs_SWfsMeetingTemplateInfo_SelectTemplateTotalCount", dic, new
            {
                pageIndex = reg.IsMatch(pageIndex + "") ? int.Parse(pageIndex) : 1,
                pageSize = pageSize,
                tempNO = tempNO,
                tempName = tempName,
                isPreView = reg.IsMatch(isPreView + "") ? int.Parse(isPreView) : -1,
                isMobile = reg.IsMatch(isMobile + "") ? int.Parse(isMobile) : -1
            }).FirstOrDefault();
            dic.Clear();
            dic = null;
            return list;
        }
        //按ID获取模板
        public SWfsMeetingTemplateInfo GetTemplateObjByID(string tempID)
        {
            Regex reg = new Regex(@"^\d+$");
            if (reg.IsMatch(tempID + ""))
            {
                return DapperUtil.QueryByIdentity<SWfsMeetingTemplateInfo>(int.Parse(tempID));
            }
            return null;
        }
        //按编号查询是否存在
        public int IsExistTemplateObjByNO(string tempNO)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsMeetingTemplateInfo_IsExistTemplateNO", new { MettingNo = tempNO }).FirstOrDefault();
        }
        //按编号获取模板信息
        public SWfsMeetingTemplateInfo GetTemplateInfoByNO(string tempNO)
        {
            if (string.IsNullOrEmpty(tempNO))
            {
                return null;
            }
            return DapperUtil.Query<SWfsMeetingTemplateInfo>("ComBeziWfs_SWfsMeetingTemplateInfo_SelectObjByTemplateNO", new { MettingNo = tempNO }).FirstOrDefault();
        }
        //批量删除
        public int DeleteByIdList(string idList)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsMeetingTemplateInfo_DeleteTemplateByIDList", new { idlist = idList.Split(',').ToArray() });
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
                            //sw.Write("fdfddf");
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

        //根据模板文件创建和编辑管理
        public string GetTemplateOrCssContent(string tempPath)
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
                else
                {
                    using (FileStream fs = new FileStream(System.Web.HttpContext.Current.Server.MapPath(tempPath), FileMode.OpenOrCreate))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {

                return null;
            }
        }
        public int SaveTemplateOrCssContent(string tempPath, string text)
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
        #endregion

        #region 模版图片信息
        /// <summary>
        /// 添加模版图片信息 by lijibo
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool AddTemplateImg(SWfsMeetingTemplatePicInfo obj)
        {
            if (DapperUtil.Insert<SWfsMeetingTemplatePicInfo>(obj, true) > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新模版图片信息 by lijibo
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool UpdateTemplateImg(SWfsMeetingTemplatePicInfo obj)
        {
            return DapperUtil.Update(obj);
        }

        /// <summary>
        /// 获取符合条件的模版图片信息 by lijibo
        /// </summary>
        /// <param name="TemplateID">模版id</param>
        /// <param name="Type">css或者js</param>
        /// <param name="FileName">原文件名字</param>
        /// <returns></returns>
        public IList<SWfsMeetingTemplatePicInfo> GetTemplateImgEntity(string TemplateID, float Type, string FileName)
        {
            return DapperUtil.Query<SWfsMeetingTemplatePicInfo>("ComBeziWfs_SWfsMeetingTemplatePicInfo_GetPicInfoListByCondThree", new { TemplateID = TemplateID, Type = Type, FileName = FileName }).ToList();
        }

        /// <summary>
        /// 获取符合条件的模版图片信息 by lijibo
        /// </summary>
        /// <param name="TemplateID">模版id</param>
        /// <param name="Type">css或者js,如果不需要传此参数的话，直接赋值为-2</param>
        /// <returns></returns>
        public IList<SWfsMeetingTemplatePicInfo> GetTemplateImgEntity(string TemplateID, float Type)
        {
            Dictionary<string, object> dicStr = new Dictionary<string, object>();
            dicStr.Add("Type", Type);
            return DapperUtil.Query<SWfsMeetingTemplatePicInfo>("ComBeziWfs_SWfsMeetingTemplatePicInfo_GetPicInfoListByCondTwo",dicStr, new { TemplateID = TemplateID, Type = Type }).ToList();
        }
        #endregion

        #region 会场区块编辑
        /// <summary>
        /// 按ID获取会场信息
        /// </summary>
        /// <param name="venueID">会场主键ID</param>
        /// <returns></returns>
        public SWfsMeetingInfo GetVenueByID(string venueID)
        {
            Regex reg = new Regex(@"^\d+$");
            if (reg.IsMatch(venueID + ""))
            {
                if (venueID == "0")
                {
                    return null;
                }
                return DapperUtil.QueryByIdentity<SWfsMeetingInfo>(int.Parse(venueID));
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 保存编辑后的会场HTML块
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="previeworstart">0预热中/1进行中</param>
        /// <param name="webormobile">0为移动端/1为web端</param>
        /// <returns></returns>
        public bool SaveVenueHtml(SWfsMeetingInfo obj, int previeworstart, int mobileorweb)
        {
            if (obj.MeetingID == 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(obj.MobilePreViewCode))
            {
                return false;
            }
            if (previeworstart == 0)//预热
            {
                if (mobileorweb == 0)//移动端
                {
                    return DapperUtil.UpdatePartialColumns<SWfsMeetingInfo>(new { MeetingID = obj.MeetingID, MobilePreViewCode = obj.MobilePreViewCode });
                }
                else//web端
                {
                    return DapperUtil.UpdatePartialColumns<SWfsMeetingInfo>(new { MeetingID = obj.MeetingID, WebPreViewCode = obj.MobilePreViewCode });
                }
            }
            else//进行中
            {
                if (mobileorweb == 0)//移动端
                {
                    return DapperUtil.UpdatePartialColumns<SWfsMeetingInfo>(new { MeetingID = obj.MeetingID, MobileStartCode = obj.MobilePreViewCode });
                }
                else//web端
                {
                    return DapperUtil.UpdatePartialColumns<SWfsMeetingInfo>(new { MeetingID = obj.MeetingID, WebStartCode = obj.MobilePreViewCode });
                }
            }
        }


        /// <summary>
        /// 新增保存会场HTML块
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="previeworstart">0预热中/1进行中</param>
        /// <param name="webormobile">0为移动端/1为web端</param>
        /// <returns></returns>
        public bool AddNenueHtml(SWfsMeetingInfoHtml obj, int ispre, int ismobile)
        {
            if (obj.ID == 0)
            {
                return false;
            }
            if (obj.TemplateType.Substring(0,1) == "0" && ispre == 0 && ismobile == 0)//移动端预热
            {
                return DapperUtil.UpdatePartialColumns<SWfsMeetingInfoHtml>(new
                {
                    ID = obj.ID,
                    MobilePreViewCode = obj.MobilePreViewCode,
                    TemplateType = obj.TemplateType,
                    IsPublish = obj.IsPublish,
                    UpdateDate = obj.UpdateDate,
                    UpdateUserId = obj.UpdateUserId
                });
            }
            else if (obj.TemplateType.Substring(1,1) == "0" && ispre == 1 && ismobile == 0)//移动端开始
            {
                return DapperUtil.UpdatePartialColumns<SWfsMeetingInfoHtml>(new
                {
                    ID = obj.ID,
                    MobileStartCode = obj.MobileStartCode,
                    TemplateType = obj.TemplateType,
                    IsPublish = obj.IsPublish,
                    UpdateDate = obj.UpdateDate,
                    UpdateUserId = obj.UpdateUserId
                });
            }
            else if (obj.TemplateType.Substring(2,1) == "0" && ispre == 0 && ismobile == 1)//web端预热
            {
                return DapperUtil.UpdatePartialColumns<SWfsMeetingInfoHtml>(new
                {
                    ID = obj.ID,
                    WebPreViewCode = obj.WebPreViewCode,
                    TemplateType = obj.TemplateType,
                    IsPublish = obj.IsPublish,
                    UpdateDate = obj.UpdateDate,
                    UpdateUserId = obj.UpdateUserId
                });
            }
            else //web端开始
            {
                return DapperUtil.UpdatePartialColumns<SWfsMeetingInfoHtml>(new
                {
                    ID = obj.ID,
                    WebStartCode = obj.WebStartCode,
                    TemplateType = obj.TemplateType,
                    IsPublish = obj.IsPublish,
                    UpdateDate = obj.UpdateDate,
                    UpdateUserId = obj.UpdateUserId
                });
            }
        }


  
        
        /// <summary>
        /// 按ID查询会场区块关联
        /// </summary>
        /// <param name="relationID">区块关联表的主键ID</param>
        /// <returns></returns>
        public SWfsMeetingRelationRegion GetMeetingRelationByID(string relationID)
        {
            Regex reg = new Regex(@"^\d+$");
            if (reg.IsMatch(relationID + ""))
            {
                if (relationID == "0")
                {
                    return null;
                }
                return DapperUtil.QueryByIdentity<SWfsMeetingRelationRegion>(int.Parse(relationID));
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 查询会场区块内容列表查询
        /// </summary>
        /// <param name="mainMeetingNO">主会场编号</param>
        /// <param name="meetingNO">会场编号</param>
        /// <param name="regionID">区块ID</param>
        /// <param name="relationType">关联模块类型</param>
        /// <returns></returns>
        public IEnumerable<SWfsMeetingRelationRegionContent> GetRegionRelationInfoByCondition(string mainMeetingNO,
            string meetingNO, string regionID, string relationType, string templateNo)
        {
            Regex reg = new Regex(@"^\d+$");
            var dic = new Dictionary<string, object>();
            dic.Add("MainMeetingNO", string.IsNullOrEmpty(mainMeetingNO) ? "" : mainMeetingNO);
            dic.Add("MeetingNO", string.IsNullOrEmpty(meetingNO) ? "" : meetingNO);
            dic.Add("TemplateNo", string.IsNullOrEmpty(templateNo) ? "" : templateNo);
            dic.Add("RegionID", reg.IsMatch(regionID) ? regionID : "");
            dic.Add("RelationType", reg.IsMatch(relationType) ? relationType : "");
            return DapperUtil.Query<SWfsMeetingRelationRegionContent>("ComBeziWfs_SWfsMeetingRelationRegion_SelectMeetingRelationByCondition", dic, new
            {
                MainMeetingNO = mainMeetingNO,
                MeetingNO = meetingNO,
                RegionID = reg.IsMatch(regionID) ? int.Parse(regionID) : -1,
                RelationType = reg.IsMatch(relationType) ? int.Parse(relationType) : -1,
                TemplateNo = templateNo
            });
        }
        /// <summary>
        /// 保存区块关联
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SaveRegionRelationInfo(SWfsMeetingRelationRegion obj, string oldRelationTypeID, string AddContent, string oldRelationContent)
        {
            if (obj.Direction == null)
            {
                obj.Direction = "";
            }
            if (obj.RelationContent == null)
            {
                obj.RelationContent = "";
            }
            if (obj.ImgNO == null)
            {
                obj.ImgNO = "";
            }
            if (obj.RegionID == 0 || obj.RelationType > 4 || obj.RelationType <= 0)
            {
                return false;
            }
            if (obj.MeetingRelationID == 0)//添加
            {
                if (string.IsNullOrEmpty(obj.MainMeetingNO))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(obj.MettingNO))
                {
                    return false;
                }
                if (obj.RelationType == 0)
                {
                    return false;
                }
                if (obj.RelationType == 1 || obj.RelationType == 2)
                {
                    if (string.IsNullOrEmpty(AddContent))
                    {
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(AddContent))
                {
                    if (obj.RelationType == 1)
                    {
                        Regex reg = new Regex(@"^\d+$");
                        if (!reg.IsMatch(AddContent + ""))
                        {
                            return false;
                        }
                        //UpdateVenueIsSelectByIDList(AddContent, true);
                    }
                    if (obj.RelationType == 2)
                    {
                        Regex reg = new Regex(@"^\d+$");
                        if (!reg.IsMatch(AddContent + ""))
                        {
                            return false;
                        }
                        //UpdateActiveAndSpecialByIDList(AddContent, true);
                        obj.AboutID = int.Parse(AddContent);
                    }
                    obj.RelationContent = AddContent;
                }
                if (GetRelationobjByContent(obj.MettingNO, obj.RegionID, obj.TemplateNO) != null)//判断是否已近存在该区块的数据
                {
                    return false;
                }
                AddOperationLog(obj.MettingNO, (obj.MettingNO == obj.MainMeetingNO) ? 1 : 2, 4, LogActionType.Add, "会场编号为:" + obj.MettingNO + "区块ID为:" + obj.RegionID + "--会场区块编辑");
                return DapperUtil.Insert<SWfsMeetingRelationRegion>(obj, true) > 0 ? true : false;
            }
            else//根据主键主键ID修改
            {
                if (obj.RelationType == 0)
                {
                    return false;
                }
                if (oldRelationTypeID == "0")
                {
                    return false;
                }
                Regex reg = new Regex(@"^\d+$");
                if (!reg.IsMatch(oldRelationTypeID + ""))
                {
                    return false;
                }
                if (obj.RelationType == 1 || obj.RelationType == 2)
                {
                    if (string.IsNullOrEmpty(AddContent))
                    {
                        if (!reg.IsMatch(obj.RelationContent + ""))
                        {
                            return false;
                        }
                    }
                }
                if (oldRelationTypeID == 1 + "")
                {
                    if (!string.IsNullOrEmpty(AddContent))
                    {
                        if (oldRelationContent != AddContent)
                        {
                            //UpdateVenueIsSelectByIDList(oldRelationContent, false);
                        }
                    }
                    if (obj.RelationType != 1 || obj.RelationType != 2)
                    {
                        //UpdateVenueIsSelectByIDList(oldRelationContent, false);
                    }
                }
                if (oldRelationTypeID == 2 + "")
                {
                    if (!string.IsNullOrEmpty(AddContent))
                    {
                        if (oldRelationContent != AddContent)
                        {
                            //UpdateActiveAndSpecialByIDList(oldRelationContent, false);
                        }
                    }
                    if (obj.RelationType != 1 || obj.RelationType != 2)
                    {
                        //UpdateActiveAndSpecialByIDList(oldRelationContent, false);
                    }
                }
                if (!string.IsNullOrEmpty(AddContent))
                {
                    obj.RelationContent = AddContent;
                    if (obj.RelationType == 1)
                    {
                        if (!reg.IsMatch(AddContent + ""))
                        {
                            return false;
                        }
                        //UpdateVenueIsSelectByIDList(AddContent, true);
                    }
                    if (obj.RelationType == 2)
                    {
                        if (!reg.IsMatch(AddContent + ""))
                        {
                            return false;
                        }
                        //UpdateActiveAndSpecialByIDList(AddContent, true);
                    }
                }
                AddOperationLog(obj.MettingNO, (obj.MettingNO == obj.MainMeetingNO) ? 1 : 2, 4, LogActionType.Edit, "会场编号为:" + obj.MettingNO + "区块ID为:" + obj.RegionID + "--会场区块编辑");
                return DapperUtil.UpdatePartialColumns<SWfsMeetingRelationRegion>(new
                {
                    MeetingRelationID = obj.MeetingRelationID,
                    ImgNO = obj.ImgNO,
                    Direction = obj.Direction,
                    RelationType = obj.RelationType,
                    RelationContent = obj.RelationContent,
                    AboutID = obj.RelationType == 2 ? obj.RelationContent : null
                });
            }
        }

        public int AddMeetingHtml(SWfsMeetingInfoHtml html)
        {
            return DapperUtil.Insert<SWfsMeetingInfoHtml>(html);

        }
        /// <summary>
        /// 分页获取会场列表数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="nameAndDomain"></param>
        /// <param name="venueNO"></param>
        /// <param name="status"></param>
        /// <param name="webOrMobile"></param>
        /// <param name="sourceFrom"></param>
        /// <param name="activeNO"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public IList<SWfsMeetingInfo> GetVenueListByPage(string pageIndex, int pageSize, string nameAndDomain,
           string venueNO, string status, string webOrMobile, string sourceFrom, string activeNO, string startTime,
            string endTime, string parentID, out int totalCount)
        {
            Regex reg = new Regex(@"^\d+$");
            var dic = new Dictionary<string, object>();
            dic.Add("NameAndDomain", string.IsNullOrEmpty(nameAndDomain) ? "" : nameAndDomain);
            dic.Add("MeetingNO", string.IsNullOrEmpty(venueNO) ? "" : venueNO);
            dic.Add("Status", string.IsNullOrEmpty(status) ? "" : status);
            dic.Add("WebOrMobile", string.IsNullOrEmpty(webOrMobile) ? "" : webOrMobile);
            dic.Add("SourceFrom", string.IsNullOrEmpty(sourceFrom) ? "" : sourceFrom);
            dic.Add("ActiveNO", string.IsNullOrEmpty(activeNO) ? "" : activeNO);
            dic.Add("StartTime", string.IsNullOrEmpty(startTime) ? "" : startTime);
            dic.Add("EndTime", string.IsNullOrEmpty(endTime) ? "" : endTime);
            dic.Add("ParentID", string.IsNullOrEmpty(parentID) ? "" : parentID);

            totalCount = DapperUtil.Query<int>("ComBeziWfs_SWfsMeetingInfo_SelectMeetingTotalCount", dic, new
            {
                pageIndex = reg.IsMatch(pageIndex + "") ? int.Parse(pageIndex) : 1,
                pageSize = pageSize,
                NameAndDomain = nameAndDomain,
                MeetingNO = venueNO,
                Status = reg.IsMatch(status + "") ? int.Parse(status) : -1,
                WebOrMobile = reg.IsMatch(webOrMobile + "") ? int.Parse(webOrMobile) : -1,
                SourceFrom = reg.IsMatch(sourceFrom + "") ? int.Parse(sourceFrom) : -1,
                ActiveNO = activeNO,
                StartTime = string.IsNullOrEmpty(startTime) ? DateTime.Now : DateTime.Parse(startTime),
                EndTime = string.IsNullOrEmpty(endTime) ? DateTime.Now : DateTime.Parse(endTime),
                ParentID = reg.IsMatch(parentID + "") ? int.Parse(parentID) : -1
            }).FirstOrDefault();
            return DapperUtil.Query<SWfsMeetingInfo>("ComBeziWfs_SWfsMeetingInfo_SelectMeetingByPage", dic, new
            {
                pageIndex = reg.IsMatch(pageIndex + "") ? int.Parse(pageIndex) : 1,
                pageSize = pageSize,
                NameAndDomain = nameAndDomain,
                MeetingNO = venueNO,
                Status = reg.IsMatch(status + "") ? int.Parse(status) : -1,
                WebOrMobile = reg.IsMatch(webOrMobile + "") ? int.Parse(webOrMobile) : -1,
                SourceFrom = reg.IsMatch(sourceFrom + "") ? int.Parse(sourceFrom) : -1,
                ActiveNO = activeNO,
                StartTime = string.IsNullOrEmpty(startTime) ? DateTime.Now : DateTime.Parse(startTime),
                EndTime = string.IsNullOrEmpty(endTime) ? DateTime.Now : DateTime.Parse(endTime),
                ParentID = reg.IsMatch(parentID + "") ? int.Parse(parentID) : -1
            }).ToList();
        }
        /// <summary>
        /// 活动和专题列表查询
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="activeNOAndName">活动编号或者名称</param>
        /// <param name="webSource">0尚品、1奥莱</param>
        /// <param name="activeType">1活动、0专题</param>
        /// <param name="activeStatus">状态</param>
        /// <param name="activeStartDate">开始时间</param>
        /// <param name="activeEndDate">结束时间</param>
        /// <returns></returns>
        public IList<ActiveAndSpecial> GetActiveAndSpecialByPage(string meetingNO, string pageIndex, int pageSize, string activeNOAndName,
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
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsMeetingActiveSpecial_SelectSubjectAndTopicsCount", dic, new
            {
                MeetingNO = meetingNO,
                ActiveNOAndName = activeNOAndName,
                WebSource = reg.IsMatch(webSource + "") ? int.Parse(webSource) : -1,
                ActiveType = reg.IsMatch(activeType + "") ? int.Parse(activeType) : -1,//活动/专题
                Status = reg.IsMatch(activeStatus + "") ? int.Parse(activeStatus) : -1,
                StartTime = string.IsNullOrEmpty(activeStartDate) ? DateTime.Now : DateTime.Parse(activeStartDate),
                EndTime = string.IsNullOrEmpty(activeEndDate) ? DateTime.Now : DateTime.Parse(activeEndDate)
            }).FirstOrDefault();
            return DapperUtil.Query<ActiveAndSpecial>("ComBeziWfs_SWfsMeetingActiveSpecial_SelectSubjectAndTopicsByPage", dic, new
            {
                MeetingNO = meetingNO,
                pageIndex = reg.IsMatch(pageIndex + "") ? int.Parse(pageIndex) : 1,
                pageSize = pageSize,
                ActiveNOAndName = activeNOAndName,
                WebSource = reg.IsMatch(webSource + "") ? int.Parse(webSource) : -1,
                ActiveType = reg.IsMatch(activeType + "") ? int.Parse(activeType) : -1,//活动/专题
                Status = reg.IsMatch(activeStatus + "") ? int.Parse(activeStatus) : -1,
                StartTime = string.IsNullOrEmpty(activeStartDate) ? DateTime.Now : DateTime.Parse(activeStartDate),
                EndTime = string.IsNullOrEmpty(activeEndDate) ? DateTime.Now : DateTime.Parse(activeEndDate)
            }).ToList();
        }
        /// <summary>
        /// 选中会场后更新会场IsSelect为True
        /// </summary>
        /// <param name="idList">会场ID</param>
        /// <param name="IsSelect">设置是否选中</param>
        /// <returns></returns>
        public bool UpdateVenueIsSelectByIDList(string idList, bool IsSelect)
        {
            if (string.IsNullOrEmpty(idList))
            {
                return false;
            }
            Regex reg = new Regex(@"^\d+$");
            if (!reg.IsMatch(idList + ""))
            {
                return false;
            }
            return DapperUtil.Execute("ComBeziWfs_SWfsMeetingInfo_UpdateMeetingIsSelect", new { IsSelect = IsSelect, MeetingID = int.Parse(idList) }) > 0 ? true : false;
            //return DapperUtil.Execute("ComBeziWfs_SWfsMeetingInfo_UpdateMeetingIsSelect", new { IsSelect = IsSelect, MeetingID = idList.Split(',').ToArray() }) > 0 ? true : false;
        }
        /// <summary>
        /// 选中后更新活动IsSelect为True
        /// </summary>
        /// <param name="idList">活动ID</param>
        /// <param name="IsSelect">设置是否选中</param>
        /// <returns></returns>
        public bool UpdateActiveAndSpecialByIDList(string idList, bool IsSelect)
        {
            if (string.IsNullOrEmpty(idList))
            {
                return false;
            }
            Regex reg = new Regex(@"^\d+$");
            if (!reg.IsMatch(idList + ""))
            {
                return false;
            }
            return DapperUtil.Execute("ComBeziWfs_SWfsMeetingActiveSpecial_UpdateActiveAndSpecialIsSelect", new { IsSelect = IsSelect, ActiveID = int.Parse(idList) }) > 0 ? true : false;
            //return DapperUtil.Execute("ComBeziWfs_SWfsMeetingActiveSpecial_UpdateActiveAndSpecialIsSelect", new { IsSelect = IsSelect, ActiveID = idList.Split(',').ToArray() }) > 0 ? true : false;
        }
        //按条件删除会场区块关联数据
        public bool DeleteRelationRegionByCondition(string MeetingNO, string TemplateNO, int RegionID)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsMeetingRelationRegion_DeleteMeetingRelationByCondition", new
            {
                MeetingNO = MeetingNO,
                TemplateNO = TemplateNO,
                RegionID = RegionID
            }) > 0 ? true : false;
        }
        //按ID删除会场区块关联数据
        public bool DeleteRelationRegionByID(int MeetingRelationID)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsMeetingRelationRegion_DeleteMeetingRelationByID", new
            {
                MeetingRelationID = MeetingRelationID
            }) > 0 ? true : false;
        }
        //按会场编号模板编号查询区块列表
        public IEnumerable<SWfsMeetingRelationRegion> GetRelationRegionList(string MeetingNO, string TemplateNO)
        {
            return DapperUtil.Query<SWfsMeetingRelationRegion>("ComBeziWfs_SWfsMeetingRelationRegion_SelectMeetingRelationByTemplateNOAndMeetingNO", new { MeetingNO = MeetingNO, TemplateNO = TemplateNO });
        }
        //（按模板编号会场编号区块ID）查询区块信息
        public SWfsMeetingRelationRegion GetRelationobjByContent(string venueNO, int regionID, string templateNO)
        {
            return DapperUtil.Query<SWfsMeetingRelationRegion>("ComBeziWfs_SWfsMeetingRelationRegion_FetchEntityByCondition", new { MeettingNO = venueNO, RegionID = regionID, TemplateNO = templateNO }).FirstOrDefault();
        }
        #endregion

        #region 会场数据统计
        public IEnumerable<SwfsVenueVisitInfoExtends> GetVenueVisitList(string meetingNO, string type, DateTime startTime, DateTime endTime, string clickRegionID)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("timeregion", string.IsNullOrEmpty(type) ? "5" : type);
            IList<SwfsVenueVisitInfoExtends> list1 = null;
            IList<SwfsVenueVisitInfoExtends> list2 = null;
            list1 = DapperUtil.Query<SwfsVenueVisitInfoExtends>("ComBeziReport_SWfsMeetingVisitInfo_SelectVenueVisitCha", dic, new { StartTime = startTime, EndTime = endTime, MeetingNO = meetingNO }).ToList();
            list2 = DapperUtil.Query<SwfsVenueVisitInfoExtends>("ComBeziReport_SWfsMeetingVisitInfo_SelectVenueVisitRegionCha", dic, new { StartTime = startTime, EndTime = endTime, MeetingNO = meetingNO, ClickRegionID = clickRegionID }).ToList();
            if (list2 != null)
            {
                for (int i = 0; i < list2.Count; i++)
                {
                    list1.Add(list2[i]);
                }
            }
            return list1 != null ? list1 : new List<SwfsVenueVisitInfoExtends>();
        }
        #endregion

        #region 会场管理 by zhangwei 20130911
        /// <summary>
        /// 添加主会场和分会场 by zhangwei 20130909
        /// </summary>
        /// <param name="model">会场数据集合</param>
        /// <returns></returns>
        public bool AddMeeting(SWfsMeetingInfo model)
        {
            if (DapperUtil.Insert<SWfsMeetingInfo>(model, true) > 0)
            {
                return true;
            }
            return false;
        }//AddMeeting
        /// <summary>
        /// 会场管理修改 by mengyinglong 2014/7/8
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddMateMeeting(SWfsMeetingMetaInfo model)
        {


            if (DapperUtil.Insert<SWfsMeetingMetaInfo>(model, true) > 0)
            {
                return true;
            }
            return false;
        }
        //按编号查询是否存在
        public SWfsMeetingMetaInfo GetInfoByMeetingID(string MeetingID)
        {
            return DapperUtil.Query<SWfsMeetingMetaInfo>("ComBeziWfs_SWfsMeetingMetaInfo_MetaInfoByMeetingID", new { MeetingID = MeetingID }).FirstOrDefault();
        }
        /// <summary>
        /// 修改主会场和分会场 by zhangwei 20130909
        /// </summary>
        /// <param name="model">会场数据集合</param>
        /// <returns></returns>
        public bool EditMeeting(SWfsMeetingInfo model)
        {
            return DapperUtil.UpdatePartialColumns<SWfsMeetingInfo>(new
            {
                MeetingID = model.MeetingID,
                MeetingName = model.MeetingName,
                WebOrMobile = model.WebOrMobile,
                Status = model.Status,
                SourceFrom = model.SourceFrom,
                PreViewTime = model.PreViewTime,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                MobileShowImg = model.MobileShowImg,
                WebPreViewNO = model.WebPreViewNO,
                WebStartNO = model.WebStartNO,
                MobilePreViewNO = model.MobilePreViewNO,
                MobileStartNO = model.MobileStartNO,
                MeetingDomain = model.MeetingDomain,
                WebPreViewCode = model.WebPreViewCode,
                WebStartCode = model.WebStartCode,
                MobilePreViewCode = model.MobilePreViewCode,
                MobileStartCode = model.MobileStartCode
            });
        }//EditMeeting
        /// <summary>
        /// 修改主会场和分会场 by mengyl 20140708
        /// </summary>
        /// <param name="model">会场数据集合</param>
        /// <returns></returns>
        public bool EditMateMeeting(SWfsMeetingMetaInfo model)
        {
            return DapperUtil.UpdatePartialColumns<SWfsMeetingMetaInfo>(new
            {
                ID = model.ID,
                MeetingID = model.MeetingID,
                Title = model.Title,
                KeyWords = model.KeyWords,
                Description = model.Description
            });
        }//EditMeeting
        /// <summary>
        /// 获取会场列表 by zhangwei 20130910
        /// </summary>
        /// <param name="meetId">会场ID</param>
        /// <param name="isSelect">是否选为分会场 0否，1是</param>
        /// <param name="meetNameOrDomain">会场名称或域名</param>
        /// <param name="activityNo">活动编号</param>
        /// <param name="status">会场状态</param>
        /// <param name="webOrMobile">展示渠道</param>
        /// <param name="meetStartDateTime">会场开始日期时间</param>
        /// <param name="meetEndDateTime">会场结束日期时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="readCount">输出总记录</param>
        /// <returns></returns>
        public IList<SWfsMeetingHtmlInfoList> GetMeetingDataList(string meetId, bool isSelect, string meetNameOrDomain, string topicSubjectType, string activityNo, string status, string webOrMobile, string SourceFrom, string meetStartDateTime, string meetEndDateTime, int pageIndex, int pageSize, out int readCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("meetId", string.IsNullOrEmpty(meetId) ? "" : meetId);
            dic.Add("isSelect", isSelect ? 1 : 0); //0主会场 1分会场
            dic.Add("meetNameOrDomain", string.IsNullOrEmpty(meetNameOrDomain) ? "" : meetNameOrDomain);
            dic.Add("topicSubjectType", string.IsNullOrEmpty(topicSubjectType) ? "" : topicSubjectType);
            dic.Add("activityNo", string.IsNullOrEmpty(activityNo) ? "" : activityNo);
            dic.Add("status", string.IsNullOrEmpty(status) ? "" : status);
            dic.Add("webOrMobile", string.IsNullOrEmpty(webOrMobile) ? "" : webOrMobile);
            dic.Add("SourceFrom", string.IsNullOrEmpty(SourceFrom) ? "" : SourceFrom);
            dic.Add("meetStartDateTime", string.IsNullOrEmpty(meetStartDateTime) ? "" : meetStartDateTime);
            dic.Add("meetEndDateTime", string.IsNullOrEmpty(meetEndDateTime) ? "" : meetEndDateTime);
            dic.Add("TopNum", 0); //0查询所有记录，1统计总记录数
            object obj = new { MeetNameOrDomain = meetNameOrDomain, ActivityNo = activityNo, Status = status, WebOrMobile = webOrMobile, SourceFrom = SourceFrom, StartTime = meetStartDateTime, EndTime = meetEndDateTime, MeetingID = meetId, IsSelect = isSelect, ActiveNO = activityNo, IsActive = topicSubjectType };
            IList<SWfsMeetingHtmlInfoList> list = DapperUtil.QueryPaging<SWfsMeetingHtmlInfoList>("ComBeziWfs_SWfsMeetingInfo_MeetingDataList", pageIndex, pageSize, "CreateTime DESC", dic, obj).ToList();
            readCount = GetRecordCount(dic, obj, "ComBeziWfs_SWfsMeetingInfo_MeetingDataList"); //获取总记录数
            return list;
        }//GetMeetingDataList


        public IList<SWfsMeetingInfoHtml> GetMeetingInfoHtmlList()
        {
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic.Add("meetId", string.IsNullOrEmpty(meetId) ? "" : meetId);
            //object obj = new { MettingId = meetId };
            //DapperUtil.Query<WfsBrand>("ComBeziWfs_WfsBrand_ListParent").ToList();
            return DapperUtil.Query<SWfsMeetingInfoHtml>("ComBeziWfs_SWfsMeetingInfoHtml_MeetingDataList").ToList();
        }
        /// <summary>
        /// 获取总记录数 by zhangwei 20130909
        /// <param name="dic">参数</param>
        /// <param name="obj">字段</param>
        /// <param name="statement">调用sql</param>
        /// </summary>
        /// <returns></returns>
        public int GetRecordCount(Dictionary<string, object> dic, object obj, string statement)
        {
            dic["TopNum"] = "1"; //0查询记录，1统计记录
            var list = DapperUtil.Query<int>(statement, dic, obj);
            if (list.Count()>0)
            {
                return list.First();
            }
            return 0;
        }//GetRecordCount

        /// <summary>
        /// 更新会场状态 by zhangwei 20130910
        /// </summary>
        ///<param name="MeetingID">会场ID</param>
        ///<param name="status">状态</param>
        public bool UpdateMeetingStatus(string MeetingID, int status)
        {
            return DapperUtil.UpdatePartialColumns<SWfsMeetingInfo>(new { MeetingID = MeetingID, Status = status });
        }//UpdateMeetingStatus

        /// <summary>
        /// 获取会场专题活动列表 by zhangwei 20130914
        /// </summary>
        /// <param name="mainMeetNo">主会场编号</param>
        /// <param name="meetNo">分会场编号</param>
        /// <param name="meetingType">会场类型 0主会场，1分会场</param>
        /// <param name="subjectTopicName">专题活动名称</param>
        /// <param name="activityTopicNo">专题活动编号</param>
        /// <param name="status">专题活动状态 0关闭，1开启</param>
        /// <param name="webSiteType">所属网站 0尚品，1奥莱</param>
        /// <param name="channelNo">渠道 zsqd001网站，zsqd002移动端</param>
        /// <param name="topicSubjectType">类型 0专题，1活动 不能为空</param>
        /// <param name="startTime">专题活动开始日期</param>
        /// <param name="endTime">专题活动结束日期</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="readCount">输出总记录数</param>
        /// <returns></returns>
        public IList<SWfsMeetingSubjectTopicM> GetMeetingSubjectTopicDataList(string mainMeetNo, string meetNo, int meetingType, string subjectTopicName, string activityTopicNo, string status, string webSiteType, string channelNo, string topicSubjectType, string startTime, string endTime, int pageIndex, int pageSize, out int readCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("TopicSubjectType", string.IsNullOrEmpty(topicSubjectType) ? "1" : topicSubjectType); //如果为空默认显示活动数据
            dic.Add("WebSiteType", webSiteType); //所属网站
            dic.Add("MeetingType", meetingType); //会场类型
            dic.Add("SubjectTopicName", string.IsNullOrEmpty(subjectTopicName) ? "" : subjectTopicName);
            dic.Add("ActivityTopicNo", string.IsNullOrEmpty(activityTopicNo) ? "" : activityTopicNo);
            dic.Add("Status", string.IsNullOrEmpty(status) ? "" : status);
            dic.Add("StartDateTime", string.IsNullOrEmpty(startTime) ? "" : startTime);
            dic.Add("EndDateTime", string.IsNullOrEmpty(endTime) ? "" : endTime);
            dic.Add("ChannelNo", string.IsNullOrEmpty(channelNo) ? "" : channelNo);
            dic.Add("TopNum", 0); //0查询所有记录，1统计总记录数

            object obj = new { MainMeetingNO = mainMeetNo, MeetingNo = meetNo, ActiveName = subjectTopicName, ActiveNO = activityTopicNo, Status = status, StartTime = startTime, EndTime = endTime, IsActive = topicSubjectType, ChannelNo = channelNo };
            IList<SWfsMeetingSubjectTopicM> list = null;
            readCount = 0;
            try
            {
                var tempList = DapperUtil.QueryPaging<SWfsMeetingSubjectTopicM>("ComBeziWfs_SWfsMeetingActiveSpecial_MeetingSubjectTopicList", pageIndex, pageSize, "CreateTime DESC", dic, obj);
                if (tempList != null && tempList.Count() > 0)
                {
                    list = tempList.ToList();
                    readCount = GetRecordCount(dic, obj, "ComBeziWfs_SWfsMeetingActiveSpecial_MeetingSubjectTopicList"); //获取总记录数
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }//GetMeetingSubjectTopicDataList

        /// <summary>
        /// 获取所有品牌列表
        /// </summary>
        /// <returns></returns>
        public IList<Brand> SelectBrandList()
        {
            IList<Brand> brandlist = DapperUtil.Query<Brand>("ComBeziWfs_WfsBrand_ListParent").ToList();
            return brandlist;
        }//SelectBrandList

        /// <summary>
        /// 添加会场专题活动关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddMeetSubjectTopicRelation(SWfsMeetingActiveSpecial model)
        {
            return DapperUtil.Insert(model, true);
        }//AddMeetSubjectTopicRelation

        /// <summary>
        /// 删除会场和专题活动关系
        /// </summary>
        /// <param name="activeId">专题活动关系ID</param>
        /// <returns></returns>
        public int DelMeetSubjectTopicRelation(string activeId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsMeetingActiveSpecial_DelMeetingActiveRelation", new { ActiveID = activeId });
        }//DelMeetSubjectTopicRelation

        /// <summary>
        /// 获取会场下专题活动列表
        /// </summary>
        /// <param name="mainMeetingNo">主会场编号</param>
        /// <param name="meetingNo">分会场编号</param>
        /// <param name="isActive">类型 0专题，1活动</param>
        /// <returns></returns>
        public IList<SWfsMeetingActiveSpecial> GetMeetingActiveSpecialList(string mainMeetingNo, int meetingNo = 0, string isActive = "")
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("meetingNO", meetingNo); //分会场编号
            dic.Add("isActive", isActive); //0专题，1活动
            return DapperUtil.Query<SWfsMeetingActiveSpecial>("ComBeziWfs_SWfsMeetingActiveSpecial_GetMeetingActiveSpecialList", dic, new { MainMeetingNO = mainMeetingNo, MeetingNO = meetingNo, IsActive = isActive }).ToList();
        }//GetMeetingActiveSpecialList

        /// <summary>
        /// 修改会场专题活动关系
        /// </summary>
        /// <param name="model">会场专题活动关系数据集</param>
        /// <returns></returns>
        public bool EditMeetingSubjectTopicRelation(SWfsMeetingActiveSpecial model)
        {
            return DapperUtil.UpdatePartialColumns<SWfsMeetingActiveSpecial>(new
            {
                ActiveID = model.ActiveID,
                ActiveLink = model.ActiveLink,
                BrandLink = model.BrandLink,
                BrandNO = model.BrandNO
            });
        }//EditMeetingSubjectTopicRelation

        /// <summary>
        /// 判断会场名称是否存在
        /// </summary>
        /// <param name="meetId">当前会场ID</param>
        /// <param name="meetDomain">会场域名</param>
        /// <returns></returns>
        public bool CheckMeetingDomainIsExist(int meetId, string meetDomain)
        {
            var code = DapperUtil.Query<int>("ComBeziWfs_SWfsMeetingInfo_CheckMeetingDomainIsExist", new { MeetingID = meetId, MeetingDomain = meetDomain }).FirstOrDefault();
            return code != null && code > 0 ? true : false;
        }//CheckMeetingDomainIsExist

        /// <summary>
        /// 判断会场编号是否重复
        /// </summary>
        /// <param name="meetNo">会场编号</param>
        /// <returns></returns>
        public bool CheckMeetingNoIsExist(string meetNo)
        {
            var code = DapperUtil.Query<int>("ComBeziWfs_SWfsMeetingInfo_CheckMeetingNoIsExist", new { MeetingNO = meetNo }).FirstOrDefault();
            return code != null && code > 0 ? true : false;
        }//CheckMeetingNoIsExist

        public string GetTemplateName(string MettingNo)
        {
            return DapperUtil.Query<string>("ComBeziWfs_SWfsMeetingTemplateInfo_TemplateByMeetingNO", new { MettingNo = MettingNo }).FirstOrDefault();
        }
        #endregion

        #region 操作日志管理 by zhangwei 20130922
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="meetNo">会场编号</param>
        /// <param name="meetType">会场类型 1主会场，2分会场</param>
        /// <param name="diaryType">日志类型 1会场，2专题，3活动，4区块，5模板</param>
        /// <param name="enumObj">动作类型（添加，修改，删除）</param>
        /// <param name="text">日志内容</param>
        /// <returns></returns>
        public bool AddOperationLog(string meetNo, int meetType, int diaryType, LogActionType enumObj, string text)
        {
            SWfsMeetingOperationDiary model = new SWfsMeetingOperationDiary();
            string userName = ""; //当前登录用户名
            Passport passport = PresentationHelper.GetPassport();
            if (passport!=null)
            {
                userName=passport.UserName;
            }
            model.MeetingNO = meetNo; //会场编号
            model.MeetingType = (short)meetType; //会场类型 1主会场，2分会场
            model.DiaryType = (short)diaryType; //日志类型 1会场，2专题，3活动，4区块，5模板
            model.ActionType = (short)enumObj.GetHashCode(); //动作类型
            model.OpratePeople = userName; //操作人
            model.OprateInfo = text; //内容
            model.IsDelete = false; //是否放回收站
            model.CreateTime = DateTime.Now; //创建日期时间
            if (DapperUtil.Insert<SWfsMeetingOperationDiary>(model, true) > 0)
            {
                return true;
            }
            return false;
        }//AddOperationLog

        /// <summary>
        /// 获取会场日志列表
        /// </summary>
        /// <param name="meetNo">会场ID</param>
        /// <param name="keyword">关键词</param>
        /// <param name="actionType">日志类型</param>
        /// <param name="startDateTime">开始日期时间</param>
        /// <param name="endDateTime">结束日期时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="readCount">输出总记录</param>
        /// <returns></returns>
        public IList<SWfsMeetingOperationDiary> GetMeetingOperationLogDataList(string meetNo, string keyword, int actionType, string startDateTime, string endDateTime, int pageIndex, int pageSize, out int readCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("MeetingNo", string.IsNullOrEmpty(meetNo) ? "" : meetNo);
            dic.Add("ActionType", actionType);
            dic.Add("KeyWord", string.IsNullOrEmpty(keyword) ? "" : keyword);
            dic.Add("StartDateTime", string.IsNullOrEmpty(startDateTime) ? "" : startDateTime);
            dic.Add("EndDateTime", string.IsNullOrEmpty(endDateTime) ? "" : endDateTime);
            dic.Add("TopNum", 0); //0查询所有记录，1统计总记录数
            object obj = new { KeyWord = keyword, ActionType = actionType, StartTime = startDateTime, EndTime = endDateTime, MeetingNo = meetNo };
            IList<SWfsMeetingOperationDiary> list = DapperUtil.QueryPaging<SWfsMeetingOperationDiary>("ComBeziWfs_SWfsMeetingOperationDiary_GetMeetingLogDataList", pageIndex, pageSize, "CreateTime DESC", dic, obj).ToList();
            readCount = GetRecordCount(dic, obj, "ComBeziWfs_SWfsMeetingOperationDiary_GetMeetingLogDataList"); //获取总记录数
            return list;
        }//GetMeetingOperationLogDataList
        #endregion

        #region 尚品活动 by zhangwei 20130929
        /// <summary>
        /// 获取尚品活动列表
        /// </summary>
        /// <param name="keyword">关键词 活动名称或编号</param>
        /// <param name="productKeyWord">商品编号</param>
        /// <param name="status">状态 0关闭，1开启</param>
        /// <param name="channelSord">分类</param>
        /// <param name="channelNo">展示渠道</param>
        /// <param name="startTime">开始日期时间</param>
        /// <param name="endTime">结束日期时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="recordCount">输出总记录数</param>
        /// <returns></returns>
        public IList<SWfsNewSubject> GetSelectNewSubjectList(string keyword, string productKeyWord, string status, string channelSord, string channelNo, string startTime, string endTime, int pageIndex, int pageSize, out int recordCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动名称/活动编号") ? "" : keyword);
            dic.Add("ProductNo", (productKeyWord == null || productKeyWord == "商品编号") ? "" : productKeyWord);
            dic.Add("Status", status == null ? "" : status);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("ChannelNo", channelNo == null ? "" : channelNo);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            dic.Add("TopNum", 0); //0查询所有记录，1统计总记录数
            object obj = new { KeyWord = keyword, ProductNo = productKeyWord, Status = status, ChannelNo = channelNo, StartTime = startTime, EndTime = endTime };
            IList<SWfsNewSubject> list = DapperUtil.QueryPaging<SWfsNewSubject>("ComBeziWfs_SWfsNewSubject_GetNewSubjectList", pageIndex, pageSize, "DateCreate desc,DateBegin desc", dic, obj).ToList();
            recordCount = GetRecordCount(dic, obj, "ComBeziWfs_SWfsNewSubject_GetNewSubjectList"); //获取总记录数
            return list;
        }//GetSelectNewSubjectList
        #endregion

        #region 秒杀

        public IEnumerable<SwfsSkillGroup> GetSwfsSkillGroupList(int meetingId, int pageIndex, int pageSize, out int count)
        {
            count = DapperUtil.Query<int>("ComBeziWfs_SwfsSkillGroup_GetSwfsSkillGroupListCount", new { MeetingID = meetingId }).First<int>();
            return DapperUtil.QueryPaging<SwfsSkillGroup>("ComBeziWfs_SwfsSkillGroup_GetSwfsSkillGroupList", pageIndex, pageSize, "StartTime  DESC", new { MeetingID = meetingId });
        }

        public SwfsSkillGroup GetSwfsSkillGroupById(int groupId)
        {
            return DapperUtil.QueryByIdentity<SwfsSkillGroup>(groupId);
        }

        public int SaveSkillGroup(SwfsSkillGroup model)
        {
            try
            {
                if (model.Id > 0)
                {
                    return DapperUtil.Update<SwfsSkillGroup>(model) ? 1 : 0;
                }
                else
                    return DapperUtil.Insert<SwfsSkillGroup>(model, true);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public int DeleteSkillGroup(int groupId)
        {
            try
            {
                var flag = DapperUtil.Execute("ComBeziWfs_SwfsSkillGroup_DeleteSwfsSkillGroupByGroupId", new { GroupId = groupId });
                if (flag > 0)
                    DeleteSkillGroupProduct(groupId);
                return flag;

            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        private int DeleteSkillGroupProduct(int groupId)
        {
            try
            {
                return DapperUtil.Execute("ComBeziWfs_SwfsSkillGroup_DeleteSwfsSkillGroupProductByGroupId", new { GroupId = groupId });

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion

        #region 秒杀产品管理
        //查询商品列表
        public IList<ProductInfo> GetProductList(string gender, string categoryNo, string keyword, int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("PNOList", "");
            dic.Add("Gender", gender == null ? "" : gender);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("pageIndex", pageIndex);
            dic.Add("pageSize", pageSize);
            IList<ProductInfo> productList = DapperUtil.Query<ProductInfo>("ComBeziWfs_WfsProduct_SelectSkillProductList", dic, new { KeyWord = keyword, pageIndex = pageIndex, pageSize = pageSize, Gender = gender, CategoryNo = categoryNo }).ToList();
            count = DapperUtil.Query<int>("ComBeziWfs_WfsProduct_SelectSkillProductListTotalCount", dic, new { KeyWord = keyword, pageIndex = pageIndex, pageSize = pageSize, Gender = gender, CategoryNo = categoryNo }).First();
            return productList;
        }
        //查询添加的秒杀产品列表
        public IEnumerable<SkillProductExtends> GetSwfsSkillProductList(int skillGroupID)
        {
            return DapperUtil.Query<SkillProductExtends>("ComBeziWfs_SWfsSkillProduct_GetSkillProductList", new { SkillGroupId = skillGroupID });
        }
        //上传图片
        public int AddProductImg(int id, string imgNO)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSkillProduct>(new { Id = id, ProductFileNo = imgNO }) ? 1 : 0;
        }
        //删除秒杀产品
        public int DeleteSkillProduct(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSkillProduct_DeleteSkillProductByID", new { Id = id });
        }
        //批量添加秒杀商品
        public int AddSkillProduct(int groupid, string productNO)
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
            Passport passport = PresentationHelper.GetPassport();
            for (int i = 0; i < newidlist.Length; i++)
            {
                DapperUtil.Insert<SWfsSkillProduct>(new SWfsSkillProduct()
                {
                    SkillGroupId = groupid,
                    ProductNo = newidlist[i],
                    ProductFileNo = "",
                    CreateTime = DateTime.Now,
                    CreateUser = passport.UserName,
                    ModifyTime = DateTime.Now,
                    ModifyUser = passport.UserName,
                    IsVoid = 0
                }, true);
            }
            return 1;
        }
        //保存排序
        public int SaveSortSkillProduct(int id, int sortValue)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSkillProduct>(new { Id = id, SortValue = sortValue }) ? 1 : 0;
        }
        //批量删除
        public int DeleteSkillProductByIdList(string idList)
        {
            if (string.IsNullOrEmpty(idList))
            {
                return 0;
            }
            return DapperUtil.Execute("ComBeziWfs_SWfsSkillProduct_DeleteSkillProductByIDList", new { idList = idList.Split(',').ToArray() });
        }
        #endregion

        #region 会场导航
        //按id查询导航
        public SWfsMeetingNavigation GetNavObj(int id)
        {
            return DapperUtil.QueryByIdentity<SWfsMeetingNavigation>(id);
        }
        //添加编辑导航
        public int AddMeetingNav(SWfsMeetingNavigation obj)
        {
            if (obj.NavNO == 0)
            {
                return DapperUtil.Insert<SWfsMeetingNavigation>(obj, true);
            }
            else
            {
                return DapperUtil.Update<SWfsMeetingNavigation>(obj) ? 1 : 0;
            }
        }
        //查询会场导航
        public IEnumerable<SWfsMeetingNavigation> GetMeetingNavList(string navName, string navStatus, string navLink, int navType, int parentNO, int meetingID, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("NavName", string.IsNullOrEmpty(navName) ? "" : navName);
            dic.Add("NavStatus", string.IsNullOrEmpty(navStatus) ? "" : navStatus);
            dic.Add("NavLink", string.IsNullOrEmpty(navLink) ? "" : navLink);
            dic.Add("NavType", navType == -1 ? "" : navType + "");
            dic.Add("ParentNO", parentNO == -1 ? "" : parentNO + "");
            dic.Add("MeetingID", meetingID == 0 ? "" : meetingID + "");
            total = DapperUtil.Query<int>("ComBeziWfs_WfsProduct_SelectSWfsMeetingNavigationTotalCount", dic, new
            {
                NavName = navName,
                NavStatus = navStatus + "" == "1",
                NavLink = navLink,
                NavType = navType,
                ParentNO = parentNO,
                MeetingID = meetingID,
                pageIndex = pageIndex,
                pageSize = pageSize
            }).First();

            return DapperUtil.Query<SWfsMeetingNavigation>("ComBeziWfs_WfsProduct_SelectSWfsMeetingNavigationList", dic, new
            {
                NavName = navName,
                NavStatus = navStatus + "" == "1",
                NavLink = navLink,
                NavType = navType,
                ParentNO = parentNO,
                MeetingID = meetingID,
                pageIndex = pageIndex,
                pageSize = pageSize
            });

        }
        //修改导航状态
        public int UpdateStatus(int id, int status)
        {
            return DapperUtil.UpdatePartialColumns<SWfsMeetingNavigation>(new { NavNO = id, NavStatus = status }) ? 1 : 0;
        }
        //修改排序
        public int UpdateOrder(int id, int order)
        {
            return DapperUtil.UpdatePartialColumns<SWfsMeetingNavigation>(new { NavNO = id, NavOrder = order }) ? 1 : 0;
        }
        //删除一级导航
        public int DeleteFirstNav(int id)
        {
            IEnumerable<SWfsMeetingNavigation> list = DapperUtil.Query<SWfsMeetingNavigation>("ComBeziWfs_SWfsMeetingNavigation_GetAllNav", new { NavNO = id });
            if (list != null)
            {
                if (list.Count() > 0)
                {
                    int[] idlist = list.Select(p => p.NavNO).ToArray();
                    return DapperUtil.Execute("ComBeziWfs_SWfsMeetingNavigation_DeleteAllNav", new { idlist = idlist });
                }
            }
            return 0;
        }
        //删除二级导航
        public int DeleteSecondNav(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsMeetingNavigation_DeleteSecondNav", new { NavNO = id });
        }
        //删除三级导航
        public int DeleteThirdNav(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsMeetingNavigation_DeleteThirdNav", new { NavNO = id });
        }
        //查询会场导航是否存在
        public int IsExistsMeetingNavPublist(int meetingid)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsMeetingNavigationPublish_IsExistNavPublist", new { MeetingID = meetingid }).First();
        }
        //发布会场
        public int PublistMeetingNav(int meetingid, string htmlcode)
        {
            int pubid = IsExistsMeetingNavPublist(meetingid);
            if (pubid > 0)
            {
                return DapperUtil.Insert<SWfsMeetingNavigationPublish>(new SWfsMeetingNavigationPublish() { MeetingID = meetingid, NavHtml = htmlcode, DateCreate = DateTime.Now }, true);
            }
            else
            {
                return DapperUtil.UpdatePartialColumns<SWfsMeetingNavigationPublish>(new { NavPublishID = pubid, NavHtml = htmlcode }) ? 1 : 0;
            }
        }
        #endregion
    }
}
