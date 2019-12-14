using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Item.Outlet
{
    /// <summary>
    ///活动列表使用到的实体类 终端页图片
    /// </summary>
    public class DetailPicture
    {
        /// <summary>
        /// 小图
        /// </summary>
        public string simg { get; set; }
        /// <summary>
        /// 中图
        /// </summary>
        public string mimg { get; set; }
        /// <summary>
        /// 大图
        /// </summary>
        public string bimg { get; set; }
    }
    /// <summary>
    /// 商品尺寸
    /// </summary>
    public class ProductSize
    {
        /// <summary>
        /// 尺寸（38）
        /// </summary>
        public string size { get; set; }
        /// <summary>
        /// 尺寸描述(中国CN 37)
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 是否有库存
        /// </summary>
        public bool stock { get; set; }
        /// <summary>
        /// 原始尺码
        /// </summary>
        public string OriginalInfo { get; set; }
    }
    /// <summary>
    /// 点击颜色图片需要展示的内容
    /// </summary>
    public class OneProductAttrInfo
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string no { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 真实库存
        /// </summary>
        public int realquantity { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 商品属性值(白色)
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 商品属性名称(ProductAttrColor）
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 商品属性的图片列表
        /// </summary>
        public IList<DetailPicture> imglist { get; set; }
        /// <summary>
        /// 商品尺寸名称
        /// </summary>
        public string ccname { get; set; }
        /// <summary>
        /// 商品尺寸列表
        /// </summary>
        public IList<ProductSize> cclist { get; set; }
    }
}
