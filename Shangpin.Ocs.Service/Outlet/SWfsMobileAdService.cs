using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;

namespace Shangpin.Ocs.Service.Outlet
{
    public class SWfsMobileAdService
    {
        public int InsertMobileAd(SWfsMobileAd mobileAd)
        {
            return DapperUtil.Insert<SWfsMobileAd>(mobileAd, false);
        }
        public bool Update(SWfsMobileAd model)
        {
            return DapperUtil.Update<SWfsMobileAd>(model);
        }
        public IList<SWfsMobileAd> GetMobileAdList(string keyWord, string channelNo, string sort, string startTime, string endTime, string status, int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", (keyWord == null || keyWord == "广告标题") ? "" : keyWord);
            dic.Add("Sort", (sort == null || sort == "位置序号") ? "" : sort);
            dic.Add("DateBegin", startTime == null ? "" : startTime);
            dic.Add("DateEnd", endTime == null ? "" : endTime);
            dic.Add("Status", status == "2" ? "" : status);
            IList<SWfsMobileAd> list = DapperUtil.Query<SWfsMobileAd>("ComBeziWfs_SWfsMobileAd_SelectMobileAdList", dic, new
            {
                KeyWord = (keyWord == null || keyWord == "广告标题") ? "" : keyWord,
                ChannelNo = channelNo,
                Sort = (sort == null || sort == "位置序号") ? "" : sort,
                Status = status == "2" ? "" : status,
                DateBegin = startTime == null ? "" : startTime,
                DateEnd = endTime == null ? "" : endTime
            }).ToList();
            count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }

        public int Delete(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsMobileAd_DeleteById", new { ID = id });
        }

        public SWfsMobileAd GetMobileAdInfo(int id)
        {
            return DapperUtil.QueryByIdentityWithNoLock<SWfsMobileAd>(id);
        }

        public bool MobileAdModify(string id, string status, string updateUserId, DateTime updateDate)
        {
            return DapperUtil.UpdatePartialColumns<SWfsMobileAd>(new
            {
                ID = id,
                UpdateUserId = updateUserId,
                UpdateDate = updateDate,
                Status = status
            });
        }
        /// <summary>
        /// 修改序号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public bool UpdateSort(string id, int sort)
        {
            return DapperUtil.UpdatePartialColumns<SWfsMobileAd>(new { ID = id, Sort = sort });
        }

        /// <summary>
        /// 根据频道查询所有
        /// </summary>
        /// <param name="channelNo"></param>
        /// <returns></returns>
        public IList<SWfsMobileAd> GetMobileAdList(string channelNo)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("KeyWord", "");
            dic.Add("Sort", "");
            dic.Add("DateBegin", "");
            dic.Add("DateEnd", "");
            dic.Add("Status", "");
            IList<SWfsMobileAd> list = DapperUtil.Query<SWfsMobileAd>("ComBeziWfs_SWfsMobileAd_SelectMobileAdList", dic, new
            {
                KeyWord = "",
                ChannelNo = channelNo,
                Sort = "",
                Status = "",
                DateBegin = "",
                DateEnd = ""
            }).ToList();
            return list;
        }

        public IList<SWfsMobileAd> GetListBySort(string sort, string channelNo)
        {
            IList<SWfsMobileAd> list = DapperUtil.Query<SWfsMobileAd>("ComBeziWfs_SWfsMobileAd_SelectDuplicates", new { Sort = sort, ChannelNo = channelNo }).ToList();
            return list;
        }
    }
}
