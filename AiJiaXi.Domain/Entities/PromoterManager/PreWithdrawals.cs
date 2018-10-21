using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Enums;

namespace Project.Domain.Entities.PromoterManager
{
    /// <summary>
    /// 预提现申请
    /// </summary>
    public class PreWithdrawals
    {
        //主键ID
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 预提现申请时间
        /// </summary>
        public string ApplyDate { get; set; }

        //预提现用户手机号
        public string Phone { get; set; }

        //已消费金额
        public string Amount { get; set; }

        //预提现状态
        public PreWithdrawalsStatus State { get; set; }

    }
}