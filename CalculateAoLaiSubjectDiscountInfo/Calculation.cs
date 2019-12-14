using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalculateAoLaiSubjectDiscountInfo.BLL;
using CalculateAoLaiSubjectDiscountInfo.Comm;

namespace CalculateAoLaiSubjectDiscountInfo
{
    partial class Calculation : ServiceBase
    {
        public Calculation()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO:  在此处添加代码以启动服务。 
            int milliseconds = Convert.ToInt32(ConfigurationManager.AppSettings["TimeSpan"].ToString());
            System.Timers.Timer myTimer = new System.Timers.Timer(milliseconds);
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            myTimer.Interval = milliseconds;
            myTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            // TODO:  在此处添加代码以执行停止服务所需的关闭操作。
        }


        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                CalculationBLL.Run();
            }
            catch (Exception ex)
            {
                ErrorLog.Log("Calculation", ex.ToString());
            }
        }
    }
}
