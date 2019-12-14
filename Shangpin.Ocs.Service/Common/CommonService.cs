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
using Shangpin.Ocs.Entity.Extenstion.Outlet;

namespace Shangpin.Ocs.Service
{
    public class CommonService
    {
        #region 图片保存 自定义类型 -hbq 20140917
        public Dictionary<string, string> SaveImgDefaultType(HttpPostedFileBase file, string imgInfo)
        {
            Dictionary<string, string> rsPic = new Dictionary<string, string>();
            CommonService commonService = new CommonService();
            if (file.ContentLength > 0)
            {
                rsPic = PostImgCustomType(file, imgInfo, new string[] { ".jpg", ".jpeg", ".gif" }, 2);
            }
            return rsPic;
        }
        /// <summary>
        /// 通用保存自定义类型图片 isArea   0 宽大于等于  1 宽大于等于，高等于  2严格宽高
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="imgSize">width:640,Height:0,Length:50</param>
        /// <returns></returns>
        public Dictionary<string, string> PostImgCustomType(HttpPostedFileBase postedFile, string imgSize, string[] allowTypes, int isArea = 0)
        {
            #region 方案二
            //string types = AppSettingManager.AppSettings["InputSystemImgType"];
            //string[] allowTypes = string.IsNullOrEmpty(types) ? (new string[] { ".jpg", ".gif" }) : (types.Split('/'));
            Dictionary<string, string> result = new Dictionary<string, string>();
            string message = string.Empty;
            if (postedFile != null && !string.IsNullOrEmpty(postedFile.FileName))
            {
                string fileFullName = postedFile.FileName;
                string fileType = Path.GetExtension(fileFullName).ToLower();
                string fileName = Path.GetFileName(fileFullName);

                if (allowTypes.Contains(fileType))
                {
                    int conLen = postedFile.ContentLength;
                    //读取文件为 二进制流 , 保存到  图片表 , 返回 图片编号
                    byte[] btImgs = new byte[conLen];
                    using (Stream s = postedFile.InputStream)
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        s.Position = 0;
                        s.Read(btImgs, 0, conLen);
                    }
                    System.IO.Stream stream = new System.IO.MemoryStream(btImgs);//将byte数组回归成流
                    System.Drawing.Image original_image = System.Drawing.Image.FromStream(stream);//使用流创建图片
                    int width = original_image.Width;//图片的宽度
                    int height = original_image.Height;//图片的高度
                    original_image.Dispose();//释放资源
                    long length = stream.Length / 1024;//图片的大小（KB）
                    stream.Close();
                    stream.Dispose();
                    //string pictureSize = AppSettingManager.AppSettings[imgSize];
                    int img_Width = Convert.ToInt32(imgSize.Split(',')[0].Split(':')[1].ToString());
                    int img_Heighth = Convert.ToInt32(imgSize.Split(',')[1].Split(':')[1].ToString());
                    int img_Length = Convert.ToInt32(imgSize.Split(',')[2].Split(':')[1].ToString());
                    if (isArea == 1)//宽大于等于，高等于
                    {
                        if (width < img_Width || height != img_Heighth)
                        {
                            message = "请上传宽大于等于" + img_Width.ToString() + "高等于" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    else if (isArea == 2)  //严格宽高
                    {
                        //如果Length0 则表示不限制此信息······
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        //如果宽0 高0 则表示不限制此信息······
                        if ((width != img_Width) || (height != img_Heighth))
                        {
                            message = "请上传" + img_Width.ToString() + "*" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    else if (isArea == 0)
                    {
                        //如果Length0 则表示不限制此信息······
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        //如果宽0 高0 则表示不限制此信息······
                        if ((img_Width > 0 && width != img_Width) || (img_Heighth > 0 && height != img_Heighth))
                        {
                            message = "请上传" + img_Width.ToString() + "*" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    SystemPictureFile model = new SystemPictureFile();
                    model.FileContent = Convert.ToBase64String(btImgs);
                    model.Extension = fileType;
                    model.FileName = fileName;
                    model.OperatorId = string.Empty;
                    model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
                    SWfsSubjectService service = new SWfsSubjectService();
                    string pictureNo = service.InserSystemImg(model);

                    result.Add("success", pictureNo);
                    return result;
                }
                else
                {
                    result.Add("error", "上传图片格式不正确！");
                    return result;
                }
            }
            string typestr = allowTypes.Aggregate((a, b) => a + "," + b);
            result.Add("error", "请上传尺寸为" + imgSize + "，格式为" + typestr + "的图片");
            result.Add("error", "请上传图片！");
            return result;
            #endregion
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="picNo"></param>
        /// <returns></returns>
        public int DeleteImg(string picNo)
        {
            return DapperUtil.Execute("ComBeziPic_SystemPictureFile_DeleteByPicNo", new { PictureFileNo = picNo });
        }
        #endregion
        public int GetNextCounterId(string sequenceKey)
        {
            int i = 1;
            SequenceCounter model = DapperUtil.Query<SequenceCounter>("ComBeziCommon_SequenceCounter_Read", new { SequenceKey = sequenceKey }).FirstOrDefault();
            if (model == null)
            {
                SequenceCounter sc = new SequenceCounter();
                sc.SequenceKey = sequenceKey;
                sc.CounterId = i;
                DapperUtil.Insert<SequenceCounter>(sc, false);//true 返回插入数据的主键，false则不返回
            }
            else
            {
                i = model.CounterId;
                model.CounterId = i + 1;
                model.SequenceKey = sequenceKey;
                DapperUtil.Update(model);
            }
            return i;
        }

        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="pictureFileNo">图片ID</param>
        /// <param name="type">1 产品图片 2系统图片 3用户图片</param>
        /// <returns></returns>
        public Image GetPic(int width, int height, string pictureFileNo, string type, out string extension)
        {
            //如果图片不存在的错误----
            string filecontent = string.Empty;
            string picExtension = string.Empty;
            try
            {
                switch (type)
                {
                    case "1":
                        ProductPictureFile model = DapperUtil.QueryByIdentityWithNoLock<ProductPictureFile>(pictureFileNo);
                        filecontent = model.FileContent;
                        picExtension = model.Extension;
                        break;


                    case "2":
                        SystemPictureFile model2 = DapperUtil.QueryByIdentityWithNoLock<SystemPictureFile>(pictureFileNo);
                        filecontent = model2.FileContent;
                        picExtension = model2.Extension;
                        break;


                    case "3":
                        UserPictureFile model3 = DapperUtil.QueryByIdentityWithNoLock<UserPictureFile>(pictureFileNo);
                        filecontent = model3.FileContent;
                        picExtension = model3.Extension;
                        break;
                }
            }
            catch (Exception)
            {
                extension = picExtension;
                return null;
            }

            Image outImage = PictureFileConverter.GetThumbnailImage(filecontent, width, height);
            extension = picExtension;
            return outImage;
        }

        public ImageModel GetPic(int width, int height, string pictureFileNo, string type)
        {

            //如果图片不存在的错误----
            string filecontent = string.Empty;
            string picExtension = string.Empty;
            try
            {
                switch (type)
                {
                    case "1":
                        ProductPictureFile model = DapperUtil.QueryByIdentityWithNoLock<ProductPictureFile>(pictureFileNo);
                        filecontent = model.FileContent;
                        picExtension = model.Extension;
                        break;


                    case "2":
                        SystemPictureFile model2 = DapperUtil.QueryByIdentityWithNoLock<SystemPictureFile>(pictureFileNo);
                        filecontent = model2.FileContent;
                        picExtension = model2.Extension;
                        break;


                    case "3":
                        UserPictureFile model3 = DapperUtil.QueryByIdentityWithNoLock<UserPictureFile>(pictureFileNo);
                        filecontent = model3.FileContent;
                        picExtension = model3.Extension;
                        break;
                }
            }
            catch (Exception)
            {
                return new ImageModel();
            }
            Image outImage = PictureFileConverter.GetThumbnailImage(filecontent, width, height);
            ImageModel imgModel = new ImageModel
            {
                Extension = picExtension,
                Image = outImage
            };
            return imgModel;
        }


        /// <summary>
        /// 通用保存图片
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="imgSize">width:640,Height:0,Length:50</param>
        /// <returns></returns>
        public Dictionary<string, string> PostImg(HttpPostedFileBase postedFile, string imgSize, int isArea = 0)
        {
            #region 方案二
            string types = AppSettingManager.AppSettings["InputSystemImgType"];
            string[] allowTypes = string.IsNullOrEmpty(types) ? (new string[] { ".jpg", ".gif" }) : (types.Split('/'));
            Dictionary<string, string> result = new Dictionary<string, string>();
            string message = string.Empty;
            if (postedFile != null && !string.IsNullOrEmpty(postedFile.FileName))
            {
                string fileFullName = postedFile.FileName;
                string fileType = Path.GetExtension(fileFullName).ToLower();
                string fileName = Path.GetFileName(fileFullName);

                if (allowTypes.Contains(fileType))
                {
                    int conLen = postedFile.ContentLength;
                    //读取文件为 二进制流 , 保存到  图片表 , 返回 图片编号
                    byte[] btImgs = new byte[conLen];
                    using (Stream s = postedFile.InputStream)
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        s.Position = 0;
                        s.Read(btImgs, 0, conLen);
                    }
                    System.IO.Stream stream = new System.IO.MemoryStream(btImgs);//将byte数组回归成流
                    System.Drawing.Image original_image = System.Drawing.Image.FromStream(stream);//使用流创建图片
                    int width = original_image.Width;//图片的宽度
                    int height = original_image.Height;//图片的高度
                    original_image.Dispose();//释放资源
                    long length = stream.Length / 1024;//图片的大小（KB）
                    stream.Close();
                    stream.Dispose();



                    //string pictureSize = AppSettingManager.AppSettings[imgSize];
                    int img_Width = Convert.ToInt32(imgSize.Split(',')[0].Split(':')[1].ToString());
                    int img_Heighth = Convert.ToInt32(imgSize.Split(',')[1].Split(':')[1].ToString());
                    int img_Length = Convert.ToInt32(imgSize.Split(',')[2].Split(':')[1].ToString());
                    if (isArea == 1)//宽大于等于，高等于
                    {
                        if (width < img_Width || height != img_Heighth)
                        {
                            message = "请上传宽大于等于" + img_Width.ToString() + "高等于" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    else if (isArea == 0)
                    {
                        //如果Length0 则表示不限制此信息······
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        //如果宽0 高0 则表示不限制此信息······
                        if ((img_Width > 0 && width != img_Width) || (img_Heighth > 0 && height != img_Heighth))
                        {
                            message = "请上传" + img_Width.ToString() + "*" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    SystemPictureFile model = new SystemPictureFile();
                    model.FileContent = Convert.ToBase64String(btImgs);
                    model.Extension = fileType;
                    model.FileName = fileName;
                    model.OperatorId = string.Empty;
                    model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
                    SWfsSubjectService service = new SWfsSubjectService();
                    string pictureNo = service.InserSystemImg(model);

                    result.Add("success", pictureNo);
                    return result;
                }
                else
                {
                    result.Add("error", "上传图片格式不正确！");
                    return result;
                }
            }
            result.Add("error", "请上传尺寸为" + imgSize + "，格式为" + types + "的图片");
            return result;
            #endregion
        }


        #region 通用保存图片 重构
        /*因为20130913商品活动需求 重构方法 添加最大值 最小值
         例width:640|640 split[0]最小值 split[0]最大值 如果都为0 不限制 如果相等固定图片大小         
         */
        /// <summary>
        /// 通用保存图片
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="imgSize">width:640|640,Height:0|0,Length:50|50</param>
        /// <param name="flag">true图片是固定大小 flase图片大小在区间之内</param>
        /// <returns></returns>
        public Dictionary<string, string> PostImg(HttpPostedFileBase postedFile, string imgSize, bool flag)
        {
            if (flag)
            {
                return PostImg(postedFile, imgSize);
            }
            string types = AppSettingManager.AppSettings["InputSystemImgType"];
            string[] allowTypes = string.IsNullOrEmpty(types) ? (new string[] { ".jpg", ".gif", "png" }) : (types.Split('/'));
            Dictionary<string, string> result = new Dictionary<string, string>();
            string message = string.Empty;
            if (postedFile != null && !string.IsNullOrEmpty(postedFile.FileName))
            {
                string fileFullName = postedFile.FileName;
                string fileType = Path.GetExtension(fileFullName).ToLower();
                string fileName = Path.GetFileName(fileFullName);

                if (allowTypes.Contains(fileType))
                {
                    int conLen = postedFile.ContentLength;
                    byte[] btImgs = new byte[conLen];
                    using (Stream s = postedFile.InputStream)
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        s.Position = 0;
                        s.Read(btImgs, 0, conLen);
                    }
                    System.IO.Stream stream = new System.IO.MemoryStream(btImgs);//将byte数组回归成流
                    System.Drawing.Image original_image = System.Drawing.Image.FromStream(stream);//使用流创建图片
                    int width = original_image.Width;//图片的宽度
                    int height = original_image.Height;//图片的高度
                    original_image.Dispose();//释放资源
                    long length = stream.Length / 1024;//图片的大小（KB）
                    stream.Close();
                    stream.Dispose();

                    #region 判断
                    /*因为20130913商品活动需求 重构方法 添加最大值 最小值
                     例width:640|640,Height:0|0,Length:50|50
                     * width:640|640 split[0]最小值 split[0]最大值 如果都为0 不限制 如果相等固定图片大小         
                     */
                    string getWidth = imgSize.Split(',')[0].Split(':')[1].ToString();
                    string getHeighth = imgSize.Split(',')[1].Split(':')[1].ToString();
                    string getLength = imgSize.Split(',')[2].Split(':')[1].ToString();

                    //宽度区间
                    int imgMinWidth, imgMaxWidth;
                    if (getWidth.Contains('|'))
                    {
                        imgMinWidth = Convert.ToInt32(getWidth.Split('|')[0]);
                        imgMaxWidth = Convert.ToInt32(getWidth.Split('|')[1]);
                    }
                    else
                    {
                        imgMinWidth = Convert.ToInt32(getWidth);
                        imgMaxWidth = Convert.ToInt32(getWidth);
                    }
                    //高度区间
                    int imgMinHeight, imgMaxHeight;
                    if (getHeighth.Contains('|'))
                    {
                        imgMinHeight = Convert.ToInt32(getHeighth.Split('|')[0]);
                        imgMaxHeight = Convert.ToInt32(getHeighth.Split('|')[1]);
                    }
                    else
                    {
                        imgMinHeight = Convert.ToInt32(getHeighth);
                        imgMaxHeight = Convert.ToInt32(getHeighth);
                    }
                    //大小区间
                    int imgMinLength, imgMaxLength;
                    if (getLength.Contains('|'))
                    {
                        imgMinLength = Convert.ToInt32(getLength.Split('|')[0]);
                        imgMaxLength = Convert.ToInt32(getLength.Split('|')[1]);
                    }
                    else
                    {
                        imgMinLength = Convert.ToInt32(getLength);
                        imgMaxLength = Convert.ToInt32(getLength);
                    }


                    if (!(imgMinWidth == imgMaxWidth && imgMinWidth == 0))
                    {
                        imgMaxWidth = imgMaxWidth == 0 ? int.MaxValue : imgMaxWidth;
                        if (width < imgMinWidth || width > imgMaxWidth)
                        {
                            if (imgMaxWidth == 0 || imgMaxWidth == int.MaxValue)
                            {
                                message = "请上传宽度大于" + imgMinWidth + "的图片！！";
                            }
                            else
                            {
                                message = "请上传宽度为" + imgMinWidth + "-" + imgMaxWidth + "的图片！！";
                            }
                            result.Add("error", message);
                            return result;
                        }
                    }

                    if (!(imgMinHeight == imgMaxHeight && imgMaxHeight == 0))
                    {
                        imgMaxHeight = imgMaxHeight == 0 ? int.MaxValue : imgMaxHeight;
                        if (imgMaxHeight != height)
                        {
                            message = "请上传高度为" + imgMinHeight + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        if (height < imgMinHeight || height > imgMaxHeight)
                        {
                            message = "请上传高度为" + imgMinHeight + "-" + imgMaxHeight + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    if (!(imgMinLength == imgMaxLength && imgMaxLength == 0))
                    {
                        if (length > imgMaxLength)
                        {
                            message = "请上传大小为" + imgMaxLength + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    #endregion

                    SystemPictureFile model = new SystemPictureFile();
                    model.FileContent = Convert.ToBase64String(btImgs);
                    model.Extension = fileType;
                    model.FileName = fileName;
                    model.OperatorId = string.Empty;
                    model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
                    SWfsSubjectService service = new SWfsSubjectService();
                    string pictureNo = service.InserSystemImg(model);
                    result.Add("success", pictureNo);
                    return result;
                }
                else
                {
                    result.Add("error", "请上传.jpg或.gif格式的图片！");
                    return result;
                }
            }
            result.Add("error", "请上传尺寸为" + imgSize + "，格式为" + types + "的图片");
            return result;
        }
        #endregion

        /// <summary>
        /// 通用保存图片（可自定义上传图片类型）
        /// </summary>
        /// <param name="postedFile">图片</param>
        /// <param name="imgSize">图片大小</param>
        /// <param name="imgType">图片类型</param>
        /// <param name="isArea"></param>
        /// <returns></returns>
        public Dictionary<string, string> PostImg(HttpPostedFileBase postedFile, string imgSize, string imgType, int isArea = 0)
        {
            string[] allowTypes = string.IsNullOrEmpty(imgType) ? (new string[] { ".jpg", ".gif", ".png" }) : (imgType.Split('/'));
            Dictionary<string, string> result = new Dictionary<string, string>();
            string message = string.Empty;
            if (postedFile != null && !string.IsNullOrEmpty(postedFile.FileName))
            {
                string fileFullName = postedFile.FileName;
                string fileType = Path.GetExtension(fileFullName).ToLower();
                string fileName = Path.GetFileName(fileFullName);

                if (allowTypes.Contains(fileType))
                {
                    int conLen = postedFile.ContentLength;
                    //读取文件为 二进制流 , 保存到  图片表 , 返回 图片编号
                    byte[] btImgs = new byte[conLen];
                    using (Stream s = postedFile.InputStream)
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        s.Position = 0;
                        s.Read(btImgs, 0, conLen);
                    }
                    System.IO.Stream stream = new System.IO.MemoryStream(btImgs);//将byte数组回归成流
                    System.Drawing.Image original_image = System.Drawing.Image.FromStream(stream);//使用流创建图片
                    int width = original_image.Width;//图片的宽度
                    int height = original_image.Height;//图片的高度
                    original_image.Dispose();//释放资源
                    long length = stream.Length / 1024;//图片的大小（KB）
                    stream.Close();
                    stream.Dispose();
                    //string pictureSize = AppSettingManager.AppSettings[imgSize];
                    int img_Width = Convert.ToInt32(imgSize.Split(',')[0].Split(':')[1].ToString());
                    int img_Heighth = Convert.ToInt32(imgSize.Split(',')[1].Split(':')[1].ToString());
                    int img_Length = Convert.ToInt32(imgSize.Split(',')[2].Split(':')[1].ToString());
                    if (isArea == 1)//宽大于等于，高等于
                    {
                        if (width < img_Width || height != img_Heighth)
                        {
                            message = "请上传宽大于等于" + img_Width.ToString() + "高等于" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    else if (isArea == 0)
                    {
                        //如果Length0 则表示不限制此信息······
                        if (img_Length > 0 && length > img_Length)
                        {
                            message = "请上传小于" + img_Length.ToString() + "KB的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                        //如果宽0 高0 则表示不限制此信息······
                        if ((img_Width > 0 && width != img_Width) || (img_Heighth > 0 && height != img_Heighth))
                        {
                            message = "请上传" + img_Width.ToString() + "*" + img_Heighth.ToString() + "的图片！！";
                            result.Add("error", message);
                            return result;
                        }
                    }
                    SystemPictureFile model = new SystemPictureFile();
                    model.FileContent = Convert.ToBase64String(btImgs);
                    model.Extension = fileType;
                    model.FileName = fileName;
                    model.OperatorId = string.Empty;
                    model.PictureFileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ThreadSafeRandom.Next(1000).ToString("000");
                    SWfsSubjectService service = new SWfsSubjectService();
                    string pictureNo = service.InserSystemImg(model);

                    result.Add("success", pictureNo);
                    return result;
                }
                else
                {
                    result.Add("error", "请上传正确的图片格式！");
                    return result;
                }
            }
            result.Add("error", "请上传尺寸为" + imgSize + "，格式为" + imgType + "的图片");
            result.Add("error", "请上传图片！");
            return result;
        }


        public static string GetTimeStamp(string linkChar)
        {
            //TimeSpan span = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());


            var unixTime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;


            return linkChar + "timestamp=" + unixTime.ToString();
        }
        public static string GetTimeStampUrl(string url)
        {
            var s = url.Split('?');

            if (s.Length > 1)
            {

                if (s[1].IndexOf("timestamp=") >= 0)
                {
                    var ss = s[1].Split('&');
                    var newurl = s[0];
                    for (int i = 0; i < ss.Length; i++)
                    {
                        if (ss[i].IndexOf("timestamp=") >= 0)
                        {
                            ss[i] = GetTimeStamp("");
                        }
                        if (newurl == s[0])
                        {
                            newurl = newurl + "?" + ss[i];
                        }
                        else
                        {
                            newurl = newurl + "&" + ss[i];
                        }
                    }
                    return newurl;
                }
                else
                {
                    return url + "&" + GetTimeStamp("");
                }
            }
            else
            {
                return url + "?" + GetTimeStamp("");
            }
        }
    }
}
