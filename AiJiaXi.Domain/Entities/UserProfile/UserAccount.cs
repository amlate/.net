using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Entities.IdentityModel;

namespace Project.Domain.Entities.UserProfile
{
    public class UserAccount
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }

        //余额
        public decimal Balance { get; set; }
        //冻结金额
        public decimal FrozenMoney { get; set; }

        //推广员提成金额
        public decimal CommissionMoney { get; set; }

        /// <summary>
        /// 积分余额
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 自定义用户表中，微信OpenId 与ApplicationUser表中一致
        /// </summary>      
        public string OpenId { get; set; }

        /// <summary>
        /// 冻结积分
        /// </summary>
        public int FrozenScore { get; set; }

        /// <summary>
        /// 会员卡号
        /// </summary>
        public string CardNumber { get; set; }

        public virtual ICollection<AccountRecord> AccountRecords { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Voucher> Vouchers { get; set; }

        //推广员二维码URL【推广用】
        public string CommissionUrl { get; set; }
    }
}