using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WeiPay
{

    /**
     * 
     * 作用：微信支付公用参数，微信支付版本V3.3.7
     * 作者：邹学典
     * 编写日期：2014-12-25
     * 备注：请正确填写相关参数
     * 
     * */
     
    public   class PayConfig
    {
        private static string WeiXinTenpay = ConfigurationManager.AppSettings["Tenpay"];
        private static string WeiXinMchId = ConfigurationManager.AppSettings["MchId"];
        private static string WeiXinAppId = ConfigurationManager.AppSettings["AppId"];
        private static string WeiXinAppSecret = ConfigurationManager.AppSettings["AppSecret"];
        private static string WeiXinAppKey = ConfigurationManager.AppSettings["AppKey"];
        private static string WeiXinSendUrl = ConfigurationManager.AppSettings["SendUrl"];
        private static string WeiXinPayUrl = ConfigurationManager.AppSettings["PayUrl"];
        private static string WeiXinNotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];

        /// <summary>
        /// 人民币
        /// </summary>
        public static string Tenpay = WeiXinTenpay; 
   
        /// <summary>
        /// mchid ， 微信支付商户号
        /// </summary>
        public static string MchId = WeiXinMchId; //

        /// <summary>
        /// appid，应用ID， 在微信公众平台中 “开发者中心”栏目可以查看到
        /// </summary>
        public static string AppId = WeiXinAppId; 

        /// <summary>
        /// appsecret ，应用密钥， 在微信公众平台中 “开发者中心”栏目可以查看到
        /// </summary>
        public static string AppSecret = WeiXinAppSecret; 
      
        /// <summary>
        /// paysignkey，API密钥，在微信商户平台中“账户设置”--“账户安全”--“设置API密钥”，只能修改不能查看
        /// </summary>
        public static string AppKey = WeiXinAppKey;

        /// <summary>
        /// 支付起点页面地址，也就是send.aspx页面完整地址
        /// 用于获取用户OpenId，支付的时候必须有用户OpenId，如果已知可以不用设置
        /// </summary>
        public static string SendUrl = WeiXinSendUrl; 
        /// <summary>
        /// 支付页面，请注意测试阶段设置授权目录，在微信公众平台中“微信支付”---“开发配置”--修改测试目录   
        /// 注意目录的层次，比如我的：http://zousky.com/
        /// </summary>
        public static string PayUrl = WeiXinPayUrl; 
        /// <summary>
        ///  支付通知页面，请注意测试阶段设置授权目录，在微信公众平台中“微信支付”---“开发配置”--修改测试目录   
        /// 支付完成后的回调处理页面,替换成notify_url.asp所在路径
        /// </summary>
        public static string NotifyUrl = WeiXinNotifyUrl; 


    }
}
