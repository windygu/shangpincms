using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shangpin.Ocs.Service.Outlet;
using Shangpin.Entity.Wfs;
using Shangpin.Ocs.Entity.Extenstion.Outlet;
using System.IO;
using Shangpin.Entity.ComBeziPic;
using Shangpin.Framework.Common;
using System.Configuration;
using Shangpin.Ocs.Service;
using System.Text;
using Shangpin.Entity.Common;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq.Expressions;
using Shangpin.Framework.Common.Cache;
using System.Text.RegularExpressions;
using ImageServer.Client;
using Shangpin.Entity.Item;
using System.Collections;
using Shangpin.Framework.Configuration;
namespace Shangpin.Ocs.Web.Areas.Outlet.Controllers
{
    public partial class SubjectController : Controller
    {
        public readonly string ImageSites = ShangPinConfigManager.ShangPinConfig.ImageSites;

        #region 保存编辑器上传图片

        /// <summary>
        /// 保存编辑器上传图片
        /// </summary>
        /// <returns></returns>
        public ActionResult EditorSaveUploadPic()
        {
            //文件保存目录URL
            string saveUrl = string.Empty;

            //图片类型
            string imgType = AppSettingManager.AppSettings["editoryImgType"].ToString();

            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable();
            extTable.Add("image", imgType);

            //最大文件大小
            int maxSize = 0;
            string tempFileSize = AppSettingManager.AppSettings["editorImgMaxSize"];
            if (string.IsNullOrWhiteSpace(tempFileSize))
            {
                maxSize = 500 * 1024;
            }
            else
            {
                maxSize = Convert.ToInt32(tempFileSize) * 1024;
            }

            var imgFile = Request.Files["imgFile"];
            if (imgFile == null)
            {
                showError("请选择文件!");
            }
            String dirName = Request.QueryString["dir"];
            if (String.IsNullOrEmpty(dirName))
            {
                dirName = "image";
            }
            if (!extTable.ContainsKey(dirName))
            {
                showError("目录名不正确!");
            }

            String fileName = imgFile.FileName;
            String fileExt = Path.GetExtension(fileName).ToLower();

            if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
            {
                return Json(new { result = 1, message = "上传文件大小超过限制!" }, "text/html", Encoding.UTF8, JsonRequestBehavior.AllowGet); //showError("上传文件大小超过限制!");
            }
            if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dirName]).Split('/'), fileExt.ToLower()) == -1)
            {
                showError("上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dirName]) + "格式。");
            }

            string imgerror = string.Empty;

            //保存图片返回名称
            string editorImgName = GetActiveImageNameN("imgFile", "editorImg", false, "编辑器图片", "Create", "", false, imgType, out imgerror);

            //根据名称获取全路径
            string imgUrl = GetImageWebUrl(PictureType.SystemPicture.ToString(), editorImgName, 0, 0, fileExt.Replace(".", ""));

            if (!string.IsNullOrEmpty(imgerror))
            {
                showError(imgerror);
            }
            else if (string.IsNullOrEmpty(editorImgName))
            {
                return Json(new { result = 1, message = "返回图片名称失败！" }, "text/html", Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                saveUrl = imgUrl;
            }

            return Json(new { error = 0, url = saveUrl }, "text/html", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 批量上传图片

        #region 获取图片全路径

        /// <summary>
        /// 获取图片路径
        /// </summary>
        /// <param name="pictureType">SystemPicture系统图片，ProductPicture商品图片，UserPicture用户图片</param>
        /// <param name="pictureFileNo">图片文件名称编号</param>
        /// <param name="width">宽 0为原图宽</param>
        /// <param name="height">高 0为原图高</param>
        /// <param name="extension">扩展</param>
        /// <returns></returns>
        public string GetImageWebUrl(string pictureType, string pictureFileNo, int width, int height, string extension)
        {
            if (string.IsNullOrEmpty(pictureType) || string.IsNullOrEmpty(pictureFileNo))
            {
                return string.Empty;
            }

            string site = GetUGCImageSiteUrl(pictureFileNo);
            if (!site.EndsWith("/"))
            {
                site = (site + "/");
            }

            ImageType imageType = ImageType.SystemPictureFile;

            if (pictureType.Equals("ProductPicture", StringComparison.CurrentCultureIgnoreCase))
                imageType = ImageType.ProductPicture;

            if (pictureType.Equals("SystemPicture", StringComparison.CurrentCultureIgnoreCase))
                imageType = ImageType.SystemPictureFile;

            if (pictureType.Equals("UserPicture", StringComparison.CurrentCultureIgnoreCase))
                imageType = ImageType.UserPicture;

            string rurl = ImageUrlService.GetStaticImageUrl(imageType, pictureFileNo, (!string.IsNullOrEmpty(extension) ? extension : "jpg"), width, height);

            string imageUrl = string.Format("{0}{1}", site, rurl.StartsWith("/") ? rurl.Substring(1) : rurl);


            return imageUrl;

        }
        #endregion

        #region 根据图片文件名称编号获取图片路径
        /// <summary>
        /// 图片路径
        /// </summary>
        /// <param name="pictureFileNo">图片文件名称编号</param>
        /// <returns></returns>
        public string GetUGCImageSiteUrl(string pictureFileNo)
        {
            string[] sites = ImageSites.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (sites.Length == 0)
                return string.Empty;
            if (sites.Length == 1)
                return ImageSites;

            string endNo = pictureFileNo.Substring(pictureFileNo.Length - 1, 1);
            int index = 0;
            int.TryParse(endNo, out index);
            int cc = pictureFileNo.GetHashCode();

            return sites[Math.Abs(cc % sites.Length)];
        }
        #endregion

        #endregion

        #region 错误提示语
        /// <summary>
        /// 错误提示语
        /// </summary>
        /// <param name="message"></param>
        private ActionResult showError(string message)
        {
            return Json(new { result = 1, message = message }, "text/html", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 图片上传返回名称
        /// <summary>
        /// 判断图片 返回图片名称
        /// </summary>
        /// <param name="FormName">接受参数值web页面标签Name</param>
        /// <param name="ConfigName">对应配置文件中配置</param>
        /// <param name="ImageConfig">宽高是否固定</param>
        /// <param name="Dir">错误描述信息</param>
        /// <param name="SubjectManage">是否是编辑</param>
        /// <param name="EditMsg">编辑中对应字段值</param>
        /// <param name="FlagEmpty">图片是否可空</param>
        /// <param name="imgType">图片类型</param>
        /// <param name="imgerror">返回错误信息</param>
        /// <returns></returns>
        public string GetActiveImageNameN(string FormName, string ConfigName, bool ImageConfig, string Dir, string SubjectManage, string EditMsg, bool FlagEmpty, string imgType, out string imgerror)
        {
            imgerror = string.Empty;
            string ImageName = ImgNameGetN(FormName, ConfigName, ImageConfig, imgType, out imgerror);
            if (string.IsNullOrEmpty(ImageName))
            {
                if (!string.IsNullOrEmpty(imgerror) && imgerror != "未添加图片")
                {
                    imgerror = string.Format("{0}错误，{1}", Dir, imgerror);
                }
                else if (SubjectManage == "Edit" && !string.IsNullOrEmpty(EditMsg))
                {
                    imgerror = string.Empty;
                    ImageName = EditMsg;
                }
                else
                {
                    if (FlagEmpty)
                    {
                        imgerror = string.Empty;
                        ImageName = EditMsg;
                    }
                    else
                    {
                        imgerror = string.Format("{0}错误，{1}", Dir, imgerror);
                    }
                }
            }
            return ImageName;
        }


        /// <summary>
        /// 上传图片显示图片名称
        /// </summary>
        /// <param name="ImgName">上传图片file</param>
        /// <param name="CongifName">文件容量大小</param>
        /// <param name="flag">true图片是固定大小 flase图片大小在区间之内</param>
        /// <param name="error"></param>
        /// <returns></returns>
        private string ImgNameGetN(string ImgName, string CongifName, bool flag, out string error)
        {
            error = string.Empty;
            if (Request.Files[ImgName] == null || Request.Files[ImgName].ContentLength == 0)
            {
                error = "未添加图片";
                return string.Empty;
            }
            Dictionary<string, string> belongsSubjectPic = new CommonService().PostImg(Request.Files[ImgName], AppSettingManager.AppSettings[CongifName].ToString(), flag);
            string belongsSubjectPicNo = belongsSubjectPic.Values.FirstOrDefault();
            string belongsSubjectPicNokey = belongsSubjectPic.Keys.FirstOrDefault();
            if (!string.IsNullOrEmpty(belongsSubjectPicNo))
            {
                if (belongsSubjectPicNokey != "error")
                {
                    return belongsSubjectPicNo;
                }
                else
                {
                    error = belongsSubjectPicNo;
                    return string.Empty;
                }
            }
            else
            {
                error = "图片上传失败";
                return string.Empty;
            }
        }

        /// <summary>
        /// 上传图片显示图片名称
        /// </summary>
        /// <param name="ImgName">上传图片file</param>
        /// <param name="CongifName">文件容量大小</param>
        /// <param name="flag">true宽大于等于，高等于 flase图片尺寸大小不限制</param>
        /// <param name="imgType">图片类型</param>
        /// <param name="error">返回错误信息</param>
        /// <returns></returns>
        private string ImgNameGetN(string ImgName, string CongifName, bool flag, string imgType, out string error)
        {
            error = string.Empty;
            if (Request.Files[ImgName] == null || Request.Files[ImgName].ContentLength == 0)
            {
                error = "未添加图片";
                return string.Empty;
            }
            Dictionary<string, string> belongsSubjectPic = new CommonService().PostImg(Request.Files[ImgName], AppSettingManager.AppSettings[CongifName].ToString(), imgType, (flag ? 1 : 0));
            string belongsSubjectPicNo = belongsSubjectPic.Values.FirstOrDefault();
            string belongsSubjectPicNokey = belongsSubjectPic.Keys.FirstOrDefault();
            if (!string.IsNullOrEmpty(belongsSubjectPicNo))
            {
                if (belongsSubjectPicNokey != "error")
                {
                    return belongsSubjectPicNo;
                }
                else
                {
                    error = belongsSubjectPicNo;
                    return string.Empty;
                }
            }
            else
            {
                error = "图片上传失败";
                return string.Empty;
            }
        }
        #endregion

    }
}
