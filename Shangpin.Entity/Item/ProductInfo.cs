using System;
using System.Collections.Generic;

namespace Shangpin.Entity.Item
{

    /// <summary>
    /// 搜索返回的商品信息
    /// </summary>
    /// Author:wangtao
    /// Date:2012/7/10
    [Serializable]
    public class ProductInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        /// <value>
        /// The product no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public string ProductNo { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public string ProductName { get; set; }

        #region 搜索重建后变为数组

        ///// <summary>
        ///// 一级分类编号
        ///// </summary>
        ///// <value>
        ///// The category no LV1.
        ///// </value>
        ///// Author:wangtao
        ///// Date:2012/7/10
        //public string CategoryNoLv1 { get; set; }
        ///// <summary>
        ///// 一级分类名称
        ///// </summary>
        ///// <value>
        ///// The category name LV1.
        ///// </value>
        ///// Author:wangtao
        ///// Date:2012/7/10
        //public string CategoryNameLv1 { get; set; }
        ///// <summary>
        ///// 二级分类编号
        ///// </summary>
        ///// <value>
        ///// The category no LV2.
        ///// </value>
        ///// Author:wangtao
        ///// Date:2012/7/10
        //public string CategoryNoLv2 { get; set; }
        ///// <summary>
        ///// 二级分类名称
        ///// </summary>
        ///// <value>
        ///// The category name LV2.
        ///// </value>
        ///// Author:wangtao
        ///// Date:2012/7/10
        //public string CategoryNameLv2 { get; set; }
        ///// <summary>
        ///// 三级分类编号
        ///// </summary>
        ///// <value>
        ///// The category no LV3.
        ///// </value>
        ///// Author:wangtao
        ///// Date:2012/7/10
        //public string CategoryNoLv3 { get; set; }
        ///// <summary>
        ///// 三级分类名称
        ///// </summary>
        ///// <value>
        ///// The category name LV3.
        ///// </value>
        ///// Author:wangtao
        ///// Date:2012/7/10
        //public string CategoryNameLv3 { get; set; }
        ///// <summary>
        ///// 四级分类编号
        ///// </summary>
        ///// <value>
        ///// The category no LV4.
        ///// </value>
        ///// Author:wangtao
        ///// Date:2012/7/10
        //public string CategoryNoLv4 { get; set; }
        ///// <summary>
        ///// 四级分类名称
        ///// </summary>
        ///// <value>
        ///// The category name LV4.
        ///// </value>
        ///// Author:wangtao
        ///// Date:2012/7/10
        //public string CategoryNameLv4 { get; set; }

        #endregion

        /// <summary>
        /// OCS中关联该商品的分类
        /// </summary>
        /// <value>
        /// The category no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/14
        public IList<string> CategoryNo { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        /// <value>
        /// The brand no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public string BrandNo { get; set; }
        /// <summary>
        /// 英文品牌名
        /// </summary>
        /// <value>
        /// The name of the brand en.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public string BrandEnName { get; set; }
        /// <summary>
        /// 中文品牌名
        /// </summary>
        /// <value>
        /// The name of the brand cn.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public string BrandCnName { get; set; }

        /// <summary>
        /// 品牌图片
        /// </summary>
        /// <value>
        /// The product pic no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public string ProductPicFile { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        /// <value>
        /// The market price.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 白金会员价
        /// </summary>
        /// <value>
        /// The platinum price.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public decimal PlatinumPrice { get; set; }
        /// <summary>
        /// 钻石会员价
        /// </summary>
        /// <value>
        /// The diamond price.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public decimal DiamondPrice { get; set; }
        /// <summary>
        /// 限时Vip会员价
        /// </summary>
        /// <value>
        /// The limited vip price.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public decimal LimitedVipPrice { get; set; }
        /// <summary>
        /// 普通会员价
        /// </summary>
        /// <value>
        /// The limited price.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public decimal LimitedPrice { get; set; }
        /// <summary>
        /// 上次会员价格
        /// </summary>
        /// <value>
        /// The last limited price.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public decimal LastLimitedPrice { get; set; }
        /// <summary>
        /// 尚品价
        /// </summary>
        /// <value>
        /// The sell price.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 上架时间
        /// </summary>
        /// <value>
        /// The date shelf.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/10
        public DateTime DateShelf { get; set; }

        /// <summary>
        /// 商品显示状态
        /// </summary>
        /// <value>
        /// The product show flag.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public string ProductNewFlag { get; set; }
        /// <summary>
        /// 商品是否在活动中
        /// </summary>
        /// <value>
        /// The product promotion flag.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public string ProductPromotionFlag { get; set; }
        /// <summary>
        /// 商品是否特价
        /// </summary>
        /// <value>
        /// The product special flag.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public string ProductSpecialFlag { get; set; }
        /// <summary>
        /// 普通商品
        /// </summary>
        /// <value>
        /// The product ordinary flag.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public string ProductOrdinaryFlag { get; set; }

        /// <summary>
        /// 是否有商品细节图
        /// </summary>
        /// <value>
        /// The detail of the picture.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public string PictureIsDetail { get; set; }

        /// <summary>
        /// 是否有商品尺寸图
        /// </summary>
        /// <value>
        /// The size of the picture.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public string PictureIsSize { get; set; }
        /// <summary>
        /// 是否有商品海报图
        /// </summary>
        /// <value>
        /// The Poster of the picture.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public string PictureIsPoster { get; set; }
        /// <summary>
        /// 是否有商品包装图
        /// </summary>
        /// <value>
        /// The Pockage of the picture.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public string PictureIsPockage { get; set; }
        /// <summary>
        /// 是否支持会员权益
        /// </summary>
        /// <value>
        /// The is support discount.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/11
        public int IsSupportDiscount { get; set; }

        /// <summary>
        /// 上架状态
        /// </summary>
        /// <value>
        /// The is self.
        /// </value>
        /// Author:wangtao
        /// Date:2012/7/13
        public int IsShelf { get; set; }

        /// <summary>
        /// 库存数
        /// </summary>
        /// <value>
        /// The available stock.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/2
        public int AvailableStock { get; set; }
        /// <summary>
        /// Erp品类编号(判断显示库存尺码已使用，避免不必要的流量)
        /// </summary>
        /// <value>
        /// The erp category no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/2
        public string ErpCategoryNo { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        /// <value>
        /// The available stock.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/2
        public short GenderStyle { get; set; }

        /// <summary>
        /// 是否显示市场价
        /// </summary>
        /// Author:lijia
        /// Date:2012/12/26
        public short? IsShowMarketPrice { get; set; }

        /// <summary>
        /// 是否有库存
        /// </summary>
        public string HasStock { get; set; }

        /// <summary>
        /// 促销价--新增字段--以后所有的前端显示促销价格均读取此字段20130424
        /// </summary>
        public decimal PromotionPrice { get; set; }

        /// <summary>
        /// VIP专区--增加折扣
        /// </summary>
        public decimal DiscountRate { get; set; }
    }
}
