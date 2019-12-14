using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Configuration;
using System.Drawing.Imaging;

namespace Shangpin.Ocs.Service.Common
{
    public class PictureFileConverter
    {
        private static string watermarkPic;
        static PictureFileConverter()
        {
            watermarkPic = AppSettingManager.AppSettings["WatermarkPicPath"];
            //备注，水印图片为预留功能，使用此功能需要在AppSetting.config中配置水印图片的路径20130709
        }
        public PictureFileConverter()
        { }


        public static Image GetThumbnailImage(string imageStringContent, int outWidth, int outHeight, Stream stream = null)
        {
            return GetThumbnailImage(imageStringContent, outWidth, outHeight, false, stream);
        }
        /// <summary>
        ///  将图片转换成字节数组
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(Image image, ImageFormat format)
        {
            //实例化流 
            MemoryStream imageStream = new MemoryStream();
            //将图片的实例保存到流中            
            image.Save(imageStream, format);
            //保存流的二进制数组 
            byte[] imageContent = new Byte[imageStream.Length];

            imageStream.Position = 0;
            //将流泻如数组中 
            imageStream.Read(imageContent, 0, (int)imageStream.Length);

            return imageStream.ToArray();
        }

        public static Image GetThumbnailImage(string imageStringContent, int outWidth, int outHeight, bool makeTransparent, Stream stream = null)
        {
            if (stream == null)
                stream = new MemoryStream(Convert.FromBase64String(imageStringContent));
            Image image = Image.FromStream(stream);
            int width = image.Width;
            int height = image.Height;
            int x = 0;
            int num4 = 0;
            int y = 0;
            int num6 = 0;
            if ((outWidth == 0) && (outHeight == 0))
            {
                outWidth = width;
                outHeight = height;
                num4 = width;
                num6 = height;
            }
            if (((outWidth == 0) && (outHeight != 0)) && (outHeight >= height))
            {
                outWidth = width;
            }
            if ((outWidth != 0) && (outHeight == 0))
            {
                if (outWidth > width)
                {
                    x = (outWidth - width) / 2;
                    num4 = x + width;
                    y = 0;
                    num6 = height;
                    outHeight = height;
                }
                if (outWidth == width)
                {
                    x = 0;
                    num4 = width;
                    y = 0;
                    num6 = height;
                    outHeight = height;
                }
                if (outWidth < width)
                {
                    x = 0;
                    num4 = outWidth;
                    decimal num7 = (outWidth * 1.00M) / (width * 1.00M);
                    outHeight = (int)(height * num7);
                    y = 0;
                    num6 = outHeight;
                }
            }
            if ((outWidth != 0) && (outHeight != 0))
            {
                decimal num8 = (outWidth * 1.00M) / (width * 1.00M);
                decimal num9 = (outHeight * 1.00M) / (height * 1.00M);
                if (num9 > num8)
                {
                    y = (outHeight - ((int)(height * (((double)outWidth) / ((double)width))))) / 2;
                    num6 = ((height * outWidth) / width) + y;
                    num4 = outWidth;
                }
                else
                {
                    x = (outWidth - ((int)(width * (((double)outHeight) / ((double)height))))) / 2;
                    num4 = ((width * outHeight) / height) + x;
                    num6 = outHeight;
                }
            }
            Rectangle destRect = new Rectangle(x, y, num4 - x, num6 - y);
            Rectangle srcRect = new Rectangle(0, 0, width, height);
            Bitmap bitmap = new Bitmap(outWidth, outHeight);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.White);
            graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
            if (makeTransparent)
            {
                graphics.Dispose();
                return MakeTransparentGif(bitmap, Color.White);
            }
            if (((outWidth > 300) && (outHeight > 300)) && File.Exists(watermarkPic))
            {
                Image image2 = Image.FromFile(watermarkPic);
                ImageAttributes imageAttr = new ImageAttributes();
                float[][] numArray2 = new float[5][];
                float[] numArray3 = new float[5];
                numArray3[0] = 1f;
                numArray2[0] = numArray3;
                float[] numArray4 = new float[5];
                numArray4[1] = 1f;
                numArray2[1] = numArray4;
                float[] numArray5 = new float[5];
                numArray5[2] = 1f;
                numArray2[2] = numArray5;
                float[] numArray6 = new float[5];
                numArray6[3] = 0.5f;
                numArray2[3] = numArray6;
                float[] numArray7 = new float[5];
                numArray7[4] = 1f;
                numArray2[4] = numArray7;
                float[][] newColorMatrix = numArray2;
                ColorMatrix matrix = new ColorMatrix(newColorMatrix);
                imageAttr.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                graphics.DrawImage(image2, new Rectangle((outWidth / 2) - 200, (outHeight / 2) - 200, 400, 400), 0, 0, image2.Width, image2.Height, GraphicsUnit.Pixel, imageAttr);
            }
            graphics.Dispose();
            return bitmap;
        }

