using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Domain.Entities.Orders
{
    /// <summary>
    /// 产品
    /// </summary>
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 图片 hover
        /// </summary>
        public string HoverImageUrl { get; set; }

        /// <summary>
        /// 定价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 包含数目
        /// </summary>
        public int Nums { get; set; }

        /// <summary>
        /// 所属产品类别
        /// </summary>
        [ForeignKey("ItemClassId")]
        public virtual OrderItemClass ItemClass { get; set; }

        public long ItemClassId { get; set; }

        /// <summary>
        /// 洗涤周期，单位天
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 添加用户
        /// </summary>
        public string AddUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public string ModifyUser { get; set; }

        /// <summary>
        /// 是否有效（显示）
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int Order { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }

        /// <summary>
        ///活动专用类别
        /// </summary>
        public Enums.ClientEventType ClientEventType { get; set; }
    }
}