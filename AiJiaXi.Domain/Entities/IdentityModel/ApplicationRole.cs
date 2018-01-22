using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AiJiaXi.Domain.Entities.IdentityModel
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole():base()
        {
        }

        public ApplicationRole(string name, string description)
            : base(name)
        {
            this.Description = description;
        }

        public string Description { get; set; }

        public virtual IList<Navbar> Navbars { get; set; }
    }
}