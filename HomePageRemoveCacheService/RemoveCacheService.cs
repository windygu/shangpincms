using HomePageRemoveCacheService.Methods;
using log4net;
using Shangpin.Framework.Common.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomePageRemoveCacheService
{
    public partial class RemoveCacheService : ServiceBase
    {
        private static ILog log = LogManager.GetLogger("RemoveCacheService_RollingLogFileAppender_Logger");
        public RemoveCacheService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Debug("启动服务");
            try
            {

                InitMemCache();//缓存配置
                int period = Convert.ToInt32(AppConfig.CheckRemoveCachePeriod()); //检测楼层清缓存间隔 
                period = 120 * 1000;
                ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), RemoveCacheMethod, null, TimeSpan.FromMilliseconds(period), false); //首页楼层切换
            }
            catch (Exception e)
            {
                log.Error("启动异常exception:" + e.Message);
            }
        }

        protected override void OnStop()
        {
            log.Debug("暂停服务");
        }
        private void RemoveCacheMethod(object obj, bool sign)
        {
            try
            {
                //Thread.Sleep(30 * 1000);
                RemoveCacheMethod comm = new RemoveCacheMethod();
                comm.Run();
                comm = null;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("首页清缓存服务异常，异常信息：{0}", ex.Message);
                //   WriteLog(ex.Message);
            }
        }
        private void InitMemCache()
        {

            //EnyimMemcachedClient.Instance.Remove("VenueAll_SWfsMeetingInfo_" + id);
            EnyimMemcachedClient.SetMemcachedCluster(ConfigurationManager.AppSettings["memcached"].Split(','));
        }

        //private void WriteLog(string content)
        //{
        //    var fs = new FileStream("d:/myServiceLog.txt", FileMode.OpenOrCreate);
        //    string strText = content;
        //    //获得字节数组
        //    byte[] data = new UTF8Encoding().GetBytes(strText);
        //    //开始写入
        //    fs.Write(data, 0, data.Length);
        //    //清空缓冲区、关闭流
        //    fs.Flush();
        //    fs.Close();
        //    fs.Dispose();
        //}
    }
}
