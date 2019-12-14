using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.ProductFlat
{
    //搜索接口参数实体类
    public class Parameters
    {
        public string productNO{ get; set; }//商品编号可多选，用","隔开

        public string productName { get; set; }//商品名称

        public string categoryNO { get; set; }//分类编号

        /// <summary>
        /// OCS分类名称
        /// </summary>
        public string OCSCategoryName { get; set; }

        public string brandNO { get; set; }//商品品牌编号

        public string colorId { get; set; }//颜色id

        public string shelfType { get; set; }//是否为新上架查询（0或null为新上架，1为自定义上架时间）

        public string StartShelfDate { get; set; }//开始上架时间

        public string EndShelfDate { get; set; }//结束上架时间

        public string shelfDate { get; set; }//上架时间

        public string StartPrice { get; set; }//最低价格

        public string EndPrice { get; set; }//最高价格

        public string price { get; set; }//价格

        public string StartStock { get; set; }//最低库存

        public string EndStock { get; set; }//最高库存

        public string stock { get; set; }//库存

        public string StartDiscountRate { get; set; }//最低折扣（接近0）

        public string EndDiscountRate { get; set; }//最高折扣（接近1）

        public string discountRate { get; set; }//折扣

        public string start { get; set; }//分页开始记录

        public string end { get; set; }//结束

        public string BrandName { get; set; }//商品品牌名称

        public string ColorName { get; set; }//色系名称

        public string postArea { get; set; }//是否海外购 1:大陆；2：海外

        public string hot { get; set; }//累计热度

        public string sevenHot { get; set; }//七天热度
    }
}
