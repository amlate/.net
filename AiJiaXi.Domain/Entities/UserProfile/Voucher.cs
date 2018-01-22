using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AiJiaXi.Domain.Entities.Orders;

namespace AiJiaXi.Domain.Entities.UserProfile
{
    public class Voucher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string VoucherNo { get; set; }

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
        public decimal Amount { get; set; }

        /// <summary>
        /// 使用代金券条件 - 满足金额
        /// </summary>
        public decimal PriceToUse { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime StartTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime EndTime { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        public string UserAccountId { get; set; }

        /// <summary>
        /// 分配状态
        /// </summary>
        public bool IsOccu { get; set; }

        /// <summary>
        /// 优惠券是否已使用  true:已使用 False:未使用
        /// </summary>
        public bool IsUsed { get; set; }

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        public long EventId { get; set; }


        [ForeignKey("AgencyId")]
        public virtual Agency Agency { get; set; }

        public long? AgencyId { get; set; }
    }
}