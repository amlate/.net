using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Domain.Enums;
using System.Collections.Generic;

namespace Project.Domain.Entities.PromoterManager
{
    /// <summary>
    /// 返回 access_token
    /// </summary>
    public class accessToken
    {
        //获取到的凭证
        public string access_token { get; set; }

        //凭证有效时间，单位：秒
        public string expires_in { get; set; }



        //错误信息
        public string errcode { get; set; }
        
        public string errmsg { get; set; }
    }

    /// <summary>
    /// Ticket返回MODEL
    /// </summary>
    public class TicketReturn
    {
        //获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。
        public string ticket { get; set; }

        //该二维码有效时间，以秒为单位。 最大不超过2592000（即30天）
        public string expire_seconds { get; set; }

        //二维码图片解析后的地址，开发者可根据该地址自行生成需要的二维码图片
        public string url { get; set; }

        public string errmsg { get; set; }
    }

    //参数MODEL=========================================================================
    /// <summary>
    /// Ticket参数
    /// </summary>
    public class TicketParam
    {
        //该二维码有效时间，以秒为单位。 最大不超过2592000（即30天），此字段如果不填，则默认有效期为30秒。
        public string expire_seconds { get; set; }

        //二维码类型，QR_SCENE为临时,QR_LIMIT_SCENE为永久,QR_LIMIT_STR_SCENE为永久的字符串参数值
        public string action_name { get; set; }

        //二维码详细信息
        public action_info action_info { get; set; }

        //场景值ID，临时二维码时为32位非0整型，永久二维码时最大值为100000（目前参数只支持1--100000）
        public string scene_id { get; set; }
        //场景值ID（字符串形式的ID），字符串类型，长度限制为1到64，仅永久二维码支持此字段   
        public string scene_str { get; set; }
      
    }
    
    public class action_info
    {
       
        public scene scene { get; set; }

    }

    public class scene
    {
        //场景值ID（字符串形式的ID），字符串类型，长度限制为1到64，仅永久二维码支持此字段  
        public string scene_str { get; set; }

    }

    //请求菜单相关MODEL
    public class Menu
    {
       
        public ICollection<button> button { get; set; }

    }

    /// <summary>
    /// 主菜单 
    /// </summary>
    public class button
    {
        //默认赋click 
        public string type { get; set; }

        //菜单名称
        public string name { get; set; }

        //菜单KEY    类似于主键KEY，ID 
        public string key { get; set; }


        //跳转地址
        public string url { get; set; }

        public ICollection<sub_button> sub_button { get; set; }
    }


    /// <summary>
    /// 子菜单 
    /// </summary>
    public class sub_button
    {
        //默认赋click
        public string type { get; set; }

        //菜单名称
        public string name { get; set; }

        //菜单KEY    类似于主键KEY，ID 
        public string key { get; set; }

        //跳转地址
        public string url { get; set; }

        //跳转地址
        //public object sub_button { get; set; }

        //
        public string media_id { get; set; }
    }

    /// <summary>
    /// 百度地图返回MODEL
    /// </summary>
    public class BaiDuMap
    {
        //返回结果状态值， 成功返回0，其他值请查看 附录 。 
        public string status { get; set; }

        //返回数据
        public result result { get; set; }

   
    }

    public class result
    {
        public addressComponent addressComponent;

    }

    public class addressComponent
    {
        //城市名
        public string city { get; set; }

        //区县名
        public string district { get; set; }

        //省名
        public string province { get; set; }


        //街道名
        public string street { get; set; }


        //街道门牌号
        public string street_number { get; set; }

    }

}