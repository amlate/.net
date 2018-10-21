using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Enums;

namespace Project.Domain.Entities.UserProfile
{
    public class AccountRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("UserAccountId")]
        public virtual UserAccount UserAccount { get; set; }

        public string UserAccountId { get; set; }

        public TradeType TradeType { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime RiseTime { get; set; }

        public ResultType ResultType { get; set; }

        public decimal TradeMoney { get; set; }

        public decimal AccountBallance { get; set; }

        public int TradeScore { get; set; }

        public int ScoreBalance { get; set; }

        public string TradeId { get; set; }

        public string Note { get; set; }
    }
}