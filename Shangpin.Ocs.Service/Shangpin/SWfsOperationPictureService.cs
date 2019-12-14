using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Ocs.Entity.Extenstion.ShangPin; 

namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsOperationPictureService
    {
        public SWfsOperationPicture GetModel(string pictureManageId)
        {
            return DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureModel", new { PictureManageId = pictureManageId }).FirstOrDefault();
        }

        public bool Update(SWfsOperationPicture model)
        {
            return DapperUtil.Update<SWfsOperationPicture>(model);
        }

        public void Add(SWfsOperationPicture model)
        {
            DapperUtil.Insert<SWfsOperationPicture>(model);
        }
        public int Add(SWfsOperationPicture model, bool bl)
        {
            return DapperUtil.Insert<SWfsOperationPicture>(model, bl);
        }
        public int Del(string picManId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsPictureManager_DelSWfsPictureManagerById", new { PictureManageId = picManId });
        }

        /// <summary>
        /// 新版首页
        /// </summary>
        /// <param name="picManId"></param>
        /// <returns></returns>
        public int DelSwitchPicture(string picManId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsOperationPicture_DelSWfsOperationPictureById", new { PictureManageId = picManId });
        }
        /// <summary>
        /// 查看轮播图每一帧的数据情况，再进行其他操作
        /// </summary>
        public IEnumerable<SWfsOperationPicture> GetSWfsOperationPictureSwitchCondition(string pageNo, string pagePositionNo, int pictureIndex = -1)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("PageNo", pageNo);
            dic.Add("PagePositionNo", pagePositionNo);
            dic.Add("PictureIndex", pictureIndex);
            return DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureByPageNoPositionNoPiceureIndex", dic, new { PageNo = pageNo, PagePositionNo = pagePositionNo, PictureIndex = pictureIndex });
        }

        public IList<SWfsOperationPicture> GetIndexOperationPictureBottom(string pageNo, string pagePositionNo)
        {

            var dic = new Dictionary<string, object>();
            dic.Add("PageNo", pageNo);
            dic.Add("PagePositionNo", pagePositionNo);
            var resultList = DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureBottom", dic, new { PageNo = pageNo, PagePositionNo = pagePositionNo }).ToList();
            return resultList;
        }

        public IList<SWfsOperationPicture> GetIndexOperationPicture(string pageNo, string pagePositionNo)
        {

            var dic = new Dictionary<string, object>();
            dic.Add("PageNo", pageNo);
            dic.Add("PagePositionNo", pagePositionNo);
            var resultList = DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureByPageNo", dic, new { PageNo = pageNo, PagePositionNo = pagePositionNo }).ToList();

            return resultList;
        }

        #region 20140821 Top首页改版，首页轮播图位置

        /// <summary>
        /// 尚品首页获取位置图片列表"3张轮播图设置：第一帧：1    第二帧：2    第三帧：3    模板宽频：4
        /// 这个分布方法通用，所以这个还用原来的SQL语句
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <returns></returns>
        public IList<SWfsOperationPicture> GetSWfsPictureManagerListTopSwitchV(ref PaginationInfoModel pageModel)
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
            object param = new { tableName = " SWfsOperationPicture  with (NoLock) ", pageSize = pageModel.PageSize, curPage = pageModel.CurrentPage, orderBy = pageModel.OrderBy, selectfield = pageModel.SelectField, field = pageModel.Field, condition = pageModel.Condition };
            List<SWfsOperationPicture> list = DapperUtil.QueryPageList<SWfsOperationPicture>("ComBeziWfs_SWfsNewSubject_PageListSearch", ref totalcount, param).ToList();
            pageModel.TotalPage = CommonHelp.GetPageCount(pageModel.PageSize, totalcount);
            pageModel.TotalCount = totalcount;
            return list;
        }


        public IList<SWfsOperationPicture> GetIndexSwitchPictures(string pageNo,string pagePositionNo)
        {
            return DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPicture", new
            {
                PageNo = pageNo,
                PagePositionNo = pagePositionNo
            }).ToList();
        }
        public IList<SWfsOperationPicture> GetIndexSwitchPicturesImgMap()
        {
          
            return DapperUtil.Query<SWfsOperationPicture>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureImgMap", null).ToList();
        }
        public int GetIndexSwitchPicturesImgMapCount()
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsOperationPicture_GetSWfsOperationPictureImgMapCount", null).First<int>();
        }  


        public int DeleteHomeBrand(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSpHomeRecommendBrand_deleteHomeRecommendBrandByID", new
            {
                RecommendBrandID = id
            });

        }
        #endregion
    }
}
