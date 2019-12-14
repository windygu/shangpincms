using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Shangpin.Ocs.Service.Common
{ 
   public class StringUtil
    {
        // Fields
        private static readonly byte[] _IVByte;
        private static readonly byte[] _KeyByte;
       
       public StringUtil()
        {

        }
        static StringUtil()
        {
            _KeyByte = new byte[] { 0xf3, 0x91, 0xd6, 0xff, 50, 0x1f, 0x4a, 2 };
            _IVByte = new byte[] { 0x15, 0x33, 0x21, 0x9f, 0xb6, 220, 0xaf, 0xd8 };
        }

       public static string Decrypt(string input)
       {
           if (string.IsNullOrEmpty(input))
           {
               return string.Empty;
           }
           input = input.Replace(' ', '+');
           ICryptoTransform transform = new DESCryptoServiceProvider().CreateDecryptor(_KeyByte, _IVByte);
           MemoryStream stream = new MemoryStream();
           CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
           byte[] buffer = Convert.FromBase64String(input);
           stream2.Write(buffer, 0, buffer.Length);
           stream2.FlushFinalBlock();
           stream2.Close();
           return Encoding.UTF8.GetString(stream.ToArray());
       }

       public static string Encrypt(string input)
       {
           ICryptoTransform transform = new DESCryptoServiceProvider().CreateEncryptor(_KeyByte, _IVByte);
           MemoryStream stream = new MemoryStream();
           CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
           byte[] bytes = Encoding.UTF8.GetBytes(input);
           stream2.Write(bytes, 0, bytes.Length);
           stream2.FlushFinalBlock();
           stream2.Close();
           return Convert.ToBase64String(stream.ToArray());
       }

       public static string ToHashString(string input)
       {
           byte[] bytes = Encoding.UTF8.GetBytes(input);
           byte[] buffer2 = new MD5CryptoServiceProvider().ComputeHash(bytes);
           StringBuilder builder = new StringBuilder();
           foreach (byte num in buffer2)
           {
               builder.Append(num.ToString("X2"));
           }
           return builder.ToString();
       }
    }
}
