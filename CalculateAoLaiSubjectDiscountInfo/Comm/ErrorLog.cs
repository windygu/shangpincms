using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateAoLaiSubjectDiscountInfo.Comm
{
    /// <summary>
    /// 功能：错误日志类，将错误信息按指定事件日志名记录在系统日志
    /// </summary>
    public static class ErrorLog
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="sourceName">日志资源名，如：Town</param>
        /// <param name="message">错误信息</param>
        public static void Log(string sourceName, string message)
        {
            EventLog eventLog = null;

            // 确定日志是否存在
            if (!(EventLog.SourceExists(sourceName)))
            {
                EventLog.CreateEventSource(sourceName, sourceName + "Log");
            }

            if (eventLog == null)
            {
                eventLog = new EventLog(sourceName + "Log");
                eventLog.Source = sourceName;
            }

            // 记录日志信息
            eventLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Error);
        }
    }
}
