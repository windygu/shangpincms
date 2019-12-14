using System.Collections.Generic;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 终端页模型
    /// </summary>
    public class DetailModel
    {
        /// <summary>
        /// 来源类型 1来自活动，0来自专题
        /// </summary>
        public string FromType { get; set; }
        /// <summary>
        /// 来源类型的值活动编号或专题编号
        /// </summary>
        public string FromTypeValue { get; set; }
        /// <summary>
        /// 活动分类
        /// </summary>
        public SWfsSubjectCategory SubjectCategory { get; set; }
        /// <summary>
        /// 活动类型代表已结束，正在进行，尚未开始
        /// </summary>
        public string SubjectType { get; set; }
        /// <summary>
        /// ERP品类
        /// </summary>
        public WfsErpCategory Category { get; set; }
        /// <summary>
        /// 一条商品属性的图片集合
        /// </summary>
        public IList<WfsProductAttrPic> ProductAttrPic { get; set; }
        /// <summary>
        /// 商品实体
        /// </summary>
        public WfsProduct Product { get; set; }
        /// <summary>
        /// 商品品牌实体
        /// </summary>
        public WfsBrand Brand { get; set; }
        /// <summary>
        /// 买手推荐rawHtml
        /// </summary>
        public string EditText { get; set; }
        /// <summary>
        /// 商品终端页展示属性
        /// </summary>
        public IList< ProductProperty> ProductProperty { get; set; }
        public IList<WfsProductPic> ProductPicList { get; set; }
        /// <summary>
        /// 最近浏览过
        /// </summary>
        public IList<SubjectProductInfo> BrowseHistory { get; set; }
        /// <summary>
        /// 你可能喜欢的
        /// </summary>
        public IList <SubjectProductInfo> LikedProduct{get;set;}
        /// <summary>
        /// 动态属性列表(商品颜色，商品尺码等字眼)
        /// </summary>
        public IList<string> DynamicAttrbuteNameList { get; set; }
        /// <summary>
        /// 商品属性列表
        /// </summary>
        public IList<WfsProductAttr> WfsProductAttrList { get; set; }
        /// <summary>
        /// 尺码/防水性等信息列表
        /// </summary>
        public IList<ProductAttrThirdInfo> ProductAttrThirdInfoList { get; set; }
        /// <summary>
        /// sku库存信息列表
        /// </summary>
        public IList<ProductInventoryInfo> ProductInvertoryList { get; set; }
        /// <summary>
        /// 活动
        /// </summary>
        public SubjectDetailPage SubjectDetail { get; set; }
        /// <summary>
        /// 活动或专题名称
        /// </summary>
        public string SubjectOrTopicName { get; set; }
        /// <summary>
        /// 是否显示价格
        /// </summary>
        public bool IfShowPrice { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string  ErrorMsg { get; set; }
        /// <summary>
        /// 专题信息
        /// </summary>
        public SWfsTopics Topic { get; set; }


        /// <summary>
        /// 售后服务
        /// </summary>
        /// <value>
        /// The service text.
        /// </value>
        public string ServiceText { get; set; }

    }
}
