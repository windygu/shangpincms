using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Sales
{
    /// <summary>
    /// 页面上部实体
    /// </summary>
    public class SalseTopModel
    {
        public List<string> Size { get; set; }
        public SaleBanner Banner { get; set; }
        /// <summary>
        /// 鞋靴
        /// </summary>
        public string Shoes { get; set; }

        /// <summary>
        /// 服装
        /// </summary>
        public string Clothe { get; set; }

        /// <summary>
        /// 箱包
        /// </summary>
        public string Bag { get; set; }

        /// <summary>
        /// 配饰
        /// </summary>
        public string Accessories { get; set; }

    }

    /// <summary>
    /// 头部图片
    /// </summary>
    public class SaleBanner
    {
        public string link { get; set; }
        public string picFileNo { get; set; }
        public string picName { get; set; }
    }

    public class SalesUrlParmModel
    {
        /// <summary>
        /// 品类
        /// </summary>
        public string categoryNo { get; set; }

        /// <summary>
        /// 打折03 35 57 
        /// </summary>
        public string discount { get; set; }

        /// <summary>
        /// 0默认 1升序 2降序
        /// </summary>
        public short price { get; set; }

        /// <summary>
        /// 尺码
        /// </summary>
        public string size { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 折扣排序
        /// </summary>
        public short discountSort { get; set; }

        /// <summary>
        /// 时间排序
        /// </summary>
        public short timeSort { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        public string brandNo { get; set; }
    }

    public class SalseSize
    {
        public string ProductNo { get; set; }
        public string ErpCatroy { get; set; }
    }

    public class SizeModel
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        public string Name { get; set; }
    }
}
