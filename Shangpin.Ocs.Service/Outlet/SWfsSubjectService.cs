using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Service.Common;
using System.Text.RegularExpressions;
using System.Collections;
using Shangpin.Framework.Common.Cache;
using System.Threading.Tasks;
using Shangpin.Framework.Common.Dapper;

namespace Shangpin.Ocs.Service.Outlet
{
    public class SWfsSubjectService
    {
        #region 获取频道
        /// <summary>
        /// 获取奥莱所有频道 by zhangwei 20140617
        /// </summary>
        /// <returns></returns>
        public List<SWfsChannel> FindSubjectChannelList()
        {
            IEnumerable<SWfsChannel> query = DapperUtil.Query<SWfsChannel>("ComBeziWfs_SWfsChannel_FindChannelList", null, null);
            if (query != null && query.Count() > 0)
            {
                return query.ToList();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 获取商品列表 by zhangwei 20140311
        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="productNo">商品编号</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        public RecordPage<ProductInfo> GetSWfsSubjectProductNew(string subjectNo, IList<string> productNo, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ProductNo", productNo);
            IList<ProductInfo> productList = DapperUtil.QueryPaging<ProductInfo>("ComBeziWfs_WfsProduct_SelectProductListBatchNew", pageIndex, pageSize, "ProductNo desc", dic, new { SubjectNo = subjectNo, ProductNo = productNo }).ToList();
            return PageConvertor.Convert(pageIndex, pageSize, productList);
        }
        #endregion

        #region 活动列表
        /// <summary>
        /// 活动列表
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="productKeyWord">商品关键词</param>
        /// <param name="status">活动状态</param>
        /// <param name="channelSord">分类</param>
        /// <param name="channelNo"渠道></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex"当前页></param>
        /// <param name="pageSize">页码</param>
        /// <param name="subjectTemplate">模板类型</param>
        /// <param name="isaudited">发布状态</param>
        /// <param name="bu">BU部门</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> SelectSubjectList(string keyword, string productKeyWord, string status, string channelSord, string channelNo, string startTime, string endTime, int pageIndex, int pageSize, string subjectTemplate = "", string isaudited = "", string bu = "")
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动名称/活动编号") ? "" : keyword);
            dic.Add("ProductNo", (productKeyWord == null || productKeyWord == "商品编号") ? "" : productKeyWord);
            dic.Add("Status", status == null ? "" : status);
            dic.Add("ChannelSord", channelSord == null ? "" : channelSord);
            dic.Add("ChannelNo", channelNo == null ? "" : channelNo);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            dic.Add("SubjectTemplate", subjectTemplate);
            dic.Add("BU", bu);
            dic.Add("IsAudited", isaudited);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectList", pageIndex, pageSize, "SWfsSubject.DateCreate desc,SWfsSubject.DateBegin desc", dic, new { KeyWord = keyword, ProductNo = productKeyWord, Status = status, ChannelSord = channelSord, ChannelNo = channelNo, StartTime = startTime, EndTime = endTime, SubjectTemplate = subjectTemplate, BU = bu, IsAudited = isaudited });
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef = GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());

            foreach (var subject in query)
            {
                subject.ChannelNo = getChannel(subject.ChannelNo);
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
                subject.TotalHour = getTimeDiffer(subject.DateEnd, subject.DateBegin);
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }
        /// <summary>
        /// 计算时间间隔
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        private static double getTimeDiffer(DateTime dt1, DateTime dt2)
        {
            return dt2.Subtract(dt1).Duration().TotalHours;
        }
        /// <summary>
        /// 改变展示渠道序号为名字
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        private static string getChannel(string Msg)
        {
            return Msg.Replace("zsqd001", "网站").Replace("zsqd002", "移动端").Replace("zsqd003", "其他渠道");
        }
        /// <summary>
        /// 根据活动编号获得所属分类
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public Dictionary<string, List<SWfsSubjectChannelSordRef>> GetSordBySubjectNoList(Array subjectNoLsit)
        {
            Dictionary<string, List<SWfsSubjectChannelSordRef>> dic = new Dictionary<string, List<SWfsSubjectChannelSordRef>>();
            List<SWfsSubjectChannelSordRef> lists = DapperUtil.Query<SWfsSubjectChannelSordRef>("ComBeziWfs_SWfsSubjectChannelSordRef_SelectBySubjectNoList", new { SubjectNoList = subjectNoLsit }).ToList();
            foreach (var item in lists)
            {
                if (dic.Keys.Contains(item.SubjectNo))
                {
                    dic[item.SubjectNo].Add(item);
                }
                else
                {
                    List<SWfsSubjectChannelSordRef> newlsit = new List<SWfsSubjectChannelSordRef>();
                    newlsit.Add(item);
                    dic.Add(item.SubjectNo, newlsit);
                }
            }
            return dic;

        }
        /// <summary>
        /// 根据活动编号查询所属频道
        /// </summary>
        /// <param name="subjectNoLsit"></param>
        /// <returns></returns>
        public Dictionary<string, List<SWfsChannel>> GetChannelBySubjectList(Array subjectNoLsit)
        {

            var dic = new Dictionary<string, List<SWfsChannel>>();
            List<SWfsChannel> data = DapperUtil.Query<SWfsChannel>("ComBeziWfs_SWfsChannel_ReadBySubjectNoList", new { SubjectNoList = subjectNoLsit }).ToList();
            foreach (SWfsChannel item in data)
            {
                string[] msg = item.ChannelNo.Split('|');
                string subjectNo = msg[0];
                item.ChannelNo = msg[1];
                if (dic.Keys.Contains(msg[0]))
                {
                    dic[subjectNo].Add(item);
                }
                else
                {
                    List<SWfsChannel> newitem = new List<SWfsChannel>();
                    newitem.Add(item);
                    dic.Add(subjectNo, newitem);
                }
            }
            return dic;
        }
        #endregion

        #region 查询活动中所有商品 用于导出EXCEL
        /// <summary>
        /// 查询活动中所有商品
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<ProductInfo> GetProductList(string subjectNo)
        {
            // modify by lijibo    原来的方法id除去掉 New 即可
            IList<ProductInfo> productList = DapperUtil.Query<ProductInfo>("ComBeziWfs_WfsProduct_SelectProductListToExcelNew", new { SubjectNo = subjectNo }).ToList();
            return productList;
        }
        #endregion

        #region 根据活动ID查询订单信息 导出EXCEl
        /// <summary>
        /// 根据活动ID查询订单信息 导出EXCEl
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<ORderToExcel> GetOrderBySubject(string subjectNo)
        {
            return DapperUtil.Query<ORderToExcel>("ComBeziWfs_WfsOrder_GetOrderBySubject", new { SubjectNo = subjectNo }).ToList();
        }
        #endregion

        #region 活动预热
        /// <summary>
        /// 预热活动相关信息添加 by zhangwei 20140321
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertSubjectTopExpand(SWfsSubjectTopExpand model)
        {
            return DapperUtil.Insert(model, false);
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public bool UpdateSubjectPreheat(SWfsSubjectTopExpand model)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSubjectTopExpand>(new
            {
                SubjectNo = model.SubjectNo,
                StExpand = model.StExpand,
                PreheatTime = model.PreheatTime
            });
        }

        /// <summary>
        /// 根据活动获取预热相关信息 by zhangwei 20140321
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        public SWfsSubjectTopExpand ReadSubjectTopExpand(string subjectNo)
        {
            SWfsSubjectTopExpand model = DapperUtil.QueryByIdentityWithNoLock<SWfsSubjectTopExpand>(subjectNo);
            return model;
        }

        /// <summary>
        /// 根据活动获取活动预热及预热页面相关信息 by zhangwei 20140321
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns></returns>
        public SubjectPreheatInfoM GetSubjectPreheatInfo(string subjectNo)
        {
            SubjectPreheatInfoM model = DapperUtil.Query<SubjectPreheatInfoM>("ComBeziWfs_SWfsSubjectTopExpand_GetSubjectPreheatAndBody", new { SubjectNo = subjectNo }).FirstOrDefault();
            return model;
        }
        #endregion

        #region 收起
        public IList<SWfsChannelSord> SelectSWfsSubjectChannelRef(string subjectNo)
        {
            IList<SWfsChannelSord> channelList = DapperUtil.Query<SWfsChannelSord>("ComBeziWfs_SWfsSubjectChannelSordRef_ReadBySubjectNo", new { SubjectNo = subjectNo }).ToList();
            return channelList;
        }

        public IList<SWfsChannelSord> SelectSWfsChannelSordList()
        {
            IList<SWfsChannelSord> channelSordList = DapperUtil.Query<SWfsChannelSord>("ComBeziWfs_SWfsChannelSord_SelectList").ToList();
            return channelSordList;
        }

        public void SubjectStatusModify(string subjectNo, string status)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsSubject_UpdateStatus", new { SubjectNo = subjectNo, Status = status });
        }

        public RecordPage<ProductInfo> GetSWfsSubjectProduct(string categoryNo, IList<string> categoryList, string subjectNo, string brandNo, string isShelf, string keyword, int pageIndex, int pageSize, string genderStyle)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("GenderStyle", genderStyle == null ? "" : genderStyle);
            IList<ProductInfo> productList = DapperUtil.QueryPaging<ProductInfo>("ComBeziWfs_WfsProduct_SelectProductList", pageIndex, pageSize, "ProductNo desc", dic, new { KeyWord = keyword, CategoryList = categoryList.ToArray(), SubjectNo = subjectNo, IsShelf = isShelf, BrandNo = brandNo, CategoryNo = categoryNo, GenderStyle = genderStyle }).ToList();
            return PageConvertor.Convert(pageIndex, pageSize, productList);
        }

        public IList<SWfsChannelSord> GetChannelSordList(int siteNo)
        {
            IList<SWfsChannelSord> sordList = DapperUtil.Query<SWfsChannelSord>("ComBeziWfs_SWfsChannelSord_SelectChannelSordListBySiteNo", new { SiteNo = siteNo }).ToList();
            return sordList;
        }

        public WfsProduct ReadProduct(string productNo)
        {
            WfsProduct product = DapperUtil.QueryByIdentityWithNoLock<WfsProduct>(productNo);
            return product;
        }

        /// <summary>
        /// 根据子类编号查询其商品编号
        /// </summary>
        /// <param name="scategoryNo"></param>
        /// <returns></returns>
        public IList<string> SelectSubjectProductRef(string scategoryNo)
        {
            IList<String> splist = DapperUtil.Query<String>("ComBeziWfs_SWfsSubjectProductRef_ReadByCategoryNo", new { CategoryNo = scategoryNo }).ToList();
            return splist;
        }

        public IList<SpCategory> GetERPCategoryListByParentNo(string parentNo)
        {
            IList<SpCategory> erpcategoryList = DapperUtil.Query<SpCategory>("ComBeziWfs_SpCategory_SelectByParentNo", new { ParentNo = parentNo }).ToList();
            return erpcategoryList;
        }

        public string InserSystemImg(SystemPictureFile model)
        {
            DapperUtil.Insert<SystemPictureFile>(model);
            return model.PictureFileNo;
        }


        public int InsertSubject(SWfsSubject subject)
        {
            return DapperUtil.Insert(subject, false);
        }

        /// <summary>
        /// 活动和所属频道关联
        /// </summary>
        /// <param name="channelsordref"></param>
        /// <returns></returns>
        public int InsertSubjectChannelSordRef(SWfsSubjectChannelSordRef channelsordref)
        {
            return DapperUtil.Insert(channelsordref, false);
        }

        /// <summary>
        /// 活动和所属品类关联
        /// </summary>
        /// <param name="categoryref"></param>
        /// <returns></returns>
        public int InsertSubjectCategoryRef(SWfsSubjectCategoryRef categoryref)
        {
            return DapperUtil.Insert(categoryref, false);
        }

        public int InsertSubjectCategory(SWfsSubjectCategory subjectcategry)
        {
            return DapperUtil.Insert(subjectcategry, false);
        }

        /// <summary>
        /// 添加活动日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public int InsertSubjectOrChannelLog(SWfsSubjectOrChannelLog log)
        {
            return DapperUtil.Insert(log, false);
        }

