using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Entity.Common;
using Shangpin.Ocs.Entity;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Service.Shangpin
{
    public class OrderService
    {
        public IList<WfsOrderBankFQPay> GetWfsOrderBankFQPay(string shangPinOrderID, string createTime, int pageIndex, int pageSize)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("OrderID", shangPinOrderID == null ? "" : shangPinOrderID);
            dic.Add("CreateDate", createTime == null ? "" : createTime);

            IList<WfsOrderBankFQPay> bankFQList = DapperUtil.Query<WfsOrderBankFQPay>("ComBeziWfs_WfsOrderBankFQPay_SelectOrderBankFQList", dic, new { OrderID = shangPinOrderID, CreateDate = createTime, pageIndex = pageIndex, pageSize = pageSize }).ToList();

            return bankFQList;
            //return PageConvertor.Convert(pageIndex, pageSize, bankFQList);
        }

        public int GetWfsOrderBankFQPayCount(string shangPinOrderID, string createTime)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("OrderNo", shangPinOrderID == null ? "" : shangPinOrderID);
            dic.Add("PayDate", createTime == null ? "" : createTime);

            int count = DapperUtil.Query<int>("ComBeziWfs_WfsOrderBankFQPay_SelectOrderBankFQListCount", dic, new { OrderNo = shangPinOrderID, PayDate = createTime }).FirstOrDefault();

            return count;
        }

        #region 招行分期订单列表 by zhangwei 20131105
        /// <summary>
        /// 获取招分期支付成功订单列表
        /// </summary>
        /// <param name="orderNo">尚品订单编号</param>
        /// <param name="payDate">支付成功日期</param>
        /// <param name="endPayDate">结束支付日期</param>
        /// <param name="isCount">是否统计总记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页码</param>
        /// <param name="readCount">输出总记录数</param>
        /// <returns></returns>
        public IList<WfsBankFQPayM> GetBankFQPayList(string orderNo, string payDate, string endPayDate, bool isCount, int pageIndex, int pageSize, out int readCount)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("OrderNo", string.IsNullOrEmpty(orderNo) ? "" : orderNo);
            dic.Add("PayDate", string.IsNullOrEmpty(payDate) ? "" : payDate);
            dic.Add("EndPayDate", string.IsNullOrEmpty(endPayDate) ? "" : endPayDate);
            dic.Add("TopNum", 0); //0查询所有记录，1统计总记录数
            object obj = new { OrderNo = orderNo, PayDate = payDate, EndPayDate = endPayDate, pageIndex = pageIndex, pageSize = pageSize };
            IList<WfsBankFQPayM> list = DapperUtil.QueryPaging<WfsBankFQPayM>("ComBeziWfs_WfsOrderBankFQPay_SelectOrderBankFQListNew", pageIndex, pageSize, "PayDate DESC", dic, obj).ToList();
            //IList<WfsBankFQPayM> list = DapperUtil.Query<WfsBankFQPayM>("ComBeziWfs_WfsOrderBankFQPay_SelectOrderBankFQListNew", dic, obj).ToList();
            if (!isCount)
            {
                readCount = 0;
            }
            else
            {
                readCount = GetRecordCount(dic, obj, "ComBeziWfs_WfsOrderBankFQPay_SelectOrderBankFQListNew"); //获取总记录数
            }
            return list;
        }//GetBankFQPayList

        /// <summary>
        /// 获取总记录数 by zhangwei 20131105
        /// <param name="dic">参数</param>
        /// <param name="obj">字段</param>
        /// <param name="statement">调用sql</param>
        /// </summary>
        /// <returns></returns>
        public int GetRecordCount(Dictionary<string, object> dic, object obj, string statement)
        {
            dic["TopNum"] = "1"; //0查询记录，1统计记录
            var list = DapperUtil.Query<int>(statement, dic, obj);
            if (list != null)
            {
                return list.First();
            }
            return 0;
        }//GetRecordCount
        #endregion
    }
}
