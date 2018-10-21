using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Domain.Entities.Logs
{
    public class OperationLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 操作模块
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 操作过程
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 操作者用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 操作者IP地址
        /// </summary>
        public string UserIP { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime RiseTime { get; set; }
    }
}