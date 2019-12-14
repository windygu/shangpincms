using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;

namespace Shangpin.Ocs.Service.Outlet
{
    public class SWfsChannelService
    {
        public IList<SWfsChannel> GetList(int pageIndex, int pageSize, out int count)
        {
            IList<SWfsChannel> list = DapperUtil.Query<SWfsChannel>("ComBeziWfs_SWfsChannel_SelectChannelList", null).ToList();
            count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }

        /// <summary>
        /// 保存排序
        /// </summary>
        /// <param name="channelNo"></param>
        /// <param name="sortNo"></param>
        /// <returns></returns>
        public bool UpdateSort(string channelNo, int sortNo)
        {
            return DapperUtil.UpdatePartialColumns<SWfsChannel>(new { ChannelNo = channelNo, SortNo = sortNo });
        }


        public int InsertChannel(SWfsChannel channel)
        {
            return DapperUtil.Insert<SWfsChannel>(channel, false);
        }

        public int InsertChannelSordRef(SWfsChannelSordRef sordref)
        {
            int i = DapperUtil.Insert<SWfsChannelSordRef>(sordref, false);
            return i;
        }

        public SWfsChannel GetChannelInfo(string channelNo)
        {
            return DapperUtil.QueryByIdentity<SWfsChannel>(channelNo);
        }

        public IList<SWfsChannelSordRef> GetSordByChannelNo(string channelNo)
        {
            return DapperUtil.Query<SWfsChannelSordRef>("ComBeziWfs_SWfsChannelSordRef_SelectByChannelNo", new { ChannelNo = channelNo }).ToList();

        }

        public SWfsChannel GetChannelByName(string name)
        {
            IList<SWfsChannel> list = DapperUtil.Query<SWfsChannel>("ComBeziWfs_SWfsChannel_SelectChannelList", null).ToList();
            SWfsChannel channel = list.Where(c => c.ChannelName == name).FirstOrDefault();
            return channel;
        }

        public bool UpdateChannel(SWfsChannel channel)
        {
            return DapperUtil.UpdatePartialColumns<SWfsChannel>(new { ChannelNo = channel.ChannelNo, ChannelName = channel.ChannelName, Status = channel.Status });
        }

        public int DeleteChannelSordRef(string channelNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsChannelSordRef_DeleteByChannelNo", new { ChannelNo = channelNo });
        }

        public int DeleteChannel(string channelNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsChannel_DeleteByChannelNo", new { ChannelNo = channelNo });
        }

        public bool UpdateStatus(SWfsChannel channel)
        {
            return DapperUtil.UpdatePartialColumns<SWfsChannel>(new { ChannelNo = channel.ChannelNo, Status = channel.Status });
        }

