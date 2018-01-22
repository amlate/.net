using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Entities.Location;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Enums;

namespace AiJiaXi.Domain.Entities
{
    public class Agency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }
        
        public int CountyId { get; set; }

        [ForeignKey("CountyId")]
        public virtual County County { get; set; }

        public int CityId { get; set; }


        public int ProvinceId { get; set; }

        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }

        public string Note { get; set; }

        //代理商姓名
        public string Contact { get; set; }

        //代理商手机号
        public string ContactMobile { get; set; }

        public string ContactEmail { get; set; }

        /// <summary>
        /// 服务范围中描述
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 服务范围地图 图片(路径说明：/upload/rangemap
        /// )
        /// </summary>
        public string RangeMap { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}