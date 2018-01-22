using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AiJiaXi.Domain.Entities.Orders;

namespace AiJiaXi.Domain.Entities.UserProfile
{
    public class VoucherViewModel
    {

        /// <summary>
        /// 代金券名称 - 可用作判断一类代金券
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 代金券名称 - 可用作判断一类代金券
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 使用代金券条件 - 满足金额
        /// </summary>
        public string PriceToUse { get; set; }

        public string StartTime { get; set; }

    
        public string EndTime { get; set; }
      
        public string UserAccountId { get; set; }
    }
}