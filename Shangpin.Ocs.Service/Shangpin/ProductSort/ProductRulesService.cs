using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Service.Common;
using System.Xml;
using Shangpin.Ocs.Entity.Extenstion.ProductFlat;
using Shangpin.Ocs.Service.Shangpin.ProductSort;


namespace Shangpin.Ocs.Service.ProductSort
{
    public class ProductRulesService
    {
        #region 新产品加入排序
        /// <summary>
        /// 产品加入排序池的运算方法
        /// </summary>
        /// <param name="OcsCategoryNo">OCS分类编号</param>
        /// <param name="ProductNo">产品编号</param>
        /// <returns>0为失败，1为成功</returns>
        public int AddProductToSort(string OcsCategoryNo, string OcsCategoryName, string CateGoryType, SortProduct Product)
        {
            int result = 0;
            if (IsExitProduct(OcsCategoryNo, Product.ProductNo))  //判断产品是否存在
            {
                #region
                SWfsSortOcsCategory CateDtos = IsRuleCategory(OcsCategoryNo);
                //该系列没有加入排序
                if (CateDtos == null)
                {
                    //添加ocs分类
                    SWfsSortOcsCategory SDto = new SWfsSortOcsCategory
                    {
                        IsApplyRule = false,
                        CategoryName = OcsCategoryName,
                        CategoryNo = OcsCategoryNo,
                        AutoLastFlag = 0,
                        DateUpdate = Convert.ToDateTime("1900-01-01"),
                        DateCreate = DateTime.Now,
                        CategoryType = Convert.ToInt16(CateGoryType)

                    };
                    AddOcsCategory(SDto);
                    SWfsSortProduct pDto = new SWfsSortProduct()
                    {
                        OcsCategoryNo = OcsCategoryNo,
                        ProductNo = Product.ProductNo,
                        Sort = 1,
                        RuleId = 0,
                        DateCreate = DateTime.Now

                    };
                    AddProductToSortSingle(pDto);
                    result = 1;
                }
                //该分类已经加入排序
                else
                {
                    //已经执行规则
                    if (CateDtos.IsApplyRule)
                    {
                        List<SWfsSortRule> NDto = GetRuleByCategoryNo(OcsCategoryNo);  //查找分类下的规则
                        SWfsSortRule RuleDto = GetSmallRuleFromListByProduct(NDto, Product);  //查找分类最小规则
                        SWfsSortProduct NproductDto = new SWfsSortProduct();
                        int RuleId = 0;

                        if (RuleDto == null || RuleDto.RuleId + "" == "")
                        {
                            NproductDto = GetMinProductByCategory(OcsCategoryNo);//查询未匹配上的最小产品
                            RuleId = -1;
                        }
                        else
                        {
                            NproductDto = GetRuleMinProduct(RuleDto.RuleId + "", OcsCategoryNo);//根据最小规则查找最小产品
                            RuleId = RuleDto.RuleId;

                        }

                        int SortId = NproductDto.Sort;
                        SWfsSortProduct pDto = new SWfsSortProduct()
                        {
                            OcsCategoryNo = OcsCategoryNo,
                            ProductNo = Product.ProductNo,
                            Sort = SortId,
                            RuleId = RuleId,
                            DateCreate = DateTime.Now
                        };
                        AddProductToSortSingle(pDto);
                    }
                    //未执行规则
                    else
                    {
                        SWfsSortProduct NDto = GetCategoryMaxProduct(OcsCategoryNo);
                        int sortId = NDto.Sort + 1;
                        SWfsSortProduct pDto = new SWfsSortProduct()
                        {
                            OcsCategoryNo = OcsCategoryNo,
                            ProductNo = Product.ProductNo,
                            Sort = sortId,
                            RuleId = 0,
                            DateCreate = DateTime.Now
                        };
                        AddProductToSortSingle(pDto);
                    }
                    result = 1;
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 查找产品对应的最小的规则表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="Product"></param>
        /// <returns></returns>
        public SWfsSortRule GetSmallRuleFromListByProduct(List<SWfsSortRule> list, SortProduct Product)
        {
            SWfsSortRule result = new SWfsSortRule();
            SWfsSortRule resultBese = new SWfsSortRule();
            SWfsSortRule resultChriden = new SWfsSortRule();
            foreach (SWfsSortRule item in list)
            {
                switch (item.RuleType)
                {
                    case 1://分类
                        if (Product.CategoryNo == item.RuleObjectNo)
                        {
                            if (item.ParentId == 0)
                            {
                                resultBese = item;
                            }
                            else
                            {
                                resultChriden = item;
                            }
                        }
                        break;
                    case 2://品牌
                        if (Product.BrandNo == item.RuleObjectNo)
                        {
                            if (item.ParentId == 0)
                            {
                                resultBese = item;
                            }
                            else
                            {
                                resultChriden = item;
                            }
                        }
                        break;
                    case 3://色系
                        if (Product.ProductPrimaryColorNO == item.RuleObjectNo)
                        {
                            if (item.ParentId == 0)
                            {
                                resultBese = item;
                            }
                            else
                            {
                                resultChriden = item;
                            }
                        }
                        break;
                    case 4://价格
                        if (Product.PriceNo + "" == item.RuleObjectNo)
                        {
                            if (item.ParentId == 0)
                            {
                                resultBese = item;
                            }
                            else
                            {
                                resultChriden = item;
                            }
                        }
                        break;
                    case 6://其他
                        foreach (SWfsSortRule itemOther in list)
                        {
                            if (itemOther.ParentId == item.RuleId)
                            {
                                switch (itemOther.RuleType)
                                {
                                    case 1://分类
                                        if (Product.CategoryNo == itemOther.RuleObjectNo)
                                        {
                                            resultChriden = itemOther;
                                        }
                                        break;
                                    case 2://品牌
                                        if (Product.BrandNo == itemOther.RuleObjectNo)
                                        {
                                            resultChriden = itemOther;
                                        }
                                        break;
                                    case 3://色系
                                        if (Product.ProductPrimaryColorNO == itemOther.RuleObjectNo)
                                        {
                                            resultChriden = itemOther;
                                        }
                                        break;
                                    case 4://价格
                                        if (Product.PriceNo + "" == itemOther.RuleObjectNo)
                                        {
                                            resultChriden = itemOther;
                                        }
                                        break;
                                    default:
                                        resultChriden = item;
                                        break;
                                }
                            }
                        }
                        if (resultBese.RuleId < 1 && resultChriden.RuleId < 1)
                        {
                            resultChriden = item;
                        }
                        break;
                }
            }
            if (resultChriden.RuleObjectNo != "")
            {
                result = resultChriden;
            }
            else if (resultBese.RuleObjectNo != "")
            {
                result = resultBese;
            }
            else
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// 判断产品是否存在
        /// </summary>
        /// <param name="CategoryNo"></param>
        /// <param name="ProductNo"></param>
        /// <returns></returns>
        public bool IsExitProduct(string OCSCategoryNo, string ProductNo)
        {
            bool result = true;

            SWfsSortOcsCategory DTO = DapperUtil.Query<SWfsSortOcsCategory>("ComBeziWfs_SWfsSortProduct_IsRepeatCategoryNo", new
            {
                OcsCategoryNo = OCSCategoryNo,
                ProductNo = ProductNo
            }).FirstOrDefault();
            if (DTO != null)
            {
                result = false;
            }
            return result;
        }

        #endregion
        /// <summary>
        /// 查询分类下的规则表
        /// </summary>
        /// <param name="CategoryNo"></param>
        /// <returns></returns>
        public List<SWfsSortRule> GetRuleByCategoryNo(string CategoryNo)
        {

            return DapperUtil.Query<SWfsSortRule>("ComBeziWfs_SWfsSortRule_GetSortRuleByCategoryNo", new
            {
                OcsCategoryNo = CategoryNo
            }).ToList();

        }


        /// <summary>
        /// 插入规则 并且返回主键ID
        /// </summary>
        /// <param name="RuleInfo"></param>
        /// <returns></returns>
        public string AddRuleToSort(SWfsSortRule ruleInfo)
        {
            string result = "0";
            //int i = DapperUtil.Insert<SWfsSortRule>(ruleInfo, true);
            result = DapperUtil.Query<string>("ComBeziWfs_SWfsSortRule_AddSortRule", ruleInfo).FirstOrDefault().ToString();
            return result;
        }

        /// <summary>
        /// 插入产品
        /// </summary>
        /// <param name="ProductInfo"></param>
        /// <returns></returns>
        public int AddProductToSortSingle(SWfsSortProduct productInfo)
        {
            int result = 0;
            result = DapperUtil.Insert<SWfsSortProduct>(productInfo, true);
            return result;
        }

        /// <summary>
        /// 插入Ocs分类
        /// </summary>
        /// <param name="SDto"></param>
        /// <returns></returns>
        public int AddOcsCategory(SWfsSortOcsCategory SDto)
        {
            int result = 0;
            result = DapperUtil.Insert<SWfsSortOcsCategory>(SDto, true);
            return result;
        }

        /// <summary>
        /// 更新时间分类排序
        /// </summary>
        /// <returns></returns>
        public int UpdateCateogryTime(string OcsCategoryNo, string IsLast)
        {

            int result = 0;
            result = DapperUtil.Execute("ComBeziWfs_SWfsSortOcsCategory_UpdateSortOcsCategoryUpdateTimeByCategoryNo", new
            {
                DateUpdate = DateTime.Now.ToShortDateString(),
                AutoLastFlag = IsLast,
                IsApplyRule = 1,
                CategoryNO = OcsCategoryNo

            });

            return result;
        }

        /// <summary>
        /// 删除COS分类下的所有产品
        /// </summary>
        /// <param name="OCSCategoryNo"></param>
        /// <returns></returns>
        public int DelProductByCategoryNo(string OCSCategoryNo)
        {
            int result = 0;
            result = DapperUtil.Execute("ComBeziWfs_SWfsSortProduct_DeleteSWfsSortProductByCategoryNo", new
            {
                OcsCategoryNO = OCSCategoryNo
            });
            return result;
        }

        /// <summary>
        /// 删除COS分类下的所有规则
        /// </summary>
        /// <param name="OCSCategoryNo"></param>
        /// <returns></returns>
        public int DelSortRuleByCategoryNo(string OCSCategoryNo)
        {
            int result = 0;
            result = DapperUtil.Execute("ComBeziWfs_SWfsSortRule_DeleteSortRuleByCategoryNo", new
            {
                OcsCategoryNO = OCSCategoryNo
            });
            return result;
        }

        /// <summary>
        /// 查询分类是否执行规则
        /// </summary>
        /// <param name="OCSCategoryNo"></param>
        /// <returns></returns>
        public SWfsSortOcsCategory IsRuleCategory(string OCSCategoryNo)
        {
            return DapperUtil.Query<SWfsSortOcsCategory>("ComBeziWfs_SWfsSortOcsCategory_GetOcsCategoryInfoByCategoryNo", new
            {
                CategoryNO = OCSCategoryNo
            }).FirstOrDefault();
        }

        /// <summary>
        /// 查询所有品牌的执行规则
        /// </summary>
        /// <param name="OCSCategoryNo"></param>
        /// <returns></returns>
        public IList<SWfsSortOcsCategory> IsRuleCategoryAll()
        {
            return DapperUtil.Query<SWfsSortOcsCategory>("ComBeziWfs_SWfsSortOcsCategory_GetOcsCategoryInfoByAll").ToList();
        }

        /// <summary>
        /// 查询规则下最小排序产品
        /// </summary>
        /// <param name="RuleId"></param>
        /// <returns></returns>
        public SWfsSortProduct GetRuleMinProduct(string RuleId, string CategoryNo)
        {
            return DapperUtil.Query<SWfsSortProduct>("ComBeziWfs_SWfsSortProduct_GetProductMinSortByCategoryNoAndRuleId", new
            {
                RuleId = RuleId,
                OcsCategoryNo = CategoryNo
            }).FirstOrDefault();

        }

        /// <summary>
        /// 查询执行排序但未匹配上规则中排序最小的产品
        /// </summary>
        /// <param name="CategoryNo"></param>
        /// <returns></returns>
        public SWfsSortProduct GetMinProductByCategory(string CategoryNo)
        {
            return DapperUtil.Query<SWfsSortProduct>("ComBeziWfs_SWfsSortProduct_GetProductMinSortByCategoryNo", new
            {
                OcsCategoryNo = CategoryNo
            }).FirstOrDefault();

        }

        /// <summary>
        /// 查询分类下最大的排序值  
        /// </summary>
        /// <param name="CategoryNo"></param>
        /// <returns></returns>
        public SWfsSortProduct GetCategoryMaxProduct(string CategoryNo)
        {
            return DapperUtil.Query<SWfsSortProduct>("ComBeziWfs_SWfsSortProduct_GetProductMaxSortByCategoryNo", new
            {
                OcsCategoryNo = CategoryNo
            }).FirstOrDefault();
        }

        /// <summary>
        /// 插入副本数据  已弃用
        /// </summary>
        /// <returns></returns>
        //public int UpdateCategoryAutolastFlagByNo(string CategoryNo, string AutoFlag)
        //{
        //    int result = 0;
        //    result = DapperUtil.Execute("ComBeziWfs_SWfsSortOcsCategory_UpdateSortOcsCategoryLastFlagByCategoryNo", new
        //    {
        //        AutolastFlag = AutoFlag,
        //        CategoryNO = CategoryNo
        //    });
        //    return result;
        //}

    }
}
