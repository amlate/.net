using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Domain.Entities.Orders
{
    /// <summary>
    /// 订单并加
    /// </summary>
    public class OrderRate
    {
        [Key]
        [ForeignKey("Order")]
        public long Id { get; set; }

        /// <summary>
        /// 订单星级评价 - 总评
        /// </summary>
        public float Stars { get; set; }

        /// <summary>
        /// 揽件评价
        /// </summary>
        public float PatchStars { get; set; }

        /// <summary>
        /// 送返评价
        /// </summary>
        public float DispatchStars { get; set; }

        /// <summary>
        /// 订单评论
        /// </summary>
        public string OrderComment { get; set; }

        /// <summary>
        /// 揽件评论
        /// </summary>
        public string PatchComment { get; set; }

        /// <summary>
        /// 送返评论
        /// </summary>
        public string DispatchComment { get; set; }

        /// <summary>
        /// 晒单图片
        /// </summary>
        public string ShareOrderImgUrls { get; set; }

        /// <summary>
        /// 所属订单
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// 评价时间
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime RiseTime { get; set; }

        /// <summary>
        /// 是否审核通过
        /// </summary>
        public bool IsApproval { get; set; }
    }
}