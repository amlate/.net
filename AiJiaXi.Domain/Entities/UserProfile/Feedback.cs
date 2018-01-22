using System;
using System.ComponentModel.DataAnnotations.Schema;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Enums;

namespace AiJiaXi.Domain.Entities.UserProfile
{
    public class Feedback
    {
        public long Id { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime RiseTime { get; set; }

        public DealStatus DealStatus { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DealTime { get; set; }
    }
}