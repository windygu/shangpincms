using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.IO;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Framework.Common;
using System.Configuration;
using Shangpin.Ocs.Service;
using System.Text;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Common;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq.Expressions;
using Shangpin.Framework.Common.Cache;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    [OCSAuthorization]
    public partial class SubjectController : Controller
    {

        #region 活动列表
        /// <summary>
        /// 活动列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="gender"></param>
        /// <param name="channelNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult Index(string keyword, string productNo, string status, string channelSord, string channelNo, string startTime, string endTime, int pageindex = 1, string Template = "", string isaudited = "", string bu = "")
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            if (!string.IsNullOrEmpty(keyword))
                keyword = keyword.Trim();
            ViewBag.KeyWord = keyword;
            ViewBag.Status = status;
            ViewBag.ChannelSord = channelSord;
            ViewBag.ChannelNo = channelNo;
            ViewBag.StartTime = startTime;
            ViewBag.EndTime = endTime;
            ViewBag.ProductNo = productNo;
            ViewBag.Template = Template;
            ViewBag.ChannelSordList = GetChannelSordList(2);
            ViewBag.BU = bu;
            ViewBag.IsAudited = isaudited;
            RecordPage<SubjectInfo> list = subject.SelectSubjectList(keyword, productNo, status, channelSord, channelNo, startTime, endTime, pageindex, pageSize, Template, isaudited, bu);
            ViewBag.ChannelList = GetChannelList(list.Items.Select(x => x.SubjectNo).ToArray());
            ViewBag.SubjectCurrUrl = Request.Url.ToString();
            return View(list);
        }
        /// <summary>
        /// 获取所属频道
        /// </summary>
        /// <param name="SubjectNolist"></param>
        /// <returns></returns>
        private Dictionary<string, List<SWfsChannel>> GetChannelList(Array SubjectNolist)
        {
            var channelList = new SWfsSubjectService().GetChannelBySubjectList(SubjectNolist);
            return channelList;

        }
        /// <summary>
        /// 获取所属分类
        /// </summary>
        /// <returns></returns>
        public IList<SWfsChannelSord> GetChannelSordList(int siteNo)
        {
            var channelSordList = new SWfsSubjectService().GetChannelSordList(2);
            channelSordList.Add(new SWfsChannelSord() { SordName = "女士", SordNo = "0" });
            channelSordList.Add(new SWfsChannelSord() { SordName = "男士", SordNo = "1" });
            channelSordList.Add(new SWfsChannelSord() { SordName = "儿童", SordNo = "2" });
            return channelSordList.OrderBy(x => x.SordNo).ToList();
        }
        #endregion

        #region 商品列表
        /// <summary>
        /// 商品列表 by zhangwei 20140311 edit 
        /// </summary>
        /// <param name="SubjectNo">活动编号</param>
        /// <param name="CategoryNo">品类</param>
        /// <param name="SCategoryNo">活动分类编号</param>
        /// <param name="BrandNo">品牌编号</param>
        /// <param name="IsShelf">上架状态</param>
        /// <param name="ProductNameOrNo">关键词（商品编号/商品名称/货号）</param>
        /// <param name="isbatch">是否批量</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="IsLoad">是否加载</param>
        /// <param name="productNos">多个商品编号以逗号分隔</param>
        /// <returns></returns>
        public ActionResult ProductList(string SubjectNo, string CategoryNo, string SCategoryNo, string BrandNo, string IsShelf, string ProductNameOrNo, string BU, bool isbatch = false, int pageindex = 1, bool IsLoad = true, string productNos = "", string GenderStyle = "")
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            if (!string.IsNullOrEmpty(ProductNameOrNo))
            {
                ProductNameOrNo = ProductNameOrNo.Trim();
            }
            string categoryNo1 = string.Empty;
            string categoryNo2 = string.Empty;
            string categoryNo3 = string.Empty;
            string categoryNo4 = string.Empty;
            string brandName = string.Empty;
            string bu = string.Empty;
            StringBuilder sb = new StringBuilder();

            SubjectInfo smodel = subject.GetSubjectInfo(SubjectNo);
            IList<SWfsSubjectCategoryRef> categoryList = subject.GetErpCategoryListBySubjectNo(SubjectNo);
            //所有的分类
            ViewBag.AllFirstCategory = subject.SelectErpCategoryByParentNo("ROOT");

            ViewBag.Category2 = string.IsNullOrEmpty(categoryNo1) ? null : subject.SelectErpCategoryByParentNo(categoryNo1);
            ViewBag.Category3 = string.IsNullOrEmpty(categoryNo2) ? null : subject.SelectErpCategoryByParentNo(categoryNo2);
            ViewBag.Category4 = string.IsNullOrEmpty(categoryNo3) ? null : subject.SelectErpCategoryByParentNo(categoryNo3);
            ViewBag.DepartmentList = subject.GetDepartmentList();
            RecordPage<ProductInfo> productList = new RecordPage<ProductInfo>();
            if (!isbatch)
            {
                categoryNo1 = Request.QueryString["CategoryNo1"] ?? "";
                categoryNo2 = Request.QueryString["CategoryNo2"] ?? "";
                categoryNo3 = Request.QueryString["CategoryNo3"] ?? "";
                categoryNo4 = Request.QueryString["CategoryNo4"] ?? "";
                brandName = Request.QueryString["BrandName"] ?? "";
                bu = Request.QueryString["BU"] ?? "";
                if (!IsLoad)
                {
                    IList<string> categoryNoList = categoryList.Select(c => c.Category).ToList();
                    productList = subject.GetSWfsSubjectProduct(CategoryNo, categoryNoList, SubjectNo, BrandNo, IsShelf, ProductNameOrNo, pageindex, pageSize, GenderStyle);
                    if (productList != null && productList.Items.Count() > 0 && !string.IsNullOrEmpty(bu))
                    {
                        IEnumerable<ProductInfo> list = productList.Items.Where(l => l.DepartmentNo.Substring(0, 6).Equals(bu));
                        productList = PageConvertor.Convert(pageindex, pageSize, list);
                    }
                }
            }
            else
            {
                string cacheKey = string.Empty;
                cacheKey = String.Format("ProductListBatchAddSubjectNo{0}SCategoryNo{1}", SubjectNo, SCategoryNo);
                string[] ProductNoStr = !string.IsNullOrEmpty(productNos) ? System.Web.HttpUtility.UrlDecode(productNos).Replace("\r\n", ",").Replace("\n\r", ",").Replace("\n", ",").Replace("\r", ",").TrimEnd(',').Trim().Split(',') : null;
                if (ProductNoStr != null && ProductNoStr.Length > 0)
                {
                    RedisCacheProvider.Instance.Set<string[]>(cacheKey, ProductNoStr, TimeSpan.FromDays(1));
                }
                else
                {
                    ProductNoStr = RedisCacheProvider.Instance.Get<string[]>(cacheKey) != null ? RedisCacheProvider.Instance.Get<string[]>(cacheKey) : null;
                }
                if (ProductNoStr != null && ProductNoStr.Length > 0)
                {
                    for (int i = 0; i < ProductNoStr.Length; i++)
                    {
                        sb.Append(ProductNoStr[i]);
                        if (i < (ProductNoStr.Length - 1))
                        {
                            sb.Append(",");
                        }

                    }
                    productList = subject.GetSWfsSubjectProductNew(SubjectNo, ProductNoStr.ToArray(), pageindex, pageSize);
                }
            }
            ViewBag.CategoryNo = categoryNo1 ?? "";
            ViewBag.CategoryNo2 = categoryNo2 ?? "";
            ViewBag.CategoryNo3 = categoryNo3 ?? "";
            ViewBag.CategoryNo4 = categoryNo4 ?? "";
            ViewBag.IsShelf = IsShelf;
            ViewBag.ProductNameOrNo = ProductNameOrNo;
            ViewBag.BrandName = brandName;
            ViewBag.IsBatch = isbatch ? "1" : "0";
            ViewBag.BatchProductNo = sb.ToString();
            ViewBag.GenderStyle = GenderStyle;
            ViewBag.BU = bu ?? "";

            //该活动下的一级分类
            ViewBag.FirstCategory = categoryList;

            ViewBag.SubjectNo = SubjectNo;
            ViewBag.SCategoryNo = SCategoryNo;
            ViewBag.CategoryName = subject.GetSubjectCategoryModel(SCategoryNo).CategoryName;
            ViewBag.SubjectName = smodel.SubjectName;


            //优化处理
            List<string> productNolist = new List<string>();
            if (productList != null && productList.TotalItems > 0)
            {
                foreach (var item in productList.Items)
                {
                    productNolist.Add(item.ProductNo);
                }
                SWfsProductService service = new SWfsProductService();
                Dictionary<string, IList<SWfsSubjectCategoryII>> dicList = service.GetSubjectCategoryByProductNoII(productNolist.ToArray(), "1");
                ViewBag.DicList = dicList;
                Dictionary<string, string> productErpAgeDic = OutLetExtendService.GetErpProductAgeingMulter(productNolist);
                ViewBag.PErpAgeDic = productErpAgeDic;
            }
            return View(productList);
        }

        #endregion

        #region 修改活动状态
        /// <summary>
        /// 修改活动状态
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubjectStatusModify(string subjectNo, string ScategoryNo, string status)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            SWfsProductService proService = new SWfsProductService();
            List<string> msgList = new List<string>();
            SubjectInfo subject = service.GetSubjectInfo(subjectNo);
            IList<string> list = service.GetProductListBySubjectNo(subjectNo, "0");
            string tempStatue = string.Empty;
            bool isAllPic = true;
            bool isOk = true;
            int type = 0;
            if (status == "1")
            {
                if (list.Count() > 0)
                {
                    if (subject.SubjectPreStartTemplateType != 1)
                    {
                        #region 优惠码模板或优惠券模板
                        if (subject.HeadShowType == 1) //纯图片
                        {
                            #region 纯图片
                            if (subject.BelongsSubjectPic != "" && subject.TitlePic2 != "" && subject.BackgroundPic != "" && subject.IPhonePic != "")
                            {
                                tempStatue = status;
                            }
                            else
                            {
                                tempStatue = "0";
                                isAllPic = false;
                                isOk = false;
                                type = 1; //优惠码模板或优惠券模板图片不能为空

                                #region 错误信息
                                if (subject.SubjectTemplate != 4)
                                {
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("顶部头图为空");
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(subject.TitlePic1))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("专题头图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("客户端图为空");
                                    }
                                }
                                if (string.IsNullOrEmpty(subject.BackgroundPic))
                                {
                                    msgList.Add("优惠信息展示图为空");
                                }
                                if (string.IsNullOrEmpty(subject.IPhonePic))
                                {
                                    msgList.Add("移动端图为空");
                                }
                                #endregion

                            }
                            #endregion
                        }
                        else if (subject.HeadShowType == 2)  //代码
                        {
                            #region 代码
                            if (!string.IsNullOrEmpty(subject.HeadWebHtml) && !string.IsNullOrEmpty(subject.HeadMobileHtml))
                            {
                                if (subject.SubjectTemplate != 4)
                                {
                                    if (subject.BelongsSubjectPic != "" && subject.IPhonePic != "" && subject.BackgroundPic != "")
                                    {
                                        tempStatue = status;
                                    }
                                    else
                                    {
                                        tempStatue = "0";
                                        isAllPic = false;
                                        isOk = false;
                                        type = 7; //优惠码模板或优惠券模板专题图片不能为空

                                        #region 错误信息
                                        if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                        {
                                            msgList.Add("列表页图为空");
                                        }
                                        if (string.IsNullOrEmpty(subject.BackgroundPic))
                                        {
                                            msgList.Add("优惠信息展示图为空");
                                        }
                                        if (string.IsNullOrEmpty(subject.IPhonePic))
                                        {
                                            msgList.Add("移动端图为空");
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(subject.TitlePic1) && !string.IsNullOrEmpty(subject.TitlePic2) && !string.IsNullOrEmpty(subject.IPhonePic) && !string.IsNullOrEmpty(subject.BackgroundPic))
                                    {

                                        tempStatue = status;
                                    }
                                    else
                                    {
                                        tempStatue = "0";
                                        isAllPic = false;
                                        isOk = false;
                                        type = 8; //优惠码模板或优惠券模板普通活动图片不能为空

                                        #region 错误信息
                                        if (string.IsNullOrEmpty(subject.TitlePic1))
                                        {
                                            msgList.Add("列表图为空");
                                        }
                                        if (string.IsNullOrEmpty(subject.TitlePic2))
                                        {
                                            msgList.Add("客户端图为空");
                                        }
                                        if (string.IsNullOrEmpty(subject.IPhonePic))
                                        {
                                            msgList.Add("移动端图为空");
                                        }
                                        if (string.IsNullOrEmpty(subject.BackgroundPic))
                                        {
                                            msgList.Add("优惠信息展示图为空");
                                        }
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                tempStatue = "0";
                                isAllPic = false;
                                isOk = false;
                                type = 2; //优惠码模板或优惠券模板代码不能为空

                                #region 错误信息
                                if (subject.SubjectTemplate != 4)
                                {
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.IPhonePic))
                                    {
                                        msgList.Add("移动端图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.BackgroundPic))
                                    {
                                        msgList.Add("优惠信息展示图为空");
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(subject.TitlePic1))
                                    {
                                        msgList.Add("列表图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("客户端图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.IPhonePic))
                                    {
                                        msgList.Add("移动端图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.BackgroundPic))
                                    {
                                        msgList.Add("优惠信息展示图为空");
                                    }
                                }
                                if (string.IsNullOrEmpty(subject.HeadWebHtml))
                                {
                                    msgList.Add("头图Web端代码为空");
                                }
                                if (string.IsNullOrEmpty(subject.HeadMobileHtml))
                                {
                                    msgList.Add("头图移动端代码为空");
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region 纯图片
                            if (subject.BelongsSubjectPic != "" && subject.TitlePic2 != "" && subject.BackgroundPic != "" && subject.IPhonePic != "")
                            {
                                tempStatue = status;
                            }
                            else
                            {
                                tempStatue = "0";
                                isAllPic = false;
                                isOk = false;
                                type = 1; //优惠码模板或优惠券模板图片不能为空

                                #region 错误信息
                                if (subject.SubjectTemplate != 4)
                                {
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("顶部头图为空");
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(subject.TitlePic1))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("专题头图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("客户端图为空");
                                    }
                                }
                                if (string.IsNullOrEmpty(subject.BackgroundPic))
                                {
                                    msgList.Add("优惠信息展示图为空");
                                }
                                if (string.IsNullOrEmpty(subject.IPhonePic))
                                {
                                    msgList.Add("移动端图为空");
                                }
                                #endregion

                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region 买手推荐模板
                        if (subject.HeadShowType == 1) //纯图片
                        {
                            #region 纯图片
                            if (subject.BelongsSubjectPic != "" && subject.TitlePic2 != "" && subject.IPhonePic != "")
                            {
                                tempStatue = status;
                            }
                            else
                            {
                                tempStatue = "0";
                                isAllPic = false;
                                isOk = false;
                                type = 3; //买手推荐模板图片不能为空

                                #region 错误信息
                                if (subject.SubjectTemplate != 4)
                                {
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("顶部头图为空");
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(subject.TitlePic1))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("专题头图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("客户端图为空");
                                    }
                                }
                                if (string.IsNullOrEmpty(subject.IPhonePic))
                                {
                                    msgList.Add("移动端图为空");
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else if (subject.HeadShowType == 2)  //代码
                        {
                            #region 代码
                            if (!string.IsNullOrEmpty(subject.HeadWebHtml) && !string.IsNullOrEmpty(subject.HeadMobileHtml))
                            {
                                if (subject.SubjectTemplate != 4)
                                {
                                    if (subject.BelongsSubjectPic != "" && subject.IPhonePic != "")
                                    {
                                        tempStatue = status;
                                    }
                                    else
                                    {
                                        tempStatue = "0";
                                        isAllPic = false;
                                        isOk = false;
                                        type = 4; //买手推荐模板专题图片不能为空

                                        #region 错误信息
                                        if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                        {
                                            msgList.Add("列表页图为空");
                                        }
                                        if (string.IsNullOrEmpty(subject.IPhonePic))
                                        {
                                            msgList.Add("移动端图为空");
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(subject.TitlePic1) && !string.IsNullOrEmpty(subject.TitlePic2) && !string.IsNullOrEmpty(subject.IPhonePic))
                                    {

                                        tempStatue = status;
                                    }
                                    else
                                    {
                                        tempStatue = "0";
                                        isAllPic = false;
                                        isOk = false;
                                        type = 5; //买手推荐模板普通活动图片不能为空

                                        #region 错误信息
                                        if (string.IsNullOrEmpty(subject.TitlePic1))
                                        {
                                            msgList.Add("列表图为空");
                                        }
                                        if (string.IsNullOrEmpty(subject.TitlePic2))
                                        {
                                            msgList.Add("客户端图为空");
                                        }
                                        if (string.IsNullOrEmpty(subject.IPhonePic))
                                        {
                                            msgList.Add("移动端图为空");
                                        }
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                tempStatue = "0";
                                isAllPic = false;
                                isOk = false;
                                type = 6; //买手推荐模板代码不能为空

                                #region 错误信息
                                if (subject.SubjectTemplate != 4)
                                {
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.IPhonePic))
                                    {
                                        msgList.Add("移动端图为空");
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(subject.TitlePic1))
                                    {
                                        msgList.Add("列表图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("客户端图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.IPhonePic))
                                    {
                                        msgList.Add("移动端图为空");
                                    }
                                }
                                if (string.IsNullOrEmpty(subject.HeadWebHtml))
                                {
                                    msgList.Add("头图Web端代码为空");
                                }
                                if (string.IsNullOrEmpty(subject.HeadMobileHtml))
                                {
                                    msgList.Add("头图移动端代码为空");
                                }
                                #endregion


                            }
                            #endregion
                        }
                        else
                        {
                            #region 纯图片
                            if (subject.BelongsSubjectPic != "" && subject.TitlePic2 != "" && subject.IPhonePic != "")
                            {
                                tempStatue = status;
                            }
                            else
                            {
                                tempStatue = "0";
                                isAllPic = false;
                                isOk = false;
                                type = 3; //买手推荐模板图片不能为空

                                #region 错误信息
                                if (subject.SubjectTemplate != 4)
                                {
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("顶部头图为空");
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(subject.TitlePic1))
                                    {
                                        msgList.Add("列表页图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.BelongsSubjectPic))
                                    {
                                        msgList.Add("专题头图为空");
                                    }
                                    if (string.IsNullOrEmpty(subject.TitlePic2))
                                    {
                                        msgList.Add("客户端图为空");
                                    }
                                }
                                if (string.IsNullOrEmpty(subject.IPhonePic))
                                {
                                    msgList.Add("移动端图为空");
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
                else
                {
                    isOk = false;
                    return Json(new { result = "0", message = "此活动中没有符合前台网站售卖条件的商品，不能开启！" });
                }
            }
            try
            {
                if (isOk)
                {
                    service.SubjectStatusModify(subjectNo, tempStatue);

                    SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                    log.SubjectOrChannelNo = subjectNo;
                    log.TypeValue = 0;
                    log.DateOperator = DateTime.Now;
                    log.OperatorContent = "修改活动状态";
                    log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                    service.InsertSubjectOrChannelLog(log);
                }
                if (!isAllPic)
                {
                    return Json(new { result = "0", message = "由于此活动的图片上传不完全或者HTML代码没有填写，所以不能开启该活动！", errorList = msgList });
                }
                else
                {
                    return Json(new { result = "1", message = "活动状态修改成功！", errorList = msgList });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = "活动状态修改失败！", errorList = msgList });
            }
        }

        //批量关闭活动
        public ActionResult CloseSubject(string subjectNos, string status)
        {
            string[] subjectNo = subjectNos.Split(',');
            SWfsSubjectService service = new SWfsSubjectService();
            foreach (string no in subjectNo)
            {
                service.SubjectStatusModify(no, status);
            }
            return Json(new { result = "1", message = "活动状态修改成功！" });
        }

        #endregion

        #region 活动商品添加
        /*20131125 批量添加商品 提示重复信息*/
        /// <summary>
        /// 向活动中添加商品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSubjectProductRef(string subjectNo, string sCategoryNo, string productNoStr)
        {
            string str = productNoStr;
            string isBatch = Request["isbatch"] ?? "0";
            SWfsSubjectService subject = new SWfsSubjectService();
            SubjectInfo mode = subject.GetSubjectInfo(subjectNo);
            if (isBatch.Equals("1")) //批量添加 20130725byliulei
            {
                str = Request["productNos"];
                subjectNo = Request["subjectNo"];
                sCategoryNo = Request["SCategoryNo"];
            }
            List<string> noneList = new List<string>();//没查询到商品的
            List<string> existList = new List<string>();//已经在列表中存在的
            List<string> nonequantity = new List<string>();//没有库存的
            List<string> categoryList = new List<string>();//类型不正确的
            if (!string.IsNullOrEmpty(str))
            {
                IList<string> categoryNoList = subject.GetErpCategoryListBySubjectNo(subjectNo).Select(c => c.Category).ToList();
                string[] ProductNoStr = str.Replace("\r\n", ",").Replace("\n\r", ",").Replace("\n", ",").Replace("\r", ",").TrimEnd(',').Trim().Split(',');

                SpProduct pdr;
                SWfsProductService proService = new SWfsProductService();
                foreach (string pid in ProductNoStr)
                {
                    if (string.IsNullOrEmpty(pid.Trim())) continue;
                    int result = 1;
                    //查看该商品是否存在
                    //  pdr = subject.ReadProduct(pid.Trim());   之前的写法
                    pdr = subject.OutletReadProduct(pid.Trim());
                    if (pdr == null)
                    {
                        noneList.Add(pid.Trim());
                        continue;
                    }
                    else
                    {
                        if (!categoryNoList.Contains(pdr.CategoryNo.Substring(0, 3)))
                        {
                            result = 0;
                            categoryList.Add(pid);
                            continue;
                        }
                    }
                    //是否有库存
                    int quantity = proService.GetInventoryByProductNo(pid.Trim()).SumQuantity;
                    if (quantity == 0)
                    {
                        result = 0;
                        nonequantity.Add(pid);
                        continue;
                    }
                    //活动商品
                    IList<string> splist = subject.GetProductListBySubjectNo(subjectNo, "");
                    //查看该商品是否已添加到该活动                  
                    if (splist.Contains(pid.Trim()))
                    {
                        result = 0;
                        existList.Add(pid);
                        continue;
                    }
                    //添加商品到活动
                    if (result == 1)
                    {
                        SWfsSubjectProductRef spr = new SWfsSubjectProductRef();
                        spr.ProductNo = pid.Trim();
                        spr.DateCreate = DateTime.Now;
                        spr.CategoryNo = sCategoryNo;
                        spr.PropertyValue = 0;
                        spr.TopFlag = false;
                        spr.SortNo = 0;
                        spr.BuyType = 0;
                        spr.TypeFlag = 0;
                        //spr.ShowTime = DateTime.Now <= mode.DateBegin ? mode.DateBegin : DateTime.Now;
                        spr.ShowTime = DateTime.Now;//2013-9-18 临时调整
                        spr.IsShow = true;
                        DapperUtil.Insert<SWfsSubjectProductRef>(spr);
                    }
                }

                SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                SubjectInfo subj = subject.GetSubjectInfo(subjectNo);
                log.SubjectOrChannelNo = subjectNo;
                log.TypeValue = 0;
                log.DateOperator = DateTime.Now;
                log.OperatorContent = "向活动中添加商品";
                log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                subject.InsertSubjectOrChannelLog(log);

                #region 商品变动重新计算活动折扣数据
                int co = subject.ExecuteDiscountTypeValue(subjectNo, sCategoryNo);
                #endregion

            }
            ViewBag.BackProductUrl = Request["BackProductUrl"] ?? "";
            existList = existList != null && existList.Count > 0 ? existList : null;
            noneList = noneList != null && noneList.Count > 0 ? noneList : null;
            nonequantity = nonequantity != null && nonequantity.Count > 0 ? nonequantity : null;
            categoryList = categoryList != null && categoryList.Count > 0 ? categoryList : null;
            if (existList != null || noneList != null || nonequantity != null || categoryList != null)
            {
                StringBuilder sbContent = new StringBuilder();
                sbContent.Append("商品列表中已存在商品:" + (existList != null ? string.Join(",", existList) : string.Empty));
                sbContent.Append("</br>");
                sbContent.Append("未查询到信息的商品:" + (noneList != null ? string.Join(",", noneList) : string.Empty));
                sbContent.Append("</br>");
                sbContent.Append("没有库存的商品:" + (nonequantity != null ? string.Join(",", nonequantity) : string.Empty));
                sbContent.Append("</br>");
                sbContent.Append("不包含类型的商品:" + (categoryList != null ? string.Join(",", categoryList) : string.Empty));
                sbContent.Append("</br>");
                sbContent.Append("<a onclick=\"javascript:history.back();\">返回上一步</a>");
                return Content(sbContent.ToString());

            }
            else
            {
                return Redirect((Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "/outlet/subject/ProductList?SubjectNo=" + subjectNo + "&SCategoryNo=" + sCategoryNo);
            }
        }

        /// <summary>
        /// 向活动中添加商品 by zhangwei 20140311
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSubjectProductRefNew(string subjectNo, string sCategoryNo, string productNoStr, string batchProductNo = "")
        {
            string str = productNoStr;
            string isBatch = Request["isbatch"] ?? "0";
            SWfsSubjectService subject = new SWfsSubjectService();
            SubjectInfo mode = subject.GetSubjectInfo(subjectNo);
            List<string> noneList = new List<string>();//没查询到商品的
            List<string> existList = new List<string>();//已经在列表中存在的
            List<string> nonequantity = new List<string>();//没有库存的
            List<string> categoryList = new List<string>();//类型不正确的
            if (!string.IsNullOrEmpty(str))
            {
                IList<string> categoryNoList = subject.GetErpCategoryListBySubjectNo(subjectNo).Select(c => c.Category).ToList();
                string[] ProductNoStr = str.Replace("\r\n", ",").Replace("\n\r", ",").Replace("\n", ",").Replace("\r", ",").TrimEnd(',').Trim().Split(',');

                SpProduct pdr;
                SWfsProductService proService = new SWfsProductService();
                foreach (string pid in ProductNoStr)
                {
                    if (string.IsNullOrEmpty(pid.Trim())) continue;
                    int result = 1;
                    //查看该商品是否存在
                    // pdr = subject.ReadProduct(pid.Trim());
                    pdr = subject.OutletReadProduct(pid.Trim());
                    if (pdr == null)
                    {
                        noneList.Add(pid.Trim());
                        continue;
                    }
                    else
                    {
                        if (!categoryNoList.Contains(pdr.CategoryNo.Substring(0, 3)))
                        {
                            result = 0;
                            categoryList.Add(pid);
                            continue;
                        }
                    }
                    //是否有库存
                    int quantity = proService.GetInventoryByProductNo(pid.Trim()).SumQuantity;
                    if (quantity == 0)
                    {
                        result = 0;
                        nonequantity.Add(pid);
                        continue;
                    }
                    //活动商品
                    IList<string> splist = subject.GetProductListBySubjectNo(subjectNo, "");
                    //查看该商品是否已添加到该活动                  
                    if (splist.Contains(pid.Trim()))
                    {
                        result = 0;
                        existList.Add(pid);
                        continue;
                    }
                    //添加商品到活动
                    if (result == 1)
                    {
                        SWfsSubjectProductRef spr = new SWfsSubjectProductRef();
                        spr.ProductNo = pid.Trim();
                        spr.DateCreate = DateTime.Now;
                        spr.CategoryNo = sCategoryNo;
                        spr.PropertyValue = 0;
                        spr.TopFlag = false;
                        spr.SortNo = 0;
                        spr.BuyType = 0;
                        spr.TypeFlag = 0;
                        //spr.ShowTime = DateTime.Now <= mode.DateBegin ? mode.DateBegin : DateTime.Now;
                        spr.ShowTime = DateTime.Now;//2013-9-18 临时调整
                        spr.IsShow = true;
                        DapperUtil.Insert<SWfsSubjectProductRef>(spr);
                    }
                }

                SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                SubjectInfo subj = subject.GetSubjectInfo(subjectNo);
                log.SubjectOrChannelNo = subjectNo;
                log.TypeValue = 0;
                log.DateOperator = DateTime.Now;
                log.OperatorContent = "向活动中添加商品";
                log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                subject.InsertSubjectOrChannelLog(log);

                //#region 商品变动重新计算活动折扣数据   
                //int co = subject.ExecuteDiscountTypeValue(subjectNo, sCategoryNo);
                //#endregion

            }
            ViewBag.BackProductUrl = Request["BackProductUrl"] ?? "";
            existList = existList != null && existList.Count > 0 ? existList : null;
            noneList = noneList != null && noneList.Count > 0 ? noneList : null;
            nonequantity = nonequantity != null && nonequantity.Count > 0 ? nonequantity : null;
            categoryList = categoryList != null && categoryList.Count > 0 ? categoryList : null;
            if (existList != null || noneList != null || nonequantity != null || categoryList != null)
            {
                StringBuilder sbContent = new StringBuilder();
                sbContent.Append("商品列表中已存在商品:" + (existList != null ? string.Join(",", existList) : string.Empty));
                sbContent.Append("</br>");
                sbContent.Append("未查询到信息的商品:" + (noneList != null ? string.Join(",", noneList) : string.Empty));
                sbContent.Append("</br>");
                sbContent.Append("没有库存的商品:" + (nonequantity != null ? string.Join(",", nonequantity) : string.Empty));
                sbContent.Append("</br>");
                sbContent.Append("不包含类型的商品:" + (categoryList != null ? string.Join(",", categoryList) : string.Empty));
                sbContent.Append("</br>");
                sbContent.Append("<a onclick=\"javascript:history.back();\">返回上一步</a>");
                return Content(sbContent.ToString());

            }
            else
            {
                if (isBatch.Equals("1"))
                {
                    string joinStr = string.Empty;
                    string url = "/outlet/subject/OutletProductList?";
                    string postData = "isbatch=true&SubjectNo=" + subjectNo + "&SCategoryNo=" + sCategoryNo + "&productNos=" + System.Web.HttpUtility.UrlEncode(batchProductNo);
                    return Redirect(url + postData);
                }
                else
                {
                    return Redirect((Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "/outlet/subject/OutletProductList?SubjectNo=" + subjectNo + "&SCategoryNo=" + sCategoryNo);
                }
            }
        }
        #endregion

        #region 活动展现与创建
        public ActionResult Create()
        {
            SWfsSubject subject = new SWfsSubject();
            SWfsSubjectService service = new SWfsSubjectService();
            IList<SWfsChannelSord> channelSordList = service.GetChannelSordList(2);
            ViewBag.ChannelSordList = channelSordList;
            // IList<WfsErpCategory> erpCategoryList = service.GetERPCategoryListByParentNo("ROOT");

            // Edit   by lijibo  20141002
            IList<SpCategory> erpCategoryList = service.OutletSelectErpCategoryByParentNo("ROOT");
            ViewBag.ErpCategoryList = erpCategoryList;
            return View();
        }
        /*20131129优化 整理 修改活动创建
         专题信息合并到活动创建 添加专题类型
         */
        [HttpPost]
        public ActionResult CreateSubject()
        {
            SWfsSubject subject = new SWfsSubject();
            SWfsSubjectService service = new SWfsSubjectService();
            SWfsSubjectConsoleService consoleService = new SWfsSubjectConsoleService();

            #region 生成活动ID
            string subjectNo = (DateTime.Now.Year.ToString().Substring(3, 1) + "-" + DateTime.Now.ToString("MM-dd")).Replace("-", "");
            CommonService cs = new CommonService();
            string subjectId = cs.GetNextCounterId("SubjectNo").ToString("000");
            subjectNo += subjectId.Substring(subjectId.Length - 3, 3);

            #endregion

            #region 获取参数 不包括图片上传信息
            string subjectName = Request.Params["SubjectName"];
            string SubjectEnName = Request.Params["SubjectEnName"];
            string sordNos = Request.Params["SordNo"];
            string categoryNos = Request.Params["CategoryNo"];
            string channelNos = Request.Params["ChannelNo"];
            string status = Request.Params["Status"];
            string subjectPreStartTemplateType = Request.Params["SubjectPreStartTemplateType"];
            string dateBegin = Request.Params["DateBegin"];
            string end = Request.Params["SubjectDuration"].ToString();
            string bu = Request.Params["BU"];//所属BU
            decimal salesForecast = Convert.ToDecimal(Request.Params["SalesForecast"]);//销售额预估
            string isPreheat = Request.Params["radIsPreheat"]; //活动预热 0关闭 1开启
            string preheatTime = Request.Params["txtPreheatTime"]; //预热时间
            string preheatBodyID = Request.Params["hidPreheatBodyID"]; //预热页面ID
            string brandLogoType = Request.Params["BrandLogoType"] ?? "0"; //品牌LOGO
            string salesInfo = Request.Params["SalesInfo"] ?? ""; //促销信息
            string HeadShowType = Request.Params["HeadShowType"]; //活动头图模板
            string HeadWebHtml = HttpUtility.UrlDecode(Request.Params["HeadWebHtml"]); //Web代码
            string HeadMobileHtml = HttpUtility.UrlDecode(Request.Params["HeadMobileHtml"]); //Mobile代码
            HeadWebHtml = HttpUtility.HtmlEncode(HeadWebHtml);
            HeadMobileHtml = HttpUtility.HtmlEncode(HeadMobileHtml);
            string seoTitle = Request.Params["SEOTitle"] == "限制30汉字以内" ? "" : Request.Params["SEOTitle"];
            DateTime dateEnd = new DateTime();
            if (!string.IsNullOrEmpty(end) && end != "0")
            {
                dateEnd = Convert.ToDateTime(dateBegin).AddHours(Convert.ToInt32(end));
            }
            else
            {
                //dateEnd = DateTime.MaxValue;
                dateEnd = Convert.ToDateTime(dateBegin).AddHours(72); //默认持续72小时
            }
            string discountType = Request.Params["DiscountType"];
            string subjectTemplate = Request.Params["SubjectTemplate"];
            string iphoneText = Request.Params["IPhoneText"];
            #endregion

            #region 图片信息相关

            string imgerror = string.Empty;
            //列表页图
            //判断是否是专题   根据不同类型 获取不同配置信息
            string BelongsAppKey = subjectTemplate == "4" ? "ToppicFlapPic" : "BelongsSubjectPic";
            string BelongsMsg = subjectTemplate == "4" ? "专题头图" : "列表页图";
            string belongsForm = subjectTemplate == "4" ? "BelongsSubjectPicUp" : "BelongsSubjectPic";
            string BelongsSubjectPic = GetActiveImageName(belongsForm, BelongsAppKey, true, BelongsMsg, "Create", "", true, out imgerror);
            if (!string.IsNullOrEmpty(imgerror))
            {
                return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
            }
            //判断列表页图 20140314 by lijia
            string belongsPic = string.Empty;
            if (subjectTemplate == "4")
            {
                belongsPic = GetActiveImageName("TitlePic1", "BelongsSubjectPic", true, "列表页图", "Create", "", true, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
                }
            }
            //顶部头图  
            string FlapPicAppKey = subjectTemplate == "4" ? "ToppicWapPicFileNoNew" : (subjectTemplate == "1" ? "FlapPic2" : (subjectTemplate == "2" || subjectTemplate == "5" ? "FlapPic" : "FlapPic1"));
            string FlapPicMsg = subjectTemplate == "4" ? "移动客户端旧版图" : "顶部头图";
            string FlapPic = GetActiveImageName("FlapPic", FlapPicAppKey, false, FlapPicMsg, "Create", "", true, out imgerror);
            if (!string.IsNullOrEmpty(imgerror))
            {
                return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
            }
            //优惠信息展示图 
            string AdPicAppKey = "AdPic";
            string AdPic = string.Empty;
            if (subjectPreStartTemplateType != "1")
            {
                AdPic = GetActiveImageName("AdPic", AdPicAppKey, true, "优惠信息展示图", "Create", "", true, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
                }
            }
            //移动端图
            string IPhonePicAppKey = subjectTemplate == "4" ? "ToppicIPhonePicFileNo" : "IPhonePic";
            string IPhonePic = GetActiveImageName("IPhonePic", IPhonePicAppKey, true, "移动客户端图", "Create", "", true, out imgerror);
            if (!string.IsNullOrEmpty(imgerror))
            {
                return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
            }
            #endregion

            #region 选择纯图片 没有活动介绍
            if (subjectTemplate != "1")
            {
                string contentIntroduction = Request["ContentIntroduction"];
                subject.ContentIntroduction = contentIntroduction;
            }
            else
            {
                subject.ContentIntroduction = "";
            }
            #endregion

            #region 根据是否上传所有图片和状态 判断活动是否开启
            bool isAll = true;
            //if (!string.IsNullOrEmpty(BelongsSubjectPic) && !string.IsNullOrEmpty(FlapPic))
            //{
            //    subject.Status = subjectPreStartTemplateType != "1" && string.IsNullOrEmpty(AdPic) ? Convert.ToInt16(0) : Convert.ToInt16(status);
            //    isAll = subjectPreStartTemplateType != "1" && string.IsNullOrEmpty(AdPic) ? false : true;
            //}
            //else
            //{
            //    subject.Status = 0;
            //    isAll = false;
            //}
            #endregion

            #region 预热日期不可大于活动开始日期
            if (isPreheat.Equals("1"))
            {
                if (DateTime.Parse(preheatTime) >= DateTime.Parse(dateBegin))
                {
                    return Json(new { result = "-1", message = "预热日期不可大于活动开始日期！" });
                }
            }
            #endregion

            #region 模板信息
            if (subjectPreStartTemplateType == "2")
            {
                subject.PrivilegeValue = Request.Params["Code"].ToString();
            }
            else if (subjectPreStartTemplateType == "3")
            {
                subject.PrivilegeValue = Request.Params["Coupon"].ToString();
            }
            else
            {
                subject.PrivilegeValue = "";
            }
            #endregion

            #region 参数赋值
            subject.BelongsSubjectPic = BelongsSubjectPic;//20140319 by lijia 专题模式时，存储专题头图；其他模式时，存储列表页图
            subject.TitlePic2 = FlapPic;
            subject.BackgroundPic = AdPic;
            subject.IPhonePic = IPhonePic;
            subject.SubjectNo = subjectNo;
            subject.SubjectName = subjectName;
            subject.SubjectEnName = SubjectEnName;
            subject.ChannelNo = channelNos;
            subject.SubjectPreStartTemplateType = Convert.ToInt16(subjectPreStartTemplateType);
            subject.DateBegin = Convert.ToDateTime(dateBegin);
            subject.DateEnd = dateEnd;
            subject.DateCreate = DateTime.Now;
            subject.IPhoneText = iphoneText;
            subject.DiscountType = Convert.ToInt16(discountType);
            subject.SubjectTemplate = Convert.ToInt16(subjectTemplate);
            subject.CreateUserId = PresentationHelper.GetPassport().UserName;
            subject.SubjectTitle = seoTitle;//SEO-Title
            subject.LogoPic = "";
            subject.TitlePic1 = belongsPic;//20140319 by lijia 选择专题模式时，此字段存储列表页图片
            subject.BrandContent = Request.Form["BrandNo"].Trim();//2014-1-9增加 活动所关联的品牌 目前为一对一关系。
            subject.BookFlag = false;
            subject.SubjectType = 0;
            subject.TopFlag = false;
            subject.VideoLink = "";
            subject.VisitCount = 0;
            subject.MaxApply = 0;
            subject.ShowType = 0;
            subject.SubjectContent = "";
            subject.BrandSign = "";
            subject.SubjectFlag = "";
            subject.FlapPic2 = "";
            subject.IPhoneTitle = "";
            subject.EstimateSale = 0;
            subject.BelongsSubjectType = 0;
            subject.Gender = 0;
            subject.Discount = "";
            subject.SubjectBeginProductEnd = 0;
            subject.SubjectEndProductBegin = 0;
            subject.SubjectIntroduction = "";
            subject.EditWord = "";
            subject.SubjectLightspot = "";
            subject.WillBeginPic = "";
            subject.DetailsPageheadPic = "";
            subject.PromotionType = 0;
            subject.BrandUrl = "";
            subject.DiscountTypeValue = "0";
            subject.ContentIntorType = 0;
            subject.ContentRecommType = 0;
            subject.IsRelated = 0;
            subject.SpreadPicture = "";
            subject.SpreadStatus = 0;
            subject.AdPic = "";
            subject.Description = "";
            subject.FlapPic = "";
            subject.ContentRecommended = "";
            subject.ContentTitleOne = "";
            subject.ContentTitleTwo = "";
            subject.BU = bu;
            subject.SalesForecast = salesForecast;
            subject.IsPreheat = Convert.ToInt16(isPreheat);
            subject.IsAudited = 0;
            subject.AuditedUserId = PresentationHelper.GetPassport().UserName;
            subject.AuditedDateTime = DateTime.Parse("1900-01-01 00:00:00");
            subject.Status = 2; //默认为未开启
            subject.BrandLogoType = Int16.Parse(brandLogoType); //品牌LOGO
            subject.SalesInfo = salesInfo;  //促销信息
            subject.HeadShowType = !string.IsNullOrEmpty(HeadShowType) ? Int16.Parse(HeadShowType) : Int16.Parse("0");
            subject.HeadWebHtml = HeadWebHtml;
            subject.HeadMobileHtml = HeadMobileHtml;
            #endregion

            #region 根据开始时间7天内的活动结束时间是否有同品牌或同品类的活动
            if (!string.IsNullOrEmpty(subject.BrandContent) && !string.IsNullOrEmpty(categoryNos))
            {
                string fistTime = subject.DateBegin.AddDays(-7).ToString();

                #region 品类
                List<SWfsSubjectCategoryRef> categoryNoList = new List<SWfsSubjectCategoryRef>();
                string[] categoryNoArray = categoryNos.Split(',');
                foreach (string item in categoryNoArray)
                {
                    SWfsSubjectCategoryRef categoryM = new SWfsSubjectCategoryRef();
                    categoryM.Category = item;
                    categoryM.SubjectNo = subjectNo;
                    categoryNoList.Add(categoryM);
                }
                #endregion

                bool IsHaveBCDiff = consoleService.GetSubjectDiffBrandClassListData(subject.SubjectNo, subject.BrandContent, categoryNoList, fistTime, subject.DateBegin.ToString());
                if (IsHaveBCDiff)
                {
                    return Json(new { result = "-1", message = "结束时间7天内已经存在同品牌同分类的活动！" });
                }
            }
            #endregion

            try
            {
                #region 关联表创建
                service.InsertSubject(subject);
                //活动与分类管理
                if (!string.IsNullOrEmpty(sordNos))
                {
                    SWfsSubjectChannelSordRef sordRef = new SWfsSubjectChannelSordRef();
                    string[] sordNoList = sordNos.Split(',');
                    foreach (string sordNo in sordNoList)
                    {
                        sordRef.SordNo = sordNo;
                        sordRef.SubjectNo = subjectNo;
                        service.InsertSubjectChannelSordRef(sordRef);
                    }
                }
                //活动与品类关联添加
                if (!string.IsNullOrEmpty(categoryNos))
                {
                    SWfsSubjectCategoryRef categryref = new SWfsSubjectCategoryRef();
                    string[] categoryNosList = categoryNos.Split(',');
                    foreach (string categoryNo in categoryNosList)
                    {
                        categryref.Category = categoryNo;
                        categryref.SubjectNo = subjectNo;
                        service.InsertSubjectCategoryRef(categryref);
                    }
                }
                #endregion

                #region 预热开启时创建预热相关信息
                if (isPreheat.Equals("1"))
                {
                    SWfsSubjectTopExpand topExpandM = new SWfsSubjectTopExpand();
                    topExpandM.SubjectNo = subjectNo;
                    topExpandM.StExpand = preheatBodyID;
                    topExpandM.TopCreateTime = new DateTime(1900, 1, 1, 0, 0, 0);
                    topExpandM.PreheatTime = DateTime.Parse(preheatTime);
                    service.InsertSubjectTopExpand(topExpandM);
                }
                #endregion

                #region 日志
                SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                log.SubjectOrChannelNo = subjectNo;
                log.TypeValue = 0;
                log.DateOperator = DateTime.Now;
                log.OperatorContent = "创建";
                log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                service.InsertSubjectOrChannelLog(log);
                #endregion

                #region 创建默认分组
                SWfsSubjectCategory subjectcategory = new SWfsSubjectCategory();
                string subjectCategoryNo = DateTime.Now.ToString("yyyyMMdd");
                string str2 = cs.GetNextCounterId("CategoryNo").ToString("000");
                subjectCategoryNo = subjectCategoryNo + str2.Substring(str2.Length - 3, 3);
                subjectcategory.CategoryName = "系统创建";
                subjectcategory.CategoryNo = subjectCategoryNo;
                subjectcategory.AdPic = "";
                subjectcategory.BackgroundPic = "";
                subjectcategory.BuyType = 4;//默认商品4列显示
                subjectcategory.DateCreate = DateTime.Now;
                subjectcategory.LogoPic = "";
                subjectcategory.ShowErpCategory = true;
                subjectcategory.SortNo = 0;
                subjectcategory.SubjectNo = subjectNo;
                subjectcategory.IsAutoBottom = 1;
                subjectcategory.MobileIsAutoBottom = 1;
                service.InsertSubjectCategory(subjectcategory);
                return Json(new { result = "1", message = isAll ? "添加成功！" : "添加成功！由于您上传图片不完全，所以该活动默认为关闭状态！" }, "text/plain", Encoding.UTF8);
                #endregion

            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
        }
        #endregion

        #region 编辑活动
        [HttpGet]
        public ActionResult Edit(string subjectNo)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            Task<SubjectInfo> task1 = Task.Factory.StartNew(() => service.GetSubjectInfo(subjectNo));
            SubjectInfo model = task1.Result;
            if (model.IsPreheat == 1)
            {
                SubjectPreheatInfoM predheatM = service.GetSubjectPreheatInfo(subjectNo);
                ViewBag.SubjectPredheatModel = predheatM;
            }
            Task<IList<SWfsSubjectChannelSordRef>> task2 = Task.Factory.StartNew(() => service.GetSordBySubjectNo(subjectNo));
            ViewBag.SubjectChannelSordList = task2.Result;
            Task<IList<SWfsChannelSord>> task3 = Task.Factory.StartNew(() => service.GetChannelSordList(2));
            ViewBag.ChannelSordList = task3.Result;
            Task<IList<SWfsSubjectCategoryRef>> task4 = Task.Factory.StartNew(() => service.GetErpCategoryListBySubjectNo(subjectNo));
            ViewBag.SubjectErpCategoryList = task4.Result;
            Task<IList<SpCategory>> task5 = Task.Factory.StartNew(() => service.GetERPCategoryListByParentNo("ROOT"));
            ViewBag.ErpCategoryList = task5.Result;

            Task<SpBrand> task6 = task1.ContinueWith<SpBrand>(p => service.GetBrandModelByBrandNo(model.BrandContent));
            SpBrand brandModel = task6.Result;
            model.BrandCnName = (brandModel == null) ? "" : brandModel.BrandCnName;
            model.BrandEnName = (brandModel == null) ? "" : brandModel.BrandEnName;
            return View(model);
        }

        [HttpPost]
        public JsonResult EditSubject(FormCollection formCollection)
        {
            SubjectInfo subject = new SubjectInfo();
            SWfsSubjectService service = new SWfsSubjectService();
            string subjectNo = Request.Params["subjectNo"].ToString();
            subject = service.GetSubjectInfo(subjectNo);

            #region 获取参数信息
            string subjectName = Request.Params["SubjectName"];
            string subjectEnName = Request.Params["subjectEnName"];
            string sordNos = Request.Params["SordNo"];
            string categoryNos = Request.Params["CategoryNo"];
            string channelNos = Request.Params["ChannelNo"];
            string subjectPreStartTemplateType = Request.Params["SubjectPreStartTemplateType"].ToString();
            string status = Request.Params["Status"];
            string dateBegin = Request.Params["DateBegin"];
            string bu = Request.Params["BU"];
            //20140305 by lijia 持续时间不能更改 Start
            string end = Request.Params["SubjectDuration"].ToString();
            string isPreheat = Request.Params["radIsPreheat"]; //活动预热 0关闭 1开启
            string preheatTime = Request.Params["txtPreheatTime"]; //预热时间
            string preheatBodyID = Request.Params["hidPreheatBodyID"]; //预热页面ID
            string brandLogoType = Request.Params["BrandLogoType"] ?? "0"; //品牌LOGO
            string salesInfo = Request.Params["SalesInfo"]; //促销信息
            string HeadShowType = Request.Params["HeadShowType"]; //活动头图模板
            string HeadWebHtml = HttpUtility.UrlDecode(Request.Params["HeadWebHtml"]); //Web代码
            string HeadMobileHtml = HttpUtility.UrlDecode(Request.Params["HeadMobileHtml"]); //Mobile代码
            HeadWebHtml = HttpUtility.HtmlEncode(HeadWebHtml);
            HeadMobileHtml = HttpUtility.HtmlEncode(HeadMobileHtml);
            string subjectTitle = Request.Params["SEOTitle"] == "限制30汉字以内" ? "" : Request.Params["SEOTitle"];
            DateTime dateEnd = new DateTime();
            if (!string.IsNullOrEmpty(end) && end != "0")
            {
                dateEnd = Convert.ToDateTime(dateBegin).AddHours(Convert.ToInt32(end));
            }
            else
            {
                dateEnd = DateTime.MaxValue;
                dateEnd = Convert.ToDateTime(dateBegin).AddHours(72); //默认持续72小时
            }
            subject.DateBegin = Convert.ToDateTime(dateBegin);
            subject.DateEnd = dateEnd;
            //持续时间不能更改 End

            string discountType = Request.Params["DiscountType"];
            string subjectTemplate = Request.Params["SubjectTemplate"];
            string iphoneText = Request.Params["IPhoneText"];
            #endregion

            #region 图片信息相关
            string imgerror = string.Empty;
            //列表页图
            //判断是否是专题 BelongsSubjectPic  根据不同类型 获取不同配置信息
            string BelongsAppKey = subjectTemplate == "4" ? "ToppicFlapPic" : "BelongsSubjectPic";
            string BelongsMsg = subjectTemplate == "4" ? "专题头图" : "列表页图";
            string BelongsSubjectPic = GetActiveImageName("BelongsSubjectPic", BelongsAppKey, true, BelongsMsg, "Edit", subject.BelongsSubjectPic, true, out imgerror);
            if (!string.IsNullOrEmpty(imgerror))
            {
                return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
            }
            string belongsPic = string.Empty;
            if (subjectTemplate == "4")
            {
                belongsPic = GetActiveImageName("TitlePic1", "BelongsSubjectPic", true, "列表页图", "Edit", subject.TitlePic1, true, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
                }
            }
            //顶部头图   别忘记修改配置格式
            string FlapPicAppKey = subjectTemplate == "4" ? "ToppicWapPicFileNoNew" : (subjectTemplate == "1" ? "FlapPic2" : (subjectTemplate == "2" || subjectTemplate == "5" ? "FlapPic" : "FlapPic1"));
            string FlapPicMsg = subjectTemplate == "4" ? "移动客户端旧版图" : "顶部头图";
            string FlapPic = GetActiveImageName("FlapPic", FlapPicAppKey, false, FlapPicMsg, "Edit", subject.TitlePic2, true, out imgerror);
            if (!string.IsNullOrEmpty(imgerror))
            {
                return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
            }
            //优惠信息展示图 
            string AdPicAppKey = "AdPic";
            string AdPic = string.Empty;
            if (subjectPreStartTemplateType != "1")
            {
                AdPic = GetActiveImageName("AdPic", AdPicAppKey, true, "优惠信息展示图", "Edit", subject.BackgroundPic, true, out imgerror);
                if (!string.IsNullOrEmpty(imgerror))
                {
                    return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
                }
            }
            //移动端图
            string IPhonePicAppKey = subjectTemplate == "4" ? "ToppicIPhonePicFileNo" : "IPhonePic";
            string IPhonePic = GetActiveImageName("IPhonePic", IPhonePicAppKey, true, "移动客户端图", "Edit", subject.IPhonePic, true, out imgerror);
            if (!string.IsNullOrEmpty(imgerror))
            {
                return Json(new { result = "-1", message = imgerror }, "text/plain", Encoding.UTF8);
            }
            #endregion

            #region 预热日期不可大于活动开始日期
            if (isPreheat.Equals("1"))
            {
                if (DateTime.Parse(preheatTime) >= DateTime.Parse(dateBegin))
                {
                    return Json(new { result = "-1", message = "预热日期不可大于活动开始日期！" });
                }
            }
            #endregion

            #region 选择纯图片 没有活动介绍
            if (subjectTemplate != "1")
            {
                string contentIntroduction = Request.Params["ContentIntroduction"];
                subject.ContentIntroduction = contentIntroduction;
            }
            #endregion

            #region 模板信息
            if (subjectPreStartTemplateType == "2")
            {
                subject.PrivilegeValue = Request.Params["Code"].ToString();
            }
            else if (subjectPreStartTemplateType == "3")
            {
                subject.PrivilegeValue = Request.Params["Coupon"].ToString();
            }
            #endregion

            #region 参数赋值
            subject.BelongsSubjectPic = BelongsSubjectPic;//20140314 by lijia 选择专题头图模式时，存储专题头图，其他模式时，存储列表页图
            subject.TitlePic1 = belongsPic;//20140314 by lijia 选择专题头图模式时，存储列表页图
            subject.TitlePic2 = FlapPic;
            subject.BackgroundPic = AdPic;
            subject.IPhonePic = IPhonePic;
            subject.SubjectPreStartTemplateType = Convert.ToInt16(subjectPreStartTemplateType);
            subject.SubjectNo = subjectNo;
            subject.SubjectName = subjectName;
            subject.SubjectEnName = subjectEnName;
            subject.ChannelNo = channelNos;
            subject.BrandContent = Request.Form["BrandNo"].Trim();//2014-1-9增加 活动所关联的品牌 目前为一对一关系。
            subject.IPhoneText = iphoneText;
            subject.SubjectTemplate = Convert.ToInt16(subjectTemplate);
            subject.BU = bu;
            subject.IsPreheat = Convert.ToInt16(isPreheat);
            subject.BrandLogoType = Int16.Parse(brandLogoType); //品牌LOGO
            subject.SalesInfo = salesInfo;  //促销信息
            subject.HeadShowType = !string.IsNullOrEmpty(HeadShowType) ? Int16.Parse(HeadShowType) : Int16.Parse("0");
            subject.HeadWebHtml = HeadWebHtml;
            subject.HeadMobileHtml = HeadMobileHtml;
            subject.SubjectTitle = !string.IsNullOrWhiteSpace(subjectTitle) ? subjectTitle : "";//SEO-Title
            #endregion

            #region 判断活动状态 并且判断活动是否符合开启的条件
            bool isAll = true;
            //if (Convert.ToInt16(status) != subject.Status && status == "1")
            //{
            //    IList<string> list = service.GetProductListBySubjectNo(subjectNo, "0");
            //    if (list.Count() <= 0)
            //    {
            //        return Json(new { result = "0", message = "此活动中没有符合前台网站售卖条件的商品，不能开启！" });
            //    }
            //    if (!string.IsNullOrEmpty(BelongsSubjectPic) && !string.IsNullOrEmpty(FlapPic))
            //    {
            //        subject.Status = subjectPreStartTemplateType != "1" && string.IsNullOrEmpty(AdPic) ? Convert.ToInt16(0) : Convert.ToInt16(status);
            //        isAll = subjectPreStartTemplateType != "1" && string.IsNullOrEmpty(AdPic) ? false : true;
            //    }
            //    else
            //    {
            //        subject.Status = 0;
            //        isAll = false;
            //    }

            //}
            #endregion

            try
            {
                bool flag = service.UpdateSubject(subject);
                if (flag)
                {
                    #region 关联表编辑
                    if (!string.IsNullOrEmpty(sordNos))
                    {
                        IList<SWfsSubjectChannelSordRef> subjectchannelSordList = service.GetSordBySubjectNo(subjectNo);
                        string[] sordList = subjectchannelSordList.Select(s => s.SordNo).ToArray();
                        SWfsSubjectChannelSordRef sordRef = new SWfsSubjectChannelSordRef();
                        string[] sordNoList = sordNos.Split(',');
                        if (!sordList.SequenceEqual(sordNoList))
                        {
                            service.DeleteSubjectChannelSordRef(subjectNo);
                            foreach (string sordNo in sordNoList)
                            {
                                sordRef.SordNo = sordNo;
                                sordRef.SubjectNo = subjectNo;
                                service.InsertSubjectChannelSordRef(sordRef);
                            }
                        }
                    }
                    //活动与品类关联添加
                    if (!string.IsNullOrEmpty(categoryNos))
                    {
                        IList<SWfsSubjectCategoryRef> categoryList = service.GetErpCategoryListBySubjectNo(subjectNo);
                        string[] clist = categoryList.Select(c => c.Category).ToArray();
                        SWfsSubjectCategoryRef categryref = new SWfsSubjectCategoryRef();
                        string[] categoryNosList = categoryNos.Split(',');
                        if (!clist.SequenceEqual(categoryNosList))
                        {
                            service.DeleteSubjectCategoryRef(subjectNo);
                            foreach (string categoryNo in categoryNosList)
                            {
                                categryref.Category = categoryNo;
                                categryref.SubjectNo = subjectNo;
                                service.InsertSubjectCategoryRef(categryref);
                            }
                        }
                    }
                    #endregion

                    #region 预热开启时编辑预热相关信息
                    if (isPreheat.Equals("1"))
                    {
                        SWfsSubjectTopExpand topExpandM = new SWfsSubjectTopExpand();
                        topExpandM.SubjectNo = subjectNo;
                        topExpandM.StExpand = preheatBodyID;
                        topExpandM.TopCreateTime = new DateTime(1900, 1, 1, 0, 0, 0);
                        topExpandM.PreheatTime = DateTime.Parse(preheatTime);
                        if (service.GetSubjectPreheatInfo(subjectNo) == null)
                        {
                            service.InsertSubjectTopExpand(topExpandM);
                        }
                        else
                        {
                            service.UpdateSubjectPreheat(topExpandM);
                        }
                    }
                    #endregion

                    #region 日志信息
                    SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                    log.SubjectOrChannelNo = subjectNo;
                    log.TypeValue = 0;
                    log.DateOperator = DateTime.Now;
                    log.OperatorContent = "修改";
                    log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                    service.InsertSubjectOrChannelLog(log);
                    #endregion

                    string msg = string.Empty;
                    msg = isAll == true ? "修改成功！" : "修改成功！由于您上传图片不完全，所以该活动默认为关闭状态！";
                    return Json(new { result = "1", message = msg }, "text/plain", Encoding.UTF8);
                }
                else
                {
                    return Json(new { result = "0", message = "修改失败！" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
        }

        #endregion

        #region 活动分类管理
        /// <summary>
        /// 活动分类管理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="datecreate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="siteNo"></param>
        /// <returns></returns>
        public ActionResult SordManage(string name, string datecreate, int pageIndex = 1, int siteNo = 2)
        {
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            IList<SWfsChannelSord> list = new SWfsSubjectService().GetSWfsChannelSordList(name, datecreate, 2);
            ViewBag.Count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.List = list;
            ViewBag.Name = name;
            ViewBag.Time = datecreate;
            return View();

        }

        /// <summary>
        /// 更新分类
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateSord()
        {
            //0 分类名称不能为空 1 正常 2 分类名称重复 3保存失败
            string sordNo = (null == Request.Form["SordNo"]) ? string.Empty : Request.Form["SordNo"].ToString().Trim();
            string sordName = (null == Request.Form["SordName"]) ? string.Empty : Request.Form["SordName"].ToString().Trim();
            if (string.IsNullOrEmpty(sordNo) || string.IsNullOrEmpty(sordName))
            {
                return Json(new { rs = 0, msg = "分类名称不能为空" });
            }
            string msg = string.Empty;
            int b = new SWfsSubjectService().UpdateChannelSordBySordNo(sordNo, sordName, out msg);
            return Json(new { rs = b, msg = msg });

        }

        /// <summary>
        /// 增加分类
        /// </summary>
        /// <returns></returns>
        public JsonResult SordHandler()
        {
            string action = Request.Form["type"].ToString();
            if (action.Equals("add"))
            {
                string sordName = (null == Request.Form["SordName"]) ? string.Empty : Request.Form["SordName"].ToString().Trim();
                if (string.IsNullOrEmpty(sordName))
                {
                    return Json(new { rs = 0, msg = "分类名称不能为空" });
                }

                string msg = string.Empty;
                int b = new SWfsSubjectService().AddSord(sordName, out msg);
                return Json(new { rs = b, msg = msg });
            }
            if (action.Equals("del"))
            {
                string sordId = Request.Form["SordID"].ToString();
                int b = new SWfsSubjectService().DelSord(sordId);
                return Json(new { rs = b, msg = (b == 1) ? "" : "操作失败" });
            }
            return Json(new { rs = 1, msg = "" });
        }
        #endregion

        #region 管理活动中商品
        //管理活动中的商品 
        public ActionResult SubjectProductRef(string SubjectNo, string CategoryNo, string SCategoryNo, string BrandNo, string IsShelf, string ProductNameOrNo, string Quantity, string BU, int pageindex = 1, string genderStyle = "")
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            //2014-4-4如果有此参数表示是网络推广查看添加商品所用，不能操作选项切记，切记
            ViewBag.looktype = 0;
            bool IsRead = false;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["looktype"]) && Request.QueryString["looktype"].Equals("read"))
            {
                ViewBag.looktype = 1;
                IsRead = true;
            }

            //?CategoryNo=A01&SubjectNo=40114127&SCategoryNo=20140114408&CategoryNo1=A01&CategoryNo2=&CategoryNo3=&CategoryNo4=&BrandName=&BrandNo=&BrandNoo=&IsShelf=0&ProductNameOrNo=&Quantity=1
            SWfsSubjectService subject = new SWfsSubjectService();
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            //int pageSize = 2;
            ViewBag.PageIndex = pageindex;
            ViewBag.PageSize = pageSize;

            ViewBag.CategoryNo = CategoryNo;
            ViewBag.SubjectNo = SubjectNo;
            if (!ProductNameOrNo.IsNullOrEmpty())
            {
                ProductNameOrNo = ProductNameOrNo.Trim();
            }
            string categoryNo1 = Request.QueryString["CategoryNo1"];
            string categoryNo2 = Request.QueryString["CategoryNo2"];
            string categoryNo3 = Request.QueryString["CategoryNo3"];
            string categoryNo4 = Request.QueryString["CategoryNo4"];
            string bu = Request.QueryString["BU"];

            ViewBag.CategoryNo1 = categoryNo1 ?? "";
            ViewBag.CategoryNo2 = categoryNo2 ?? "";
            ViewBag.CategoryNo3 = categoryNo3 ?? "";
            ViewBag.CategoryNo4 = categoryNo4 ?? "";
            ViewBag.BU = bu ?? "";
            ViewBag.DepartmentList = subject.GetDepartmentList();
            ViewBag.IsShelf = IsShelf;
            ViewBag.ProductNameOrNo = ProductNameOrNo;
            ViewBag.BrandName = Request.QueryString["BrandName"];

            SubjectInfo smodel = subject.GetSubjectInfo(SubjectNo);
            ViewBag.FirstCategory = subject.GetErpCategoryListBySubjectNo(SubjectNo);
            ViewBag.AllFirstCategory = subject.SelectErpCategoryByParentNo("ROOT");

            ViewBag.Category2 = string.IsNullOrEmpty(categoryNo1) ? null : subject.SelectErpCategoryByParentNo(categoryNo1);
            ViewBag.Category3 = string.IsNullOrEmpty(categoryNo2) ? null : subject.SelectErpCategoryByParentNo(categoryNo2);
            ViewBag.Category4 = string.IsNullOrEmpty(categoryNo3) ? null : subject.SelectErpCategoryByParentNo(categoryNo3);

            ViewBag.SubjectNo = SubjectNo;
            if (!IsRead)
            {
                ViewBag.SCategoryNo = SCategoryNo;
            }
            if (!IsRead)
            {
                ViewBag.CategoryName = subject.GetSubjectCategoryModel(SCategoryNo).CategoryName;
            }
            ViewBag.Quantity = Quantity;
            ViewBag.GenderStyle = genderStyle; //性别
#if DEBUG
            sw.Stop();
            long t1 = sw.ElapsedMilliseconds;
            sw.Reset();
#endif

            IList<SubjectProductRef> productList;
            if (IsRead)
            {
                IList<SWfsSubjectCategory> templist = subject.GetSubjectCategoryList(SubjectNo);
                productList = subject.SelectSubjectProductRefListRead(CategoryNo, templist.Select(r => r.CategoryNo).ToList(), BrandNo, IsShelf, ProductNameOrNo, genderStyle);
            }
            else
            {
                productList = subject.SelectSubjectProductRefListII(CategoryNo, SCategoryNo, BrandNo, IsShelf, ProductNameOrNo, genderStyle);
            }

            if (productList != null && productList.Count > 0 && !string.IsNullOrEmpty(bu))
            {
                productList = productList.Where(l => l.DepartmentNo.Substring(0, 6).Equals(bu)).ToList();
            }


            #region 针对分组商品排序
            //SWfsSubjectService SubjectService = new SWfsSubjectService();
            //// IList<SubjectProductRef> 

            //IList<SWfsSubjectProductSort> sortList = SubjectService.GetProductSortList(SCategoryNo);
            //List<SubjectProductRef> tmplList = productList.ToList();
            //if (sortList.Count > 0)
            //{
            //    productList = (from l in productList
            //                   join s in sortList on l.ProductNo equals s.ProductNo
            //                   orderby s.Sort ascending
            //                   select l).ToList();
            //}
            //if (productList.Count < tmplList.Count)
            //{
            //    tmplList = (from t in tmplList
            //                where !(from b in productList select b.ProductNo).Contains(t.ProductNo)
            //                select t).ToList();

            //    if (tmplList != null && tmplList.Count() > 0)
            //    {
            //        foreach (var item in tmplList)
            //        {
            //            productList.Add(item);
            //        }
            //    }
            //}
            #endregion

            List<string> productNolist = new List<string>();
            IList<SubjectProductRef> result = new List<SubjectProductRef>();
            SWfsProductService service = new SWfsProductService();
            List<ProductInventory> PIEntityList = new List<ProductInventory>();
            //List<ProductInventory> productInventoryList= service.GetInventoryByProductNos(productNolist);

            if (Quantity == "1") //查询有库存的商品
            {
                foreach (SubjectProductRef sp in productList)
                {
                    productNolist.Add(sp.ProductNo);
                    ProductInventory pinventory = service.GetInventoryByProductNo(sp.ProductNo);
                    if (pinventory.SumQuantity > 0)
                    {
                        result.Add(sp);
                    }
                    PIEntityList.Add(pinventory);
                }
            }
            else if (Quantity == "0") //查询无库存的商品
            {
                foreach (SubjectProductRef sp in productList)
                {
                    productNolist.Add(sp.ProductNo);
                    ProductInventory pinventory = service.GetInventoryByProductNo(sp.ProductNo);
                    if (pinventory.SumQuantity == 0)
                    {
                        result.Add(sp);
                    }
                    PIEntityList.Add(pinventory);
                }
            }
            else
            {
                foreach (SubjectProductRef sp in productList)
                {
                    productNolist.Add(sp.ProductNo);
                    result.Add(sp);

                    ProductInventory pinventory = service.GetInventoryByProductNo(sp.ProductNo);
                    PIEntityList.Add(pinventory);
                }
            }
            ViewBag.ProductInventoryList = PIEntityList;
            ViewBag.TotalCount = result.Count();
            productList = result.Skip((pageindex - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.SubjectName = smodel.SubjectName;
            //ViewBag.BackSubjectUrl = Request.QueryString["BackSubjectUrl"].ToString();
            //ViewBag.BackCategoryUrl = Request.QueryString["BackCategoryUrl"].ToString();
#if DEBUG
            sw.Stop();
            long t2 = sw.ElapsedMilliseconds;
            sw.Reset();
#endif
            //优化处理

            if (productList != null && productList.Count > 0)
            {
                Task<Dictionary<string, IList<SWfsSubjectCategoryII>>> task3 = Task.Factory.StartNew(() => service.GetSubjectCategoryByProductNoII(productNolist.ToArray(), "1"));
                ViewBag.DicList = task3.Result;
                //Dictionary<string, IList<SWfsSubjectCategoryII>> dicList = service.GetSubjectCategoryByProductNoII(productNolist.ToArray(), "1");

                Task<Dictionary<string, string>> task4 = Task.Factory.StartNew(() => OutLetExtendService.GetErpProductAgeingMulter(productNolist));
                ViewBag.PErpAgeDic = task4.Result;
            }
#if DEBUG
            sw.Stop();
            long t3 = sw.ElapsedMilliseconds;
            sw.Reset();
#endif
            return View(productList);
        }


        /// <summary>
        /// 删除活动中的商品
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSubjectProductRef(string categoryNo, string relationId)
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            subject.DeleteSubjectProductRef(relationId);

            string subjectNo = Request.Params["SubjectNo"];
            SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
            log.SubjectOrChannelNo = subjectNo;
            log.TypeValue = 0;
            log.DateOperator = DateTime.Now;
            log.OperatorContent = "删除活动中的商品";
            log.OperatorUserId = PresentationHelper.GetPassport().UserName;
            subject.InsertSubjectOrChannelLog(log);

            #region 商品变动重新计算活动折扣数据
            int pco = subject.ExecuteDiscountTypeValue(subjectNo, categoryNo);
            #endregion

            string url = Request["CurUrl"];
            return Redirect(url + "&pcount=" + pco + CommonService.GetTimeStamp("&"));
        }

        /// <summary>
        /// 批量删除活动中的商品
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="relationIds"></param>
        /// <returns></returns>
        public ActionResult DeleteSubjectProductRefs(string categoryNo, string relationIds)
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            if (!string.IsNullOrEmpty(relationIds))
            {
                string[] res = relationIds.Split(',');
                foreach (var reid in res)
                {
                    subject.DeleteSubjectProductRef(reid);
                }
            }

            string subjectNo = Request.Params["SubjectNo"];
            SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
            log.SubjectOrChannelNo = subjectNo;
            log.TypeValue = 0;
            log.DateOperator = DateTime.Now;
            log.OperatorContent = "删除活动中的商品";
            log.OperatorUserId = PresentationHelper.GetPassport().UserName;
            subject.InsertSubjectOrChannelLog(log);
            #region 商品变动重新计算活动折扣数据
            int pcount = subject.ExecuteDiscountTypeValue(subjectNo, categoryNo);
            #endregion
            string url = Request["CurUrl1"];
            return Redirect(url + "&pcount=" + pcount + CommonService.GetTimeStamp("&"));
        }

        //更改商品是否在活动中显示
        public ActionResult UpdateProductShow(int subjectproductrefId, bool isShow)
        {
            SWfsProductService service = new SWfsProductService();
            SWfsSubjectProductRef model = service.GetSubjectProduct(subjectproductrefId);
            model.IsShow = isShow;
            bool flag = DapperUtil.Update(model);

            if (flag)
            {
                #region 商品变动重新计算活动折扣数据
                new SWfsSubjectService().ExecuteDiscountTypeValue(Request["subjectNo"], "");
                #endregion
                return Json(new { result = 1, message = "保存成功！" });
            }
            else
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
        }
        #endregion

        #region Ajax操作
        //商品标签
        public ActionResult AjaxSaveIsStarProduct(int refId, int isStartProduct)
        {
            SWfsProductService service = new SWfsProductService();
            SWfsSubjectProductRef model = service.GetSubjectProduct(refId);
            model.IsStarProduct = Convert.ToInt16(isStartProduct);
            bool flag = DapperUtil.Update(model);
            if (flag)
            {
                return Json(new { result = 1, message = "保存成功！" });
            }
            else
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
        }
        //更改显示时间
        public ActionResult AjaxSaveShowTime(int refId, string showTime, string subjectNo)
        {
            SWfsProductService service = new SWfsProductService();
            SWfsSubjectService subject = new SWfsSubjectService();
            DateTime dt = Convert.ToDateTime(showTime);
            SubjectInfo info = subject.GetSubjectInfo(subjectNo);
            if (dt < info.DateBegin)
            {
                return Json(new { result = 0, message = "显示时间不能小于活动开始时间！" });
            }
            else if (dt > info.DateEnd)
            {
                return Json(new { result = 0, message = "显示时间不能大于活动结束时间！" });
            }
            SWfsSubjectProductRef model = service.GetSubjectProduct(refId);
            model.ShowTime = dt;
            bool flag = DapperUtil.Update(model);
            if (flag)
            {
                #region 商品变动重新计算活动折扣数据
                new SWfsSubjectService().ExecuteDiscountTypeValue(Request["subjectNo"], "");
                #endregion

                return Json(new { result = 1, message = "保存成功！" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
        }
        public ActionResult AjaxCategory(string parentNo, string categoryType)
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            ViewBag.CategoryType = categoryType;
            ViewBag.CategoryNo = parentNo;
            ViewBag.ErpCategory = subject.SelectErpCategoryByParentNo(parentNo);
            return View();
        }


        public ActionResult AjaxBrand(string selectType, string categoryno)
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            Task<List<BrandInfo>> task1 = Task.Factory.StartNew(() => subject.getBrandGroup(selectType, categoryno));
            ViewBag.Brands = task1.Result;
            Task<IList<BrandInfo>> task2 = Task.Factory.StartNew(() => subject.getBrandGroupFristLetter(selectType, categoryno));
            ViewBag.BrandFristLetter = task2.Result;


            //ViewBag.Brands = subject.getBrandGroup(selectType, categoryno);
            //ViewBag.BrandFristLetter = subject.getBrandGroupFristLetter(selectType, categoryno);
            return View();

        }
        public ActionResult AjaxBrandNew(string selectType, string categoryno)
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            Task<List<BrandInfo>> task1 = Task.Factory.StartNew(() => subject.getBrandGroup(selectType, categoryno));
            ViewBag.Brands = task1.Result;
            Task<IList<BrandInfo>> task2 = Task.Factory.StartNew(() => subject.getBrandGroupFristLetter(selectType, categoryno));
            ViewBag.BrandFristLetter = task2.Result;


            //ViewBag.Brands = subject.getBrandGroup(selectType, categoryno);
            //ViewBag.BrandFristLetter = subject.getBrandGroupFristLetter(selectType, categoryno);
            return View();

        }
        //保存商品排序
        public ActionResult AjaxSaveProductSort(string subjectNo, string productNos, string sortType, string showType, string IsAutoBottom)
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            int flag = 0;
            if (showType == "1")
                flag = subject.DeleteProductSortBySubjectNONoNUll(subjectNo, showType);
            else
                flag = subject.DeleteProductSortBySubjectNO(subjectNo, showType);
            if (flag < 0)
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
            else
            {
                string[] proNoslist = productNos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                SWfsSubjectCategory CategoryEntity = subject.GetSubjectCategoryModel(subjectNo, true);
                if (showType == "0")
                {
                    CategoryEntity.SortRuleType = sortType;
                    CategoryEntity.IsAutoBottom = short.Parse(IsAutoBottom);
                }
                else
                {
                    CategoryEntity.MobileSortRuleType = sortType;
                    CategoryEntity.MobileIsAutoBottom = short.Parse(IsAutoBottom);
                }
                if (!subject.UpdateSubjectCategory(CategoryEntity))
                {
                    return Json(new { result = 0, message = "分组修改保存失败！" });
                }
                for (int i = 0; i < proNoslist.Count(); i++)
                {
                    SWfsSubjectProductSort prosort = new SWfsSubjectProductSort();
                    prosort.SubjectNo = subjectNo;
                    prosort.ProductNo = proNoslist[i];
                    prosort.Sort = i;
                    prosort.ShowChannelType = short.Parse(showType);
                    try
                    {
                        subject.InsertProductSort(prosort);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { result = 0, message = ex.Message });
                    }
                }
                SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                log.SubjectOrChannelNo = subjectNo;
                log.TypeValue = 0;
                log.DateOperator = DateTime.Now;
                log.OperatorContent = "修改活动中商品排序";
                log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                subject.InsertSubjectOrChannelLog(log);
                return Json(new { result = 1, message = "保存成功！" });
            }
        }

        //保存活动排序
        /// <summary>
        /// 保存活动排序 by zhangwei edit 20140618
        /// </summary>
        /// <param name="subjectNos">活动编号</param>
        /// <param name="channelNo">频道编号</param>
        /// <param name="type">类型 1今日新开，2正在进行，3即将结束</param>
        /// <param name="showType">展示渠道</param>
        /// <param name="showDateTime">预期排序日期</param>
        /// <returns></returns>
        public ActionResult AjaxSaveSubjectSort(string subjectNos, string channelNo, short type, string showType, string showDateTime)
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            int temp = subject.DeleteSubjectSortBySubjectNO(channelNo, type, showType, showDateTime);
            if (temp < 0)
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
            else
            {
                string[] subjectNoslist = subjectNos.Split(',');
                for (int i = 0; i < subjectNoslist.Count(); i++)
                {
                    SWfsSubjectSort sort = new SWfsSubjectSort();
                    sort.SubjectNo = subjectNoslist[i];
                    sort.ChannelNo = channelNo;
                    sort.Type = type;
                    sort.Sort = i;
                    sort.ShowType = !string.IsNullOrEmpty(showType) ? Int32.Parse(showType) : 0; //1网站，2手机端
                    sort.ShowDateTime = !string.IsNullOrEmpty(showDateTime) ? DateTime.Parse(showDateTime) : DateTime.Parse("1900-01-01 00:00:00");
                    try
                    {
                        subject.InsertSubjectSort(sort);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { result = 0, message = ex.Message });
                    }
                }
                SWfsSubjectOrChannelLog log = new SWfsSubjectOrChannelLog();
                log.SubjectOrChannelNo = channelNo;
                log.TypeValue = 0;
                log.DateOperator = DateTime.Now;
                log.OperatorContent = "修改活动排序";
                log.OperatorUserId = PresentationHelper.GetPassport().UserName;
                subject.InsertSubjectOrChannelLog(log);
                return Json(new { result = 1, message = "保存成功！" });
            }
        }


        /// <summary>
        /// 商品视图页根据颜色排序
        /// </summary>
        /// <param name="colorall"></param>
        /// <returns></returns>
        public ActionResult AjaxSearchColorDetail(string colorall)
        {
            if (!string.IsNullOrEmpty(colorall))
            {
                SWfsSubjectService subject = new SWfsSubjectService();
                IList<WfsPrimaryColor> ColorList = subject.GetWfsPrimaryColorListByColorDetail(colorall);
                string ColorData = string.Empty;
                if (ColorList != null && ColorList.Count() > 0)
                {
                    foreach (WfsPrimaryColor model in ColorList)
                    {
                        string _color = model.PrimaryColorId + "," + model.PrimaryColorName + "|";
                        ColorData += _color;
                    }
                    return Json(new { result = true, message = ColorData });
                }
            }

            return Json(new { result = false, message = "查询失败！" });
        }


        /// <summary>
        /// 商品视图页根据颜色排序（页面初始化）
        /// </summary>
        /// <param name="colorall"></param>
        /// <returns></returns>
        public ActionResult AjaxSearchColorDetailInit(string colorall)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            string ColorData = string.Empty;
            if (!string.IsNullOrEmpty(colorall))
            {
                string[] _tempColor = colorall.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<WfsPrimaryColor> _ColorList = new List<WfsPrimaryColor>();
                IList<WfsPrimaryColor> ColorList = service.GetWfsPrimaryColorListByColorID(colorall);
                for (int _j = 0; _j < _tempColor.Length; _j++)
                {
                    foreach (WfsPrimaryColor model in ColorList)
                    {
                        if (model.PrimaryColorId.ToString() == _tempColor[_j])
                        {
                            string _color = model.PrimaryColorId + "," + model.PrimaryColorName + "|";
                            ColorData += _color;
                        }
                    }
                }
                return Json(new { result = true, message = ColorData });
            }
            return Json(new { result = false, message = "查询失败！" });
        }
        #endregion

        #region 活动可视化编辑

        #region 商品可视化(Web)

        /// <summary>
        ///商品可视化        
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="SCategoryNo"></param>
        /// <returns></returns>
        public ActionResult ProductView(string subjectNo, string SCategoryNo)
        {
            string sortType = Request["SortType"];    //排序方式
            string isAutoBottom = Request["IsAutoBottom"];   // 1 Yes  0  No
            string isOne = Request["isOne"];

            int showCount = AppSettingManager.AppSettings["SubjectProductShowCount"].ToInt(500);
            SWfsSubjectService service = new SWfsSubjectService();

            string ShowType = "0";
            List<SubjectProductRef> list1 = service.SelectSubjectProductRefList(SCategoryNo);
            List<SubjectProductRef> list = list1;

            //查询上次是已什么方式排序的
            SWfsSubjectCategory CategoryEntity = service.GetSubjectCategoryModel(SCategoryNo, true);
            if (string.IsNullOrEmpty(isOne))
            {
                IList<SWfsSubjectProductSort> sortList = new List<SWfsSubjectProductSort>();
                if (string.IsNullOrWhiteSpace(CategoryEntity.SortRuleType))
                {
                    sortList = service.GetProductSortList(SCategoryNo);
                    if (sortList != null && sortList.Count() > 0)
                    {
                        sortList = sortList.Where(c => c.ShowChannelType == 0).ToList();
                    }
                }
                else
                {
                    sortList = service.GetProductSortList(SCategoryNo, short.Parse(ShowType));
                }
                if (sortList != null && sortList.Count > 0)
                {
                    List<SubjectProductRef> proSortList = new List<SubjectProductRef>();
                    List<SubjectProductRef> proListAll = new List<SubjectProductRef>();
                    List<SubjectProductRef> proNoSortList = new List<SubjectProductRef>();
                    proSortList = (from l in list
                                   join s in sortList on l.ProductNo equals s.ProductNo
                                   orderby s.Sort ascending
                                   select l).ToList();

                    if (proSortList != null && proSortList.Count() > 0)
                    {
                        proListAll.AddRange(proSortList);
                    }
                    if (sortList.Count < list.Count)
                    {
                        proNoSortList = (from t in list
                                         where !(from b in sortList select b.ProductNo).Contains(t.ProductNo)
                                         select t).ToList();
                        if (proNoSortList != null && proNoSortList.Count() > 0)
                        {
                            proListAll.AddRange(proNoSortList);
                        }
                    }
                    list = proListAll;
                }
            }
            #region 显示库存 by zhangwei 20140625 开启使用
            SWfsProductService productService = new SWfsProductService();
            ProductInventory pin = new ProductInventory();
            foreach (var product in list)
            {
                pin = productService.GetInventoryByProductNo(product.ProductNo);
                product.Quantity = pin.SumQuantity;
                product.LockQuantity = pin.SumLockQuantity;
            }
            #endregion

            list = list.Distinct(new ComparerSubjectProductRef()).ToList();
            Dictionary<string, string> BrandDic = new Dictionary<string, string>();
            Dictionary<string, string> CategoryDic = new Dictionary<string, string>();
            string CategoryNos = string.Empty;
            string productNos = string.Empty;
            string ColorAll = string.Empty;
            string ColorData = string.Empty;
            string isSortData = string.Empty;
            string isIAB = string.Empty;
            if (list != null && list.Count() > 0)
            {
                ViewBag.SortRuleType = CategoryEntity.SortRuleType;
                ViewBag.IAB = CategoryEntity.IsAutoBottom;

                if (string.IsNullOrEmpty(sortType))
                    sortType = "";
                else
                    ViewBag.SortRuleType = sortType;

                if (string.IsNullOrEmpty(isAutoBottom))
                    isAutoBottom = ViewBag.IAB.ToString();
                else
                    ViewBag.IAB = short.Parse(isAutoBottom);
                isSortData = sortType;

                if (string.IsNullOrEmpty(isSortData))
                    isSortData = "";
                string[] sortData = isSortData.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (sortData.Length == 2)
                {
                    list = SearchSortList(list, sortData);
                }
                for (int i = 0; i < list.Count(); i++)
                {
                    #region 品牌
                    var singleDic = BrandDic.Where(c => c.Key == list[i].BrandNo);
                    if (singleDic.Count() == 0)
                    {
                        string tempBrandName = string.IsNullOrEmpty(list[i].BrandEnName) ? list[i].BrandCnName.Trim().Replace(" ", "_") : list[i].BrandEnName.Trim().Replace(" ", "_");
                        BrandDic.Add(list[i].BrandNo, tempBrandName);   // 品牌
                    }
                    #endregion

                    #region 组合商品编码、品类编码
                    if (CategoryNos.IndexOf(list[i].CategoryNo.Substring(0, 6) + ",") < 0)
                    {
                        CategoryNos += list[i].CategoryNo.Substring(0, 6) + ",";
                    }
                    productNos += list[i].ProductNo + ",";
                    #endregion

                    #region 组合颜色
                    string tempColor = GetProColor(list[i].ProductXmlText);

                    if (!string.IsNullOrEmpty(tempColor))
                    {
                        if (tempColor.IndexOf(",") > 0)
                        {
                            string[] _tempColor = tempColor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < _tempColor.Length; j++)
                            {
                                if (ColorAll.IndexOf(_tempColor[j] + ",") < 0)
                                {
                                    ColorAll += _tempColor[j] + ",";
                                }
                            }
                        }
                        else
                        {
                            if (ColorAll.IndexOf(tempColor + ",") < 0)
                            {
                                ColorAll += tempColor + ",";
                            }
                        }
                    }
                    #endregion
                }

                #region 判断类型根据查询做处理

                IList<WfsErpCategory> CategoryEntityList = service.GetWfsErpCategoryListByCategoryNo(CategoryNos);
                if (!string.IsNullOrEmpty(isSortData))
                {
                    if (sortData[0] == "8")   //色系
                    {
                        string[] _tempColor = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        List<WfsPrimaryColor> _ColorList = new List<WfsPrimaryColor>();
                        IList<WfsPrimaryColor> ColorList = service.GetWfsPrimaryColorListByColorID(sortData[1]);
                        for (int _j = 0; _j < _tempColor.Length; _j++)
                        {
                            foreach (WfsPrimaryColor model in ColorList)
                            {
                                if (model.PrimaryColorId.ToString() == _tempColor[_j])
                                {
                                    string _color = model.PrimaryColorId + "," + model.PrimaryColorName + "|";
                                    ColorData += _color;
                                }
                            }
                        }
                    }
                    if (sortData[0] == "7")   // 品类
                    {
                        List<WfsErpCategory> _CategoryEntityList = new List<WfsErpCategory>();
                        string[] sortcategory = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < sortcategory.Length; j++)
                        {
                            _CategoryEntityList.Add(CategoryEntityList.FirstOrDefault(c => c.CategoryNo == sortcategory[j]));
                        }
                        CategoryEntityList = _CategoryEntityList;
                    }

                    if (sortData[0] == "4")  //  收藏
                    {

                    }
                }
                #endregion

                #region 处理品类信息

                for (int i = 0; i < CategoryEntityList.Count(); i++)
                {
                    var singleDic = CategoryDic.Where(c => c.Key == CategoryEntityList[i].CategoryNo);
                    if (singleDic.Count() == 0)
                    {
                        CategoryDic.Add(CategoryEntityList[i].CategoryNo, CategoryEntityList[i].CategoryName);   // 品类
                    }
                }
                #endregion

            }
            else
            {
                ViewBag.SortRuleType = null;
                ViewBag.IAB = short.Parse("0");
            }
            if (isAutoBottom == "1")        // 是否沉底
            {
                list1 = new List<Entity.Extenstion.Outlet.SubjectProductRef>();
                List<SubjectProductRef> outProductsList = list.Where(c => c.Quantity <= 0).ToList();
                foreach (SubjectProductRef model in outProductsList)
                {
                    list.Remove(model);
                    list1.Add(model);
                }
                list.AddRange(list1);
            }
            ViewBag.productNos = productNos;
            ViewBag.Brand = BrandDic;
            ViewBag.Category = CategoryDic;
            ViewBag.ColorAll = ColorAll; // 所有商品的颜色（不重复）
            ViewBag.ColorData = ColorData;  //一级颜色
            return View(list);
        }
        #endregion


        /// <summary>
        ///商品可视化        
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="SCategoryNo"></param>
        /// <returns></returns>
        public ActionResult ProductMobileView(string subjectNo, string SCategoryNo)
        {
            string sortType = Request["SortType"];    //排序方式
            string isAutoBottom = Request["IsAutoBottom"];   // 1 Yes  0  No
            string isOne = Request["isOne"];   // 1 Yes  0  No
            int showCount = AppSettingManager.AppSettings["SubjectProductShowCount"].ToInt(500);
            SWfsSubjectService service = new SWfsSubjectService();

            string ShowType = "1";

            List<SubjectProductRef> list1 = service.SelectSubjectProductRefList(SCategoryNo);
            List<SubjectProductRef> list = list1;
            //查询上次是已什么方式排序的
            SWfsSubjectCategory CategoryEntity = service.GetSubjectCategoryModel(SCategoryNo, true);
            if (string.IsNullOrEmpty(isOne))
            {
                IList<SWfsSubjectProductSort> sortList = new List<SWfsSubjectProductSort>();
                if (string.IsNullOrWhiteSpace(CategoryEntity.MobileSortRuleType))  //string.IsNullOrWhiteSpace(CategoryEntity.SortRuleType)
                {
                    sortList = service.GetProductSortList(SCategoryNo);
                    if (sortList != null && sortList.Count() > 0)
                    {
                        sortList = sortList.Where(c => c.ShowChannelType == 1).ToList();
                    }
                }
                else
                {
                    sortList = service.GetProductSortList(SCategoryNo, short.Parse(ShowType));
                }
                List<SubjectProductRef> tmplList = list.ToList();
                if (sortList != null && sortList.Count > 0)
                {
                    List<SubjectProductRef> proSortList = new List<SubjectProductRef>();
                    List<SubjectProductRef> proListAll = new List<SubjectProductRef>();
                    List<SubjectProductRef> proNoSortList = new List<SubjectProductRef>();
                    proSortList = (from l in list
                                   join s in sortList on l.ProductNo equals s.ProductNo
                                   orderby s.Sort ascending
                                   select l).ToList();

                    if (proSortList != null && proSortList.Count() > 0)
                    {
                        proListAll.AddRange(proSortList);
                    }
                    if (sortList.Count < list.Count)
                    {
                        proNoSortList = (from t in list
                                         where !(from b in sortList select b.ProductNo).Contains(t.ProductNo)
                                         select t).ToList();
                        if (proNoSortList != null && proNoSortList.Count() > 0)
                        {
                            proListAll.AddRange(proNoSortList);
                        }
                    }
                    list = proListAll;
                }
            }

            #region 显示库存 by zhangwei 20140625 开启使用
            SWfsProductService productService = new SWfsProductService();
            ProductInventory pin = new ProductInventory();
            foreach (var product in list)
            {
                pin = productService.GetInventoryByProductNo(product.ProductNo);
                product.Quantity = pin.SumQuantity;
                product.LockQuantity = pin.SumLockQuantity;
            }
            #endregion

            list = list.Distinct(new ComparerSubjectProductRef()).ToList();

            Dictionary<string, string> BrandDic = new Dictionary<string, string>();
            Dictionary<string, string> CategoryDic = new Dictionary<string, string>();
            string CategoryNos = string.Empty;
            string productNos = string.Empty;
            string ColorAll = string.Empty;
            string ColorData = string.Empty;
            string isSortData = string.Empty;
            string isIAB = string.Empty;
            if (list != null && list.Count() > 0)
            {
                ViewBag.SortRuleType = CategoryEntity.MobileSortRuleType;
                ViewBag.IAB = CategoryEntity.MobileIsAutoBottom;


                if (string.IsNullOrEmpty(sortType))
                    sortType = "";
                else
                    ViewBag.SortRuleType = sortType;

                if (string.IsNullOrEmpty(isAutoBottom))
                    isAutoBottom = ViewBag.IAB.ToString();
                else
                    ViewBag.IAB = short.Parse(isAutoBottom);
                isSortData = sortType;

                if (string.IsNullOrEmpty(isSortData))
                    isSortData = "";
                string[] sortData = isSortData.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (sortData.Length == 2)
                {
                    list = SearchSortList(list, sortData);
                }
                for (int i = 0; i < list.Count(); i++)
                {
                    #region 品牌
                    var singleDic = BrandDic.Where(c => c.Key == list[i].BrandNo);
                    if (singleDic.Count() == 0)
                    {
                        string tempBrandName = string.IsNullOrEmpty(list[i].BrandEnName) ? list[i].BrandCnName.Trim().Replace(" ", "_") : list[i].BrandEnName.Trim().Replace(" ", "_");
                        BrandDic.Add(list[i].BrandNo, tempBrandName);   // 品牌
                    }
                    #endregion

                    #region 组合商品编码、品类编码
                    if (CategoryNos.IndexOf(list[i].CategoryNo.Substring(0, 6) + ",") < 0)
                    {
                        CategoryNos += list[i].CategoryNo.Substring(0, 6) + ",";
                    }
                    productNos += list[i].ProductNo + ",";
                    #endregion

                    #region 组合颜色
                    string tempColor = GetProColor(list[i].ProductXmlText);

                    if (!string.IsNullOrEmpty(tempColor))
                    {
                        if (tempColor.IndexOf(",") > 0)
                        {
                            string[] _tempColor = tempColor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < _tempColor.Length; j++)
                            {
                                if (ColorAll.IndexOf(_tempColor[j] + ",") < 0)
                                {
                                    ColorAll += _tempColor[j] + ",";
                                }
                            }
                        }
                        else
                        {
                            if (ColorAll.IndexOf(tempColor + ",") < 0)
                            {
                                ColorAll += tempColor + ",";
                            }
                        }
                    }
                    #endregion
                }

                #region 判断类型根据查询做处理

                IList<WfsErpCategory> CategoryEntityList = service.GetWfsErpCategoryListByCategoryNo(CategoryNos);
                if (!string.IsNullOrEmpty(isSortData))
                {
                    if (sortData[0] == "8")   //色系
                    {
                        string[] _tempColor = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        List<WfsPrimaryColor> _ColorList = new List<WfsPrimaryColor>();
                        IList<WfsPrimaryColor> ColorList = service.GetWfsPrimaryColorListByColorID(sortData[1]);
                        for (int _j = 0; _j < _tempColor.Length; _j++)
                        {
                            foreach (WfsPrimaryColor model in ColorList)
                            {
                                if (model.PrimaryColorId.ToString() == _tempColor[_j])
                                {
                                    string _color = model.PrimaryColorId + "," + model.PrimaryColorName + "|";
                                    ColorData += _color;
                                }
                            }
                        }
                    }
                    if (sortData[0] == "7")   // 品类
                    {
                        List<WfsErpCategory> _CategoryEntityList = new List<WfsErpCategory>();
                        string[] sortcategory = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < sortcategory.Length; j++)
                        {
                            _CategoryEntityList.Add(CategoryEntityList.FirstOrDefault(c => c.CategoryNo == sortcategory[j]));
                        }
                        CategoryEntityList = _CategoryEntityList;
                    }

                }
                #endregion

                #region 处理品类信息

                for (int i = 0; i < CategoryEntityList.Count(); i++)
                {
                    var singleDic = CategoryDic.Where(c => c.Key == CategoryEntityList[i].CategoryNo);
                    if (singleDic.Count() == 0)
                    {
                        CategoryDic.Add(CategoryEntityList[i].CategoryNo, CategoryEntityList[i].CategoryName);   // 品类
                    }
                }
                #endregion

            }
            else
            {
                ViewBag.SortRuleType = null;
                ViewBag.IAB = short.Parse("0");
            }
            if (isAutoBottom == "1")        // 是否沉底
            {
                list1 = new List<Entity.Extenstion.Outlet.SubjectProductRef>();
                List<SubjectProductRef> outProductsList = list.Where(c => c.Quantity <= 0).ToList();
                foreach (SubjectProductRef model in outProductsList)
                {
                    list.Remove(model);
                    list1.Add(model);
                }
                list.AddRange(list1);
            }
            ViewBag.productNos = productNos;
            ViewBag.Brand = BrandDic;
            ViewBag.Category = CategoryDic;
            ViewBag.ColorAll = ColorAll; // 所有商品的颜色（不重复）
            ViewBag.ColorData = ColorData;  //一级颜色
            return View(list);
        }


        #region 排序switch方法

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        public List<SubjectProductRef> SearchSortList(List<SubjectProductRef> list, string[] sortData)
        {
            switch (Convert.ToInt32(sortData[0]))
            {
                #region 按价格

                case (int)SubjectSortRuleType.price://"price":    //按价格
                    if (sortData[1] == "0")
                        list = list.OrderByDescending(c => c.LimitedVipPrice).ToList();
                    else
                        list = list.OrderBy(c => c.LimitedVipPrice).ToList();
                    break;

                #endregion

                #region 按库存

                case (int)SubjectSortRuleType.stock://"stock":   //按库存
                    if (sortData[1] == "0")
                        list = list.OrderByDescending(c => c.Quantity).ToList();
                    else
                        list = list.OrderBy(c => c.Quantity).ToList();
                    break;

                #endregion

                #region 按折扣

                case (int)SubjectSortRuleType.discount://"discount":  //
                    if (sortData[1] == "0")
                        list = list.OrderByDescending(c => c.MarketPrice == 0 ? 0 : Math.Round((c.LimitedVipPrice * 10) / c.MarketPrice, 1, MidpointRounding.AwayFromZero)).ToList();
                    else
                        list = list.OrderBy(c => c.MarketPrice == 0 ? 0 : Math.Round((c.LimitedVipPrice * 10) / c.MarketPrice, 1, MidpointRounding.AwayFromZero)).ToList();
                    break;

                #endregion

                #region 上架时间

                case (int)SubjectSortRuleType.putawaytime://"putawaytime":  //上架时间
                    if (sortData[1] == "0")
                        list = list.OrderByDescending(c => c.DateShelf).ToList();
                    else
                        list = list.OrderBy(c => c.DateShelf).ToList();
                    break;

                #endregion

                #region 品牌

                case (int)SubjectSortRuleType.brand://"brand":  //品牌
                    List<SubjectProductRef> brandEntityList = new List<SubjectProductRef>();
                    string[] sortbrand = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < sortbrand.Length; j++)
                    {
                        foreach (SubjectProductRef model in list)
                        {
                            if (model.BrandNo == sortbrand[j])
                            {
                                brandEntityList.Add(model);
                            }
                        }
                    }
                    list = brandEntityList;
                    break;
                #endregion

                #region 品类

                case (int)SubjectSortRuleType.category://"category":  //品类
                    List<SubjectProductRef> categoryEntityList = new List<SubjectProductRef>();
                    string[] sortcategory = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < sortcategory.Length; j++)
                    {
                        categoryEntityList.AddRange(list.Where(c => c.CategoryNo.Substring(0, 6) == sortcategory[j]));
                    }
                    list = categoryEntityList;
                    break;

                #endregion

                #region 色系

                case (int)SubjectSortRuleType.color://"color":  色系

                    List<SubjectProductRef> colorEntityList = new List<SubjectProductRef>();
                    List<SubjectProductRef> TempColorEntityList = new List<SubjectProductRef>();
                    SWfsSubjectService subject = new SWfsSubjectService();
                    IList<WfsProductColorAttrDetail> wpcaEntity = subject.GetWfsProductColorAttrDetailListByColorDetail(sortData[1]);
                    List<WfsProductColorAttrDetail> _wpcaEntity = new List<WfsProductColorAttrDetail>();

                    if (wpcaEntity != null && wpcaEntity.Count() > 0)
                    {
                        string[] sortcolor = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < sortcolor.Length; j++)
                        {
                            int colorNo = Convert.ToInt32(sortcolor[j]);
                            _wpcaEntity.AddRange(wpcaEntity.Where(c => c.PrimaryColorId == colorNo));
                        }
                        wpcaEntity = _wpcaEntity;
                    }
                    foreach (WfsProductColorAttrDetail colorModel in wpcaEntity)
                    {
                        foreach (SubjectProductRef model in list)
                        {
                            string tempColor = GetProColor(model.ProductXmlText);

                            if (!string.IsNullOrEmpty(tempColor))
                            {
                                if (tempColor.IndexOf(",") > 0)
                                {
                                    string[] _tempColor = tempColor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (colorModel.ColorName == _tempColor[0])
                                    {
                                        colorEntityList.Add(model);
                                    }
                                }
                                else
                                {
                                    if (colorModel.ColorName == tempColor)
                                    {
                                        colorEntityList.Add(model);
                                    }
                                }
                            }
                        }
                    }
                    if (colorEntityList.Count() != list.Count())
                    {
                        foreach (SubjectProductRef model in colorEntityList)
                        {
                            list.Remove(model);
                        }
                        colorEntityList.AddRange(list);
                        list = colorEntityList;
                    }
                    else
                    {
                        list = colorEntityList;
                    }
                    break;
                #endregion

                #region 收藏
                case (int)SubjectSortRuleType.collect:
                    string TempProductNos = string.Empty;
                    List<SubjectProductRef> _list = list;
                    for (int i = 0; i < list.Count(); i++)
                    {
                        TempProductNos += list[i].ProductNo + ",";
                    }
                    List<SubjectProductRef> _ProductCountList = new List<SubjectProductRef>();

                    SWfsSubjectService service = new SWfsSubjectService();
                    IList<FavoriteProductCount> ProductCountList = service.GetSWfsFavoriteProductByProductNos(TempProductNos);
                    if (sortData[1] == "0")
                    {
                        ProductCountList = ProductCountList.OrderByDescending(c => c.ProCount).OrderByDescending(c => c.ProductNo).ToList();
                        for (int i = 0; i < ProductCountList.Count(); i++)
                        {
                            SubjectProductRef _model = list.SingleOrDefault(c => c.ProductNo == ProductCountList[i].ProductNo);
                            _ProductCountList.Add(_model);
                            _list.Remove(_model);
                        }
                        _ProductCountList.AddRange(_list);
                        list = _ProductCountList;
                    }
                    else
                    {
                        ProductCountList = ProductCountList.OrderBy(c => c.ProCount).OrderBy(c => c.ProductNo).ToList();
                        for (int i = 0; i < ProductCountList.Count(); i++)
                        {
                            SubjectProductRef _model = list.SingleOrDefault(c => c.ProductNo == ProductCountList[i].ProductNo);
                            _ProductCountList.Add(_model);
                            _list.Remove(_model);
                        }
                        _list.AddRange(_ProductCountList);
                        list = _list;
                    }
                    break;
                #endregion

                #region 默认

                case (int)SubjectSortRuleType._default://"default":  //默认
                    list = list.OrderByDescending(c => c.DateCreate).ToList();
                    break;

                #endregion
            }

            return list;
        }

        #endregion


        /// <summary>
        /// 活动预览--可视化编辑
        /// </summary>
        /// <param name="channelNo">3首页,0女士,1男士等等，默认为首页</param>
        /// <param name="showType">展示渠道 (zsqd001网站,zsqd002移动端)，默认为网站</param>
        /// <param name="showDateTime">预排日期时间(只有首页有预排功能，频道活动不需要预排功能)</param>
        /// <returns></returns>
        public ActionResult SubjectView(string channelNo = "3", string showType = "1", string showDateTime = "")
        {
            #region bankcode
            string bankcode = string.Empty;
            HttpCookie cookie = Request.Cookies["BankCodeCookie"];
            if (cookie != null)
            {
                bankcode = cookie.Value;
            }
            #endregion

            SWfsSubjectService service = new SWfsSubjectService();
            //活动展示个数
            int showCount = AppSettingManager.AppSettings["SubjectShowCount"].ToInt(100);
            int tdShowCount = showCount;
            int aeShowCount = showCount;
            int abShowCount = showCount;
            string txtShowType = string.Empty;
            txtShowType = showType.Equals("1") ? "zsqd001" : showType.Equals("2") ? "zsqd002" : "";
            if (channelNo.Equals("3"))   //首页
            {
                showDateTime = !string.IsNullOrEmpty(showDateTime) ? showDateTime : DateTime.Now.ToString("yyyy-MM-dd");

                #region 首页

                #region 今天新开的活动

                Task<IEnumerable<SubjectInfo>> task1 = Task.Factory.StartNew(() => service.GetSubjectsNew(SubjectType.ToDaySubject.ToString(), channelNo, txtShowType, showDateTime, 0, tdShowCount));
                IEnumerable<SubjectInfo> todaySubjectList = task1.Result;

                Task<IEnumerable<SubjectInfo>> task2 = Task.Factory.StartNew(() => service.GetSubjectsNew(SubjectType.SaleingToDaySubject.ToString(), channelNo, txtShowType, showDateTime, 0, tdShowCount));
                IEnumerable<SubjectInfo> saleingSubject = task2.Result;

                Task<IEnumerable<SubjectInfo>> task3 = Task.Factory.StartNew(() => service.GetSubjectsNew(SubjectType.AboutExpireSubject.ToString(), channelNo, txtShowType, showDateTime, 0, tdShowCount));
                IEnumerable<SubjectInfo> aboutExpireSubject = task3.Result;

                Task.WaitAll(task1, task2, task3);

                List<SubjectInfo> newtodaySubjectList = new List<SubjectInfo>();
                if (todaySubjectList != null)
                {
                    SWfsSubjectConsoleService subjectConsole = new SWfsSubjectConsoleService();

                    #region 排除重复今日新开广告活动

                    /*今日新开广告图*/
                    List<SWfsPictureManager> picADList = subjectConsole.GetIndexADPicListALL(3, new int[] { 10 });
                    SWfsPictureManager todayAdModel = picADList.Where(r => r.Position.Equals(10)).OrderByDescending(r => r.DateCreate).FirstOrDefault();

                    //增加一个规则20140604如果今日新开的广告图关联的活动与今日新开中的活动重复，则排除掉今日新开中的活动
                    if (todayAdModel != null)
                    {
                        string adSubjectNo = GetSubjectNoByLink(todayAdModel.LinkAddress);
                        todaySubjectList = todaySubjectList.Where(r => r.SubjectNo != adSubjectNo).ToList();
                    }

                    #endregion

                    #region 排除专题重复活动

                    List<SubjectTopicM> topPics = new List<SubjectTopicM>();
                    //修改逻辑如下20140515：
                    List<SubjectTopicM> tmptoplist = new List<SubjectTopicM>();
                    List<SubjectTopicM> toplist = subjectConsole.GetFoucsAreaSubjectList();
                    if (toplist == null || toplist.Count() < 3)
                    {
                        List<string> subNos = new List<string>();
                        if (toplist != null) subNos = toplist.Select(t => t.SubjectNo).ToList();
                        int tmpNum = (toplist != null ? 3 - toplist.Count() : 3);
                        //移动端只排除手工设置的专题
                        if (showType.Equals("1"))
                        {
                            tmptoplist = subjectConsole.GetSubjectTopicListNewNormal(tmpNum, subNos);//20140515修改为读取正常开启专题类型的活动
                        }
                    }
                    if (tmptoplist != null && toplist != null)
                    {
                        toplist.AddRange(tmptoplist);
                    }

                    topPics = (toplist == null ? tmptoplist : toplist); //前台判断链接

                    string[] subNosNew = null;
                    if (topPics != null)
                    {
                        subNosNew = topPics.Select(p => p.SubjectNo).ToArray();
                        todaySubjectList = (from today in todaySubjectList
                                            where !subNosNew.Contains(today.SubjectNo)
                                            select today).ToList();
                    }
                    #endregion

                    newtodaySubjectList.AddRange(todaySubjectList);
                }
                List<SubjectInfo> tmpList = newtodaySubjectList;
                List<SubjectInfo> tmpListII = new List<SubjectInfo>();
                //IList<SWfsSubjectSort> sortList = service.GetSubjectSortListNew(channelNo, 1, showType, showDateTime);
                IList<SWfsSubjectSort> sortList = GetSubjectSortDataList(channelNo, 1, showType, showDateTime);
                if (sortList.Count > 0)
                {
                    newtodaySubjectList = (from l in newtodaySubjectList
                                           join s in sortList on l.SubjectNo equals s.SubjectNo
                                           orderby s.Sort ascending
                                           select l).ToList();
                }
                if (newtodaySubjectList.Count < tmpList.Count)
                {
                    /*
                     * 排序规则
                     * 1首先读取已有排序的活动
                     * 2排除已有排序的活动
                     * 3在第2步活动中选择专题类型的活动添加到顶部显示
                     * 4其余的活动在底部显示
                     * 保证没有被手动排序的专题类型的活动默认显示在顶部
                     */
                    tmpList = (from t in tmpList
                               where !(from b in newtodaySubjectList select b.SubjectNo).Contains(t.SubjectNo)
                               orderby t.DateBegin descending, t.DateCreate descending
                               select t).ToList();
                    tmpListII = tmpList.Where(r => r.SubjectTemplate == 4).OrderByDescending(r => r.DateBegin).ToList();//专题类型的活动;
                    tmpList = tmpList.Where(r => r.SubjectTemplate != 4).ToList();
                    if (tmpList != null && tmpList.Count > 0)
                    {
                        newtodaySubjectList.AddRange(tmpList);
                    }
                    if (tmpListII != null && tmpListII.Count > 0)
                    {
                        newtodaySubjectList.InsertRange(0, tmpListII);
                    }
                }

                newtodaySubjectList = newtodaySubjectList != null ? newtodaySubjectList.Distinct(new ComparerSubject()).ToList() : null;
                ViewBag.ToDaySubject = newtodaySubjectList;

                #endregion

                #region 正在进行的活动

                List<SubjectInfo> subjectList = saleingSubject == null ? null : saleingSubject.ToList();
                if (null != saleingSubject)
                {
                    //IList<SWfsSubjectSort> saleingsortlist = service.GetSubjectSortListNew(channelNo, 3, showType, showDateTime);
                    IList<SWfsSubjectSort> saleingsortlist = GetSubjectSortDataList(channelNo, 3, showType, showDateTime);
                    List<SubjectInfo> tmpsaleingList = null == saleingSubject ? new List<SubjectInfo>() : saleingSubject.ToList();
                    if (null != saleingsortlist && saleingsortlist.Count > 0)
                    {
                        saleingSubject = from l in saleingSubject
                                         join s in saleingsortlist on l.SubjectNo equals s.SubjectNo
                                         orderby s.Sort ascending
                                         select l;
                        subjectList = saleingSubject.ToList();
                    }
                    if (null != saleingSubject && saleingSubject.Count() < tmpsaleingList.Count)
                    {
                        tmpsaleingList = (from t in tmpsaleingList
                                          where !(from b in saleingSubject select b.SubjectNo).Contains(t.SubjectNo)
                                          orderby t.DateBegin descending, t.DateCreate descending
                                          select t).ToList();
                        subjectList = saleingSubject.ToList();
                        subjectList.AddRange(tmpsaleingList);
                    }
                }
                subjectList = subjectList != null ? subjectList.Distinct(new ComparerSubject()).ToList() : null;
                ViewBag.SaleingSubject = subjectList;

                #endregion

                #region 即将结束的活动

                List<SubjectInfo> aboutExpireSubjectList = aboutExpireSubject == null ? null : aboutExpireSubject.ToList();
                if (aboutExpireSubject != null)
                {
                    //IList<SWfsSubjectSort> sortlist = service.GetSubjectSortListNew(channelNo, 2, showType, showDateTime);
                    IList<SWfsSubjectSort> sortlist = GetSubjectSortDataList(channelNo, 2, showType, showDateTime);
                    List<SubjectInfo> tmplList = null == aboutExpireSubject ? new List<SubjectInfo>() : aboutExpireSubject.ToList();
                    if (null != sortlist && sortlist.Count > 0)
                    {
                        aboutExpireSubject = from l in aboutExpireSubject
                                             join s in sortlist on l.SubjectNo equals s.SubjectNo
                                             orderby s.Sort ascending
                                             select l;
                        aboutExpireSubjectList = aboutExpireSubject.ToList();
                    }
                    if (null != aboutExpireSubject && aboutExpireSubject.Count() < tmplList.Count)
                    {
                        tmplList = (from t in tmplList
                                    where !(from b in aboutExpireSubject select b.SubjectNo).Contains(t.SubjectNo)
                                    orderby t.DateBegin descending, t.DateCreate descending
                                    select t).ToList();
                        aboutExpireSubjectList = aboutExpireSubject.ToList();
                        aboutExpireSubjectList.AddRange(tmplList);
                    }
                }

                aboutExpireSubjectList = aboutExpireSubjectList != null ? aboutExpireSubjectList.Distinct(new ComparerSubject()).ToList() : null;
                ViewBag.AboutExpireSubject = aboutExpireSubjectList;
                #endregion

                #region 暂时不用
                //即将开启的活动
                //jjBeginList = jjBeginList != null ? jjBeginList.Distinct(new ComparerSubject()).ToList() : null;

                //ViewBag.AboutBeginSubject = jjBeginList;
                #endregion

                #endregion
            }
            else
            {

                #region 频道
                SWfsSubjectConsoleService serviceConsole = new SWfsSubjectConsoleService();
                Task<IEnumerable<SubjectInfo>> task1 = Task.Factory.StartNew(() => serviceConsole.GetChannelSubjectList(channelNo, txtShowType));
                IEnumerable<SubjectInfo> channelSubjectList = task1.Result;
                Task.WaitAll(task1);

                #region 网站排除频道推广活动 by zhangwei 20140822
                if (showType.Equals("1"))
                {
                    Task<List<SubjectInfo>> task2 = Task.Factory.StartNew(() => serviceConsole.GetChannelMainSubjectList(channelNo));

                    List<SubjectInfo> mainSubjectlist = task2.Result;
                    Task.WaitAll(task2);

                    #region 处理频道主推活动
                    if (mainSubjectlist != null && mainSubjectlist.Count() > 0)
                    {

                        List<SubjectInfo> tempMainList = new List<SubjectInfo>();
                        for (int i = 1; i <= 2; i++)
                        {
                            #region 获取位置的所有活动
                            List<SubjectInfo> firstAllSubjectList = mainSubjectlist.Where(a => a.Location == i).OrderByDescending(o => o.DateShow).ToList();

                            //按展示时间获取时间分组
                            List<string> groupList = new List<string>();
                            if (firstAllSubjectList != null && firstAllSubjectList.Count() > 0)
                            {
                                foreach (var item in firstAllSubjectList)
                                {
                                    if (!groupList.Contains(item.DateShow.ToString("yyyy-MM-dd")))
                                    {
                                        groupList.Add(item.DateShow.ToString("yyyy-MM-dd"));
                                    }
                                }
                            }
                            if (groupList != null && groupList.Count() > 0)
                            {
                                foreach (var group in groupList)
                                {
                                    DateTime nowdt = DateTime.Now; //当前日期时间
                                    DateTime gdt = DateTime.Parse(group);  //时间组日期

                                    if (new DateTime(nowdt.Year, nowdt.Month, nowdt.Day, 0, 0, 0) == new DateTime(gdt.Year, gdt.Month, gdt.Day, 0, 0, 0))
                                    {
                                        DateTime dt = new DateTime(nowdt.Year, nowdt.Month, nowdt.Day, nowdt.Hour, nowdt.Minute, 0);
                                        List<SubjectInfo> firstList = mainSubjectlist.Where(a => a.Location == i && a.DateShow <= dt).OrderByDescending(o => o.DateShow).ToList();
                                        if (firstList != null && firstList.Count() > 0)
                                        {
                                            tempMainList.AddRange(firstList.Take(1));
                                            break;
                                        }
                                    }
                                    else if (new DateTime(nowdt.Year, nowdt.Month, nowdt.Day, 0, 0, 0) > new DateTime(gdt.Year, gdt.Month, gdt.Day, 0, 0, 0))
                                    {
                                        DateTime dt = new DateTime(gdt.Year, gdt.Month, gdt.Day, 0, 0, 0);
                                        List<SubjectInfo> firstList = mainSubjectlist.Where(a => a.Location == i && a.DateShow.Year == gdt.Year && a.DateShow.Month == gdt.Month && a.DateShow.Day == gdt.Day).OrderByDescending(o => o.DateShow).ToList();
                                        if (firstList != null && firstList.Count() > 0)
                                        {
                                            tempMainList.AddRange(firstList.Take(1));
                                            break;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }

                        mainSubjectlist = tempMainList;
                    }
                    #endregion

                    if (null != mainSubjectlist && mainSubjectlist.Count >= 2)
                    {
                        foreach (var item in mainSubjectlist.Take(2))
                        {
                            channelSubjectList = channelSubjectList.Where(r => r.SubjectNo != item.SubjectNo).ToList();
                        }
                    }
                }
                #endregion

                #region 排序
                List<SubjectInfo> channelSubjectMainList = channelSubjectList == null ? null : channelSubjectList.ToList();
                if (channelSubjectList != null && channelSubjectList.Count() > 0)
                {
                    //IList<SWfsSubjectSort> sortlist = service.GetSubjectSortListNew(channelNo, 0, showType, "");
                    IList<SWfsSubjectSort> sortlist = GetSubjectSortDataList(channelNo, 0, showType, "");
                    List<SubjectInfo> tmplList = null == channelSubjectList ? new List<SubjectInfo>() : channelSubjectList.ToList();
                    if (null != sortlist && sortlist.Count > 0)
                    {
                        channelSubjectList = from l in channelSubjectList
                                             join s in sortlist on l.SubjectNo equals s.SubjectNo
                                             orderby s.Sort ascending
                                             select l;
                        channelSubjectMainList = channelSubjectList.ToList();
                    }
                    if (null != channelSubjectList && channelSubjectList.Count() < tmplList.Count)
                    {
                        tmplList = (from t in tmplList
                                    where !(from b in channelSubjectList select b.SubjectNo).Contains(t.SubjectNo)
                                    orderby t.DateBegin descending, t.DateCreate descending
                                    select t).ToList();
                        channelSubjectMainList = channelSubjectList.ToList();

                        #region 最新活动追加到前面 by zhangwei 20140822
                        tmplList = tmplList.OrderByDescending(a => a.DateBegin).ToList();
                        tmplList.AddRange(channelSubjectMainList);
                        #endregion

                        channelSubjectMainList = tmplList;
                    }
                }
                #endregion

                channelSubjectMainList = channelSubjectMainList != null ? channelSubjectMainList.Distinct(new ComparerSubject()).ToList() : null;
                ViewBag.ChannelSubjectList = channelSubjectMainList;
                #endregion
            }
            ViewBag.ChannelNo = channelNo.ToString();
            ViewBag.ChannelList = service.FindSubjectChannelList();
            ViewBag.ShowTypeNo = showType;
            ViewBag.ShowDateTime = !string.IsNullOrEmpty(showDateTime) ? showDateTime : DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        /// <summary>
        /// 获取奥莱首页和频道页排序数据  by zhangwei 20140818
        /// </summary>
        /// <param name="channelNo">频道编号</param>
        /// <param name="type">1今日新开，3正在进行，2即将结束</param>
        /// <param name="showType">显示渠道 1网站，2移动端</param>
        /// <param name="showDateTime">排期日期(只限制首页)</param>
        /// <returns></returns>
        private IList<SWfsSubjectSort> GetSubjectSortDataList(string channelNo, short type, string showType, string showDateTime)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            IList<SWfsSubjectSort> sortlist = new List<SWfsSubjectSort>();

            sortlist = service.GetSubjectSortListNew(channelNo, type, showType, showDateTime);
            if (sortlist != null && sortlist.Count() > 0)
            {
                return sortlist;
            }
            else
            {
                #region 切换为移动端时，并且移动端排序数据为空，则读取网站排序
                if (showType.Equals("2"))
                {
                    showType = "1"; //网站
                    sortlist = service.GetSubjectSortListNew(channelNo, type, showType, showDateTime);
                }
                #endregion
            }
            return sortlist;
        }

        private string GetSubjectNoByLink(string linkUrl)
        {
            string subjectNo = string.Empty;
            if (string.IsNullOrEmpty(linkUrl))
            {
                return subjectNo;
            }
            if (linkUrl.ToLower().Contains("aolai.com/subject/index"))
            {
                string[] tempArry = linkUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                Regex reg = new Regex(@"^\d{8}$", RegexOptions.IgnoreCase);
                foreach (var item in tempArry)
                {
                    if (reg.IsMatch(item))
                    {
                        subjectNo = item;
                        break;
                    }
                }
            }
            return subjectNo;
        }

        #endregion

        #region 比较
        public class ComparerSubject : IEqualityComparer<SubjectInfo>
        {
            public bool Equals(SubjectInfo t1, SubjectInfo t2)
            {
                return (t1.SubjectNo == t2.SubjectNo);
            }
            public int GetHashCode(SubjectInfo t)
            {
                return t.ToString().GetHashCode();
            }
        }
        /*20131021测试说出现商品重复问题，本地不能复现，添加强制排重*/
        /// <summary>
        /// 比较 用于排序
        /// </summary>
        public class ComparerSubjectProductRef : IEqualityComparer<SubjectProductRef>
        {
            public bool Equals(SubjectProductRef t1, SubjectProductRef t2)
            {
                return (t1.ProductNo == t2.ProductNo);
            }
            public int GetHashCode(SubjectProductRef t)
            {
                return t.ToString().GetHashCode();
            }
        }
        #endregion

        #region 日志管理
        //日志管理
        public ActionResult LogManage(string subjectNo, string key, string begintime, string endtime, int pageIndex = 1)
        {
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;
            SWfsSubjectService service = new SWfsSubjectService();
            IList<SWfsSubjectOrChannelLog> list = service.GetLogList(subjectNo, key, begintime, endtime);
            ViewBag.Count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();//默认每页显示20条数据
            ViewBag.LogList = list;
            ViewBag.SubjectName = service.GetSubjectInfo(subjectNo).SubjectName;
            return View();

        }
        #endregion

        #region 数据统计
        //数据统计
        public ActionResult DataStatistics(string subjectNo, string range = "0", string beginTime = "", string endTime = "", string buNo = "", string brandNo = "", string categoryNo = "", string cacheType = "")
        {
            SWfsSubjectService service = new SWfsSubjectService();
            IList<SubjectProductStatistic> productStatisticList = new List<SubjectProductStatistic>();
            IList<SubjectProductStatistic> tempProductStatisticList = new List<SubjectProductStatistic>();
            ViewBag.subjectNo = subjectNo;
            ViewBag.range = range;
            ViewBag.endTime = endTime;
            ViewBag.SubjectCurrUrl = Request.QueryString["BackSubjectUrl"].ToString();
            ViewBag.BUNo = buNo;
            ViewBag.BrandNo = brandNo;
            ViewBag.CategoryNo = categoryNo;
            if (range == "5")
            {
                if (string.IsNullOrWhiteSpace(beginTime))
                {
                    beginTime = DateTime.Now.AddMonths(-3).ToString();
                }
            }
            else
            {
                beginTime = string.Empty;
            }
            SubjectInfo subjectModel = service.GetSubjectInfo(subjectNo);
            ViewBag.SubjectName = subjectModel.SubjectName;
            ViewBag.beginTime = beginTime;

            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            Task<IList<SubjectProductStatistic>> task1 = Task.Factory.StartNew(() => service.GetSubjectProductStatisticList(subjectNo, range, beginTime, endTime));
            productStatisticList = task1.Result;
            sw1.Stop();
            ViewBag.Task1 = "查询商品统计耗时" + sw1.ElapsedMilliseconds;

            if (productStatisticList != null && productStatisticList.Count() > 0)
            {
                #region 条件筛选
                tempProductStatisticList = productStatisticList;
                Expression<Func<SubjectProductStatistic, bool>> predicate = PredicateBuilder.True<SubjectProductStatistic>();
                if (!string.IsNullOrEmpty(buNo))
                {
                    predicate = predicate.And(a => a.BU.Equals(buNo));
                }
                if (!string.IsNullOrEmpty(brandNo))
                {
                    predicate = predicate.And(a => a.BrandNo.Equals(brandNo));
                }
                if (!string.IsNullOrEmpty(categoryNo))
                {
                    predicate = predicate.And(a => a.CategoryNo.Equals(categoryNo));
                }
                tempProductStatisticList = productStatisticList.Where(predicate.Compile()).ToList();
                #endregion
            }
            ViewBag.subjectProductStatistics = tempProductStatisticList;

            #region 统计数据读取
            if (range.Equals("0"))
            {
                SubjectSaleVisitStatisticsDataM saleVisitModel = CheckSubjectRunType(subjectModel, range);
                if (saleVisitModel != null && !string.IsNullOrEmpty(saleVisitModel.SubjectNo))
                {
                    if (saleVisitModel.SaleStatistic != null && !string.IsNullOrEmpty(saleVisitModel.SaleStatistic.SubjectNo))
                    {
                        ViewBag.subjectSaleStatistics = saleVisitModel.SaleStatistic;
                    }
                    else
                    {
                        sw1.Restart();
                        Task<SubjectSaleStatistic> task2 = Task.Factory.StartNew(() => service.GetSubjectSaleStatistic(subjectNo, range, beginTime, endTime));
                        ViewBag.subjectSaleStatistics = task2.Result;
                        sw1.Stop();
                        ViewBag.Task2 = "查询销售统计耗时" + sw1.ElapsedMilliseconds;
                    }
                    if (saleVisitModel.VisitStatistic != null && !string.IsNullOrEmpty(saleVisitModel.VisitStatistic.SubjectNo))
                    {
                        ViewBag.subjectVisitStatistic = saleVisitModel.VisitStatistic;
                    }
                    else
                    {
                        sw1.Restart();
                        Task<SubjectVisitStatistic> task3 = Task.Factory.StartNew(() => service.GetSubjectVisitStatistic(subjectNo, range, beginTime, endTime));
                        ViewBag.subjectVisitStatistic = task3.Result;
                        sw1.Stop();
                        ViewBag.Task3 = "查询访问统计耗时" + sw1.ElapsedMilliseconds;
                    }
                }
                else
                {
                    sw1.Restart();
                    Task<SubjectSaleStatistic> task2 = Task.Factory.StartNew(() => service.GetSubjectSaleStatistic(subjectNo, range, beginTime, endTime));
                    ViewBag.subjectSaleStatistics = task2.Result;
                    sw1.Stop();
                    ViewBag.Task2 = "查询销售统计耗时" + sw1.ElapsedMilliseconds;

                    sw1.Restart();
                    Task<SubjectVisitStatistic> task3 = Task.Factory.StartNew(() => service.GetSubjectVisitStatistic(subjectNo, range, beginTime, endTime));
                    ViewBag.subjectVisitStatistic = task3.Result;
                    sw1.Stop();
                    ViewBag.Task3 = "查询访问统计耗时" + sw1.ElapsedMilliseconds;
                }
            }
            else
            {
                sw1.Restart();
                Task<SubjectSaleStatistic> task2 = Task.Factory.StartNew(() => service.GetSubjectSaleStatistic(subjectNo, range, beginTime, endTime));
                ViewBag.subjectSaleStatistics = task2.Result;
                sw1.Stop();
                ViewBag.Task2 = "查询销售统计耗时" + sw1.ElapsedMilliseconds;

                sw1.Restart();
                Task<SubjectVisitStatistic> task3 = Task.Factory.StartNew(() => service.GetSubjectVisitStatistic(subjectNo, range, beginTime, endTime));
                ViewBag.subjectVisitStatistic = task3.Result;
                sw1.Stop();
                ViewBag.Task3 = "查询访问统计耗时" + sw1.ElapsedMilliseconds;
            }
            #endregion

            IList<ORderToExcel> tmpList = service.GetOrderBySubject(subjectNo.Trim());
            ViewBag.OrderNums = tmpList != null ? tmpList.Count() : 0;

            #region BU|品牌|品类|筛选数据
            Dictionary<string, string> buDic = new Dictionary<string, string>(); //BU
            Dictionary<string, string> brandDic = new Dictionary<string, string>(); //品牌
            Dictionary<string, string> categoryDic = new Dictionary<string, string>(); //品类
            if (productStatisticList != null && productStatisticList.Count() > 0)
            {
                filterStatisticData(productStatisticList, ref buDic, ref brandDic, ref categoryDic);
            }
            ViewBag.BUStatisticsData = buDic; //BU
            ViewBag.BrandStatisticsData = brandDic; //品牌
            ViewBag.CategoryStatisticsData = categoryDic; //品类
            #endregion

            return View();
        }

        /// <summary>
        /// 判断当前活动是运行类型
        /// </summary>
        /// <param name="model">活动基本信息</param>
        /// <param name="range">0全部 1今天 2昨天 3本周 4本月 5自定义</param>
        /// <returns>1今日新开 2进行中 3已结束</returns>
        private SubjectSaleVisitStatisticsDataM CheckSubjectRunType(SubjectInfo model, string range)
        {
            #region //版本一 直接查询缓存中的数据
            //int type = 0;
            //SubjectSaleVisitStatisticsDataM statisticsM = new SubjectSaleVisitStatisticsDataM();
            //List<SubjectSaleVisitStatisticsDataM> list = new List<SubjectSaleVisitStatisticsDataM>();
            //if (model != null)
            //{
            //    if (DateTime.Compare(new DateTime(model.DateBegin.Year, model.DateBegin.Month, model.DateBegin.Day, 0, 0, 0), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)) == 0) //今日新开
            //    {
            //        type = 0;
            //        list = RedisCacheProvider.Instance.Get<List<SubjectSaleVisitStatisticsDataM>>("AoLaiSubjectSaleVisitStatisticData_Today");
            //    }
            //    else if (model.DateBegin < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0) && model.DateEnd > DateTime.Now) //进行中
            //    {
            //        type = 1;
            //        list = RedisCacheProvider.Instance.Get<List<SubjectSaleVisitStatisticsDataM>>("AoLaiSubjectSaleVisitStatisticData_Runing");
            //    }
            //    else if (model.DateEnd < DateTime.Now) //已结束
            //    {
            //        type = 2;
            //        list = RedisCacheProvider.Instance.Get<List<SubjectSaleVisitStatisticsDataM>>("AoLaiSubjectSaleVisitStatisticData_End");
            //    }
            //    if (list != null && list.Count() > 0)
            //    {
            //        statisticsM = list.Where(a => a.SubjectNo.Equals(model.SubjectNo)).FirstOrDefault();
            //    }
            //}
            //return statisticsM;
            #endregion

            //版本二取临时表中的数据---这里的商品统计呢？
            SubjectSaleVisitStatisticsDataM statisticsM = new SubjectSaleVisitStatisticsDataM();
            List<string> tmpSubjectNolist = new List<string>();
            tmpSubjectNolist.Add(model.SubjectNo);
            List<SubjectSaleVisitStatisticsDataM> list = new TempStatisticsService().GetStatisticsListBySubjectNoes(tmpSubjectNolist).ToList();
            statisticsM = list.Where(r => r.SubjectNo == model.SubjectNo).FirstOrDefault();
            return statisticsM;
        }

        /// <summary>
        /// 获取BU/品牌/品类筛选数据 by zhangwei 20140306
        /// </summary>
        /// <param name="list">所有商品统计数据</param>
        /// <param name="buDic">BU</param>
        /// <param name="brandDic">品牌</param>
        /// <param name="categoryDic">品类</param>
        public void filterStatisticData(IList<SubjectProductStatistic> list, ref Dictionary<string, string> buDic, ref Dictionary<string, string> brandDic, ref Dictionary<string, string> categoryDic)
        {
            if (list != null && list.Count() > 0)
            {
                //BU
                var tempBulist = (from b in list group b by new { b.BU } into g select new { g.Key, g }).ToList();
                if (tempBulist != null && tempBulist.Count() > 0)
                {
                    foreach (var item in tempBulist)
                    {
                        buDic.Add(item.Key.BU, item.Key.BU);
                    }
                }

                //品牌
                var tempBrandlist = (from b in list group b by new { b.BrandNo, b.BrandName } into g select new { g.Key, g }).ToList();
                if (tempBrandlist != null && tempBrandlist.Count() > 0)
                {
                    foreach (var item in tempBrandlist)
                    {
                        brandDic.Add(item.Key.BrandNo, item.Key.BrandName);
                    }
                }

                //品类
                var tempCategorylist = (from c in list group c by new { c.CategoryNo, c.CategoryName } into g select new { g.Key, g }).ToList();
                if (tempCategorylist != null && tempCategorylist.Count() > 0)
                {
                    foreach (var item in tempCategorylist)
                    {
                        categoryDic.Add(item.Key.CategoryNo, item.Key.CategoryName);
                    }
                }

            }
        }//filterStatisticData
        #endregion

        #region 导出Excel
        #region 活动商品导出Excel
        /// <summary>
        /// 活动商品导出Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectToExcel()
        {
            string subjectNo = Request.Params["subjectNo"] ?? "";
            if (!string.IsNullOrEmpty(subjectNo))
            {
                SWfsSubjectService service = new SWfsSubjectService();
                IList<ProductInfo> productList = service.GetProductList(subjectNo.Trim());
                List<ProductInfo> list = new List<ProductInfo>();
                if (productList != null)
                    list = productList.ToList();

                //获取当前活动下的所有分组
                IList<SWfsSubjectCategory> categoryList = service.GetSWfsSubjectProductList(subjectNo.Trim());
                string tempCategoryNo = string.Empty;
                if (categoryList != null)
                {
                    //判断分组情况，单组情况按以前排序
                    if (categoryList.Count == 1)
                    {
                        #region 无分组排序
                        tempCategoryNo = categoryList[0].CategoryNo;
                        IList<SWfsSubjectProductSort> sortList = service.GetProductSortList(tempCategoryNo);
                        if (sortList != null && sortList.Count() > 0)
                        {
                            sortList = sortList.Where(c => c.ShowChannelType == 0).ToList();
                        }
                        if (sortList != null && sortList.Count > 0)
                        {
                            List<ProductInfo> proSortList = new List<ProductInfo>();
                            List<ProductInfo> proListAll = new List<ProductInfo>();
                            List<ProductInfo> proNoSortList = new List<ProductInfo>();
                            proSortList = (from l in list
                                           join s in sortList on l.ProductNo equals s.ProductNo
                                           orderby s.Sort ascending
                                           select l).ToList();

                            if (proSortList != null && proSortList.Count() > 0)
                            {
                                proListAll.AddRange(proSortList);
                            }
                            if (sortList.Count < list.Count)
                            {
                                proNoSortList = (from t in list
                                                 where !(from b in sortList select b.ProductNo).Contains(t.ProductNo)
                                                 select t).ToList();
                                if (proNoSortList != null && proNoSortList.Count() > 0)
                                {
                                    proListAll.AddRange(proNoSortList);
                                }
                            }
                            list = proListAll;
                        }
                        #endregion
                    }
                    else   // 楼层情况，拆分成单组情况排序，分组顺序依照显示遍历
                    {
                        #region 分组排序
                        //单分组
                        List<ProductInfo> singleList = new List<ProductInfo>();
                        //聚合全部数据
                        List<ProductInfo> ListSum = new List<ProductInfo>();

                        //为防止多次访问数据库，直接查出所有分组数据
                        IList<SWfsSubjectProductSort> sortList = new List<SWfsSubjectProductSort>();
                        foreach (var model in categoryList)
                        {
                            tempCategoryNo += model.CategoryNo + ",";
                            sortList = service.GetProductSortList(tempCategoryNo);
                        }

                        //循环各组，拆分
                        foreach (var model in categoryList)
                        {
                            IList<SWfsSubjectProductSort> TempSortList = sortList.Where(c => c.SubjectNo == model.CategoryNo).ToList();
                            if (TempSortList != null && TempSortList.Count() > 0)
                            {
                                TempSortList = TempSortList.Where(c => c.ShowChannelType == 0).ToList();
                            }
                            if (TempSortList != null && TempSortList.Count > 0)
                            {
                                //查出当前分组里面的所有商品
                                List<ProductInfo> singleALLList = productList.Where(c => c.CategoryNo == model.CategoryNo).ToList();

                                //排序的插入
                                singleList = (from l in singleALLList
                                              join s in TempSortList on l.ProductNo equals s.ProductNo
                                              orderby s.Sort ascending
                                              select l).ToList();
                                ListSum.AddRange(singleList);

                                //判断是否有无排序的，如果有，提取插入
                                if (TempSortList.Count < singleALLList.Count)
                                {
                                    singleList = (from t in singleALLList
                                                  where !(from b in TempSortList select b.ProductNo).Contains(t.ProductNo)
                                                  select t).ToList();
                                    ListSum.AddRange(singleList);
                                }
                            }
                            else
                            {
                                //无分排序直接插入
                                ListSum.AddRange(productList.Where(c => c.CategoryNo == model.CategoryNo).ToList());
                            }
                        }
                        list = ListSum;
                        #endregion
                    }
                }
                //判断当前商品数据，如果有数据那么可以导出，如果无数据，判断返回
                if (list.Count > 0)
                {
                    byte[] fileContents = Encoding.UTF8.GetBytes(ExcelMsg(subjectNo, list));
                    var fileStream = new MemoryStream(fileContents);
                    string excelname = "活动：" + subjectNo + "日期：" + DateTime.Now + ".xls";
                    return File(fileStream, "application/ms-excel", excelname);
                }
                else
                {
                    string TempAlert = string.Format("<script>alert('当前分组无商品数据！');history.back(-1);</script>");
                    return Content(TempAlert, "text/html");
                }
            }
            return View();
        }

        private string ExcelMsg(string subjectNo, IList<ProductInfo> productList)
        {
            #region 获取活动名称
            SWfsSubjectService service = new SWfsSubjectService();
            IList<SWfsSubject> subjectEntity = service.GetSWfsSubjectBySubjectNo(subjectNo);
            string sujectName = string.Empty;
            if (subjectEntity != null && subjectEntity.Count() > 0)
                sujectName = string.IsNullOrEmpty(subjectEntity[0].SubjectName) ? subjectEntity[0].SubjectEnName : subjectEntity[0].SubjectName;
            #endregion
            StringBuilder sb = new StringBuilder("<meta http-equiv=\"content-type\" content=\"application/ms-excel; charset=utf-8\"/>" + "<table width=\"100%\"><tr><td colspan=\"10\" rowspan=\"2\"><h2 width=\"100%\">活动名称：" + sujectName + "</h2></td></tr></table><h2>活动编号：" + subjectNo + "</h2><h2>活动商品</h2><table cellpadding=\"0\" cellspacing=\"0\" border=\"1\"  width=\"758px\" id=\"AccountListTable\" >");

            sb.AppendLine("<tr style=\"background-color:#FFFF00;\">");
            sb.AppendLine("<td><span>分组名称</span></td>");
            sb.AppendLine("<td><span>商品编号</span></td>");
            sb.AppendLine("<td><span>商品名</span></td>");
            sb.AppendLine("<td><span>品牌</span></td> ");
            //sb.AppendLine("<td><span>销售数量</span></td> ");
            //sb.AppendLine("<td><span>一周内是否销售</span></td> ");
            //sb.AppendLine("<td><span>一月内是否销售</span></td> ");
            sb.AppendLine("</tr>");
            foreach (ProductInfo psingle in productList)
            {
                #region 导出excel格式模板
                string brandName = string.IsNullOrEmpty(psingle.BrandEnName) == true ? psingle.BrandCnName : psingle.BrandEnName;
                sb.AppendLine("<tr align=\"left\">");
                sb.AppendLine(String.Format("<td>{0}</td>", psingle.CategoryName));
                sb.AppendLine(String.Format("<td style=\"mso-number-format:\\@;\">{0}</td>", psingle.ProductNo));
                sb.AppendLine(String.Format("<td>{0}</td>", psingle.ProductName));
                sb.AppendLine(String.Format("<td>{0}</td>", brandName));
                //sb.AppendLine(String.Format("<td>{0}</td>", ""));  //销售数量
                //sb.AppendLine(String.Format("<td>{0}</td>", "")); //一周内是否销售
                //sb.AppendLine(String.Format("<td>{0}</td>", ""));  //一月内是否销售              
                sb.AppendLine("</tr>");
                #endregion
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }
        #endregion

        #region 活动订单导出Excel
        public ActionResult OrderToExcel()
        {
            string subjectNo = Request.Params["subjectNo"] ?? "";
            if (!string.IsNullOrEmpty(subjectNo))
            {
                SWfsSubjectService service = new SWfsSubjectService();
                IList<ORderToExcel> OrderList = service.GetOrderBySubject(subjectNo.Trim());

                //判断是否为空，有数据可以导出，无数据无法导出
                if (OrderList != null && OrderList.Count() > 0)
                {
                    byte[] fileContents = Encoding.UTF8.GetBytes(ExcelOrderMsg(OrderList));
                    var fileStream = new MemoryStream(fileContents);
                    string excelname = "活动：" + subjectNo + "日期：" + DateTime.Now + ".xls";
                    return File(fileStream, "application/ms-excel", excelname);
                }
                else
                {
                    string TempAlert = "<script>alert('此活动无订单数据！');history.back(-1);</script>";
                    return Content(TempAlert, "text/html");
                }
            }
            return View();
        }
        private string ExcelOrderMsg(IList<ORderToExcel> OrderList)
        {
            StringBuilder sb = new StringBuilder();
            if (OrderList.Count > 0)
            {
                ORderToExcel order = OrderList.FirstOrDefault();
                sb.AppendLine("<meta http-equiv=\"content-type\" content=\"application/ms-excel; charset=utf-8\"/><h2>活动编号：" + order.SubjectNo + "</h2><h2>活动名称：" + order.SubjectName + "</h2>");
                sb.AppendLine("<table cellpadding=\"0\" cellspacing=\"1\" border=\"1\"  width=\"758px\" >");
                sb.AppendLine("<tr style=\"background-color:#FFFF00;\">");
                sb.AppendLine("<td><span>订单号</span></td>");
                sb.AppendLine("<td><span>订单状态</span></td>");
                sb.AppendLine("<td><span>商品编号</span></td>");
                sb.AppendLine("<td><span>商品名</span></td> ");
                sb.AppendLine("</tr>");
                foreach (var item in OrderList)
                {
                    sb.AppendLine("<tr>");
                    sb.AppendLine(String.Format("<td>{0}</td>", item.OrderNo + "&nbsp;"));
                    sb.AppendLine(String.Format("<td>{0}</td>", item.status));
                    sb.AppendLine(String.Format("<td>{0}</td>", item.productNo + "&nbsp;"));
                    sb.AppendLine(String.Format("<td>{0}</td>", item.ProductName));
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("</table>");
            }

            return sb.ToString();
        }
        #endregion
        #endregion

        #region 活动子类
        //活动子类列表
        public ActionResult SubjectChildList(string subjectNo, string categoryName, int pageIndex = 1)
        {
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            SWfsSubjectService service = new SWfsSubjectService();
            int count = 0;
            int isMulti = 0;
            IList<SWfsSubjectCategory> list = service.GetSubjectCategoryList(subjectNo, categoryName, pageSize, pageIndex, out count);
            ViewBag.SubjectModel = service.GetSubjectInfo(subjectNo);
            ViewBag.PageSize = pageSize;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalCount = count;
            IList<SWfsSubjectCategory> templist = service.GetSubjectCategoryList(subjectNo);
            if (templist != null && templist.Count() == 1)
            {
                string no = templist.FirstOrDefault().CategoryNo;
                //查询该该子类下是否有商品
                isMulti = service.SelectSubjectProductRef(no).Count();
            }
            ViewBag.IsMulti = isMulti;
            ViewBag.Count = templist.Count();
            ViewBag.CategoryName = categoryName;
            ViewBag.CategoryCurrUrl = Request.Url.ToString();
            //ViewBag.SubjectCurrUrl = Request.QueryString["BackSubjectUrl"].ToString();
            return View(list);
        }

        public ActionResult SubjectChildCreate(string subjectNo)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            string name = service.GetSubjectInfo(subjectNo).SubjectName;
            ViewBag.SubjectNo = subjectNo;
            ViewBag.SubjectName = name;
            return View();
        }
        /// <summary>
        /// 添加活动子类
        /// </summary>
        /// <returns></returns>
        public ActionResult AjaxSubjectChildCreate()
        {
            SWfsSubjectCategory subjectcategory = new SWfsSubjectCategory();
            SWfsSubjectService service = new SWfsSubjectService();
            CommonService cs = new CommonService();
            string adPicSize = AppSettingManager.AppSettings["SubjectChildAdPic"].ToString();
            string subjectNo = Request.Params["SubjectNo"].ToString();
            string subjectCategoryNo = DateTime.Now.ToString("yyyyMMdd");
            string str2 = cs.GetNextCounterId("CategoryNo").ToString("000");
            string topPic = Request.Params["TopPic"].ToString(); //获取活动显示状态
            string webHtmlText = HttpUtility.UrlDecode(Request.Params["txtWebHtmlText"].ToString());//获取网页HTML代码
            string mobileHtmlText = HttpUtility.UrlDecode(Request.Params["txtMobileHtmlText"].ToString());//获取移动端HTML代码
            webHtmlText = HttpUtility.HtmlEncode(webHtmlText);
            mobileHtmlText = HttpUtility.HtmlEncode(mobileHtmlText);
            subjectCategoryNo = subjectCategoryNo + str2.Substring(str2.Length - 3, 3);
            subjectcategory.CategoryName = Request.Params["CategoryName"];
            subjectcategory.CategoryNo = subjectCategoryNo;
            subjectcategory.ShowType = Convert.ToInt16(topPic);
            subjectcategory.WebHtmlText = webHtmlText;
            subjectcategory.MobileHtmlText = mobileHtmlText;

            //当活动显示状态为广告图片时
            if (topPic == "1")
            {
                HttpPostedFileBase file = Request.Files["AdPicfile"];
                if (file != null)
                {
                    Dictionary<string, string> adPic = new CommonService().PostImg(file, adPicSize);
                    string adPicNo = adPic.Values.FirstOrDefault();
                    string adPicNokey = adPic.Keys.FirstOrDefault();
                    if (!string.IsNullOrEmpty(adPicNo))
                    {
                        if (adPicNokey != "error")
                        {
                            subjectcategory.AdPic = adPicNo;
                        }
                        else
                        {
                            return Json(new { result = "3", message = adPicNo });
                        }
                    }
                }
                else
                {
                    return Json(new { result = "2", message = "请上传广告图！" });
                }
            }
            subjectcategory.BackgroundPic = "";
            subjectcategory.BuyType = Convert.ToInt16(Request.Params["BuyType"]);
            subjectcategory.DateCreate = DateTime.Now;
            subjectcategory.LogoPic = "";
            subjectcategory.ShowErpCategory = true;
            subjectcategory.SortNo = 1;
            subjectcategory.SubjectNo = subjectNo;
            subjectcategory.IsAutoBottom = 1;
            subjectcategory.MobileIsAutoBottom = 1;
            try
            {
                service.InsertSubjectCategory(subjectcategory);
                return Json(new { result = "1", message = "保存成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "0", message = ex.Message });
            }
        }


        public ActionResult AjaxSubjectCategorySort(string categoryNo, string sortNo)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            bool flag = service.UpdateSort(categoryNo, Convert.ToInt32(sortNo));
            if (flag)
            {
                return Json(new { result = 1, message = "保存成功！" });
            }
            else
            {
                return Json(new { result = 0, message = "保存失败！" });
            }
        }

        //删除活动子类
        public ActionResult AjaxDeleteCategory(string categoryNo, string subjectNo)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            IList<SWfsSubjectCategory> list = service.GetSubjectCategoryList(subjectNo);
            if (list.Count > 2)
            {
                string adPic = service.GetSubjectCategoryModel(categoryNo).AdPic;
                int i = service.DeleteSubjectCategory(categoryNo);
                if (i >= 0)
                {
                    service.DeletePic(adPic);
                    service.DeleteProductBycategoryNo(categoryNo);
                    service.ExecuteDiscountTypeValue(subjectNo, categoryNo);//重新计算活动列表页显示值
                    return Json(new { result = "1", message = "删除成功！" });
                }
                else
                {
                    return Json(new { result = "0", message = "删除失败！" });
                }
            }
            else
            {
                return Json(new { result = "2", message = "手动创建的活动子类只剩一个,不能删除！" });
            }
        }

        public ActionResult SubjectChildEdit(string categoryNo)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            //SWfsSubjectCategory modle = service.GetSubjectCategoryModel(categoryNo);//从缓存中读取数据
            SWfsSubjectCategory modle = service.GetSubjectCategoryModel(categoryNo, false);//重新读取数据
            return View(modle);
        }

        public ActionResult AjaxSubjectChildEdit()
        {
            SWfsSubjectService service = new SWfsSubjectService();
            string adPicSize = AppSettingManager.AppSettings["SubjectChildAdPic"].ToString();
            string categotyNo = Request.Params["CategoryNo"].ToString();
            string categotyName = Request.Params["CategoryName"].ToString();
            short buyType = 0;
            if (!string.IsNullOrEmpty(Request.Params["BuyType"]))
            {
                buyType = Convert.ToInt16(Request.Params["BuyType"]);
            }
            short showType = 1;//活动显示类型（默认为广告图）
            if (!string.IsNullOrEmpty(Request.Params["TopPic"]))
            {
                showType = Convert.ToInt16(Request.Params["TopPic"]);
            }
            string webHtmlText = HttpUtility.UrlDecode(Request.Params["txtWebHtmlText"].ToString());//获取网页HTML代码
            string mobileHtmlText = HttpUtility.UrlDecode(Request.Params["txtMobileHtmlText"].ToString());//获取移动端HTML代码
            webHtmlText = HttpUtility.HtmlEncode(webHtmlText);
            mobileHtmlText = HttpUtility.HtmlEncode(mobileHtmlText);
            SWfsSubjectCategory model = service.GetSubjectCategoryModel(categotyNo);
            string newadPicNo = model.AdPic;
            string oldadPicNo = Request.Params["hidAdPicUp"].ToString();
            if (Request.Files["AdPicfile"] != null && !string.IsNullOrEmpty(Request.Files["AdPicfile"].FileName))
            {
                Dictionary<string, string> adPic = new CommonService().PostImg(Request.Files["AdPicfile"], adPicSize);
                string adPicNo = adPic.Values.FirstOrDefault();
                string adPickey = adPic.Keys.FirstOrDefault();
                if (!string.IsNullOrEmpty(adPicNo))
                {
                    if (adPickey != "error")
                    {
                        service.DeleteImg(oldadPicNo);
                        newadPicNo = adPicNo;
                    }
                    else
                    {
                        return Json(new { result = "AdPicfile", message = adPicNo });
                    }
                }
            }
            model.CategoryNo = categotyNo;
            model.CategoryName = categotyName;
            model.BuyType = buyType;
            model.AdPic = newadPicNo;
            model.ShowType = showType;
            model.WebHtmlText = webHtmlText;
            model.MobileHtmlText = mobileHtmlText;
            if (service.UpdateSubjectCategory(model))
            {
                return Json(new { result = "1", message = "修改成功！" });
            }
            else
            {
                return Json(new { result = "0", message = "修改失败！" });
            }
        }

        public ActionResult AjaxUpdateShowType(string categoryNo, string type)
        {
            SWfsSubjectService service = new SWfsSubjectService();
            SWfsSubjectCategory model = service.GetSubjectCategoryModel(categoryNo);
            model.BuyType = Convert.ToInt16(type);
            if (service.UpdateSubjectCategory(model))
            {
                return Json(new { result = "1", message = "修改成功！" });
            }
            else
            {
                return Json(new { result = "0", message = "修改失败！" });
            }
        }
        #endregion

        #region 图片上传
        #region 判断图片 返回图片名称
        /// <summary>
        /// 判断图片 返回图片名称
        /// 图片为必选，固定宽高图片
        /// 不限可使用，100-200类型不可使用
        /// </summary>
        /// <param name="FormName">接受参数值web页面标签Name</param>
        /// <param name="ConfigName">对应配置文件中配置</param>
        /// <param name="Dir">错误描述信息</param>
        /// <param name="SubjectManage">是否是编辑</param>
        /// <param name="EditMsg">编辑中对应字段值</param>
        /// <param name="imgerror">返回错误信息</param>        
        /// <returns></returns>
        public string GetActiveImageName(string FormName, string ConfigName, string Dir, string SubjectManage, string EditMsg, out string imgerror)
        {
            bool FlagEmpty = false;
            bool ImageConfig = true;
            return GetActiveImageName(FormName, ConfigName, ImageConfig, Dir, SubjectManage, EditMsg, FlagEmpty, out  imgerror);
        }
        /// <summary>
        /// 判断图片 返回图片名称
        /// </summary>
        /// <param name="FormName">接受参数值web页面标签Name</param>
        /// <param name="ConfigName">对应配置文件中配置</param>
        /// <param name="ImageConfig">宽高是否固定</param>
        /// <param name="Dir">错误描述信息</param>
        /// <param name="SubjectManage">是否是编辑</param>
        /// <param name="EditMsg">编辑中对应字段值</param>
        /// <param name="imgerror">返回错误信息</param>
        /// <param name="FlagEmpty">图片是否可空</param>
        /// <returns></returns>
        public string GetActiveImageName(string FormName, string ConfigName, bool ImageConfig, string Dir, string SubjectManage, string EditMsg, bool FlagEmpty, out string imgerror)
        {
            imgerror = string.Empty;
            string ImageName = ImgNameGet(FormName, ConfigName, ImageConfig, out imgerror);
            if (string.IsNullOrEmpty(ImageName))
            {
                if (!string.IsNullOrEmpty(imgerror) && imgerror != "未添加图片")
                {
                    imgerror = string.Format("{0}错误，{1}", Dir, imgerror);
                }
                else if (SubjectManage == "Edit" && !string.IsNullOrEmpty(EditMsg))
                {
                    imgerror = string.Empty;
                    ImageName = EditMsg;
                }
                else
                {
                    if (FlagEmpty)
                    {
                        imgerror = string.Empty;
                        ImageName = EditMsg;
                    }
                    else
                    {
                        imgerror = string.Format("{0}错误，{1}", Dir, imgerror);
                    }
                }
            }
            return ImageName;
        }
        #endregion

        #region 上传图片显示图片名称
        /// <summary>
        /// 上传图片返回名称
        /// </summary>
        /// <param name="ImgName"></param>
        /// <param name="CongifName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private string ImgNameGet(string ImgName, string CongifName, out string error)
        {
            error = string.Empty;
            bool flag = true;
            return ImgNameGet(ImgName, CongifName, flag, out error);
        }
        /// <summary>
        /// 上传图片显示图片名称
        /// </summary>
        /// <param name="ImgName"></param>
        /// <param name="CongifName"></param>
        /// <param name="flag">true图片是固定大小 flase图片大小在区间之内</param>
        /// <param name="error"></param>
        /// <returns></returns>
        private string ImgNameGet(string ImgName, string CongifName, bool flag, out string error)
        {
            error = string.Empty;
            if (Request.Files[ImgName] == null || Request.Files[ImgName].ContentLength == 0)
            {
                error = "未添加图片";
                return string.Empty;
            }
            Dictionary<string, string> belongsSubjectPic = new CommonService().PostImg(Request.Files[ImgName], AppSettingManager.AppSettings[CongifName].ToString(), flag);
            string belongsSubjectPicNo = belongsSubjectPic.Values.FirstOrDefault();
            string belongsSubjectPicNokey = belongsSubjectPic.Keys.FirstOrDefault();
            if (!string.IsNullOrEmpty(belongsSubjectPicNo))
            {
                if (belongsSubjectPicNokey != "error")
                {
                    return belongsSubjectPicNo;
                }
                else
                {
                    error = belongsSubjectPicNo;
                    return string.Empty;
                }
            }
            else
            {
                error = "图片上传失败";
                return string.Empty;
            }
        }
        #endregion

        #endregion

        #region 活动数据统计列表最新需求 20140304 添加
        /// <summary>
        /// 活动数据统计列表最新需求20140228 添加
        /// </summary>
        /// <returns></returns>
        public ActionResult DataStatisticsNew()
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            string subjectNos = string.Empty;
            SubjectOLNewStatisticInfo newsInfo = new SubjectOLNewStatisticInfo();
            SubjectSaleStatistic saleSum = new SubjectSaleStatistic();
            SubjectVisitStatistic visitSum = new SubjectVisitStatistic();
            string txtSubjectNo = Rq.GetStringForm("subjectNo") ?? Rq.GetStringQueryString("subjectNo");
            string range = Rq.GetStringForm("range") ?? Rq.GetIntQueryString("range").ToString();
            string beginTime = Rq.GetStringForm("beginTime") ?? Rq.GetStringQueryString("beginTime");
            string endTime = Rq.GetStringForm("endTime") ?? Rq.GetStringQueryString("endTime");
            string beginTimeSubject = Rq.GetStringForm("beginTimeSubject") ?? Rq.GetStringQueryString("beginTimeSubject");
            string endTimeSubject = Rq.GetStringForm("endTimeSubject") ?? Rq.GetStringQueryString("endTimeSubject");
            string toexcel = Rq.GetStringQueryString("toexcel");
            if (!string.IsNullOrEmpty(txtSubjectNo))
            {
                subjectNos = txtSubjectNo.Replace("\r\n", ",").Replace("\n\r", ",").Replace("\n", ",").Replace("\r", ",").TrimEnd(',').Replace(" ", ",");
            }
            List<SubjectOLNewStatistic> subjectNS = new List<SubjectOLNewStatistic>();
            IList<SubjectInfo> subjectList = subject.SelectSubjectList(beginTimeSubject, endTimeSubject, subjectNos, range);
            IList<SpCategory> erpCategoryList = subject.GetERPCategoryListByParentNo("ROOT");

            #region 读取统计信息
            if (subjectList != null && subjectList.Count() > 0)
            {
                foreach (var item in subjectList)
                {
                    SubjectOLNewStatistic statisticNew = new SubjectOLNewStatistic();
                    SubjectSaleStatistic sale = new SubjectSaleStatistic();
                    SubjectVisitStatistic visitStatistic = new SubjectVisitStatistic();

                    sale = subject.GetSubjectSaleStatisticNew(item.SubjectNo, range, beginTime, endTime);
                    visitStatistic = subject.GetSubjectVisitStatisticNew(item.SubjectNo, range, beginTime, endTime);

                    statisticNew.SaleStatistic = sale ?? new SubjectSaleStatistic();
                    statisticNew.VisitStatistic = visitStatistic ?? new SubjectVisitStatistic();
                    statisticNew.NewSubject = item;
                    IList<SWfsChannel> channelList = subject.GetSubjectChannelSord(item.SubjectNo); //频道
                    if (channelList != null && channelList.Count() > 0)
                    {
                        statisticNew.ChannelList = channelList;
                    }
                    IList<SWfsSubjectCategoryRef> CategoryRefList = subject.GetErpCategoryListBySubjectNo(item.SubjectNo);
                    if (CategoryRefList != null && CategoryRefList.Count() > 0)
                    {
                        statisticNew.CategoryRefList = CategoryRefList;
                    }
                    subjectNS.Add(statisticNew);
                    if (sale != null)
                    {
                        saleSum.Amount = saleSum.Amount + sale.Amount;
                        saleSum.OrderNums = saleSum.OrderNums + sale.OrderNums;
                        saleSum.CostAmount = saleSum.CostAmount + sale.CostAmount;
                        saleSum.StockCount = saleSum.StockCount + sale.StockCount;
                        saleSum.SKUCount = saleSum.SKUCount + sale.SKUCount;
                        saleSum.SaleCount = saleSum.SaleCount + sale.SaleCount;
                        saleSum.SaledSKUCount = saleSum.SaledSKUCount + sale.SaledSKUCount;
                        saleSum.StockTotalAmount = saleSum.StockTotalAmount + sale.StockTotalAmount;
                    }
                    if (visitStatistic != null)
                    {
                        visitSum.PV = visitSum.PV + visitStatistic.PV;
                        visitSum.UV = visitSum.UV + visitStatistic.UV;
                        visitSum.VIP = visitSum.VIP + visitStatistic.VIP;
                        visitSum.GoldenVIP = visitSum.GoldenVIP + visitStatistic.GoldenVIP;
                        visitSum.PlatinaVIP = visitSum.PlatinaVIP + visitStatistic.PlatinaVIP;
                        visitSum.DiamondVIP = visitSum.DiamondVIP + visitStatistic.DiamondVIP;
                        visitSum.OrderNums = visitSum.OrderNums + visitStatistic.OrderNums;
                    }
                }
            }

            newsInfo.SubjectNewStatisticList = subjectNS;
            newsInfo.SaleStatistic = saleSum;
            newsInfo.VisitStatistic = visitSum;
            newsInfo.SubjectNos = subjectNos;
            newsInfo.Range = range;
            newsInfo.BeginTime = beginTime;
            newsInfo.EndTime = endTime;
            newsInfo.BeginTimeSubject = beginTimeSubject;
            newsInfo.EndTimeSubject = endTimeSubject;
            newsInfo.ErpCategoryList = erpCategoryList;
            newsInfo.SubjectCount = subjectList != null ? subjectList.Count() : 0;

            #endregion

            if (!string.IsNullOrEmpty(toexcel))
            {
                switch (toexcel)
                {
                    case "1":
                        subject.GetSubjectDataStatisticsToExcelNew(newsInfo);
                        break;
                }
            }

            return View(newsInfo);
        }
        #endregion

        #region 解析商品表中ProductXmlText字段信息

        /// <summary>
        /// 解析商品表中ProductXmlText字段信息
        /// </summary>
        /// <param name="XMLString"></param>
        /// <returns></returns>
        public string GetProColor(string XMLString)
        {
            if (!string.IsNullOrWhiteSpace(XMLString))
            {
                XElement xElement = XElement.Parse(XMLString, LoadOptions.PreserveWhitespace);
                if (xElement != null)
                {
                    var DynamicAttributes = from node in xElement.Elements()
                                            where node.Name == "DynamicAttributes"
                                            select node;
                    if (DynamicAttributes != null && DynamicAttributes.Count() > 0)
                    {
                        var DynamicAttribute = from node in DynamicAttributes.Elements()
                                               where node.Name == "DynamicAttribute"
                                               select node;

                        foreach (XElement ele in DynamicAttribute)
                        {
                            if (ele.Attribute("AttributeName") != null)
                            {
                                if (ele.Attribute("AttributeName").Value == "ProductAttrColor")
                                {
                                    return ele.Value.Replace("/", ",");
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        #region 以下应对 EP 数据结构变化

        /**
         *【说明】
         * 以下查询数据如有问题，查找原方法： 排除当前方法名前面的 “Outlet” 即可找到之前的方法体
         * 目前所有修改的方法，命名规则都是在之前的方法名基础上加了  “Outlet” 字符
         * */

        #region 活动管理商品 by lijibo  20141002

        /// <summary>
        /// 活动管理商品 by lijibo  20141002
        /// </summary>
        /// <param name="SubjectNo"></param>
        /// <param name="CategoryNo"></param>
        /// <param name="SCategoryNo"></param>
        /// <param name="BrandNo"></param>
        /// <param name="IsShelf"></param>
        /// <param name="ProductNameOrNo"></param>
        /// <param name="Quantity"></param>
        /// <param name="BU"></param>
        /// <param name="pageindex"></param>
        /// <param name="genderStyle"></param>
        /// <returns></returns>
        public ActionResult OutletSubjectProductRef(string SubjectNo, string CategoryNo, string SCategoryNo, string BrandNo, string IsShelf, string ProductNameOrNo, string Quantity, string BU, int pageindex = 1, string genderStyle = "")
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            //2014-4-4如果有此参数表示是网络推广查看添加商品所用，不能操作选项切记，切记
            ViewBag.looktype = 0;
            bool IsRead = false;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["looktype"]) && Request.QueryString["looktype"].Equals("read"))
            {
                ViewBag.looktype = 1;
                IsRead = true;
            }

            //?CategoryNo=A01&SubjectNo=40114127&SCategoryNo=20140114408&CategoryNo1=A01&CategoryNo2=&CategoryNo3=&CategoryNo4=&BrandName=&BrandNo=&BrandNoo=&IsShelf=0&ProductNameOrNo=&Quantity=1
            SWfsSubjectService subject = new SWfsSubjectService();
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            ViewBag.PageIndex = pageindex;
            ViewBag.PageSize = pageSize;

            ViewBag.CategoryNo = CategoryNo;
            ViewBag.SubjectNo = SubjectNo;
            if (!ProductNameOrNo.IsNullOrEmpty())
            {
                ProductNameOrNo = ProductNameOrNo.Trim();
            }
            string categoryNo1 = Request.QueryString["CategoryNo1"];
            string categoryNo2 = Request.QueryString["CategoryNo2"];
            string categoryNo3 = Request.QueryString["CategoryNo3"];
            string categoryNo4 = Request.QueryString["CategoryNo4"];
            string bu = Request.QueryString["BU"];

            ViewBag.CategoryNo1 = categoryNo1 ?? "";
            ViewBag.CategoryNo2 = categoryNo2 ?? "";
            ViewBag.CategoryNo3 = categoryNo3 ?? "";
            ViewBag.CategoryNo4 = categoryNo4 ?? "";
            ViewBag.BU = bu ?? "";
            ViewBag.DepartmentList = subject.GetDepartmentList();
            ViewBag.IsShelf = IsShelf;
            ViewBag.ProductNameOrNo = ProductNameOrNo;
            ViewBag.BrandName = Request.QueryString["BrandName"];

            SubjectInfo smodel = subject.GetSubjectInfo(SubjectNo);
            ViewBag.FirstCategory = subject.GetErpCategoryListBySubjectNo(SubjectNo);
            ViewBag.AllFirstCategory = subject.OutletSelectErpCategoryByParentNo("ROOT");

            ViewBag.Category2 = string.IsNullOrEmpty(categoryNo1) ? null : subject.OutletSelectErpCategoryByParentNo(categoryNo1);
            ViewBag.Category3 = string.IsNullOrEmpty(categoryNo2) ? null : subject.OutletSelectErpCategoryByParentNo(categoryNo2);
            ViewBag.Category4 = string.IsNullOrEmpty(categoryNo3) ? null : subject.OutletSelectErpCategoryByParentNo(categoryNo3);

            ViewBag.SubjectNo = SubjectNo;
            if (!IsRead)
            {
                ViewBag.SCategoryNo = SCategoryNo;
            }
            if (!IsRead)
            {
                ViewBag.CategoryName = subject.GetSubjectCategoryModel(SCategoryNo).CategoryName;
            }
            ViewBag.Quantity = Quantity;
            ViewBag.GenderStyle = genderStyle; //性别
#if DEBUG
            sw.Stop();
            long t1 = sw.ElapsedMilliseconds;
            sw.Reset();
#endif

            IList<SubjectProductRef> productList = new List<SubjectProductRef>();
            IList<SubjectProductRef> _productList = new List<SubjectProductRef>();
            if (IsRead)
            {
                IList<SWfsSubjectCategory> templist = subject.GetSubjectCategoryList(SubjectNo);
                productList = subject.OutletSelectSubjectProductRefListRead(CategoryNo, templist.Select(r => r.CategoryNo).ToList(), BrandNo, IsShelf, ProductNameOrNo, genderStyle);
            }
            else
            {
                productList = subject.OutletSelectSubjectProductRefListII(CategoryNo, SCategoryNo, BrandNo, IsShelf, ProductNameOrNo, genderStyle);
            }

            #region 修改状态数据  by lijibo 20141003
            //修改状态数据  by lijibo 20141003
            productList = subject.TransformationEntityListRef(productList);

            if (!string.IsNullOrEmpty(IsShelf))
            {
                short _isShelf = short.Parse(IsShelf);
                IEnumerable<SubjectProductRef> _NewProductList = productList.Where(c => c.IsShelf == _isShelf);
                if (_NewProductList != null)
                {
                    productList = _NewProductList.ToList();
                }
                else
                {
                    productList = new List<SubjectProductRef>();
                }
            }
            #endregion


            if (productList != null && productList.Count > 0 && !string.IsNullOrEmpty(bu))
            {
                productList = productList.Where(l => l.DepartmentNo != null && l.DepartmentNo != "" && l.DepartmentNo.Length >= 6).Where(p => p.DepartmentNo.Substring(0, 6).Equals(bu)).ToList();
            }


            List<string> productNolist = new List<string>();
            IList<SubjectProductRef> result = new List<SubjectProductRef>();
            SWfsProductService service = new SWfsProductService();
            List<ProductInventory> PIEntityList = new List<ProductInventory>();
            Dictionary<string, int> _dicInventory = new Dictionary<string, int>();
            //List<ProductInventory> productInventoryList= service.GetInventoryByProductNos(productNolist);

            if (Quantity == "1") //查询有库存的商品
            {
                string _productNos = string.Empty;
                foreach (SubjectProductRef sp in productList)
                {
                    _productNos += sp.ProductNo + ",";
                    productNolist.Add(sp.ProductNo);
                    //ProductInventory pinventory = service.GetInventoryByProductNo(sp.ProductNo);
                    //if (pinventory.SumQuantity > 0)
                    //{
                    //    result.Add(sp);
                    //}
                    //PIEntityList.Add(pinventory);
                }

                _dicInventory = service.GetProductsInventoryNew(_productNos);

                foreach (KeyValuePair<string, int> _tempDic in _dicInventory)
                {
                    if (_tempDic.Value > 0)
                    {
                        result.Add(productList.Where(c => c.ProductNo == _tempDic.Key).FirstOrDefault());
                    }
                }
            }
            else if (Quantity == "0") //查询无库存的商品
            {
                string _productNos = string.Empty;
                foreach (SubjectProductRef sp in productList)
                {
                    _productNos += sp.ProductNo + ",";
                    productNolist.Add(sp.ProductNo);
                    //ProductInventory pinventory = service.GetInventoryByProductNo(sp.ProductNo);
                    //if (pinventory.SumQuantity == 0)
                    //{
                    //    result.Add(sp);
                    //}
                    //PIEntityList.Add(pinventory);
                }
                _dicInventory = service.GetProductsInventoryNew(_productNos);
                foreach (KeyValuePair<string, int> _tempDic in _dicInventory)
                {
                    if (_tempDic.Value == 0)
                    {
                        result.Add(productList.Where(c => c.ProductNo == _tempDic.Key).FirstOrDefault());
                    }
                }
            }
            else
            {
                string _productNos = string.Empty;
                foreach (SubjectProductRef sp in productList)
                {
                    _productNos += sp.ProductNo + ",";
                    productNolist.Add(sp.ProductNo);
                    result.Add(sp);

                    //ProductInventory pinventory = service.GetInventoryByProductNo(sp.ProductNo);
                    //PIEntityList.Add(pinventory);
                }
                _dicInventory = service.GetProductsInventoryNew(_productNos);
            }
            ViewBag.ProductInventoryListDic = _dicInventory;
            ViewBag.ProductInventoryList = PIEntityList;
            ViewBag.TotalCount = result.Count();
            productList = result.Skip((pageindex - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.SubjectName = smodel.SubjectName;

#if DEBUG
            sw.Stop();
            long t2 = sw.ElapsedMilliseconds;
            sw.Reset();
#endif
            //优化处理

            if (productList != null && productList.Count > 0)
            {
                Task<Dictionary<string, IList<SWfsSubjectCategoryII>>> task3 = Task.Factory.StartNew(() => service.GetSubjectCategoryByProductNoII(productNolist.ToArray(), "1"));
                ViewBag.DicList = task3.Result;
                //Dictionary<string, IList<SWfsSubjectCategoryII>> dicList = service.GetSubjectCategoryByProductNoII(productNolist.ToArray(), "1");

                Task<Dictionary<string, string>> task4 = Task.Factory.StartNew(() => OutLetExtendService.GetErpProductAgeingMulter(productNolist));
                ViewBag.PErpAgeDic = task4.Result;
            }
#if DEBUG
            sw.Stop();
            long t3 = sw.ElapsedMilliseconds;
            sw.Reset();
#endif
            return View(productList);
        }

        #endregion

        #region 商品列表 by lijibo 20141002

        /// <summary>
        /// 商品列表 by lijibo 20141002 
        /// </summary>
        /// <param name="SubjectNo">活动编号</param>
        /// <param name="CategoryNo">品类</param>
        /// <param name="SCategoryNo">活动分类编号</param>
        /// <param name="BrandNo">品牌编号</param>
        /// <param name="IsShelf">上架状态</param>
        /// <param name="ProductNameOrNo">关键词（商品编号/商品名称/货号）</param>
        /// <param name="isbatch">是否批量</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="IsLoad">是否加载</param>
        /// <param name="productNos">多个商品编号以逗号分隔</param>
        /// <returns></returns>
        public ActionResult OutletProductList(string SubjectNo, string CategoryNo, string SCategoryNo, string BrandNo, string IsShelf, string ProductNameOrNo, string BU, bool isbatch = false, int pageindex = 1, bool IsLoad = true, string productNos = "", string GenderStyle = "")
        {
            SWfsSubjectService subject = new SWfsSubjectService();
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            if (!string.IsNullOrEmpty(ProductNameOrNo))
            {
                ProductNameOrNo = ProductNameOrNo.Trim();
            }
            string categoryNo1 = string.Empty;
            string categoryNo2 = string.Empty;
            string categoryNo3 = string.Empty;
            string categoryNo4 = string.Empty;
            string brandName = string.Empty;
            string bu = string.Empty;
            StringBuilder sb = new StringBuilder();

            SubjectInfo smodel = subject.GetSubjectInfo(SubjectNo);
            IList<SWfsSubjectCategoryRef> categoryList = subject.GetErpCategoryListBySubjectNo(SubjectNo);
            //所有的分类
            ViewBag.AllFirstCategory = subject.OutletSelectErpCategoryByParentNo("ROOT");

            ViewBag.Category2 = string.IsNullOrEmpty(categoryNo1) ? null : subject.OutletSelectErpCategoryByParentNo(categoryNo1);
            ViewBag.Category3 = string.IsNullOrEmpty(categoryNo2) ? null : subject.OutletSelectErpCategoryByParentNo(categoryNo2);
            ViewBag.Category4 = string.IsNullOrEmpty(categoryNo3) ? null : subject.OutletSelectErpCategoryByParentNo(categoryNo3);
            ViewBag.DepartmentList = subject.GetDepartmentList();
            RecordPage<ProductInfo> productList = new RecordPage<ProductInfo>();
            if (!isbatch)
            {
                categoryNo1 = Request.QueryString["CategoryNo1"] ?? "";
                categoryNo2 = Request.QueryString["CategoryNo2"] ?? "";
                categoryNo3 = Request.QueryString["CategoryNo3"] ?? "";
                categoryNo4 = Request.QueryString["CategoryNo4"] ?? "";
                brandName = Request.QueryString["BrandName"] ?? "";
                bu = Request.QueryString["BU"] ?? "";
                if (!IsLoad)
                {
                    IList<string> categoryNoList = categoryList.Select(c => c.Category).ToList();
                    productList = subject.OutletGetSWfsSubjectProduct(CategoryNo, categoryNoList, SubjectNo, BrandNo, IsShelf, ProductNameOrNo, pageindex, pageSize, GenderStyle);
                    if (productList != null && productList.Items.Count() > 0 && !string.IsNullOrEmpty(bu))
                    {
                        IEnumerable<ProductInfo> list = productList.Items.Where(l => l.DepartmentNo != null && l.DepartmentNo != "" && l.DepartmentNo.Length >= 6).Where(p => p.DepartmentNo.Substring(0, 6).Equals(bu));
                        productList = PageConvertor.Convert(pageindex, pageSize, list);
                    }
                }
            }
            else
            {
                string cacheKey = string.Empty;
                cacheKey = String.Format("ProductListBatchAddSubjectNo{0}SCategoryNo{1}", SubjectNo, SCategoryNo);
                string[] ProductNoStr = !string.IsNullOrEmpty(productNos) ? System.Web.HttpUtility.UrlDecode(productNos).Replace("\r\n", ",").Replace("\n\r", ",").Replace("\n", ",").Replace("\r", ",").TrimEnd(',').Trim().Split(',') : null;
                if (ProductNoStr != null && ProductNoStr.Length > 0)
                {
                    RedisCacheProvider.Instance.Set<string[]>(cacheKey, ProductNoStr, TimeSpan.FromDays(1));
                    //EnyimMemcachedClient.Instance.Set(cacheKey, ProductNoStr, TimeSpan.FromDays(1));
                }
                else
                {
                    ProductNoStr = RedisCacheProvider.Instance.Get<string[]>(cacheKey) != null ? RedisCacheProvider.Instance.Get<string[]>(cacheKey) : null;
                    //ProductNoStr = EnyimMemcachedClient.Instance.Get(cacheKey) != null ? (string[])EnyimMemcachedClient.Instance.Get(cacheKey) : null;
                }
                if (ProductNoStr != null && ProductNoStr.Length > 0)
                {
                    for (int i = 0; i < ProductNoStr.Length; i++)
                    {
                        sb.Append(ProductNoStr[i]);
                        if (i < (ProductNoStr.Length - 1))
                        {
                            sb.Append(",");
                        }

                    }
                    productList = subject.OutletGetSWfsSubjectProductNew(SubjectNo, ProductNoStr.ToArray(), pageindex, pageSize);
                }
            }

            #region 修改状态数据  by lijibo 20141003

            //修改状态数据  by lijibo 20141003
            productList.Items = TransformationEntityList(productList.Items);
            if (!string.IsNullOrEmpty(IsShelf))
            {
                //short _isShelf = short.Parse(IsShelf);
                //IEnumerable<ProductInfo> _NewProductList = productList.Items.Where(c => c.IsShelf == _isShelf);
                //if (_NewProductList != null)
                //{
                //    productList.Items = _NewProductList.ToList();
                //}
                //else
                //{
                //    productList.Items = new List<ProductInfo>();
                //}
            }

            #endregion

            ViewBag.CategoryNo1 = categoryNo1 ?? "";
            ViewBag.CategoryNo2 = categoryNo2 ?? "";
            ViewBag.CategoryNo3 = categoryNo3 ?? "";
            ViewBag.CategoryNo4 = categoryNo4 ?? "";
            ViewBag.IsShelf = IsShelf;
            ViewBag.ProductNameOrNo = ProductNameOrNo;
            ViewBag.BrandName = brandName;
            ViewBag.BrandNo = BrandNo;
            ViewBag.IsBatch = isbatch ? "1" : "0";
            ViewBag.BatchProductNo = sb.ToString();
            ViewBag.GenderStyle = GenderStyle;
            ViewBag.BU = bu ?? "";

            //该活动下的一级分类
            ViewBag.FirstCategory = categoryList;

            ViewBag.SubjectNo = SubjectNo;
            ViewBag.SCategoryNo = SCategoryNo;
            ViewBag.CategoryName = subject.GetSubjectCategoryModel(SCategoryNo).CategoryName;
            ViewBag.SubjectName = smodel.SubjectName;


            //优化处理
            List<string> productNolist = new List<string>();
            if (productList != null && productList.TotalItems > 0)
            {
                foreach (var item in productList.Items)
                {
                    productNolist.Add(item.ProductNo);
                }
                SWfsProductService service = new SWfsProductService();
                Dictionary<string, IList<SWfsSubjectCategoryII>> dicList = service.GetSubjectCategoryByProductNoII(productNolist.ToArray(), "1");
                ViewBag.DicList = dicList;

                /**********   此处库龄需要修改       **********/
                Dictionary<string, string> productErpAgeDic = OutLetExtendService.GetErpProductAgeingMulter(productNolist);
                ViewBag.PErpAgeDic = productErpAgeDic;
            }
            return View(productList);
        }
        #endregion

        #region 活动商品导出Excel   by lijibo  20141002
        /// <summary>
        /// 活动商品导出Excel  by lijibo
        /// </summary>
        /// <returns></returns>
        public ActionResult OutletProjectToExcel()
        {
            string subjectNo = Request.Params["subjectNo"] ?? "";
            if (!string.IsNullOrEmpty(subjectNo))
            {
                SWfsSubjectService service = new SWfsSubjectService();
                IList<ProductInfo> productList = service.OutletGetProductList(subjectNo.Trim());
                List<ProductInfo> list = new List<ProductInfo>();
                if (productList != null)
                    list = productList.ToList();

                //获取当前活动下的所有分组
                IList<SWfsSubjectCategory> categoryList = service.GetSWfsSubjectProductList(subjectNo.Trim());
                string tempCategoryNo = string.Empty;
                if (categoryList != null)
                {
                    //判断分组情况，单组情况按以前排序
                    if (categoryList.Count == 1)
                    {
                        #region 无分组排序
                        tempCategoryNo = categoryList[0].CategoryNo;
                        IList<SWfsSubjectProductSort> sortList = service.GetProductSortList(tempCategoryNo);
                        if (sortList != null && sortList.Count() > 0)
                        {
                            sortList = sortList.Where(c => c.ShowChannelType == 0).ToList();
                        }
                        if (sortList != null && sortList.Count > 0)
                        {
                            List<ProductInfo> proSortList = new List<ProductInfo>();
                            List<ProductInfo> proListAll = new List<ProductInfo>();
                            List<ProductInfo> proNoSortList = new List<ProductInfo>();
                            proSortList = (from l in list
                                           join s in sortList on l.ProductNo equals s.ProductNo
                                           orderby s.Sort ascending
                                           select l).ToList();

                            if (proSortList != null && proSortList.Count() > 0)
                            {
                                proListAll.AddRange(proSortList);
                            }
                            if (sortList.Count < list.Count)
                            {
                                proNoSortList = (from t in list
                                                 where !(from b in sortList select b.ProductNo).Contains(t.ProductNo)
                                                 select t).ToList();
                                if (proNoSortList != null && proNoSortList.Count() > 0)
                                {
                                    proListAll.AddRange(proNoSortList);
                                }
                            }
                            if (proListAll.Count() > 0)
                            {
                                list = proListAll;
                            }
                        }
                        #endregion
                    }
                    else   // 楼层情况，拆分成单组情况排序，分组顺序依照显示遍历
                    {
                        #region 分组排序
                        //单分组
                        List<ProductInfo> singleList = new List<ProductInfo>();
                        //聚合全部数据
                        List<ProductInfo> ListSum = new List<ProductInfo>();

                        //为防止多次访问数据库，直接查出所有分组数据
                        IList<SWfsSubjectProductSort> sortList = new List<SWfsSubjectProductSort>();
                        foreach (var model in categoryList)
                        {
                            tempCategoryNo += model.CategoryNo + ",";
                            sortList = service.GetProductSortList(tempCategoryNo);
                        }

                        //循环各组，拆分
                        foreach (var model in categoryList)
                        {
                            IList<SWfsSubjectProductSort> TempSortList = sortList.Where(c => c.SubjectNo == model.CategoryNo).ToList();
                            if (TempSortList != null && TempSortList.Count() > 0)
                            {
                                TempSortList = TempSortList.Where(c => c.ShowChannelType == 0).ToList();
                            }
                            if (TempSortList != null && TempSortList.Count > 0)
                            {
                                //查出当前分组里面的所有商品
                                List<ProductInfo> singleALLList = productList.Where(c => c.CategoryNo == model.CategoryNo).ToList();

                                //排序的插入
                                singleList = (from l in singleALLList
                                              join s in TempSortList on l.ProductNo equals s.ProductNo
                                              orderby s.Sort ascending
                                              select l).ToList();
                                ListSum.AddRange(singleList);

                                //判断是否有无排序的，如果有，提取插入
                                if (TempSortList.Count < singleALLList.Count)
                                {
                                    singleList = (from t in singleALLList
                                                  where !(from b in TempSortList select b.ProductNo).Contains(t.ProductNo)
                                                  select t).ToList();
                                    ListSum.AddRange(singleList);
                                }
                            }
                            else
                            {
                                //无分排序直接插入
                                ListSum.AddRange(productList.Where(c => c.CategoryNo == model.CategoryNo).ToList());
                            }
                        }
                        if (ListSum.Count() > 0)
                        {
                            list = ListSum;
                        }
                        #endregion
                    }
                }
                //判断当前商品数据，如果有数据那么可以导出，如果无数据，判断返回
                if (list.Count > 0)
                {
                    byte[] fileContents = Encoding.UTF8.GetBytes(ExcelMsg(subjectNo, list));
                    var fileStream = new MemoryStream(fileContents);
                    string excelname = "活动：" + subjectNo + "日期：" + DateTime.Now + ".xls";
                    return File(fileStream, "application/ms-excel", excelname);



                    ////第一种:使用FileContentResult
                    //byte[] fileContents = Encoding.Default.GetBytes(sbHtml.ToString());
                    //return File(fileContents, "application/ms-excel", "fileContents.xls");

                    ////第二种:使用FileStreamResult
                    //var fileStream = new MemoryStream(fileContents);
                    //return File(fileStream, "application/ms-excel", "fileStream.xls");


                    //Response.Clear();
                    //Response.Buffer = false;
                    //Response.ContentEncoding = Encoding.UTF8;
                    //Response.Charset = "utf-8";
                    //Response.ContentType = "application/ms-excel";
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(excelname, Encoding.UTF8) + ".xls");
                    //Response.Write(ExcelMsg(subjectNo, list));
                    //Response.Flush();
                    //Response.End();
                }
                else
                {
                    string TempAlert = string.Format("<script>alert('当前分组无商品数据！');history.back(-1);</script>");
                    return Content(TempAlert, "text/html");
                }
            }
            return View();
        }


        #endregion

        #region 商品可视化(Web)   by lijibo 20141004

        /// <summary>
        ///商品可视化        
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="SCategoryNo"></param>
        /// <returns></returns>
        public ActionResult OutletProductView(string subjectNo, string SCategoryNo)
        {
            string sortType = Request["SortType"];    //排序方式
            string isAutoBottom = Request["IsAutoBottom"];   // 1 Yes  0  No
            string isOne = Request["isOne"];

            int showCount = AppSettingManager.AppSettings["SubjectProductShowCount"].ToInt(500);
            SWfsSubjectService service = new SWfsSubjectService();

            string ShowType = "0";
            List<SubjectProductRef> list1 = service.OutletSelectSubjectProductRefList(SCategoryNo);

            //修改状态数据  by lijibo 20141004
            list1 = service.TransformationEntityListRef(list1).ToList();

            List<SubjectProductRef> list = list1;

            //查询上次是已什么方式排序的
            SWfsSubjectCategory CategoryEntity = service.GetSubjectCategoryModel(SCategoryNo, true);
            if (string.IsNullOrEmpty(isOne))
            {
                IList<SWfsSubjectProductSort> sortList = new List<SWfsSubjectProductSort>();
                if (string.IsNullOrWhiteSpace(CategoryEntity.SortRuleType))
                {
                    sortList = service.GetProductSortList(SCategoryNo);
                    if (sortList != null && sortList.Count() > 0)
                    {
                        sortList = sortList.Where(c => c.ShowChannelType == 0).ToList();
                    }
                }
                else
                {
                    sortList = service.GetProductSortList(SCategoryNo, short.Parse(ShowType));
                }
                if (sortList != null && sortList.Count > 0)
                {
                    List<SubjectProductRef> proSortList = new List<SubjectProductRef>();
                    List<SubjectProductRef> proListAll = new List<SubjectProductRef>();
                    List<SubjectProductRef> proNoSortList = new List<SubjectProductRef>();
                    proSortList = (from l in list
                                   join s in sortList on l.ProductNo equals s.ProductNo
                                   orderby s.Sort ascending
                                   select l).ToList();

                    if (proSortList != null && proSortList.Count() > 0)
                    {
                        proListAll.AddRange(proSortList);
                    }

                    #region 商品在排序数据中的数量  by zhangwei 20141212 edit
                    int sortDataNum = 0;
                    sortDataNum = (from s in sortList
                                   join l in list on s.ProductNo equals l.ProductNo
                                   orderby s.Sort ascending
                                   select l).ToList().Count();
                    #endregion

                    if (sortDataNum < list.Count)
                    {
                        proNoSortList = (from t in list
                                         where !(from b in sortList select b.ProductNo).Contains(t.ProductNo)
                                         select t).ToList();
                        if (proNoSortList != null && proNoSortList.Count() > 0)
                        {
                            proListAll.AddRange(proNoSortList);
                        }
                    }
                    if (proListAll.Count() > 0)
                    {
                        list = proListAll;
                    }
                }
            }
            #region 显示库存 by zhangwei 20140625 开启使用
            SWfsProductService productService = new SWfsProductService();
            ProductInventory pin = new ProductInventory();
            foreach (var product in list)
            {
                pin = productService.GetInventoryByProductNo(product.ProductNo);
                product.Quantity = pin.SumQuantity;
                product.LockQuantity = pin.SumLockQuantity;
            }
            #endregion

            list = list.Distinct(new ComparerSubjectProductRef()).ToList();
            Dictionary<string, string> BrandDic = new Dictionary<string, string>();
            Dictionary<string, string> CategoryDic = new Dictionary<string, string>();
            string CategoryNos = string.Empty;
            string productNos = string.Empty;
            string ColorAll = string.Empty;
            string ColorData = string.Empty;
            string isSortData = string.Empty;
            string isIAB = string.Empty;
            if (list != null && list.Count() > 0)
            {
                ViewBag.SortRuleType = CategoryEntity.SortRuleType;
                ViewBag.IAB = CategoryEntity.IsAutoBottom;

                if (string.IsNullOrEmpty(sortType))
                    sortType = "";
                else
                    ViewBag.SortRuleType = sortType;

                if (string.IsNullOrEmpty(isAutoBottom))
                    isAutoBottom = ViewBag.IAB.ToString();
                else
                    ViewBag.IAB = short.Parse(isAutoBottom);
                isSortData = sortType;

                if (string.IsNullOrEmpty(isSortData))
                    isSortData = "";
                string[] sortData = isSortData.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (sortData.Length == 2)
                {
                    list = SearchSortList(list, sortData);
                }
                for (int i = 0; i < list.Count(); i++)
                {
                    #region 品牌
                    var singleDic = BrandDic.Where(c => c.Key == list[i].BrandNo);
                    if (singleDic.Count() == 0)
                    {
                        string tempBrandName = string.IsNullOrEmpty(list[i].BrandEnName) ? list[i].BrandCnName.Trim().Replace(" ", "_") : list[i].BrandEnName.Trim().Replace(" ", "_");
                        BrandDic.Add(list[i].BrandNo, tempBrandName);   // 品牌
                    }
                    #endregion

                    #region 组合商品编码、品类编码
                    if (CategoryNos.IndexOf(list[i].CategoryNo.Substring(0, 6) + ",") < 0)
                    {
                        CategoryNos += list[i].CategoryNo.Substring(0, 6) + ",";
                    }
                    productNos += list[i].ProductNo + ",";
                    #endregion

                    #region 组合颜色  （在新的数据结构中 无法区分色系，因此此块暂时不做）
                    //string tempColor = GetProColor(list[i].ProductXmlText);

                    //if (!string.IsNullOrEmpty(tempColor))
                    //{
                    //    if (tempColor.IndexOf(",") > 0)
                    //    {
                    //        string[] _tempColor = tempColor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //        for (int j = 0; j < _tempColor.Length; j++)
                    //        {
                    //            if (ColorAll.IndexOf(_tempColor[j] + ",") < 0)
                    //            {
                    //                ColorAll += _tempColor[j] + ",";
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (ColorAll.IndexOf(tempColor + ",") < 0)
                    //        {
                    //            ColorAll += tempColor + ",";
                    //        }
                    //    }
                    //}
                    #endregion
                }

                #region 判断类型根据查询做处理

                IList<WfsErpCategory> CategoryEntityList = service.GetWfsErpCategoryListByCategoryNo(CategoryNos);
                if (!string.IsNullOrEmpty(isSortData))
                {
                    if (sortData[0] == "8")   //色系   （在新的数据结构中 无法区分色系，因此此块暂时不做）
                    {
                        //string[] _tempColor = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        //List<WfsPrimaryColor> _ColorList = new List<WfsPrimaryColor>();
                        //IList<WfsPrimaryColor> ColorList = service.GetWfsPrimaryColorListByColorID(sortData[1]);
                        //for (int _j = 0; _j < _tempColor.Length; _j++)
                        //{
                        //    foreach (WfsPrimaryColor model in ColorList)
                        //    {
                        //        if (model.PrimaryColorId.ToString() == _tempColor[_j])
                        //        {
                        //            string _color = model.PrimaryColorId + "," + model.PrimaryColorName + "|";
                        //            ColorData += _color;
                        //        }
                        //    }
                        //}
                    }
                    if (sortData[0] == "7")   // 品类
                    {
                        List<WfsErpCategory> _CategoryEntityList = new List<WfsErpCategory>();
                        string[] sortcategory = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < sortcategory.Length; j++)
                        {
                            _CategoryEntityList.Add(CategoryEntityList.FirstOrDefault(c => c.CategoryNo == sortcategory[j]));
                        }
                        CategoryEntityList = _CategoryEntityList;
                    }

                    if (sortData[0] == "4")  //  收藏
                    {

                    }
                }
                #endregion

                #region 处理品类信息

                for (int i = 0; i < CategoryEntityList.Count(); i++)
                {
                    var singleDic = CategoryDic.Where(c => c.Key == CategoryEntityList[i].CategoryNo);
                    if (singleDic.Count() == 0)
                    {
                        CategoryDic.Add(CategoryEntityList[i].CategoryNo, CategoryEntityList[i].CategoryName);   // 品类
                    }
                }
                #endregion

            }
            else
            {
                ViewBag.SortRuleType = null;
                ViewBag.IAB = short.Parse("0");
            }
            if (isAutoBottom == "1")        // 是否沉底
            {
                list1 = new List<Entity.Extenstion.Outlet.SubjectProductRef>();
                List<SubjectProductRef> outProductsList = list.Where(c => c.Quantity <= 0).ToList();
                foreach (SubjectProductRef model in outProductsList)
                {
                    list.Remove(model);
                    list1.Add(model);
                }
                list.AddRange(list1);
            }
            ViewBag.productNos = productNos;
            ViewBag.Brand = BrandDic;
            ViewBag.Category = CategoryDic;
            ViewBag.ColorAll = ColorAll; // 所有商品的颜色（不重复）
            ViewBag.ColorData = ColorData;  //一级颜色
            return View(list);
        }
        #endregion

        #region 商品可视化（mobile） by lijibo 20141004
        /// <summary>
        /// 商品可视化（mobile） by lijibo 20141004  
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="SCategoryNo"></param>
        /// <returns></returns>
        public ActionResult OutletProductMobileView(string subjectNo, string SCategoryNo)
        {
            string sortType = Request["SortType"];    //排序方式
            string isAutoBottom = Request["IsAutoBottom"];   // 1 Yes  0  No
            string isOne = Request["isOne"];   // 1 Yes  0  No
            int showCount = AppSettingManager.AppSettings["SubjectProductShowCount"].ToInt(500);
            SWfsSubjectService service = new SWfsSubjectService();

            string ShowType = "1";
            List<SubjectProductRef> list1 = service.OutletSelectSubjectProductRefList(SCategoryNo);

            //修改状态数据  by lijibo 20141004
            list1 = service.TransformationEntityListRef(list1).ToList();
            List<SubjectProductRef> list = list1;


            //查询上次是已什么方式排序的
            SWfsSubjectCategory CategoryEntity = service.GetSubjectCategoryModel(SCategoryNo, true);
            if (string.IsNullOrEmpty(isOne))
            {
                IList<SWfsSubjectProductSort> sortList = new List<SWfsSubjectProductSort>();
                if (string.IsNullOrWhiteSpace(CategoryEntity.MobileSortRuleType))  //string.IsNullOrWhiteSpace(CategoryEntity.SortRuleType)
                {
                    sortList = service.GetProductSortList(SCategoryNo);
                    if (sortList != null && sortList.Count() > 0)
                    {
                        sortList = sortList.Where(c => c.ShowChannelType == 1).ToList();
                    }
                }
                else
                {
                    sortList = service.GetProductSortList(SCategoryNo, short.Parse(ShowType));
                }
                List<SubjectProductRef> tmplList = list.ToList();
                if (sortList != null && sortList.Count > 0)
                {
                    List<SubjectProductRef> proSortList = new List<SubjectProductRef>();
                    List<SubjectProductRef> proListAll = new List<SubjectProductRef>();
                    List<SubjectProductRef> proNoSortList = new List<SubjectProductRef>();
                    proSortList = (from l in list
                                   join s in sortList on l.ProductNo equals s.ProductNo
                                   orderby s.Sort ascending
                                   select l).ToList();

                    if (proSortList != null && proSortList.Count() > 0)
                    {
                        proListAll.AddRange(proSortList);
                    }

                    #region 商品在排序数据中的数量  by zhangwei 20141212 edit
                    int sortDataNum = 0;
                    sortDataNum = (from s in sortList
                                   join l in list on s.ProductNo equals l.ProductNo
                                   orderby s.Sort ascending
                                   select l).ToList().Count();
                    #endregion

                    if (sortDataNum < list.Count)
                    {
                        proNoSortList = (from t in list
                                         where !(from b in sortList select b.ProductNo).Contains(t.ProductNo)
                                         select t).ToList();
                        if (proNoSortList != null && proNoSortList.Count() > 0)
                        {
                            proListAll.AddRange(proNoSortList);
                        }
                    }
                    if (proListAll.Count() > 0)
                    {
                        list = proListAll;
                    }
                }
            }

            #region 显示库存 by zhangwei 20140625 开启使用
            SWfsProductService productService = new SWfsProductService();
            ProductInventory pin = new ProductInventory();
            foreach (var product in list)
            {
                pin = productService.GetInventoryByProductNo(product.ProductNo);
                product.Quantity = pin.SumQuantity;
                product.LockQuantity = pin.SumLockQuantity;
            }
            #endregion

            list = list.Distinct(new ComparerSubjectProductRef()).ToList();

            Dictionary<string, string> BrandDic = new Dictionary<string, string>();
            Dictionary<string, string> CategoryDic = new Dictionary<string, string>();
            string CategoryNos = string.Empty;
            string productNos = string.Empty;
            string ColorAll = string.Empty;
            string ColorData = string.Empty;
            string isSortData = string.Empty;
            string isIAB = string.Empty;
            if (list != null && list.Count() > 0)
            {
                ViewBag.SortRuleType = CategoryEntity.MobileSortRuleType;
                ViewBag.IAB = CategoryEntity.MobileIsAutoBottom;


                if (string.IsNullOrEmpty(sortType))
                    sortType = "";
                else
                    ViewBag.SortRuleType = sortType;

                if (string.IsNullOrEmpty(isAutoBottom))
                    isAutoBottom = ViewBag.IAB.ToString();
                else
                    ViewBag.IAB = short.Parse(isAutoBottom);
                isSortData = sortType;

                if (string.IsNullOrEmpty(isSortData))
                    isSortData = "";
                string[] sortData = isSortData.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (sortData.Length == 2)
                {
                    list = SearchSortList(list, sortData);
                }
                for (int i = 0; i < list.Count(); i++)
                {
                    #region 品牌
                    var singleDic = BrandDic.Where(c => c.Key == list[i].BrandNo);
                    if (singleDic.Count() == 0)
                    {
                        string tempBrandName = string.IsNullOrEmpty(list[i].BrandEnName) ? list[i].BrandCnName.Trim().Replace(" ", "_") : list[i].BrandEnName.Trim().Replace(" ", "_");
                        BrandDic.Add(list[i].BrandNo, tempBrandName);   // 品牌
                    }
                    #endregion

                    #region 组合商品编码、品类编码
                    if (CategoryNos.IndexOf(list[i].CategoryNo.Substring(0, 6) + ",") < 0)
                    {
                        CategoryNos += list[i].CategoryNo.Substring(0, 6) + ",";
                    }
                    productNos += list[i].ProductNo + ",";
                    #endregion

                    #region 组合颜色   （在新的数据结构中 无法区分色系，因此此块暂时不做）
                    //string tempColor = GetProColor(list[i].ProductXmlText);

                    //if (!string.IsNullOrEmpty(tempColor))
                    //{
                    //    if (tempColor.IndexOf(",") > 0)
                    //    {
                    //        string[] _tempColor = tempColor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //        for (int j = 0; j < _tempColor.Length; j++)
                    //        {
                    //            if (ColorAll.IndexOf(_tempColor[j] + ",") < 0)
                    //            {
                    //                ColorAll += _tempColor[j] + ",";
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (ColorAll.IndexOf(tempColor + ",") < 0)
                    //        {
                    //            ColorAll += tempColor + ",";
                    //        }
                    //    }
                    //}
                    #endregion
                }

                #region 判断类型根据查询做处理

                IList<WfsErpCategory> CategoryEntityList = service.GetWfsErpCategoryListByCategoryNo(CategoryNos);
                if (!string.IsNullOrEmpty(isSortData))
                {
                    if (sortData[0] == "8")   //色系  （在新的数据结构中 无法区分色系，因此此块暂时不做）
                    {
                        //string[] _tempColor = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        //List<WfsPrimaryColor> _ColorList = new List<WfsPrimaryColor>();
                        //IList<WfsPrimaryColor> ColorList = service.GetWfsPrimaryColorListByColorID(sortData[1]);
                        //for (int _j = 0; _j < _tempColor.Length; _j++)
                        //{
                        //    foreach (WfsPrimaryColor model in ColorList)
                        //    {
                        //        if (model.PrimaryColorId.ToString() == _tempColor[_j])
                        //        {
                        //            string _color = model.PrimaryColorId + "," + model.PrimaryColorName + "|";
                        //            ColorData += _color;
                        //        }
                        //    }
                        //}
                    }
                    if (sortData[0] == "7")   // 品类
                    {
                        List<WfsErpCategory> _CategoryEntityList = new List<WfsErpCategory>();
                        string[] sortcategory = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < sortcategory.Length; j++)
                        {
                            _CategoryEntityList.Add(CategoryEntityList.FirstOrDefault(c => c.CategoryNo == sortcategory[j]));
                        }
                        CategoryEntityList = _CategoryEntityList;
                    }

                }
                #endregion

                #region 处理品类信息

                for (int i = 0; i < CategoryEntityList.Count(); i++)
                {
                    var singleDic = CategoryDic.Where(c => c.Key == CategoryEntityList[i].CategoryNo);
                    if (singleDic.Count() == 0)
                    {
                        CategoryDic.Add(CategoryEntityList[i].CategoryNo, CategoryEntityList[i].CategoryName);   // 品类
                    }
                }
                #endregion

            }
            else
            {
                ViewBag.SortRuleType = null;
                ViewBag.IAB = short.Parse("0");
            }
            if (isAutoBottom == "1")        // 是否沉底
            {
                list1 = new List<Entity.Extenstion.Outlet.SubjectProductRef>();
                List<SubjectProductRef> outProductsList = list.Where(c => c.Quantity <= 0).ToList();
                foreach (SubjectProductRef model in outProductsList)
                {
                    list.Remove(model);
                    list1.Add(model);
                }
                list.AddRange(list1);
            }
            ViewBag.productNos = productNos;
            ViewBag.Brand = BrandDic;
            ViewBag.Category = CategoryDic;
            ViewBag.ColorAll = ColorAll; // 所有商品的颜色（不重复）
            ViewBag.ColorData = ColorData;  //一级颜色
            return View(list);
        }
        #endregion


        #region 排序switch方法  by lijibo 20141004

        /// <summary>
        /// 排序方法  by lijibo 20141004
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        public List<SubjectProductRef> OutletSearchSortList(List<SubjectProductRef> list, string[] sortData)
        {
            switch (Convert.ToInt32(sortData[0]))
            {
                #region 按价格

                case (int)SubjectSortRuleType.price://"price":    //按价格
                    if (sortData[1] == "0")
                        list = list.OrderByDescending(c => c.LimitedVipPrice).ToList();
                    else
                        list = list.OrderBy(c => c.LimitedVipPrice).ToList();
                    break;

                #endregion

                #region 按库存

                case (int)SubjectSortRuleType.stock://"stock":   //按库存
                    if (sortData[1] == "0")
                        list = list.OrderByDescending(c => c.Quantity).ToList();
                    else
                        list = list.OrderBy(c => c.Quantity).ToList();
                    break;

                #endregion

                #region 按折扣

                case (int)SubjectSortRuleType.discount://"discount":  //
                    if (sortData[1] == "0")
                        list = list.OrderByDescending(c => c.MarketPrice == 0 ? 0 : Math.Round((c.LimitedVipPrice * 10) / c.MarketPrice, 1, MidpointRounding.AwayFromZero)).ToList();
                    else
                        list = list.OrderBy(c => c.MarketPrice == 0 ? 0 : Math.Round((c.LimitedVipPrice * 10) / c.MarketPrice, 1, MidpointRounding.AwayFromZero)).ToList();
                    break;

                #endregion

                #region 上架时间

                case (int)SubjectSortRuleType.putawaytime://"putawaytime":  //上架时间
                    if (sortData[1] == "0")
                        list = list.OrderByDescending(c => c.DateShelf).ToList();
                    else
                        list = list.OrderBy(c => c.DateShelf).ToList();
                    break;

                #endregion

                #region 品牌

                case (int)SubjectSortRuleType.brand://"brand":  //品牌
                    List<SubjectProductRef> brandEntityList = new List<SubjectProductRef>();
                    string[] sortbrand = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < sortbrand.Length; j++)
                    {
                        foreach (SubjectProductRef model in list)
                        {
                            if (model.BrandNo == sortbrand[j])
                            {
                                brandEntityList.Add(model);
                            }
                        }
                    }
                    list = brandEntityList;
                    break;
                #endregion

                #region 品类

                case (int)SubjectSortRuleType.category://"category":  //品类
                    List<SubjectProductRef> categoryEntityList = new List<SubjectProductRef>();
                    string[] sortcategory = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < sortcategory.Length; j++)
                    {
                        categoryEntityList.AddRange(list.Where(c => c.CategoryNo.Substring(0, 6) == sortcategory[j]));
                    }
                    list = categoryEntityList;
                    break;

                #endregion

                #region 色系   （在新的数据结构中 无法区分色系，因此此块暂时不做）

                //case (int)SubjectSortRuleType.color://"color":  色系

                //    List<SubjectProductRef> colorEntityList = new List<SubjectProductRef>();
                //    List<SubjectProductRef> TempColorEntityList = new List<SubjectProductRef>();
                //    SWfsSubjectService subject = new SWfsSubjectService();
                //    IList<WfsProductColorAttrDetail> wpcaEntity = subject.GetWfsProductColorAttrDetailListByColorDetail(sortData[1]);
                //    List<WfsProductColorAttrDetail> _wpcaEntity = new List<WfsProductColorAttrDetail>();

                //    if (wpcaEntity != null && wpcaEntity.Count() > 0)
                //    {
                //        string[] sortcolor = sortData[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //        for (int j = 0; j < sortcolor.Length; j++)
                //        {
                //            int colorNo = Convert.ToInt32(sortcolor[j]);
                //            _wpcaEntity.AddRange(wpcaEntity.Where(c => c.PrimaryColorId == colorNo));
                //        }
                //        wpcaEntity = _wpcaEntity;
                //    }
                //    foreach (WfsProductColorAttrDetail colorModel in wpcaEntity)
                //    {
                //        foreach (SubjectProductRef model in list)
                //        {
                //            string tempColor = GetProColor(model.ProductXmlText);

                //            if (!string.IsNullOrEmpty(tempColor))
                //            {
                //                if (tempColor.IndexOf(",") > 0)
                //                {
                //                    string[] _tempColor = tempColor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //                    if (colorModel.ColorName == _tempColor[0])
                //                    {
                //                        colorEntityList.Add(model);
                //                    }
                //                }
                //                else
                //                {
                //                    if (colorModel.ColorName == tempColor)
                //                    {
                //                        colorEntityList.Add(model);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    if (colorEntityList.Count() != list.Count())
                //    {
                //        foreach (SubjectProductRef model in colorEntityList)
                //        {
                //            list.Remove(model);
                //        }
                //        colorEntityList.AddRange(list);
                //        list = colorEntityList;
                //    }
                //    else
                //    {
                //        list = colorEntityList;
                //    }
                //    break;
                #endregion

                #region 收藏
                case (int)SubjectSortRuleType.collect:
                    string TempProductNos = string.Empty;
                    List<SubjectProductRef> _list = list;
                    for (int i = 0; i < list.Count(); i++)
                    {
                        TempProductNos += list[i].ProductNo + ",";
                    }
                    List<SubjectProductRef> _ProductCountList = new List<SubjectProductRef>();

                    SWfsSubjectService service = new SWfsSubjectService();
                    IList<FavoriteProductCount> ProductCountList = service.GetSWfsFavoriteProductByProductNos(TempProductNos);
                    if (sortData[1] == "0")
                    {
                        ProductCountList = ProductCountList.OrderByDescending(c => c.ProCount).OrderByDescending(c => c.ProductNo).ToList();
                        for (int i = 0; i < ProductCountList.Count(); i++)
                        {
                            SubjectProductRef _model = list.SingleOrDefault(c => c.ProductNo == ProductCountList[i].ProductNo);
                            _ProductCountList.Add(_model);
                            _list.Remove(_model);
                        }
                        _ProductCountList.AddRange(_list);
                        list = _ProductCountList;
                    }
                    else
                    {
                        ProductCountList = ProductCountList.OrderBy(c => c.ProCount).OrderBy(c => c.ProductNo).ToList();
                        for (int i = 0; i < ProductCountList.Count(); i++)
                        {
                            SubjectProductRef _model = list.SingleOrDefault(c => c.ProductNo == ProductCountList[i].ProductNo);
                            _ProductCountList.Add(_model);
                            _list.Remove(_model);
                        }
                        _list.AddRange(_ProductCountList);
                        list = _list;
                    }
                    break;
                #endregion

                #region 默认

                case (int)SubjectSortRuleType._default://"default":  //默认
                    list = list.OrderByDescending(c => c.DateCreate).ToList();
                    break;

                #endregion
            }

            return list;
        }

        #endregion


        #region 活动添加商品列表价格及状态    by lijibo 20141003

        /// <summary>
        ///活动添加商品列表价格及状态    by lijibo 20141003
        /// </summary>
        /// <param name="entitylist"></param>
        /// <returns></returns>
        public IEnumerable<ProductInfo> TransformationEntityList(IEnumerable<ProductInfo> entitylist)
        {
            if (entitylist != null && entitylist.Count() > 0)
            {
                SWfsSubjectService service = new SWfsSubjectService();
                IList<ProductInfo> _entitylist = new List<ProductInfo>();
                string tempProductNos = string.Empty;
                foreach (ProductInfo prModel in entitylist)
                {
                    tempProductNos += prModel.ProductNo + ",";
                }
                IList<SkuExtendPrice> SkuList = service.OutletGetSkuListByProductNo(tempProductNos);
                if (SkuList != null && SkuList.Count() > 0)
                {
                    foreach (ProductInfo prModel in entitylist)
                    {
                        ProductInfo _prModel = new ProductInfo();
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
                            //else if (isSelfList.Contains(1) && !isSelfList.Contains(2) && !isSelfList.Contains(3))
                            //{
                            //    SkuExtendPrice skuPrice = _SkuList.Where(s => s.IsShelf == 1).OrderBy(s => s.LimitedVipPrice).FirstOrDefault();
                            //    _prModel.IsShelf = 1;
                            //    _prModel.MarketPrice = skuPrice.MarketPrice;
                            //    _prModel.SellPrice = skuPrice.SellPrice;
                            //    _prModel.PlatinumPrice = skuPrice.PlatinumPrice;
                            //    _prModel.DiamondPrice = skuPrice.DiamondPrice;
                            //    _prModel.LimitedPrice = skuPrice.LimitedPrice;
                            //    _prModel.LimitedVipPrice = skuPrice.LimitedVipPrice;
                            //    _prModel.DisabledState = skuPrice.DisabledState;
                            //    _prModel.DiscountRate = skuPrice.DiscountRate;
                            //    _entitylist.Add(_prModel);//未上架
                            //}
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


        /// <summary>
        /// 更新市场价
        /// </summary>
        /// <param name="subjectNO">活动编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AjaxUpdateProductMarketPrice()
        {
            string productNos = Request["productNos"];
            IList<SkuExtendPrice> SkuList = new List<SkuExtendPrice>();
            if (!string.IsNullOrEmpty(productNos))
            {
                SWfsSubjectService service = new SWfsSubjectService();
                IList<SkuExtendPrice> _SkuList = service.OutletGetMarketPriceSkuListByProductNo(productNos);
                if (_SkuList != null && _SkuList.Count() > 0)
                {
                    string[] productArray = productNos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string tempProduct in productArray)
                    {
                        SkuExtendPrice _singleEntity = _SkuList.Where(c => c.ProductNo == tempProduct).FirstOrDefault();
                        if (_singleEntity != null)
                        {
                            SkuList.Add(_singleEntity);
                        }
                    }
                }
            }
            return Json(new { jsonProduct = SkuList });
        }

        #endregion

        /// <summary>
        /// 查询系统创建子类中是否已经存在商品
        /// </summary>
        /// <param name="subjectNO">活动编号</param>
        /// <returns></returns>
        public ActionResult AjaxGetProductCount(string subjectNO)
        {
            int count = 0;
            SWfsSubjectService service = new SWfsSubjectService();
            SWfsSubjectCategory cateInfo = service.GetSubjectCategoryList(subjectNO).Where(c => c.CategoryName == "系统创建").FirstOrDefault();
            count = service.SelectSubjectProductRef(cateInfo.CategoryNo).Count();
            if (count > 0)
            {
                return Json(new { result = "1", message = "系统创建中存在商品，不能切换！" }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = "0", message = "可以切换" }, "text/plain", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
