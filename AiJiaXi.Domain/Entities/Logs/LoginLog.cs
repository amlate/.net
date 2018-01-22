using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiJiaXi.Domain.Entities.Logs
{
    public class LoginLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 尝试登录的用户名.
        /// </summary>
        public string LoginUserName { get; set; }

        /// <summary>
        /// 登录Ip.
        /// </summary>
        public string LoginIp { get; set; }

        public int LoginDesc { get; set; }


        /// <summary>
        /// 操作时间
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime RiseTime { get; set; }
    }
}