using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Repositories.Impl;
using AiJiaXi.Domain.Repositories.Interface;

namespace AiJiaXi.Common.Helpers
{
    public class VoucherHelper
    {
        private static object LOCK = new object();

        public static string GenerateVoucherNumber(IList<Voucher> list)
        {
            string voucherNO = String.Empty;

            lock (LOCK)
            {
                IRepository<Voucher> _voucheRepository = new Repository<Voucher>();
                string datetime = DateTimeOffset.Now.Ticks.ToString().Substring(0, 12);
                voucherNO = $"D{datetime}{Next(1000, 4)}";
                if (_voucheRepository.Exist(item => item.VoucherNo == voucherNO) || list.FirstOrDefault(item => item.VoucherNo == voucherNO) != null)
                {
                    return GenerateVoucherNumber(list);
                }

                return voucherNO;
            }
        }

        private static int Next(int numSeeds, int length)
        {
            // Create a byte array to hold the random value.  
            byte[] buffer = new byte[length];
            // Create a new instance of the RNGCryptoServiceProvider.  
            System.Security.Cryptography.RNGCryptoServiceProvider Gen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            // Fill the array with a random value.  
            Gen.GetBytes(buffer);
            // Convert the byte to an uint value to make the modulus operation easier.  
            uint randomResult = 0x0;//这里用uint作为生成的随机数  
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)buffer[i] << ((length - 1 - i) * 8));
            }
            // Return the random number mod the number  
            // of sides. The possible values are zero-based  
            return (int)(randomResult % numSeeds);
        }
    }
}