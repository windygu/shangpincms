using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;

namespace Shangpin.Ocs.Service.Outlet
{
   public class SWfsAolaiAdvancePageService
    {
       public SWfsAolaiAdvancePage GetModel(string id)
        {
            return DapperUtil.Query<SWfsAolaiAdvancePage>("ComBeziWfs_SWfsAolaiAdvancePage_GetSWfsAolaiAdvancePageModel", new { SWfsAolaiNoticePageId = id }).FirstOrDefault();
        }

        public void Add(SWfsAolaiAdvancePage model)
        {
            DapperUtil.Insert<SWfsAolaiAdvancePage>(model);
        }

        public bool Update(SWfsAolaiAdvancePage model)
        {
            return DapperUtil.Update<SWfsAolaiAdvancePage>(model);
        }

        public void Del(string id)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsAolaiAdvancePage_DelSWfsAolaiAdvancePage", new { SWfsAolaiNoticePageId = id });
        }

        public IList<SWfsAolaiAdvancePage> GetList(string title, string datecreate, int pageIndex, int pageSize, int islimitedOutlet, out int totalCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("PageName", string.IsNullOrEmpty(title) ? "" : title);
            dic.Add("DateCreate", string.IsNullOrEmpty(datecreate) ? "" : datecreate);
            totalCount = DapperUtil.Query<int>("ComBeziWfs_SWfsAolaiAdvancePage_GetSWfsAolaiAdvancePageListTotal", dic, new { PageName = title, DateCreate = datecreate, pageIndex = pageIndex, pageSize = pageSize, isLimitedOutlet = islimitedOutlet }).FirstOrDefault();
            return DapperUtil.Query<SWfsAolaiAdvancePage>("ComBeziWfs_SWfsAolaiAdvancePage_GetSWfsAolaiAdvancePageList", dic, new { PageName = title, DateCreate = datecreate, pageIndex = pageIndex, pageSize = pageSize, isLimitedOutlet = islimitedOutlet }).ToList();
        }

        public IList<SWfsAolaiAdvancePagePic> GetPicList(string id)
        {
            return DapperUtil.Query<SWfsAolaiAdvancePagePic>("ComBeziWfs_SWfsAolaiAdvancePagePic_GetSWfsAolaiAdvancePagePicList", new { SwfsAolaiNoticePageId = id }).ToList();
        }

        public SWfsAolaiAdvancePagePic GetPicModel(string picId)
        {
            return DapperUtil.Query<SWfsAolaiAdvancePagePic>("ComBeziWfs_SWfsAolaiAdvancePagePic_GetSWfsAolaiAdvancePagePicModel", new { SWfsAolaiNoticePagPicId = picId }).FirstOrDefault();
        }

        public void DelPic(string picId)
        {
            DapperUtil.Execute("ComBeziWfs_SWfsAolaiAdvancePagePic_DelSWfsAolaiAdvancePagePic", new { SWfsAolaiNoticePagPicId = picId });
        }

        public bool UpdatePagePic(SWfsAolaiAdvancePagePic model)
        {
            return DapperUtil.Update<SWfsAolaiAdvancePagePic>(model);
        }

        public void AddPic(SWfsAolaiAdvancePagePic model)
        {
            DapperUtil.Insert<SWfsAolaiAdvancePagePic>(model);
        }
    }
}
