using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.Text;
using Shangpin.Framework.Common;
using ImageServer.Client;
namespace Shangpin.Ocs.Web.Infrastructure
{
    public static class HtmHelpers
    {

        public static string AjaxPage(this HtmlHelper html, string javascriptFun, int currentPage, int pageSize, int totalCount)
        {
            int allpage = 0;
            int next = 0;
            int pre = 0;
            int startcount = 0;
            int endcount = 0;
            string pagestr = string.Empty;
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            //计算总页数
            allpage = ((totalCount % pageSize) == 0 ? totalCount / pageSize : (totalCount / pageSize) + 1);
            if (currentPage > allpage)
            {
                currentPage = allpage;
            }
            next = currentPage + 1;
            pre = currentPage - 1;
            startcount = (currentPage + 5) > allpage ? allpage - 9 : currentPage - 4;//中间页起始序号
            endcount = currentPage < 5 ? 10 : currentPage + 5;//中间页终止序号

            if (startcount < 1) { startcount = 1; } //为了避免输出的时候产生负数，设置如果小于1就从序号1开始
            if (allpage < endcount) { endcount = allpage; }//页码+5的可能性就会产生最终输出序号大于总页码，那么就要将其控制在页码数之内
            pagestr = "共" + allpage + "页&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            pagestr += currentPage > 1 ? "<a href='###' onclick='" + javascriptFun + "(1)'>首页</a>&nbsp;&nbsp;<a href='###' onclick='" + javascriptFun + "(" + pre + ")'>上一页</a>" : "首页 上一页";
            //中间页处理，这个增加时间复杂度，减小空间复杂度
            for (int i = startcount; i <= endcount; i++)
            {
                pagestr += currentPage == i ? "&nbsp;&nbsp;<font style=\"color:#600;font-weight:bold\" color=\"#ff0000\">" + i + "</font>" : "&nbsp;&nbsp;<a href='###' onclick='" + javascriptFun + "(" + i + ")'>" + i + "</a>";
            }
            pagestr += currentPage != allpage ? "&nbsp;&nbsp;<a href='###' onclick='" + javascriptFun + "(" + next + ")'>下一页</a>&nbsp;&nbsp;<a href='###' onclick='" + javascriptFun + "(" + allpage + ")'>末页</a>" : " 下一页 末页";

            return pagestr;
        }

        public static string PagerX(this HtmlHelper html, string currentPageStr, int currentPage, int pageSize, int totalCount, bool isLower = true)
        {
            return Pagination(html, currentPageStr, currentPage, pageSize, totalCount, "", isLower);
        }

        public static string GetPostionName(this HtmlHelper helper, int position)
        {
            return IndexAD.GetPositionName(position);
        }


