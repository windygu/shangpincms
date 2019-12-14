using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Threading;
namespace SubjectStatisticsDataWindowsService
{
    public partial class Service1 : ServiceBase
    {

        //private Timer _timer;
        //private Timer _timer1;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            int period = Convert.ToInt32(AppConfig.Period());
            int ClearPeriod = Convert.ToInt32(AppConfig.ClearPeriod());

            //_timer = new Timer(CreateSubjectStatisticsDataThread, null, 10000, period); //生成活动统计数据
            //_timer1 = new Timer(ClearSubjectStatisticsData, null, 10000, ClearPeriod); //清除2个月前活动统计数据

            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), CreateSubjectStatisticsDataThread, null, TimeSpan.FromMilliseconds(period), false); //生成活动统计数据
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), ClearSubjectStatisticsData, null, TimeSpan.FromMilliseconds(ClearPeriod), false); //清除2个月前活动统计数据

            #region 线程

            //ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), CreateSubjectStatisticsDataThread, null, TimeSpan.FromMilliseconds(period), false); //生成活动统计数据

            //ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), ClearSubjectStatisticsData, null, TimeSpan.FromMilliseconds(period), false); //清除2个月前活动统计数据

            //Console.WriteLine("时间:{0} 工作线程请注意，您需要等待5s才能执行。\n", DateTime.Now);

            //今日新开
            //ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), (state, bTimeout) => CreateSubjectStatisticsDataThread(1), "S", TimeSpan.FromSeconds(period), false);

            //进行中
            //ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), (state, bTimeout) => CreateSubjectStatisticsDataThread(2), "S", TimeSpan.FromSeconds(period), false);

            //已结束
            //ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), (state, bTimeout) => CreateSubjectStatisticsDataThread(3), "S", TimeSpan.FromSeconds(period), false);

            //已结束
            //ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), (state, bTimeout) => CreateSubjectStatisticsDataThread(3), "S", TimeSpan.FromSeconds(period), false);

            //清除奥莱活动2个月之前的统计数据
            //ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), (state, bTimeout) => ClearSubjectStatisticsData(), "S", TimeSpan.FromSeconds(ClearPeriodLen), false);
            #endregion

        }

        protected override void OnStop()
        {
            //if (_timer != null) _timer.Dispose();
        }

        /// <summary>
        /// 生成奥莱活动统计数据
        /// </summary>
        private void CreateSubjectStatisticsDataThread(object obj, bool sign)
        {
            try
            {
                SWfsSubjectStatisticsService comm = new SWfsSubjectStatisticsService();
                comm.Run();
                comm = null;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 清除奥莱活动2个月之前的统计数据
        /// </summary>
        private void ClearSubjectStatisticsData(object obj, bool sign)
        {
            try
            {
                SWfsSubjectStatisticsService comm = new SWfsSubjectStatisticsService();
                comm.ClearDataRun();
                comm = null;
            }
            catch (Exception ex)
            {

            }
        }

        private void Run(object obj, bool sign)
        {
            SWfsSubjectStatisticsService service = new SWfsSubjectStatisticsService();
            string RunConfig = AppConfig.RunConfig();
            service.GetSubjectDataList(2);
            Console.WriteLine("全部统计数据成功加入缓存");
        }

    }
}
