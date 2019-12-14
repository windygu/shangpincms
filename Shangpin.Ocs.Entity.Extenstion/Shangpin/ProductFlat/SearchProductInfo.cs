using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ProductFlat
{
    //商品实体类
    public class InterfaceProductInfo
    {
        public string ProductNo { get; set; }//商品编号

        public string ProductName { get; set; }//商品名称

        public string GoodsNo { get; set; }//商品货号

        public string ProductPicFile { get; set; }//商品图片

        public string BrandNo { get; set; }//商品品牌

        public string BrandEnName { get; set; }//商品品牌英文名称

        public string BrandCnName { get; set; }//商品品牌中文名称

        public int stock { get; set; }//商品库存

        public decimal MarketPrice { get; set; }//市场价

        public decimal LimitedPrice { get; set; }//尚品价

        public decimal PromotionPrice { get; set; }//促销价

        public string DiscountRate { get; set; }//折扣

        //public DateTime LaunchDate { get; set; }//上市日期

        //public string MarketType { get; set; }//销售类型

        //public string StorageClass { get; set; }//仓储分类

        public string DateShelf { get; set; }//上架日期

        public string Category { get; set; }  //产品分类编号集合,逗号间隔

        public string ProductPrimaryColor { get; set; }  //产品色系编号集合，逗号间隔
        public string ProductPrimaryColorName { get; set; }

        public int IsSelected { get; set; }

        public long Hot { get; set; }//热度
        public long SevenHot { get; set; }//七日热度

        public long EditeHot { get; set; }//修改后的热度
        public long EditeSevenHot { get; set; }//修改后的七日热度
    }
}
