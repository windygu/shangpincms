using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateAoLaiSubjectDiscountInfo.Model
{
    [Serializable]
    public class SubjectProductM
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 奥莱价
        /// </summary>
        public decimal OutletPrice { get; set; }

        /// <summary>
        /// 活动多少折扣
        /// </summary>
        public decimal DiscountRate { get; set; }

        /// <summary>
        /// 活动多少元
        /// </summary>
        public decimal LimitedVipPrice { get; set; }

    }
}