        private static string Pagination(this HtmlHelper html, string currentPageStr, int currentPage, int pageSize, int totalCount, string pagetamp, bool isLower = true)
        {
            int allpage = 0;
            int next = 0;
            int pre = 0;
            int startcount = 0;
            int endcount = 0;
            string pagestr = "";
            string query_stringLower = string.Empty;
            string query_string = string.Empty;
            if (isLower)
            {
                #region 全部小写
                var querylist = html.ViewContext.HttpContext.Request.QueryString.ToString().ToLower();
                if (querylist.IndexOf("&pageindex") > -1)
                {
                    querylist = querylist.Substring(0, querylist.IndexOf("&pageindex"));
                }
                if (querylist.IndexOf("?pageindex") > -1)
                {
                    querylist = querylist.Substring(0, querylist.IndexOf("?pageindex"));
                }
                if (querylist.IndexOf("pageindex") > -1)
                {
                    querylist = querylist.Substring(0, querylist.IndexOf("pageindex"));
                }

                query_string = html.ViewContext.HttpContext.Request.RawUrl.ToLower();

                if (query_string.IndexOf("&pageindex") > -1)
                {
                    query_string = query_string.Substring(0, query_string.IndexOf("&pageindex"));
                }
                if (query_string.IndexOf("?pageindex") > -1)
                {
                    query_string = query_string.Substring(0, query_string.IndexOf("?pageindex"));
                }
                if (query_string.IndexOf("pageindex") > -1)
                {
                    query_string = query_string.Substring(0, query_string.IndexOf("pageindex"));
                }

                if (currentPage < 1) { currentPage = 1; }
                //计算总页数
                if (pageSize != 0)
                {
                    allpage = (totalCount / pageSize);
                    allpage = ((totalCount % pageSize) != 0 ? allpage + 1 : allpage);
                    allpage = (allpage == 0 ? 1 : allpage);
                }
                next = currentPage + 1;
                pre = currentPage - 1;
                startcount = (currentPage + 5) > allpage ? allpage - 9 : currentPage - 4;//中间页起始序号
                //中间页终止序号
                endcount = currentPage < 5 ? 10 : currentPage + 5;
                if (startcount < 1) { startcount = 1; } //为了避免输出的时候产生负数，设置如果小于1就从序号1开始
                if (allpage < endcount) { endcount = allpage; }//页码+5的可能性就会产生最终输出序号大于总页码，那么就要将其控制在页码数之内
                pagestr = "共" + allpage + "页&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                var joinstr = "?";
                if (!string.IsNullOrEmpty(querylist)) joinstr = "&";

                pagestr += currentPage > 1 ? "<a href=\"" + query_string + joinstr + "pageindex=1\">首页</a>&nbsp;&nbsp;<a href=\"" + query_string + joinstr + "pageindex=" + pre + "\">上一页</a>" : "首页 上一页";
                //中间页处理，这个增加时间复杂度，减小空间复杂度
                for (int i = startcount; i <= endcount; i++)
                {
                    pagestr += currentPage == i ? "&nbsp;&nbsp;<font style=\"color:#600;font-weight:bold\" color=\"#ff0000\">" + i + "</font>" : "&nbsp;&nbsp;<a href=\"" + query_string + joinstr + "pageindex=" + i + "\">" + i + "</a>";
                }
                pagestr += currentPage != allpage ? "&nbsp;&nbsp;<a href=\"" + query_string + joinstr + "pageindex=" + next + "\">下一页</a>&nbsp;&nbsp;<a href=\"" + query_string + joinstr + "pageindex=" + allpage + "\">末页</a>" : " 下一页 末页";

                return pagestr;


                #endregion
            }
            else
            {
                #region 不转换大小写
                var querylist = html.ViewContext.HttpContext.Request.QueryString.ToString();
                if (querylist.ToLower().IndexOf("&pageindex") > -1)
                {
                    querylist = querylist.Substring(0, querylist.ToLower().IndexOf("&pageindex"));
                }
                if (querylist.ToLower().IndexOf("?pageindex") > -1)
                {
                    querylist = querylist.Substring(0, querylist.ToLower().IndexOf("?pageindex"));
                }
                if (querylist.ToLower().IndexOf("pageindex") > -1)
                {
                    querylist = querylist.Substring(0, querylist.ToLower().IndexOf("pageindex"));
                }

                query_string = html.ViewContext.HttpContext.Request.RawUrl;

                if (query_string.ToLower().IndexOf("&pageindex") > -1)
                {
                    query_string = query_string.Substring(0, query_string.ToLower().IndexOf("&pageindex"));
                }
                if (query_string.IndexOf("?pageindex") > -1)
                {
                    query_string = query_string.Substring(0, query_string.ToLower().IndexOf("?pageindex"));
                }
                if (query_string.IndexOf("pageindex") > -1)
                {
                    query_string = query_string.Substring(0, query_string.ToLower().IndexOf("pageindex"));
                }

                if (currentPage < 1) { currentPage = 1; }
                //计算总页数
                if (pageSize != 0)
                {
                    allpage = (totalCount / pageSize);
                    allpage = ((totalCount % pageSize) != 0 ? allpage + 1 : allpage);
                    allpage = (allpage == 0 ? 1 : allpage);
                }
                next = currentPage + 1;
                pre = currentPage - 1;
                startcount = (currentPage + 5) > allpage ? allpage - 9 : currentPage - 4;//中间页起始序号
                //中间页终止序号
                endcount = currentPage < 5 ? 10 : currentPage + 5;
                if (startcount < 1) { startcount = 1; } //为了避免输出的时候产生负数，设置如果小于1就从序号1开始
                if (allpage < endcount) { endcount = allpage; }//页码+5的可能性就会产生最终输出序号大于总页码，那么就要将其控制在页码数之内
                pagestr = "共" + allpage + "页&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                var joinstr = "?";
                if (!string.IsNullOrEmpty(querylist)) joinstr = "&";

                pagestr += currentPage > 1 ? "<a href=\"" + query_string + joinstr + "pageindex=1\">首页</a>&nbsp;&nbsp;<a href=\"" + query_string + joinstr + "pageindex=" + pre + "\">上一页</a>" : "首页 上一页";
                //中间页处理，这个增加时间复杂度，减小空间复杂度
                for (int i = startcount; i <= endcount; i++)
                {
                    pagestr += currentPage == i ? "&nbsp;&nbsp;<font style=\"color:#600;font-weight:bold\" color=\"#ff0000\">" + i + "</font>" : "&nbsp;&nbsp;<a href=\"" + query_string + joinstr + "pageindex=" + i + "\">" + i + "</a>";
                }
                pagestr += currentPage != allpage ? "&nbsp;&nbsp;<a href=\"" + query_string + joinstr + "pageindex=" + next + "\">下一页</a>&nbsp;&nbsp;<a href=\"" + query_string + joinstr + "pageindex=" + allpage + "\">末页</a>" : " 下一页 末页";

                return pagestr;
                #endregion
            }
        }


