using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiJiaXi.Domain.Entities.Orders
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public long OrderId { get; set; }

        [ForeignKey("OrderItemId")]
        public virtual OrderItem OrderItem { get; set; }

        public long OrderItemId { get; set; }

        public int Nums { get; set; }

        public virtual ICollection<OrderImage> OrderImages { get; set; }
    }
}