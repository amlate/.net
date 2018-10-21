using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Project.Domain.Entities.IdentityModel
{
    public class ApplicationUser : IdentityUser
    {
        public string RealName { get; set; }

        public string Dept { get; set; }

        public string Position { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? AddTime { get; set; }


        public string HeadAppearUrl { get; set; }

        public virtual ICollection<UserAddress> UserAddresses { get; set; }

        public virtual ICollection<EventAward> EventAwards { get; set; }

        public UserType UserType { get; set; }

        [ForeignKey("AgencyId")]
        public virtual Agency Agency { get; set; }

        public long? AgencyId { get; set; }
        
        public virtual UserAccount UserAccount { get; set; }

        /// <summary>
        /// 负责区域
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 系统用户表中，微信OpenId 与UserAccount表中一致
        /// </summary>      
        public string OpenId { get; set; }
        /// <summary>
        /// 帐号冻结
        /// </summary>
        public bool IsFrozen { get; set; }

        //推广员的好友人数
        [NotMapped]
        public string FrendCount { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }
}