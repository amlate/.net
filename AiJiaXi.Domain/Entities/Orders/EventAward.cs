using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.UserProfile;

namespace Project.Domain.Entities.Orders
{
    /// <summary>
    /// 抽奖活动的得奖者
    /// </summary>
    public class EventAward
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public virtual string UserId { get; set; }

        [ForeignKey("AddressId")]
        public virtual UserAddress Address { get; set; }

        public virtual long? AddressId { get; set; }

        [ForeignKey("EventPrizeId")]
        public virtual EventPrize EventPrize { get; set; }

        public long EventPrizeId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        public bool Flag { get; set; }
    }
}