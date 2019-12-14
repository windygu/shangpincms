using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Shangpin.Ocs.Entity.Extenstion.Login;

namespace Shangpin.Ocs.Service.Common
{
    public class PresentationHelper
    {
        private static string cookieName = AppSettingManager.AppSettings["LoginCookieName"].ToString();
        // Methods
        public static void ClearCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public static void DeleteCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public static void SetLoginCookie(Passport passport)
        {
            string value = string.Format("IdentityId={0}&SessionId={1}&UserName={2}", HttpUtility.UrlEncode(passport.IdentityId), HttpUtility.UrlEncode(passport.SessionId), HttpUtility.UrlEncode(passport.UserName));
            SetCookie(cookieName, value, DateTime.Now.AddDays(1));
        }

        public static Passport GetPassport()
        {
            string value = GetCookie(cookieName);
            Passport passport = new Passport();
            if (!string.IsNullOrEmpty(value))
            {
                string[] cookieValue = value.Split(new char[]{'&'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in cookieValue)
                {
                    string[] strArray2 = str.Split(new char[] { '=' });
                    switch (strArray2[0])
                    {
                        case "SessionId":
                            passport.SessionId = HttpUtility.UrlDecode(strArray2[1]);
                            break;

                        case "IdentityId":
                            passport.IdentityId = HttpUtility.UrlDecode(strArray2[1]);
                            break;

                        case "UserName":
                            passport.UserName = HttpUtility.UrlDecode(strArray2[1]);
                            break;
                    }
                }
            }
            else
            {
                passport.IdentityId = "0";
                passport.SessionId = HttpContext.Current.Session.SessionID;
            }
            return passport;
        }



        public static void SetCookie(string cookieName, string cookieValue, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(cookieName, cookieValue);
            cookie.Expires = expires;
            cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static string GetCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                return string.Empty;
            }
            return cookie.Value;
        }


        public static void SetBrowserCookie(string cookieName, string cookieValue)
        {
            HttpCookie cookie = new HttpCookie(cookieName, cookieValue);
            cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}
