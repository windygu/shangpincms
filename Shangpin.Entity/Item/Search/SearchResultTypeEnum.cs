using System.ComponentModel;

namespace Shangpin.Entity.Item.Search
{
    /// <summary>
    /// 搜索返回数据类型枚举
    /// </summary>
    public enum SearchResultTypeEnum
    {
        [Description("返回数据类型为json")]
        Json=0,
        [Description("返回数据类型为xml")]
        Xml=1
    }
}
