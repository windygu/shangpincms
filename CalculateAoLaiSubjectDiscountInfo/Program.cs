using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CalculateAoLaiSubjectDiscountInfo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {

            log4net.Config.XmlConfigurator.Configure();
            //CalculateAoLaiSubjectDiscountInfo.BLL.CalculationBLL.Run();
            //Console.ReadLine();

            // 同一进程中可以运行多个用户服务。若要将
            // 另一个服务添加到此进程中，请更改下行以
            // 创建另一个服务对象。例如，


            if (args.Length == 0)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new Calculation() };
                ServiceBase.Run(ServicesToRun);
            }
            // 安装服务
            else if (args[0].ToLower() == "/i" || args[0].ToLower() == "-i")
            {
                try
                {
                    string[] cmdline = { };
                    string serviceFileName = System.Reflection.Assembly.GetExecutingAssembly().Location;

                    TransactedInstaller transactedInstaller = new TransactedInstaller();
                    AssemblyInstaller assemblyInstaller = new AssemblyInstaller(serviceFileName, cmdline);
                    transactedInstaller.Installers.Add(assemblyInstaller);
                    transactedInstaller.Install(new System.Collections.Hashtable());
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }
            // 删除服务
            else if (args[0].ToLower() == "/u" || args[0].ToLower() == "-u")
            {
                try
                {
                    string[] cmdline = { };
                    string serviceFileName = System.Reflection.Assembly.GetExecutingAssembly().Location;

                    TransactedInstaller transactedInstaller = new TransactedInstaller();
                    AssemblyInstaller assemblyInstaller = new AssemblyInstaller(serviceFileName, cmdline);
                    transactedInstaller.Installers.Add(assemblyInstaller);
                    transactedInstaller.Uninstall(null);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }

        }



    }
}
