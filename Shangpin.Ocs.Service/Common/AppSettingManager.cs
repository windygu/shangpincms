using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Shangpin.Framework.Common;
using System.Xml.Linq;
using System.Configuration;

namespace Shangpin.Ocs.Service.Common
{
    /// <summary>
    /// 读取Appsetting,实时读取！
    /// </summary>
    public static class AppSettingManager
    {
        private static NameValueCollection _nameCollection = new NameValueCollection();
        static AppSettingManager()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            path = string.Format("{0}ConfigFileCollection/App/AppSetting.config", CommonHelper.GetParentPath(path, 2));

            XDocument doc = XDocument.Load(path);

            var q2 = doc.Descendants("appSettings").Count();
            if (q2 != 1)
                throw new Exception(path + " reapeat config element item [appsettings]");

            var q =
                doc.Descendants("add").Select(
                    x =>
                    {
                        var xAttribute = x.Attribute("key");
                        var attribute = x.Attribute("value");
                        if (attribute != null)
                            return xAttribute != null ? new { Key = xAttribute.Value, Value = attribute.Value } : null;
                        return null;
                    });
            q.ToList().ForEach(x => _nameCollection.Add(x.Key, x.Value));

        }
        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static NameValueCollection AppSettings
        {
            get { return _nameCollection; }
        }

    }


    /// <summary>
    /// 读取ResetPwdEmailGotoUrl,实时读取！
    /// </summary>
    public static class ResetPwdEmailManager
    {
        private static NameValueCollection _nameCollection = new NameValueCollection();
        static ResetPwdEmailManager()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            path = string.Format("{0}ConfigFileCollection/App/ResetPwdEmailGotoUrl.config", CommonHelper.GetParentPath(path, 2));

            XDocument doc = XDocument.Load(path);

            var q2 = doc.Descendants("appSettings").Count();
            if (q2 != 1)
                throw new ConfigurationErrorsException(path + " reapeat config element item [appsettings]");

            var q =
                doc.Descendants("add").Select(
                    x =>
                    {
                        var xAttribute = x.Attribute("key");
                        var attribute = x.Attribute("value");
                        if (attribute != null)
                            return xAttribute != null ? new { Key = xAttribute.Value, Value = attribute.Value } : null;
                        return null;
                    });
            q.ToList().ForEach(x => _nameCollection.Add(x.Key, x.Value));

        }
        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static NameValueCollection ResetPwdEmailSettings
        {
            get { return _nameCollection; }
        }

    }
}

