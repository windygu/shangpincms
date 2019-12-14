using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Entity.Item.Search;
using Shangpin.Entity.User;

namespace Shangpin.Entity.Item.Vip
{
    /// <summary>
    /// 最终页价格显示条件
    /// </summary>
    public class ShowPriceCondition
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 来源类型1VIP专区会员 2VIP专区BDCODE  3其他LIST
        /// </summary>
        public short Type { get; set; }

        /// <summary>
        /// 如果显示VIP价格名称则预售价/VIP专享价
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否显示VIP价格名称
        /// </summary>
        public bool ShowVipName { get; set; }

        /// <summary>
        /// 关联活动标识主键（例如Type12标识VIP活动主键 Type3标识为空··）
        /// </summary>
        public string RelationID { get; set; }


        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogin { get; set; }


        /// <summary>
        /// 如果Type1标识VIP专区运行的会员级别集合
        /// </summary>
        public List<string> LevelNo { get; set; }

    }



    /// <summary>
    /// VIP首页
    /// </summary>
    public class VipIndexModel
    {
        /// <summary>
        /// 活动限制类型 0普通活动不限制 1为会员 2Code码
        /// </summary>
        public short RestrictedType { get; set; }

        public string VipSubjectNo { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 活动链接--根据活动类型读取
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviteCode { get; set; }

        /// <summary>
        /// 活动副标题
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// 活动类型1预售 2第三方合作  3会员专享 4普通活动
        /// </summary>
        public short Type { get; set; }

        /// <summary>
        /// 开始时间--需要根据类型计算
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 介绍时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 活动大图
        /// </summary>
        public string BigPicNO { get; set; }

        /// <summary>
        /// 活动小图
        /// </summary>
        public string SmallPicNO { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    public class VipCategory
    {
        public string VName { get; set; }

        public string VNO { get; set; }

    }


    public class VipProductInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        public string BrandNo { get; set; }

        /// <summary>
        /// 英文品牌名
        /// </summary>
        public string BrandEnName { get; set; }
        /// <summary>
        /// 中文品牌名
        /// </summary>
        public string BrandCnName { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string ProductPicFile { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 白金会员价
        /// </summary>
        public decimal PlatinumPrice { get; set; }

        /// <summary>
        /// 钻石会员价
        /// </summary>
        public decimal DiamondPrice { get; set; }

        /// <summary>
        /// 限时Vip会员价
        /// </summary>
        public decimal LimitedVipPrice { get; set; }

        /// <summary>
        /// 普通会员价
        /// </summary>
        public decimal LimitedPrice { get; set; }

        /// <summary>
        /// 上次会员价格
        /// </summary>
        public decimal LastLimitedPrice { get; set; }

        /// <summary>
        /// 尚品价
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// 促销价--VIP专区读取此价格显示
        /// </summary>
        public decimal PromotionPrice { get; set; }

        /// <summary>
        /// 库存数---需要后续的计算添加上
        /// </summary>
        public int AvailableStock { get; set; }


        /// <summary>
        /// 性别
        /// </summary>
        public short GenderStyle { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal DiscountRate { get; set; }

    }


    public class VipList
    {
        public CustomerInfo CustomerInfo { get; set; }

        /// <summary>
        /// 产品集合
        /// </summary>
        public IList<ProductInfo> ProductList { get; set; }

        /// <summary>
        /// VIP活动信息
        /// </summary>
        public SWfsVipSubject VipSubject { get; set; }

        /// <summary>
        /// 当前分类
        /// </summary>
        public string CurCategoryNo { get; set; }

        /// <summary>
        /// 当前品牌
        /// </summary>
        public string CurBrandNo { get; set; }

        /// <summary>
        /// 当前价格
        /// </summary>
        public string CurPrice { get; set; }

        /// <summary>
        /// 分类数据集合
        /// </summary>
        public IList<VipCategory> CategoryList {get;set;}

        /// <summary>
        /// 品牌数据集合
        /// </summary>
        public IList<BrandInfo> BrandList { get; set; }

          /// <summary>
          /// 价格数据集合
          /// </summary>
        public IList<PriceSetting> PriceList { get; set; }


    }


}
