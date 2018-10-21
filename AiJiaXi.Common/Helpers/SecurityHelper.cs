using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace Project.Common.Helpers
{
    /// <summary>
    /// 验证码工具类.
    /// </summary>
    public class SecurityHelper
    {
        /// <summary>
        /// 创建验证码字符
        /// </summary>
        /// <param name="length">字符长度</param>
        /// <returns>验证码字符</returns>
        public static string CreateVerificationText(int length)
        {
            char[] _verification = new char[length];
            char[] _dictionary = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            Random _random = new Random();
            for (int i = 0; i < length; i++)
            {
                _verification[i] = _dictionary[_random.Next(_dictionary.Length - 1)];
            }

            return new string(_verification);
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="verificationText">验证码字符串</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片长度</param>
        /// <returns>图片</returns>
        public static Bitmap CreateVerificationImage(string verificationText)
        {
            int iwidth = (int)(verificationText.Length * 16);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(iwidth, 27);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            //定义颜色
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Chocolate, Color.Brown, Color.DarkCyan, Color.Purple };
            //定义字体
            string[] font = { "Arial", "Verdana", "Microsoft Sans Serif", "Comic Sans MS" };
            Random rand = new Random();
            //随机输出噪点
            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(image.Width);
                int y = rand.Next(image.Height);
                g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
            }

            //输出不同字体和颜色的验证码字符
            for (int i = 0; i < verificationText.Length; i++)
            {
                int cindex = rand.Next(7);
                int findex = rand.Next(4);

                Font f = new System.Drawing.Font(font[findex], 13, System.Drawing.FontStyle.Bold);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                g.DrawString(verificationText.Substring(i, 1), f, b, 1 + (i * 14), 1);
            }

            //画一个边框(在此设置边框颜色为绿色)
            g.DrawRectangle(new Pen(Color.Gray, 0), 0, 0, image.Width - 1, image.Height - 1);
            g.Dispose();
            return image;
        }

        /// <summary>
        /// 256位散列加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns>密文</returns>
        public static string Sha256(string plainText)
        {
            SHA256Managed _sha256 = new SHA256Managed();
            byte[] _cipherText = _sha256.ComputeHash(Encoding.Default.GetBytes(plainText));
            return Convert.ToBase64String(_cipherText);
        }

        public static string GenPhoneCode()
        {
            Random random = new Random();

            return random.Next(1000, 9999).ToString();
        }
    }
}