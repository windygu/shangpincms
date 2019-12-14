using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class VCodeService
    {
        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public SWfsVActivity VCodeId(string activityId)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsVActivity>(activityId);
        }

        /// <summary>
        /// 查询一条V码所属
        /// </summary>
        /// <param name="activityTypeId"></param>
        /// <returns></returns>
        public IEnumerable<SWfsVActivityType> VCodeTypeId()
        {
            return DapperUtil.Query<SWfsVActivityType>("ComBeziWfs_WfsCmsContent_SWfsVActivityType_List");
        }

        /// <summary>
        /// 添加V码活动
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int VCodeCreate(SWfsVActivity obj)
        {
            return DapperUtil.Insert<SWfsVActivity>(obj,false);
        }

        /// <summary>
        /// 修改V码活动
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool VCodeUpdate(SWfsVActivity obj)
        {
            return DapperUtil.Update<SWfsVActivity>(obj);
        }

        /// <summary>
        /// 修改活动状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateVCodeStatus(string activityId, int status)
        {
            return DapperUtil.UpdatePartialColumns<SWfsVActivity>(new { ActivityId = activityId, ActivityStatus = status });
        }

        /// <summary>
        /// 删除V码
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public int DeleteVCode(string activityId)
        {
            return DapperUtil.Execute("ComBeziWfs_WfsCmsContent_SWfsVActivity_Delete", new { ActivityId = activityId });
        }

        /// <summary>
        /// 添加微码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CreateCrode(SWfsVActivityCodesRef obj)
        {
            return DapperUtil.Insert<SWfsVActivityCodesRef>(obj, true);
        }

        /// <summary>
        /// 查询用户关联的微码
        /// </summary>
        /// <param name="vcode"></param>
        /// <returns></returns>
        public IEnumerable<SWfsVUserCodesRef> UserCode(string vcode)
        {
            return DapperUtil.Query<SWfsVUserCodesRef>("ComBeziWfs_SWfsVUserCodesRef_InquiryVCode", new { VCode = vcode});
        }
        /// <summary>
        /// 根据活动编号查询活动下的微码
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public IEnumerable<SWfsVActivityCodesRef> ActivityVCode(string activityId)
        {
            return DapperUtil.Query<SWfsVActivityCodesRef>("ComBeziWfs_SWfsVActivityCodesRef_List", new { ActivityId = activityId });
        }

        public IEnumerable<SWfsVUserCodesRef> VCodeRef(string activityId)
        {
            return DapperUtil.Query<SWfsVUserCodesRef>("ComBeziWfs_SWfsVActivityCodesRef_InquiryVCode", new { ActivityId = activityId });
        }

        /// <summary>
        /// 修改微码状态 
        /// </summary>
        /// <param name="vcode"></param>
        /// <param name="status"></param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public int UserCodeStatus(string vcode, int status, string userId)
        {
            //return DapperUtil.UpdatePartialColumns<SWfsVUserCodesRef>(new { VCode = vcode, UseCodeStatus = status });
            return DapperUtil.Execute("ComBeziWfs_SWfsVUserCodesRef_UpdateVCode", new { VCode = vcode, status = status, UserId = userId });
        }
        /// <summary>
        /// 保存专题
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int TopicsRefCreate(SWfsVActivityTopicsRef obj)
        {
            return DapperUtil.Insert<SWfsVActivityTopicsRef>(obj,true);
        }
        /// <summary>
        /// 取消关联
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public int TopicRefDelete(string topicno, string activityId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsVActivityTopicsRef_Delete", new { TopicNo = topicno , ActivityId=activityId});
        }

        /// <summary>
        /// 获取关联信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public SWfsVActivityTopicsRef SelectSingleBySubjectNoRef(string topicno, string activityId)
        {
            return DapperUtil.Query<SWfsVActivityTopicsRef>("ComBeziWfs_SWfsVActivityTopicsRef_SingleBySubjectNo", new { TopicNo = topicno, ActivityId = activityId }).FirstOrDefault();
        }
    }
}
