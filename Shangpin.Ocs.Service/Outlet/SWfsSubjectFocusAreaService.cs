using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Service.Outlet
{
   public class SWfsSubjectFocusAreaService
    {
       public SWfsSubjectFocusArea GetModel(string id)
       {
           SWfsSubjectFocusArea model = DapperUtil.Query<SWfsSubjectFocusArea>("ComBeziWfs_SWfsSubjectFocusArea_GetModel", new { ID = id }).FirstOrDefault();
           return model;
       }

       public IList<SWfsSubjectFocusUIModel> GetList(string subjectNoName, string startTime, string endTime, int pageIndex, int pageSize, out int totalCount)
       {

           Dictionary<string, object> dic = new Dictionary<string, object>();
           dic.Add("subjectNoName", string.IsNullOrWhiteSpace(subjectNoName) ? "0" : subjectNoName);
           dic.Add("startTime", string.IsNullOrWhiteSpace(startTime)?"0":startTime);
           dic.Add("endTime", string.IsNullOrWhiteSpace(endTime)?"0":endTime);

           IList<SWfsSubjectFocusUIModel> list = DapperUtil.Query<SWfsSubjectFocusUIModel>("ComBeziWfs_SWfsSubjectFocusArea_GetList", dic, new { subjectNoName = subjectNoName, startTime = startTime, endTime = endTime }).ToList();
           totalCount = list.Count();
           list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
           return list;
       }

       public void Insert(SWfsSubjectFocusArea model)
       {
           DapperUtil.Insert<SWfsSubjectFocusArea>(model);
       }

       public void Update(SWfsSubjectFocusArea model)
       {
           DapperUtil.Update<SWfsSubjectFocusArea>(model);
       }

       public List<SWfsSubjectFocusArea> GetSWfsSubjectFocusAreaList(DateTime showDate)
       {
           return DapperUtil.Query<SWfsSubjectFocusArea>("ComBeziWfs_SWfsSubjectFocusArea_GetSWfsSubjectFocusAreaList", new { ShowDate = showDate }).ToList();
       }


       public void DelById(string Id)
       {
           SWfsSubjectFocusArea model = GetModel(Id);
           model.Status = 0;
           Update(model);
       }

       public bool SortUpdate(int id, int sort)
       {
           return DapperUtil.UpdatePartialColumns<SWfsSubjectFocusArea>(new { ID = id, Sort = sort });
       }
    }
}
