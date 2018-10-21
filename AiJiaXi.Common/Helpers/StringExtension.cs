using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Project.Common.Helpers
{
    public static class StringExtension
    {
         /// <summary>
        /// 合并物理路径
        /// </summary>
        /// <param name="s1">路径1</param>
        /// <param name="s2">路径2</param>
        /// <returns>合并后的路径</returns>
        public static string CombinePath(this string s1, string s2)
        {
            if (s1 == string.Empty)
            {
                return s2;
            }

            if (string.IsNullOrEmpty(s2))
            {
                return s1;
            }
 
            s1 = s1.Replace('/', '\\');
            s2 = s2.Replace('/', '\\');

            while (s1.EndsWith("\\"))
            {
                s1 = s1.Substring(0, s1.Length - 1);
            }

            while (s2.StartsWith("\\"))
            {
                s2 = s2.Substring(1);
            }

            return string.Format("{0}\\{1}", s1, s2);
        }

        /// <summary>
        /// 合并url路径
        /// </summary>
        /// <param name="s1">路径1</param>
        /// <param name="s2">路径2</param>
        /// <returns>合并后的路径</returns>
        public static string CombineRelative(this string s1, string s2)
        {
            if (s1 == string.Empty)
            {
                return s2;
            }

            if (string.IsNullOrEmpty(s2))
            {
                return s1;
            }

            s1 = s1.Replace('\\', '/');
            s2 = s2.Replace('\\', '/');
            while (s1.EndsWith("/"))
            {
                s1 = s1.Substring(0, s1.Length - 1);
            }

            while (s2.StartsWith("/"))
            {
                s2 = s2.Substring(1);
            }

            return string.Format("{0}/{1}", s1, s2);
        }
        

        /// <summary>
        /// 将传入时间戳截取为两部分
        /// </summary>
        /// <param name="longStr">传入的时间戳字符串</param>
        /// <returns>返回长度为2的long数组</returns>
        public static long[] SubToLong(this string longStr)
        {
            if (string.IsNullOrEmpty(longStr) || longStr.Length <= 5)
            {
                return null;
            }

            int part = longStr.Length - 5;

            string firstPart = longStr.Substring(part);
            string secondPart = longStr.Substring(0, part);

            return new[] { Convert.ToInt64(firstPart), Convert.ToInt64(secondPart) };
        }

        /// <summary>
        /// 计算字符串的MD5哈希（若字符串为空，则返回空；否则返回计算结果）
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ComputeMD5Hash(this string str)
        {
            string hash = str;
            if (str != null)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] data = Encoding.ASCII.GetBytes(str);
                data = md5.ComputeHash(data);
                hash = "";
                for (int i = 0; i < data.Length; i++)
                    hash += data[i].ToString("X2");
            }

            return hash;
        }

        #region 身份证号码验证
        /// <summary>
        /// 验证身份证号码
        /// </summary>
        /// <param name="Id">身份证号码</param>
        /// <returns>验证成功为True，否则为False</returns>
        public static bool IsValidIDCard(this string Id)
        {
            if (Id.Length == 18)
            {
                bool check = CheckIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                bool check = CheckIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证18位身份证号
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>验证成功为True，否则为False</returns>
        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (!arrVarifyCode[y].Equals(Id.Substring(17, 1), StringComparison.OrdinalIgnoreCase))
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 验证15位身份证号
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>验证成功为True，否则为False</returns>
        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;// 数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }
        #endregion

        /// <summary>
        /// 验证输入字符串为邮政编码
        /// </summary>
        /// <param name="P_str_postcode">输入字符串</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsValidatePostCode(this string P_str_postcode)
        {
            return Regex.IsMatch(P_str_postcode, @"[1-9]\d{5}(?!\d)");
        }

        /// <summary>
        /// 验证输入字符串为E-mail地址
        /// </summary>
        /// <param name="P_str_email">输入字符串</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsValidateEmail(this string P_str_email)
        {
            return Regex.IsMatch(P_str_email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }

        /// <summary>
        /// 替换手机号中间四位为*
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        public static string ReturnPhoneNO(string phoneNo)
        {
            Regex re = new Regex("(\\d{3})(\\d{4})(\\d{4})", RegexOptions.None);
            phoneNo = re.Replace(phoneNo, "$1****$3");
            return phoneNo;
        }
    }
}