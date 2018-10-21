using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Domain.Entities.Configs
{
    public class EmailConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string ServerUrl { get; set; }

        public int Port { get; set; }

        public string UserAccount { get; set; }

        public string Password { get; set; }

    }
}