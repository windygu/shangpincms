using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    /// <summary>
    /// 20131217导出活动商品订单到excel
    /// </summary>
    public class ORderToExcel
    {
        public string SubjectNo { get; set; }
        public string SubjectName { get; set; }

        public string OrderNo { get; set; }

        private string _status;
        public string status
        {
            get
            {
                switch (_status)
                {
                    case "0":
                        _status = "已取消";
                        break;
                    case "10":
                        _status = "等待客户确认";
                        break;
                    case "11":
                        _status = "待支付";
                        break;
                    case "12":
                        _status = "已确认";
                        break;
                    case "13":
                        _status = "收款已确认";
                        break;
                    case "14":
                        _status = "配货中";
                        break;
                    case "15":
                        _status = "已部分发货";
                        break;
                    case "16":
                        _status = "已全部发货";
                        break;
                    case "18":
                        _status = "COD收款已确认";
                        break;
                    case "97":
                        _status = "交易异常终止";
                        break;
                    case "98":
                        _status = "交易部分完成";
                        break;
                    case "99":
                        _status = "交易全部完成";
                        break;
                    default:
                        _status = "未知：" + _status;
                        break;
                }
                return _status;
            }

            set { _status = value; }
        }

        public string productNo { get; set; }
        public string ProductName { get; set; }
    }

}
