using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;


namespace Shangpin.Ocs.Service.Outlet
{
    public class VenueService
    {
        public IList<SWfsMeetingTemplatePicInfo> GetTemplatePicListByTID(string meetingNo, short type)
        {
            if (string.IsNullOrEmpty(meetingNo))
            {
                return null;
            }
            return DapperUtil.Query<SWfsMeetingTemplatePicInfo>("ComBeziWfs_SWfsMeetingTemplatePicInfo_SelectObjByTemplateNO", new { TemplateID = meetingNo, Type = type }).ToList();
        }

        public IEnumerable<MeetingRelationRegion> GetRelationRegionListNew(string MeetingNO, string TemplateNO)
        {
            return DapperUtil.Query<MeetingRelationRegion>("ComBeziWfs_SWfsMeetingRelationRegion_SelectMeetingRelationNew", new { MeetingNO = MeetingNO, TemplateNO = TemplateNO });
        }

        public SWfsMeetingActiveSpecial GetActiveSpecialInfoById(int activeId)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsMeetingActiveSpecial>(activeId);

        }
        /// <summary>
        /// 保存会场HTML
        /// </summary>
        /// <param name="obj">会场html信息</param>
        /// <param name="ispre">是否预热</param>
        /// <param name="ismobile">是否移动端</param>
        /// <returns></returns>
        public bool SaveVenueHtml(SWfsMeetingInfoHtml obj, int ispre, int ismobile)
        {
            if (obj.ID == 0)
            {
                return false;
            }

            if (ispre == 0 && ismobile == 0)//移动端预热
            {
                return DapperUtil.UpdatePartialColumns<SWfsMeetingInfoHtml>(new
                {
                    ID = obj.ID,
                    MobilePreViewCode = obj.MobilePreViewCode,
                    TemplateType = obj.TemplateType,
                    IsPublish = obj.IsPublish,
                    UpdateDate = obj.UpdateDate,
                    UpdateUserId = obj.UpdateUserId
                });
            }
            else if (ispre == 1 && ismobile == 0)//移动端开始
            {
                return DapperUtil.UpdatePartialColumns<SWfsMeetingInfoHtml>(new
                {
                    ID = obj.ID,
                    MobileStartCode = obj.MobileStartCode,
                    TemplateType = obj.TemplateType,
                    IsPublish = obj.IsPublish,
                    UpdateDate = obj.UpdateDate,
                    UpdateUserId = obj.UpdateUserId
                });
            }
            else if (ispre == 0 && ismobile == 1)//web端预热

            {
                return DapperUtil.UpdatePartialColumns<SWfsMeetingInfoHtml>(new
                {
                    ID = obj.ID,
                    WebPreViewCode = obj.WebPreViewCode,
                    TemplateType = obj.TemplateType,
                    IsPublish = obj.IsPublish,
                    UpdateDate = obj.UpdateDate,
                    UpdateUserId = obj.UpdateUserId
                });
            }
            else //web端开始
            {
                return DapperUtil.UpdatePartialColumns<SWfsMeetingInfoHtml>(new
                {
                    ID = obj.ID,
                    WebStartCode = obj.WebStartCode,
                    TemplateType = obj.TemplateType,
                    IsPublish = obj.IsPublish,
                    UpdateDate = obj.UpdateDate,
                    UpdateUserId = obj.UpdateUserId
                });
            }
        }

        public SWfsMeetingInfoHtml GetHtmlByMeetingId(int meetingId)
        {
            return DapperUtil.Query<SWfsMeetingInfoHtml>("ComBeziWfs_SWfsMeetingInfoHtml_GetHtmlByMeetingId", new { MettingId = meetingId }).FirstOrDefault();
        }

        public int AddMeetingHtml(SWfsMeetingInfoHtml html)
        {
            return DapperUtil.Insert<SWfsMeetingInfoHtml>(html);

        }
    }
}
