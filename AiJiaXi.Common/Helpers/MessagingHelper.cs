using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Project.Common.Helpers
{
    /// <summary>
    /// 数据传输工具类 - 用于生成与管局service交互所需的参数
    /// </summary>
    public class MessagingHelper
    {
        // 用于产生随机字符串用到字符串常量，包括数字和大小写的字母
        private const string CHARS = "1234567890abcdefghigklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// 随机产生字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns>生成的随机字符串</returns>
        public static string GenerateRandVal(int length)
        {
            char[] chars = CHARS.ToCharArray();
            // Console.WriteLine(chars.Length);
            StringBuilder newRandom = new StringBuilder();
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(chars[rd.Next(62)]);
            }

            return newRandom.ToString();
        }
    }
}
