using System;
using System.Collections.Generic;

namespace Shangpin.Entity.Item
{
    /// <summary>
    /// 组合产品颜色尺码对应类
    /// </summary>
    [Serializable]
    public class ProductAtrrColorSize
    {
        /// <summary>
        /// 颜色名称
        /// </summary>
        public string dataname { get; set; }

        /// <summary>
        ///  尺码名称
        /// </summary>
        public string sizename { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<Color> datalist { get; set; }
    }

    [Serializable]
    public class Color
    {
        public string id { get; set; }
        public string type { get; set; }
        public string realquantity { get; set; }
        public string quantity { get; set; }
        public string value { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string no { get; set; }
        public List<Size> sizelist { get; set; }

    }


    [Serializable]
    public class Size
    {
        /// <summary>
        /// 尺码
        /// </summary>
        public string size { get; set; }
        /// <summary>
        /// 尺码描述
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 是否有货
        /// </summary>
        public bool stock { get; set; }

        
        /// <summary>
        /// name="0148546_紫色_43"
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string val { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int quantity { get; set; }
    }

    public class SingleProductColorSize
    {
        /// <summary>
        /// 颜色编号
        /// </summary>
        public string no { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 库存?
        /// </summary>
        public string realquantity { get; set; }
        /// <summary>
        /// 库存?
        /// </summary>
        public string quantity { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }


        /// <summary>
        /// 尺码名称
        /// </summary>
        public string ccname { get; set; }
        /// <summary>
        /// 尺码列表
        /// </summary>
        public List<Size> cclist { get; set; }

    }
}