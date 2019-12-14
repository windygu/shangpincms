using System.Collections.Generic;
using Shangpin.Entity.User;
using Shangpin.Entity.Item.Vip;
using Shangpin.Entity.Item.Search;

namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 为页面视图生成的视图类
    /// </summary>
    public class TopicModel
    {
        /// <summary>
        /// 性别
        /// </summary>
        public short Gender { get; set; }
        /// <summary>
        /// 专题编号
        /// </summary>
        public string TopicNo { get; set; }
        
        /// <summary>
        /// 专题名称
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// 专题BANNER图片地址
        /// </summary>
        public string TopicURL { get; set; }
        
        //该专题下产品列表
        public IList<ProductInfo> PrdouctInfoList { get; set; }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public CustomerInfo CustomerInfo { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public IDictionary<string, int> PrdouctKC { get; set; }


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
        public IList<VipCategory> CategoryList { get; set; }

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