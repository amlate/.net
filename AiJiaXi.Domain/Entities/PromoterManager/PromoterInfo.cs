using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Enums;

namespace Project.Domain.Entities.PromoterManager
{
    /// <summary>
    /// 推广信息
    /// </summary>
    public class PromoterInfo
    {
        //主键ID
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 好友微信唯一ID【好友关注后未注册时，只会存一个这个ID，之后会根据这个ID更新这个手机号】
        /// </summary>
        public string FriendsWeiXinId { get; set; }

        /// <summary>
        /// 好友手机号
        /// </summary>
        public string FriendsPhone { get; set; }

        //成为好友时间
        public string FollowDate { get; set; }

        //我的手机号
        public string MyPhone { get; set; }

        //消费金额
        [NotMapped]
        public string ConsumptionAmount { get; set; }

        [NotMapped]
        //我的提成金额
        public string MyAmount { get; set; }


    }
}