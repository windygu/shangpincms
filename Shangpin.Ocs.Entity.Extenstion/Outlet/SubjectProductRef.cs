using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    public class SubjectProductRef : PagingEntityBase
    {
        public string SubjectProductRefId { get; set; }
        public int PropertyValue { get; set; }
        public int TopFlag { get; set; }
        public string SortNo { get; set; }
        public string BuyType { get; set; }
        public short IsStarProduct { get; set; }
        public DateTime DateShelf { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string ProductPicFile { get; set; }
        public string GoodsNo { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal PlatinumPrice { get; set; }
        public decimal DiamondPrice { get; set; }
        public decimal LimitedPrice { get; set; }
        public decimal LimitedVipPrice { get; set; }
        public short IsShelf { get; set; }
        public string ProductXmlText { get; set; }
        public string BrandNo { get; set; }
        public string BrandEnName { get; set; }
        public string BrandCnName { get; set; }
        public int Quantity { get; set; }
        public int LockQuantity { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime ShowTime { get; set; }
        public bool IsShow { get; set; }
        public DateTime DateCreate { get; set; }
        public string DepartmentNo { get; set; }

        //2014-1-13 秒杀新增by liulei
        public string CategoryNo { get; set; }
        public string SubjectNo { get; set; }
        public int ProductNum { get; set; }

        /// <summary>
        /// 冻结状态：0、未冻结    1、已冻结  by lijibo 20141003
        /// </summary>
        public short DisabledState { get; set; }
    }
}