        public IList<SWfsChannelSord> GetSWfsChannelSordList(string name, string datecreate, int siteNo = 2)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("SordName", name ?? "");
            dic.Add("DateCreate", datecreate ?? "");
            IList<SWfsChannelSord> list = DapperUtil.Query<SWfsChannelSord>("ComBeziWfs_SWfsChannelSord_ReadChannelSordListByContion", dic, new { SiteNo = siteNo, SordName = name, DateCreate = datecreate }).ToList();
            list = list.Where(r => r.Status == 1).OrderBy(r => r.SortNo).OrderByDescending(r => r.DateCreate).ToList();
            return list;

        }

        public int UpdateChannelSordBySordNo(string sordNo, string sordName, out string msg)
        {
            string tmpMsg = string.Empty;
            SWfsChannelSord model = DapperUtil.Query<SWfsChannelSord>("ComBeziWfs_SWfsChannelSord_ReadChannelSordBySordName", new { SordName = sordName }).FirstOrDefault();
            if (model != null && !model.SordNo.Equals(sordNo))
            {
                msg = "分类名称重复";
                return 2;
            }
            bool rs = DapperUtil.UpdatePartialColumns<SWfsChannelSord>(new { SordNo = sordNo, SordName = sordName, DateUpdate = DateTime.Now });
            if (!rs)
            {
                msg = "保存失败";
                return 3;
            }
            msg = string.Empty;
            return 1;
        }

        public int DelSord(string sordNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsChannelSord_DelChannelSordBySordNo", new { SordNo = sordNo });
        }

        public int AddSord(string sordName, out string msg)
        {

            SWfsChannelSord tmpModel = DapperUtil.Query<SWfsChannelSord>("ComBeziWfs_SWfsChannelSord_ReadChannelSordBySordName", new { SordName = sordName }).FirstOrDefault();
            int b = (tmpModel != null) ? 2 : 0;
            if (b == 2)
            {
                msg = "分类名称已存在";
                return 2;
            }
            SWfsChannelSord model = new SWfsChannelSord();
            model.SordNo = DateTime.Now.ToString("yyyyMMdd") + new CommonService().GetNextCounterId("SordNo").ToString("000");
            model.SordName = sordName;
            model.SortNo = 0;
            model.Status = 1;
            model.SiteNo = 2;
            model.DateCreate = DateTime.Now;
            model.CreateUserId = PresentationHelper.GetPassport().UserName;
            model.DateUpdate = DateTime.Now;
            try
            {

                DapperUtil.Insert<SWfsChannelSord>(model);
                b = 1;
                msg = string.Empty;
                return b;
            }
            catch (Exception)
            {
                b = 3;
                msg = "保存失败";
                return b;
            }
            msg = string.Empty;
            return b;
        }
        /// <summary>
        /// 获得一条活动信息
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public SubjectInfo GetSubjectInfo(string subjectNo)
        {
            DynamicParameters SaleParam = new DynamicParameters();
            SaleParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            return DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_SelectBySubjectNo", SaleParam).FirstOrDefault();
        }

        /// <summary>
        /// 根据活动编号获得所属分类
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SWfsSubjectChannelSordRef> GetSordBySubjectNo(string subjectNo)
        {
            return DapperUtil.Query<SWfsSubjectChannelSordRef>("ComBeziWfs_SWfsSubjectChannelSordRef_SelectBySubjectNo", new { SubjectNo = subjectNo }).ToList();
        }


        /// <summary>
        /// 根据活动编号获得活动信息
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SWfsSubject> GetSWfsSubjectBySubjectNo(string subjectNo)
        {
            return DapperUtil.Query<SWfsSubject>("ComBeziWfs_SWfsSubject_SelectEntityBySubjectNo", new { SubjectNo = subjectNo }).ToList();
        }

        /// <summary>
        /// 根据活动编号获得所属品类
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SWfsSubjectCategoryRef> GetErpCategoryListBySubjectNo(string subjectNo)
        {
            DynamicParameters SaleParam = new DynamicParameters();
            SaleParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            return DapperUtil.Query<SWfsSubjectCategoryRef>("ComBeziWfs_SWfsSubjectCategoryRef_SelectBySubjectNo", SaleParam).ToList();
        }
        /// <summary>
        /// 删除分类与活动的关联
        /// </summary>
        /// <param name="subjectNO"></param>
        /// <returns></returns>
        public int DeleteSubjectChannelSordRef(string subjectNO)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSubjectChannelSordRef_DeleteBySubjectNo", new { SubjectNo = subjectNO });
        }
        /// <summary>
        /// 删除品类与活动的关联
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public int DeleteSubjectCategoryRef(string subjectNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSubjectCategoryRef_DeleteBySubjectNo", new { SubjectNo = subjectNo });
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public bool UpdateSubject(SubjectInfo subject)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSubject>(new
            {
                SubjectNo = subject.SubjectNo,
                SubjectName = subject.SubjectName,
                SubjectEnName = subject.SubjectEnName,
                ChannelNo = subject.ChannelNo,
                Status = subject.Status,
                DateBegin = subject.DateBegin,
                DateEnd = subject.DateEnd,
                DateCreate = subject.DateCreate,
                IPhoneText = subject.IPhoneText,
                DiscountType = subject.DiscountType,
                SubjectTemplate = subject.SubjectTemplate,
                ContentTitleOne = subject.ContentTitleOne,
                ContentIntroduction = subject.ContentIntroduction,
                ContentTitleTwo = subject.ContentTitleTwo,
                ContentRecommended = subject.ContentRecommended,
                BelongsSubjectPic = subject.BelongsSubjectPic,
                TitlePic1 = subject.TitlePic1,
                TitlePic2 = subject.TitlePic2,
                BackgroundPic = subject.BackgroundPic,
                IPhonePic = subject.IPhonePic,
                FlapPic = subject.FlapPic,
                PrivilegeValue = subject.PrivilegeValue,
                SubjectPreStartTemplateType = subject.SubjectPreStartTemplateType,
                BrandContent = subject.BrandContent,
                BU = subject.BU,
                IsPreheat = subject.IsPreheat,
                BrandLogoType = subject.BrandLogoType,
                SalesInfo = subject.SalesInfo,
                HeadShowType = subject.HeadShowType,
                HeadWebHtml = subject.HeadWebHtml,
                HeadMobileHtml = subject.HeadMobileHtml,
                SubjectTitle = subject.SubjectTitle
            });
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="picNo"></param>
        /// <returns></returns>
        public int DeleteImg(string picNo)
        {
            return DapperUtil.Execute("ComBeziPic_SystemPictureFile_DeleteByPicNo", new { PictureFileNo = picNo });
        }

        /// <summary>
        /// 查询活动子类下关联的商品
        /// </summary>
        /// <param name="categoryNo">商品品类</param>
        /// <param name="scategoryNo">活动子分类编号</param>
        /// <param name="brandNo">品牌编号</param>
        /// <param name="isShelf">是否上架</param>
        /// <param name="keyword">关键词</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页数</param>
        /// <returns></returns>
        public RecordPage<SubjectProductRef> SelectSubjectProductRefList(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo ?? "");//商品的品类
            dic.Add("SCategoryNo", scategoryNo ?? "");//活动子分类
            dic.Add("BrandNo", brandNo ?? "");
            dic.Add("IsShelf", isShelf ?? "");
            dic.Add("Keyword", keyword ?? "");
            IEnumerable<SubjectProductRef> query = DapperUtil.QueryPaging<SubjectProductRef>("ComBeziWfs_SWfsSubjectProductRef_SelectByCategoryNo",
                pageIndex, pageSize, "SortNo ASC,SWfsSubjectProductRef.DateCreate DESC", dic,
                new
                {
                    KeyWord = keyword ?? "",
                    SCategoryNo = scategoryNo ?? "",
                    IsShelf = isShelf ?? "",
                    BrandNo = brandNo ?? "",
                    CategoryNo = categoryNo ?? ""
                });
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }

        public IList<SubjectProductRef> SelectSubjectProductRefListRead(string categoryNo, List<string> scategoryNo, string brandNo, string isShelf, string keyword, string genderStyle)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo ?? "");//商品的品类
            dic.Add("BrandNo", brandNo ?? "");
            dic.Add("IsShelf", isShelf ?? "");
            dic.Add("Keyword", keyword ?? "");
            dic.Add("GenderStyle", genderStyle ?? "");
            IEnumerable<SubjectProductRef> query = DapperUtil.Query<SubjectProductRef>("ComBeziWfs_SWfsSubjectProductRef_SelectByCategoryNoRead",
                dic, new
                {
                    KeyWord = keyword ?? "",
                    IsShelf = isShelf ?? "",
                    BrandNo = brandNo ?? "",
                    CategoryNo = categoryNo ?? "",
                    SCategoryNo = scategoryNo.ToArray(),
                    GenderStyle = genderStyle ?? ""
                });
            return query.ToList();
        }

        public IList<SubjectProductRef> SelectSubjectProductRefListII(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword, string genderStyle)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo ?? "");//商品的品类
            dic.Add("SCategoryNo", scategoryNo ?? "");//活动子分类
            dic.Add("BrandNo", brandNo ?? "");
            dic.Add("IsShelf", isShelf ?? "");
            dic.Add("Keyword", keyword ?? "");
            dic.Add("GenderStyle", genderStyle ?? "");
            IEnumerable<SubjectProductRef> query = DapperUtil.Query<SubjectProductRef>("ComBeziWfs_SWfsSubjectProductRef_SelectByCategoryNo",
                dic, new
                {
                    KeyWord = keyword ?? "",
                    SCategoryNo = scategoryNo ?? "",
                    IsShelf = isShelf ?? "",
                    BrandNo = brandNo ?? "",
                    CategoryNo = categoryNo ?? "",
                    GenderStyle = genderStyle ?? ""
                });
            return query.ToList();
        }

        /// <summary>
        /// 商品可视化查询
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<SubjectProductRef> SelectSubjectProductRefList(string category)
        {
            return DapperUtil.Query<SubjectProductRef>("ComBeziWfs_SWfsSubjectProductRef_SelectByCategoryNoView", new { CategoryNo = category, ShowTime = DateTime.Now.ToString() }).ToList();
        }

        /// <summary>
        /// 根据品类二级编码获取品类信息 by lijibo
        /// </summary>
        /// <param name="CategoryNo"></param>
        /// <returns></returns>
        public IList<WfsErpCategory> GetWfsErpCategoryListByCategoryNo(string CategoryNo)
        {
            return DapperUtil.Query<WfsErpCategory>("ComBeziWfs_WfsErpCategory_SelectByCategoryNo", new { CategoryNo = CategoryNo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) }).ToList();
        }

        /// <summary>
        /// 根据颜色获取erp详细颜色信息
        /// </summary>
        /// <param name="colorAll"></param>
        /// <returns></returns>
        public IList<WfsPrimaryColor> GetWfsPrimaryColorListByColorDetail(string colorAll)
        {
            string[] colorstring = colorAll.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return DapperUtil.Query<WfsPrimaryColor>("ComBeziWfs_WfsPrimaryColor_SelectByColor", new { ColorName = colorstring }).ToList();
        }


        /// <summary>
        /// 根据颜色获取erp详细颜色信息
        /// </summary>
        /// <param name="colorAll"></param>
        /// <returns></returns>
        public IList<WfsProductColorAttrDetail> GetWfsProductColorAttrDetailListByColorDetail(string colorAll)
        {
            string[] colorstring = colorAll.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] _colors = new int[colorstring.Length];
            for (int i = 0; i < colorstring.Length; i++)
            {
                _colors[i] = Convert.ToInt32(colorstring[i]);
            }
            return DapperUtil.Query<WfsProductColorAttrDetail>("ComBeziWfs_WfsProductColorAttrDetail_SelectByColor", new { PrimaryColorId = _colors }).ToList();
        }

        /// <summary>
        /// 根据一级颜色ID获取一级颜色信息 by lijibo
        /// </summary>
        /// <param name="colorAll"></param>
        /// <returns></returns>
        public IList<WfsPrimaryColor> GetWfsPrimaryColorListByColorID(string colorAll)
        {
            string[] colorstring = colorAll.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] _colors = new int[colorstring.Length];
            for (int i = 0; i < colorstring.Length; i++)
            {
                _colors[i] = Convert.ToInt32(colorstring[i]);
            }
            return DapperUtil.Query<WfsPrimaryColor>("ComBeziWfs_WfsPrimaryColor_SelectByColorID", new { PrimaryColorId = _colors }).ToList();
        }


        /// <summary>
        /// 根据商品编码获取收藏信息  by lijibo
        /// </summary>
        /// <param name="colorAll"></param>
        /// <returns></returns>
        public IList<FavoriteProductCount> GetSWfsFavoriteProductByProductNos(string ProductNos)
        {
            string[] ProductNosString = ProductNos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return DapperUtil.Query<FavoriteProductCount>("ComBeziWfs_SWfsFavoriteProduct_SelectCountByProductNos", new { ProductNo = ProductNosString }).ToList();
        }

        /// <summary>
        /// 查询活动下存在的所有商品编号 modify lijibo 20141011
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<string> GetProductListBySubjectNo(string subjectNo, string isAll)
        {
            if (string.IsNullOrEmpty(isAll))
            {
                return DapperUtil.Query<string>("ComBeziWfs_SWfsSubjectProductRef_SelectBySubjectNo", new { SubjectNo = subjectNo }).ToList();
            }
            else
            {
                //这是之前的方法,应对 EP
                //return DapperUtil.Query<string>("ComBeziWfs_SWfsSubjectProductRef_SelectBySubjectNo1", new { SubjectNo = subjectNo }).ToList();
                return DapperUtil.Query<string>("ComBeziWfs_SWfsSubjectProductRef_SelectBySubjectNo1Outlet", new { SubjectNo = subjectNo }).ToList();
            }

        }
        /// <summary>
        /// 删除活动下关联的商品
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="productNo"></param>
        public void DeleteSubjectProductRef(string relationId)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsSubjectProductRef_DeleteBySubjectProductRefId", new { RelationId = relationId });
        }

        /// <summary>
        /// 于今天开始，但当前时刻还未开始的活动
        /// </summary>
        /// <param name="subjectType"></param>
        /// <param name="gender"></param>
        /// <param name="curPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="isNext"></param>
        /// <returns></returns>
        public IEnumerable<SubjectInfo> GetSubjects(string subjectType, string gender, int curPage, int pageSize)
        {
            string startDate = DateTime.Now.ToString("yyyy-MM-dd");
            string stopDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            var dic = new Dictionary<string, object>();
            dic["TopCount"] = pageSize;


            if (subjectType.Equals("NotStartedToDay"))
            {
                #region 于今天开始，但当前时刻还未开始的活动

                dic["type"] = 1;
                dic["OrderBy"] = "DateBegin desc";

                #endregion
            }
            else if (subjectType.Equals("ToDaySubject"))
            {
                #region 今天新开的活动

                dic["type"] = 2;
                dic["OrderBy"] = "DateCreate desc";
                startDate = DateTime.Now.ToString("yyyy-MM-dd");
                //stopDate = DateTime.Now.AddHours(24).ToString();

                #endregion
            }
            else if (subjectType.Equals("SaleingToDaySubject"))
            {
                #region 正在进行的活动

                dic["type"] = 5;
                dic["OrderBy"] = "DateCreate desc";
                startDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                stopDate = DateTime.Now.AddHours(24).ToString("yyyy-MM-dd hh:mm:ss");

                #endregion
            }
            else if (subjectType.Equals("AboutExpireSubject"))
            {
                #region 即将结束的活动

                dic["type"] = 3;
                dic["OrderBy"] = "DateCreate desc";
                startDate = DateTime.Now.ToString();
                stopDate = DateTime.Now.AddHours(24).ToString();

                #endregion
            }
            else if (subjectType.Equals("AboutBeginSubject"))
            {
                #region 即将开启的活动

                dic["type"] = 4;
                dic["OrderBy"] = "DateCreate desc";
                startDate = DateTime.Now.AddDays(1).ToShortDateString();
                stopDate = DateTime.Now.AddDays(2).ToShortDateString();

                #endregion
            }

            //var list = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_HomeSubjectList", dic, new { Gender = gender.ToString(), StartDate = startDate, StopDate = stopDate }).FilterList();
            var list = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_NewHomeSubjectList", dic, new { Gender = gender.ToString(), StartDate = startDate, StopDate = stopDate }).FilterList();

            //此处可优化！，通过js去显示余下的信息
            if (list == null)
            {
                return null;
            }
            else
            {
                //20131202添加 过滤类型为专题的活动
                //20140314修改 by lijia 去掉此过滤条件，读取常规活动和专题活动
                //list = list.Where(x => x.SubjectTemplate != 4).ToList();
                var totalCount = list.Count();
                return list.Skip(curPage * pageSize).Take(pageSize);
            }
        }

        /// <summary>
        /// 奥莱V3获取活动列表 by zhangwei 20140110
        /// </summary>
        /// <param name="subjectType"></param>
        /// <param name="gender"></param>
        /// <param name="curPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="isNext"></param>
        /// <returns></returns>
        public IEnumerable<SubjectInfo> GetSubjectsNew(string subjectType, string gender, int curPage, int pageSize)
        {
            string startDate = DateTime.Now.ToString("yyyy-MM-dd");
            string stopDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            var dic = new Dictionary<string, object>();
            dic["TopCount"] = pageSize;


            if (subjectType.Equals("NotStartedToDay"))
            {
                #region 于今天开始，但当前时刻还未开始的活动

                dic["type"] = 1;
                dic["OrderBy"] = "DateBegin desc";

                #endregion
            }
            else if (subjectType.Equals("ToDaySubject"))
            {
                #region 今天新开的活动

                dic["type"] = 2;
                dic["OrderBy"] = "DateCreate desc";
                startDate = DateTime.Now.ToString("yyyy-MM-dd");

                #endregion
            }
            else if (subjectType.Equals("SaleingToDaySubject"))
            {
                #region 正在进行的活动

                dic["type"] = 5;
                dic["OrderBy"] = "DateCreate desc";
                startDate = DateTime.Now.ToString("yyyy-MM-dd");
                stopDate = DateTime.Now.AddHours(24).ToString("yyyy-MM-dd HH:mm:ss");

                #endregion
            }
            else if (subjectType.Equals("AboutExpireSubject"))
            {
                #region 即将结束的活动

                dic["type"] = 3;
                dic["OrderBy"] = "DateCreate desc";
                startDate = DateTime.Now.ToString();
                stopDate = DateTime.Now.AddHours(24).ToString();

                #endregion
            }
            else if (subjectType.Equals("AboutBeginSubject"))
            {
                #region 即将开启的活动

                dic["type"] = 4;
                dic["OrderBy"] = "DateCreate desc";
                startDate = DateTime.Now.AddDays(1).ToShortDateString();
                stopDate = DateTime.Now.AddDays(2).ToShortDateString();

                #endregion
            }

            var list = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectListNew", dic, new { StartDate = startDate, StopDate = stopDate }).FilterList();

            //此处可优化！，通过js去显示余下的信息
            if (list == null)
            {
                return null;
            }
            else
            {
                //20131202添加 过滤类型为专题的活动
                //20140314修改 by lijia 去掉次过滤条件，活动可视化时读取常规活动和专题活动
                //list = list.Where(x => x.SubjectTemplate != 4).ToList();
                var totalCount = list.Count();
                return list.Skip(curPage * pageSize).Take(pageSize);
            }
        }

        /// <summary>
        /// 奥莱V3获取活动列表 by zhangwei 20140618
        /// </summary>
        /// <param name="subjectType">1今日新开，2正在进行中，3即将结束</param>
        /// <param name="channelNo">频道编号</param>
        /// <param name="showType">展示渠道</param>
        /// <param name="showDateTime">展示日期</param>
        /// <param name="curPage">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public IEnumerable<SubjectInfo> GetSubjectsNew(string subjectType, string channelNo, string showType, string showDateTime, int curPage, int pageSize)
        {
            DateTime nowTime = (!string.IsNullOrEmpty(channelNo) && channelNo.Equals("3") && !string.IsNullOrEmpty(showDateTime) ? DateTime.Parse(showDateTime + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second) : DateTime.Now);
            string startDate = nowTime.ToString("yyyy-MM-dd");
            string stopDate = nowTime.AddDays(1).ToString("yyyy-MM-dd");

            var dic = new Dictionary<string, object>();
            dic["TopCount"] = pageSize;
            dic["ShowType"] = showType;

            if (subjectType.Equals("NotStartedToDay"))
            {
                #region 于今天开始，但当前时刻还未开始的活动

                dic["type"] = 1;
                dic["OrderBy"] = "DateBegin desc";

                #endregion
            }
            else if (subjectType.Equals("ToDaySubject"))
            {
                #region 今天新开的活动

                dic["type"] = 2;
                dic["OrderBy"] = "DateCreate desc";
                startDate = nowTime.ToString("yyyy-MM-dd");
                stopDate = nowTime.ToString();
                #endregion
            }
            else if (subjectType.Equals("SaleingToDaySubject"))
            {
                #region 正在进行的活动

                dic["type"] = 5;
                dic["OrderBy"] = "DateCreate desc";
                startDate = nowTime.ToString("yyyy-MM-dd");
                stopDate = nowTime.AddHours(24).ToString("yyyy-MM-dd HH:mm:ss");

                #endregion
            }
            else if (subjectType.Equals("AboutExpireSubject"))
            {
                #region 即将结束的活动

                dic["type"] = 3;
                dic["OrderBy"] = "DateCreate desc";
                startDate = nowTime.ToString();
                stopDate = nowTime.AddHours(24).ToString();

                #endregion
            }
            else if (subjectType.Equals("AboutBeginSubject"))
            {
                #region 即将开启的活动

                dic["type"] = 4;
                dic["OrderBy"] = "DateCreate desc";
                startDate = nowTime.AddDays(1).ToString();
                stopDate = nowTime.AddHours(24).ToString();

                #endregion
            }

            var list = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_SubjectListNew", dic, new { StartDate = startDate, StopDate = stopDate, ShowType = showType }).FilterList();

            //此处可优化！，通过js去显示余下的信息
            if (list == null)
            {
                return null;
            }
            else
            {
                //20131202添加 过滤类型为专题的活动
                //20140314修改 by lijia 去掉次过滤条件，活动可视化时读取常规活动和专题活动
                //list = list.Where(x => x.SubjectTemplate != 4).ToList();
                var totalCount = list.Count();
                return list.Skip(curPage * pageSize).Take(pageSize);
            }
        }

        public IEnumerable<SubjectInfo> GetSubjectsMonitorNew(SubjectMonitorSearchParm parm)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SubjectNameNo", (parm == null || parm.SubjectNameNo == "活动编号/名称" || string.IsNullOrEmpty(parm.SubjectNameNo)) ? "" : parm.SubjectNameNo);
            dic.Add("BrandNo", (string.IsNullOrWhiteSpace(parm.BrandNo) || parm.BrandNo == "品牌") ? "" : parm.BrandNo);
            dic.Add("ChannelSord", string.IsNullOrWhiteSpace(parm.ChannelSord) ? "" : parm.ChannelSord);
            dic.Add("BU", string.IsNullOrWhiteSpace(parm.BU) ? "" : parm.BU);

            string startDate = DateTime.Now.ToString("yyyy-MM-dd");
            var list = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubjectTodayOpenSubject_SelectList", dic,
                new
                {
                    StartDate = startDate,
                    SubjectNameNo = parm.SubjectNameNo,
                    BrandNo = parm.BrandNo,
                    ChannelSord = parm.ChannelSord,
                    BU = parm.BU
                }).FilterList();

            //此处可优化！，通过js去显示余下的信息
            if (list == null)
            {
                return null;
            }
            else
            {
                //20131202添加 过滤类型为专题的活动
                //20140314修改 by lijia 去掉次过滤条件，活动可视化时读取常规活动和专题活动
                //list = list.Where(x => x.SubjectTemplate != 4).ToList();
                var totalCount = list.Count();
                return list;
            }
        }


        /// <summary>
        /// 根据银行代码获取奥莱活动(注：如果银行代码为空，按普通用户逻辑查询奥莱活动)
        /// </summary>
        /// <param name="bankCode">银行代码</param>
        /// <returns></returns>
        public List<SubjectInfo> GetSubjectInfoListByBandCode(string bankCode, int gender)
        {
            var dic = new Dictionary<string, object>();

            if (bankCode.IsNullOrEmpty())
            {
                dic["type"] = 1;//普通用户逻辑
            }
            else
            {
                dic["type"] = 2;//特定链接来源用户逻辑
            }

            return DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_HomeBDSubjectList", dic, new { Gender = gender, Belong = bankCode }).FilterList();
        }

        /// <summary>
        /// 活动列表页面 -商品列表信息
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="curPage">当前页</param>
        /// <param name="pageSize">读取条数</param>
        /// <returns></returns>
        public IEnumerable<SubjectProductInfo> GetSubjectProduct(string subjectNo, int curPage, int pageSize)
        {
            List<SubjectProductInfo> productPart = DapperUtil.Query<SubjectProductInfo>("ComBeziWfs_SWfsSubject_GetSubjectProduct", null, new { subjectNo = subjectNo, pageSize = pageSize }).ToList();
            return productPart;
        }



        /// <summary>
        /// 查询一、二、三、四级分类。
        /// </summary>
        public IList<SpCategory> SelectErpCategoryByParentNo(string parentNo)
        {
            IList<SpCategory> categorylist = DapperUtil.Query<SpCategory>("ComBeziWfs_SpCategory_SelectByParentNo", new { ParentNo = parentNo }).ToList();
            return categorylist;
        }


        public IList<SpBrand> SelectBrandList()
        {
            IList<SpBrand> brandlist = DapperUtil.Query<SpBrand>("ComBeziWfs_SpBrand_ListParent").ToList();
            return brandlist;
        }


        public List<BrandInfo> getBrandGroup(string selectType, string categoryno)
        {
            //XmlDataTable dt = new XmlDataTable("ErpBrand");
            List<BrandInfo> brandinfolist = new List<BrandInfo>();
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();


            //if (dic == null)
            //{
            dic = getBrandControlDate(selectType, categoryno); //建立缓存数据
            //}

            foreach (KeyValuePair<string, List<string>> p in dic)
            {
                BrandInfo brand = new BrandInfo();
                brand.BrandNo = p.Key;
                brand.BrandCnName = p.Value[1];
                brand.BrandEnName = p.Value[2];
                brand.FristLetter = p.Value[0].ToUpper();

                brandinfolist.Add(brand);
            }

            return brandinfolist;
        }



        public Dictionary<string, List<string>> getBrandControlDate(string selectType, string categoryno)
        {
            IList<SpBrand> brandlist = SelectBrandList();

            Dictionary<string, List<string>> BrandDic = new Dictionary<string, List<string>>();
            foreach (SpBrand brand in brandlist)
            {
                //if (brand.BrandCnName == null || brand.BrandCnName.Trim().Equals(""))
                //{
                //}
                //else if (brand.BrandEnName == null || brand.BrandEnName.Trim().Equals(""))
                //{
                //}
                //else

                if (brand.BrandEnName.Length > 0)
                {
                    try
                    {
                        string pinyinStr = CharactersConvertTopinyin.Convert(brand.BrandEnName.Substring(0, 1));
                        pinyinStr = pinyinStr.Substring(0, 1).ToLower();
                        if (!BrandDic.ContainsKey(brand.BrandNo))
                        {
                            List<string> L = new List<string>();
                            L.Add(pinyinStr);
                            L.Add(brand.BrandCnName);
                            L.Add(brand.BrandEnName);
                            BrandDic.Add(brand.BrandNo, L);
                        }
                    }
                    catch (Exception e)
                    {
                        string empty = string.Empty;
                    }
                }
            }

            //if (!string.IsNullOrEmpty(selectType))
            //{
            //    if (selectType.Equals("1")) //查询所有显示的品牌
            //    {
            //        //可选择的品牌必须包含已经关联的分类商品
            //        List<string> brands = this.SelectCategoryBrand(categoryno);
            //        Dictionary<string, List<string>> NewBrandDic = new Dictionary<string, List<string>>();
            //        foreach (KeyValuePair<string, List<string>> p in BrandDic)
            //        {
            //            if (brands.Contains(p.Key))
            //            {
            //                NewBrandDic.Add(p.Key, p.Value);
            //            }
            //        }

            //        //MemCacheManager.Create().SetCache("ErpBrandControlListIsShow_" + categoryno, NewBrandDic, DateTime.Now.AddMinutes(5));
            //        return NewBrandDic;
            //    }
            //    else if (selectType.Equals("2")) // 查询所有的国际知名品牌
            //    {
            //        //MemCacheManager.Create().SetCache("ErpBrandControlListInternational", BrandDic, DateTime.Now.AddMinutes(5));
            //    }
            //}
            //else
            //{
            //    //MemCacheManager.Create().SetCache("ErpBrandControlList", BrandDic, DateTime.Now.AddMinutes(5));
            //}
            //end modify 

            return BrandDic;
        }


        /// <summary>
        /// 查询某个OCS分类下的所有的品牌
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <returns></returns>
        public List<string> SelectCategoryBrand(string categoryNo)
        {
            List<string> branslist = new List<string>();// (List<string>)MemCacheManager.Create().GetCache("AllBrandByCategoryNo_" + categoryNo);
            if (branslist == null)
            {
                IList<string> brandlist = DapperUtil.Query<string>("ComBeziWfs_SWfsProductCategory_SelectBrandByCategoryNo", new { CategoryNo = categoryNo }).ToList();

                List<string> brands = new List<string>();
                foreach (string brandno in brandlist)
                {
                    if (!brands.Contains(brandno))
                    {
                        brands.Add(brandno);
                    }
                }
                //MemCacheManager.Create().SetCache("AllBrandByCategoryNo_" + categoryNo, brands, DateTime.Now.AddMinutes(30));
                return brands;
            }
            return branslist;
        }


        public IList<BrandInfo> getBrandGroupFristLetter(string selectType, string categoryno)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            IList<BrandInfo> brandFL = new List<BrandInfo>();
            //modify by hehuan 2012-10-16
            //if (!string.IsNullOrEmpty(selectType))
            //{
            //    if (selectType.Equals("1")) //查询所有显示的品牌
            //    {
            //        //dic = (Dictionary<string, List<string>>)MemCacheManager.Create().GetCache("ErpBrandControlListIsShow_" + categoryno);
            //    }
            //    else if (selectType.Equals("2")) // 查询所有的国际知名品牌
            //    {
            //        // dic = (Dictionary<string, List<string>>)MemCacheManager.Create().GetCache("ErpBrandControlListInternational");
            //    }
            //}
            //else
            //{
            //    //dic = (Dictionary<string, List<string>>)MemCacheManager.Create().GetCache("ErpBrandControlList");
            //}
            //if (dic == null)
            //{
            dic = getBrandControlDate(selectType, categoryno); //建立缓存数据
            //}
            //end modify

            ArrayList list = new ArrayList();
            foreach (KeyValuePair<string, List<string>> p in dic)
            {
                if (!list.Contains(p.Value[0].ToUpper()))
                {
                    list.Add(p.Value[0].ToUpper());
                }
            }
            list.Sort();
            foreach (object L in list)
            {
                BrandInfo brand = new BrandInfo();
                brand.FristLetter = L.ToString();
                brandFL.Add(brand);
            }
            return brandFL;
        }

        /// <summary>
        /// 保存商品排序
        /// </summary>
        /// <param name="subjectsort"></param>
        /// <returns></returns>
        public int InsertProductSort(SWfsSubjectProductSort productSort)
        {
            return DapperUtil.Insert(productSort);
        }

        public int DeleteProductSortBySubjectNO(string subjectNO, string ShowChannelType)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSubjectProductSort_DeleteBySubjectNO", new { SubjectNo = subjectNO, ShowChannelType = short.Parse(ShowChannelType) });
        }

        /// <summary>
        /// 移动端排序所用by lijibo
        /// </summary>
        /// <param name="subjectNO"></param>
        /// <param name="ShowChannelType"></param>
        /// <returns></returns>
        public int DeleteProductSortBySubjectNONoNUll(string subjectNO, string ShowChannelType)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSubjectProductSort_DeleteBySubjectNONull", new { SubjectNo = subjectNO, ShowChannelType = short.Parse(ShowChannelType) });
        }

        public IList<SWfsSubjectProductSort> GetProductSortList(string subjectNo)
        {
            return DapperUtil.Query<SWfsSubjectProductSort>("ComBeziWfs_SWfsSubjectProductSort_SelectBySubjectNo", new { SubjectNo = subjectNo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) }).ToList();
        }

        /// <summary>
        /// 根据分组编号，展示渠道获取实体集 by lijibo
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="ShowChannelType"></param>
        /// <returns></returns>
        public IList<SWfsSubjectProductSort> GetProductSortList(string subjectNo, short ShowChannelType)
        {
            return DapperUtil.Query<SWfsSubjectProductSort>("ComBeziWfs_SWfsSubjectProductSort_SelectBySubjectNoAndShowChannelType", new { SubjectNo = subjectNo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), ShowChannelType = ShowChannelType }).ToList();
        }

        /// <summary>
        /// 获取对应活动下的分组
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <returns>所有分组</returns>
        public IList<SWfsSubjectCategory> GetSWfsSubjectProductList(string subjectNo)
        {
            return DapperUtil.Query<SWfsSubjectCategory>("ComBeziWfs_SWfsSubjectCategory_SelectAllBySubjectNo", new { SubjectNo = subjectNo }).ToList();
        }

        /// <summary>
        /// 添加活动排序
        /// </summary>
        /// <param name="subjectsort"></param>
        /// <returns></returns>
        public int InsertSubjectSort(SWfsSubjectSort subjectsort)
        {
            return DapperUtil.Insert(subjectsort);
        }

        public IList<SWfsSubjectSort> GetSubjectList(int channelNo, short type)
        {
            return DapperUtil.Query<SWfsSubjectSort>("ComBeziWfs_SWfsSubjectSort_SelectByChannelNo", new { ChannelNo = channelNo, type = type }).ToList();
        }
        public IList<SWfsSubjectSort> GetSubjectSortListNew(object channelNo, short type)
        {
            return DapperUtil.Query<SWfsSubjectSort>("ComBeziWfs_SWfsSubjectSort_SelectByChannelNo", new { ChannelNo = channelNo, type = type }).ToList();
        }
        public IList<SWfsSubjectSort> GetSubjectSortListNew(object channelNo, short type, string showType, string showDateTime)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ShowType", showType ?? "");
            dic.Add("ShowDateTime", showDateTime ?? "");
            return DapperUtil.Query<SWfsSubjectSort>("ComBeziWfs_SWfsSubjectSort_SelectByChannelNoNew", dic, new { ChannelNo = channelNo, type = type, ShowType = showType, ShowDateTime = showDateTime }).ToList();
        }
        public int DeleteSubjectSortBySubjectNO(string channelNo, short type)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ShowType", "");
            dic.Add("ShowDateTime", "");
            return DapperUtil.Query<int>("ComBeziWfs_SWfsSubjectSort_DeleteBySubjectNO", dic, new { ChannelNo = channelNo, type = type, ShowType = "", ShowDateTime = "" }).FirstOrDefault();
        }
        public int DeleteSubjectSortBySubjectNO(string channelNo, short type, string showType, string showDateTime)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ShowType", showType ?? "");
            dic.Add("ShowDateTime", showDateTime ?? "");
            return DapperUtil.Query<int>("ComBeziWfs_SWfsSubjectSort_DeleteBySubjectNO", dic, new { ChannelNo = channelNo, type = type, ShowType = showType, ShowDateTime = showDateTime }).FirstOrDefault();
        }
        /// <summary>
        /// 活动日志
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SWfsSubjectOrChannelLog> GetLogList(string subjectNo, string key, string begintime, string endtime)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Key", key ?? "");
            dic.Add("BeginTime", begintime ?? "");
            dic.Add("EndTime", endtime ?? "");
            return DapperUtil.Query<SWfsSubjectOrChannelLog>("ComBeziWfs_SWfsSubjectOrChannelLog_SelectBySubjectNo", dic, new { SubjectNo = subjectNo, Key = key, BeginTime = begintime, EndTime = endtime }).ToList();
        }

        #region 数据统计
        /*20130923修改，原为统计到截止日期总数，先修改为时间段内统计数量*/
        /// <summary>
        /// 获取返回时间
        /// </summary>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="EndTime"></param>
        private void getBeginAndEndTime(string rang, ref string beginTime, ref string endTime)
        {
            switch (rang)
            {
                case "0":
                    beginTime = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;//自定义的不去处理
                case "1":
                    beginTime = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "2":
                    beginTime = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.Date.AddMilliseconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "3":
                    beginTime = DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "4":
                    beginTime = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "5":
                    if (string.IsNullOrEmpty(beginTime)) { beginTime = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss"); }
                    if (string.IsNullOrEmpty(endTime)) { endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
                    break;
                default:
                    if (string.IsNullOrEmpty(beginTime)) { beginTime = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss"); }
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;//自定义的不去处理
            }
        }

        /// <summary>
        /// 销售统计
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SubjectSaleStatistic GetSubjectSaleStatistic(string subjectNo, string rang, string beginTime, string endTime)
        {

            //根据rang获取开始，结束时间
            getBeginAndEndTime(rang, ref beginTime, ref endTime);
            //查询出当前销售信息，和以前最后一条销售信息
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("rang", rang);

            DynamicParameters SaleParam = new DynamicParameters();
            SaleParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            SaleParam.Add("BeginTime", beginTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            SaleParam.Add("EndTime", endTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            IList<SubjectSaleStatistic> list = DapperUtil.Query<SubjectSaleStatistic>("ComBeziReport_SubjectSaleStatistics_SelectByConditon", dic, SaleParam).ToList();
            SubjectSaleStatistic mSale = null;
            if (list != null && list.Count > 0)
            {
                mSale = new SubjectSaleStatistic();
                //如果rand为0 查询全部 时间倒叙返回一天信息
                if (rang == "0")
                {
                    mSale = list.FirstOrDefault();
                }
                else
                {
                    //rang不为0，如果返回一条信息，判断是否为要查询的信息，是就返回，否没有最新记录
                    if (list.Count == 1)
                    {
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mSale = list.FirstOrDefault();
                        }
                        else
                        {
                            mSale = null;
                        }
                    }
                    //如果是返回2条信息，判断哪条是最新信息，返回最新信息减去以前的最后一条信息
                    else if (list.Count == 2)
                    {
                        SubjectSaleStatistic mSaleNow = null;
                        SubjectSaleStatistic mSaleBef = null;
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mSaleNow = list[0];
                            mSaleBef = list[1];
                        }
                        else
                        {
                            mSaleBef = list[0];
                            mSaleNow = list[1];
                        }
                        mSale.SubjectNo = mSaleNow.SubjectNo;
                        mSale.DateStatistics = mSaleNow.DateStatistics;
                        mSale.OrderNums = mSaleNow.OrderNums - mSaleBef.OrderNums;
                        mSale.Amount = mSaleNow.Amount - mSaleBef.Amount;
                        mSale.CostAmount = mSaleNow.CostAmount - mSaleBef.CostAmount;
                        mSale.StockCount = mSaleNow.StockCount;
                        mSale.SKUCount = mSaleNow.SKUCount - mSaleBef.SKUCount;
                        mSale.SaleCount = mSaleNow.SaleCount - mSaleBef.SaleCount;
                        mSale.SaledSKUCount = mSaleNow.SaledSKUCount - mSaleBef.SaledSKUCount;
                    }
                    else
                    {
                        mSale = null;
                    }
                }
            }
            return mSale;

        }
        /// <summary>
        /// 访问统计
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SubjectVisitStatistic GetSubjectVisitStatistic(string subjectNo, string rang, string beginTime, string endTime)
        {
            //根据rang获取开始，结束时间
            getBeginAndEndTime(rang, ref beginTime, ref endTime);
            //查询出当前统计信息，和以前最后一条统计信息
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("rang", rang);

            DynamicParameters VisitParam = new DynamicParameters();
            VisitParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            VisitParam.Add("BeginTime", beginTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            VisitParam.Add("EndTime", endTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            IList<SubjectVisitStatistic> list = DapperUtil.Query<SubjectVisitStatistic>("ComBeziReport_SubjectVisitDataStatistics_SelectByConditon", dic, VisitParam).ToList();
            SubjectVisitStatistic mVisit = null;
            if (list != null && list.Count > 0)
            {
                mVisit = new SubjectVisitStatistic();
                //如果rand为0 查询全部 时间倒叙返回一天信息
                if (rang == "0")
                {
                    mVisit = list.FirstOrDefault();
                }
                else
                {
                    //rang不为0，如果返回一条信息，判断是否为要查询的信息，是就返回，否没有最新记录
                    if (list.Count == 1)
                    {
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mVisit = list.FirstOrDefault();
                        }
                        else
                        {
                            mVisit = null;
                        }
                    }
                    //如果是返回2条信息，判断哪条是最新信息，返回最新信息减去以前的最后一条信息
                    else if (list.Count == 2)
                    {
                        SubjectVisitStatistic mVisitNow = null;
                        SubjectVisitStatistic mVisitBef = null;
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mVisitNow = list[0];
                            mVisitBef = list[1];
                        }
                        else
                        {
                            mVisitBef = list[0];
                            mVisitNow = list[1];
                        }
                        mVisit.SubjectNo = mVisitNow.SubjectNo;
                        mVisit.DateStatistics = mVisitNow.DateStatistics;
                        mVisit.PV = mVisitNow.PV - mVisitBef.PV;
                        mVisit.UV = mVisitNow.UV - mVisitBef.UV;
                        mVisit.VIP = mVisitNow.VIP - mVisitBef.VIP;
                        mVisit.GoldenVIP = mVisitNow.GoldenVIP;
                        mVisit.PlatinaVIP = mVisitNow.PlatinaVIP - mVisitBef.PlatinaVIP;
                        mVisit.DiamondVIP = mVisitNow.DiamondVIP - mVisitBef.DiamondVIP;
                        mVisit.OrderNums = mVisitNow.OrderNums - mVisitBef.OrderNums;
                    }
                    else
                    {
                        mVisit = null;
                    }
                }
            }
            return mVisit;

        }
        public IList<SubjectProductStatistic> GetSubjectProductStatisticList(string subjectNo, string rang, string beginTime, string endTime)
        {
            //根据rang获取开始，结束时间
            getBeginAndEndTime(rang, ref beginTime, ref endTime);

            DynamicParameters ProductParam = new DynamicParameters();
            ProductParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            ProductParam.Add("BeginTime", beginTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            ProductParam.Add("EndTime", endTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            IList<SubjectProductStatistic> list = DapperUtil.Query<SubjectProductStatistic>("ComBeziReport_SubjectProductDataStatistics_SelectByConditon", ProductParam).ToList();
            if (rang == "0")
            {
                return list;
            }

            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    DynamicParameters ProductBeforeParam = new DynamicParameters();
                    ProductBeforeParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
                    ProductBeforeParam.Add("BeginTime", beginTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    ProductBeforeParam.Add("BrandNo", item.BrandNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 50);
                    ProductBeforeParam.Add("CategoryNo", item.CategoryNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 20);
                    SubjectProductStatistic mProductStatistic = null;
                    mProductStatistic = DapperUtil.Query<SubjectProductStatistic>("ComBeziReport_SubjectProductDataStatistics_SelectBeforeProject", ProductBeforeParam).FirstOrDefault();
                    if (mProductStatistic != null)
                    {
                        item.SKUCount = item.SKUCount - mProductStatistic.SKUCount;
                        item.StockCount = item.StockCount - mProductStatistic.StockCount;
                        item.SaleCount = item.SaleCount - mProductStatistic.SaleCount;
                    }
                }

            }
            return list;
        }
        #endregion
        /// <summary>
        /// 修改活动折扣类型和值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdateSubjectDiscountType(string subjectNo, short type, decimal value)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSubject>(new { SubjectNo = subjectNo, DiscountType = type, DiscountTypeValue = value });
        }

        /// <summary>
        /// 上传活动推广图
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public bool UpdateSpreadPicture(SubjectInfo subject)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSubject>(new
            {
                SubjectNo = subject.SubjectNo,
                SpreadPicture = subject.SpreadPicture
            });
        }

        /// <summary>
        /// 根据活动编号获得所属频道
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public List<SWfsChannel> GetChannelBySubjectNo(string subjectNo)
        {
            return DapperUtil.Query<SWfsChannel>("ComBeziWfs_SWfsChannel_ReadBySubjectNo", new { SubjectNo = subjectNo }).ToList();

        }

        #region 计算商品折扣
        /// <summary>
        /// 专题活动下的商品变动时重新计算活动的折和 显示值
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="sCategoryNo">此参数已作废</param>
        public int ExecuteDiscountTypeValue(string subjectNo, string sCategoryNo = "")
        {
            //最新修改2013-8-26

            // 之前的写法 var pList =DapperUtil.Query<string>("ComBeziWfs_WfsProduct_selectProductBySubjectNoDisCount", new { SubjectNo = subjectNo });
            IEnumerable<string> tempList = DapperUtil.Query<string>("ComBeziWfs_WfsProduct_selectProductBySubjectNoDisCountOutlet", new { SubjectNo = subjectNo });
            string tempProductNos = string.Empty;
            foreach (string prModel in tempList)
            {
                tempProductNos += prModel + ",";
            }
            List<SubjectDisCountType> pList = new List<SubjectDisCountType>();
            IList<SkuExtendPrice> SkuList = OutletGetSkuListByProductNo(tempProductNos);
            if (SkuList != null && SkuList.Count() > 0)
            {
                foreach (string prModel in tempList)
                {
                    SkuExtendPrice _singleEntity = SkuList.Where(c => c.ProductNo == prModel).FirstOrDefault();
                    if (_singleEntity != null)
                    {
                        SubjectDisCountType model = new SubjectDisCountType();
                        model.LimitedVipPrice = _singleEntity.LimitedVipPrice;
                        model.DiscountRate = _singleEntity.DiscountRate;
                        pList.Add(model);
                    }
                }
            }

            SubjectInfo sub = GetSubjectInfo(subjectNo);
            //计算时间差 用于创建缓存时间
            #region 计算时间差 用于创建缓存时间 20131225(取消使用)
            TimeSpan tsnow = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan tsend = new TimeSpan(sub.DateEnd.Ticks);
            double Hour = tsend.Subtract(tsnow).TotalHours;
            #endregion

            //如果活动类型是 "多少元" 如果添加的商品的价格不一样 活动类型修改为 "多少元起" 将最低价的商品修改为
            if (sub.DiscountType == 4 || sub.DiscountType == 5)//全场X
            {
                var subprolist = pList.OrderBy(o => o.LimitedVipPrice).Select(s => s.LimitedVipPrice).ToList().Distinct();
                short discountType = 0;
                if (subprolist.Count() > 1)
                {
                    discountType = 5;//全场X起
                }
                else
                {
                    discountType = 4;//全场X
                }
                var leastPrice = System.Decimal.Round(subprolist.FirstOrDefault(), 0);
                //增加缓存
                CreateDiscountTypeValueMemcached(subjectNo, leastPrice.ToString(), Hour);
                UpdateSubjectDiscountType(subjectNo, discountType, leastPrice);
            }
            //将"多少折" 修改为"多少折起"
            if (sub.DiscountType == 3 || sub.DiscountType == 1)//X折
            {
                var subprolist = pList.OrderBy(o => o.DiscountRate).Select(s => s.DiscountRate).ToList().Distinct();
                short discountType = 0;
                if (subprolist.Count() > 1)
                {
                    discountType = 1;//X折起
                }
                else
                {
                    discountType = 3;//X折
                }
                var leastPrice = subprolist.FirstOrDefault();
                //增加缓存
                CreateDiscountTypeValueMemcached(subjectNo, leastPrice.ToString(), Hour);
                UpdateSubjectDiscountType(subjectNo, discountType, leastPrice);
            }
            return pList.Count();
        }
        #endregion

        #region 创建缓存
        private void CreateDiscountTypeValueMemcached(string SubjectNo, string Value, double Hour)
        {

            string staticKey = "Aolai_Subject_DiscountInfo_By{0}";
            string CachKey = string.Empty;
            if (!string.IsNullOrEmpty(SubjectNo) && Hour > 0)
            {
                CachKey = string.Format(staticKey, SubjectNo);
                EnyimMemcachedClient.Instance.Set(CachKey, Value, TimeSpan.FromMinutes(2));
            }

        }
        #endregion

        /// <summary>
        /// 根据活动编号和查询条件获得其子类列表
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SWfsSubjectCategory> GetSubjectCategoryList(string subjectNo, string categoryName, int pageSize, int pageIndex, out int count)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryName", categoryName == null ? "" : categoryName);
            IList<SWfsSubjectCategory> list = DapperUtil.Query<SWfsSubjectCategory>("ComBeziWfs_SWfsSubjectCategory_SelectBySubjectNo", dic, new { SubjectNo = subjectNo, CategoryName = categoryName }).ToList();
            count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }
        /// <summary>
        /// 根据活动编号获得所有的子类数据
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SWfsSubjectCategory> GetSubjectCategoryList(string subjectNo)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryName", "");

            DynamicParameters SaleParam = new DynamicParameters();
            SaleParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            SaleParam.Add("CategoryName", "", System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            IList<SWfsSubjectCategory> list = DapperUtil.Query<SWfsSubjectCategory>("ComBeziWfs_SWfsSubjectCategory_SelectBySubjectNo", dic, SaleParam).ToList();

            return list;
        }
        /// <summary>
        /// 根据活动子类编号获得其信息
        /// </summary>
        /// <param name="categoryNo">活动子类编号</param>
        /// <returns></returns>
        public SWfsSubjectCategory GetSubjectCategoryModel(string categoryNo)
        {
            //MemcachedProvider
            SWfsSubjectCategory model = EnyimMemcachedClient.Instance.Get<SWfsSubjectCategory>("CMS_Aolai_SWfsSubjectCategory_By" + categoryNo);
            if (model != null)
            {
                return model;
            }
            else
            {
                model = DapperUtil.QueryByIdentity<SWfsSubjectCategory>(categoryNo);
                if (model != null)
                {
                    EnyimMemcachedClient.Instance.Set("CMS_Aolai_SWfsSubjectCategory_By" + categoryNo, model, TimeSpan.FromSeconds(60));
                }
            }
            return model;
        }
        /// <summary>
        /// 根据活动子类编号获得其信息(直接从sql中读取，而不是缓存中）
        /// </summary>
        /// <param name="categoryNo">活动子类编号</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public SWfsSubjectCategory GetSubjectCategoryModel(string categoryNo, bool flag)
        {
            SWfsSubjectCategory model = DapperUtil.Query<SWfsSubjectCategory>("ComBeziWfs_SWfsSubjectCategory_SelectByCategoryNo", null, new { CategoryNo = categoryNo }).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return new SWfsSubjectCategory();
        }
        /// <summary>
        /// 活动子类排序
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="sortNo"></param>
        /// <returns></returns>
        public bool UpdateSort(string categoryNo, int sortNo)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSubjectCategory>(new { CategoryNo = categoryNo, SortNo = sortNo });
        }

        /// <summary>
        /// 删除子类
        /// </summary>
        /// <param name="channelNo"></param>
        /// <returns></returns>
        public int DeleteSubjectCategory(string categoryNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSubjectCategory_DeleteByCategoryNo", new { CategoryNo = categoryNo });
        }

        /// <summary>
        /// 删除活动子类广告图
        /// </summary>
        /// <param name="fileNo"></param>
        public void DeletePic(string fileNo)
        {
            DapperUtil.Execute("ComBeziPic_SystemPictureFile_DeleteByPicFileNo", new { PictureFileNo = fileNo });
        }

        /// <summary>
        /// 修改活动子类
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public bool UpdateSubjectCategory(SWfsSubjectCategory sc)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSubjectCategory>(new
            {
                CategoryNo = sc.CategoryNo,
                CategoryName = sc.CategoryName,
                BuyType = sc.BuyType,
                AdPic = sc.AdPic,
                ShowType = sc.ShowType,
                WebHtmlText = sc.WebHtmlText,
                MobileHtmlText = sc.MobileHtmlText,
                SortRuleType = sc.SortRuleType,
                IsAutoBottom = sc.IsAutoBottom,
                MobileIsAutoBottom = sc.MobileIsAutoBottom,
                MobileSortRuleType = sc.MobileSortRuleType
            });
        }
        /// <summary>
        /// 删除活动子类下的商品
        /// </summary>
        /// <param name="categoryNo"></param>
        public void DeleteProductBycategoryNo(string categoryNo)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsSubjectProductRef_DeleteByCategoryNo", new { CategoryNo = categoryNo });
        }


        public RecordPage<SubjectProductRef> GetSubjectSpikeProductList(string subjectNo, int pageIndex, int pageSize)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SubjectNo", subjectNo);

            IEnumerable<SubjectProductRef> query = DapperUtil.QueryPaging<SubjectProductRef>("ComBeziWfs_SWfsSubjectSpikeProduct_GetSpikeProductBySubjectNo",
                pageIndex, pageSize, "SWfsSubjectProductRef.DateCreate DESC", dic,
                new
                {
                    SubjectNo = subjectNo
                });
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }


        public SpBrand GetBrandModelByBrandNo(string brandNo)
        {
            return DapperUtil.Query<SpBrand>("ComBeziWfs_SpBrand_GetModelOutlet", new { BrandNo = brandNo }).FirstOrDefault();
        }

        #region 奥莱活动数据统计 by zhangwei 20140304
        /// <summary>
        /// 销售统计
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SubjectSaleStatistic GetSubjectSaleStatisticNew(string subjectNo, string rang, string beginTime, string endTime)
        {
            //根据rang获取开始，结束时间
            if (string.IsNullOrEmpty(beginTime))
            {
                rang = "0";
                beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (string.IsNullOrEmpty(endTime))
            {
                rang = "0";
                endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            //查询出当前销售信息，和以前最后一条销售信息
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("rang", rang);

            DynamicParameters SaleParam = new DynamicParameters();
            SaleParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            SaleParam.Add("BeginTime", beginTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            SaleParam.Add("EndTime", endTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            IList<SubjectSaleStatistic> list = DapperUtil.Query<SubjectSaleStatistic>("ComBeziReport_SubjectSaleStatistics_SelectByConditonNew", dic, SaleParam).ToList();
            SubjectSaleStatistic mSale = null;
            if (list != null && list.Count > 0)
            {
                mSale = new SubjectSaleStatistic();
                //如果rand为0 查询全部 时间倒叙返回一天信息
                if (rang == "0")
                {
                    mSale = list.FirstOrDefault();
                }
                else
                {
                    //rang不为0，如果返回一条信息，判断是否为要查询的信息，是就返回，否没有最新记录
                    if (list.Count == 1)
                    {
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mSale = list.FirstOrDefault();
                        }
                        else
                        {
                            mSale = null;
                        }
                    }
                    //如果是返回2条信息，判断哪条是最新信息，返回最新信息减去以前的最后一条信息
                    else if (list.Count == 2)
                    {
                        SubjectSaleStatistic mSaleNow = null;
                        SubjectSaleStatistic mSaleBef = null;
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mSaleNow = list[0];
                            mSaleBef = list[1];
                        }
                        else
                        {
                            mSaleBef = list[0];
                            mSaleNow = list[1];
                        }
                        mSale.SubjectNo = mSaleNow.SubjectNo;
                        mSale.DateStatistics = mSaleNow.DateStatistics;
                        mSale.OrderNums = mSaleNow.OrderNums - mSaleBef.OrderNums;
                        mSale.Amount = mSaleNow.Amount - mSaleBef.Amount;
                        mSale.CostAmount = mSaleNow.CostAmount - mSaleBef.CostAmount;
                        mSale.StockCount = mSaleNow.StockCount;
                        mSale.SKUCount = mSaleNow.SKUCount - mSaleBef.SKUCount;
                        mSale.SaleCount = mSaleNow.SaleCount - mSaleBef.SaleCount;
                        mSale.SaledSKUCount = mSaleNow.SaledSKUCount - mSaleBef.SaledSKUCount;
                    }
                    else
                    {
                        mSale = null;
                    }
                }
            }
            return mSale;

        }

        /// <summary>
        /// 访问统计
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SubjectVisitStatistic GetSubjectVisitStatisticNew(string subjectNo, string rang, string beginTime, string endTime)
        {
            //根据rang获取开始，结束时间
            if (string.IsNullOrEmpty(beginTime))
            {
                rang = "0";
                beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (string.IsNullOrEmpty(endTime))
            {
                rang = "0";
                endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            //查询出当前统计信息，和以前最后一条统计信息
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("rang", rang);

            DynamicParameters SaleParam = new DynamicParameters();
            SaleParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            SaleParam.Add("BeginTime", beginTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            SaleParam.Add("EndTime", endTime, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            IList<SubjectVisitStatistic> list = DapperUtil.Query<SubjectVisitStatistic>("ComBeziReport_SubjectVisitDataStatistics_SelectByConditonNew", dic, SaleParam).ToList();
            SubjectVisitStatistic mVisit = null;
            if (list != null && list.Count > 0)
            {
                mVisit = new SubjectVisitStatistic();
                //如果rand为0 查询全部 时间倒叙返回一天信息
                if (rang == "0")
                {
                    mVisit = list.FirstOrDefault();
                }
                else
                {
                    //rang不为0，如果返回一条信息，判断是否为要查询的信息，是就返回，否没有最新记录
                    if (list.Count == 1)
                    {
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mVisit = list.FirstOrDefault();
                        }
                        else
                        {
                            mVisit = null;
                        }
                    }
                    //如果是返回2条信息，判断哪条是最新信息，返回最新信息减去以前的最后一条信息
                    else if (list.Count == 2)
                    {
                        SubjectVisitStatistic mVisitNow = null;
                        SubjectVisitStatistic mVisitBef = null;
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mVisitNow = list[0];
                            mVisitBef = list[1];
                        }
                        else
                        {
                            mVisitBef = list[0];
                            mVisitNow = list[1];
                        }
                        mVisit.SubjectNo = mVisitNow.SubjectNo;
                        mVisit.DateStatistics = mVisitNow.DateStatistics;
                        mVisit.PV = mVisitNow.PV - mVisitBef.PV;
                        mVisit.UV = mVisitNow.UV - mVisitBef.UV;
                        mVisit.VIP = mVisitNow.VIP - mVisitBef.VIP;
                        mVisit.GoldenVIP = mVisitNow.GoldenVIP;
                        mVisit.PlatinaVIP = mVisitNow.PlatinaVIP - mVisitBef.PlatinaVIP;
                        mVisit.DiamondVIP = mVisitNow.DiamondVIP - mVisitBef.DiamondVIP;
                        mVisit.OrderNums = mVisitNow.OrderNums - mVisitBef.OrderNums;
                    }
                    else
                    {
                        mVisit = null;
                    }
                }
            }
            return mVisit;

        }

        /// <summary>
        /// 读取活动查询方法
        /// </summary>
        /// <param name="scategoryNo"></param>
        /// <returns></returns>
        public IList<SubjectInfo> SelectSubjectList(string bgTime, string edTime, string subjectNos, string type)
        {
            if (type == "1" && string.IsNullOrEmpty(bgTime) && string.IsNullOrEmpty(edTime) && string.IsNullOrEmpty(subjectNos))
            { return null; }
            var dic = new Dictionary<string, object>();
            dic.Add("TypeValue", type);
            subjectNos = subjectNos.Trim();
            if (!string.IsNullOrEmpty(subjectNos))
            {
                if (subjectNos.Substring(subjectNos.Length - 1) == ",")
                {
                    subjectNos = subjectNos.Substring(0, subjectNos.Length - 1);
                }
            }
            DynamicParameters SaleParam = new DynamicParameters();
            SaleParam.Add("SubjectNos", subjectNos, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            SaleParam.Add("TypeValue", type, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            SaleParam.Add("BeginTime", bgTime, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);
            SaleParam.Add("EndTime", edTime, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input);


            IList<SubjectInfo> splist = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_GetSWfsSubjectByDataStatisticsList", dic, SaleParam).ToList();
            return splist;
        }

        #region 导出ToExcel活动报表数据新活动报表规则20140302 lxy
        /// <summary>
        /// 导出ToExcel活动报表数据新活动报表规则20140302 lxy
        /// </summary>
        /// <param name="subjectNo"></param>
        public void GetSubjectDataStatisticsToExcelNew(SubjectOLNewStatisticInfo newStatisticList)
        {
            #region 导出excel 信息 缺少支付金额
            StringBuilder sb = new StringBuilder("<h1>活动报表数据</h1>");
            if (newStatisticList != null)
            {
                sb.AppendLine("<h2>销售统计</h2>");
                sb.AppendLine("<table cellpadding=\"0\" cellspacing=\"1\" border=\"1\"  width=\"758px\" id=\"AccountListTable\" >");
                sb.AppendLine("<tr style=\"background-color:#FFFF00;\">");
                sb.AppendLine("<td width=\"10%\"><span>活动编号</span></td>");
                sb.AppendLine("<td><span>频道</span></td>");
                sb.AppendLine("<td><span>品类</span></td>");
                sb.AppendLine("<td><span>BU</span></td> ");
                sb.AppendLine("<td><span>创建人</span></td> ");
                sb.AppendLine("<td><span>状态</span></td> ");
                sb.AppendLine("<td><span>时间</span></td> ");
                sb.AppendLine("<td><span>成功订单</span></td> ");
                sb.AppendLine("<td><span>成功订单金额</span></td> ");
                sb.AppendLine("<td><span>订单转化率</span></td> ");
                sb.AppendLine("<td><span>总成本价</span></td> ");
                sb.AppendLine("<td><span>平均毛利率</span></td> ");
                sb.AppendLine("<td><span>总库存</span></td> ");

                sb.AppendLine("<td><span>可销售金额</span></td> ");
                sb.AppendLine("<td><span>总单款数</span></td> ");
                sb.AppendLine("<td><span>售罄率</span></td> ");
                sb.AppendLine("<td><span>金额售罄率</span></td> ");
                sb.AppendLine("<td><span>动销率</span></td> ");

                sb.AppendLine("<td><span>UV</span></td> ");
                sb.AppendLine("<td><span>PV</span></td> ");
                sb.AppendLine("<td><span>UV产值</span></td> ");
                sb.AppendLine("<td><span>会员数量</span></td> ");
                sb.AppendLine("</tr>");
                #region 导出excel格式模板
                foreach (var statisticSingle in newStatisticList.SubjectNewStatisticList)
                {
                    var Visit = statisticSingle.VisitStatistic;
                    var Sale = statisticSingle.SaleStatistic;

                    #region 频道
                    string channelName = string.Empty;
                    if (statisticSingle.ChannelList != null && statisticSingle.ChannelList.Count() > 0)
                    {
                        foreach (var item in statisticSingle.ChannelList)
                        {
                            channelName = channelName + item.ChannelName + "<br/>";
                        }
                    }
                    #endregion

                    #region 品类
                    string erpCategoryName = string.Empty;
                    IList<SWfsSubjectCategoryRef> CategoryRefList = statisticSingle.CategoryRefList;
                    if (CategoryRefList != null && CategoryRefList.Count() > 0)
                    {
                        foreach (var item in CategoryRefList)
                        {
                            SpCategory erpSingle = newStatisticList.ErpCategoryList.SingleOrDefault(p => p.CategoryNo == item.Category);
                            erpCategoryName = erpCategoryName + Regex.Replace(erpSingle.CategoryName, @"[a-zA-Z\s]", "").Trim().ToString() + "<br/>";
                        }
                    }
                    #endregion

                    sb.AppendLine("<tr>");
                    sb.AppendLine(String.Format("<td> {0}</td>", statisticSingle.NewSubject.SubjectNo));
                    sb.AppendLine(String.Format("<td> {0}</td>", channelName)); //频道
                    sb.AppendLine(String.Format("<td> {0}</td>", erpCategoryName)); //品类
                    sb.AppendLine(String.Format("<td> {0}</td>", !string.IsNullOrEmpty(statisticSingle.NewSubject.BU) ? statisticSingle.NewSubject.BU.Equals("0") ? "其它" : "BU" + statisticSingle.NewSubject.BU : "")); //BU
                    sb.AppendLine(String.Format("<td> {0}</td>", statisticSingle.NewSubject.CreateUserId));
                    sb.AppendLine(String.Format("<td> {0}</td>", statisticSingle.NewSubject.Status == 1 ? "开启" : "关闭"));
                    sb.AppendLine(String.Format("<td style=\"text-align:left;white-space:nowrap;\"> {0}</td>", "开始时间：" + statisticSingle.NewSubject.DateBegin + " <br />关闭时间：" + statisticSingle.NewSubject.DateEnd));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.OrderNums));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.Amount));
                    sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide((int)Visit.OrderNums, (int)Visit.UV, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.CostAmount));
                    sb.AppendLine(String.Format("<td> {0}</td>", (Sale.Amount == 0 ? "0" : (((Sale.Amount - Sale.CostAmount) / Sale.Amount) * 100).ToString(".##") + "%")));
                    sb.AppendLine(String.Format("<td> {0}</td>", (Sale.StockCount + Sale.SaleCount)));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.StockTotalAmount));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.SKUCount));
                    sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide(Sale.SaleCount, Sale.StockCount + Sale.SaleCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide(Sale.Amount, (int)Sale.StockTotalAmount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide(Sale.SaledSKUCount, Sale.SKUCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", Visit.UV));
                    sb.AppendLine(String.Format("<td> {0}</td>", Visit.PV));
                    sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide((int)(Sale.Amount), Visit.UV, 2)));
                    sb.AppendLine(String.Format("<td style=\"text-align:left;white-space:nowrap;\"> {0}</td>", "普通会员:" + Visit.VIP + "<br />黄金会员:" + Visit.GoldenVIP + "<br />白金会员:" + Visit.PlatinaVIP + "<br />钻石会员:" + Visit.DiamondVIP));
                    sb.AppendLine("</tr>");
                }

                #endregion


                sb.AppendLine("<tr>");
                sb.AppendLine("<td colspan=\"22\" style=\"text-align:left; font-size:13px; font-weight:bold; border-bottom:1px; border-bottom-style:inherit; border-bottom-color:Black;\">统计</td>");
                sb.AppendLine("</tr>");

                if (newStatisticList.SaleStatistic != null || newStatisticList.VisitStatistic != null)
                {
                    int subjectCount = newStatisticList.SubjectCount;
                    var Visit = newStatisticList.VisitStatistic;
                    var Sale = newStatisticList.SaleStatistic;
                    sb.AppendLine("<tr>");
                    sb.AppendLine("<td>合计：</td>");
                    sb.AppendLine(" <td></td>");
                    sb.AppendLine(" <td></td>");
                    sb.AppendLine(" <td></td>");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.OrderNums));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.Amount));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.CostAmount));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", (Sale.StockCount + Sale.SaleCount)));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.StockTotalAmount));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.SKUCount));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", Visit.UV));
                    sb.AppendLine(String.Format("<td> {0}</td>", Visit.PV));
                    sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide((int)(Sale.Amount), Visit.UV, 2)));
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine("</tr>");

                    sb.AppendLine("<tr>");
                    sb.AppendLine("<td>平均：</td>");
                    sb.AppendLine(" <td></td>");
                    sb.AppendLine(" <td></td>");
                    sb.AppendLine(" <td></td>");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)Sale.OrderNums, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)Sale.Amount, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.DivideDecimal((int)Visit.OrderNums, (int)Visit.UV, 2) / subjectCount).ToString() + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)Sale.CostAmount, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", (Sale.Amount == 0 || subjectCount <= 0) ? "0" : (((Sale.Amount - Sale.CostAmount) / Sale.Amount) * 100).ToString(".##") + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)(Sale.StockCount + Sale.SaleCount), subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)(Sale.StockTotalAmount), subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)(Sale.SKUCount), subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.DivideDecimal(Sale.SaleCount, Sale.StockCount + Sale.SaleCount, 2) / subjectCount).ToString() + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.DivideDecimal(Sale.Amount, (int)Sale.StockTotalAmount, 2) / subjectCount).ToString() + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.DivideDecimal(Sale.SaledSKUCount, Sale.SKUCount, 2) / subjectCount).ToString() + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue(Visit.UV, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue(Visit.PV, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.Divide((int)(Sale.Amount), Visit.UV, 2)).ToString()));
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine("</tr>");

                }
                sb.AppendLine("</table>");
            }

            CommonHelp.OutputExcel(sb.ToString(), "ReportTOTAL" + DateTime.Now.ToString());

            #endregion
        }
        #endregion

        /// <summary>
        /// 获取活动关联分类所属下的频道
        /// </summary>
        /// <returns></returns>
        public IList<SWfsChannel> GetSubjectChannelSord(string subjectNo)
        {
            DynamicParameters SaleParam = new DynamicParameters();
            SaleParam.Add("SubjectNo", subjectNo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 30);
            IList<SWfsChannel> list = DapperUtil.Query<SWfsChannel>("ComBeziWfs_SWfsChannel_GetChannelSordRefList", null, SaleParam).ToList();
            return list;
        }//GetSubjectChannelSord
        #endregion

        public SubjectInfo GetSubjectApplyInfo(string subjectNo)
        {
            return DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubjectApply_SubjectInfo", new { SubjectNo = subjectNo }).FirstOrDefault();
        }

        public void UpdateSWfsSubjectSort(SWfsSubjectSort sort)
        {
            DapperUtil.Update<SWfsSubjectSort>(sort);
        }

        public SWfsSubjectSort GetSWfsSubjectSort(string sno, short type, string channelNo)
        {
            return DapperUtil.Query<SWfsSubjectSort>("ComBeziWfs_SWfsSubjectSort_GetSWfsSubjectSort", new { Type = type, ChannelNo = channelNo, SubjectNo = sno }).FirstOrDefault();
        }

        /// <summary>
        /// 奥莱控制台-进行中活动列表
        /// </summary>
        /// <param name="parm">查询参数</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public RecordPage<SubjectInfo> GetSubjectMonitorRuning(SubjectMonitorSearchParm parm, int pageindex = 1, int pageSize = 10)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SubjectNameNo", (parm == null || parm.SubjectNameNo == "活动编号/名称" || string.IsNullOrEmpty(parm.SubjectNameNo)) ? "" : parm.SubjectNameNo);
            dic.Add("BrandNo", (string.IsNullOrWhiteSpace(parm.BrandNo) || parm.BrandNo == "品牌") ? "" : parm.BrandNo);
            dic.Add("ChannelSord", string.IsNullOrWhiteSpace(parm.ChannelSord) ? "" : parm.ChannelSord);
            dic.Add("BU", string.IsNullOrWhiteSpace(parm.BU) ? "" : parm.BU);
            dic.Add("QueryStartTime", string.IsNullOrWhiteSpace(parm.QueryStartTime) ? "" : parm.QueryStartTime);
            dic.Add("QueryEndTime", string.IsNullOrWhiteSpace(parm.QueryEndTime) ? "" : parm.QueryEndTime);
            DateTime now = DateTime.Now;
            string startDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
            string endDate = DateTime.Now.AddHours(24).ToString("yyyy-MM-dd HH:mm:ss");
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubjectMonitorRuning_SelectList", pageindex, pageSize, "DateBegin desc,DateCreate DESC", dic,
                new
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    SubjectNameNo = parm.SubjectNameNo,
                    BrandNo = parm.BrandNo,
                    ChannelSord = parm.ChannelSord,
                    BU = parm.BU,
                    QueryStartTime = parm.QueryStartTime,
                    QueryEndTime = parm.QueryEndTime
                }).FilterList();
            if (query == null)
            {
                return null;
            }
            return PageConvertor.Convert(pageindex, pageSize, query.ToList());
        }

        /// <summary>
        /// 奥莱控制台-进行中活动列表
        /// </summary>
        /// <param name="parm">查询参数</param>
        /// <returns></returns>
        public IEnumerable<SubjectInfo> GetSubjectMonitorRuning(SubjectMonitorSearchParm parm)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SubjectNameNo", (parm == null || parm.SubjectNameNo == "活动编号/名称" || string.IsNullOrEmpty(parm.SubjectNameNo)) ? "" : parm.SubjectNameNo);
            dic.Add("BrandNo", (string.IsNullOrWhiteSpace(parm.BrandNo) || parm.BrandNo == "品牌") ? "" : parm.BrandNo);
            dic.Add("ChannelSord", string.IsNullOrWhiteSpace(parm.ChannelSord) ? "" : parm.ChannelSord);
            dic.Add("BU", string.IsNullOrWhiteSpace(parm.BU) ? "" : parm.BU);
            dic.Add("QueryStartTime", string.IsNullOrWhiteSpace(parm.QueryStartTime) ? "" : parm.QueryStartTime);
            dic.Add("QueryEndTime", string.IsNullOrWhiteSpace(parm.QueryEndTime) ? "" : parm.QueryEndTime);
            DateTime now = DateTime.Now;
            string startDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
            string endDate = DateTime.Now.AddHours(24).ToString("yyyy-MM-dd HH:mm:ss");
            IEnumerable<SubjectInfo> query = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubjectMonitorRuning_SelectList", dic,
                new
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    SubjectNameNo = parm.SubjectNameNo,
                    BrandNo = parm.BrandNo,
                    ChannelSord = parm.ChannelSord,
                    BU = parm.BU,
                    QueryStartTime = parm.QueryStartTime,
                    QueryEndTime = parm.QueryEndTime
                }).FilterList();
            if (query == null)
            {
                return null;
            }
            return query.ToList();
        }
        #endregion

        /// <summary>
        /// 查询商品所属BU部门
        /// </summary>
        /// <returns></returns>
        public IList<WfsDepartment> GetDepartmentList()
        {
            return DapperUtil.Query<WfsDepartment>("ComBeziWfs_WfsDepartment_SelectAllList").ToList();
        }

        #region 以下应对 EP 数据结构变化

        #region 活动管理商品 by lijibo  20141002

        /// <summary>
        /// 活动管理商品 by lijibo  20141002
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="scategoryNo"></param>
        /// <param name="brandNo"></param>
        /// <param name="isShelf"></param>
        /// <param name="keyword"></param>
        /// <param name="genderStyle"></param>
        /// <returns></returns>
        public IList<SubjectProductRef> OutletSelectSubjectProductRefListRead(string categoryNo, List<string> scategoryNo, string brandNo, string isShelf, string keyword, string genderStyle)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo ?? "");//商品的品类
            dic.Add("BrandNo", brandNo ?? "");
            dic.Add("IsShelf", isShelf ?? "");
            dic.Add("Keyword", keyword ?? "");
            dic.Add("GenderStyle", genderStyle ?? "");
            IEnumerable<SubjectProductRef> query = DapperUtil.Query<SubjectProductRef>("ComBeziWfs_SWfsSubject_GetProductRefListOutlet",
                dic, new
                {
                    KeyWord = keyword ?? "",
                    IsShelf = isShelf ?? "",
                    BrandNo = brandNo ?? "",
                    CategoryNo = categoryNo ?? "",
                    SCategoryNo = scategoryNo.ToArray(),
                    GenderStyle = genderStyle ?? ""
                });
            return query.ToList();
        }


        /// <summary>
        /// 活动管理商品II by lijibo  20141002
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="scategoryNo"></param>
        /// <param name="brandNo"></param>
        /// <param name="isShelf"></param>
        /// <param name="keyword"></param>
        /// <param name="genderStyle"></param>
        /// <returns></returns>
        public IList<SubjectProductRef> OutletSelectSubjectProductRefListII(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword, string genderStyle)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo ?? "");//商品的品类
            dic.Add("SCategoryNo", scategoryNo ?? "");//活动子分类
            dic.Add("BrandNo", brandNo ?? "");
            dic.Add("IsShelf", isShelf ?? "");
            dic.Add("Keyword", keyword ?? "");
            dic.Add("GenderStyle", genderStyle ?? "");
            IEnumerable<SubjectProductRef> query = DapperUtil.Query<SubjectProductRef>("ComBeziWfs_SWfsSubject_GetProductRefListIIOutlet",
                dic, new
                {
                    KeyWord = keyword ?? "",
                    SCategoryNo = scategoryNo ?? "",
                    IsShelf = isShelf ?? "",
                    BrandNo = brandNo ?? "",
                    CategoryNo = categoryNo ?? "",
                    GenderStyle = genderStyle ?? ""
                });
            return query.ToList();
        }
        #endregion

        #region 管理商品列表  by lijibo 20141002

        /// <summary>
        /// 管理商品列表  by lijibo 20141002
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="categoryList"></param>
        /// <param name="subjectNo"></param>
        /// <param name="brandNo"></param>
        /// <param name="isShelf"></param>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="genderStyle"></param>
        /// <returns></returns>
        public RecordPage<ProductInfo> OutletGetSWfsSubjectProduct(string categoryNo, IList<string> categoryList, string subjectNo, string brandNo, string isShelf, string keyword, int pageIndex, int pageSize, string genderStyle)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("GenderStyle", genderStyle == null ? "" : genderStyle);
            IList<ProductInfo> productList = DapperUtil.QueryPaging<ProductInfo>("ComBeziWfs_SpProduct_GetCreateProductListOutlet", pageIndex, pageSize, "sp.ProductNo desc", dic, new { KeyWord = keyword, CategoryList = categoryList.ToArray(), SubjectNo = subjectNo, IsShelf = isShelf, BrandNo = brandNo, CategoryNo = categoryNo, GenderStyle = genderStyle }).ToList();
            return PageConvertor.Convert(pageIndex, pageSize, productList);
        }

        /// <summary>
        /// 获取商品列表II by lijibo 20141002
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="productNo">商品编号</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        public RecordPage<ProductInfo> OutletGetSWfsSubjectProductNew(string subjectNo, IList<string> productNo, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ProductNo", productNo);
            IList<ProductInfo> productList = DapperUtil.QueryPaging<ProductInfo>("ComBeziWfs_SpProduct_GetCreateProductListIIOutlet", pageIndex, pageSize, "sp.ProductNo desc", dic, new { SubjectNo = subjectNo, ProductNo = productNo }).ToList();
            return PageConvertor.Convert(pageIndex, pageSize, productList);
        }

        #endregion

        #region 查询活动中所有商品 用于导出EXCEL  by lijibo 20141002
        /// <summary>
        /// 查询活动中所有商品  by lijibo 20141002
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<ProductInfo> OutletGetProductList(string subjectNo)
        {
            IList<ProductInfo> productList = DapperUtil.Query<ProductInfo>("ComBeziWfs_SpProduct_SelectProductListToExcelOutlet", new { SubjectNo = subjectNo }).ToList();
            return productList;
        }
        #endregion

        #region 查询秒杀商品   by lijibo 20141002
        /// <summary>
        /// 查询秒杀商品   by lijibo 20141002
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public RecordPage<SubjectProductRef> OutletGetSubjectSpikeProductList(string subjectNo, int pageIndex, int pageSize)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SubjectNo", subjectNo);

            IEnumerable<SubjectProductRef> query = DapperUtil.QueryPaging<SubjectProductRef>("ComBeziWfs_SWfsSubjectSpikeProduct_GetSpikeProductBySubjectNoOutlet",
                pageIndex, pageSize, "SpRef.DateCreate DESC", dic,
                new
                {
                    SubjectNo = subjectNo
                });
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }
        #endregion

        #region 查询一、二、三、四级分类 by lijibo 20141002

        /// <summary>
        /// 查询一、二、三、四级分类 by lijibo 20141002
        /// </summary>
        /// <param name="parentNo"></param>
        /// <returns></returns>
        public IList<SpCategory> OutletSelectErpCategoryByParentNo(string parentNo)
        {
            IList<SpCategory> categorylist = DapperUtil.Query<SpCategory>("ComBeziWfs_SpCategory_SelectByParentNo", new { ParentNo = parentNo }).ToList();
            return categorylist;
        }
        #endregion

        #region 根据商品编号获取sku信息 by lijibo 20141003

        /// <summary>
        /// 根据商品编号获取sku信息 by lijibo 20141003
        /// </summary>
        /// <param name="productNos"></param>
        /// <returns></returns>
        public IList<SkuExtendPrice> OutletGetSkuListByProductNo(string productNos)
        {
            string[] productNosArray = productNos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return DapperUtil.Query<SkuExtendPrice>("ComBeziWfs_SpfSkuExtend_GetSkuListByProductNoOutlet", new { ProductNo = productNosArray }).ToList();
        }
        #endregion

        #region 根据商品编号获取sku信息活动关联 by lijibo 20141003

        /// <summary>
        /// 根据商品编号获取sku信息活动关联 by lijibo 20141003
        /// </summary>
        /// <param name="productNos"></param>
        /// <returns></returns>
        public IList<SkuExtendPrice> OutletGetSkuListByProductNoRef(string productNos)
        {
            string[] productNosArray = productNos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return DapperUtil.Query<SkuExtendPrice>("ComBeziWfs_SpfSkuExtend_GetSkuListByProductNoOutletRef", new { ProductNo = productNosArray }).ToList();
        }
        #endregion

        #region 商品可视化查询   by lijibo 20141003

        /// <summary> 
        /// 商品可视化查询   by lijibo 20141003
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<SubjectProductRef> OutletSelectSubjectProductRefList(string category)
        {
            return DapperUtil.Query<SubjectProductRef>("ComBeziWfs_SpfProductExtend_GetSubjectProductViewListOutlet", new { CategoryNo = category, ShowTime = DateTime.Now.ToString() }).ToList();
        }
        #endregion

        /// <summary>
        /// 判断商品是否存在  by lijibo 20141011
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public SpProduct OutletReadProduct(string productNo)
        {
            SpProduct product = DapperUtil.Query<SpProduct>("ComBeziWfs_SpProduct_GetIsExistenceListOutlet", new { ProductNo = productNo }).FirstOrDefault();
            return product;
        }

        #endregion

        #region 根据商品编号获取sku信息（目前sku价格取的是以市场价为最高的值） by lijibo 20141003

        /// <summary>
        /// 根据商品编号获取sku信息（目前sku价格取的是以市场价为最高的值） by lijibo 20141003
        /// </summary>
        /// <param name="productNos"></param>
        /// <returns></returns>
        public IList<SkuExtendPrice> OutletGetMarketPriceSkuListByProductNo(string productNos)
        {
            string[] productNosArray = productNos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return DapperUtil.Query<SkuExtendPrice>("ComBeziWfs_SpfSkuExtend_GetMarketPriceSkuListByProductNoOutlet", new { ProductNo = productNosArray }).ToList();
        }
        #endregion

        #region 活动控制台修改_EP by zhangman 20141015
        public IEnumerable<SubjectInfo> GetSubjectsMonitorNew_EP(SubjectMonitorSearchParm parm)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SubjectNameNo", (parm == null || parm.SubjectNameNo == "活动编号/名称" || string.IsNullOrEmpty(parm.SubjectNameNo)) ? "" : parm.SubjectNameNo);
            dic.Add("BrandNo", (string.IsNullOrWhiteSpace(parm.BrandNo) || parm.BrandNo == "品牌") ? "" : parm.BrandNo);
            dic.Add("ChannelSord", string.IsNullOrWhiteSpace(parm.ChannelSord) ? "" : parm.ChannelSord);
            dic.Add("BU", string.IsNullOrWhiteSpace(parm.BU) ? "" : parm.BU);

            string startDate = DateTime.Now.ToString("yyyy-MM-dd");
            var list = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubjectTodayOpenSubject_SelectList_EP", dic,
                new
                {
                    StartDate = startDate,
                    SubjectNameNo = parm.SubjectNameNo,
                    BrandNo = parm.BrandNo,
                    ChannelSord = parm.ChannelSord,
                    BU = parm.BU
                }).FilterList();

            //此处可优化！，通过js去显示余下的信息
            if (list == null)
            {
                return null;
            }
            else
            {
                //20131202添加 过滤类型为专题的活动
                //20140314修改 by lijia 去掉次过滤条件，活动可视化时读取常规活动和专题活动
                //list = list.Where(x => x.SubjectTemplate != 4).ToList();
                var totalCount = list.Count();
                return list;
            }
        }

        /// <summary>
        /// 奥莱控制台-进行中活动列表
        /// </summary>
        /// <param name="parm">查询参数</param>
        /// <returns></returns>
        public IEnumerable<SubjectInfo> GetSubjectMonitorRuning_EP(SubjectMonitorSearchParm parm)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SubjectNameNo", (parm == null || parm.SubjectNameNo == "活动编号/名称" || string.IsNullOrEmpty(parm.SubjectNameNo)) ? "" : parm.SubjectNameNo);
            dic.Add("BrandNo", (string.IsNullOrWhiteSpace(parm.BrandNo) || parm.BrandNo == "品牌") ? "" : parm.BrandNo);
            dic.Add("ChannelSord", string.IsNullOrWhiteSpace(parm.ChannelSord) ? "" : parm.ChannelSord);
            dic.Add("BU", string.IsNullOrWhiteSpace(parm.BU) ? "" : parm.BU);
            dic.Add("QueryStartTime", string.IsNullOrWhiteSpace(parm.QueryStartTime) ? "" : parm.QueryStartTime);
            dic.Add("QueryEndTime", string.IsNullOrWhiteSpace(parm.QueryEndTime) ? "" : parm.QueryEndTime);
            DateTime now = DateTime.Now;
            string startDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
            string endDate = DateTime.Now.AddHours(24).ToString("yyyy-MM-dd HH:mm:ss");
            IEnumerable<SubjectInfo> query = DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubjectMonitorRuning_SelectList_EP", dic,
                new
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    SubjectNameNo = parm.SubjectNameNo,
                    BrandNo = parm.BrandNo,
                    ChannelSord = parm.ChannelSord,
                    BU = parm.BU,
                    QueryStartTime = parm.QueryStartTime,
                    QueryEndTime = parm.QueryEndTime
                }).FilterList();
            if (query == null)
            {
                return null;
            }
            return query.ToList();
        }


        #endregion

        #region 活动关联商品价格及状态  by lijibo 20141003

        /// <summary>
        /// 活动关联商品价格及状态  by lijibo 20141003
        /// </summary>
        /// <param name="entitylist"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public IList<SubjectProductRef> TransformationEntityListRef(IList<SubjectProductRef> entitylist)
        {
            if (entitylist != null && entitylist.Count() > 0)
            {
                SWfsSubjectService service = new SWfsSubjectService();
                IList<SubjectProductRef> _entitylist = new List<SubjectProductRef>();
                string tempProductNos = string.Empty;
                foreach (SubjectProductRef prModel in entitylist)
                {
                    tempProductNos += prModel.ProductNo + ",";
                }
                IList<SkuExtendPrice> SkuList = service.OutletGetSkuListByProductNo(tempProductNos);
                if (SkuList != null && SkuList.Count() > 0)
                {
                    foreach (SubjectProductRef prModel in entitylist)
                    {
                        SubjectProductRef _prModel = new SubjectProductRef();
                        _prModel = prModel;
                        IEnumerable<SkuExtendPrice> _SkuList = SkuList.Where(c => c.ProductNo == prModel.ProductNo);
                        if (_SkuList != null && _SkuList.Count() > 0)
                        {
                            SkuExtendPrice _singleEntity = _SkuList.FirstOrDefault();
                            IList<short> isSelfList = _SkuList.Select(s => s.IsShelf).ToList();
                            if (isSelfList.Contains(2))//上架
                            {
                                SkuExtendPrice skuPrice = _SkuList.Where(s => s.IsShelf == 2).OrderBy(s => s.LimitedVipPrice).FirstOrDefault();
                                _prModel.IsShelf = 2;
                                _prModel.MarketPrice = skuPrice.MarketPrice;
                                _prModel.SellPrice = skuPrice.SellPrice;
                                _prModel.PlatinumPrice = skuPrice.PlatinumPrice;
                                _prModel.DiamondPrice = skuPrice.DiamondPrice;
                                _prModel.LimitedPrice = skuPrice.LimitedPrice;
                                _prModel.LimitedVipPrice = skuPrice.LimitedVipPrice;
                                _prModel.DisabledState = skuPrice.DisabledState;
                                _prModel.DiscountRate = skuPrice.DiscountRate;
                                _entitylist.Add(_prModel);

                            }
                            else if (isSelfList.Contains(3) && !isSelfList.Contains(2) && !isSelfList.Contains(1))//下架
                            {
                                SkuExtendPrice skuPrice = _SkuList.Where(s => s.IsShelf == 3).OrderBy(s => s.LimitedVipPrice).FirstOrDefault();
                                _prModel.IsShelf = 3;
                                _prModel.MarketPrice = skuPrice.MarketPrice;
                                _prModel.SellPrice = skuPrice.SellPrice;
                                _prModel.PlatinumPrice = skuPrice.PlatinumPrice;
                                _prModel.DiamondPrice = skuPrice.DiamondPrice;
                                _prModel.LimitedPrice = skuPrice.LimitedPrice;
                                _prModel.LimitedVipPrice = skuPrice.LimitedVipPrice;
                                _prModel.DisabledState = skuPrice.DisabledState;
                                _prModel.DiscountRate = skuPrice.DiscountRate;
                                _entitylist.Add(_prModel);
                            }
                            else if (isSelfList.Contains(1) && !isSelfList.Contains(2) && !isSelfList.Contains(3))
                            {
                                SkuExtendPrice skuPrice = _SkuList.Where(s => s.IsShelf == 1).OrderBy(s => s.LimitedVipPrice).FirstOrDefault();
                                _prModel.IsShelf = 1;
                                _prModel.MarketPrice = skuPrice.MarketPrice;
                                _prModel.SellPrice = skuPrice.SellPrice;
                                _prModel.PlatinumPrice = skuPrice.PlatinumPrice;
                                _prModel.DiamondPrice = skuPrice.DiamondPrice;
                                _prModel.LimitedPrice = skuPrice.LimitedPrice;
                                _prModel.LimitedVipPrice = skuPrice.LimitedVipPrice;
                                _prModel.DisabledState = skuPrice.DisabledState;
                                _prModel.DiscountRate = skuPrice.DiscountRate;
                                _entitylist.Add(_prModel);//未上架
                            }
                        }
                    }
                }
                else
                {
                    return entitylist;
                }
                return _entitylist;

            }
            else
            {
                return entitylist;
            }
        }
        #endregion
    }


    public class CharactersConvertTopinyin
    {
        private static int[] pyValue = new int[]
        {
        -20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,
        -20032,-20026,-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,
        -19756,-19751,-19746,-19741,-19739,-19728,-19725,-19715,-19540,-19531,-19525,-19515,
        -19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,-19261,-19249,
        -19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,
        -19003,-18996,-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,
        -18731,-18722,-18710,-18697,-18696,-18526,-18518,-18501,-18490,-18478,-18463,-18448,
        -18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183, -18181,-18012,
        -17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,
        -17733,-17730,-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,
        -17468,-17454,-17433,-17427,-17417,-17202,-17185,-16983,-16970,-16942,-16915,-16733,
        -16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,-16452,-16448,
        -16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,
        -16212,-16205,-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,
        -15933,-15920,-15915,-15903,-15889,-15878,-15707,-15701,-15681,-15667,-15661,-15659,
        -15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,-15408,-15394,
        -15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,
        -15149,-15144,-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,
        -14941,-14937,-14933,-14930,-14929,-14928,-14926,-14922,-14921,-14914,-14908,-14902,
        -14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,-14663,-14654,
        -14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,
        -14170,-14159,-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,
        -14109,-14099,-14097,-14094,-14092,-14090,-14087,-14083,-13917,-13914,-13910,-13907,
        -13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,-13611,-13601,
        -13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,
        -13340,-13329,-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,
        -13068,-13063,-13060,-12888,-12875,-12871,-12860,-12858,-12852,-12849,-12838,-12831,
        -12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,-12320,-12300,
        -12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,
        -11781,-11604,-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,
        -11055,-11052,-11045,-11041,-11038,-11024,-11020,-11019,-11018,-11014,-10838,-10832,
        -10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,-10329,-10328,
        -10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254
        };

        private static string[] pyName = new string[]
        {
        "A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben",
        "Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Ba","Cai","Can",
        "Cang","Cao","Ce","Ceng","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng",
        "Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong",
        "Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De",
        "Deng","Di","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui",
        "Dun","Duo","E","En","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fo",
        "Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gong",
        "Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han",
        "Hang","Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan",
        "Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing",
        "Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Ke",
        "Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo",
        "La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang",
        "Liao","Lie","Lin","Ling","Liu","Long","Lou","Lu","Lv","Luan","Lue","Lun",
        "Luo","Ma","Mai","Man","Mang","Mao","Me","Mei","Men","Meng","Mi","Mian",
        "Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Na","Nai","Nan","Nang",
        "Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning",
        "Niu","Nong","Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan",
        "Pang","Pao","Pei","Pen","Peng","Pi","Pian","Piao","Pie","Pin","Ping","Po",
        "Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu",
        "Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou",
        "Ru","Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen",
        "Seng","Sha","Shai","Shan","Shang","Shao","She","Shen","Sheng","Shi","Shou","Shu",
        "Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan",
        "Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian",
        "Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai",
        "Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao",
        "Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yan","Yang",
        "Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun",
        "Za", "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan",
        "Zhang","Zhao","Zhe","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan",
        "Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo"
        };

        /// <summary>
        /// 把汉字转换成拼音(全拼)
        /// </summary>
        /// <param name="hzString">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static string Convert(string hzString)
        {
            // 匹配中文字符
            Regex regex = new Regex("^[\u4e00-\u9fa5]$");
            byte[] array = new byte[2];
            string pyString = "";
            int chrAsc = 0;
            int i1 = 0;
            int i2 = 0;
            char[] noWChar = hzString.ToCharArray();

            for (int j = 0; j < noWChar.Length; j++)
            {
                // 中文字符
                if (regex.IsMatch(noWChar[j].ToString()))
                {
                    array = System.Text.Encoding.Default.GetBytes(noWChar[j].ToString());
                    i1 = (short)(array[0]);
                    i2 = (short)(array[1]);
                    chrAsc = i1 * 256 + i2 - 65536;
                    if (chrAsc > 0 && chrAsc < 160)
                    {
                        pyString += noWChar[j];
                    }
                    else
                    {
                        // 修正部分文字
                        if (chrAsc == -9254)  // 修正“圳”字
                        {
                            pyString += "Zhen";
                        }
                        else if (chrAsc == -6191)  // 修正“缪”字
                        {
                            pyString += "miu";
                        }
                        else if (chrAsc == -8542)  // 修正“蔻”字
                        {
                            pyString += "kou";
                        }
                        else if (chrAsc == -5901)  // 修正“梵”字
                        {
                            pyString += "fan";
                        }
                        else if (chrAsc == -6150)  // 修正“琥”字
                        {
                            pyString += "hu";
                        }
                        else if (chrAsc == -6169)  // 修正“珑”字
                        {
                            pyString += "long";
                        }
                        else if (chrAsc == -9797)  // 修正“倩”字
                        {
                            pyString += "qian";
                        }
                        else if (chrAsc == -9017)  // 修正“芮”字
                        {
                            pyString += "rui";
                        }
                        else if (chrAsc == -8527)  // 修正“薇”字
                        {
                            pyString += "wei";
                        }
                        else if (chrAsc == -6156)  // 修正“玺”字
                        {
                            pyString += "xi";
                        }
                        else if (chrAsc == -9004)  // 修正“茉”字
                        {
                            pyString += "mo";
                        }
                        else
                        {
                            for (int i = (pyValue.Length - 1); i >= 0; i--)
                            {
                                if (pyValue[i] <= chrAsc)
                                {
                                    pyString += pyName[i];
                                    break;
                                }
                            }
                        }
                    }
                }
                // 非中文字符
                else
                {
                    pyString += noWChar[j].ToString();
                }
            }
            return pyString;
        }
    }
}
