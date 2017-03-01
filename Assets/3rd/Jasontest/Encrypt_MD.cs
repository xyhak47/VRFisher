using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyWard.tools
{
   public class Encrypt_MD
    {
        public static string MakeSign(string str)
        {
            //转url格式
            //string str = ToUrl();
            //在string后加入API KEY
            //str += "&key=" + WxPayConfig.KEY;
            str += "ImmAimee";
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString();
        }

       public static string BoxPingSign(string str)
       {
           str += "ImmDownloadExcel";
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString();
       }
    }
}
