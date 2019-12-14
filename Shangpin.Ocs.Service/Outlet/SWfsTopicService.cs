using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using Shangpin.Entity.Common;

namespace Shangpin.Ocs.Service.Outlet
{
    public class SWfsTopicService
    {
        public IList<SWfsTopics> GetTopicList(string titleOrNo, string headTitle, string status,string gender, int pageIndex, int pageSize,out int readCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>(); 

            dic.Add("titleOrNo", string.IsNullOrEmpty(titleOrNo) ? "" : titleOrNo);//却分是名称还是编号
            dic.Add("htitle", string.IsNullOrEmpty(headTitle)? "":headTitle);
            dic.Add("status", string.IsNullOrEmpty(status) ? "" : status);
            dic.Add("gender", string.IsNullOrEmpty(gender) ? "" : gender);
            IList<SWfsTopics> list = DapperUtil.Query<SWfsTopics>("ComBeziWfs_SWfsTopics_SelectTopic", dic, new { TopicNameOrNo = titleOrNo, SubHeading = headTitle, Status = status, Gender = gender }).ToList();
            readCount = list.Count;
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;
        }

        
        public int Edit(string topicId, string action, string status = "1")
        {
            SWfsTopics t = new SWfsTopics();
       
            int rs = 0;
            switch (action)
            {
                case "del"://删除

                    rs=DapperUtil.Execute("ComBeziWfs_SWfsTopics_DeleteTopicByTopicNo", (new { TopicNo = topicId }));

                    break;
                case "switch"://开启和关闭
                    rs = DapperUtil.UpdatePartialColumns<SWfsTopics>(new { TopicNo = topicId, status = status }) ? 1 : 0;

                    break;

                case "top": //置顶和取消
                    rs = DapperUtil.UpdatePartialColumns<SWfsTopics>(new { TopicNo = topicId, IsTop = (status=="1")?true:false, DateTop = DateTime.Now }) ? 1 : 0;
                    break;
            }
            return rs;
              
        }

        public SWfsTopics GetSWfsTopics(string topicId)
        {
            SWfsTopics model = DapperUtil.QueryByIdentity<SWfsTopics>(topicId);
            return model;
        }

        public void AddSWfsTopics(SWfsTopics model)
        {
             DapperUtil.Insert<SWfsTopics>(model);
        }

        public bool UpateSWfsTopics(SWfsTopics model)
        {
            return DapperUtil.Update<SWfsTopics>(model);
        }

        public RecordPage<ProductInfo> GetProductList(IDictionary<string,object> dic, int pageIndex, int pageSize)
        {

            IList<ProductInfo> productList = DapperUtil.QueryPaging<ProductInfo>("ComBeziWfs_WfsProduct_SelectTopicProductList", pageIndex, pageSize, "ProductNo desc", dic, new { gCategroyNo = dic["gCategroyNo"].ToString(), brandNo = dic["brandNo"].ToString(), productNoOrName = dic["productNoOrName"].ToString(), gender = dic["gender"].ToString()}).ToList();

            return PageConvertor.Convert(pageIndex, pageSize, productList); 
        }

        public List<SWfsTopicProductRef> GetSWfsTopicProductList(string productNoes, string topicNo)
        {
            List<SWfsTopicProductRef> rs = DapperUtil.Query<SWfsTopicProductRef>("ComBeziWfs_SWfsTopicProductRef_SelectTopicProductPref", new { ProductNo = productNoes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), TopicNo = topicNo }).ToList();
            return rs;
        }

        public void AddProductNo(SWfsTopicProductRef model)
        {
            DapperUtil.Insert<SWfsTopicProductRef>(model);
        }

        public RecordPage<ProductInfo> GetProductListByTopicNo(IDictionary<string, object> dic, int pageIndex, int pageSize)
        {
            IList<ProductInfo> productList = DapperUtil.QueryPaging<ProductInfo>("ComBeziWfs_SWfsTopicProductRef_SelectTopicProductByTopicNo", pageIndex, pageSize, "SWfsTopicProductRef.OrderFlag asc", dic, new { gCategroyNo = dic["gCategroyNo"].ToString(), brandNo = dic["brandNo"].ToString(), TopicNo = dic["topicId"].ToString(), productNoOrName = dic["productNoOrName"].ToString() }).ToList();

            return PageConvertor.Convert(pageIndex, pageSize, productList); 
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="topicNo"></param>
        /// <param name="productNoes"></param>
        /// <returns></returns>
        public int DelTopicProduct(string topicNo, string productNoes)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsTopicProductRef_DelTopicProductByTopicNo", new { TopicNo = topicNo, ProductNo = productNoes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) });
        }

        /// <summary>
        /// 单个更新排序
        /// </summary>
        /// <param name="orderFlag"></param>
        /// <param name="topicNo"></param>
        /// <param name="productNo"></param>
        public void UpdateTopicProductOrderFlag(string topicProductNo, string orderFlag)
        {
            DapperUtil.UpdatePartialColumns<SWfsTopicProductRef>(new { TopicProductNo = topicProductNo, OrderFlag = orderFlag});
        }
    }
}
