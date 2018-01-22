using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Enums;

namespace AiJiaXi.Domain.Entities.Orders
{
    /// <summary>
    /// 活动
    /// </summary>
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 活动描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 活动类型
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// 活动地址，领取代金券或抽奖活动用
        /// </summary>
        public string EventUrl { get; set; }

        /// <summary>
        /// 折扣，价格优惠时用,充值活动时按百分比优惠的时候用这个
        /// </summary>
        public float Discount { get; set; }

        /// <summary>
        /// 价格满减或免运费，充值满多少用这个
        /// </summary>
        public decimal PriceTo { get; set; }

        /// <summary>
        /// 满减优惠， 充值满PriceTo送这个字段的金额
        /// </summary>
        public decimal BenefitPrice { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime StartTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public ApplyStatus ApplyStatus { get; set; }

        /// <summary>
        /// 活动是否进行
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// 当活动类型为优惠券时表示每个用户持有或者使用的最大优惠券数，活动类型为抢先优惠或其他时表示该活动每天前几笔订单享受优惠
        /// </summary>
        public int UseMaxVoucherNum { get; set; }

        /// <summary>
        /// 参与互动次数
        /// </summary>
        public int Nums { get; set; }
        
        public virtual ICollection<EventPrize> EventPrizes { get; set; }

        public virtual ICollection<Voucher> Vouchers { get; set; }

        public virtual List<Agency> Agencies { get; set; }
    }
}