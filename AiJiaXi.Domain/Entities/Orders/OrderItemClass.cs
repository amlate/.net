using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AiJiaXi.Domain.Entities.Location;

namespace AiJiaXi.Domain.Entities.Orders
{
    public class OrderItemClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类别图标
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// 类别图标 - hover
        /// </summary>
        public string HoverIconUrl { get; set; }

        /// <summary>
        /// 所属城市
        /// </summary>
        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        public int CityId { get; set; }

        public string Counties { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        public virtual ICollection<OrderItem> OrderItems { get; set; }

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
        /// 是否显示
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 商品类别页面
        /// </summary>
        public string Url { get; set; }

   
    }
}