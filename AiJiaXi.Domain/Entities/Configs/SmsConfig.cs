using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiJiaXi.Domain.Entities.Configs
{
    public class SmsConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string EntId { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        public string ExtNo { get; set; }

        public string SmsSendUrl { get; set; }
    }
}