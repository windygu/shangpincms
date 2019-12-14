using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Entity.Item;

namespace Shangpin.Entity.Outlet
{
    [Serializable]
    public class GroupSalesInfo
    {
        /// <summary>
        /// 组合号
        /// </summary>
        public string PromotionGroupNo { get; set; }
        /// <summary>
        /// 促销策略号
        /// </summary>
        public string PromotionDeviceNo { get; set; }
        /// <summary>
        /// 策略名称
        /// </summary>
        public string PromotionDeviceName { get; set; }
        /// <summary>
        /// 组合总价
        /// </summary>
        public decimal GroupAmount { get; set; }
        /// <summary>
        /// 促销策略说明
        /// </summary>
        public string PromotionCodeDesc { get; set; }
        /// <summary>
        /// 促销策略状态
        /// </summary>
        public short Status { get; set; }
        /// <summary>
        /// 策略开始时间
        /// </summary>
        public DateTime PromotionDeviceNoStart { get; set; }
        /// <summary>
        /// 策略结束时间
        /// </summary>
        public DateTime PromotionDeviceNoEnd { get; set; }
        /// <summary>
        /// 主商品实体
        /// </summary>
        public WfsProduct FirstProduct { get; set; }
        /// <summary>
        /// 副商品实体
        /// </summary>
        public WfsProduct SecondProduct { get; set; }
        /// <summary>
        /// 主商品动态属性列表(商品颜色，商品尺码等字眼)
        /// </summary>
        public IList<string> FirstDynamicAttrbuteNameList { get; set; }
        /// <summary>
        /// 副商品动态属性列表(商品颜色，商品尺码等字眼)
        /// </summary>
        public IList<string> SecondDynamicAttrbuteNameList { get; set; }
        /// <summary>
        /// 主商品sku库存信息列表
        /// </summary>
        public IList<ProductInventoryInfo> FirstProductInvertoryList { get; set; }
        /// <summary>
        /// 副商品sku库存信息列表
        /// </summary>
        public IList<ProductInventoryInfo> SecondProductInvertoryList { get; set; }
        /// <summary>
        /// 原始奥莱价之和
        /// </summary>
        public decimal OriginalPrice { get; set; }
        /// <summary>
        /// 主商品品牌实体
        /// </summary>
        public WfsBrand FirstBrand { get; set; }
        /// <summary>
        /// 副商品品牌实体
        /// </summary>
        public WfsBrand SecondBrand { get; set; }

        public decimal FirstGroupPrice { get; set; }

        public decimal SecondGroupPrice { get; set; }
        /// <summary>
        /// 主商品属性
        /// </summary>
        public IList<WfsProductAttr> FirstWfsProductAttrList { get; set; }
        /// <summary>
        /// 副商品属性
        /// </summary>
        public IList<WfsProductAttr> SecondWfsProductAttrList { get; set; }
        /// <summary>
        /// 主商品尺码/防水性等信息列表
        /// </summary>
        public IList<ProductAttrThirdInfo> FirstProductAttrThirdInfoList { get; set; }
        /// <summary>
        /// 副商品尺码/防水性等信息列表
        /// </summary>
        public IList<ProductAttrThirdInfo> SecondProductAttrThirdInfoList { get; set; }
        /// <summary>
        /// 来源类型 1来自活动，0来自专题
        /// </summary>
        public string FromType { get; set; }
        /// <summary>
        /// 专题信息
        /// </summary>
        public SWfsTopics Topic { get; set; }
        /// <summary>
        /// 活动分类
        /// </summary>
        public SWfsSubjectCategory SubjectCategory { get; set; }
    }
}
