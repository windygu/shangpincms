using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    [Serializable]
    public class SpfSkuExtendInfo
    {
        public string ProductNo { get; set; }
        public string GoodsNo { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public string CategoryNo { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 奥莱价
        /// </summary>
        public decimal OutletPrice { get; set; }
        /// <summary>
        /// 黄金价
        /// </summary>
        public decimal GoldPrice { get; set; }
        /// <summary>
        /// 白金会员价
        /// </summary>
        public decimal PlatinumPrice { get; set; }
        /// <summary>
        /// 钻石会员价
        /// </summary>
        public decimal DiamondPrice { get; set; }
        /// <summary>
        /// 促销价
        /// </summary>
        public decimal PromotionPrice { get; set; }
        /// <summary>
        /// 尚品价
        /// </summary>
        public decimal StandardPrice { get; set; }
        public decimal MinStandardPrice { get; set; }
        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public int Quantity { get; set; }
        public int LockQuantity { get; set; }
        public Int16 Status { get; set; }
        //库龄
        public int InventoryAge { get; set; }
        /// <summary>
        /// 是否上架 
        /// </summary>
        public Int16 IsShelf { get; set; }
        /// <summary>
        /// 商品所属部门
        /// </summary>
        public string DepartmentNo { get; set; }
        public Int16 PcShowState { get; set; }
        public Int16 DisabledState { get; set; }
        public DateTime DateShelf { get; set; }
    }
}
