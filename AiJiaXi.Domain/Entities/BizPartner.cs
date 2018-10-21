using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Domain.Entities
{
    public class BizPartner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }

        public string ThumbNail { get; set; }

        [ForeignKey("ImageEntityId")]
        public virtual ImageEntity ImageEntity { get; set; }

        public long ImageEntityId { get; set; }

        public string Desc { get; set; }

        public bool IsShow { get; set; }

        public int Order { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime AddTime { get; set; }
    }
}