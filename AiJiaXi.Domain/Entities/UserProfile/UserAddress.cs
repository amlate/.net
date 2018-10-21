using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Enums;

namespace Project.Domain.Entities.UserProfile
{
    public class UserAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Contact { get; set; }

        public Gender Gender { get; set; }

        public string ContactPhoneNum { get; set; }

        public string Addr { get; set; }

        public string Note { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public string ApplicationUserId { get; set; }

        public bool IsDefault { get; set; }

        public bool IsShow { get; set; }
    }
}