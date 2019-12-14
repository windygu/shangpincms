using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;

namespace Shangpin.Ocs.Service.Outlet
{ 
   public class WfsCmsContentService
    {
       public WfsCmsContent GetModel(string id)
        {
            return DapperUtil.Query<WfsCmsContent>("ComBeziWfs_WfsCmsContent_GetWfsCmsContentModel", new { CmsContentNo = id }).FirstOrDefault();
        }

       public void Add(WfsCmsContent model) 
        {
            DapperUtil.Insert<WfsCmsContent>(model);
        }

       public bool Update(WfsCmsContent model)
        {
            return DapperUtil.Update<WfsCmsContent>(model);
        }

       public IList<WfsCmsContent> GetList(string title, string datecreate)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("Title", string.IsNullOrEmpty(title) ? "" : title);
            dic.Add("PublishTime", string.IsNullOrEmpty(datecreate) ? "" : datecreate);
            return DapperUtil.Query<WfsCmsContent>("ComBeziWfs_WfsCmsContent_GetWfsCmsContentList", dic, new { Title = title, PublishTime = datecreate, CmsContentCategoryName ="首页公告"}).ToList();
        }

        public void Del(string id)
        {
            DapperUtil.Execute("ComBeziWfs_WfsCmsContent_DelWfsCmsContentById", new { CmsContentNo = id });
        }

        public void Update(string id, int value)
        {
            DapperUtil.Execute("ComBeziWfs_WfsCmsContent_UpdateWfsCmsContentById", new { CmsContentNo = id, ShowStatus = value });
        }
    }
}