        /// <summary>
        /// 活动首页显示折扣信息
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="discountType">折扣类型</param>
        /// <param name="discountTypeValue">折扣值</param>
        /// <returns></returns>
        public static string SubjectDiscountInfo(this HtmlHelper helper, short discountType, string discountTypeValue)
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(discountTypeValue))
            {
                discountTypeValue = "0";
            }
            if (discountType == 4 || discountType == 5)
            {
                if (discountType == 4)
                {
                    str.Append("<em><i>&yen;</i>" + Math.Round(Convert.ToDecimal(discountTypeValue), 0) + "</em>");
                }
                if (discountType == 5)//最低这个价位
                {
                    str.Append("<em><i>&yen;</i>" + Math.Round(Convert.ToDecimal(discountTypeValue), 0) + "</em>");
                    str.Append("<text>起售</text>");
                }
            }
            else
            {
                if (discountType == 1)
                {
                    str.Append("<em>" + Utility.PriceConvter(discountTypeValue) + "</em>");
                    str.Append("<text>折起售</text>");
                }
                if (discountType == 3)
                {
                    str.Append("<text>全场</text>");
                    str.Append("<em>" + Utility.PriceConvter(discountTypeValue) + "</em>");
                    str.Append("<text>折</text>");
                }
            }
            return str.ToString();
        }

        public static string GetLevel(this HtmlHelper helper, int level)
        {
            string rs = "A级";
            switch (level)
            {
                case 1:
                    rs = "A级";
                    break;

                case 2:
                    rs = "B级";
                    break;

                case 3:
                    rs = "C级";
                    break;

                case 4:
                    rs = "D级";
                    break;

                default:
                    rs = "A级";
                    break;
            }
            return rs;
        }

        public static string ConsoleSubjectUrl(this HtmlHelper helper, string url, SubjectMonitorSearchParm parm, int pageSize)
        {
            StringBuilder str = new StringBuilder(url + "?pageindex=1&pagesize={0}&SubjectNameNo={1}&BrandNo={2}&BrandName={3}&ChannelSord={4}&BU={5}");
            return string.Format(str.ToString(), pageSize, parm.SubjectNameNo, parm.BrandNo, parm.BrandName, parm.ChannelSord, parm.BU);

        }

        public static string GetPostion(this HtmlHelper helper, int position)
        {
            string str = "第一张";
            switch (position)
            {
                case 1:
                    str = "第一张";
                    break;
                case 2:
                    str = "第二张";
                    break;
                case 3:
                    str = "第三张";
                    break;
                case 4:
                    str = "第四张";
                    break;
                case 5:
                    str = "第五张";
                    break;
                case 6:
                    str = "第六张";
                    break;
                case 7:
                    str = "第七张";
                    break;
                case 8:
                    str = "第八张";
                    break;
                case 9:
                    str = "第九张";
                    break;
                case 10:
                    str = "第十张";
                    break;
            }
            return str;
        }
    }
}