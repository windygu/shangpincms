using HomePageWindowsService.Methods;
using Shangpin.Framework.Common.Cache;
using Shangpin.Ocs.Service.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomePageWindowsService
{
    partial class RemoveCacheService : ServiceBase
    {
        public RemoveCacheService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                InitMemCache();//缓存配置
                int period = Convert.ToInt32(AppConfig.CheckRemoveCachePeriod()); //检测楼层模板切换  
                ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), RemoveCacheMethod, null, TimeSpan.FromMilliseconds(period), false); //首页楼层切换
            }
            catch (Exception e)
            {

            }
        }

        protected override void OnStop()
        {
        }
        private void RemoveCacheMethod(object obj, bool sign)
        {
            try
            {
                Thread.Sleep(30 * 1000);
                RemoveCacheMethod comm = new RemoveCacheMethod();
                comm.Run();
                comm = null;
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }
        private void InitMemCache()
        {
            MemcachedProvider.SetMemcachedCluster(AppSettingManager.AppSettings["memcacheServer"].Split(','));
        }

        private void WriteLog(string content)
        {
            var fs = new FileStream("d:/myServiceLog.txt", FileMode.OpenOrCreate);
            string strText = content;
            //获得字节数组
            byte[] data = new UTF8Encoding().GetBytes(strText);
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }
    }
}
