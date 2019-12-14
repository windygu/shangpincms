using Shangpin.Entity.Item.Search;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Ocs.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsBrandWallService
    {
        /// <summary>
        /// 获取品牌列表
        /// </summary>
        /// <param name="channelNo"></param>
        /// <param name="brandNoAndBrandName"></param>
        /// <returns></returns>
        public IEnumerable<SpHomeRecommendBrandExtends> GetBrandList(string pageNo, string pagePositionNo, int isRecommendBrand, string startTime, string endTime, string brandNoAndBrandName)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("PageNo", pageNo);
            dic.Add("PagePositionNo", pagePositionNo);
            dic.Add("StartDate", startTime == null ? "" : startTime);
            dic.Add("EndDate", endTime == null ? "" : endTime);
            dic.Add("BrandNoAndBrandName", brandNoAndBrandName == null ? "" : brandNoAndBrandName);
            dic.Add("RecommendBrandID", "");

            IEnumerable<SpHomeRecommendBrandExtends> brandList = DapperUtil.Query<SpHomeRecommendBrandExtends>("ComBeziWfs_SWfsSpHomeRecommendBrand_GetRecommendBrandByChannelNo", dic, new
            {
                PageNo = pageNo,
                PagePositionNo = pagePositionNo,
                IsRecommendBrand = isRecommendBrand,
                StartDate = startTime,
                EndDate = endTime,
                BrandNoAndBrandName = brandNoAndBrandName,
                RecommendBrandID = ""
            });

            return brandList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pagePositionNo"></param>
        /// <param name="isRecommendBrand"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="brandNoAndBrandName"></param>
        /// <returns></returns>
        public IEnumerable<SpHomeRecommendBrandExtends> GetBrandList(string pageNo, string pagePositionNo, int isRecommendBrand, string startTime, string endTime, string brandNoAndBrandName, string recommendBrandID)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("PageNo", pageNo);
            dic.Add("PagePositionNo", pagePositionNo);
            dic.Add("StartDate", startTime == null ? "" : startTime);
            dic.Add("EndDate", endTime == null ? "" : endTime);
            dic.Add("BrandNoAndBrandName", brandNoAndBrandName == null ? "" : brandNoAndBrandName);
            dic.Add("RecommendBrandID", recommendBrandID);

            IEnumerable<SpHomeRecommendBrandExtends> brandList = DapperUtil.Query<SpHomeRecommendBrandExtends>("ComBeziWfs_SWfsSpHomeRecommendBrand_GetRecommendBrandByChannelNo", dic, new
            {
                PageNo = pageNo,
                PagePositionNo = pagePositionNo,
                IsRecommendBrand = isRecommendBrand,
                StartDate = startTime,
                EndDate = endTime,
                BrandNoAndBrandName = brandNoAndBrandName,
                RecommendBrandID = recommendBrandID
            });

            return brandList;
        }

        public IEnumerable<SpHomeRecommendBrandExtends> GetBrandByPk(int recommendBrandID)
        {
            return DapperUtil.Query<SpHomeRecommendBrandExtends>("ComBeziWfs_SWfsSpHomeRecommendBrand_GetRecommendBrandByRId", new { RecommendBrandID = recommendBrandID });
        }

        /// <summary>
        /// 获取可用楼层
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SWfsIndexModule> GetUsableFloor()
        {
            var floors = DapperUtil.Query<SWfsIndexModule>("ComBeziWfs_SWfsIndexModule_GetUsableFloor");

            return floors;
        }

        /// <summary>
        /// 插入品牌数据
        /// </summary>
        /// <param name="recommendBrand"></param>
        /// <returns></returns>
        public int InsertSWfsSpChannelRecommendBrand(SWfsSpHomeRecommendBrand recommendBrand)
        {
            if (UpdateSWfsSpChannelRecommendBrand(recommendBrand, PresentationHelper.GetPassport().UserName))
            {
                return DapperUtil.Insert<SWfsSpHomeRecommendBrand>(recommendBrand, true);
            }
            return -1;
        }

        public int InsertSWfsSpHomeRecommendBrand(SWfsSpHomeRecommendBrand recommendBrand)
        {
            return DapperUtil.Insert<SWfsSpHomeRecommendBrand>(recommendBrand, true);
        }
        public bool UpdateSWfsSpHomeRecommendBrand(SWfsSpHomeRecommendBrand recommendBrand)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSpHomeRecommendBrand>(recommendBrand);
        }
        /// <summary>
        /// 修改positionName
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="pagePositionNo"></param>
        /// <param name="PagePositionName"></param>
        /// <param name="WebSiteNo"></param>
        /// <param name="IsRecommendBrand"></param>
        /// <returns></returns>
        public int UpdateRecommendBrandPositionName(string PageNo, string pagePositionNo, string PagePositionName, string WebSiteNo = "shangpin.com", string IsRecommendBrand = "0")
        {
            int result = 0;
            result = DapperUtil.Execute("ComBeziWfs_SWfsSpHomeRecommendBrand_UpdateHomeRecommendBrandPagePositionName", new { PagePositionName = PagePositionName, PageNo = PageNo, PagePositionNo = pagePositionNo, WebSiteNo = WebSiteNo, IsRecommendBrand = IsRecommendBrand });
            return result;
        }

        /// <summary>
        /// 如果添加品牌存在（或被逻辑删除），则更新品牌状态
        /// </summary>
        /// <param name="recommendBrand"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool UpdateSWfsSpChannelRecommendBrand(SWfsSpHomeRecommendBrand recommendBrand, string userId)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("PageNo", recommendBrand.PageNo);
            dic.Add("PagePositionNo", recommendBrand.PagePositionNo);
            dic.Add("BrandNO", recommendBrand.IsRecommendBrand == 1 ? "" : recommendBrand.BrandNO);
            dic.Add("IsRecommendBrand", recommendBrand.IsRecommendBrand);
            //查询品牌
            var originalBrand = DapperUtil.Query<SWfsSpHomeRecommendBrand>("ComBeziWfs_SWfsSpHomeRecommendBrand_GetRecommendBrand", dic, new
            {
                PageNo = recommendBrand.PageNo,
                PagePositionNo = recommendBrand.PagePositionNo,
                BrandNO = recommendBrand.BrandNO,
                IsRecommendBrand = recommendBrand.IsRecommendBrand
            }).FirstOrDefault();

            //如果存在则修改，如果不存在则返回
            if (originalBrand == null)
            {
                return true;
            }
            else
            {
                var result = DapperUtil.UpdatePartialColumns<SWfsSpHomeRecommendBrand>(new
                {
                    RecommendBrandID = originalBrand.RecommendBrandID,
                    Status = recommendBrand.Status,
                    DataState = 1,
                    UpdateOperateUserId = userId,
                    UpdateDate = DateTime.Now,
                    SortId = 0,
                    PictureFileNo = recommendBrand.PictureFileNo,
                    BrandNo = recommendBrand.BrandNO,
                    DateBegin=recommendBrand.DateBegin,
                    PictureIndex=recommendBrand.PictureIndex,
                    PictureFileTitle=recommendBrand.PictureFileTitle
                });

                return false;
            }
        }

        /// <summary>
        /// 用于频道页逻辑
        /// </summary>
        /// <param name="recommendBrand"></param>
        /// <returns></returns>
        public int InsertSWfsSpChannelRecommendBrandChannel(SWfsSpHomeRecommendBrand recommendBrand)
        {
            if (UpdateSWfsSpChannelRecommendBrandChannel(recommendBrand, PresentationHelper.GetPassport().UserName))
            {
                return DapperUtil.Insert<SWfsSpHomeRecommendBrand>(recommendBrand, true);
            }
            return -1;
        }

        /// <summary>
        /// 如果添加品牌存在（或被逻辑删除），则更新品牌状态(用于频道页)
        /// </summary>
        /// <param name="recommendBrand"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool UpdateSWfsSpChannelRecommendBrandChannel(SWfsSpHomeRecommendBrand recommendBrand, string userId)
        {
            //ComBeziWfs_SWfsSpHomeRecommendBrand_GetRecommendBrandByID

            if (recommendBrand.RecommendBrandID + "" != "")
            {
                var dic = new Dictionary<string, object>();
                dic.Add("RecommendBrandID", recommendBrand.RecommendBrandID);
                var originalBrand = DapperUtil.Query<SWfsSpHomeRecommendBrand>("ComBeziWfs_SWfsSpHomeRecommendBrand_GetRecommendBrandByID", dic, new
                {
                    RecommendBrandID = recommendBrand.RecommendBrandID,

                }).FirstOrDefault();
                //如果存在则修改，如果不存在则返回
                if (originalBrand == null)
                {
                    return true;
                }
                else
                {
                    var result = DapperUtil.UpdatePartialColumns<SWfsSpHomeRecommendBrand>(new
                    {
                        RecommendBrandID = originalBrand.RecommendBrandID,
                        Status = 1,
                        DataState = 1,
                        UpdateOperateUserId = userId,
                        UpdateDate = DateTime.Now,
                        SortId = 0,
                        PictureFileNo = recommendBrand.PictureFileNo,
                        BrandNo = recommendBrand.BrandNO,
                        CreateDate = recommendBrand.IsRecommendBrand == 1 ? recommendBrand.CreateDate : originalBrand.CreateDate
                    });

                    return false;
                }
            }
            else
            {
                var dic = new Dictionary<string, object>();
                dic.Add("PageNo", recommendBrand.PageNo);
                dic.Add("PagePositionNo", recommendBrand.PagePositionNo);
                dic.Add("BrandNO", recommendBrand.BrandNO);
                dic.Add("IsRecommendBrand", recommendBrand.IsRecommendBrand);
                //查询品牌
                var originalBrand = DapperUtil.Query<SWfsSpHomeRecommendBrand>("ComBeziWfs_SWfsSpHomeRecommendBrand_GetRecommendBrand", dic, new
                {
                    PageNo = recommendBrand.PageNo,
                    PagePositionNo = recommendBrand.PagePositionNo,
                    BrandNO = recommendBrand.BrandNO,
                    IsRecommendBrand = recommendBrand.IsRecommendBrand
                }).FirstOrDefault();

                //如果存在则修改，如果不存在则返回
                if (originalBrand == null)
                {
                    return true;
                }
                else
                {
                    var result = DapperUtil.UpdatePartialColumns<SWfsSpHomeRecommendBrand>(new
                    {
                        RecommendBrandID = originalBrand.RecommendBrandID,
                        Status = 1,
                        DataState = 1,
                        UpdateOperateUserId = userId,
                        UpdateDate = DateTime.Now,
                        SortId = 0,
                        PictureFileNo = recommendBrand.PictureFileNo,
                        BrandNo = recommendBrand.BrandNO,
                        CreateDate = recommendBrand.IsRecommendBrand == 1 ? recommendBrand.CreateDate : originalBrand.CreateDate
                    });

                    return false;
                }
            }
        }

        /// <summary>
        /// 品牌排序
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortValue"></param>
        /// <returns></returns>
        public bool EditeBrandSortValue(int id, int sortValue)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSpHomeRecommendBrand>(new
            {
                RecommendBrandID = id,
                SortId = sortValue
            });
        }


        /// <summary>
        /// 添加首页推荐品牌
        /// </summary>
        /// <param name="channelNo">A01或者A02区分男女</param>
        /// <param name="brandNo">品牌编号</param>
        /// <returns></returns>
        public int AddHomeBrand(string channelNo, string brandNo)
        {
            return DapperUtil.Insert<SWfsSpHomeRecommendBrand>(new SWfsSpHomeRecommendBrand
            {
                PageNo = channelNo,
                BrandNO = brandNo,
                SortId = 999,
                CreateDate = DateTime.Now
            }, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelNo">A01或者A02区分男女</param>
        /// <param name="brandNo">品牌编号</param>
        /// <returns></returns>
        public int IsExistHomeBrand(string pagelNo, string pagePositionNo, string brandNo)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RecommendBrandID", "0");
            return DapperUtil.Query<int>("ComBeziWfs_SWfsSpHomeRecommendBrand_IsExistsHomeBrand",dic, new
            {
                PageNo = pagelNo,
                pagePositionNo = pagePositionNo,
                brandNo = brandNo
            }).FirstOrDefault();
        }
        /// <summary>
        /// 查询是否存在重复的品牌 RecommendBrandID
        /// </summary>
        /// <param name="pagelNo"></param>
        /// <param name="pagePositionNo"></param>
        /// <param name="brandNo"></param>
        /// <returns></returns>
        public int IsExistHomeBrand(string pagelNo, string pagePositionNo, string brandNo, string recommendBrandID)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("RecommendBrandID", recommendBrandID);
            return DapperUtil.Query<int>("ComBeziWfs_SWfsSpHomeRecommendBrand_IsExistsHomeBrand",dic, new
            {
                PageNo = pagelNo,
                pagePositionNo = pagePositionNo,
                brandNo = brandNo,
                RecommendBrandID = recommendBrandID
            }).FirstOrDefault();
        }

        public SWfsSpHomeRecommendBrand GetHomeBrandByPK(int subjectNo)
        {
            return DapperUtil.QueryByIdentity<SWfsSpHomeRecommendBrand>(subjectNo);
        }
        /// <summary>
        /// 按主键删除首页推荐品牌
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public int DeleteHomeBrand(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpHomeRecommendBrand_deleteHomeBrandByID", new
            {
                RecommendBrandID = id
            });

        }
        public bool UpDateHomeBrand(SWfsSpHomeRecommendBrand brand)
        {
            return DapperUtil.UpdatePartialColumns<SWfsSpHomeRecommendBrand>(brand);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pagePositionNo"></param>
        /// <returns>BrandInfo.FirstName在频道页用于记录PagePositionName</returns>
        public IEnumerable<BrandInfo> GetHomePageRecommendBrand(string pageNo, string pagePositionNo)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PageNo", pageNo);
            dic.Add("PagePositionNo", pagePositionNo);
            var brandList = DapperUtil.Query<BrandInfo>("ComBeziWfs_SWfsSpHomeRecommendBrand_GetHomePageRecommendByPageNo", dic, new
            {
                PageNo = pageNo,
                PagePositionNo = pagePositionNo
            });

            if (brandList != null)
            {
                for (int i = 0; i < brandList.Count(); i++)
                {
                    brandList.ElementAt(i).Alias = GetAlias_ByBrandNo(brandList.ElementAt(i).BrandNo);
                }
            }
            return brandList;
        }

        public string GetAlias_ByBrandNo(string brandNo)
        {
            if (brandNo.IsNullOrEmpty() || brandNo.Equals("0"))
            {
                return brandNo;
            }
            var alias = string.Empty;

            //读取数据库
            var list = DapperUtil.Query<string>("ComBeziWfs_SWfsCategoryBrandAlias_GetAliasByObjectNo", new { typeId = 2, brandNo = brandNo });
            if (list != null)
                alias = list.FirstOrDefault();

            return string.IsNullOrEmpty(alias) ? brandNo : alias;
        }
        /// <summary>
        /// 查询推荐品牌列表
        /// </summary>
        /// <param name="pageModel"></param>
        /// <returns></returns>
        public IEnumerable<SpHomeRecommendBrandExtends> GetSWfsSpHomeRecommendBrandList(ref PaginationInfoModel pageModel)
        {
            int totalcount = 0;
            if (string.IsNullOrEmpty(pageModel.OrderBy))
            { pageModel.OrderBy = " DateBegin desc "; }
            if (string.IsNullOrEmpty(pageModel.Field))
            { pageModel.Field = " *  "; }
            if (string.IsNullOrEmpty(pageModel.SelectField))
            { pageModel.SelectField = "*"; }
            if (string.IsNullOrEmpty(pageModel.Condition))
            { pageModel.Condition = ""; }
            object param = new { tableName = " SWfsSpHomeRecommendBrand  with (NoLock) ", 
                pageSize = pageModel.PageSize, 
                curPage = pageModel.CurrentPage,
                orderBy = pageModel.OrderBy, 
                selectfield = pageModel.SelectField, 
                field = pageModel.Field, 
                condition = pageModel.Condition };
            List<SpHomeRecommendBrandExtends> list = DapperUtil.QueryPageList<SpHomeRecommendBrandExtends>("ComBeziWfs_SWfsNewSubject_PageListSearch", ref totalcount, param).ToList();
            pageModel.TotalPage = CommonHelp.GetPageCount(pageModel.PageSize, totalcount);
            pageModel.TotalCount = totalcount;
            return list;
        }

        public IEnumerable<SpHomeRecommendBrandExtends> GetSWfsSpHomeRecommendBrandList(string pageNo,string pagePositionNo,string startTime, string endTime,string brandNoAndBrandName, int status, int pictureIndex,int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("PageNo", pageNo == null ? "" : pageNo);
            dic.Add("PagePositionNo", pagePositionNo == null ? "" : pagePositionNo);
            dic.Add("BrandNoAndBrandName", brandNoAndBrandName == null ? "" : brandNoAndBrandName);
            dic.Add("Status", status);
            dic.Add("PictureIndex", pictureIndex);
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            total = DapperUtil.Query<int>("ComBeziWfs_SpHomeRecommendBrandExtends_SelectSpHomeRecommendBrandExtendsCount", dic, new { PageNo = pageNo, PagePositionNo = pagePositionNo, StartTime = startTime, EndTime = endTime, BrandNoAndBrandName = brandNoAndBrandName, Status = status, PictureIndex = pictureIndex }).FirstOrDefault();
            var commentList = DapperUtil.Query<SpHomeRecommendBrandExtends>("ComBeziWfs_SpHomeRecommendBrandExtends_SelectSpHomeRecommendBrandExtendsList", dic, new { PageNo = pageNo, PagePositionNo = pagePositionNo, StartTime = startTime, EndTime = endTime, BrandNoAndBrandName = brandNoAndBrandName, Status = status, PictureIndex = pictureIndex, pageIndex = pageIndex, pageSize = pageSize }).ToList();
            return commentList;
        }
    }
}
