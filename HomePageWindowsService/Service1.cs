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
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {  
                int period = Convert.ToInt32(AppConfig.AdShowTypeChangePeriod()); //检测楼层模板切换  
                ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), ChangeHomePageFloorAdShowType, null, TimeSpan.FromMilliseconds(period), false); //首页楼层切换
            }
            catch (Exception e)
            { 
            }
        }

        protected override void OnStop()
        {
        }
        private void ChangeHomePageFloorAdShowType(object obj, bool sign)
        {
            try
            { 
                HomePageService comm = new HomePageService();
                comm.Run();
                comm = null;
            }
            catch (Exception ex)
            { 
            }
        }

    }
}
