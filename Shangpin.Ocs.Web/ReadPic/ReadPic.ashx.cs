using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shangpin.Ocs.Service;
using System.Drawing;
using Shangpin.Ocs.Service.Common;

namespace Shangpin.Ocs.Web.ReadPic
{
    /// <summary>
    /// ReadPic 的摘要说明
    /// </summary>
    public class ReadPic : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int width = Convert.ToInt32(context.Request.QueryString["width"]);
            int height = Convert.ToInt32(context.Request.QueryString["height"]);
            string pictureFileNo = context.Request.QueryString["pictureFileNo"].ToString();
            string type = context.Request.QueryString["type"].ToString();



            //V2版 CDN图片
            //string picUrl = ServicePic.ResolveUGCImage(GetType(type), pictureFileNo, width, height);
            //context.Response.Redirect(picUrl, true);

            #region V1版读取--直接读取数据库中的二进制
            CommonService service = new CommonService();
            string extension = string.Empty;
            Image outImage = service.GetPic(width, height, pictureFileNo, type, out extension);
            context.Response.ContentType = "image/jpeg";
            context.Response.Clear();
            context.Response.BufferOutput = true;

            if (null == outImage)
            {
                //输出默认图片
                Bitmap defalut = PictureFileConverter.GetDefalutPic(width, height);
                defalut.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                context.Response.End();
            }
            switch (extension)
            {
                case "jpg":
                    outImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case "gif":
                    outImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case "png":
                    outImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                default:
                    outImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
            }

            context.Response.End();
            #endregion
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        
    }
 
}