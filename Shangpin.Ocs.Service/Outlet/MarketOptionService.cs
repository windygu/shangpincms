using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Entity.Common;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Service.Outlet
{
    public class MarketOptionService
    {
        public int AddApply(SWfsSubjectApplyPromotion model)
        {
            return DapperUtil.Insert<SWfsSubjectApplyPromotion>(model, true);
        }

        public SWfsSubjectApplyPromotion GetModelBySubjectNo(string subjectNo)
        {
            return DapperUtil.Query<SWfsSubjectApplyPromotion>("ComBeziWfs_SWfsSubjectApplyPromotion_GetModelBySubjectNo", new { subjectNo = subjectNo }).FirstOrDefault();
        }

        public RecordPage<SubjectInfo> AddApplyList(MarketOptionSearchParm parm, int pageIndex = 1, int pageSize = 10)
        {
           
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (parm == null || parm.SubjectNoName == "活动名称或活动编号" || string.IsNullOrEmpty(parm.SubjectNoName)) ? "" : parm.SubjectNoName);
            dic.Add("BrandNo", (parm == null || string.IsNullOrEmpty(parm.BrandNo)) ? "" : parm.BrandNo);
            dic.Add("ApplyBeginTime", (parm == null || string.IsNullOrEmpty(parm.ApplyBeginTime)) ? "" : parm.ApplyBeginTime);
            dic.Add("ApplyEndTime", (parm == null || string.IsNullOrEmpty(parm.ApplyEndTime)) ? "" : parm.ApplyEndTime);
            dic.Add("SpreadStatus", (parm == null || string.IsNullOrEmpty(parm.SpreadStatus)) ? "" : parm.SpreadStatus);
            dic.Add("Level", (parm == null || string.IsNullOrEmpty(parm.Level)) ? "" : parm.Level);
            dic.Add("SubjectType", (parm == null || string.IsNullOrEmpty(parm.SubjectType)) ? "" : parm.SubjectType);
            dic.Add("CategoryNo", (parm == null || string.IsNullOrEmpty(parm.CategoryNo)) ? "" : parm.CategoryNo);
            IEnumerable<SubjectInfo> query = DapperUtil.QueryPaging<SubjectInfo>("ComBeziWfs_SWfsSubjectApply_SubjectList", pageIndex, pageSize, "CreateDateTime desc", dic, new {
                KeyWord = parm.SubjectNoName,
                BrandNo = parm.BrandNo,
                ApplyBeginTime = parm.ApplyBeginTime,
                ApplyEndTime = string.IsNullOrWhiteSpace(parm.ApplyEndTime) ? "" : Convert.ToDateTime(parm.ApplyEndTime).AddDays(1).ToString("yyyy-MM-dd"),
                SpreadStatus = parm.SpreadStatus,
                Level = parm.Level,
                SubjectType = parm.SubjectType,
                CategoryNo = parm.CategoryNo
            });

            Dictionary<string, List<SWfsSubjectChannelSordRef>> dicSordRef =new SWfsSubjectService().GetSordBySubjectNoList(query.Select(x => x.SubjectNo).ToArray());

            foreach (var subject in query)
            {
                subject.ChannelSordList = dicSordRef.Keys.Contains(subject.SubjectNo) ? dicSordRef[subject.SubjectNo] : null;
            }
            return PageConvertor.Convert(pageIndex, pageSize, query.ToList());
        }

        public int AddPromotionChannel(SWfsSubjectPromotionChannel model)
        {
            return DapperUtil.Insert<SWfsSubjectPromotionChannel>(model,true);
        }
        public SWfsSubjectPromotionChannel SelectPromotionChannel(string name)
        {
            return DapperUtil.Query<SWfsSubjectPromotionChannel>("ComBeziWfs_SWfsSubjectPromotionChannel_GetModelByChannelName", new { ChannelName = name }).FirstOrDefault();
        }


        public List<SWfsSubjectPromotionChannel> SelectPromotionChannelList()
        {
            return DapperUtil.Query<SWfsSubjectPromotionChannel>("ComBeziWfs_SWfsSubjectPromotionChannel_SelectList").ToList();
        }

        public void UpdateApplyPromotion(SWfsSubjectApplyPromotion model)
        {
            DapperUtil.Update<SWfsSubjectApplyPromotion>(model);
        }
    }
}
