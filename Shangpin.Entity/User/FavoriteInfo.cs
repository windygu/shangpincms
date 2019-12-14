using System;
using Shangpin.Entity.Common;

namespace Shangpin.Entity.User
{
    public class FavoriteInfo
    {
        public class FavoriteBrand : PagingEntityBase
        { 
            public string UserId {get;set;}
            public short RepleNotice {get;set;}
            public short IsVoid {get;set;}
            public int FavoriteBrandId {get;set;}
            public string Ex01 {get;set;}
            public DateTime DateCreate {get;set;}
            public string BrandNo {get;set;}
            public short ActiveNotice {get;set;}
            public string BrandEnName {get;set;}
            public string BrandCnName {get;set;}
            public int ActiveCount {get;set;}
            public int RepleCount {get;set;}
            public short BrandGender { get; set; }
            public short NewBrandGender { get; set; }
            public int IsNewBrand { get; set; }
            public int BrandStatus { get; set; }
        }
        public class FavoriteProduct : PagingEntityBase
        {
            public string UserId {get;set;}
            public string SkuNo {get;set;}
            public string SCategoryNo {get;set;}
            public string ProductNo {get;set;}
            public short PriceNotice {get;set;}
            public decimal Price {get;set;}
            public short IsVoid {get;set;}
            public int FavoriteProductId {get;set;}
            public string  Ex01 {get;set;}
            public DateTime DateCreate {get;set;}
            public string Attributes {get;set;}
            public short ArrivalNotice {get;set;}
            public string CategoryNo {get;set;}
            public string ProductName {get;set;}
            public string SkuAttrText {get;set;}
            public string SkuHashText {get;set;}
            public string ProductPicFile {get;set;}
            public short SizeStandard {get;set;}
            public decimal DiffAmount  {get;set;}
            public string NewSize {get;set;}
            public string StandardName {get;set;}
            public string Attrs {get;set;}
            public int Stock {get;set;}
            public string ProductGender { get; set; }
            public int IsPromotion { get; set; }
            public int TopicSubjectFlag { get; set; }
            public decimal LimitedPrice { get; set; }
            public int FavoriteCount { get; set; }
            public short Gender { get; set; }
            /// <summary>
            /// 产品类型，1电子礼品卡，2实物礼品卡，0正常商品
            /// </summary>
            public short ProductType { get; set; }
            public string ProductCategoryNo { get; set; }
            public string Guid { get; set; }

        }

    }
}
