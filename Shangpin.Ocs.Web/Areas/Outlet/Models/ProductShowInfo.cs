using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Service.Shangpin;
using Shangpin.Ocs.Service.Outlet;

namespace Shangpin.Ocs.Web.Areas.Outlet.Models
{
    /*用于展示商品信息*/
    public class ProductShowInfo
    {
        private ProductInfo _ProductInfo;
        public ProductShowInfo(ProductInfo pProductInfo)
        {
            if (pProductInfo == null)
            {
                _ProductInfo = new ProductInfo();
            }
            else
            {
                _ProductInfo = pProductInfo;
            }
        }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductNo
        {
            get
            {
                return _ProductInfo.ProductNo;
            }
            set { ProductNo = value; }
        }

        /// <summary>
        /// 商品货号
        /// </summary>
        public string GoodsNo
        {
            get
            {
                return _ProductInfo.GoodsNo;
            }
            set { GoodsNo = value; }
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get
            {
                return _ProductInfo.ProductName;
            }
            set { ProductName = value; }
        }
        /// <summary>
        /// 商品英文名称
        /// </summary>
        public string BrandEnName
        {
            get
            {
                return _ProductInfo.BrandEnName;
            }
            set { BrandEnName = value; }
        }

        /// <summary>
        /// 商品中文名称
        /// </summary>
        public string BrandCnName
        {
            get { return _ProductInfo.BrandCnName; }
            set { BrandCnName = value; }
        }

        /// <summary>
        /// 市场价
        /// </summary>
        public string MarketPrice
        {
            get
            {
                return _ProductInfo.MarketPrice.ToString();
            }
            set { MarketPrice = value; }
        }
        /// <summary>
        /// 奥来价格
        /// </summary>
        public string LimitedVipPrice
        {
            get
            {
                return _ProductInfo.LimitedVipPrice.ToString();
            }
            set { LimitedVipPrice = value; }
        }
        /// <summary>
        /// 上架状态
        /// </summary>
        public string IsShelf
        {
            get
            {
                return _ProductInfo.IsShelf == 0 ? "未上架" : (_ProductInfo.IsShelf == 1 ? "已上架" : "已下架");
            }
            set { IsShelf = value; }
        }

        /// <summary>
        /// 库龄
        /// </summary>
        public string pAge
        {
            get
            {
                return SWfsNewProductService.GetErpProductAgeingSingle(_ProductInfo.ProductNo);
            }
            set { pAge = value; }
        }
        /// <summary>
        /// 所属活动
        /// </summary>
        public IList<SWfsSubjectCategory> subjectCategory { get; set; }
        /// <summary>
        /// 所属专题
        /// </summary>
        public IList<SWfsTopicProductRef> topiclist { get; set; }

        /// <summary>
        /// 库存相关
        /// </summary>
        public ProductInventory inventory
        {
            get
            {
                return new SWfsProductService().GetInventoryByProductNo(_ProductInfo.ProductNo);
            }
            set { inventory = value; }
        }

    }
}