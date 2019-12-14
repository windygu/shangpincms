using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shangpin.Ocs.Service;
using System.Drawing;
using Shangpin.Ocs.Service.Common;
using Shangpin.Framework.WebMvc;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.Web.Caching;
using System.IO;
using System.Drawing.Imaging;

namespace Shangpin.Ocs.Web.ReadPic
{
    /// <summary>
    /// GetPicII 的摘要说明
    /// </summary>
    public class GetPicII : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int width = Convert.ToInt32(context.Request.QueryString["width"]);
            int height = Convert.ToInt32(context.Request.QueryString["height"]);
            string pictureFileNo = context.Request.QueryString["pictureFileNo"].ToString();
            string type = context.Request.QueryString["type"].ToString();

            CommonService service = new CommonService();
            string extension = string.Empty;
            string cacheKey = string.Format("PicRead_{0}_{1}_{2}_{3}", pictureFileNo, width, height, type);
            Image outImage;
            ImageModel imageModel = (ImageModel)HttpRuntime.Cache.Get(cacheKey);
            if (imageModel == null)
            {
                imageModel = service.GetPic(width, height, pictureFileNo, type);
                if (imageModel != null)
                {
                    HttpRuntime.Cache.Add(cacheKey, imageModel, null, DateTime.Now.AddMinutes(15), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
            }
            outImage = imageModel.Image;
            switch (imageModel.Extension)
            {
                case ".jpg":
                    context.Response.ContentType = "image/jpeg";
                    break;
                case ".gif":
                    context.Response.ContentType = "image/gif";
                    break;
                case ".png":
                    context.Response.ContentType = "image/png";
                    break;
                default:
                    context.Response.ContentType = "image/jpeg";
                    break;
            }
            context.Response.Clear();
            context.Response.BufferOutput = true;

            if (null == outImage)//输出默认图片--图片不存在时
            {
                Bitmap defalut = PictureFileConverter.GetDefalutPic(width, height);
                defalut.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                context.Response.End();
            }
            switch (imageModel.Extension)
            {
                case ".jpg":
                    outImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case ".gif":
                    outImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case ".png":

                    using (MemoryStream ms = new MemoryStream())
                    {

                        outImage.Save(ms, ImageFormat.Png);
                        ms.WriteTo(context.Response.OutputStream);
                    }
                    //outImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                default:
                    outImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
            }
            context.Response.End();
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