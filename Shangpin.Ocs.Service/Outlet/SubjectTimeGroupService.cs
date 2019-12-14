using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;

namespace Shangpin.Ocs.Service.Outlet
{
   public class SubjectTimeGroupService
    {

       public IList<SWfsSubjectTimeGroup> GetList(string name, int showType)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(name))
            {
                dic.Add("name", name);
            }
            List<SWfsSubjectTimeGroup> list = DapperUtil.Query<SWfsSubjectTimeGroup>("ComBeziWfs_SWfsSubjectTimeGroup_GetList",dic, new { Name = name, ShowType=showType }).ToList();
            return list;
        }

       public SWfsSubjectTimeGroup GetModelById(int GID)
       {
           return DapperUtil.Query<SWfsSubjectTimeGroup>("ComBeziWfs_SWfsSubjectTimeGroup_GetModelById", new { GID = GID }).FirstOrDefault();
       }

       public void Del(int GID)
       {
           DapperUtil.Execute("ComBeziWfs_SWfsSubjectTimeGroup_DelModelById", new { GID = GID});
       }

       public void Add(SWfsSubjectTimeGroup model)
       {
           DapperUtil.Insert<SWfsSubjectTimeGroup>(model);
       }

       public void Update(SWfsSubjectTimeGroup model)
       {
           DapperUtil.Update<SWfsSubjectTimeGroup>(model);
       }

       public SWfsSubjectTimeGroup SelectByName(string name)
       {
           return DapperUtil.Query<SWfsSubjectTimeGroup>("ComBeziWfs_SWfsSubjectTimeGroup_SelectByName", new { GroupName = name }).FirstOrDefault();
       }
       public SWfsSubjectTimeGroup SelectByTime(DateTime DateBegin,DateTime DateEnd,int gid)
       {
           Dictionary<string, object> dic = new Dictionary<string, object>();
           dic.Add("gid", gid);
           return DapperUtil.Query<SWfsSubjectTimeGroup>("ComBeziWfs_SWfsSubjectTimeGroup_SelectByTime",dic, new { DateBegin = DateBegin, DateEnd = DateEnd,GID=gid}).FirstOrDefault();
       }
       
    }
}