        public IList<SubjectInfo> GetSubjectList(string type, string channelNo, string spreadStatus, string promotionType, string status, string belongsSubjectType, string subjectName, string startTime, string endTime, int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Type", Convert.ToInt32(type));
            dic.Add("ChannelNo", channelNo);
            dic.Add("SpreadStatus", spreadStatus);
            dic.Add("PromotionType", promotionType);
            dic.Add("Status", status);
            dic.Add("BelongsSubjectType", belongsSubjectType);
            dic.Add("Key", (subjectName == null || subjectName == "活动名称/活动编号") ? "" : subjectName);
            dic.Add("DateBegin", startTime == null ? "" : startTime);
            dic.Add("DateEnd", endTime == null ? "" : endTime);
            string sqlId = string.Empty;
            switch (type)
            {
                case "4": //频道页中的主推
                    sqlId = "ComBeziWfs_SWfsSubject_GetChannelSubjectByZhuTui";
                    break;

                case "7": //首页--轮播图
                    sqlId = "ComBeziWfs_SWfsSubjectTopExpand_GetSWfsSubjectTopExpand_List";
                    break;

                case "5":
                case "1":
                    if (channelNo == "sy")
                    {
                        //首页今日品牌推荐//首页今日活动推荐
                        sqlId = "ComBeziWfs_SWfsSubject_Index_GetChannelSubjectByType";
                    }
                    else
                    {
                        //频道页中的正在售卖，即将开始，假日预约
                        sqlId = "ComBeziWfs_SWfsSubject_GetChannelSubjectByType";
                    }
                    break;

                default: //频道页中的正在售卖，即将开始，假日预约
                    sqlId = "ComBeziWfs_SWfsSubject_GetChannelSubjectByType";
                    break;
            }


            IList<SubjectInfo> list = DapperUtil.Query<SubjectInfo>(sqlId, dic, new
            {
                Type = Convert.ToInt32(type),
                ChannelNo = channelNo,
                SpreadStatus = spreadStatus,
                PromotionType = promotionType,
                Status = status,
                BelongsSubjectType = belongsSubjectType,
                Key = subjectName,
                DateBegin = startTime == null ? "" : startTime,
                DateEnd = endTime == null ? "" : endTime
            }).ToList();
            count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }
        #region 频道主推活动
        //获取频道主推活动信息列表
        public IList<ChannelFeaturedEventsInfo> GetChannelFeaturedEventsList(string channelNo, string subjectName, string startTime, string endTime, int pageIndex, int pageSize, out int count)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ChannelNo", channelNo);
            dic.Add("Key", (subjectName == null || subjectName == "活动名称/活动编号") ? "" : subjectName);
            dic.Add("DateBegin", startTime == null ? "" : startTime);
            dic.Add("DateEnd", endTime == null ? "" : endTime);
            IList<ChannelFeaturedEventsInfo> list = DapperUtil.Query<ChannelFeaturedEventsInfo>("ComBeziWfs_SWfsSubject_GetChannelFeaturedEvents", dic, new
            {
                ChannelNo = channelNo,
                Key = subjectName,
                DateBegin = startTime == null ? "" : startTime,
                DateEnd = endTime == null ? "" : endTime
            }).ToList();
            count = list.Count();
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }
        //根据id获取频道主推活动信息
        public SWfsSubjectFeaturedEvents GetFeatureEventInfo(int iD) 
        {
            SWfsSubjectFeaturedEvents model = DapperUtil.Query<SWfsSubjectFeaturedEvents>("ComBeziWfs_SWfsSubject_GetChannelFeaturedEventByID", new { ID = iD }).FirstOrDefault();
            return model;
        }
        //根据活动位置和活动时间获取频道主推活动信息
        public List<SWfsSubjectFeaturedEvents> GetFeatureEventsByDateShow(string channelNo,DateTime dateShow,int location)
        {
            List<SWfsSubjectFeaturedEvents> list = DapperUtil.Query<SWfsSubjectFeaturedEvents>("ComBeziWfs_SWfsSubject_GetChannelFeaturedEventByLocation", new
            {
                ChannelNo = channelNo,
                DateShow = dateShow,
                Location = location
            }).ToList();
            return list;
        }
        /// <summary>
        /// 根据活动编号获取活动信息
        /// </summary>
        /// <param name="subjectNo"></param>
        /// <returns></returns>
        public SubjectInfo GetSubjectInfoBySubjectNo(string subjectNo)
        {
            return DapperUtil.Query<SubjectInfo>("ComBeziWfs_SWfsSubject_GetSubjectInfoBySubjectNo", new { SubjectNo = subjectNo }).FirstOrDefault();
        }
        //添加频道主推活动
        public void Insert(SWfsSubjectFeaturedEvents model)
        {
            DapperUtil.Insert<SWfsSubjectFeaturedEvents>(model);
        }
        //修改频道主推活动
        public void Update(SWfsSubjectFeaturedEvents model)
        {
            DapperUtil.Update<SWfsSubjectFeaturedEvents>(model);
        }
        //删除频道主推活动信息
        public int DeleteFeatureEventById(int id)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSubject_DeleteChannelFeaturedEventByID", new { ID = id });
        }
        #endregion
        public IList<SWfsSubjectLableType> GetSubjectLableTypeList(int type, string channelNo, string subjectNo)
        {
            IEnumerable<SWfsSubjectLableType> list = DapperUtil.Query<SWfsSubjectLableType>("ComBeziWfs_SWfsSubjectLableType_ReadBySubjectNoType", new { Type = type, SubjectNo = subjectNo, ChannelNo = channelNo });
            return list.ToList();
        }

        public int InsertSubjectLabelType(SWfsSubjectLableType labeltype)
        {
            return DapperUtil.Insert(labeltype, false);
        }

        public int DeleteSubjectLableType(int type, string channelNo, string subjectNo)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSubjectLableType_DeleteBySubjectNoType", new { Type = type, SubjectNo = subjectNo, ChannelNo = channelNo });
        }

        public SWfsChannel GetChannelByChannelNo(string channel)
        {
            return DapperUtil.Query<SWfsChannel>("ComBeziWfs_SWfsChannel_GetChannelByChannelNo", new { ChannelNo = channel }).FirstOrDefault();
        }

        public bool UpdateChannelHoliDay(SWfsChannel channel)
        {
            return DapperUtil.UpdatePartialColumns<SWfsChannel>(new { ChannelNo = channel.ChannelNo, HolidayMode = channel.HolidayMode });
        }

        #region 添加置顶
        /// <summary>
        /// 添加置顶
        /// </summary>
        /// <param name="topExpand"></param>
        /// <returns></returns>
        public int InsertSubjectTop(SWfsSubjectTopExpand topExpand)
        {
            return DapperUtil.Insert(topExpand);
        }
        /// <summary>
        ///根据主键查询
        /// </summary>
        /// <param name="SubjectNo"></param>
        /// <returns></returns>
        public SWfsSubjectTopExpand SelectSubjectTopBySubjectNo(string SubjectNo)
        {

            return DapperUtil.Query<SWfsSubjectTopExpand>("ComBeziWfs_SWfsSubjectTopExpand_GetSWfsSubjectTopExpandBySubjectNo", new { SubjectNo = SubjectNo }).FirstOrDefault();
        }
        /// <summary>
        /// 更新置顶扩展表
        /// </summary>
        /// <param name="topExpand"></param>
        /// <returns></returns>
        public bool EditSubjectTop(SWfsSubjectTopExpand topExpand)
        {
            return DapperUtil.Update(topExpand);
        }
        #endregion

        public List<SubjectBrand> GetSubjectBrandList(List<string> subjectNoList)
        {
            return DapperUtil.Query<SubjectBrand>("ComBeziWfs_SWfsSubjectBrand_GetSubjectBrandOutlet", new { SubjectNo = subjectNoList.ToArray() }).ToList();

        }

        /// <summary>
        /// 秒杀商品读取
        /// </summary>
        /// <returns></returns>
        public Dictionary<DateTime, List<SpikeProductManage>> GetSpikeProductList(string productNoName, string subjectNo, DateTime lookDate, int pageIndex, int pageSize, out int totalCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("ProductNoName", productNoName);
            dic.Add("SubjectNo", subjectNo);
            List<SpikeProductManage> spikeList = DapperUtil.Query<SpikeProductManage>("ComBeziWfs_SWfsSubjectSpikeProductManage_GetSpikeProductList", dic, new { ProductNoName = productNoName,SubjectNo=subjectNo, Date = lookDate }).ToList();
            //spikeList = spikeList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            Dictionary<DateTime, List<SpikeProductManage>> reslut = new Dictionary<DateTime, List<SpikeProductManage>>();
            if (spikeList != null && spikeList.Count > 0)
            {
                foreach (var item in spikeList)
                {
                    if (!reslut.ContainsKey(item.DateDay))
                    {
                        List<SpikeProductManage> tmpList = spikeList.Where(r => r.DateDay.Equals(item.DateDay)).ToList();
                        reslut.Add(item.DateDay, tmpList);
                    }
                }
            }
            reslut = reslut.OrderByDescending(m => m.Key).ToDictionary(m => m.Key, m => m.Value);
            totalCount = reslut.Keys.Count;

            Dictionary<DateTime, List<SpikeProductManage>> dataDic = new Dictionary<DateTime, List<SpikeProductManage>>();

            int indexNum = (pageIndex * pageSize) > totalCount ? totalCount : (pageIndex * pageSize);

            for (int i = (pageIndex - 1) * pageSize; i < indexNum; i++)
            {
                dataDic.Add(reslut.ElementAt(i).Key, reslut.ElementAt(i).Value);
            }

            //reslut = reslut.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToDictionary<DateTime, List<SpikeProductManage>>();

            return dataDic;
        }


        public void AddSWfsSubjectSpikeProduct(SWfsSubjectSpikeProductManage model)
        {
            DapperUtil.Insert<SWfsSubjectSpikeProductManage>(model);

        }

        public void UpdateSWfsSubjectSpikeProduct(SWfsSubjectSpikeProductManage model)
        {
            DapperUtil.Update<SWfsSubjectSpikeProductManage>(model);
        }

        public bool IsValidSpikeProduct(DateTime dateTime, int Id, out string msg)
        {
            msg = "";
            List<SWfsSubjectSpikeProductManage> list = DapperUtil.Query<SWfsSubjectSpikeProductManage>("ComBeziWfs_SWfsSubjectSpikeProductManage_IsValidSpikeProduct", new { ShowTime = dateTime }).ToList();
            var model = list.Where(r => r.ShowTime.Equals(dateTime)).FirstOrDefault();
            if (Id == 0) //添加时判断数量和时间是否存在
            {
                if (model != null) //存在一个相同时间段的商品，或则该天内添加的商品个数大于3个，均不能再添加了
                {
                    msg = "该时间段已有商品";
                    return true;
                }
                if (list.Count >= 3)
                {
                    msg = "每组添加的商品个数不能大于3个";
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //修改时，只判断时间
            {
                SWfsSubjectSpikeProductManage oldModel = GetSWfsSubjectSpikeProductManage(Id.ToString());

                if (oldModel == null)
                {
                    msg = "修改秒杀商品不存在";
                    return true;
                }

                if (oldModel.ShowTime.Year == dateTime.Year && oldModel.ShowTime.Month == dateTime.Month && oldModel.ShowTime.Day == dateTime.Day)
                {
                    if (model != null && !model.ID.Equals(Id)) //表示修改的日期组与已有的相同
                    {
                        msg = "该时间段已有商品";
                        return true;
                    }
                }
                else //修改到其他组
                {
                    if (list.Count >= 3)
                    {
                        msg = "每组添加的商品个数不能大于3个";
                        return true;
                    }
                }
            }
            
            //var t = DapperUtil.Query<int>("ComBeziWfs_SWfsSubjectSpikeProductManage_IsValidSpikeProduct", new { ShowTime = dateTime }).FirstOrDefault();
            //if (t > 0)
            //{
            //    return true; 
            //}
            return false;
        }

        public SWfsSubjectSpikeProductManage GetSWfsSubjectSpikeProductManage(string Id)
        {
            return DapperUtil.Query<SWfsSubjectSpikeProductManage>("ComBeziWfs_SWfsSubjectSpikeProductManage_GetModel", new { ID = Id }).FirstOrDefault();
        }



        public Dictionary<string, string> GetSubjectDicSPage(List<string> subjectNoList)
        {
            Dictionary<string, string> dic=new Dictionary<string,string>();
            if (subjectNoList != null && subjectNoList.Count() > 0)
            {
                List<SWfsSubjectTopExpand> List = DapperUtil.Query<SWfsSubjectTopExpand>("ComBeziWfs_SWfsSubjectTopExpand_GetSubjectDicSPage", new { SubjectNo = subjectNoList.ToArray() }).ToList();
                if (List != null && List.Count() > 0)
                {
                    SWfsSubjectTopExpand model = null;
                    foreach (var item in subjectNoList)
                    {
                        model = List.Where(r => r.SubjectNo.Equals(item)).FirstOrDefault();
                        if (model != null && !string.IsNullOrWhiteSpace(model.StExpand) && !dic.ContainsKey(item))
                        {
                            dic.Add(item, model.StExpand);
                        }
                        else
                        {
                            dic.Add(item, "0");
                        }
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 获得所有奥莱开启频道
        /// </summary>
        /// <returns></returns>
        public List<SWfsChannel> GetChannelAllList()
        {
            List<SWfsChannel> list = DapperUtil.Query<SWfsChannel>("ComBeziWfs_SWfsChannel_GetChannelList").ToList();
            return list;
        }

        
        #region 根据时间和商品名称获取秒杀商品信息_EP by zhangman 20141002
        /// <summary>
        /// 秒杀商品读取
        /// </summary>
        /// <returns></returns>
        public Dictionary<DateTime, List<SpikeProductManage>> SelectSpikeProductListByTimeAndName(string productNoName, string subjectNo, DateTime lookDate, int pageIndex, int pageSize, out int totalCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("ProductNoName", productNoName);
            dic.Add("SubjectNo", subjectNo);
            List<SpikeProductManage> spikeList = DapperUtil.Query<SpikeProductManage>("ComBeziWfs_SWfsSubjectSpikeProductManage_SelectSpikeProductListByTimeAndPName", dic, new { ProductNoName = productNoName, SubjectNo = subjectNo, Date = lookDate }).ToList();
            //spikeList = spikeList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            Dictionary<DateTime, List<SpikeProductManage>> reslut = new Dictionary<DateTime, List<SpikeProductManage>>();
            if (spikeList != null && spikeList.Count > 0)
            {
                foreach (var item in spikeList)
                {
                    if (!reslut.ContainsKey(item.DateDay))
                    {
                        List<SpikeProductManage> tmpList = spikeList.Where(r => r.DateDay.Equals(item.DateDay)).ToList();
                        reslut.Add(item.DateDay, tmpList);
                    }
                }
            }
            reslut = reslut.OrderByDescending(m => m.Key).ToDictionary(m => m.Key, m => m.Value);
            totalCount = reslut.Keys.Count;

            Dictionary<DateTime, List<SpikeProductManage>> dataDic = new Dictionary<DateTime, List<SpikeProductManage>>();

            int indexNum = (pageIndex * pageSize) > totalCount ? totalCount : (pageIndex * pageSize);

            for (int i = (pageIndex - 1) * pageSize; i < indexNum; i++)
            {
                dataDic.Add(reslut.ElementAt(i).Key, reslut.ElementAt(i).Value);
            }

            //reslut = reslut.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToDictionary<DateTime, List<SpikeProductManage>>();

            return dataDic;
        }
        #endregion 
    }
}
