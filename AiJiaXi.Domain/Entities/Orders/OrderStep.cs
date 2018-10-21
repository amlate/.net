using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;

namespace Project.Domain.Entities.Orders
{
    public class OrderStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 记录每一步的订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime RiseTime { get; set; }

        /// <summary>
        /// 订单雇员 - 暂无此功能，可选择配送员
        /// </summary>
        public virtual Employee Employee { get; set; }

        public long? EmployeeId { get; set; }


        /// <summary>
        /// 操作用户
        /// </summary>
        public string OperationUser { get; set; }

        /// <summary>
        /// 操作用户类型
        /// </summary>
        public UserType UserType { get; set; }
        
        /// <summary>
        /// 订单备注
        /// </summary>
        public string Note { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public long OrderId { get; set; }
    }
}