using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    public class VsinsHotProduct
    {
        public string HotProductId { get; set; }
        public DateTime StartDate { get; set;}
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string ProductNo { get; set; }
        public string GoodsNo { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }

        //市场价
        public decimal MarketPrice { get; set; }

        //奥莱价
        public decimal LimitedVipPrice { get; set; }

        /// 白金会员价
        public decimal PlatinumPrice { get; set; }

        /// 钻石会员价
        public decimal DiamondPrice { get; set; }

        /// 普通会员价
        public decimal LimitedPrice { get; set; }

        //促销价
        public decimal PromotionPrice { get; set; }
        // 尚品价--黄金价
        public decimal SellPrice { get; set; }



        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public int Quantity { get; set; }
        public int LockQuantity { get; set; }
        public int Status { get; set; }
        //库龄
        public int InventoryAge { get; set; }

        public short IsShelf { get; set; }

        public short IsLimitedOutlet { get; set; }     
    }
}
