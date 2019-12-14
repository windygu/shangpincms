using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Service.ProductSort;
using Shangpin.Ocs.Entity.Extenstion.ShangPin.ProductFlat;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ProductFlat;
using System.Text;
using Shangpin.Entity.Wfs;


namespace Shangpin.Ocs.Service.Shangpin.ProductSort
{
    public class SWfsSortHistoryService
    {
        //根据分类ID查询 查询历史记录
        public IList<SWfsSortHistory> SelectHistory() 
        {
            return DapperUtil.Query<SWfsSortHistory>("ComBeziWfs_SWfsSortHistory_Select").ToList();
        }

        //添加查询历史记录
        public int InsertHistory(Parameters p, string url, string userID) 
        {
            //StringBuilder str = new StringBuilder();
            //if (p.productNO != null && p.productNO != "")
            //{
            //    str.Append(p.productNO+">");
            //}
            //if (p.productName != null && p.productName != "")
            //{
            //    str.Append(p.productName+">");
            //}
            //if (p.categoryNO != null && p.categoryNO != "")
            //{
            //    str.Append("&categoryNO=" + p.categoryNO);
            //}
            //if (p.brandNO != null && p.brandNO != "")
            //{
            //    str.Append("&brandNO=" + p.brandNO);
            //}
            //if (p.colorId != null && p.colorId != "")
            //{
            //    str.Append("&colorId=" + p.colorId);
            //}
            //if (p.shelfDate != null && p.shelfDate != "")
            //{
            //    str.Append("&shelfDate=" + p.shelfDate);
            //}
            //if (p.price != null && p.price != "")
            //{
            //    str.Append("&price=" + p.price);
            //}
            //if (p.stock != null && p.stock != "")
            //{
            //    str.Append("&stock=" + p.stock);
            //}
            //if (p.discountRate != null && p.discountRate != "")
            //{
            //    str.Append("&discountRate=" + p.discountRate);
            //}
            //if (p.start != null && p.start != "")
            //{
            //    str.Append("&start=" + p.start);
            //}
            SWfsSortHistory ssh = new SWfsSortHistory();
            SearchSortService sssDal=new SearchSortService();
            ssh.SearchUrl = url;
            ssh.Direction = sssDal.SelectDirection(p);
            ssh.CreateDate = System.DateTime.Now;
            ssh.UserId = userID;
            return DapperUtil.Insert<SWfsSortHistory>(ssh, true); //添加
        }

        //清除历史
        public int ClearHistory(string userId)
        {
            return DapperUtil.Execute("ComBeziWfs_SWfsSortHistory_DeleteSortHistoryByUserId", new 
            {
                UserId = userId
            });
        }

       
    }
}