        public static Bitmap MakeTransparentGif(Bitmap bitmap, Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Gif);
            MemoryStream stream2 = new MemoryStream((int)stream.Length);
            int count = 0;
            byte[] buffer = new byte[0x100];
            byte num5 = 0;
            stream.Seek(0L, SeekOrigin.Begin);
            count = stream.Read(buffer, 0, 13);
            if (((buffer[0] != 0x47) || (buffer[1] != 0x49)) || (buffer[2] != 70))
            {
                return null;
            }
            stream2.Write(buffer, 0, 13);
            int num6 = 0;
            if ((buffer[10] & 0x80) > 0)
            {
                num6 = ((((int)1) << ((buffer[10] & 7) + 1)) == 0x100) ? 0x100 : 0;
            }
            while (num6 != 0)
            {
                stream.Read(buffer, 0, 3);
                if (((buffer[0] == r) && (buffer[1] == g)) && (buffer[2] == b))
                {
                    num5 = (byte)(0x100 - num6);
                }
                stream2.Write(buffer, 0, 3);
                num6--;
            }
            bool flag = false;
        Label_00FC:
            stream.Read(buffer, 0, 1);
            stream2.Write(buffer, 0, 1);
            if (buffer[0] != 0x21)
            {
                goto Label_01BD;
            }
            stream.Read(buffer, 0, 1);
            stream2.Write(buffer, 0, 1);
            flag = buffer[0] == 0xf9;
            while (true)
            {
                stream.Read(buffer, 0, 1);
                stream2.Write(buffer, 0, 1);
                if (buffer[0] == 0)
                {
                    goto Label_00FC;
                }
                count = buffer[0];
                if (stream.Read(buffer, 0, count) != count)
                {
                    return null;
                }
                if (flag && (count == 4))
                {
                    buffer[0] = (byte)(buffer[0] | 1);
                    buffer[3] = num5;
                }
                stream2.Write(buffer, 0, count);
            }
        Label_01A6:
            count = stream.Read(buffer, 0, 1);
            stream2.Write(buffer, 0, 1);
        Label_01BD:
            if (count > 0)
            {
                goto Label_01A6;
            }
            stream.Close();
            stream2.Flush();
            return new Bitmap(stream2);
        }

        public string ImageStreamToBase64(Stream imageStream)
        {
            int length = (int)imageStream.Length;
            byte[] buffer = new byte[length];
            imageStream.Read(buffer, 0, length);
            imageStream.Close();
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 生成默认图片
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap GetDefalutPic(int width, int height)
        {
            //创建一字体风格 
            Font rectangleFont = new Font("Arial", 10, FontStyle.Bold);

            //创建整数变量 
            width = width <= 0 ? 100 : width;
            height = height <= 0 ? 100 : height;

            //创建一个随机数字生成器并且基于它创建
            //变量值 
            Random r = new Random();
            int x = r.Next(75);
            int a = r.Next(155);
            int x1 = r.Next(100);

            //创建一张位图并且使用它创建一个
            //Graphics对象 
            Bitmap bmp = new Bitmap(
            width, height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.LightGray);

            //使用这个Graphics对象绘制3个矩形 
            g.DrawRectangle(Pens.White, 1, 1, width - 3, height - 3);
            //g.DrawRectangle(Pens.Aquamarine, 2, 2, width - 3, height - 3);
            //g.DrawRectangle(Pens.Black, 0, 0, width, height);

            //使用这个Graphics对象输出一个字符串
            // on the rectangles. 
            g.DrawString("图片不存在", rectangleFont, SystemBrushes.WindowText, new PointF(10, 40));

            //在其中两个矩形上添加颜色 
            //g.FillRectangle(new SolidBrush(Color.FromArgb(a, 255, 128, 255)), x, 20, 100, 50);

            //g.FillRectangle(new LinearGradientBrush(new Point(x, 10),new Point(x1 + 75, 50 + 30),
            //Color.FromArgb(128, 0, 0, 128),
            //Color.FromArgb(255, 255, 255, 240)),
            //x1, 50, 75, 30);

            //把位图保存到响应流中并且把它转换成JPEG格式 
            return bmp;
            //释放掉Graphics对象和位图所使用的内存空间 
            //g.Dispose();
            //bmp.Dispose();
        }
    }
}
