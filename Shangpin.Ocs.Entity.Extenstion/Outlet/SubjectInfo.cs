using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public class SubjectInfo : PagingEntityBase
    {
        private string _belongsubjectpic;
        private string _titlepic1;
        private short _subjecttemplate;

        /// <summary>
        /// 活动包含商品的数量
        /// </summary>
        public int ProCount { get; set; } 
        public string BackgroundPic { get; set; }
        public string BelongsSubjectPic
        {
            get;
            set;

            //get
            //{
            //    if (_subjecttemplate == 4)
            //    {
            //        return this._titlepic1;
            //    }
            //    else
            //    {
            //        return this._belongsubjectpic;
            //    }
            //}
            //set
            //{
            //    this._belongsubjectpic = value;
            //}
        }
        public string ChannelNo { get; set; }
        public string ContentIntroduction { get; set; }
        public string ContentRecommended { get; set; }
        public string CreateUserId { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEnd { get; set; }
        public string Discount { get; set; }
        public short DiscountType { get; set; }
        public string DiscountTypeValue { get; set; }
        public string FlapPic { get; set; }
        public string IPhonePic { get; set; }
        public string IPhoneText { get; set; }
        /// <summary>
        /// 活动状态
        /// </summary>
        public short Status { get; set; }
        public string SubjectName { get; set; }
        public string SubjectEnName { get; set; }
        public string SubjectNo { get; set; }
        public short SubjectTemplate
        {
            get { return this._subjecttemplate; }
            set { this._subjecttemplate = value; }

        }

        public string TitlePic1//专题头图 20140314
        {

            get { return _titlepic1; }
            set { this._titlepic1 = value; }
        }
        public string TitlePic2 { get; set; }
        public bool TopFlag { get; set; }
        public string Hours { get; set; }
        public string Hour { get; set; }
        public string ContentTitleOne { get; set; }
        public string ContentTitleTwo { get; set; }
        public string BrandContent { get; set; }
        public string BU { get; set; }
        public decimal SalesForecast { get; set; }

        /// <summary>
        /// 活动子类的编号
        /// </summary>
        public string CategoryNo { get; set; }
        public IList<SWfsSubjectChannelSordRef> ChannelSordList { get; set; }
        public double TotalHour { get; set; }

        public string SpreadPicture { get; set; }
        public short SpreadStatus { get; set; }
        public int Orderflag { get; set; }
        public short SubjectPreStartTemplateType { get; set; }
        public string PrivilegeValue { get; set; }

        //活动表加入推荐品牌编号字段后加入的字段
        public string BrandSign { get; set; }
        public string BrandLogo { get; set; }
        public string BrandCnName { get; set; }
        public string BrandEnName { get; set; }

        public short IsPreheat { get; set; }
        public DateTime PreheatTime { get; set; }

        /// <summary>
        /// 预热页ID
        /// </summary>
        public string StExpand { get; set; }
        public short IsAudited { get; set; }
        public string AuditedUserId { get; set; }
        public DateTime AuditedDateTime { get; set; }

        /// <summary>
        /// 推广级别
        /// </summary>
        public short Level { get; set; }
        public DateTime PromotionApplyTime { get; set; }
        public DateTime PromotionConfirmTime { get; set; }

        public int APID { get; set; }

        /// <summary>
        /// 卖点
        /// </summary>
        public string SubjectSalesHot { get; set; }
        /// <summary>
        /// 货品信息
        /// </summary>
        public string ProductInfo { get; set; }
        /// <summary>
        /// 其他说明
        /// </summary>
        public string OtherDescription { get; set; }
        /// <summary>
        /// 推广渠道
        /// </summary>
        public string PromotionChannelNo { get; set; }
        /// <summary>
        /// SeoTitle
        /// </summary>
        public string SeoTitle { get; set; }
        /// <summary>
        /// SeoKeyWords
        /// </summary>
        public string SeoKeyWords { get; set; }
        /// <summary>
        /// SeoDescription
        /// </summary>
        public string SeoDescription { get; set; }

        /// <summary>
        /// 推广素材路径
        /// </summary>
        public string PicFileUrl { get; set; }

        /// <summary>
        /// 推广状态1 已经确认推广 0未确认推广--即未推广
        /// </summary>
        public short IsChecked { get; set; }

        /// <summary>
        /// 开始时间是否标红
        /// </summary>
        public bool IsStartTimeColor { get; set; }

        /// <summary>
        /// 结束时间是否标红
        /// </summary>
        public bool IsEndTimeColor { get; set; }

        /// <summary>
        /// 预热时间是否标红
        /// </summary>
        public bool IsPreheatTimeColor { get; set; }

        public IList<SWfsSubjectCategoryRef> ChannelCategoryList { get; set; }

        public short BrandLogoType { get; set; }
        public string SalesInfo { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 头图模板类型
        /// </summary>
        public short HeadShowType { get; set; }

        /// <summary>
        /// 头图Web代码
        /// </summary>
        public string HeadWebHtml { get; set; }

        /// <summary>
        /// 头图移动端代码
        /// </summary>
        public string HeadMobileHtml { get; set; }

        /// <summary>
        /// SEO-Title
        /// </summary>
        public string SubjectTitle { get; set; }

        public int Location { get; set; }

        public DateTime DateShow { get; set; }
    }

    [Serializable]
    public class SWfsSubjectCategoryII
    {
        public string ProductNo { get; set; }
        public string AdPic { get; set; }
        public string BackgroundPic { get; set; }
        public short BuyType { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNo { get; set; }
        public DateTime DateCreate { get; set; }
        public string LogoPic { get; set; }
        public bool ShowErpCategory { get; set; }
        public int SortNo { get; set; }
        public string SubjectNo { get; set; }
    }

    public class SubjectCheckStatusM
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}