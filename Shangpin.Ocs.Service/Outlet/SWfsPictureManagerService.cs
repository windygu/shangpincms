using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
namespace Shangpin.Ocs.Service.Outlet
{
    public class SWfsPictureManagerService
    {

        public SWfsPictureManager GetModel(string pictureManageId)
        {
            return DapperUtil.Query<SWfsPictureManager>("ComBeziWfs_SWfsPictureManager_GetSWfsPictureManagerModel", new { PictureManageId = pictureManageId }).FirstOrDefault();
        }

        public void Add(SWfsPictureManager model)
        {
            DapperUtil.Insert<SWfsPictureManager>(model);
        }
        public int Add(SWfsPictureManager model, bool bl)
        {
            return DapperUtil.Insert<SWfsPictureManager>(model, bl);
        }
        public bool Update(SWfsPictureManager model)
        {
            return DapperUtil.Update<SWfsPictureManager>(model);
        }

        public IList<SWfsPictureManager> GetList(string name, string position, string begindate, string enddate, string gender, int siteNo, int pagePosition)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PictureName", string.IsNullOrEmpty(name) ? "" : name);
            dic.Add("Position", string.IsNullOrEmpty(position) ? "" : position);
            dic.Add("BCreate", string.IsNullOrEmpty(begindate) ? "" : begindate);
            dic.Add("ECreate", string.IsNullOrEmpty(enddate) ? "" : enddate);
            dic.Add("Gender", string.IsNullOrEmpty(gender) ? "" : gender);
            return DapperUtil.Query<SWfsPictureManager>("ComBeziWfs_SWfsPictureManager_GetSWfsPictureManagerList", dic, new { PictureName = name, Position = position, DateBegin = begindate, DateEnd = enddate, Gender = gender, WebSite = siteNo, PagePosition = pagePosition }).ToList();
        }

        public int Del(string picManId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsPictureManager_DelSWfsPictureManagerById", new { PictureManageId = picManId });
        }

        /// <summary>
        /// 尚品首页获取位置图片方法轮播图
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <returns></returns>
        public SWfsPictureManager GetSWfsPictureManagerSingle(string pagePosition, int gender)
        {
            return DapperUtil.Query<SWfsPictureManager>("ComBeziWfs_SWfsPictureManager_GetSWfsPictureManagerSingle", new { Gender = gender, PagePosition = pagePosition }).FirstOrDefault();
        }

        #region 底部位置图广告位置

        /// <summary>
        /// 尚品底部位置图广告位置 图片列表"67,68,69"
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <returns></returns>
        public IList<SWfsPictureManager> GetSWfsPictureManagerListSp(int gender)
        {
            return DapperUtil.Query<SWfsPictureManager>("ComBeziWfs_SWfsPictureManager_GetSWfsPictureManagerListSP", new { Gender = gender }).ToList();
        }
        #endregion


        #region 20140313第四次首页改版，首页轮播图位置

