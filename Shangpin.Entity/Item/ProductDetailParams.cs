using System.Collections.Generic;
using Shangpin.Entity.User;
using Shangpin.Entity.Wfs;

namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 商品终端页参数
    /// </summary>
    public class ProductDetailParams
    {
        /// <summary>
        /// 现金券批次
        /// </summary>
        public WfsPromotionTicket CDKBatch { get; set; }
        /// <summary>
        /// 面值
        /// </summary>
        public IList<string> MoneyText { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public short Gender { get; set; }
        /// <summary>
        /// 动态属性名称
        /// </summary>
        /// <value>
        /// The name of the dynamic attr.
        /// </value>
        public IList<string> DynamicAttrName { get; set; }
        /// <summary>
        /// 商品第一个动态属性
        /// </summary>
        /// <value>
        /// The product attr info.
        /// </value>
        public IList<WfsProductAttr> ProductAttrInfo { get; set; }
        /// <summary>
        /// 商品动态属性(第二个?)
        /// </summary>
        /// <value>
        /// The product minor attr info.
        /// </value>
        /// 
        /// Date:2012/7/24
        public IList<ProductMinorAttr> ProductMinorAttrInfo { get; set; }
        /// <summary>
        /// 第一个动态属性的属性图片
        /// </summary>
        /// <value>
        /// The product attr pic info.
        /// </value>
        /// 
        /// Date:2012/7/24
        public IList<WfsProductAttrPic> ProductAttrPicInfo { get; set; }
        /// <summary>
        /// 首次加载的商品图片
        /// </summary>
        /// <value>
        /// The product pic info.
        /// </value>
        /// 
        /// Date:2012/7/24
        public IList<WfsProductPic> ProductPicInfo { get; set; }
        /// <summary>
        /// 当前用户信息
        /// </summary>
        /// <value>
        /// The customer info.
        /// </value>
        /// 
        /// Date:2012/7/24
        public CustomerInfo CustomerInfo { get; set; }
        /// <summary>
        /// 第一个商品属性下所有SKU的库存信息
        /// </summary>
        /// <value>
        /// The product inventory info.
        /// </value>
        /// 
        /// Date:2012/7/24
        public IList<ProductInventoryInfo> ProductInventoryInfo { get; set; }
        /// <summary>
        /// SKU类型
        /// </summary>
        /// <value>
        /// The type of the sku.
        /// </value>
        /// 
        /// Date:2012/7/24
        public short SkuType { get; set; }
        /// <summary>
        /// 商品详情
        /// </summary>
        /// <value>
        /// The product propertys.
        /// </value>
        /// 
        /// Date:2012/7/24
        public IList<ProductProperty> ProductPropertys { get; set; }
        /// <summary>
        /// 售后服务
        /// </summary>
        /// <value>
        /// The service text.
        /// </value>
        /// 
        /// Date:2012/7/24
        public string ServiceText { get; set; }
        /// <summary>
        /// 品牌介绍
        /// </summary>
        /// <value>
        /// The brand story.
        /// </value>
        /// 
        /// Date:2012/7/24
        public string BrandStory { get; set; }

        /// <summary>
        /// 买手推荐信息
        /// </summary>
        /// <value>
        /// The buyer recommend.
        /// </value>
        /// 
        /// Date:2012/7/24
        public string BuyerRecommend { get; set; }

        /// <summary>
        /// 默认选择的第一个动态属性
        /// </summary>
        /// <value>
        /// The default attr.
        /// </value>
        /// 
        /// Date:2012/7/27
        public string DefaultAttr { get; set; }
        /// <summary>
        /// 默认选择的第二个动态属性
        /// </summary>
        /// <value>
        /// The default minor attr.
        /// </value>
        /// 
        /// Date:2012/7/27
        public string DefaultMinorAttr { get; set; }

    }

    public class AjaxProductDetailParams
    {
        public ProductDetailParams pdparams { get; set; }

        public WfsProduct product  { get; set; }

        public ProductInfo productBaseInfo { get; set; }

        public Shangpin.Entity.Item.Vip.ShowPriceCondition SPCModel { get; set; }

        public Dictionary<string, List<Dictionary<string, string>>> sizeTable { get; set; }

        public IEnumerable<ProductLabelRef> plrList { get; set; }
    }
}