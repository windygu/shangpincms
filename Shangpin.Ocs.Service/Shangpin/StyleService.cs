using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Framework.Common.Dapper;
using Shangpin.Entity.Wfs;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
using Shangpin.Entity.ComBeziPicLab;
namespace Shangpin.Ocs.Service.Shangpin
{
    public class StyleService
    {
        #region 搭配专题 By lijia 20140909
        /// <summary>
        /// 搭配专题列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="position">位置</param>
        /// <param name="sTime">时间范围（开始）</param>
        /// <param name="eTime">时间范围（结束）</param>
        /// <returns>查询结果数据集合</returns>
        public IList<SWfsStyleMatchSpecial> GetList(string name, string position, string sTime, string eTime, int pageIndex, int pageSize, int type, out int count)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("SpecialName", string.IsNullOrEmpty(name) ? "" : name, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 50);
            param.Add("Position", position, System.Data.DbType.Int16, System.Data.ParameterDirection.Input);
            param.Add("StartTime", (string.IsNullOrEmpty(sTime) ? "1900-01-01" : sTime), System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("EndTime", (string.IsNullOrEmpty(eTime) ? "1900-01-01" : eTime), System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("Type", type,System.Data.DbType.Int16,System.Data.ParameterDirection.Input);
            var dic = new Dictionary<string, object>();
            dic.Add("SpecialName", string.IsNullOrEmpty(name) ? "" : name);
            dic.Add("Position", string.IsNullOrEmpty(position) ? "" : position);
            dic.Add("StartTime", string.IsNullOrEmpty(sTime) ? "" : sTime);
            dic.Add("EndTime", string.IsNullOrEmpty(eTime) ? "" : eTime);
            dic.Add("pageSize", pageSize);
            IList<SWfsStyleMatchSpecial> list = DapperUtil.Query<SWfsStyleMatchSpecial>("ComBeziWfs_SWfsMatchSpecial_GetList", dic, param).ToList();
            count = list.Count();
            if (pageSize != 0)
            {
                list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            return list;
        }
        /// <summary>
        /// 根据主键获得实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SWfsStyleMatchSpecial GetModel(int id)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsStyleMatchSpecial>(id);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(SWfsStyleMatchSpecial model)
        {
            return DapperUtil.Insert(model);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(SWfsStyleMatchSpecial model)
        {
            return DapperUtil.Update(model);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(string id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsStyleMatchSpecial_DeleteById", new { ID = id });
        }

        #endregion
        #region 标签展示 By lijibo
        /// <summary>
        /// 标签展示列表（无区分哪一列）
        /// </summary>
        /// <returns></returns>
        public IList<SWfsTagRel> GetTagsList()
        {
            IList<SWfsTagRel> list = DapperUtil.Query<SWfsTagRel>("ComBeziWfs_SWfsTagRel_GetList").ToList();
            return list;
        }

        /// <summary>
        /// 标签展示列表根据位置获取
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public IList<SWfsTagRel> GetTagsList(short location)
        {
            IList<SWfsTagRel> list = DapperUtil.Query<SWfsTagRel>("ComBeziWfs_SWfsTagRel_GetListByLocation", new { Location = location }).ToList();
            return list;
        }

        /// <summary>
        /// 标签展示列表根据位置获取
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public int GetDataCount(short location)
        {
            return DapperUtil.Query<int>("ComBeziWfs_SWfsTagRel_GetDataCountByLocation", new { Location = location }).First<int>();
        }

        /// <summary>
        /// 根据编号删除标签展示(多标签)
        /// </summary>
        /// <param name="tagNo"></param>
        /// <returns></returns>
        public int DeleteTagsById(string tagNos, short location)
        {
            string[] tagNosArray = tagNos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return DapperUtil.Execute("ComBeziWfs_SWfsTagRel_DeleteByTagNos", new { TagNo = tagNosArray, Location = location });
        }


        /// <summary>
        /// 根据位置删除标签展示
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public int DeleteTagsByLocation(short location)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsTagRel_DeleteByLocation", new { Location = location });
        }


        /// <summary>
        /// 根据编号添加标签展示
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddTags(SWfsTagRel model)
        {
            return DapperUtil.Insert(model);
        }

        /// <summary>
        /// 修改标签展示顺序
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateTags(SWfsTagRel model)
        {
            return DapperUtil.Update(model);
        }
        #endregion

        #region 标签添加列表操作 By lijibo

        /// <summary>
        ///标签添加列表
        /// </summary>
        /// <param name="tag_name">标签名</param>
        /// <param name="sTime">时间范围（开始）</param>
        /// <param name="eTime">时间范围（结束）</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总数</param>
        /// <returns></returns>
        public IList<T_tag_baseExtenstion> GetTagsCreatList(string tag_name, string sTime, string eTime, int pageIndex, int pageSize, out int count)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("tag_name", string.IsNullOrEmpty(tag_name) ? "" : tag_name, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 255);
            param.Add("StartTime", (string.IsNullOrEmpty(sTime) ? "1900-01-01" : sTime), System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("EndTime", (string.IsNullOrEmpty(eTime) ? "1900-01-01" : eTime), System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            var dic = new Dictionary<string, object>();
            dic.Add("tag_name", string.IsNullOrEmpty(tag_name) ? "" : tag_name);
            dic.Add("StartTime", string.IsNullOrEmpty(sTime) ? "" : sTime);
            dic.Add("EndTime", string.IsNullOrEmpty(eTime) ? "" : eTime);
            IList<T_tag_baseExtenstion> list = DapperUtil.Query<T_tag_baseExtenstion>("ComBeziPicLab_t_tag_base_GetTagsCreateList", dic, param).ToList();
            count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }


        /// <summary>
        /// 根据id获取标签添加列表（多个","分开）
        /// </summary>
        /// <param name="tag_id"></param>
        /// <returns></returns>
        public IList<t_tag_base> GetTagsCreatListByTagIds(string tag_id)
        {
            string[] tagNosArray = tag_id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            IList<t_tag_base> list = DapperUtil.Query<t_tag_base>("ComBeziPicLab_t_tag_base_GetTagsCreateListBytag_id", new { tag_id = tagNosArray }).ToList();
            return list;
        }

        #endregion

        #region 活动图管理
        /// <summary>
        /// 添加活动图
        /// </summary>
        /// <param name="model">信息</param>
        /// <returns></returns>
        public int InsertStyleActivityPic(SWfsStyleActivityPic model)
        {
            return DapperUtil.Insert(model, false);
        }

        /// <summary>
        /// 活动图管理列表
        /// </summary>
        /// <param name="keyword">活动图名称</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex"当前页></param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        public RecordPage<SWfsStyleActivityPicM> SelectActivityPicList(string keyword, string startTime, string endTime, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyword == null || keyword == "活动图名称") ? "" : keyword.Trim());
            dic.Add("StartTime", startTime == null ? "" : startTime);
            dic.Add("EndTime", endTime == null ? "" : endTime);
            DynamicParameters param = new DynamicParameters();
            param.Add("ActivityName", keyword, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 100);
            param.Add("StartTime", (!string.IsNullOrWhiteSpace(startTime) ? startTime : "1900-01-01 00:00:00"), System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            param.Add("EndTime", (!string.IsNullOrWhiteSpace(endTime) ? endTime : "1900-01-01 00:00:00"), System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
            IEnumerable<SWfsStyleActivityPicM> query = DapperUtil.QueryPaging<SWfsStyleActivityPicM>("ComBeziWfs_SWfsStyleActivityPic_FindStyleActivityPicList", pageIndex, pageSize, "StartTime desc,CreateDate desc", dic, param);
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }

        /// <summary>
        /// 根据活动图ID获取信息
        /// </summary>
        /// <param name="id">活动图编号</param>
        /// <returns></returns>
        public SWfsStyleActivityPic GetStyleActivityPicModel(string id)
        {
            return DapperUtil.Query<SWfsStyleActivityPic>("ComBeziWfs_SWfsStyleActivityPic_GetActivityModel", new { SAID = id }).FirstOrDefault();
        }

        /// <summary>
        /// 修改活动图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateStyleActivityPic(SWfsStyleActivityPic model)
        {
            return DapperUtil.UpdatePartialColumns<SWfsStyleActivityPic>(new
            {
                SAID = model.SAID,
                ActivityName = model.ActivityName,
                PicNo = model.PicNo,
                PicUrl = model.PicUrl,
                StartTime = model.StartTime,
                CreateUserId = model.CreateUserId
            });
        }

        /// <summary>
        /// 根据活动图ID删除活动图
        /// </summary>
        /// <param name="id">活动图编号</param>
        /// <returns></returns>
        public int DelStyleActivityPic(string id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsStyleActivityPic_DeleteActivityPicById", new { SAID = id });
        }
        #endregion

        #region 轮播图管理
        
        #endregion
    }
}
