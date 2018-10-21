using System;
using Project.Domain.Entities.Orders;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;

namespace Project.Common.Helpers
{
    public class OrderHelper
    {
        private static object LOCK = new object();

        public static string GenerateOrderNumber()
        {
            string orderNo = String.Empty;

            lock (LOCK)
            {
                IRepository<Order> _orderRepository = new Repository<Order>();
                string datetime = DateTimeOffset.Now.ToString("yyyyMMddHHmmss");
                orderNo = $"{datetime}{Next(1000, 4)}";
                if (_orderRepository.Exist(item => item.OrderNo == orderNo))
                {
                    return GenerateOrderNumber();
                }

                return orderNo;
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