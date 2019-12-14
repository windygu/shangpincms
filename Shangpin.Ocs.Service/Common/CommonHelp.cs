using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shangpin.Framework.Common;
using Shangpin.Entity.Common;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Ocs.Service.Common;
using System.Drawing;
using System.Web;
using System.IO;
using Shangpin.Ocs.Service.Outlet;
using System.Text.RegularExpressions;

namespace Shangpin.Ocs.Service
{
    /// <summary>
    /// 公共帮助类【仅适用于本解决方案内项目】
    /// </summary>
    public class CommonHelp
    { 

        #region 分页计算页数方法
        /// <summary>
        /// 分页计算页数方法
        /// </summary>
        /// <param name="PageSize">每页条数</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public static int GetPageCount(int PageSize, int recordCount)
        {

            int PageCount = recordCount % PageSize == 0 ? recordCount / PageSize : recordCount / PageSize + 1;

            if (PageCount < 1) PageCount = 1;

            return PageCount;

        }
        #endregion

        #region 导出excel数据

        /// <summary>
        /// 导出excel数据
        /// </summary>
        /// <param name="tableHtml"></param>
        /// <param name="title"></param>
        public static void OutputExcel(String tableHtml, String title)
        {
            string metaValue = "<meta http-equiv=\"content-type\" content=\"application/ms-excel; charset=UTF-8\"/>";
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.Charset = "utf-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            if (System.Web.HttpContext.Current.Request.UserAgent.ToLower().IndexOf("firefox") > -1)
            {
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + title + ".xls");

            }
            else
            {
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(title, System.Text.Encoding.UTF8) + ".xls");
            }
            System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";


            System.Web.HttpContext.Current.Response.Write(metaValue + tableHtml);

            System.Web.HttpContext.Current.Response.End();
        }
        #endregion  

        #region JS弹出窗口,可自已定义转到页面
        
        /// <summary>
        /// JS弹出窗口,可自已定义转到页面
        /// </summary>
        /// <param name="Msg">例：添加成功！</param>
        /// <param name="URL">例：News_list.aspx</param>
        public static void Alert(string Msg, string URL)
        {
            HttpContext.Current.Response.Write("<script>alert('" + Msg + "');window.location.href='" + URL + "'</script>");
            HttpContext.Current.Response.End();
        } 
        #endregion 
      
    } 
    #region//读取参数方法
    public sealed class Rq
    {

        #region 过滤字符参数等方法


        /// <summary>
        /// 安全的获取form值,如果不是整数，则返回0
        /// </summary>
        /// <param name="Key">form表单按钮的name</param>
        /// <returns>整数</returns>
        public static int GetIntForm(string Key)
        {
            int num = 0;
            try
            {
                num = Convert.ToInt32(HttpContext.Current.Request.Form[Key]);
            }
            catch
            {
            }
            return num;
        }
        #region 判断是否是数字,转换不成功时返回（直接传入参数名称） 0,也可转换不成功时给出提示,终止程序执行,弹出提示
        /// <summary>
        /// 判断是否是数字,转换不成功时返回（直接传入参数名称） 0,也可转换不成功时给出提示,终止程序执行,弹出提示
        /// </summary>
        /// <param name="para">传入的参数(直接传入参数名称即可，将优先表单接受值)</param>
        /// <returns>返回转换后的数字</returns>

        public static int GetIntQueryString_Form(string para)
        {
            string value = string.Empty;
            int number = 0;
            if (para.Trim().Length > 0)
            {
                if ("" + HttpContext.Current.Request.Form[para] != "" || "" + HttpContext.Current.Request.QueryString[para] != "")
                {
                    value = "" + HttpContext.Current.Request.Form[para] != "" ? HttpContext.Current.Request.Form[para].ToString().Trim() : HttpContext.Current.Request.QueryString[para].ToString().Trim();
                }
                if (Regex.IsMatch(value, @"^-?[0-9]+$"))
                {
                    number = int.Parse(value);
                }
            }
            return number;
        }
        #endregion
        /// <summary>
        /// 安全的获取url值,如果不是整数，则返回0
        /// </summary>
        /// <param name="Key">例：Rq.GetIntQueryString("url参数名");</param>
        /// <returns>url的值</returns>
        public static int GetIntQueryString(string Key)
        {
            int num = 0;
            try
            {
                num = Convert.ToInt32(HttpContext.Current.Request.QueryString[Key]);
            }
            catch
            {
                //
            }
            return num;
        }
        /// <summary>
        /// 安全的获取url值,如果不是整数，则返回0
        /// </summary>
        /// <param name="Key">例：Rq.GetIntQueryString("url参数名");</param>
        /// <returns>url的值</returns>
        public static Int64 GetInt64QueryString(string Key)
        {
            Int64 num = 0;
            try
            {
                num = Convert.ToInt64(HttpContext.Current.Request.QueryString[Key]);
            }
            catch
            {
                //
            }
            return num;
        } 
        /// <summary>
        /// 安全的获取form值
        /// </summary>
        /// <param name="Key"></param>
        /// <returns>字符串</returns>
        public static string GetStringForm(string Key)
        {
            string text = null;
            try
            {
                text = HttpContext.Current.Request.Form[Key].Trim();
            }
            catch
            {
            }
            return text;
        }
        /// <summary>
        /// 安全的获取url值
        /// </summary>
        /// <param name="Key">例：Rq.GetStringQueryString("url参数");</param>
        /// <returns>的值</returns>
        public static string GetStringQueryString(string Key)
        {
            string text = null;
            try
            {
                text = HttpContext.Current.Request.QueryString[Key];
            }
            catch
            {
                //
            }
            return text;
        }

        public static string RawUrl
        {
            get
            {
                return HttpContext.Current.Request.RawUrl;
            }
        }

        #endregion 

    }

    #endregion 
  
}
