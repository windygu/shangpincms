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
    partial class ClearStatisticsDataService : ServiceBase
    {
        private Timer _timer;

        public ClearStatisticsDataService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            int period = Convert.ToInt32(AppConfig.ClearPeriod());
            _timer = new Timer(Watch, null, 10000, period);
        }

        protected override void OnStop()
        {
            if (_timer != null) _timer.Dispose();
        }

        private void Watch(object obj)
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
    }
}
