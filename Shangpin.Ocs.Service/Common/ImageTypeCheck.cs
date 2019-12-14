using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shangpin.Ocs.Service.Common
{
    public class ImageTypeCheck
    {
        static ImageTypeCheck()
        {
            _imageTag = InitImageTag();
        }
        private static SortedDictionary<int, ImageTypeEnum> _imageTag;

        public static readonly string ErrType = ImageTypeEnum.None.ToString();

        private static SortedDictionary<int, ImageTypeEnum> InitImageTag()
        {
            SortedDictionary<int, ImageTypeEnum> list = new SortedDictionary<int, ImageTypeEnum>();

            list.Add((int)ImageTypeEnum.BMP, ImageTypeEnum.BMP);
            list.Add((int)ImageTypeEnum.JPG, ImageTypeEnum.JPG);
            list.Add((int)ImageTypeEnum.GIF, ImageTypeEnum.GIF);
            list.Add((int)ImageTypeEnum.PCX, ImageTypeEnum.PCX);
            list.Add((int)ImageTypeEnum.PNG, ImageTypeEnum.PNG);
            list.Add((int)ImageTypeEnum.PSD, ImageTypeEnum.PSD);
            list.Add((int)ImageTypeEnum.RAS, ImageTypeEnum.RAS);
            list.Add((int)ImageTypeEnum.SGI, ImageTypeEnum.SGI);
            list.Add((int)ImageTypeEnum.TIFF, ImageTypeEnum.TIFF);
            return list;

        }

        /// <summary>  
        /// 通过文件头判断图像文件的类型  
        /// </summary>  
        /// <param name="path"></param>  
        /// <returns></returns>  
        public static string CheckImageTypeName(string path)
        {
            return CheckImageType(path).ToString();
        }
        /// <summary>  
        /// 通过文件头判断图像文件的类型  
        /// </summary>  
        /// <param name="path"></param>  
        /// <returns></returns>  
        public static ImageTypeEnum CheckImageType(string path)
        {
            byte[] buf = new byte[2];
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    int i = sr.BaseStream.Read(buf, 0, buf.Length);
                    if (i != buf.Length)
                    {
                        return ImageTypeEnum.None;
                    }
                }
            }
            catch (Exception exc)
            {
                //Debug.Print(exc.ToString());
                return ImageTypeEnum.None;
            }
            return CheckImageType(buf);
        }
        /// <summary>  
        /// 通过文件头判断图像文件的类型  
        /// </summary>  
        /// <param name="path"></param>  
        /// <returns></returns>  
        public static ImageTypeEnum CheckImageType(Stream sr, Boolean needDispose = true)
        {
            byte[] buf = new byte[2];
            try
            {

                int i = sr.Read(buf, 0, buf.Length);
                if (needDispose)
                    sr.Dispose();
                if (i != buf.Length)
                {
                    return ImageTypeEnum.None;
                }
            }
            catch (Exception exc)
            {
                //Debug.Print(exc.ToString());
                return ImageTypeEnum.None;
            }
            return CheckImageType(buf);
        }
        /// <summary>  
        /// 通过文件的前两个自己判断图像类型  
        /// </summary>  
        /// <param name="buf">至少2个字节</param>  
        /// <returns></returns>  
        public static ImageTypeEnum CheckImageType(byte[] buf)
        {
            if (buf == null || buf.Length < 2)
            {
                return ImageTypeEnum.None;
            }

            int key = (buf[1] << 8) + buf[0];
            ImageTypeEnum s;
            if (_imageTag.TryGetValue(key, out s))
            {
                return s;
            }
            return ImageTypeEnum.None;
        }

    }

    /// <summary>  
    /// 图像文件的类型  
    /// </summary>  
    public enum ImageTypeEnum
    {
        None = 0,
        BMP = 0x4D42,
        JPG = 0xD8FF,
        GIF = 0x4947,
        PCX = 0x050A,
        PNG = 0x5089,
        PSD = 0x4238,
        RAS = 0xA659,
        SGI = 0xDA01,
        TIFF = 0x4949
    }


}
