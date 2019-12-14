namespace Shangpin.Entity.Item.Brand
{
    /// <summary>
    /// 品牌索引页面
    /// </summary>
   public class BrandIndexInfo
    {

        /// <summary>
        ///英文 首字母
        /// </summary>
       public string FristLetter { get; set; }

       /// <summary>
       ///汉字 首字母
       /// </summary>
       public int? FristLetterValue { get; set; }

       /// <summary>
       ///品牌数量
       /// </summary>
       public long LetterCount { get; set; }


       /// <summary>
       ///品牌数量索引值
       /// </summary>
       public long RowNumber { get; set; } 
    }
}
