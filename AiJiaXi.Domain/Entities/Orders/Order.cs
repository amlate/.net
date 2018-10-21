using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations.Model;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;

namespace Project.Domain.Entities.Orders
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 用户地址
        /// </summary>
        [ForeignKey("UserAddressId")]
        public virtual UserAddress UserAddress { get; set; }

        /// <summary>
        /// 用户地址
        /// </summary>
        public long? UserAddressId { get; set; }

        /// <summary>
        /// 下单用户
        /// </summary>
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        /// <summary>
        /// 下单用户
        /// </summary>
        public string ApplicationUserId { get; set; }

        /// <summary>
        /// 购物车条目
        /// </summary>
        public virtual ICollection<CartItem> CartItems { get; set; }

        /// <summary>
        /// 预约时间
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime Appointment { get; set; }

        public string AppointmentTime { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 实际支付
        /// </summary>
        public decimal Fact { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime RiseTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? CompleteTime { get; set; }

        /// <summary>
        /// 订单进度
        /// </summary>
        public virtual ICollection<OrderStep> OrderSteps { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        public PayType PayType { get; set; }

        /// <summary>
        /// 代理商
        /// </summary>
        [ForeignKey("AgencyId")]
        public virtual Agency Agency { get; set; }

        public long AgencyId { get; set; }

        /// <summary>
        /// 订单评价
        /// </summary>
        [ForeignKey("OrderRateId")]
        public virtual OrderRate OrderRate { get; set; }

        public long? OrderRateId { get; set; }

        /// <summary>
        /// 用户使用优惠券
        /// </summary>
        [ForeignKey("VoucherId")]
        public virtual Voucher Voucher { get; set; }

        public Guid? VoucherId { get; set; }

        /// <summary>
        /// 活动相关
        /// </summary>
        public string Events { get; set; }

        /// <summary>
        /// 投诉类型
        /// </summary>
        public ComplaintType ComplaintType { get; set; }

        /// <summary>
        /// 投诉备注
        /// </summary>
        public string ComplaintNote  { get; set; }
    }
}