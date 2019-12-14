using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Ocs.Service.Common;
using Shangpin.Entity.Wfs;
using Shangpin.Framework.Common;
using Shangpin.Ocs.Entity.Extenstion.Login;
using System.Web;
using Shangpin.Entity.Common;
using Com.Shang.SMSService;


namespace Shangpin.Ocs.Service.Login
{
   public class LoginService
    {
       string RandomNumber = "";
       public OcsServiceResult Authenticate(string userName, string passWord, string remberUser="")
       {
           OcsServiceResult result = new OcsServiceResult();
           Dictionary<string, object> content = new Dictionary<string, object>();

           if (string.IsNullOrEmpty(userName))
           {
               result.IsSuccess = false;
               content.Add("Msg", "用户名错误");
               content.Add("Flag", "1");
               result.ContentDic = content;
               return result;
           }
           if (string.IsNullOrEmpty(passWord))
           {
               result.IsSuccess = false;
               content.Add("Msg", "密码错误");
               content.Add("Flag", "2");
               result.ContentDic = content;
               return result;
           }
           string strB = StringUtil.ToHashString(passWord);
           WfsOperator model = DapperUtil.Query<WfsOperator>("ComBeziWfs_WfsOperator_Authenticate", new { UserName = userName }).FirstOrDefault();
           if (null == model)
           {
               result.IsSuccess = false;
               content.Add("Msg", "用户名错误");
               content.Add("Flag", "1");
               result.ContentDic = content;
               return result;
           }
           if (model.Password.CompareTo(strB) != 0)
           {
               result.IsSuccess = false;
               content.Add("Msg", "密码错误");
               content.Add("Flag", "2");
               result.ContentDic = content;
               return result;
           }
           if (model.IsValid == 0)
           {
               result.IsSuccess = false;
               content.Add("Msg", "此用户已禁用");
               content.Add("Flag", "1");
               result.ContentDic = content;
               return result;
           }
           #region 随机数
           System.Random random = new Random();
           for (int i = 0; i < 4; i++)
           {
               RandomNumber += random.Next(9).ToString();
           }
           #endregion
           //是否需要手机验证和IP验证后期补充--目前只实现了线上的手机号码验证功能
           if (AppSettingManager.AppSettings["MobileValidateFlag"].Equals("1"))
           {
               //尚未完成--待处理
               //int perIp = int.Parse(AppSettingManager.AppSettings["SMSLimitIpCount"] ?? "50");//每个IP限制数量
               //int perUserId = int.Parse(AppSettingManager.AppSettings["SMSLimitUserCount"] ?? "10");//每个用户限制数量
               //int perTarget = int.Parse(AppSettingManager.AppSettings["SMSLimitTargetCount"] ?? "10");//每个目标手机限制数量
               //int perContent = int.Parse(AppSettingManager.AppSettings["SMSLimitContentCount"] ?? "10");//每个发送内容限制数量 
               //int perMinute = int.Parse(AppSettingManager.AppSettings["SMSLimitMinuteCount"] ?? "10");//每分钟限制数量 暂无
               //string logFile = AppSettingManager.AppSettings["SmsSendLogPath"];//日志文件路径（不含文件）
               //string smsTcpServer = AppSettingManager.AppSettings["SmsTcpServer"];
               //string smsAuthorizationCode = AppSettingManager.AppSettings["SmsAuthorizationCode"];
               //Com.Shang.SMSService.GlobalConfig.Initial(perIp, perUserId, perTarget, perContent, perMinute, logFile, smsTcpServer, smsAuthorizationCode);
               HttpContext.Current.Session["suiji"] = RandomNumber;
               //var msg = SmsServiceUtil.SendSms(userName, model.Mobile, RandomNumber);
           }

           //登录用户的权限---待权限规划后赋予

           #region //登录后存COOKIE
           if (remberUser.Trim().Equals("1"))
           {
               PresentationHelper.SetCookie("RemberOCSUser", userName, DateTime.Now.AddDays(7));
           }
           else
           {
               PresentationHelper.DeleteCookie("RemberOCSUser");
           }
           Passport passport = new Passport();
           passport.IdentityId = HttpContext.Current.Session.SessionID;
           passport.SessionId = HttpContext.Current.Session.SessionID;
           passport.UserName = model.UserName;
           PresentationHelper.SetLoginCookie(passport);
           #endregion

           #region //登录日志记录
           SystemLoginLog sysLoginModel = new SystemLoginLog();
           sysLoginModel.UserID = userName;
           sysLoginModel.VerifyCode=string.Empty;
           sysLoginModel.LogStatus=1;
           sysLoginModel.SysFlag=3;
           sysLoginModel.LogTime=DateTime.Now;
           sysLoginModel.LogIP=Utility.GetUserIpAddress();
           sysLoginModel.ProvinceName = string.Empty;
           sysLoginModel.CityName = string.Empty;
           sysLoginModel.Memo = (0 == 0) ? "未经过手机验证" : "手机验证时Ip被排除";//0:1手机验证的值
           //DapperUtil.Insert<SystemLoginLog>(sysLoginModel);
           #endregion

           result.IsSuccess = true;
           return result;
       }

       public void LoginOut()
       {
           PresentationHelper.ClearCookie();
       }
   
   }
}
