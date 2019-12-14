using System;

namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 促销产品附加信息
    /// </summary>
    public class AddProInformation
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public string PName { get; set; }

        /// <summary>
        /// 品牌中文名称
        /// </summary>
        public string BCName { get; set; }

        /// <summary>
        /// 品牌英文名称
        /// </summary>
        public string BEName { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 尚品价--黄金价
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 普通会员价
        /// </summary>
        public decimal LimitedPrice { get; set; }

        /// <summary>
        /// 白金会员价
        /// </summary>
        public decimal PlatinumPrice { get; set; }

        /// <summary>
        /// 钻石价
        /// </summary>
        public decimal DiamondPrice { get; set; }


        /// <summary>
        /// 产品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 产品图片
        /// </summary>
        public string PicFileNo { get; set; }

        /// <summary>
        /// 产品组合价
        /// </summary>
        public decimal GroupPrice { get; set; }





    }

    /// <summary>
    /// 查询促销产品实体信息
    /// </summary>
    [Serializable]
    public class ProProductInfo
    {
        /// <summary>
        /// 促销编号
        /// </summary>
        public string PromotionDeviceNo { get; set; }

        /// <summary>
        /// 促销描述
        /// </summary>
        public string PromotionDes { get; set; }

        /// <summary>
        /// 促销组合号
        /// </summary>
        public string PromotionGroupNo { get; set; }

        /// <summary>
        /// 促销类型
        /// </summary>
        public short PromotionType { get; set; }

        /// <summary>
        /// 促销类型名称
        /// </summary>
        public string PromotionTypeName { get; set; }

        /// <summary>
        /// 促销商品编号
        /// </summary>
        public string PromotionProductNo { get; set; }

        /// <summary>
        /// 促销商品的组合单价
        /// </summary>
        public decimal SPromotionPrice { get; set; }

        /// <summary>
        /// 主商品编号
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 促销产品附加信息
        /// </summary>
        public ProductInfo AddInfo { get; set; }

        /// <summary>
        /// 促销商品颜色尺寸信息
        /// </summary>
        public ProductAtrrColorSize PColoSize { get; set; }

        /// <summary>
        /// 主商品颜色尺码信息
        /// </summary>
        public ProductAtrrColorSize MColorSize { get; set; }

        /// <summary>
        /// 显示的促销价格
        /// </summary>
        //public decimal ShowPromotionPrice { get; set; }

        public string PromotionCodeDesc { get; set; }

        public short PromotionTitleFlag { get; set; }


        /// <summary>
        /// 促销组合总价
        /// </summary>
        public decimal PromotionZHPrice { get; set; }

    }

    /// <summary>
    /// 产品是否促销20121128优化增加
    /// </summary>
    [Serializable]
    public class IsGp
    {
        /// <summary>
        /// 产品ID(主商品)
        /// </summary>
        public string pid { get; set; }
        /// <summary>
        /// 是否显示促销标志1 为显示 0不显示
        /// </summary>
        public string isgpFlag { get; set; }

        /// <summary>
        /// 组合商品ID
        /// </summary>
        public string spid { get; set; }

        /// <summary>
        /// 组合号
        /// </summary>
        public string gno { get; set; }

        /// <summary>
        /// 组合价格
        /// </summary>
        public decimal gprice { get; set; }
    }

    /// <summary>
    /// 促销主要内容实体类
    /// </summary>
    public class GPInfoModel
    {
        /// <summary>
        /// 主商品ID
        /// </summary>
        public string MPid { get; set; }

        /// <summary>
        /// 促销组合中主商品价格
        /// </summary>
        public decimal MPrice { get; set; }

        /// <summary>
        /// 促销组合编号
        /// </summary>
        public string GPNo { get; set; }
        /// <summary>
        /// 促销商品ID
        /// </summary>
        public string SPid { get; set; }

        /// <summary>
        /// 组合总价
        /// </summary>
        public decimal GPrice { get; set; }
    }

    /// <summary>
    /// 是否有组合产品---列表页批量判断是否有组合产品标识实体20130315
    /// </summary>
    public class IsGroupProduct
    {
        public string PromotionGroupNo { get; set; }
        public string ProductNo { get; set; }
    }

}
