using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Service.Common;
using Shangpin.Ocs.Service.Shangpin;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using Shangpin.Ocs.Entity.Extenstion.ShangPin;

namespace Shangpin.Ocs.Web.Areas.Shangpin.Controllers
{
    //[OCSAuthorization]
    public class OrderController : Controller 
    {
        /// <summary>
        /// 尚品订单号跟招行订单号对应关系 查询
        /// </summary>
        /// <param name="shangPinOrderID">尚品订单号</param>
        /// <param name="createTime">开始支付时间</param>
        /// <param name="endTime">结束支付时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public ActionResult CMBOrderIDQuery(string shangPinOrderID, string createTime, string endTime, int pageIndex = 1)
        {
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            int recordCount = 0;
            ViewBag.CurPage = pageIndex;
            ViewBag.PageSize = pageSize;

            OrderService bankFQService = new OrderService();
            //IList<WfsOrderBankFQPay> list = bankFQService.GetWfsOrderBankFQPay(shangPinOrderID, createTime, pageIndex, pageSize);
            IList<WfsBankFQPayM> list = bankFQService.GetBankFQPayList(shangPinOrderID, createTime, endTime, true, pageIndex, pageSize, out recordCount);
            if (list != null && list.Count() > 0)
            {
                foreach (WfsBankFQPayM item in list)
                {
                    item.CMBOrderNo = GetXMLNodesValue(item.RetureData, "CrdBllNbr");
                }
            }
            ViewBag.Count = recordCount;
            ViewBag.List = list;
            ViewBag.ShangPinOrderId = shangPinOrderID;
            ViewBag.Time = createTime;
            ViewBag.EndTime = endTime;
            return View();
        }

        //
        // GET: /Shangpin/Order/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Shangpin/Order/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Shangpin/Order/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Shangpin/Order/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Shangpin/Order/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Shangpin/Order/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Shangpin/Order/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Shangpin/Order/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// 导出招行分期订单列表 by zhangwei 2013-11-01
        /// </summary>
        /// <param name="shangPinOrderID">尚品订单号</param>
        /// <param name="createTime">创建时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OutInputCMBOrderDataList(string shangPinOrderID, string createTime, string endPayDate, int pageIndex = 1)
        {
            bool IsSuccess = false;
            int recordCount = 0;
            int pageCount = 0;
            int pageSize = int.Parse(AppSettingManager.AppSettings["ComonListPageNum"].ToString());
            string ExcleSavePath = AppSettingManager.AppSettings["ExcleSavePath"].ToString();
            string ExcelFileWeb = AppSettingManager.AppSettings["ExcelFileWeb"].ToString();
            OrderService bankFQService = new OrderService();
            IList<WfsBankFQPayM> list = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("尚品订单号");
            dt.Columns.Add("招行订单号");
            dt.Columns.Add("招行内部订单号");
            dt.Columns.Add("分期类型");
            dt.Columns.Add("支付金额");
            dt.Columns.Add("支付时间");
            #region 统计总记录
            bankFQService.GetBankFQPayList(shangPinOrderID, createTime, endPayDate, true, 1, 1, out recordCount);
            //recordCount = bankFQService.GetWfsOrderBankFQPayCount(shangPinOrderID, createTime);
            pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(recordCount) / Convert.ToDouble(pageSize)));
            #endregion
            if (pageCount > 0)
            {
                for (int i = 0; i < pageCount; i++)
                {
                    list = bankFQService.GetBankFQPayList(shangPinOrderID, createTime, endPayDate, false, (i + 1), pageSize, out recordCount);
                    if (list != null && list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            dt.Rows.Add(item.ShangOrderNo,
                                item.BankFQOrderNo,
                                GetXMLNodesValue(item.RetureData, "CrdBllNbr"),
                                (item.Period + "期"),
                                item.AmountSend,
                                item.PayDate.ToString("yyyy-MM-dd HH:mm:ss")
                                );
                        }
                    }
                }
                CreateExecl(dt, ExcleSavePath + "\\招行分期订单列表.xls");
                IsSuccess = true;
            }
            return Json(new { IsSuccess = IsSuccess, Url = ExcelFileWeb + "/招行分期订单列表.xls" });
        }//OutInputCMBOrderDataList

        /// <summary>
        /// 获取XML节点值
        /// </summary>
        /// <param name="strNotice">xml字符串</param>
        /// <param name="node">节点名称</param>
        /// <returns></returns>
        private string GetXMLNodesValue(string strNotice, string node)
        {
            string CMBOrderNo = string.Empty;
            if (!string.IsNullOrEmpty(strNotice) && !string.IsNullOrEmpty(node))
            {
                string xmlData = strNotice.Replace("$", "");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);
                XmlNode rootNode = xmlDoc.SelectSingleNode("CDPNotice_Pay");
                XmlNode bodyNode = rootNode.SelectSingleNode("Body");
                CMBOrderNo = bodyNode.SelectSingleNode(node).InnerText.ToString();
            }
            return CMBOrderNo;
        }//GetXMLNodesValue

        /// <summary>
        /// 生成Excel
        /// </summary>
        /// <param name="dtable">数据集</param>
        /// <param name="name">保存路径+文件名称</param>
        private void CreateExecl(DataTable dtable, string name)
        {
            if (string.IsNullOrEmpty(name) || name.IndexOf('\\') < 0)
            {
                throw new Exception("文件路径错误！");
            }
            if (!Directory.Exists(name.Substring(0, name.LastIndexOf('\\'))))
            {
                Directory.CreateDirectory(name.Substring(0, name.LastIndexOf('\\')));
            }

            FileInfo FInfo = new FileInfo(name);
            try
            {
                StringBuilder str = new StringBuilder();
                StreamWriter sw = new StreamWriter(name, false, System.Text.Encoding.GetEncoding("gb2312"));
                DataTable dt = new DataTable();
                dt.Columns.Add("序号", typeof(System.String));
                dt.Merge(dtable);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    str.Append(dt.Columns[i].ColumnName.ToString());
                    if (i < (dt.Columns.Count - 1))
                        str.Append("\t");
                }
                sw.WriteLine(str.ToString());
                int n = 0;

                StringBuilder txt = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    n++;
                    for (int j = 0; j < dr.Table.Columns.Count; j++)
                    {
                        if (j > 0)
                            txt.Append(dr[j] + "\t");
                    }
                    sw.WriteLine(n + "\t" + txt.ToString());

                    txt.Remove(0, txt.Length);
                }
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }//CreateExecl

    }
}
