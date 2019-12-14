using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shangpin.Ocs.Service;
using System.Drawing;
using Shangpin.Ocs.Service.Common;
using Shangpin.Framework.WebMvc;

namespace Shangpin.Ocs.Web.ReadPic
{
    /// <summary>
    /// 读取图片 
    /// </summary>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <param name="pictureFileNo">图片ID</param>
    /// <param name="type">1 产品图片 2系统图片 3用户图片</param>
    /// <returns></returns>
    public class GetPic : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int width = Convert.ToInt32(context.Request.QueryString["width"]);
            int height = Convert.ToInt32(context.Request.QueryString["height"]);
            string pictureFileNo = context.Request.QueryString["pictureFileNo"].ToString();
            string type = context.Request.QueryString["type"].ToString();



            //V2版 走CDN图片---此方法不能正常显示PNG GIF等带透明度的图片格式
            //GetPicII.ashx方法支持PNG GIF等格式，但是不走CND
            string picUrl = ServicePic.ResolveUGCImage(GetType(type), pictureFileNo, width, height);
            context.Response.Redirect(picUrl, true);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }  
        }

        private string GetType(string type)
        {
            string t = "1";
            switch (type)
            {
                case "1":
                    t = PictureType.ProductPicture.ToString();
                    break;

                case "2":
                    t = PictureType.SystemPicture.ToString();
                    break;

                case "3":
                    t = PictureType.UserPicture.ToString();
                    break;
            }
            return t;
        }
    }
    public enum PictureType
    {
        /// <summary>
        /// 系统图片
        /// </summary>
        /// Author:wangtao
        /// Date:2012/7/9
        SystemPicture,
        /// <summary>
        /// 商品图片
        /// </summary>
        /// Author:wangtao
        /// Date:2012/7/9
        ProductPicture,
        /// <summary>
        /// 用户图片
        /// </summary>
        /// Author : zhaoxiaojun
        /// Date : 2012/11/6
        UserPicture
    }
}