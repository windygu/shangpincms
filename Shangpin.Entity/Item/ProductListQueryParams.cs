namespace Shangpin.Entity.Item
{
    public class ProductListQueryParams
    {
        private string baseCategoryNo;
        /// <summary>
        /// 基础分类编号
        /// </summary>
        /// <value>
        /// The base category no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public string BaseCategoryNo
        {
            get
            {
                return baseCategoryNo;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    baseCategoryNo = "0";
                else
                    baseCategoryNo = value;
            }
        }
        private string categoryNo;
        /// <summary>
        /// 分类编号
        /// </summary>
        /// <value>
        /// The category no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public string CategoryNo
        {
            get
            {
                return categoryNo;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    categoryNo = "0";
                else
                    categoryNo = value;
            }
        }
        private string brandNo;
        /// <summary>
        /// 品牌编号
        /// </summary>
        /// <value>
        /// The brand no.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public string BrandNo
        {
            get
            {
                return brandNo;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    brandNo = "0";
                else
                    brandNo = value;
            }
        }
        private string productSize;
        /// <summary>
        /// 尺码
        /// </summary>
        /// <value>
        /// The size of the product.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public string ProductSize
        {
            get
            {
                return productSize;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    productSize = "0";
                else
                    productSize = value;
            }
        }
        private string tag;
        /// <summary>
        /// 属性标签1品类，2品牌，3颜色 4尺码  5价格 6趋势 7材质 8最新到货日期-20130323修改
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    tag = "0";
                else
                    tag = value;
            }
        }
        private string price;
        /// <summary>
        /// 价格区间
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public string Price
        {
            get
            {
                return price;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    price = "0";
                else
                    price = value;
            }
        }
        private string color;
        /// <summary>
        /// 商品颜色
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public string Color
        {
            get
            {
                return color;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    color = "0";
                else
                    color = value;
            }
        }
        private short sortBy;
        
        /// <summary>
        /// 1新品 2价格升序 3价格降序  排序字段20130415修改BY liulei
        /// </summary>
        public short SortBy 
        {
            get
            {
                return sortBy;
            }
            set
            {
                sortBy = value;
            }
        }
        private short sortType;
        
        /// <summary>
        /// 1售罄 2013415修改BY liulei
        /// </summary>
        public short SortType
        {
            get
            {
                return sortType;
            }
            set
            {
                sortType = value;
            }
        }
        private int pageIndex;
        /// <summary>
        /// 页码
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public int PageIndex
        {
            get
            {
                return pageIndex;
            }
            set
            {
                if (value == 0)
                    pageIndex = 1;
                else
                    pageIndex = value;
            }
        }
        private short gender;
        /// <summary>
        /// 性别
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        /// Author:wangtao
        /// Date:2012/8/24
        public short Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;
            }
        }

        /// <summary>
        /// 是否显示有库存
        /// </summary>
        private string hasStock;
        public string HasStock
        {
            get
            {
                return hasStock;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    hasStock = "0";
                else
                    hasStock = value;
            }
        }

        /// <summary>
        /// 显示页码条数切换
        /// </summary>
        private int pageSize;
        public int PageSize
        {
            get { return pageSize; }

            set
            {
                if (value==0)
                    pageSize = 1;
                else
                    pageSize = value;
            }
        }


        /// <summary>
        /// 最新到货时间
        /// </summary>
        private string arrivalTime;
        public string ArrivalTime
        {
            get
            {
                return arrivalTime;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    arrivalTime = "0";
                else
                    arrivalTime = value;
            }
        }
    }
}
