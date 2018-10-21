using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Domain.Entities.Orders
{
    public class EventPrize
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        public long EventId { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public string Images { get; set; }

        public int Nums { get; set; }

        public long Weight { get; set; }

        public bool IsLog { get ; set; }
        public virtual ICollection<EventAward> EventAwards { get; set; }
    }
}