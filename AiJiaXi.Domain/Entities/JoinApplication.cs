using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AiJiaXi.Domain.Enums;

namespace AiJiaXi.Domain.Entities
{
    public class JoinApplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string JoinType { get; set; }

        public string Area { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime RiseTime { get; set; }

        public FeedbackStatus FeedbackStatus { get; set; }
    }
}