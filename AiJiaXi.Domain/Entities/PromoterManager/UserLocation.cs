using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Enums;

namespace Project.Domain.Entities.PromoterManager
{
    /// <summary>
    /// 用户当前地理位置
    /// </summary>
    public class UserLocation
    {
        //主键ID
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 开发者微信号 
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        ///发送方帐号（一个OpenID） 
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间 （整型） 
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 地理位置纬度 
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 地理位置经度 
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 地理位置精度 
        /// </summary>
        public double Precision { get; set; }


        /// <summary>
        /// 省名
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市名称 
        /// </summary>
        public string CityName { get; set; }


        /// <summary>
        /// 区域名称 
        /// </summary>
        public string CountyName { get; set; }


        /// <summary>
        /// 定位当前省名
        /// </summary>
        public string LocationProvinceName { get; set; }

        /// <summary>
        /// 定位当前城市名称 
        /// </summary>
        public string LocationCityName { get; set; }


        /// <summary>
        /// 定位当前区域名称 
        /// </summary>
        public string LocationCountyName { get; set; }
    }
}