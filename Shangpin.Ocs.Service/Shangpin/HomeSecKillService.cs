using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;
namespace Shangpin.Ocs.Service.Shangpin
{
    public class HomeSecKillService
    {
        public List<SWfsHomeSecKill> SelectAllSecKillList()
        {
            return DapperUtil.Query<SWfsHomeSecKill>("ComBeziWfs_SWfsHomeSecKill_SelectAll").ToList();
        }

        public bool CheckTime(DateTime dt, DateTime dt2, string channelNo,int secKillId)
        {
            var result = false;
            var newDateTimeList = SelectAllSecKillList().Where(c => c.ChannelNo == channelNo && c.ShowTime.Date == dt.Date && c.SecKillId != secKillId).Select(c => new { c.ShowTime, c.StartTime }).ToList();
            if (newDateTimeList != null && newDateTimeList.Count > 0)
            {
                newDateTimeList.ForEach(c =>
                {
                    var show = c.ShowTime;
                    var start = c.StartTime;
                    if ((show >= dt && show <= dt2) || (start >= dt && start <= dt2))
                    {
                        result = true;
                        return;
                    }
                    else
                    {
                        result = false;
                    }
                });
            }
            return result;
        }

        public bool GetHomeSekKillByTime(DateTime dateTime, short type, string channelNo, short secKillId)
        {
            var flag = false;
            if (type == 1)//爆款
            {
                List<int> typeList = new List<int>();
                typeList.Add(1);
                typeList.Add(2);
                var oneSecKill = SelectAllSecKillList().Where(c => c.ShowTime.Date == dateTime.Date && typeList.Contains(c.SecKillType) && c.ChannelNo == channelNo && c.SecKillId != secKillId).FirstOrDefault();
                if (oneSecKill != null)
                {
                    flag = true;
                }
            }
            else//秒杀
            {
                var oneSecKill = SelectAllSecKillList().Where(c => c.ShowTime.Date == dateTime.Date && c.SecKillType != type && c.ChannelNo == channelNo && c.SecKillId != secKillId).FirstOrDefault();
                if (oneSecKill != null)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public List<HomeSecKill> SelectSecKillList(Dictionary<string, object> dicParam, int pagesize, int pageindex)
        {
            return DapperUtil.Query<HomeSecKill>("ComBeziWfs_SWfsHomeSecKill_SelectList", dicParam, new { SecKillTitle = dicParam["SecKillTitle"], ProductNo = dicParam["ProductNo"], StartTime = dicParam["StartTime"], EndTime = dicParam["EndTime"], Status = dicParam["SecKillStatus"], SecKillType = dicParam["SecKillType"], ChannelNo = dicParam["ChannelNo"], pageIndex = pageindex, pageSize = pagesize }).ToList();
        }
    }
}
