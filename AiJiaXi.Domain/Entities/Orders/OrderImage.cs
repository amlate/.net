using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Enums;

namespace Project.Domain.Entities.Orders
{
    /// <summary>
    /// 订单图片
    /// </summary>
    public class OrderImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 图片url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图片名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 订单图片备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 所属购物车条目
        /// </summary>
        [ForeignKey("CartItemId")]
        public virtual CartItem CartItem { get; set; }

        public long CartItemId { get; set; }

        /// <summary>
        /// 订单图片类型 - 晒单，瑕疵图片
        /// </summary>
        public OrderImageType OrderImageType { get; set; }
    }
}