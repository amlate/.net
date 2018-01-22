using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AiJiaXi.Domain.Enums;

namespace AiJiaXi.Domain.Entities.PromoterManager
{
    /// <summary>
    /// 提现申请
    /// </summary>
    public class Withdrawals
    {
        //主键ID
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        //提现用户手机号
        public string Phone { get; set; }

        /// <summary>
        /// 预提现申请时间
        /// </summary>
        public string PreApplyDate { get; set; }

        /// <summary>
        /// 提现申请时间
        /// </summary>
        public string ApplyDate { get; set; }

        //提现银行
        public string Bank { get; set; }


        //提现用户姓名
        public string Name { get; set; }

        //提现银行帐号
        public string Accounts { get; set; }

        //提现金额
        public string Amount { get; set; }

        /// <summary>
        /// 处理提现时间
        /// </summary>
        public string HandleDate { get; set; }

        //提现状态
        public WithdrawalsStatus State { get; set; }

        /// <summary>
        /// 推广员已消费金额
        /// </summary>
        [NotMapped]
        public string AlreadyConsumed { get; set; }


    }
}