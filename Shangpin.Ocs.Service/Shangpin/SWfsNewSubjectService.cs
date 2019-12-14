
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
//using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Service.Common;
using System.Xml;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Framework.Common.Cache;
using System.Text.RegularExpressions;


namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsNewSubjectService
    {
        #region 商品管理
        public RecordPage<ProductInfoNew> GetSWfsSubjectProduct(string categoryNo, string scategoryNo, string brandNo, string isShelf, string genderStyle, string keyword,string isout, decimal lpstartPrice, decimal lpendPrice, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("SCategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("GenderStyle", genderStyle == null ? "" : genderStyle);
            dic.Add("StartPrice", lpstartPrice <= 0 ? 0 : lpstartPrice);
            dic.Add("EndPrice", lpendPrice <= 0 ? 0 : lpendPrice);
            dic.Add("IsOutSide", string.IsNullOrEmpty(isout) ? "" : isout);
            IList<ProductInfoNew> productList = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_WfsProduct_SelectProductListNew", dic, new { KeyWord = keyword, SCategoryNo = scategoryNo, IsShelf = isShelf, GenderStyle = genderStyle, BrandNo = brandNo, CategoryNo = categoryNo,IsOutSide=isout, StartPrice = lpstartPrice, EndPrice = lpendPrice, pageIndex = pageIndex, pageSize = pageSize }).ToList();
            productList = productList = productList.GroupBy(p => p.ProductNo).Select(p => new ProductInfoNew()
            {
                ProductNo = p.ElementAt(0).ProductNo,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                IsShelf = p.ElementAt(0).IsShelf,
                PcSaleState = p.ElementAt(0).PcSaleState,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                MarketPrice = p.ElementAt(0).MarketPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice),
                IsOutSide=p.ElementAt(0).IsOutSide
            }).ToList();
            return PageConvertor.Convert(pageIndex, pageSize, productList);
        }
        //查询商品总条数 by tianxiuquan 
        public int GetSWfsSubjectProductTotalCount(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword, string genderStyle,string isout, decimal lpstartPrice, decimal lpendPrice)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("SCategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("GenderStyle", genderStyle == null ? "" : genderStyle);
            dic.Add("StartPrice", lpstartPrice <= 0 ? 0 : lpstartPrice);
            dic.Add("EndPrice", lpendPrice <= 0 ? 0 : lpendPrice);
            dic.Add("IsOutSide", string.IsNullOrEmpty(isout) ? "" : isout);
            return DapperUtil.Query<int>("ComBeziWfs_WfsProduct_SelectProductListNewCount", dic, new { KeyWord = keyword, SCategoryNo = scategoryNo, IsShelf = isShelf, GenderStyle = genderStyle, BrandNo = brandNo, CategoryNo = categoryNo, IsOutSide = isout, StartPrice = lpstartPrice, EndPrice = lpendPrice }).FirstOrDefault();
        }
        public IList<ProductInfoNew> GetSWfsSubjectAllProduct(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword,string isout)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("SCategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("IsOutSide", string.IsNullOrEmpty(isout) ? "" : isout);
            IList<ProductInfoNew> productList = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_WfsProduct_SelectAllProductListNew", dic, new { KeyWord = keyword, SCategoryNo = scategoryNo, IsShelf = isShelf, BrandNo = brandNo, CategoryNo = categoryNo }).ToList();
            productList = productList.GroupBy(p => p.ProductNo).Select(p => new ProductInfoNew()
            {
                ProductNo = p.ElementAt(0).ProductNo,
                ProductModel = p.ElementAt(0).ProductModel,
                GoodsNo = p.ElementAt(0).GoodsNo,
                PcSaleState = p.ElementAt(0).PcSaleState,
                IsShelf = p.ElementAt(0).IsShelf,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                MarketPrice = p.ElementAt(0).MarketPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPriceRegion) + "~" + p.Max(a => a.StandardPriceRegion),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPriceRegion) + "~" + p.Max(a => a.PlatinumPriceRegion),
                DiamondPriceRegion = p.Min(a => a.DiamondPriceRegion) + "~" + p.Max(a => a.DiamondPriceRegion),
                PromotionPriceRegion = p.Min(a => a.PromotionPriceRegion) + "~" + p.Max(a => a.PromotionPriceRegion),
                GoldPriceRegion = p.Min(a => a.GoldPriceRegion) + "~" + p.Max(a => a.GoldPriceRegion),
                IsOutSide=p.ElementAt(0).IsOutSide
            }).ToList();
            return productList;
        }
        public RecordPage<ProductInfoNew> GetSWfsSubjectProductList_over(string categoryNo, string scategoryNo, string brandNo, string isShelf, string genderStyle, string keyword,string isout, int pageIndex, int pageSize)
        {
            string[] keywordNew = keyword.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("SCategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("GenderStyle", genderStyle == null ? "" : genderStyle);
            dic.Add("IsOutSide", string.IsNullOrEmpty(isout) ? "" : isout);
            //dic.Add("Keyword", keyword == null ? "" : keyword);
            IList<ProductInfoNew> productList = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_WfsProduct_BatchSelectProductListNew", dic, new { KeyWord = keywordNew, SCategoryNo = scategoryNo, IsShelf = isShelf, GenderStyle = genderStyle, BrandNo = brandNo, CategoryNo = categoryNo, pageIndex = pageIndex, pageSize = pageSize,IsOutSide=isout }).ToList();
            productList = productList.GroupBy(p => p.ProductNo).Select(p => new ProductInfoNew()
            {
                ProductNo = p.ElementAt(0).ProductNo,
                ProductModel = p.ElementAt(0).ProductModel,
                GoodsNo = p.ElementAt(0).GoodsNo,
                PcSaleState = p.ElementAt(0).PcSaleState,
                IsShelf = p.ElementAt(0).IsShelf,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                MarketPrice = p.ElementAt(0).MarketPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPriceRegion) + "~" + p.Max(a => a.StandardPriceRegion),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPriceRegion) + "~" + p.Max(a => a.PlatinumPriceRegion),
                DiamondPriceRegion = p.Min(a => a.DiamondPriceRegion) + "~" + p.Max(a => a.DiamondPriceRegion),
                PromotionPriceRegion = p.Min(a => a.PromotionPriceRegion) + "~" + p.Max(a => a.PromotionPriceRegion),
                GoldPriceRegion = p.Min(a => a.GoldPriceRegion) + "~" + p.Max(a => a.GoldPriceRegion),
                IsOutSide=p.ElementAt(0).IsOutSide
            }).ToList();
            return PageConvertor.Convert(pageIndex, pageSize, productList);
        }
        public IEnumerable<ProductInfoNew> GetSWfsSubjectProductList(string categoryNo, string scategoryNo, string brandNo, string isShelf, string ProductSex, string keyWord, string isout, string isAddProduct, string productNoList, string lpstart, string lpend, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            Regex reg = new Regex(@"^[0-9a-zA-Z]+$");
            if (!string.IsNullOrEmpty(keyWord) && reg.IsMatch(keyWord.Trim()))
            {
                dic.Add("ProductNoOrProductModel", keyWord);
                dic.Add("ProductName", "");
            }
            else
            {
                dic.Add("ProductNoOrProductModel", "");
                dic.Add("ProductName", string.IsNullOrEmpty(keyWord) ? "" : keyWord.Trim());
            }
            //楼层编号 scategoryNo 不能为空 否则不能进行任何 添加查询商品操作
            if (string.IsNullOrEmpty(scategoryNo))
            {
                total = 0;
                return new List<ProductInfoNew>();
            }
            dic.Add("ProductNoList",string.IsNullOrEmpty(productNoList)?"":"1212");
            dic.Add("SCategoryNo", string.IsNullOrEmpty(scategoryNo) ? "" : scategoryNo);
            dic.Add("IsAddProduct", string.IsNullOrEmpty(isAddProduct) ? "" : isAddProduct);
            dic.Add("CategoryNo", string.IsNullOrEmpty(categoryNo)? "" : categoryNo);
            dic.Add("BrandNo", string.IsNullOrEmpty(brandNo) ? "" : brandNo);
            dic.Add("IsShelf", string.IsNullOrEmpty(isShelf)? "" : isShelf);
            dic.Add("ProductSex", string.IsNullOrEmpty(ProductSex) ? "" : ProductSex);
            dic.Add("IsOutSide", string.IsNullOrEmpty(isout) ? "" : isout);
            dic.Add("lpstart", string.IsNullOrEmpty(lpstart) ? "" : lpstart);
            dic.Add("lpend", string.IsNullOrEmpty(lpend) ? "" : lpend);

            string[] readyAddProductNoList=new string[0];
            if (!string.IsNullOrEmpty(productNoList))
            {
                readyAddProductNoList = productNoList.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                for (int i = 0; i < readyAddProductNoList.Length; i++)
                {
                    readyAddProductNoList[i] = readyAddProductNoList[i].Trim();
                }
            }
            IList<ProductInfoNew> productList = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_SpProduct_BatchSelectProductListNew", dic, new
            {
                ProductName = keyWord,
                SCategoryNo = scategoryNo,
                IsShelf = isShelf,
                BrandNo = brandNo,
                CategoryNo = categoryNo,
                lpstart=lpstart,
                lpend=lpend,
                pageIndex = pageIndex,
                pageSize = pageSize,
                ProductNoOrProductModel=keyWord,
                IsOutSide = string.IsNullOrEmpty(isout) ? -1 : int.Parse(isout),
                ProductSex = string.IsNullOrEmpty(ProductSex) ? -1 : int.Parse(ProductSex),
                ProductNoList = readyAddProductNoList
            }).ToList();
            total = DapperUtil.Query<int>("ComBeziWfs_SpProduct_BatchSelectProductListNewTotalCount", dic, new
            {
                ProductName = keyWord,
                SCategoryNo = scategoryNo,
                IsShelf = isShelf,
                GenderStyle = ProductSex,
                BrandNo = brandNo,
                CategoryNo = categoryNo,
                lpstart = lpstart,
                lpend = lpend,
                pageIndex = pageIndex,
                pageSize = pageSize,
                ProductNoOrProductModel = keyWord,
                IsOutSide = isout,
                ProductNoList = readyAddProductNoList
            }).FirstOrDefault();
            productList = productList.GroupBy(p => p.ProductNo).Select(p => new ProductInfoNew()
            {
                ProductNo = p.ElementAt(0).ProductNo,
                ProductModel = p.ElementAt(0).ProductModel,
                GoodsNo = p.ElementAt(0).GoodsNo,
                PcSaleState = p.ElementAt(0).PcSaleState,
                IsShelf = p.ElementAt(0).IsShelf,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                MarketPrice = p.ElementAt(0).MarketPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice),
                IsOutSide = p.ElementAt(0).IsOutSide
            }).ToList();
            return productList;
        }
        public IEnumerable<SubjectProductRefNew> GetSWfsSubjectAlreadyAddProductList(string subjectNo,string categoryNo, string scategoryNo,
            string brandNo, string isShelf, string ProductSex, string keyWord, string isout, string isAddProduct,
            string productNoList, int pageIndex, int pageSize, out int total)
        {
 
            var dic = new Dictionary<string, object>();
            Regex reg = new Regex(@"^[0-9a-zA-Z]+$");
            if (!string.IsNullOrEmpty(keyWord) && reg.IsMatch(keyWord.Trim()))
            {
                dic.Add("ProductNoOrProductModel", keyWord);
                dic.Add("ProductName", "");
            }
            else
            {
                dic.Add("ProductNoOrProductModel", "");
                dic.Add("ProductName", string.IsNullOrEmpty(keyWord) ? "" : keyWord.Trim());
            }
            //楼层编号 scategoryNo 不能为空 否则不能进行任何 添加查询商品操作
            if (string.IsNullOrEmpty(scategoryNo))
            {
                total = 0;
                return new List<SubjectProductRefNew>();
            }
            //dic.Add("ProductNoList", string.IsNullOrEmpty(productNoList) ? "" : "1212");
            //dic.Add("SCategoryNo", string.IsNullOrEmpty(scategoryNo) ? "" : scategoryNo);
            dic.Add("CategoryNo", string.IsNullOrEmpty(categoryNo) ? "" : categoryNo);
            dic.Add("BrandNo", string.IsNullOrEmpty(brandNo) ? "" : brandNo);
            dic.Add("IsShelf", string.IsNullOrEmpty(isShelf) ? "" : isShelf);
            dic.Add("ProductSex", string.IsNullOrEmpty(ProductSex) ? "" : ProductSex);
            dic.Add("IsOutSide", string.IsNullOrEmpty(isout) ? "" : isout);
            IList<SubjectProductRefNew> productList = DapperUtil.Query<SubjectProductRefNew>("ComBeziWfs_SWfsNewSubjectProductRef_GetAlReadyAddSubjectProductList", dic, new
            {
                ProductName = keyWord,
                SCategoryNo = scategoryNo,
                IsShelf = isShelf,
                BrandNo = brandNo,
                CategoryNo = categoryNo,
                pageIndex = pageIndex,
                pageSize = pageSize,
                ProductNoOrProductModel = keyWord,
                IsOutSide = string.IsNullOrEmpty(isout) ? -1 : int.Parse(isout),
                ProductSex = string.IsNullOrEmpty(ProductSex) ? -1 : int.Parse(ProductSex),
                //ProductNoList = productNoList.Split(new string[] { "\r\n" }, StringSplitOptions.None)
            }).ToList();
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsNewSubjectProductRef_GetAlReadyAddSubjectProductListTotalCount", dic, new
            {
                ProductName = keyWord,
                SCategoryNo = scategoryNo,
                IsShelf = isShelf,
                BrandNo = brandNo,
                CategoryNo = categoryNo,
                pageIndex = pageIndex,
                pageSize = pageSize,
                ProductNoOrProductModel = keyWord,
                IsOutSide = string.IsNullOrEmpty(isout) ? -1 : int.Parse(isout),
                ProductSex = string.IsNullOrEmpty(ProductSex) ? -1 : int.Parse(ProductSex),
                //ProductNoList = productNoList.Split(new string[] { "\r\n" }, StringSplitOptions.None)
            }).FirstOrDefault();

    
            productList = productList.GroupBy(p => p.ProductNo).Select(p => new SubjectProductRefNew()
            {
                SubjectProductRefId = p.ElementAt(0).SubjectProductRefId,
                PropertyValue = p.ElementAt(0).PropertyValue,
                TopFlag = p.ElementAt(0).TopFlag,
                SortNo = p.ElementAt(0).SortNo,
                BuyType = p.ElementAt(0).BuyType,
                ShowTime = p.ElementAt(0).ShowTime,
                IsShow = p.ElementAt(0).IsShow,
                ProductNo = p.ElementAt(0).ProductNo,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                MarketPrice = p.ElementAt(0).MarketPrice,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                IsShelf = p.ElementAt(0).IsShelf,
                PcSaleState = p.ElementAt(0).PcSaleState,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice),
                IsOutSide = p.ElementAt(0).IsOutSide
            }).ToList();
            return productList;
        }
        
        //更新活动的商品数量
        public void UpdateSubjectProductCount(string subjectNo) 
        {
            if (!string.IsNullOrEmpty(subjectNo))
            {
                //查询活动的商品总数量
                int subjectTotalProuctCount = DapperUtil.Query<int>("ComBeziWfs_SWfsNewSubject_GetSubjectTotalCount", new
                {
                    SubjectNo = subjectNo
                }).FirstOrDefault();
                //更新活动中添加的商品数量
                DapperUtil.Execute("ComBeziWfs_SWfsNewSubject_UpdateSubjectProductCount", new
                {
                    ProductCount = subjectTotalProuctCount,
                    SubjectNo = subjectNo
                });
            }
        }

        //添加商品到活动楼层
        public int AddProductToSubjectFloor(string subjectNo,string sCategoryNo,string productNo) 
        {
            if (string.IsNullOrEmpty(productNo) || string.IsNullOrEmpty(sCategoryNo))
            {
                return 0;
            }
            
            #region 过滤已经添加过的商品编号
            List<string> readyAddProductNoList = new List<string>();
            string[] productNoList = productNo.Split(',');
            IEnumerable<string> AreadyAddProductNoList = DapperUtil.Query<string>("ComBeziWfs_SWfsNewSubject_ExsitsSubjectFloorProduct", new
            {
                SCategoryNo = sCategoryNo
            });

            for (int i = 0; i < productNoList.Length; i++)
            {
                if (!AreadyAddProductNoList.Contains(productNoList[i].Trim()))
                {
                    readyAddProductNoList.Add(productNoList[i].Trim());
                }
            }
            #endregion
            //增加商品
            int result= DapperUtil.Execute("ComBeziWfs_SpProduct_BatchSelectProductListAddToSubjectFloor", new 
            {
                SCategoryNo=sCategoryNo,
                ProductNoList = readyAddProductNoList
            });

            if (result>0)
            {
                UpdateSubjectProductCount(subjectNo);//更新活动的商品数量
            }
            else
            {
                return 0;
            }
            return readyAddProductNoList.Count;
        }
        //查询总条数 by tianxiuquan  
        public int GetSWfsSubjectProductListTotalCount(string categoryNo, string scategoryNo, string brandNo, string isShelf, string genderStyle, string keyword,string isout)
        {
            string[] keywordNew = keyword.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("SCategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("GenderStyle", genderStyle == null ? "" : genderStyle);
            dic.Add("IsOutSide",string.IsNullOrEmpty(isout)?"":isout);
            return DapperUtil.Query<int>("ComBeziWfs_WfsProduct_BatchSelectProductListNewTotalCount", dic, new { KeyWord = keywordNew, SCategoryNo = scategoryNo, IsShelf = isShelf, GenderStyle = genderStyle, BrandNo = brandNo, CategoryNo = categoryNo ,IsOutSide=isout}).FirstOrDefault();
        }
        public IList<ProductInfoNew> GetSWfsSubjectAllProductList(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword)
        {
            string[] keywordNew = keyword.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("SCategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            IList<ProductInfoNew> productList = DapperUtil.Query<ProductInfoNew>("ComBeziWfs_WfsProduct_BatchSelectProductListNew", dic, new { KeyWord = keywordNew, SCategoryNo = scategoryNo, IsShelf = isShelf == null ? "" : isShelf, BrandNo = brandNo == null ? "" : brandNo, CategoryNo = categoryNo == null ? "" : categoryNo }).ToList();

            return productList;
        }
        /// <summary>
        /// 查询一、二、三、四级分类。
        /// </summary>
        public IList<ErpCategory> SelectErpCategoryByParentNo(string parentNo)
        {
            IList<ErpCategory> categorylist = DapperUtil.Query<ErpCategory>("ComBeziWfs_WfsErpCategory_ReadCategoryByParentNO", new { ParentNo = parentNo }).ToList();
            return categorylist;
        }
        /// <summary>
        /// 根据活动编号获得所属品类
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SWfsSubjectCategoryRef> GetErpCategoryListBySubjectNo(string subjectNo)
        {
            return DapperUtil.Query<SWfsSubjectCategoryRef>("ComBeziWfs_SWfsSubjectCategoryRef_SelectBySubjectNo", new { SubjectNo = subjectNo }).ToList();
        }
        /// <summary>
        /// 根据活动子类编号获得其信息
        /// </summary>
        /// <param name="categoryNo">活动子类编号</param>
        /// <returns></returns>
        public SWfsNewSubjectCategory GetSubjectCategoryModel(string categoryNo)
        {
            return DapperUtil.QueryByIdentity<SWfsNewSubjectCategory>(categoryNo);
        }
        /// <summary>
        /// 获得一条活动信息
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public SubjectInfoNew GetSubjectInfo(string subjectNo)
        {
            return DapperUtil.Query<SubjectInfoNew>("ComBeziWfs_SWfsNewSubject_SelectBySubjectNo", new { SubjectNo = subjectNo }).FirstOrDefault();
        }
        /// <summary>
        /// 获得一条活动信息New
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public SWfsNewSubject GetSubjectInfoNew(string subjectNo)
        {
            return DapperUtil.Query<SWfsNewSubject>("ComBeziWfs_SWfsNewSubject_SelectBySubjectNo", new { SubjectNo = subjectNo }).FirstOrDefault();
        }
        /// <summary>
        /// 查询活动子类下关联的商品
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="scategoryNo"></param>
        /// <param name="brandNo"></param>
        /// <param name="isShelf"></param>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public RecordPage<SubjectProductRefNew> SelectSubjectProductRefList(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword,int isout, int pageIndex, int pageSize)
        { 
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("SCategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("IsOutSide",isout==0?"":isout+"");

            IEnumerable<SubjectProductRefNew> query = DapperUtil.QueryPaging<SubjectProductRefNew>("ComBeziWfs_SWfsNewSubjectProductRef_SelectByCategoryNo",
                pageIndex, pageSize, " SWfsNewSubjectProductRef.SortNo ASC", dic,
                new
                {
                    KeyWord = keyword == null ? "" : keyword,
                    SCategoryNo = categoryNo == null ? "" : categoryNo,
                    IsShelf = isShelf == null ? "" : isShelf,
                    BrandNo = brandNo == null ? "" : brandNo,
                    CategoryNo = scategoryNo,
                    IsOutSide = isout
                });

            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }
        public IEnumerable<SubjectProductRefNew> SelectSubjectProductRefLists(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword, int isout, int pageIndex, int pageSize,out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("SCategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("IsOutSide", isout == 0 ? "" : isout + "");
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsNewSubjectProductRef_SelectByCategoryNoPageCount", new 
            {
                KeyWord = keyword == null ? "" : keyword,
                SCategoryNo = categoryNo == null ? "" : categoryNo,
                IsShelf = isShelf == null ? "" : isShelf,
                BrandNo = brandNo == null ? "" : brandNo,
                CategoryNo = scategoryNo,
                IsOutSide = isout
            }).FirstOrDefault();
            IEnumerable<SubjectProductRefNew> query = DapperUtil.QueryPaging<SubjectProductRefNew>("ComBeziWfs_SWfsNewSubjectProductRef_SelectByCategoryNoPage",
                pageIndex, pageSize, " SWfsNewSubjectProductRef.SortNo ASC", dic,
                new
                {
                    KeyWord = keyword == null ? "" : keyword,
                    SCategoryNo = categoryNo == null ? "" : categoryNo,
                    IsShelf = isShelf == null ? "" : isShelf,
                    BrandNo = brandNo == null ? "" : brandNo,
                    CategoryNo = scategoryNo,
                    IsOutSide = isout
                });

            return query;
        }
        /// <summary>
        /// 商品可视化查询
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<SubjectProductRefNew> SelectSubjectProductRefList(string category)
        {

            IEnumerable<SubjectProductRefNew> list = DapperUtil.Query<SubjectProductRefNew>("ComBeziWfs_SWfsNewSubjectProductRef_SelectByCategoryNoView", new 
            {
                CategoryNo = category
            });
            list = list.GroupBy(p => p.ProductNo).Select(p => new SubjectProductRefNew()
            {
                SubjectProductRefId = p.ElementAt(0).SubjectProductRefId,
                PropertyValue = p.ElementAt(0).PropertyValue,
                TopFlag = p.ElementAt(0).TopFlag,
                SortNo = p.ElementAt(0).SortNo,
                BuyType = p.ElementAt(0).BuyType,
                ShowTime = p.ElementAt(0).ShowTime,
                IsShow = p.ElementAt(0).IsShow,
                ProductNo = p.ElementAt(0).ProductNo,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                MarketPrice = p.ElementAt(0).MarketPrice,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                IsShelf = p.ElementAt(0).IsShelf,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                DiscountRate = p.ElementAt(0).DiscountRate,
                DiscountShangpin = p.ElementAt(0).DiscountShangpin,
            }).ToList();
            return list.ToList();
        }

        //public WfsProduct ReadProduct(string productNo)
        //{
        //    WfsProduct product = DapperUtil.QueryByIdentityWithNoLock<WfsProduct>(productNo);
        //    return product;
        //}
        public bool IsExistProduct(string productNo)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsSpProduct_ValidateProductExistByProductNo", new { ProductNo = productNo }).FirstOrDefault() > 0 ? true : false;
        }
        public IList<string> SelectSubjectProductRef(string scategoryNo)
        {
            IList<String> splist = DapperUtil.Query<String>("ComBeziWfs_SWfsNewSubjectProductRef_ReadByCategoryNo", new { CategoryNo = scategoryNo }).ToList();
            return splist;
        }
        /// <summary>
        /// 专题活动下的商品变动时重新计算活动的折和 显示值 （作废）
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="sCategoryNo"></param>
        public void ExecuteDiscountTypeValue(string subjectNo, string sCategoryNo,int isout)
        {
            #region 商品变动重新计算活动  折和元数据  //增加商品时，删除商品时

            SubjectInfoNew sub = GetSubjectInfo(subjectNo);
            //如果活动类型是 "多少元" 如果添加的商品的价格不一样 活动类型修改为 "多少元起" 将最低价的商品修改为
            if (sub.DiscountType == 4 || sub.DiscountType == 5)//全场X
            {
                var subprolist = SelectSubjectProductRefList("", sCategoryNo, "", "", "", isout, 1, 9999).Items.OrderBy(o => o.LimitedVipPrice).Select(s => s.LimitedVipPrice).ToList().Distinct();
                short discountType = 0;
                if (subprolist.Count() > 1)
                {
                    discountType = 5;//全场X起
                }
                else
                {
                    discountType = 4;//全场X
                }
                var leastPrice = subprolist.FirstOrDefault();
                UpdateSubjectDiscountType(subjectNo, discountType, leastPrice);
            }
            //将"多少折" 修改为"多少折起"
            if (sub.DiscountType == 3 || sub.DiscountType == 1)//X折
            {
                var subprolist = SelectSubjectProductRefList("", sCategoryNo, "", "", "",isout, 1, 9999).Items.OrderBy(o => o.DiscountRate).Select(s => s.DiscountRate).ToList().Distinct();
                short discountType = 0;
                if (subprolist.Count() > 1)
                {
                    discountType = 1;//X折起
                }
                else
                {
                    discountType = 3;//X折
                }
                var leastPrice = subprolist.FirstOrDefault();
                UpdateSubjectDiscountType(subjectNo, discountType, leastPrice);
            }
            #endregion

        }
        /// <summary>
        /// 修改活动折扣类型和值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdateSubjectDiscountType(string subjectNo, short type, decimal value)
        {
            return DapperUtil.UpdatePartialColumns<SWfsNewSubject>(new { SubjectNo = subjectNo, DiscountType = type, DiscountTypeValue = value });
        }
        /// <summary>
        /// 添加活动日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public int InsertSubjectOrChannelLog(SWfsSubjectOrChannelLog log)
        {
            return DapperUtil.Insert(log, false);
        }
        /// <summary>
        /// 删除活动下关联的商品
        /// </summary>
        /// <param name="categoryNo"></param>
        /// <param name="productNo"></param>
        public void DeleteSubjectProductRef(string relationId)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsNewSubjectProductRef_DeleteBySubjectProductRefId", new { RelationId = relationId });
        }
        /// <summary>
        /// 根据商品编号和子类编号获得信息
        /// </summary>
        /// <param name="productNo"></param>
        /// <param name="categoryNo"></param>
        /// <returns></returns>
        public SWfsNewSubjectProductRef GetSubjectProduct(int refId)
        {
            return DapperUtil.QueryByIdentity<SWfsNewSubjectProductRef>(refId);
            //return DapperUtil.Query<SWfsSubjectProductRef>("ComBeziWfs_SWfsSubjectProductRef_SelectByProNoAndCategoryNo", new { ProductNo = productNo, CategoryNo = categoryNo }).FirstOrDefault();
        }
        public IList<SWfsSubjectProductSort> GetProductSortList(string subjectNo)
        {
            return DapperUtil.Query<SWfsSubjectProductSort>("ComBeziWfs_SWfsSubjectProductSort_SelectBySubjectNo", new { SubjectNo = subjectNo }).ToList();
        }
        #endregion

        #region 活动分页列表信息
        /// <summary>
        /// 分页查询  【注意：list=null或list.Count&lt;1时，list为null】
        /// </summary>
        /// <param name="sqlStatements">配置在Sql.xml中的sql语句ID</param>
        /// <param name="param">sql语句中的参数[匿名类]</param>
        public List<SWfsNewSubject> GetSubjectPageList(ref PaginationInfoModel pageModel)
        {
            int totalcount = 0;
            if (string.IsNullOrEmpty(pageModel.OrderBy))
            { pageModel.OrderBy = " DateCreate desc "; }
            if (string.IsNullOrEmpty(pageModel.Field))
            { pageModel.Field = " *  "; }
            if (string.IsNullOrEmpty(pageModel.SelectField))
            { pageModel.SelectField = "*"; }
            if (string.IsNullOrEmpty(pageModel.Condition))
            { pageModel.Condition = ""; }
            object param = new { tableName = " SWfsNewSubject  with (NoLock) ", pageSize = pageModel.PageSize, curPage = pageModel.CurrentPage, orderBy = pageModel.OrderBy, selectfield = pageModel.SelectField, field = pageModel.Field, condition = pageModel.Condition };
            List<SWfsNewSubject> list = DapperUtil.QueryPageList<SWfsNewSubject>("ComBeziWfs_SWfsNewSubject_PageListSearch", ref totalcount, param).ToList();
            pageModel.TotalPage = CommonHelp.GetPageCount(pageModel.PageSize, totalcount);
            pageModel.TotalCount = totalcount;
            return list;
        }

        /// <summary>
        /// 获取活动列表中所有的商品编号
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<string> GetSWfsNewSubjectProductRefList(string subjectNo)
        {
            return DapperUtil.Query<string>("ComBeziWfs_SWfsNewSubjectProductRef_SelectByCategoryNoProudctNo", new { SubjectNo = subjectNo }).ToList();

        }
        /// <summary>
        /// 获取所有活动列表
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SWfsNewSubjectCategory> GetSWfsNewSubjectCategoryList()
        {
            return DapperUtil.Query<SWfsNewSubjectCategory>("ComBeziWfs_SWfsNewSubjectCategory_List", null).ToList();

        }
        /// <summary>
        /// 获取所有活动列表
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SWfsNewSubjectCategory> GetSWfsNewSubjectCategoryListBySubjectNo(string subjectNo)
        {
            return DapperUtil.Query<SWfsNewSubjectCategory>("ComBeziWfs_SWfsNewSubjectCategory_ListBySubjectNo", new { SubjectNo = subjectNo }).ToList();

        }
        /// <summary>
        /// 获得一条活动信息
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public SWfsNewSubject GetNewSubjectInfo(string subjectNo)
        {
            return DapperUtil.Query<SWfsNewSubject>("ComBeziWfs_SWfsNewSubject_SelectBySubjectNo", new { SubjectNo = subjectNo }).FirstOrDefault();
        }
        /// <summary>
        /// 活动状态修改
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="status"></param>
        public void NewSubjectStatusModify(string subjectNo, string status)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsNewSubject_UpdateStatus", new { SubjectNo = subjectNo, Status = status });
        }


        /// <summary>
        /// 获取活动列表中所有的商品信息 使用在管理商品，标准模板
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SubjectNewProductRefModel> GetSWfsNewSubjectProductRefListAll(string subjectNo)
        {
            IList<SubjectNewProductRefModel> list = DapperUtil.Query<SubjectNewProductRefModel>("ComBeziWfs_SWfsNewSubjectProductRef_SelectByCategoryNoProudctListALL", new { SubjectNo = subjectNo }).ToList();
            list = list.GroupBy(p => p.ProductNo).Select(p => new SubjectNewProductRefModel()
            {
                SubjectNo = p.ElementAt(0).SubjectNo,
                SubjectTemplateType = p.ElementAt(0).SubjectTemplateType,
                Status = p.ElementAt(0).Status,
                CategoryNo = p.ElementAt(0).CategoryNo,
                SubjectProductRefId = p.ElementAt(0).SubjectProductRefId,
                PropertyValue = p.ElementAt(0).PropertyValue,
                TopFlag = p.ElementAt(0).TopFlag,
                BuyType = p.ElementAt(0).BuyType,
                ShowTime = p.ElementAt(0).ShowTime,
                IsShow = p.ElementAt(0).IsShow,
                DateCreate = p.ElementAt(0).DateCreate,
                ProductFlag = p.ElementAt(0).ProductFlag,
                ProductNo = p.ElementAt(0).ProductNo,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                MarketPrice = p.ElementAt(0).MarketPrice,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                ProductPromotionFlag = p.ElementAt(0).ProductPromotionFlag,
                IsShelf = p.ElementAt(0).IsShelf,
                PcSaleState = p.ElementAt(0).PcSaleState,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                DiscountShangpin = p.ElementAt(0).DiscountShangpin,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice)
            }).ToList();
            return list;
        }

        /// <summary>
        /// 获取活动列表中所有的商品信息,楼层模板
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<SubjectNewProductRefModel> GetSWfsNewSubjectProductRefList(string subjectNo, string categoryNo)
        {
            IList<SubjectNewProductRefModel> list = DapperUtil.Query<SubjectNewProductRefModel>("ComBeziWfs_SWfsNewSubjectProductRef_SelectByCategoryNoProudctList", new { SubjectNo = subjectNo, CategoryNo = categoryNo }).ToList();
            list = list.GroupBy(p => p.ProductNo).Select(p => new SubjectNewProductRefModel()
            {
                SubjectNo = p.ElementAt(0).SubjectNo,
                SubjectTemplateType = p.ElementAt(0).SubjectTemplateType,
                Status = p.ElementAt(0).Status,
                CategoryNo = p.ElementAt(0).CategoryNo,
                SubjectProductRefId = p.ElementAt(0).SubjectProductRefId,
                PropertyValue = p.ElementAt(0).PropertyValue,
                TopFlag = p.ElementAt(0).TopFlag,
                BuyType = p.ElementAt(0).BuyType,
                ShowTime = p.ElementAt(0).ShowTime,
                IsShow = p.ElementAt(0).IsShow,
                DateCreate = p.ElementAt(0).DateCreate,
                ProductFlag = p.ElementAt(0).ProductFlag,
                ProductNo = p.ElementAt(0).ProductNo,
                ProductName = p.ElementAt(0).ProductName,
                ProductPicFile = p.ElementAt(0).ProductPicFile,
                ProductShowPic = p.ElementAt(0).ProductShowPic,
                GoodsNo = p.ElementAt(0).GoodsNo,
                ProductModel = p.ElementAt(0).ProductModel,
                MarketPrice = p.ElementAt(0).MarketPrice,
                SellPrice = p.ElementAt(0).SellPrice,
                GoldPrice = p.ElementAt(0).GoldPrice,
                PlatinumPrice = p.ElementAt(0).PlatinumPrice,
                DiamondPrice = p.ElementAt(0).DiamondPrice,
                LimitedPrice = p.ElementAt(0).LimitedPrice,
                StandardPrice = p.ElementAt(0).StandardPrice,
                PromotionPrice = p.ElementAt(0).PromotionPrice,
                ProductPromotionFlag = p.ElementAt(0).ProductPromotionFlag,
                IsShelf = p.ElementAt(0).IsShelf,
                PcSaleState = p.ElementAt(0).PcSaleState,
                BrandNo = p.ElementAt(0).BrandNo,
                BrandCnName = p.ElementAt(0).BrandCnName,
                BrandEnName = p.ElementAt(0).BrandEnName,
                DiscountShangpin = p.ElementAt(0).DiscountShangpin,
                MarketPriceRegion = p.Min(a => a.MarketPrice) + "~" + p.Max(a => a.MarketPrice),
                StandardPriceRegion = p.Min(a => a.StandardPrice) + "~" + p.Max(a => a.StandardPrice),
                PlatinumPriceRegion = p.Min(a => a.PlatinumPrice) + "~" + p.Max(a => a.PlatinumPrice),
                DiamondPriceRegion = p.Min(a => a.DiamondPrice) + "~" + p.Max(a => a.DiamondPrice),
                PromotionPriceRegion = p.Min(a => a.PromotionPrice) + "~" + p.Max(a => a.PromotionPrice),
                GoldPriceRegion = p.Min(a => a.GoldPrice) + "~" + p.Max(a => a.GoldPrice)
            }).ToList();
            return list;

        }
        #endregion

        #region 添加活动信息
        /// <summary>
        /// 添加创建活动
        /// </summary>
        /// <param name="mSWfsNewSubject"></param>
        /// <returns></returns>
        public int NewSubjectCreate(SWfsNewSubject mSWfsNewSubject)
        {
            return DapperUtil.Insert<SWfsNewSubject>(mSWfsNewSubject);
        }
        /// <summary>
        /// 编辑活动信息
        /// </summary>
        /// <param name="mSWfsNewSubject"></param>
        /// <returns></returns>
        public bool NewSubjectEdit(SWfsNewSubject mSWfsNewSubject)
        {
            return DapperUtil.Update<SWfsNewSubject>(mSWfsNewSubject);
        }
        #endregion
        #region 分组列表，商品添加等
        /// <summary>
        /// 更新试图模式商品排列信息
        /// </summary>
        /// <param name="mSWfsNewSubject"></param>
        /// <returns></returns>
        public int UpdateSWfsNewSubjectProductRefSort(int sort, string productNo, string categoryNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsNewSubjectProductRef_UpdateProudctSort", new { SortNo = sort, CategoryNo = categoryNo, ProductNo = productNo });
        }
        #endregion
        #region 分组管理
        /// <summary>
        /// 新建分组
        /// </summary>
        /// <param name="mSWfsNewSubjectCategory"></param>
        /// <returns></returns>
        public int NewGroupCreate(SWfsNewSubjectCategory mSWfsNewSubjectCategory)
        {
            return DapperUtil.Insert<SWfsNewSubjectCategory>(mSWfsNewSubjectCategory);
        }
        /// <summary>
        /// 编辑分组
        /// </summary>
        /// <param name="mSWfsNewSubjectCategory"></param>
        /// <returns></returns>
        public bool NewGroupEdit(SWfsNewSubjectCategory mSWfsNewSubjectCategory)
        {
            return DapperUtil.Update<SWfsNewSubjectCategory>(mSWfsNewSubjectCategory);
        }
        /// <summary>
        /// 获取一条分组信息
        /// </summary>
        /// <param name="groupNo"></param>
        /// <returns></returns>
        public SWfsNewSubjectCategory GetNewGroupInfo(string groupNo)
        {
            return DapperUtil.Query<SWfsNewSubjectCategory>("ComBeziWfs_SWfsNewSubjectCategory_SelectByGroupNo", new { GroupNo = groupNo }).FirstOrDefault();
        }
        /// <summary>
        /// 查询所有分组
        /// </summary>
        /// <param name="pageModel"></param>
        /// <returns></returns>
        public List<SWfsNewSubjectCategory> GetGroupList(ref PaginationInfoModel pageModel)
        {
            int totalcount = 0;
            if (string.IsNullOrEmpty(pageModel.OrderBy))
            { pageModel.OrderBy = " DateCreate desc "; }
            if (string.IsNullOrEmpty(pageModel.Field))
            { pageModel.Field = " *  "; }
            if (string.IsNullOrEmpty(pageModel.SelectField))
            { pageModel.SelectField = "*"; }
            if (string.IsNullOrEmpty(pageModel.Condition))
            { pageModel.Condition = ""; }
            object param = new { tableName = " SWfsNewSubjectCategory  with (NoLock) ", pageSize = pageModel.PageSize, curPage = pageModel.CurrentPage, orderBy = pageModel.OrderBy, selectfield = pageModel.SelectField, field = pageModel.Field, condition = pageModel.Condition };
            List<SWfsNewSubjectCategory> list = DapperUtil.QueryPageList<SWfsNewSubjectCategory>("ComBeziWfs_SWfsNewSubject_PageListSearch", ref totalcount, param).ToList();
            pageModel.TotalPage = CommonHelp.GetPageCount(pageModel.PageSize, totalcount);
            pageModel.TotalCount = totalcount;
            return list;
        }
        /// <summary>
        /// 根据活动获取一条分组信息
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public SWfsNewSubjectCategory GetSWfsNewSubjectCategorySingle(string subjectNo)
        {
            return DapperUtil.Query<SWfsNewSubjectCategory>("ComBeziWfs_SWfsNewSubjectCategory_SelectByCategoryNoSingle", new { SubjectNo = subjectNo }).FirstOrDefault();
        }
        #endregion

        #region 获取活动的数据统计信息
        /// <summary>
        /// 获取活动的数据统计信息 作废lxy 20130925
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="beginTime">查询时间段的开始时间</param>
        /// <param name="endTime">查询时间段的结束时间</param>
        /// <returns></returns>
        public SubjectDataStatistics GetStatisticList(string subjectNo, string rang, string beginTime, string endTime)
        {
            SubjectDataStatistics retObj = new SubjectDataStatistics();

            switch (rang)
            {
                case "0":
                    beginTime = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "1":
                    beginTime = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.Date.AddMilliseconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "2":
                    beginTime = DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "3":
                    beginTime = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                default:
                    break;//自定义的不去处理
            }

            retObj.subjectSaleStatistics = DapperUtil.Query<SubjectSaleStatistic>("ComBeziReport_SubjectSaleStatistics_SelectByConditon", new { SubjectNo = subjectNo, BeginTime = beginTime, EndTime = endTime, ListingOutletFlag = 1 }).ToList();
            retObj.subjectVisitStatistic = DapperUtil.Query<SubjectVisitStatistic>("ComBeziReport_SubjectVisitDataStatistics_SelectByConditon", new { SubjectNo = subjectNo, BeginTime = beginTime, EndTime = endTime, ListingOutletFlag = 1 }).ToList();
            retObj.subjectProductStatistics = DapperUtil.Query<SubjectProductStatistic>("ComBeziReport_SubjectProductDataStatistics_SelectByConditon", new { SubjectNo = subjectNo, BeginTime = beginTime, EndTime = endTime, ListingOutletFlag = 1 }).ToList();
            return retObj;
        }
        #endregion
        #region 判断展示渠道方法 获取渠道内容
        /// <summary>
        /// 判断展示渠道方法 获取渠道内容
        /// </summary>
        /// <param name="mSWfsNewSubject"></param>
        /// <returns></returns>
        public static string GetNewSubjectChannelTypeName(string channelType)
        {
            string channelValue = string.Empty;
            if (!string.IsNullOrEmpty(channelType))
            {
                string[] channelSp = channelType.Split(',');
                foreach (var item in channelSp)
                {
                    switch (item)
                    {
                        case "1"://网站
                            channelValue += ",网站";
                            break;
                        case "2"://移动端
                            channelValue += ",移动端";
                            break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(channelValue))
            { channelValue = channelValue.Substring(1); }
            return channelValue;
        }
        /// <summary>
        ///  //活动展示时间小时
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static string GetNewSubjectShowHours(DateTime startTime, DateTime endTime)
        {
            double TotalHour = 0;
            try
            {
                if (endTime.Year == 9999)
                {
                    TotalHour = 9999;
                }
                else
                {
                    TimeSpan ts1 = new TimeSpan(startTime.Ticks);
                    TimeSpan ts2 = new TimeSpan(endTime.Ticks);
                    TimeSpan ts = ts2.Subtract(ts1).Duration();
                    TotalHour = ts.TotalHours;
                }
            }
            catch { }
            return TotalHour.ToString();
        }
        /// <summary>
        ///  //活动展示时间天
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static string GetShowDays(DateTime startTime, DateTime endTime)
        {
            string totalDays = string.Empty;
            try
            {
                TimeSpan ts1 = new TimeSpan(startTime.Ticks);
                TimeSpan ts2 = new TimeSpan(endTime.Ticks);
                TimeSpan ts = ts2.Subtract(ts1).Duration();

                totalDays = ts.Days.ToString();

            }
            catch { }
            return totalDays;
        }
        #endregion


        #region 数据统计
        /*20130923修改，原为统计到截止日期总数，先修改为时间段内统计数量*/
        /// <summary>
        /// 获取返回时间
        /// </summary>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="EndTime"></param>
        private void getBeginAndEndTime(string rang, ref string beginTime, ref string endTime)
        {
            switch (rang)
            {
                case "1":
                    beginTime = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "2":
                    beginTime = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.Date.AddMilliseconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "3":
                    beginTime = DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "4":
                    beginTime = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.ToString("yyyy-MM-dd HH:mm:ss");
                    endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case "5":
                    if (string.IsNullOrEmpty(beginTime)) { beginTime = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd HH:mm:ss"); }
                    if (string.IsNullOrEmpty(endTime)) { endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
                    break;
                default:
                    if (string.IsNullOrEmpty(beginTime)) { beginTime = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd HH:mm:ss"); }
                    if (string.IsNullOrEmpty(endTime)) { endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
                    break;//自定义的不去处理
            }
        }
        /// <summary>
        /// 销售统计
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SubjectSaleStatistic GetSubjectSaleStatistic(string subjectNo, string rang, string beginTime, string endTime)
        {
            //根据rang获取开始，结束时间
            getBeginAndEndTime(rang, ref beginTime, ref endTime);
            //查询出当前销售信息，和以前最后一条销售信息
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("rang", rang);
            IList<SubjectSaleStatistic> list = DapperUtil.Query<SubjectSaleStatistic>("ComBeziReport_SubjectSaleStatistics_SelectByConditonListing", dic, new { SubjectNo = subjectNo, BeginTime = beginTime, EndTime = endTime }).ToList();
            SubjectSaleStatistic mSale = null;
            if (list != null && list.Count > 0)
            {
                mSale = new SubjectSaleStatistic();
                //如果rand为0 查询全部 时间倒叙返回一天信息
                if (rang == "0")
                {
                    mSale = list.FirstOrDefault();
                }
                else
                {
                    //rang不为0，如果返回一条信息，判断是否为要查询的信息，是就返回，否没有最新记录
                    if (list.Count == 1)
                    {
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mSale = list.FirstOrDefault();
                        }
                        else
                        {
                            mSale = null;
                        }
                    }
                    //如果是返回2条信息，判断哪条是最新信息，返回最新信息减去以前的最后一条信息
                    else if (list.Count == 2)
                    {
                        SubjectSaleStatistic mSaleNow = null;
                        SubjectSaleStatistic mSaleBef = null;
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mSaleNow = list[0];
                            mSaleBef = list[1];
                        }
                        else
                        {
                            mSaleBef = list[0];
                            mSaleNow = list[1];
                        }
                        mSale.SubjectNo = mSaleNow.SubjectNo;
                        mSale.DateStatistics = mSaleNow.DateStatistics;
                        mSale.OrderNums = mSaleNow.OrderNums - mSaleBef.OrderNums;
                        mSale.Amount = mSaleNow.Amount - mSaleBef.Amount;
                        mSale.CostAmount = mSaleNow.CostAmount - mSaleBef.CostAmount;
                        mSale.StockCount = mSaleNow.StockCount;
                        mSale.SKUCount = mSaleNow.SKUCount - mSaleBef.SKUCount;
                        mSale.SaleCount = mSaleNow.SaleCount - mSaleBef.SaleCount;
                        mSale.SaledSKUCount = mSaleNow.SaledSKUCount - mSaleBef.SaledSKUCount;
                    }
                    else
                    {
                        mSale = null;
                    }
                }
            }
            return mSale;

        }
        /// <summary>
        /// 访问统计
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SubjectVisitStatistic GetSubjectVisitStatistic(string subjectNo, string rang, string beginTime, string endTime)
        {
            //根据rang获取开始，结束时间
            getBeginAndEndTime(rang, ref beginTime, ref endTime);
            //查询出当前统计信息，和以前最后一条统计信息
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("rang", rang);
            IList<SubjectVisitStatistic> list = DapperUtil.Query<SubjectVisitStatistic>("ComBeziReport_SubjectVisitDataStatistics_SelectByConditonListing", dic, new { SubjectNo = subjectNo, BeginTime = beginTime, EndTime = endTime }).ToList();
            SubjectVisitStatistic mVisit = null;
            if (list != null && list.Count > 0)
            {
                mVisit = new SubjectVisitStatistic();
                //如果rand为0 查询全部 时间倒叙返回一天信息
                if (rang == "0")
                {
                    mVisit = list.FirstOrDefault();
                }
                else
                {
                    //rang不为0，如果返回一条信息，判断是否为要查询的信息，是就返回，否没有最新记录
                    if (list.Count == 1)
                    {
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mVisit = list.FirstOrDefault();
                        }
                        else
                        {
                            mVisit = null;
                        }
                    }
                    //如果是返回2条信息，判断哪条是最新信息，返回最新信息减去以前的最后一条信息
                    else if (list.Count == 2)
                    {
                        SubjectVisitStatistic mVisitNow = null;
                        SubjectVisitStatistic mVisitBef = null;
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mVisitNow = list[0];
                            mVisitBef = list[1];
                        }
                        else
                        {
                            mVisitBef = list[0];
                            mVisitNow = list[1];
                        }
                        mVisit.SubjectNo = mVisitNow.SubjectNo;
                        mVisit.DateStatistics = mVisitNow.DateStatistics;
                        mVisit.PV = mVisitNow.PV - mVisitBef.PV;
                        mVisit.UV = mVisitNow.UV - mVisitBef.UV;
                        mVisit.VIP = mVisitNow.VIP - mVisitBef.VIP;
                        mVisit.GoldenVIP = mVisitNow.GoldenVIP;
                        mVisit.PlatinaVIP = mVisitNow.PlatinaVIP - mVisitBef.PlatinaVIP;
                        mVisit.DiamondVIP = mVisitNow.DiamondVIP - mVisitBef.DiamondVIP;
                        mVisit.OrderNums = mVisitNow.OrderNums - mVisitBef.OrderNums;
                    }
                    else
                    {
                        mVisit = null;
                    }
                }
            }
            return mVisit;

        }
        public IList<SubjectProductStatistic> GetSubjectProductStatisticList(string subjectNo, string rang, string beginTime, string endTime)
        {
            //根据rang获取开始，结束时间
            getBeginAndEndTime(rang, ref beginTime, ref endTime);
            IList<SubjectProductStatistic> list = DapperUtil.Query<SubjectProductStatistic>("ComBeziReport_SubjectProductDataStatistics_SelectByConditonListing", new { SubjectNo = subjectNo, BeginTime = beginTime, EndTime = endTime }).ToList();
            if (rang == "0")
            {
                return list;
            }

            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    SubjectProductStatistic mProductStatistic = null;
                    mProductStatistic = DapperUtil.Query<SubjectProductStatistic>("ComBeziReport_SubjectProductDataStatistics_SelectBeforeProjectListing", new { SubjectNo = subjectNo, BeginTime = beginTime, BrandNo = item.BrandNo, CategoryNo = item.CategoryNo }).FirstOrDefault();
                    if (mProductStatistic != null)
                    {
                        item.SKUCount = item.SKUCount - mProductStatistic.SKUCount;
                        item.StockCount = item.StockCount - mProductStatistic.StockCount;
                        item.SaleCount = item.SaleCount - mProductStatistic.SaleCount;
                    }
                }

            }
            return list;
        }
        #endregion


        #region 新版列表更改信息20131007
        public IList<string> GetSWfsSubjectAllProductList_LxyAdd(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword)
        {
            string[] keywordNew = keyword.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("SCategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            IList<string> productList = DapperUtil.Query<string>("ComBeziWfs_WfsProduct_BatchSelectProductListNew_LXYADD", dic, new { KeyWord = keywordNew, SCategoryNo = scategoryNo, IsShelf = isShelf == null ? "" : isShelf, BrandNo = brandNo == null ? "" : brandNo, CategoryNo = categoryNo == null ? "" : categoryNo }).ToList();

            return productList;
        }
        public IList<string> GetSWfsSubjectAllProduct_LxyAdd(string categoryNo, string scategoryNo, string brandNo, string isShelf, string keyword, string genderStyle, decimal lpstartPrice, decimal lpendPrice)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("SCategoryNo", scategoryNo == null ? "" : scategoryNo);
            dic.Add("BrandNo", brandNo == null ? "" : brandNo);
            dic.Add("IsShelf", isShelf == null ? "" : isShelf);
            dic.Add("Keyword", keyword == null ? "" : keyword);
            dic.Add("GenderStyle", genderStyle == null ? "" : genderStyle);
            dic.Add("StartPrice", lpstartPrice <= 0 ? 0 : lpstartPrice);
            dic.Add("EndPrice", lpendPrice <= 0 ? 0 : lpendPrice);
            IList<string> productList = DapperUtil.Query<string>("ComBeziWfs_WfsProduct_SelectAllProductListNew_LXYADD", dic, new { KeyWord = keyword, SCategoryNo = scategoryNo, IsShelf = isShelf, BrandNo = brandNo, CategoryNo = categoryNo, GenderStyle = genderStyle, StartPrice = lpstartPrice, EndPrice = lpendPrice }).ToList();
            return productList;
        }
        /// <summary>
        /// 判断商品是否存在此服务中
        /// </summary>
        /// <param name="scategoryNo"></param>
        /// <param name="productNo"></param>
        /// <returns></returns>
        public IList<string> SelectSubjectProductRef_LxyAdd(string scategoryNo, string[] productNoS)
        {
            //string[] keywordNew = productNoS.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            IList<string> prList = DapperUtil.Query<string>("ComBeziWfs_SWfsNewSubjectProductRef_ReadByCategoryNo_ProductNoS", new { CategoryNo = scategoryNo, ProductNo = productNoS }).ToList();
            return prList;
        }
        #endregion


        #region 新增加，获取，女士，男士，中性
        /// <summary>
        /// 新增加，获取，女士，男士，中性
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, String> GetSubjectGenderList()
        {
            Dictionary<String, String> keyName = new Dictionary<String, String>();
            keyName.Add("0", "女士");// 女士
            keyName.Add("1", "男士");// 女士
            keyName.Add("2", "中性");// 女士 
            return keyName;
        }
        #endregion

        #region 获取ERP分类信息
        /// <summary>
        /// 获取ERP分类信息
        /// </summary>
        /// <param name="parentNo"></param>
        /// <returns></returns>
        public IList<ErpCategory> GetERPCategoryListByParentNo(string parentNo)
        {
            IList<ErpCategory> erpcategoryList = DapperUtil.Query<ErpCategory>("ComBeziWfs_SpCategory_SelectByParentNo", new { ParentNo = parentNo }).ToList();
            return erpcategoryList;
        }
        #endregion


        /// <summary>
        /// 获取活动列表中所有的商品信息 使用在管理商品更新数据信息
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public int GetSWfsNewSubject_UpdateProductCount(string subjectNo)
        {
            int updateCount = DapperUtil.Query<string>("ComBeziWfs_SWfsNewSubjectProductRef_GetReadyUpdateProductCount", new { SubjectNo = subjectNo }).Distinct().Count();
            return DapperUtil.Execute("ComBeziWfs_SWfsNewSubjectProductRef_UpdateProductCount", new { SubjectNo = subjectNo, ProductCount = updateCount });
        }

        #region 导出ToExcel
        /// <summary>
        /// 导出ToExcel活动商品列表导出
        /// </summary>
        /// <param name="subjectNo"></param>
        public void GetSubjectProductToExcel(SWfsNewSubject subjectSingle)
        {
            #region 导出excel 信息 缺少支付金额

            IList<SubjectNewProductRefModel> productList = GetSWfsNewSubjectProductRefListAll(subjectSingle.SubjectNo);
            SWfsNewSubjectService newSubjectService = new SWfsNewSubjectService();
            IList<SWfsNewSubjectCategory> subjectList = newSubjectService.GetSWfsNewSubjectCategoryListBySubjectNo(subjectSingle.SubjectNo);


            StringBuilder sb = new StringBuilder("<h2>活动名称：" + subjectSingle.SubjectName + "</h2><h2>活动编号：" + subjectSingle.SubjectNo + "</h2><h2>活动商品</h2><table cellpadding=\"0\" cellspacing=\"1\" border=\"1\"  width=\"758px\" id=\"AccountListTable\" >");

            sb.AppendLine("<tr style=\"background-color:#FFFF00;\">");
            sb.AppendLine("<td><span>分组名称</span></td> ");
            sb.AppendLine("<td width=\"10%\"><span>商品编号</span></td>");
            sb.AppendLine("<td><span>商品名</span></td>");
            sb.AppendLine("<td><span>品牌</span></td> ");

            sb.AppendLine("</tr>");
            foreach (var item in subjectList)
            {

                IList<SubjectNewProductRefModel> pdSearchList = (from p in productList where p.CategoryNo.Equals(item.CategoryNo) select p).ToList();
                foreach (SubjectNewProductRefModel psingle in pdSearchList)
                {
                    #region 导出excel格式模板

                    string brandName = string.IsNullOrEmpty(psingle.BrandEnName) == true ? psingle.BrandCnName : psingle.BrandEnName;
                    sb.AppendLine("<tr>");
                    if (subjectSingle.SubjectTemplateType == 2)
                    {
                        sb.AppendLine(String.Format("<td>{0}</td>", item.CategoryName));
                    }
                    else { sb.AppendLine(String.Format("<td>{0}</td>", "")); }
                    sb.AppendLine(String.Format("<td> {0}</td>", psingle.ProductNo + "&nbsp;"));
                    sb.AppendLine(String.Format("<td> {0}</td>", psingle.ProductName));
                    sb.AppendLine(String.Format("<td>{0}</td>", brandName));

                    sb.AppendLine("</tr>");
                    #endregion
                }
            }


            sb.AppendLine("</table>");

            CommonHelp.OutputExcel(sb.ToString(), "活动：" + subjectSingle.SubjectName + "/" + subjectSingle.SubjectNo + "/" + DateTime.Now.ToString("yyyy-MM-dd"));

            #endregion
        }
        #endregion

        #region 导出ToExcel活动报表数据
        /// <summary>
        /// 导出ToExcel活动商品列表导出
        /// </summary>
        /// <param name="subjectNo"></param>
        public void GetSubjectDataStatisticsToExcel(IList<SubjectProductStatistic> productList, SubjectSaleStatistic sale, SubjectVisitStatistic Visit, SWfsNewSubject subjectSingle)
        {
            #region 导出excel 信息 缺少支付金额
            StringBuilder sb = new StringBuilder("<h1>活动报表数据</h1><h2>活动名称：" + subjectSingle.SubjectName + "</h2><h2>活动编号：" + subjectSingle.SubjectNo + "</h2>");
            if (sale != null)
            {
                sb.AppendLine("<h2>销售统计</h2>");
                sb.AppendLine("<table cellpadding=\"0\" cellspacing=\"1\" border=\"1\"  width=\"758px\" id=\"AccountListTable\" >");
                sb.AppendLine("<tr style=\"background-color:#FFFF00;\">");
                sb.AppendLine("<td width=\"10%\"><span>日期</span></td>");
                sb.AppendLine("<td><span>成功订单数</span></td>");
                sb.AppendLine("<td><span>成功订单金额</span></td> ");
                sb.AppendLine("<td><span>总成本价</span></td> ");
                sb.AppendLine("<td><span>成功订单平均毛利率</span></td> ");
                sb.AppendLine("<td><span>成功订单总利润</span></td> ");
                sb.AppendLine("<td><span>总库存</span></td> ");
                sb.AppendLine("<td><span>总单款数</span></td> ");
                sb.AppendLine("<td><span>售罄率</span></td> ");
                sb.AppendLine("<td><span>动销率</span></td> ");
                sb.AppendLine("</tr>");
                #region 导出excel格式模板
                sb.AppendLine("<tr>");
                sb.AppendLine(String.Format("<td> {0}</td>", sale.DateStatistics));
                sb.AppendLine(String.Format("<td> {0}</td>", sale.OrderNums));
                sb.AppendLine(String.Format("<td>{0}</td>", sale.Amount));

                sb.AppendLine(String.Format("<td> {0}</td>", sale.CostAmount));
                sb.AppendLine(String.Format("<td> {0}</td>", sale.Amount == 0 ? "0" : (((sale.Amount - sale.CostAmount) / sale.Amount) * 100).ToString(".##") + "%"));
                sb.AppendLine(String.Format("<td>{0}</td>", sale.Amount - sale.CostAmount));

                sb.AppendLine(String.Format("<td> {0}</td>", sale.StockCount + sale.SaleCount));
                sb.AppendLine(String.Format("<td> {0}</td>", sale.SKUCount));
                sb.AppendLine(String.Format("<td>{0}</td>", DivideHelper.Divide(sale.SaleCount, sale.StockCount + sale.SaleCount, 2)));
                sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide(sale.SaledSKUCount, sale.SKUCount, 2)));

                sb.AppendLine("</tr>");
                #endregion

                sb.AppendLine("</table>");
            }

            if (sale != null)
            {
                sb.AppendLine("<h2>访问统计</h2>");
                sb.AppendLine("<table cellpadding=\"0\" cellspacing=\"1\" border=\"1\"  width=\"758px\" id=\"AccountListTable\" >");
                sb.AppendLine("<tr style=\"background-color:#FFFF00;\">");
                sb.AppendLine("<td width=\"10%\"><span>日期</span></td>");
                sb.AppendLine("<td><span>PV</span></td>");
                sb.AppendLine("<td><span>UV</span></td> ");
                sb.AppendLine("<td><span>普通会员</span></td> ");
                sb.AppendLine("<td><span>黄金会员</span></td> ");
                sb.AppendLine("<td><span>白金会员</span></td> ");
                sb.AppendLine("<td><span>钻石会员</span></td> ");
                sb.AppendLine("<td><span>订单转化率</span></td> ");
                sb.AppendLine("<td><span>会员平均订单</span></td> ");
                sb.AppendLine("</tr>");
                #region 导出excel格式模板
                sb.AppendLine("<tr>");
                sb.AppendLine(String.Format("<td> {0}</td>", Visit.DateStatistics));
                sb.AppendLine(String.Format("<td> {0}</td>", Visit.PV));
                sb.AppendLine(String.Format("<td>{0}</td>", Visit.UV));

                sb.AppendLine(String.Format("<td> {0}</td>", Visit.VIP));
                sb.AppendLine(String.Format("<td> {0}</td>", Visit.GoldenVIP));
                sb.AppendLine(String.Format("<td>{0}</td>", Visit.PlatinaVIP));

                sb.AppendLine(String.Format("<td> {0}</td>", Visit.DiamondVIP));
                sb.AppendLine(String.Format("<td>{0}</td>", DivideHelper.Divide((int)Visit.OrderNums, (int)Visit.UV, 2)));
                sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide((int)Visit.OrderNums, (int)(Visit.VIP + Visit.GoldenVIP + Visit.PlatinaVIP + Visit.DiamondVIP), 2)));
                sb.AppendLine("</tr>");
                #endregion

                sb.AppendLine("</table>");
            }

            if (productList != null)
            {
                sb.AppendLine("<h2>商品统计</h2>");
                sb.AppendLine("<table cellpadding=\"0\" cellspacing=\"1\" border=\"1\"  width=\"758px\" id=\"AccountListTable\" >");
                sb.AppendLine("<tr style=\"background-color:#FFFF00;\">");
                sb.AppendLine("<td width=\"10%\"><span>品牌名称</span></td>");
                sb.AppendLine("<td><span>品类</span></td>");
                sb.AppendLine("<td><span>商品数量</span></td> ");
                sb.AppendLine("<td><span>总库存</span></td> ");
                sb.AppendLine("<td><span>售罄率</span></td> ");
                sb.AppendLine("<td><span>总售罄率</span></td> ");
                sb.AppendLine("</tr>");
                #region 导出excel格式模板

                foreach (SubjectProductStatistic item in productList)
                {
                    sb.AppendLine("<tr>");
                    sb.AppendLine(String.Format("<td> {0}</td>", item.BrandName));
                    sb.AppendLine(String.Format("<td> {0}</td>", item.CategoryName));
                    sb.AppendLine(String.Format("<td>{0}</td>", item.SKUCount));

                    sb.AppendLine(String.Format("<td> {0}</td>", item.StockCount + item.SaleCount));
                    sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide(item.SaleCount, item.StockCount + item.SaleCount, 2)));
                    sb.AppendLine(String.Format("<td>{0}</td>", DivideHelper.Divide(item.SaleCount, item.StockCount + item.SaleCount, 2)));
                    sb.AppendLine("</tr>");
                }
                #endregion

                sb.AppendLine("</table>");
            }


            CommonHelp.OutputExcel(sb.ToString(), "活动报表数据(" + subjectSingle.SubjectName + "/" + subjectSingle.SubjectNo + "/" + DateTime.Now.ToString("yyyy-MM-dd") + ")");

            #endregion
        }
        #endregion

        #region 清除缓存数据
        //批量清除数据活动  
        /// <summary>
        /// 批量清除数据活动
        /// </summary>
        /// <param name="subjectNo">活动编号</param>
        public void ClearSubjectListRedisCache(string subjectNo)
        {
            int count = 0;
            #region 清除缓存
            if (!string.IsNullOrEmpty(subjectNo))
            {
                string redisCache = AppSettingManager.AppSettings["RedisCacheProviderKeyShow"];
                string redisCacheTopic = AppSettingManager.AppSettings["RedisCacheNewSubjectTopicNo"];

                RedisCacheProvider.SetRedisCluster(redisCache);
                var instance = RedisCacheProvider.Instance;
                if (!string.IsNullOrEmpty(subjectNo))
                {
                    string redisCacheTopicNo = redisCacheTopic + subjectNo;
                    if (instance.Get<object>(redisCacheTopicNo) != null)
                    {
                        instance.Remove(redisCacheTopicNo);
                        count = count + 1;
                    }
                }

            }
            #endregion


        }
        #endregion

        #region 获取价格类型数据
        /// <summary>
        /// 获取价格类型数据
        /// </summary>
        /// <param name="priceType">价格类型信息，预热页面等等</param>
        /// <returns></returns>
        public IList<SWfsNewSubjectPriceType> GetSWfsNewSubjectPriceTypeList(int priceType)
        {
            IList<SWfsNewSubjectPriceType> prList = DapperUtil.Query<SWfsNewSubjectPriceType>("ComBeziWfs_SWfsNewSubjectPriceType_SelectList", new { PriceType = priceType }).ToList();
            return prList;
        }
        #endregion
        #region 添加价格类型数据
        /// <summary>
        /// 添加价格类型数据
        /// </summary>
        /// <param name="mSWfsNewSubject"></param>
        /// <returns></returns>
        public int CreateSWfsNewSubjectPriceType(SWfsNewSubjectPriceType newSubjectPriceType)
        {
            return DapperUtil.Insert<SWfsNewSubjectPriceType>(newSubjectPriceType, true);
        }
        #endregion

        #region 获取价格类型数据是否重复
        /// <summary>
        /// 添加价格类型数据
        /// </summary>
        /// <param name="mSWfsNewSubject"></param>
        /// <returns></returns>
        public int SelectSingleSWfsNewSubjectPriceType(string priceTag)
        {
            int prSingle = DapperUtil.Query<int>("ComBeziWfs_SWfsNewSubjectPriceType_SelectSingle", new { PriceTag = priceTag }).FirstOrDefault();
            return prSingle;
        }

        #endregion

        #region 根据活动ID查询订单信息 导出EXCEl
        /// <summary>
        /// 根据活动ID查询订单信息 导出EXCEl
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<ORderToExcel> GetOrderByNewSubject(string subjectNo)
        {
            return DapperUtil.Query<ORderToExcel>("ComBeziWfs_WfsOrder_GetOrderBySWfsNewSubject", new { SubjectNo = subjectNo }).ToList();
        }
        #endregion


        #region 导出ToExcel 活动订单数据列表导出
        /// <summary>
        /// 导出ToExcel  活动订单数据列表导出
        /// </summary>
        /// <param name="subjectNo"></param>
        public void GetSubjectOrderSumToExcel(IList<ORderToExcel> orderList, SWfsNewSubject subjectSingle)
        {
            #region 导出excel 信息 缺少支付金额
            StringBuilder sb = new StringBuilder("<h2>活动名称：" + subjectSingle.SubjectName + "</h2><h2>活动编号：" + subjectSingle.SubjectNo + "</h2><h2>活动商品</h2><table cellpadding=\"0\" cellspacing=\"1\" border=\"1\"  width=\"758px\" id=\"AccountListTable\" >");
            sb.AppendLine("<tr style=\"background-color:#FFFF00;\">");
            sb.AppendLine("<td width=\"10%\"><span>活动编号</span></td>");
            sb.AppendLine("<td><span>活动名称</span></td>");
            sb.AppendLine("<td><span>订单编号</span></td> ");
            sb.AppendLine("<td><span>订单状态</span></td> ");
            sb.AppendLine("<td><span>商品编号</span></td> ");
            sb.AppendLine("<td><span>商品名称</span></td> ");
            sb.AppendLine("</tr>");

            if (orderList != null)
            {
                foreach (ORderToExcel orderSingle in orderList)
                {
                    #region 导出excel格式模板
                    sb.AppendLine("<tr>");
                    sb.AppendLine(String.Format("<td> {0}</td>", orderSingle.SubjectNo));
                    sb.AppendLine(String.Format("<td> {0}</td>", orderSingle.SubjectName));
                    sb.AppendLine(String.Format("<td>{0}</td>", orderSingle.OrderNo));
                    sb.AppendLine(String.Format("<td> {0}</td>", orderSingle.status));
                    sb.AppendLine(String.Format("<td> {0}</td>", orderSingle.productNo + "&nbsp;"));
                    sb.AppendLine(String.Format("<td>{0}</td>", orderSingle.ProductName));
                    sb.AppendLine("</tr>");
                    #endregion
                }
            }
            sb.AppendLine("</table>");



            CommonHelp.OutputExcel(sb.ToString(), "订单商品(" + subjectSingle.SubjectName + "/" + subjectSingle.SubjectNo + "/" + DateTime.Now.ToString("yyyy-MM-dd") + ")");

            #endregion
        }
        #endregion


        #region 活动--新统计数据方法 20140302添加最新方法lxy
        /// <summary>
        /// 销售统计
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SubjectSaleStatistic GetSubjectSaleStatisticNew(string subjectNo, string rang, string beginTime, string endTime)
        {
            rang = "1";
            //根据rang获取开始，结束时间
            if (string.IsNullOrEmpty(beginTime)) { rang = "0"; beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
            if (string.IsNullOrEmpty(endTime)) { rang = "0"; endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
            //查询出当前销售信息，和以前最后一条销售信息
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("rang", rang);
            IList<SubjectSaleStatistic> list = DapperUtil.Query<SubjectSaleStatistic>("ComBeziReport_SubjectSaleStatistics_SelectByConditonListingNew", dic, new { SubjectNo = subjectNo, BeginTime = beginTime, EndTime = endTime }).ToList();
            SubjectSaleStatistic mSale = null;
            if (list != null && list.Count > 0)
            {
                mSale = new SubjectSaleStatistic();
                //如果rand为0 查询全部 时间倒叙返回一天信息
                if (rang == "0")
                {
                    mSale = list.FirstOrDefault();
                }
                else
                {
                    //rang不为0，如果返回一条信息，判断是否为要查询的信息，是就返回，否没有最新记录
                    if (list.Count == 1)
                    {
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mSale = list.FirstOrDefault();
                        }
                        else
                        {
                            mSale = null;
                        }
                    }
                    //如果是返回2条信息，判断哪条是最新信息，返回最新信息减去以前的最后一条信息
                    else if (list.Count == 2)
                    {
                        SubjectSaleStatistic mSaleNow = null;
                        SubjectSaleStatistic mSaleBef = null;
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mSaleNow = list[0];
                            mSaleBef = list[1];
                        }
                        else
                        {
                            mSaleBef = list[0];
                            mSaleNow = list[1];
                        }
                        mSale.SubjectNo = mSaleNow.SubjectNo;
                        mSale.DateStatistics = mSaleNow.DateStatistics;
                        mSale.OrderNums = mSaleNow.OrderNums - mSaleBef.OrderNums;
                        mSale.Amount = mSaleNow.Amount - mSaleBef.Amount;
                        mSale.CostAmount = mSaleNow.CostAmount - mSaleBef.CostAmount;
                        mSale.StockCount = mSaleNow.StockCount;
                        mSale.SKUCount = mSaleNow.SKUCount - mSaleBef.SKUCount;
                        mSale.SaleCount = mSaleNow.SaleCount - mSaleBef.SaleCount;
                        mSale.SaledSKUCount = mSaleNow.SaledSKUCount - mSaleBef.SaledSKUCount;
                    }
                    else
                    {
                        mSale = null;
                    }
                }
            }
            return mSale;

        }

        /// <summary>
        /// 访问统计
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <param name="rang"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public SubjectVisitStatistic GetSubjectVisitStatisticNew(string subjectNo, string rang, string beginTime, string endTime)
        {
            rang = "1";
            //根据rang获取开始，结束时间
            if (string.IsNullOrEmpty(beginTime)) { rang = "0"; beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
            if (string.IsNullOrEmpty(endTime)) { rang = "0"; endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
            //查询出当前统计信息，和以前最后一条统计信息
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("rang", rang);
            IList<SubjectVisitStatistic> list = DapperUtil.Query<SubjectVisitStatistic>("ComBeziReport_SubjectVisitDataStatistics_SelectByConditonListingNew", dic, new { SubjectNo = subjectNo, BeginTime = beginTime, EndTime = endTime }).ToList();
            SubjectVisitStatistic mVisit = null;
            if (list != null && list.Count > 0)
            {
                mVisit = new SubjectVisitStatistic();
                //如果rand为0 查询全部 时间倒叙返回一天信息
                if (rang == "0")
                {
                    mVisit = list.FirstOrDefault();
                }
                else
                {
                    //rang不为0，如果返回一条信息，判断是否为要查询的信息，是就返回，否没有最新记录
                    if (list.Count == 1)
                    {
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mVisit = list.FirstOrDefault();
                        }
                        else
                        {
                            mVisit = null;
                        }
                    }
                    //如果是返回2条信息，判断哪条是最新信息，返回最新信息减去以前的最后一条信息
                    else if (list.Count == 2)
                    {
                        SubjectVisitStatistic mVisitNow = null;
                        SubjectVisitStatistic mVisitBef = null;
                        if (list[0].DateStatistics >= Convert.ToDateTime(beginTime) && list[0].DateStatistics <= Convert.ToDateTime(endTime))
                        {
                            mVisitNow = list[0];
                            mVisitBef = list[1];
                        }
                        else
                        {
                            mVisitBef = list[0];
                            mVisitNow = list[1];
                        }
                        mVisit.SubjectNo = mVisitNow.SubjectNo;
                        mVisit.DateStatistics = mVisitNow.DateStatistics;
                        mVisit.PV = mVisitNow.PV - mVisitBef.PV;
                        mVisit.UV = mVisitNow.UV - mVisitBef.UV;
                        mVisit.VIP = mVisitNow.VIP - mVisitBef.VIP;
                        mVisit.GoldenVIP = mVisitNow.GoldenVIP;
                        mVisit.PlatinaVIP = mVisitNow.PlatinaVIP - mVisitBef.PlatinaVIP;
                        mVisit.DiamondVIP = mVisitNow.DiamondVIP - mVisitBef.DiamondVIP;
                        mVisit.OrderNums = mVisitNow.OrderNums - mVisitBef.OrderNums;
                    }
                    else
                    {
                        mVisit = null;
                    }
                }
            }
            return mVisit;

        }
        /// <summary>
        /// 读取活动查询方法
        /// </summary>
        /// <param name="scategoryNo"></param>
        /// <returns></returns>
        public IList<SWfsNewSubject> SelectSubjectList(string bgTime, string edTime, string subjectNos, string type)
        {
            if (type == "1" && string.IsNullOrEmpty(bgTime) && string.IsNullOrEmpty(edTime) && string.IsNullOrEmpty(subjectNos))
            { return null; }
            var dic = new Dictionary<string, object>();
            dic.Add("TypeValue", type);
            string[] subjectNosNew = subjectNos.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            IList<SWfsNewSubject> splist = DapperUtil.Query<SWfsNewSubject>("ComBeziWfs_SWfsNewSubject_GetSWfsNewSubjectByDataStatisticsList", dic, new { TypeValue = type, SubjectNos = subjectNosNew, BeginTime = bgTime, EndTime = edTime }).ToList();
            return splist;
        }
        #endregion

        #region 导出ToExcel活动报表数据新活动报表规则20140302 lxy
        /// <summary>
        /// 导出ToExcel活动报表数据新活动报表规则20140302 lxy
        /// </summary>
        /// <param name="subjectNo"></param>
        public void GetSubjectDataStatisticsToExcelNew(SubjectNewStatisticInfo newStatisticList)
        {
            #region 导出excel 信息 缺少支付金额
            StringBuilder sb = new StringBuilder("<h1>活动报表数据</h1>");
            if (newStatisticList != null)
            {
                sb.AppendLine("<h2>销售统计</h2>");
                sb.AppendLine("<table cellpadding=\"0\" cellspacing=\"1\" border=\"1\"  width=\"758px\" id=\"AccountListTable\" >");
                sb.AppendLine("<tr style=\"background-color:#FFFF00;\">");
                sb.AppendLine("<td width=\"10%\"><span>活动编号</span></td>");
                sb.AppendLine("<td><span>品类</span></td>");
                sb.AppendLine("<td><span>BU</span></td> ");
                sb.AppendLine("<td><span>创建人</span></td> ");
                sb.AppendLine("<td><span>状态</span></td> ");
                sb.AppendLine("<td><span>时间</span></td> ");
                sb.AppendLine("<td><span>成功订单</span></td> ");
                sb.AppendLine("<td><span>成功订单金额</span></td> ");
                sb.AppendLine("<td><span>订单转化率</span></td> ");
                sb.AppendLine("<td><span>总成本价</span></td> ");
                sb.AppendLine("<td><span>平均毛利率</span></td> ");
                sb.AppendLine("<td><span>总库存</span></td> ");

                sb.AppendLine("<td><span>可销售金额</span></td> ");
                sb.AppendLine("<td><span>总单款数</span></td> ");
                sb.AppendLine("<td><span>售罄率</span></td> ");
                sb.AppendLine("<td><span>金额售罄率</span></td> ");
                sb.AppendLine("<td><span>动销率</span></td> ");

                sb.AppendLine("<td><span>UV</span></td> ");
                sb.AppendLine("<td><span>PV</span></td> ");
                sb.AppendLine("<td><span>UV产值</span></td> ");
                sb.AppendLine("<td><span>会员数量</span></td> ");
                sb.AppendLine("</tr>");
                if (newStatisticList.SubjectNewStatisticList != null)
                {
                    #region 导出excel格式模板
                    foreach (var statisticSingle in newStatisticList.SubjectNewStatisticList)
                    {
                        var Visit = statisticSingle.VisitStatistic;
                        var Sale = statisticSingle.SaleStatistic;
                        string erpCategoryName = string.Empty;
                        if (statisticSingle.NewSubject.SubjectCategoryNo.IsNullOrEmpty() == false)
                        {
                            string[] channelList = statisticSingle.NewSubject.SubjectCategoryNo.Split(',');
                            if (null != channelList && channelList.Length > 0)
                            {
                                foreach (var item in channelList)
                                {
                                    ErpCategory erpSingle = newStatisticList.ErpCategoryList.SingleOrDefault(p => p.CategoryNo == item);
                                    erpCategoryName = erpCategoryName + erpSingle.CategoryName + "<br/>";
                                }
                            }
                        }

                        sb.AppendLine("<tr>");
                        sb.AppendLine(String.Format("<td> {0}</td>", statisticSingle.NewSubject.SubjectNo));
                        sb.AppendLine(String.Format("<td> {0}</td>", erpCategoryName));
                        sb.AppendLine(String.Format("<td> {0}</td>", statisticSingle.NewSubject.SubjectBU == 6 ? "其他" : "BU" + statisticSingle.NewSubject.SubjectBU));
                        sb.AppendLine(String.Format("<td> {0}</td>", statisticSingle.NewSubject.CreateUserId));
                        sb.AppendLine(String.Format("<td> {0}</td>", statisticSingle.NewSubject.Status == 1 ? "开启" : "关闭"));
                        sb.AppendLine(String.Format("<td style=\"text-align:left;white-space:nowrap;\"> {0}</td>", "预热时间:" + (statisticSingle.NewSubject.DatePre.ToString().Contains("9999") == true ? "" : "" + statisticSingle.NewSubject.DatePre.ToString()) + "<br />开始时间：" + statisticSingle.NewSubject.DateBegin + " <br />关闭时间：" + statisticSingle.NewSubject.DateEnd));
                        sb.AppendLine(String.Format("<td> {0}</td>", Sale.OrderNums));
                        sb.AppendLine(String.Format("<td> {0}</td>", Sale.Amount));
                        sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide((int)Visit.OrderNums, (int)Visit.UV, 2)));
                        sb.AppendLine(String.Format("<td> {0}</td>", Sale.CostAmount));
                        sb.AppendLine(String.Format("<td> {0}</td>", (Sale.Amount == 0 ? "0" : (((Sale.Amount - Sale.CostAmount) / Sale.Amount) * 100).ToString(".##") + "%")));
                        sb.AppendLine(String.Format("<td> {0}</td>", (Sale.StockCount + Sale.SaleCount)));
                        sb.AppendLine(String.Format("<td> {0}</td>", Sale.StockTotalAmount));
                        sb.AppendLine(String.Format("<td> {0}</td>", Sale.SKUCount));
                        sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide(Sale.SaleCount, Sale.StockCount + Sale.SaleCount, 2)));
                        sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide(Sale.Amount, (int)Sale.StockTotalAmount, 2)));
                        sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide(Sale.SaledSKUCount, Sale.SKUCount, 2)));
                        sb.AppendLine(String.Format("<td> {0}</td>", Visit.UV));
                        sb.AppendLine(String.Format("<td> {0}</td>", Visit.PV));
                        sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide((int)(Sale.Amount), Visit.UV, 2)));
                        sb.AppendLine(String.Format("<td style=\"text-align:left;white-space:nowrap;\"> {0}</td>", "普通会员:" + Visit.VIP + "<br />黄金会员:" + Visit.GoldenVIP + "<br />白金会员:" + Visit.PlatinaVIP + "<br />钻石会员:" + Visit.DiamondVIP));


                        sb.AppendLine("</tr>");
                    }

                    #endregion
                }

                sb.AppendLine("<tr>");
                sb.AppendLine("<td colspan=\"22\" style=\"text-align:left; font-size:13px; font-weight:bold; border-bottom:1px; border-bottom-style:inherit; border-bottom-color:Black;\">统计</td>");
                sb.AppendLine("</tr>");

                if (newStatisticList.SaleStatistic != null || newStatisticList.VisitStatistic != null)
                {
                    int subjectCount = newStatisticList.SubjectCount;
                    var Visit = newStatisticList.VisitStatistic;
                    var Sale = newStatisticList.SaleStatistic;
                    sb.AppendLine("<tr>");
                    sb.AppendLine("<td>合计：</td>");
                    sb.AppendLine(" <td></td>");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.OrderNums));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.Amount));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.CostAmount));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", (Sale.StockCount + Sale.SaleCount)));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.StockTotalAmount));
                    sb.AppendLine(String.Format("<td> {0}</td>", Sale.SKUCount));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine(String.Format("<td> {0}</td>", Visit.UV));
                    sb.AppendLine(String.Format("<td> {0}</td>", Visit.PV));
                    sb.AppendLine(String.Format("<td> {0}</td>", DivideHelper.Divide((int)(Sale.Amount), Visit.UV, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine("</tr>");

                    sb.AppendLine("<tr>");
                    sb.AppendLine("<td>平均：</td>");
                    sb.AppendLine(" <td></td>");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");
                    sb.AppendLine(" <td></td> ");

                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)Sale.OrderNums, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)Sale.Amount, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.DivideDecimal((int)Visit.OrderNums, (int)Visit.UV, 2) / subjectCount).ToString() + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)Sale.CostAmount, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", (Sale.Amount == 0 || subjectCount <= 0) ? "0" : (((Sale.Amount - Sale.CostAmount) / Sale.Amount) * 100).ToString(".##") + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)(Sale.StockCount + Sale.SaleCount), subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)(Sale.StockTotalAmount), subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue((int)(Sale.SKUCount), subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.DivideDecimal(Sale.SaleCount, Sale.StockCount + Sale.SaleCount, 2) / subjectCount).ToString() + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.DivideDecimal(Sale.Amount, (int)Sale.StockTotalAmount, 2) / subjectCount).ToString() + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.DivideDecimal(Sale.SaledSKUCount, Sale.SKUCount, 2) / subjectCount).ToString() + "%"));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue(Visit.UV, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? 0 : DivideHelper.DivideDecimalValue(Visit.PV, subjectCount, 2)));
                    sb.AppendLine(String.Format("<td> {0}</td>", subjectCount <= 0 ? "0" : (DivideHelper.Divide((int)(Sale.Amount), Visit.UV, 2)).ToString()));
                    sb.AppendLine(String.Format("<td> {0}</td>", ""));
                    sb.AppendLine("</tr>");

                }
                sb.AppendLine("</table>");
            }





            CommonHelp.OutputExcel(sb.ToString(), "ReportTOTAL" + DateTime.Now.ToString());

            #endregion
        }
        #endregion


        #region 活动列表的基本筛选方法
        /// <summary>
        /// 获取BU/品牌/品类筛选数据 by zhangwei 20140306
        /// </summary>
        /// <param name="list">所有商品统计数据</param>
        /// <param name="buDic">BU</param>
        /// <param name="brandDic">品牌</param>
        /// <param name="categoryDic">品类</param>
        public void NewSubjectFilterStatisticData(IList<SubjectProductStatistic> list, ref Dictionary<string, string> buDic, ref Dictionary<string, string> brandDic, ref Dictionary<string, string> categoryDic)
        {
            if (list != null && list.Count() > 0)
            {
                //BU
                var tempBulist = (from b in list group b by new { b.BU } into g select new { g.Key, g }).ToList();
                if (tempBulist != null && tempBulist.Count() > 0)
                {
                    foreach (var item in tempBulist)
                    {
                        buDic.Add(item.Key.BU, item.Key.BU);
                    }
                }

                //品牌
                var tempBrandlist = (from b in list group b by new { b.BrandNo, b.BrandName } into g select new { g.Key, g }).ToList();
                if (tempBrandlist != null && tempBrandlist.Count() > 0)
                {
                    foreach (var item in tempBrandlist)
                    {
                        brandDic.Add(item.Key.BrandNo, item.Key.BrandName);
                    }
                }

                //品类
                var tempCategorylist = (from c in list group c by new { c.CategoryNo, c.CategoryName } into g select new { g.Key, g }).ToList();
                if (tempCategorylist != null && tempCategorylist.Count() > 0)
                {
                    foreach (var item in tempCategorylist)
                    {
                        categoryDic.Add(item.Key.CategoryNo, item.Key.CategoryName);
                    }
                }

            }
        }

        #endregion

        #region 活动列表商品属性值，名称 lxy
        /// <summary>
        /// 活动列表商品属性值，名称 lxy
        /// </summary>
        /// <returns></returns>
        public static string GetSubjectRefProductByProperty(int pValue)
        {
            //商品属性值 0普通，1新品，2限时,(新增)3 预热商品
            string pName = string.Empty;
            switch (pValue)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    pName = "预热商品";
                    break;
            }
            return pName;
        }
        #endregion



        #region 删除活动中的商品信息
        /// <summary>
        /// 获取活动列表中所有的商品编号 删除活动中的商品信息
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public IList<string> GetSWfsNewSubjectProductRefListByDelete(string subjectNo)
        {
            return DapperUtil.Query<string>("ComBeziWfs_SWfsNewSubjectProductRef_SelectByCategoryNoProudctNoDelete", new { SubjectNo = subjectNo }).Distinct().ToList();
        }
        #endregion


        #region by tianxiuquan 2014-10-12 新改版活动
        /// <summary>
        /// 查询活动列表页
        /// </summary>
        /// <param name="subjectName">活动名称</param>
        /// <param name="productNo">商品编号</param>
        /// <param name="subjectNo">活动编号</param>
        /// <param name="createUser">活动创建人</param>
        /// <param name="subjectStatus">活动状态</param>
        /// <param name="subjectErpCategory">活动所属ERP分类</param>
        /// <param name="channelNo">活动所属渠道(0网站移动 1网站2移动)</param>
        /// <param name="gender">活动所属性别(0女士1男士2中性)</param>
        /// <param name="startTime">活动开始时间</param>
        /// <param name="endTime">活动结束时间</param>
        /// <returns></returns>
        public IEnumerable<SWfsNewSubject> GetSubjectIndexList(string subjectNameOrSubjectNo,string productNo,
            string createUserId, string subjectStatus, string subjectCategoryNo,
            string channelType, string subjectGender, string subjectDateEndStart, string subjectDateEndEnd,int pageIndex,int pageSize, out int total)
        {
            total = 0;
            Regex reg = new Regex(@"^\d+$");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SubjectName", string.IsNullOrEmpty(subjectNameOrSubjectNo) ? "" : reg.IsMatch(subjectNameOrSubjectNo) ? "" : subjectNameOrSubjectNo);
            dic.Add("SubjectNo", string.IsNullOrEmpty(subjectNameOrSubjectNo) ? "" : reg.IsMatch(subjectNameOrSubjectNo) ? subjectNameOrSubjectNo : "");
            dic.Add("ProductNo", string.IsNullOrEmpty(productNo) ? "" : productNo);
            dic.Add("CreateUserId", string.IsNullOrEmpty(createUserId) ? "" : createUserId);
            dic.Add("SubjectStatus", string.IsNullOrEmpty(subjectStatus) ? "" : subjectStatus);
            dic.Add("SubjectCategoryNo", string.IsNullOrEmpty(subjectCategoryNo) ? "" : subjectCategoryNo);
            dic.Add("ChannelType", string.IsNullOrEmpty(channelType) ? "" : channelType);
            dic.Add("SubjectGender", string.IsNullOrEmpty(subjectGender) ? "" : subjectGender);
            dic.Add("SubjectDateEndStart", string.IsNullOrEmpty(subjectDateEndStart) ? "" : subjectDateEndStart);
            dic.Add("SubjectDateEndEnd", string.IsNullOrEmpty(subjectDateEndEnd) ? "" : subjectDateEndEnd);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsSubject_GetSubjectListTotalCount", dic,new 
            {
                SubjectName = subjectNameOrSubjectNo,
                SubjectNo = subjectNameOrSubjectNo,
                ProductNo = productNo,
                CreateUserId = createUserId,
                SubjectStatus = string.IsNullOrEmpty(subjectStatus) ? -1 : int.Parse(subjectStatus),
                SubjectCategoryNo = subjectCategoryNo,
                ChannelType = channelType,
                SubjectGender = string.IsNullOrEmpty(subjectGender) ? "" : subjectGender,
                SubjectDateEndStart = subjectDateEndStart,
                SubjectDateEndEnd = subjectDateEndEnd,
            }).FirstOrDefault();
            return DapperUtil.Query<SWfsNewSubject>("ComBeziWfs_SWfsSubject_GetSubjectList",dic, new 
            {
                SubjectName = subjectNameOrSubjectNo,
                SubjectNo = subjectNameOrSubjectNo,
                ProductNo = productNo,
                CreateUserId = createUserId,
                SubjectStatus = string.IsNullOrEmpty(subjectStatus) ? "" : subjectStatus,
                SubjectCategoryNo = subjectCategoryNo,
                ChannelType = channelType,
                SubjectGender = string.IsNullOrEmpty(subjectGender) ? "" : subjectGender,
                SubjectDateEndStart = subjectDateEndStart,
                SubjectDateEndEnd = subjectDateEndEnd,
                pageIndex=pageIndex,
                pageSize=pageSize
            });
        }
        //验证活动头图是否上传
        public string SubjectIsExistPic(string subjectNo)
        {
            return DapperUtil.Query<string>("ComBeziWfs_SWfsSubject_ValidateExistSubjectPic", new 
            {
                SubjectNo=subjectNo
            }).FirstOrDefault();
        }
        #endregion
    }
}