        /// <summary>
        /// 尚品首页获取位置图片列表"3张轮播图设置：第一帧：81第二帧：82第三帧：83 模板宽频：80
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <returns></returns>
        public IList<SWfsPictureManager> GetSWfsPictureManagerListTopSwitchV(ref PaginationInfoModel pageModel)
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
            object param = new { tableName = " SWfsPictureManager  with (NoLock) ", pageSize = pageModel.PageSize, curPage = pageModel.CurrentPage, orderBy = pageModel.OrderBy, selectfield = pageModel.SelectField, field = pageModel.Field, condition = pageModel.Condition };
            List<SWfsPictureManager> list = DapperUtil.QueryPageList<SWfsPictureManager>("ComBeziWfs_SWfsNewSubject_PageListSearch", ref totalcount, param).ToList();
            pageModel.TotalPage = CommonHelp.GetPageCount(pageModel.PageSize, totalcount);
            pageModel.TotalCount = totalcount;
            return list;
        }
        #endregion



        #region 20140317第四版首页功能
        // 尚品首页获取位置图片列表3张轮播图设置：
        //第一帧：81
        //第二帧：82
        //第三帧：83
        //模板宽频：80

        //大运营位置图片：
        //新品：84
        //促销：85
        //搭配：86
        //品类：87
        //杂志：88

        //小运营位置：
        //奥莱：89
        //服装：90
        //箱包：91
        //鞋靴：92
        //扩展品类：93 

        /// <summary>
        ///  该区域后台可二选一显示，轮播图可同时显示三张，若为大图，则只显示一张；
        /// </summary>
        /// <param name="count"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public IList<SWfsPictureManager> GetWomenBannerNew(int gender)
        {
            return DapperUtil.Query<SWfsPictureManager>("ComBeziWfs_SWfsPictureManager_GetSWfsPictureManagerByGenderNew",
                                                         new { gender = gender }).ToList();

        }
        /// <summary>
        /// 大运营位置图片
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public IList<SWfsPictureManager> GetBigPagePositionNew(int gender)
        {
            return DapperUtil.Query<SWfsPictureManager>("ComBeziWfs_SWfsPictureManager_GetSWfsPictureManagerByGenderBigPicNew",
                                                         new { gender = gender }).ToList();

        }
        /// <summary>
        /// 小运营位置图片
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public IList<SWfsPictureManager> GetSmallPagePositionNew(int gender)
        {
            return DapperUtil.Query<SWfsPictureManager>("ComBeziWfs_SWfsPictureManager_GetSWfsPictureManagerByGenderSmallPicNew",
                                                         new { gender = gender }).ToList();

        }

        /// <summary>
        /// 底部运营位置图片
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public IList<SWfsPictureManager> GetButtonPagePositionNew(int gender)
        {
            var q = DapperUtil.Query<SWfsPictureManager>("ComBeziWfs_SWfsPictureManager_GetSWfsPictureManagerByGenderBottonNew",
                                                         new { gender = gender }).ToList();

            return q;
        }
        /// <summary>
        /// 秒杀位置图
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public SWfsHomeSecKill GetSWfsHomeSecKillSingle(int gender)
        {
            string channelNo = gender == 1 ? "A02" : "A01";
            return DapperUtil.Query<SWfsHomeSecKill>("ComBeziWfs_SWfsHomeSecKill_GetSWfsHomeSecKillByChannelNO",
                                                         new { ChannelNo = channelNo }).FirstOrDefault();
        }

        #region 秒杀位置图读取单品数据
        /// <summary>
        /// 秒杀位置图读取单品数据
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public ProductInfo GetSWfsHomeSecKillByProductNoSingle(string productNo)
        {
            return DapperUtil.Query<ProductInfo>("ComBeziWfs_SWfsHomeSecKill_GetSWfsHomeSecKillByProductNo",
                                                         new { ProductNo = productNo }).FirstOrDefault();
        }
        #endregion

        /// <summary>
        /// 首页运营位复制
        /// </summary>
        /// <param name="id">复制数据的主键ID</param>
        /// <param name="positionID">运营位ID</param>
        /// <returns></returns>
        public int CopySmallPagePosition(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsPictureManager_CopySmallPagePosition", new
            {
                PictureManageId = id,
            });
        }
        #endregion

        #region 首页运营位置图注解
        /// <summary>
        /// 首页运营位置图注解
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static string GetSWfsPictureManagerContentPosition(string positionNo, int pictureIndex)
        {
            string positionValue = string.Empty;
            switch (positionNo)
            {
                case "":
                    positionValue = "底部运营位1";
                    break;
                //case 68:
                //    positionValue = "底部运营位2";
                //    break;
                //case 69:
                //    positionValue = "底部运营位3";
                //    break;
                //case 80:
                //    positionValue = "模板宽频";
                //    break;
                //case 81:
                //    positionValue = "第一帧";
                //    break;
                //case 82:
                //    positionValue = "第二帧";
                //    break;
                //case 83:
                //    positionValue = "第三帧";
                //    break;
                //case 84:
                //    positionValue = "大运营位1";
                //    break;
                //case 85:
                //    positionValue = "大运营位2";
                //    break;
                //case 86:
                //    positionValue = "大运营位3";
                //    break;
                //case 87:
                //    positionValue = "大运营位4";
                //    break;
                //case 88:
                //    positionValue = "大运营位5";
                //    break;
                //case 89:
                //    positionValue = "小运营位1";
                //    break;
                //case 90:
                //    positionValue = "小运营位2";
                //    break;
                //case 91:
                //    positionValue = "小运营位3";
                //    break;
                //case 92:
                //    positionValue = "小运营位4";
                //    break;
                //case 93:
                //    positionValue = "小运营位5";
                //    break;

                //case 94:
                //    positionValue = "大运营位6";
                //    break;

                //case 95:
                //    positionValue = "小运营位6";
                //    break;
            }
            return positionValue;
        }
        #endregion

        #region 首页推荐品牌管理
        /// <summary>
        /// 获取首页推荐品牌
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IEnumerable<ChannelRecommendBrandExtends> GetHomeRecommendBrand(string channelNo, string brandNoAndBrandName, string startTime, string endTime, int pageIndex, int pageSize, out int total)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("channelNo", channelNo);
            dic.Add("startTime", string.IsNullOrEmpty(startTime) ? "" : startTime);
            dic.Add("endTime", string.IsNullOrEmpty(endTime) ? "" : endTime);
            dic.Add("brandNoAndBrandName", string.IsNullOrEmpty(brandNoAndBrandName) ? "" : brandNoAndBrandName);
            total = DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelRecommendBrand_GetHomeRecommendByChannelNoTotalCount", dic, new
            {
                channelNo = channelNo,
                startTime = startTime,
                endTime = string.IsNullOrEmpty(endTime) ? endTime : DateTime.Parse(endTime).AddDays(1).ToString(),
                brandNoAndBrandName = brandNoAndBrandName,
                pageIndex = pageIndex,
                pageSize = pageSize
            }).FirstOrDefault();
            return DapperUtil.Query<ChannelRecommendBrandExtends>("ComBeziWfs_SWfsSpChannelRecommendBrand_GetHomeRecommendByChannelNo", dic, new
            {
                channelNo = channelNo,
                startTime = startTime,
                endTime = string.IsNullOrEmpty(endTime) ? endTime : DateTime.Parse(endTime).AddDays(1).ToString(),
                brandNoAndBrandName = brandNoAndBrandName,
                pageIndex = pageIndex,
                pageSize = pageSize
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
            return DapperUtil.Insert<SWfsSpChannelRecommendBrand>(new SWfsSpChannelRecommendBrand
            {
                ChannelNO = channelNo,
                BrandNO = brandNo,
                SortValue = 999,
                CreateDate = DateTime.Now
            }, true);
        }
        /// <summary>
        /// 查询是否存在重复的品牌
        /// </summary>
        /// <param name="channelNo">A01或者A02区分男女</param>
        /// <param name="brandNo">品牌编号</param>
        /// <returns></returns>
        public int IsExistHomeBrand(string channelNo, string brandNo)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsSpChannelRecommendBrand_IsExistsHomeBrand", new
            {
                channelNo = channelNo,
                brandNo = brandNo
            }).FirstOrDefault();
        }

        /// <summary>
        /// 按主键修改首页品牌排序值
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="sortValue">排序值</param>
        /// <returns></returns>
        public bool EditeBrandSortValue(int id, int sortValue)
        {
            return DapperUtil.UpdatePartialColumns<SWfsPictureManager>(new
            {
                RecommendBrandID = id,
                SortId = sortValue
            });
        }
        #endregion

        #region
        /// <summary>
        /// 获取导航列表信息
        /// </summary>
        /// <returns></returns>
        public List<SWfsNavigationManage> GetNavigaList()
        {
            return DapperUtil.Query<SWfsNavigationManage>("ComBeziWfs_SWfsNavigationManage_GetNavigationList").ToList();
        }

        /// <summary>
        /// 获取最大的sort值
        /// </summary>
        /// <returns></returns>
        public int GetMaxSortNavig()
        {
            List<SWfsNavigationManage> list = DapperUtil.Query<SWfsNavigationManage>

 ("ComBeziWfs_SWfsNavigationManage_GetMaxNavigSortId").ToList();
            return list.Count == 0 ? 0 : (list[0].SortId + 1);
        }

        /// <summary>
        /// 添加导航
        /// </summary>
        /// <param name="name"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public int AddNaviga(string name, string link)
        {
            int result = 0;
            SWfsNavigationManage NewDto = new SWfsNavigationManage
            {
                CreateDate = DateTime.Now,
                DataState = 1,
                Status = 1,
                NavigationLink = link,
                NavigationTitle = name,
                SortId = GetMaxSortNavig(),
                UpdateDate = DateTime.Now
            };
            result = DapperUtil.Insert<SWfsNavigationManage>(NewDto);
            return result;
        }

        /// <summary>
        /// 更新导航信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="link"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int UpdateNavigaInfo(string name, string link, string id)
        {
            int result = 0;
            result = DapperUtil.Execute("ComBeziWfs_SWfsNavigationManage_UpdateNavigationInfoById", new
            {
                NavigationTitle = name,
                NavigationLink = link,
                NavigationId = id
            });
            return result;
        }
        /// <summary>
        /// 删除导航信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int delNavigaInfoById(string id)
        {
            int result = 0;
            result = DapperUtil.Execute("ComBeziWfs_SWfsNavigationManage_DeleteNavigationSortById", new
            {
                NavigationId = id
            });
            return result;
        }

        /// <summary>
        /// 移动导航信息
        /// </summary>
        /// <param name="newid"></param>
        /// <param name="newsortid"></param>
        /// <returns></returns>
        public int MoveNaviga(string newid, string newsortid)
        {
            int result = 0;
            result = DapperUtil.UpdatePartialColumns<SWfsNavigationManage>(new
            {
                NavigationId = Convert.ToInt32(newid),
                SortId = Convert.ToInt32(newsortid)

            }) == true ? 1 : 0;

            return result;
        }
        #endregion

    }
}
