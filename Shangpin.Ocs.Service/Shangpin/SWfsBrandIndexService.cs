using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Framework.Common.Dapper;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
namespace Shangpin.Ocs.Service.Shangpin
{
    public class SWfsBrandIndexService
    {
        private static SWfsBrandIndexService instance;
        private SWfsBrandIndexService()
        {

        }
        public static SWfsBrandIndexService GetInstance()
        {
            if (instance == null)
            {
                instance = new SWfsBrandIndexService();
            }
            return instance;
        }
        /// <summary>
        /// 获取尚品品牌首页热门品牌和旗舰店展示图片列表 by zhangwei 20140902
        /// </summary>
        /// <param name="typeId">类型 1旗舰店展示图，2热门品牌</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public RecordPage<BrandIndexM> GetBrandIndexDataList(int typeId, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("TypeId", typeId);
            DynamicParameters param = new DynamicParameters();
            param.Add("TypeId", typeId, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            List<BrandIndexM> list = new List<BrandIndexM>();
            IEnumerable<BrandIndexM> query = DapperUtil.QueryPaging<BrandIndexM>("ComBeziWfs_SWfsBrandIndex_FindBrandIndexDataList", pageIndex, pageSize, "SWfsBrandIndex.Sort ASC ,SWfsBrandIndex.DateCreate ASC", dic, param);
            if (query != null && query.Count() > 0)
            {
                list = query.ToList();
            }
            list = (list == null ? new List<BrandIndexM>() : list);
            return PageConvertor.Convert(pageIndex, pageSize, list);
        }
        /// <summary>
        /// EP改版 20141003 by lijia
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public RecordPage<BrandIndexM> GetBrandIndexDataListNew(int typeId, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("TypeId", typeId);
            DynamicParameters param = new DynamicParameters();
            param.Add("TypeId", typeId, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
            List<BrandIndexM> list = new List<BrandIndexM>();
            IEnumerable<BrandIndexM> query = DapperUtil.QueryPaging<BrandIndexM>("ComBeziWfs_SWfsBrandIndex_FindBrandIndexDataListNew", pageIndex, pageSize, "SWfsBrandIndex.Sort ASC ,SWfsBrandIndex.DateCreate ASC", dic, param);
            if (query != null && query.Count() > 0)
            {
                list = query.ToList();
            }
            list = (list == null ? new List<BrandIndexM>() : list);
            return PageConvertor.Convert(pageIndex, pageSize, list);
        }
        /// <summary>
        /// 根据查询条件获得运营位置列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="position">位置</param>
        /// <param name="sTime">运营范围（开始）</param>
        /// <param name="eTime">运营范围（结束）</param>
        /// <returns></returns>
        public IList<SWfsBrandAdsInfo> GetList(string name, string position, string sTime, string eTime)
        {
            DynamicParameters adParam = new DynamicParameters();
            adParam.Add("AdName", string.IsNullOrEmpty(name) ? "" : name, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 50);
            adParam.Add("Position", position, System.Data.DbType.Int16, System.Data.ParameterDirection.Input);
            adParam.Add("StartTime", (string.IsNullOrEmpty(sTime) ? "1900-01-01" : sTime), System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            adParam.Add("EndTime", (string.IsNullOrEmpty(eTime) ? "1900-01-01" : eTime), System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("AdName", name);
            dic.Add("Position", position);
            dic.Add("StartTime", sTime);
            dic.Add("EndTime", eTime);
            return DapperUtil.Query<SWfsBrandAdsInfo>("ComBeziWfs_SWfsBrandAdsInfo_GetSWfsBrandAdsInfoList", dic, adParam).ToList();
        }

        /// <summary>
        /// 根据运营位开始时间获得数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public SWfsBrandAdsInfo GetByTime(string time, string position)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("StartTime", time);
            param.Add("Position", position);
            return DapperUtil.Query<SWfsBrandAdsInfo>("ComBeziWfs_SWfsBrandAdsInfo_GetByStartTime", param).FirstOrDefault();
        }

        /// <summary>
        /// 根据主键获得实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SWfsBrandAdsInfo GetModel(int id)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsBrandAdsInfo>(id);
        }

        /// <summary>
        /// 添加运营位广告
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(SWfsBrandAdsInfo model)
        {
            return DapperUtil.Insert(model);
        }
        /// <summary>
        /// 修改运营位广告
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(SWfsBrandAdsInfo model)
        {
            return DapperUtil.Update(model);
        }

        /// <summary>
        /// 删除运营位置图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(string id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsBrandAdsInfo_DeleteById", new { ID = id });
        }
    }
}
