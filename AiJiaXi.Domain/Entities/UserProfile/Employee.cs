using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Enums;

namespace AiJiaXi.Domain.Entities.UserProfile
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("AgencyId")]
        public virtual Agency Agency { get; set; }

        public long AgencyId { get; set; }

        public virtual ICollection<OrderStep> OrderSteps { get; set; }

        public string EmployeeNo { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public string Phone { get; set; }

        public string IdNum { get; set; }

        public InServiceState InServiceState { get; set; }
    }
}