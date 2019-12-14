using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Entity.Extenstion.Login;
using Shangpin.Ocs.Service.Common;

namespace Shangpin.Ocs.Service.Shangpin
{

    public class SWfsProductRefService
    {
        
        #region 模板管理
        //按ID查询模板
        public SWfsProductTemplate GetTemplateObj(int id)
        {
            return DapperUtil.Query<SWfsProductTemplate>("ComBeziWfs_SWfsProductTemplate_GetProductTemplateById", new 
            {
                TemplateID=id
            }).FirstOrDefault();
        }
        //按模板编号查询模板
        public SWfsProductTemplate GetSProductTemplateObjByNo(string tempNO)
        {
            return DapperUtil.Query<SWfsProductTemplate>("ComBeziWfs_SWfsProductTemplate_GetProductTemplateByTempNO", new
            {
                TemplateNO = tempNO
            }).FirstOrDefault();
        }
        //按模板编号获取模板代码
        public string GetTemplateCodeByTempNO(string tempNO)
        {
            return DapperUtil.Query<string>("ComBeziWfs_SWfsProductTemplate_GetProductTemplateCodeByTempNO", new
            {
                TemplateNO = tempNO
            }).FirstOrDefault();
        }
        //验证是否存在重复模板编号
        private int IsExistsTemplateNO(string tempNO)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsProductTemplate_IsExistProductTemplateByTempNO", new
            {
                TemplateNO = tempNO
            }).FirstOrDefault();
        }
        //验证模板名称是否重复
        private IEnumerable<int> IsExistsTemplateName(string tName)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsProductTemplate_IsExistProductTemplateByTempName", new
            {
                TemplateName = tName
            });
        }
        //编辑模板模板
        public int EditeProductTemplate(SWfsProductTemplate obj)
        {
            
            if (obj.TemplateCode == null)
            {
                obj.TemplateCode = "";
            }
            if (obj.TemplateCodeMobile == null)
            {
                obj.TemplateCodeMobile = "";
            }
            if (string.IsNullOrEmpty(obj.TemplateNO))
            {
                return 0;
            }
            if (obj.TemplateID == 0)
            {
                if (IsExistsTemplateNO(obj.TemplateNO) > 0)
                {
                    return -1;
                }
                if (IsExistsTemplateName(obj.TemplateName).Count()>0)
                {
                    return -2;
                }
                obj.CreateDate = DateTime.Now;
                return DapperUtil.Insert<SWfsProductTemplate>(obj, true);
            }
            else
            {
                IEnumerable<int> isExists=IsExistsTemplateName(obj.TemplateName);
                if (isExists.Count(p=>p!=obj.TemplateID) > 0)
                {
                    return -2;
                }
                return DapperUtil.UpdatePartialColumns<SWfsProductTemplate>(new 
                {
                    TemplateID=obj.TemplateID,
                    TemplateNO=obj.TemplateNO,
                    TemplateName=obj.TemplateName,
                    TemplateDirection=obj.TemplateDirection,
                    TemplateCode=obj.TemplateCode,
                    TemplateCodeMobile=obj.TemplateCodeMobile
                }) ? 1 : 0;
            }
        }
        //查询模板列表
        public IEnumerable<SWfsProductTemplate> GetTemplateList(int pageIndex, int pageSize, string tempNO, string tempName, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("TemplateNO", string.IsNullOrEmpty(tempNO) ? "" : tempNO);
            dic.Add("TemplateName", string.IsNullOrEmpty(tempName) ? "" : tempName);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsProductTemplate_GetProductTemplateTotalCount", dic, new { TemplateNO = tempNO, TemplateName = tempName }).FirstOrDefault();
            return DapperUtil.Query<SWfsProductTemplate>("ComBeziWfs_SWfsProductTemplate_GetProductRecommendTemplateList", dic, new
            {
                TemplateNO = tempNO,
                TemplateName = tempName,
                pageIndex = pageIndex,
                pageSize = pageSize
            });
        }
        //删除模板
        public int DeleteTemplate(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsProductTemplate_DeleteProductTemplateByID", new { TemplateID = id });
        }
        #endregion

        #region 产品编辑块管理
        //按产品编号查询商品编辑块内容
        public SWfsProductRef GetProductRefByProductNO(string pNO)
        {
            return DapperUtil.Query<SWfsProductRef>("ComBeziWfs_SWfsProductRef_GetSWfsProductRefHtmlCodeByProductNo", new
            {
                ProductNO = pNO
            }).FirstOrDefault();
        }
        //添加商品编辑块内容
        public int AddProductRefContent(SWfsProductRef obj)
        {
            if (obj.ProductNO == null)
            {
                return 0;
            }
            obj.CreateDate = DateTime.Now;
            obj.EditeDate = DateTime.Now;
            Passport passport = PresentationHelper.GetPassport();
            obj.PublishTime = DateTime.Now;
            obj.EditePeople = (passport != null ? passport.UserName : "");
            obj.PublishPeople = "";
            obj.ProductTitle = "";
            obj.TemplateNO = "";
            obj.PublishHTML = "";
            if (obj.HTMLCode == null)
            {
                obj.HTMLCode = "";
            }
            if (obj.HTMLCodeMobile == null)
            {
                obj.HTMLCodeMobile = "";
            }
            return DapperUtil.Insert<SWfsProductRef>(obj, true);
        }
        //编辑商品推荐内容
        public int EditeProductRefContent(SWfsProductRef obj)
        {
            if (obj.TemplateNO == null)
            {
                obj.TemplateNO = "";
            }
            Passport passport = PresentationHelper.GetPassport();
            return DapperUtil.UpdatePartialColumns<SWfsProductRef>(new
            {
                RefID = obj.RefID,
                EditeDate = DateTime.Now,
                EditePeople=(passport==null?"":passport.UserName),
                HTMLCode = obj.HTMLCode,
                HTMLCodeMobile = obj.HTMLCodeMobile,
            }) ? 1 : 0;
        }
        //修改商品所选择的模板编号
        public int EditeProductTemplateNO(int id, string tempNO)
        {
            string templateCode = GetTemplateCodeByTempNO(tempNO);//按模板编号获取模板代码
            return DapperUtil.UpdatePartialColumns<SWfsProductRef>(new
            {
                RefID = id,
                TemplateNO = tempNO == null ? "" : tempNO,
                HTMLCode = templateCode == null ? "" : templateCode
            }) ? 1 : 0;
        }
        //发布产品编辑内容
        public int PublishProductContent(int publishID)
        {
            if (publishID == 0)
            {
                return 0;
            }
            Passport passport = PresentationHelper.GetPassport();
            return DapperUtil.Execute("ComBeziWfs_SWfsProductRef_PublishProductRefHtmlCodeByRefID", new
            {
                RefID = publishID,
                PublishTime=DateTime.Now,
                PublishPeople = (passport!=null?passport.UserName:"")
            });
        }
        #endregion

        #region 产品列表
        //按组ID获取组内产品列表
        public IEnumerable<SkillProductExtends> GetSWfsProductList(string editePeople, string publishPeople, string keyWord, string productNo, string categoryNo,
            string brandNO, string isnewShelf, string timeType, string startTime, string endTime, string priceStart, string priceEnd, string rateStart, string rateEnd,
            string pNoList, string isEdite, string isPublish, int pageIndex, int pageSize,string isout, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("EditePeople", editePeople == null ? "" : editePeople);
            dic.Add("PublishPeople", publishPeople == null ? "" : publishPeople);
            dic.Add("KeyWord", keyWord == null ? "" : keyWord);
            dic.Add("ProductNo", productNo == null ? "" : productNo);
            dic.Add("CategoryNo", categoryNo == null ? "" : categoryNo);
            dic.Add("BrandNO", brandNO == null ? "" : brandNO);
            dic.Add("IsnewShelf", isnewShelf);
            dic.Add("TimeType", timeType);

            dic.Add("PriceStart", priceStart == null ? "" : priceStart);
            dic.Add("PriceEnd", priceEnd == null ? "" : priceEnd);
            dic.Add("RateStart", rateStart == null ? "" : rateStart);
            dic.Add("RateEnd", rateEnd == null ? "" : rateEnd);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);

            dic.Add("IsEdite", isEdite == "-1" ? "" : isEdite);
            dic.Add("IsPublish", isPublish == "-1" ? "" : isPublish);
            dic.Add("IsOutSide", (isout=="0"||isout==null)?"":isout);
            string[] productNoList=new string[0];
            if (string.IsNullOrEmpty(pNoList))
            {
                dic.Add("ProductNoList", "");
            }
            else
            {
                productNoList = pNoList.Split(new string[]{ "\r\n"},StringSplitOptions.RemoveEmptyEntries);
                dic.Add("ProductNoList", "1");
            }

            if (productNoList.Length > 0)
            {
                total = 1;
            }
            else 
            {
                total = DapperUtil.Query<int>("ComBeziWfs_SWfsProduct_SelectSWfsProductCountNew", dic, new
                {
                    EditePeople = editePeople,
                    PublishPeople = publishPeople,
                    ProductNo = productNo,
                    KeyWord = keyWord,
                    CategoryNo = categoryNo,
                    BrandNO = brandNO,
                    NewShelfStart = DateTime.Now.AddDays(-7),
                    NewShelfEnd = DateTime.Now,
                    StartTime = startTime,
                    EndTime = string.IsNullOrEmpty(endTime) ? DateTime.Now.ToString() : endTime,
                    PriceStart = priceStart,
                    PriceEnd = priceEnd,
                    RateStart = rateStart,
                    RateEnd = rateEnd,
                    ProductNoList = productNoList,
                    IsPublish = isPublish,
                    IsEdite = isEdite,
                    IsOutSide = string.IsNullOrEmpty(isout) ? -1 : int.Parse(isout)
                }).FirstOrDefault();
            }

            return DapperUtil.Query<SkillProductExtends>("ComBeziWfs_SWfsProduct_SelectSWfsProductListNew", dic, new
            {
                EditePeople = editePeople,
                PublishPeople = publishPeople,
                ProductNo = productNo,
                KeyWord = keyWord,
                CategoryNo = categoryNo,
                BrandNO = brandNO,
                NewShelfStart = DateTime.Now.AddDays(-7),
                NewShelfEnd = DateTime.Now,
                StartTime = startTime,
                EndTime = string.IsNullOrEmpty(endTime) ? DateTime.Now.ToString() : endTime,
                PriceStart = priceStart,
                PriceEnd = priceEnd,
                RateStart = rateStart,
                RateEnd = rateEnd,
                ProductNoList = productNoList,
                IsPublish = isPublish,
                IsEdite = isEdite,
                pageIndex = pageIndex,
                pageSize = pageSize,
                IsOutSide = string.IsNullOrEmpty(isout)?-1:int.Parse(isout) 
            });
        }
        #endregion

        #region 商品模板组合编辑
        //获取商品的title
        public string GetProductTitle(string productNo) 
        {
            return DapperUtil.Query<string>("ComBeziWfs_SWfsProductRef_GetProductTitle", new 
            {
                ProductNO = productNo
            }).FirstOrDefault();
        }
        //按ID获取结构
        public SWfsProductRefTemplate GetStructById(int structId)
        {
            return DapperUtil.Query<SWfsProductRefTemplate>("ComBeziWfs_SWfsProductRef_ParentStructById", new
            {
                ProductRefTemplateID=structId
            }).FirstOrDefault();
        }
        //获取全部父类结构列表
        public IEnumerable<SWfsProductRefTemplate> GetStructList() 
        {
            return DapperUtil.Query<SWfsProductRefTemplate>("ComBeziWfs_SWfsProductRef_ParentStruct");
        }
        //获取正在使用的父类结构
        public SWfsProductRefTemplate GetUseParentStructList(string productNo)
        {
            SWfsProductRefTemplate obj= DapperUtil.Query<SWfsProductRefTemplate>("ComBeziWfs_SWfsProductRef_UseParentStruct", new 
            {
                ProductNo=productNo
            }).FirstOrDefault();
            if (obj==null)
            {
                obj = new SWfsProductRefTemplate();
            }
            return obj;
        }
        //获取商品编辑当前正在使用的子类结构列表
        public IEnumerable<SWfsProductRefTemplate> GetStructChild(int parentId)
        {
            if (parentId!=0)
            {
                IEnumerable<SWfsProductRefTemplate> list = DapperUtil.Query<SWfsProductRefTemplate>("ComBeziWfs_SWfsProductRef_SaveChildStruct", new
                {
                    ProductRefTemplateID = parentId
                });
                return list;
            }
            else
            {
                return new List<SWfsProductRefTemplate>();
            }
        }
        //获取模板列表
        public IEnumerable<SWfsProductTemplate> GetTemplateList() 
        {
            return DapperUtil.Query<SWfsProductTemplate>("ComBeziWfs_SWfsProductTemplate_GetTemplatePreviewList");
        }
        //获取模板的内容
        public IEnumerable<SWfsProductTemplate> GetTemplateContentByTemplateNoList(string templateNoList)
        {
            if (string.IsNullOrEmpty(templateNoList))
            {
                return new List<SWfsProductTemplate>();
            }
            return DapperUtil.Query<SWfsProductTemplate>("ComBeziWfs_SWfsProductTemplate_GetTemplateHtmlByTemplateNoList", new
            {
                TemplateNOList = templateNoList.Split(',').Distinct().ToArray()
            });
        }

        
        //保存商品的title
        public int SaveProductTitle(string productNo,string productTitle)
        {
            //判断是否存在编辑过的商品
            SWfsProductRef productObj = GetProductRefByProductNO(productNo);
            if (productObj==null)
            {
                productObj.ProductNO = productNo;
                productObj.TemplateNO = "";
                productObj.CreateDate = DateTime.Parse("1900-01-01 00:00:00");
                productObj.EditeDate = DateTime.Parse("1900-01-01 00:00:00");
                productObj.HTMLCode = "";
                productObj.PublishHTML = "";
                productObj.ProductTitle = productTitle;
                productObj.PublishTime = DateTime.Parse("1900-01-01 00:00:00");
                productObj.PublishPeople = "";
                productObj.EditePeople = "";
                productObj.HTMLCodeMobile = "";
                productObj.PublishHTMLMobile = "";
                return DapperUtil.Insert<SWfsProductRef>(productObj);
            }
            else
            {
                return DapperUtil.UpdatePartialColumns<SWfsProductRef>(new 
                {
                    RefID=productObj.RefID,
                    ProductTitle=productTitle
                })?1:0;
            }
            return DapperUtil.Execute("ComBeziWfs_SWfsProductRef_SaveProductTitle", new 
            {
                ProductTitle=productTitle,
                ProductNO=productNo
            });
        }
        //保存结构名称
        public int SaveParentStructName(string structName,string productNo)
        {
            return DapperUtil.Execute("SWfsProductRefTemplate_SaveParentStructName", new 
            {
                ModuleName=structName,
                ProductNo=productNo
            });
        }

        //验证结构是否存在按名称查询
        public int IsExistsStructName(string structName)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsProductRefTemplate_IsExistsStructName", new
            {
                ModuleName=structName
            }).FirstOrDefault();
        }
        //验证结构是否存在按照结构查询
        //public int IsExistsStruc(string[] templateNoGroup, string productNo)
        //{
        //    int result = 0;
        //    for (int i = 0; i < templateNoGroup.Length; i++)
        //    {
        //        result = DapperUtil.Query<int>("ComBeziWfs_SWfsProductRefTemplate_IsExistsStruct", new
        //        {
        //            TemplateNo = templateNoGroup[i],
        //            ProductNo = productNo,
        //            ModuleStatus=1
        //        }).FirstOrDefault();
        //        if (result==0)
        //        {
        //            break;
        //        }
        //    }
        //    return result;
        //}


        //保存结构的父类返回自增列ID
        public int SaveStructParent(string structName, string productNo, int stuctStatus)
        {
            //增加父类
            string identityID= DapperUtil.Query<string>("ComBeziWfs_SWfsProductRefTemplate_SaveStructParentReturnID", new 
            {
                ModuleName = structName,
                ProductNo=productNo,
                ModuleStatus = stuctStatus
            }).FirstOrDefault();
            return int.Parse(identityID);
        }
        //保存结构的子类
        public int SaveStructChild(IEnumerable<SWfsProductRefTemplate>region,int parentId)
        {
            if (parentId==0)
            {
                return 0;
            }
            for (int i = 0; i < region.Count(); i++)
            {
                region.ElementAt(i).ParentId = parentId;
                if (region.ElementAt(i).TemplateHtmlCode == null || region.ElementAt(i).ModuleStatus == 0)
                {
                    region.ElementAt(i).TemplateHtmlCode = "";
                }
                if (region.ElementAt(i).TemplateHtmlCodeMobile == null || region.ElementAt(i).ModuleStatus == 0)
                {
                    region.ElementAt(i).TemplateHtmlCodeMobile = "";
                }
                if (region.ElementAt(i).ProductNo==null)
                {
                    region.ElementAt(i).ProductNo = "";
                }
                DapperUtil.Insert<SWfsProductRefTemplate>(region.ElementAt(i), true);
            }
            return 1;
        }

        //修改结构的父类
        public int EditeStructParent(int parentStructId,string structName,string productNo)
        {
            return DapperUtil.UpdatePartialColumns<SWfsProductRefTemplate>(new 
            {
                ProductRefTemplateID = parentStructId,
                ProductNo=productNo,
                ModuleName = structName,
            })?1:0;
        }
        //修改结构的子类
        public int EditeStructChild(IEnumerable<SWfsProductRefTemplate> region) 
        {
            for (int i = 0; i < region.Count(); i++)
            {
                if (region.ElementAt(i).ModuleStatus==0)
                {
                    region.ElementAt(i).TemplateHtmlCode = "";
                    region.ElementAt(i).TemplateHtmlCodeMobile = "";
                }
                DapperUtil.UpdatePartialColumns<SWfsProductRefTemplate>(new 
                {
                    ProductRefTemplateID=region.ElementAt(i).ProductRefTemplateID,
                    ModuleName = region.ElementAt(i).ModuleName,
                    ModuleStatus = region.ElementAt(i).ModuleStatus,
                    TemplateNo = region.ElementAt(i).TemplateNo,
                    ProductNo = region.ElementAt(i).ProductNo,
                    Sort = region.ElementAt(i).Sort,
                    TemplateHtmlCode=region.ElementAt(i).TemplateHtmlCode.Trim(),
                    TemplateHtmlCodeMobile =region.ElementAt(i).TemplateHtmlCodeMobile.Trim()
                });
            }
            return 1;
        }

        
        //使用结构
        public int UseStruct(string productNo,int struckId)
        {
            //使用结构
            return DapperUtil.Execute("ComBeziWfs_SWfsProductRefTemplate_UseStructById", new 
            {
                ProductNo=productNo,
                ProductRefTemplateID=struckId
            });
        }

        //使用结构的情况下保存编辑的内容
        public int SaveContentByStruct(int id,string htmlCode) 
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsProductRefTemplate_SaveContentByStruct", new 
            {
                ProductRefTemplateID=id,
                TemplateHtmlCode=htmlCode
            });
        }
        //保存最终html
        public int SaveAllHtml(int stuctid,int id)
        {
            //获取每个块的 列表信息 
            IList<SWfsProductRefTemplate> list= DapperUtil.Query<SWfsProductRefTemplate>("ComBeziWfs_SWfsProductRefTemplate_SaveRegionContentList", new 
            {
                ParentId = stuctid  
            }).OrderBy(p=>p.Sort).ToList();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i].TemplateHtmlCode);
            }
            return DapperUtil.Execute("ComBeziWfs_SWfsProductRef_SaveAllHtml", new 
            {
                RefID=id,
                HTMLCode=sb.ToString()
            });
        }
        //获取商品信息
        public Product GetProductInfoByProductNo(string productNo) 
        {
            Product obj = DapperUtil.Query<Product>("ComBeziWfs_WfsProduct_GetProductInfoByProductNo", new 
            {
                ProductNo=productNo
            }).FirstOrDefault();
            if (obj!=null&&!string.IsNullOrEmpty(obj.CategoryNo))
            {
                string categoryNoList =string.Empty;
                //查询所在分类
                switch (obj.CategoryNo.Length)
                {
                    case 3:
                        categoryNoList = obj.CategoryNo;
                        break;
                    case 6:
                        categoryNoList = obj.CategoryNo.Substring(0,3)+","+obj.CategoryNo;
                        break;
                    case 9:
                        categoryNoList = obj.CategoryNo.Substring(0, 3) + "," + obj.CategoryNo.Substring(0,6)+","+obj.CategoryNo;
                        break;
                    case 12:
                        categoryNoList = obj.CategoryNo.Substring(0, 3) + "," + obj.CategoryNo.Substring(0, 6) + "," + obj.CategoryNo.Substring(0,9)+","+obj.CategoryNo;
                        break;
                }
                IEnumerable<ErpCategory> categoryList = DapperUtil.Query<ErpCategory>("ComBeziWfs_SWfsCategory_GetProductCategoryAllByCategoryNoList", new 
                {
                    CategoryNoList = categoryNoList.Split(',')
                });
                switch (categoryList.Count())
                {
                    case 1:
                        obj.CategoryNo = categoryList.ElementAt(0).CategoryName;
                        break;
                    case 2:
                        obj.CategoryNo = categoryList.Single(p => p.CategoryNo.Length == 3).CategoryName + ">" + categoryList.Single(p => p.CategoryNo.Length == 6).CategoryName;
                        break;
                    case 3:
                        obj.CategoryNo = categoryList.Single(p => p.CategoryNo.Length == 3).CategoryName + ">" + categoryList.Single(p => p.CategoryNo.Length == 6).CategoryName + ">" + categoryList.Single(p => p.CategoryNo.Length == 9).CategoryName;
                        break;
                    case 4:
                        obj.CategoryNo = categoryList.Single(p => p.CategoryNo.Length == 3).CategoryName + ">" + categoryList.Single(p => p.CategoryNo.Length == 6).CategoryName + ">" + categoryList.Single(p => p.CategoryNo.Length == 9).CategoryName + ">" + categoryList.Single(p => p.CategoryNo.Length == 12).CategoryName;
                        break;
                }
            }
            return obj;
        }
        
        #endregion

        #region 查询历史记录
        //获取历史记录
        public IEnumerable<SWfsProductSearchHistory> GetSearchHistory(int count)
        {
            IEnumerable<SWfsProductSearchHistory> list = new List<SWfsProductSearchHistory>();
            Passport passport = PresentationHelper.GetPassport();
            if (passport != null)
            {
                if (!string.IsNullOrEmpty(passport.UserName))
                {
                    //查询历史记录
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("getCount", count);
                    list = DapperUtil.Query<SWfsProductSearchHistory>("ComBeziWfs_SWfsProductSearchHistory_GetProductSearchHistory", dic, new { SearchUser = passport.UserName });
                }
            }
            return list;
        }
        //添加历史记录
        public int AddSearchHistory(SWfsProductSearchHistory obj)
        {
            Passport passport = PresentationHelper.GetPassport();
            if (passport != null && !string.IsNullOrEmpty(passport.UserName))
            {
                obj.CreateDate = DateTime.Now;
                obj.SearchUser = passport.UserName;
                obj.LogType = 1;
                return DapperUtil.Insert<SWfsProductSearchHistory>(obj, true);
            }
            return 0;
        }
        //删除历史记录
        public int ClearLabelHistory(string userId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsProductSearchHistory_DeleteProductHistory", new
            {
                SearchUser = userId
            });
        }
        #endregion

        #region 预览页读取数据
        //图片
        public IEnumerable<string> GetProductPicList(string productNo)
        {
            return DapperUtil.Query<string>("ComBeziWfs_WfsPrductAttr_GetProductPicList", new { ProductNo = productNo });
        }
        //详情Html
        public string GetProductDetailHtml(string productNo,int type)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("type",type);
            string result= DapperUtil.Query<string>("ComBeziWfs_SWfsProductRef_GetProductPreviewHtml", dic, new 
            {
                ProductNo=productNo
            }).FirstOrDefault();
            return result;
        }
        //商品信息
        public Product GetProductDetailByProductNo(string productNo)
        {
            return DapperUtil.Query<Product>("ComBeziWfs_SWfsProduct_GetSpProductById", new { ProductNo = productNo }).FirstOrDefault();
            //return DapperUtil.QueryByIdentity<Product>(productNo);
        }
        #endregion

    }
}
