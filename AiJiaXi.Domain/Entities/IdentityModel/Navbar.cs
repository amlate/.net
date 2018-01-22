using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiJiaXi.Domain.Entities.IdentityModel
{
    /// <summary>
    /// 导航菜单
    /// </summary>
    public class Navbar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "显示名称")]
        public string NameOption { get; set; }

        [Display(Name = "Controller")]
        public string Controller { get; set; }

        [Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "Area")]
        public string Area { get; set; }

        [Display(Name = "图标类别")]
        public string ImageClass { get; set; }

        [Display(Name = "状态")]
        public bool Status { get; set; }

        [Display(Name = "父类Id")]
        public int ParentId { get; set; }

        [Display(Name = "顶级菜单")]
        public bool IsParent { get; set; }

        [Display(Name = "排序")]
        public int Order { get; set; }

        public virtual IList<ApplicationRole> ApplicationRoles { get; set; }
    }
}