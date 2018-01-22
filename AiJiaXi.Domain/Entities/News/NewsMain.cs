using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiJiaXi.Domain.Entities.News
{
    public class NewsMain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Title { get; set; }

        [ForeignKey("ThumbNailId")]
        public virtual ImageEntity ThumbNail { get; set; }

        public long? ThumbNailId { get; set; }

        [ForeignKey("ImageEntityId")]
        public virtual ImageEntity ImageEntity { get; set; }

        public long? ImageEntityId { get; set; }

        public string Content { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime AddTime { get; set; }

        public string AddUser { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ModifyTime { get; set; }

        public string ModifyUser { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? StartTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? EndTime { get; set; }

        public string IsShow { get; set; }

        public string IsDelete { get; set; }
    }
}