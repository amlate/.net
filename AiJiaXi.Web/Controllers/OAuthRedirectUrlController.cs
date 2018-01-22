using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Plugin;
using AiJiaXi.Plugin.WeiXin;
using AiJiaXi.Domain;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Entities.Location;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Repositories.Impl;
using AiJiaXi.Domain.Repositories.Interface;
using ZhiYuan.IAR.Repository.EF;
using AiJiaXi.Domain.Entities.PromoterManager;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities.Configs;
using System.Net;
using Newtonsoft.Json;
using AiJiaXi.Common;

namespace AiJiaXi.Web.Controllers
{

    //LC专用Controller
    public class OAuthRedirectUrlController : Controller
    {
        private string _appid = "wx653820407bca1a42";
        private string _appsecret = "c17aa5110fdd94f5b48caf54fe354e67";
        private string reurl = "";

        
        public OAuthRedirectUrlController()
        {

        }
        
        public async Task<ActionResult> Index()
        {

            //获取传过来的值
            if (Request.QueryString["reurl"] != null && Request.QueryString["reurl"].ToString() != "")
            {
                reurl = Request.QueryString["reurl"].ToString();
            }
            string code = "";
            if (Request.QueryString["code"] != null && Request.QueryString["code"] != "")
            {
                code = Request.QueryString["code"].ToString();               
                OAuth_Token Model = Get_token(code);              
                OAuthUser OAuthUser_Model = Get_UserInfo(Model.access_token, Model.openid);
              
                //"http://xx.com/" + reurl + ".aspx?openid=" + OAuthUser_Model.openid;
                string strurl = "";


                #region 获得用户当前位置                 
                string cityIdParm = "";//城市名
                string countyIdParm = "";//区域名
                IRepository<UserLocation> _userLocationRepository = new Domain.Repositories.Impl.Repository<UserLocation>();
                var localModel = _userLocationRepository.Find(t=>t.FromUserName== OAuthUser_Model.openid);
                if (localModel != null)
                {
                    cityIdParm = localModel.CityName;
                    countyIdParm = localModel.CountyName;
                }
                #endregion

                switch (reurl)
                {
                    case "baiximeIndex"://我要洗衣菜单
                        strurl = "http://dl-aijiaxi.com/baixime/Index?OpenId="+ OAuthUser_Model.openid+ "&type=2&cityIdParm="+ cityIdParm+ "&countyIdParm="+ countyIdParm;

                    
                        break;
                    case "FrontPageUserInfo"://个人中心
                        strurl = "http://dl-aijiaxi.com/FrontPage/UserInfo?openId=" + OAuthUser_Model.openid+ "&cityName=" + cityIdParm + "&countyName=" + countyIdParm; 

                        break;

                    case "FrontPageJoin"://我要加盟
                        strurl = "http://dl-aijiaxi.com/FrontPage/Join";
                        break;
                    case "ErCodeIndex"://我的二维码
                        strurl = "http://dl-aijiaxi.com/ErCode/Index?OpenId=" + OAuthUser_Model.openid;
                        break;
                    case "FrontPageServiceFlow"://服务流程
                        strurl = "http://dl-aijiaxi.com/FrontPage/ServiceFlow?OpenId=" + OAuthUser_Model.openid;
                        break;
                    case "FrontPageTurntable"://我要抽奖
                        strurl = "http://dl-aijiaxi.com/FrontPage/Turntable?OpenId=" + OAuthUser_Model.openid;
                        break;
                    case "FrontPagePartner"://合作伙伴
                        strurl = "http://dl-aijiaxi.com/FrontPage/Partner";
                        break;
                    case "FrontPageExhibition"://洗涤展示
                        strurl = "http://dl-aijiaxi.com/FrontPage/Exhibition";
                        break;

                        
                }
                NLogger.Debug("72:" + reurl);
                Response.Redirect(strurl);
                //Response.Write("reurl:" + reurl + ",code:" + code + ",openid:" + OAuthUser_Model.openid);
            }

            return null;

        }


        //获得Token
        protected OAuth_Token Get_token(string Code)
        {
            string Str = GetJson("https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + _appid + "&secret=" + _appsecret + "&code=" + Code + "&grant_type=authorization_code");
            // OAuth_Token Oauth_Token_Model = JsonHelper.ParseFromJson(Str);
            OAuth_Token Oauth_Token_Model = JsonConvert.DeserializeObject<OAuth_Token>(Str);
            return Oauth_Token_Model;
        }
        //刷新Token
        protected OAuth_Token refresh_token(string REFRESH_TOKEN)
        {
            string Str = GetJson("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid=" + _appid + "&grant_type=refresh_token&refresh_token=" + REFRESH_TOKEN);
            OAuth_Token Oauth_Token_Model = JsonConvert.DeserializeObject<OAuth_Token>(Str);
            // OAuth_Token Oauth_Token_Model = JsonHelper.ParseFromJson(Str);
            return Oauth_Token_Model;
        }
        //获得用户信息
        protected OAuthUser Get_UserInfo(string REFRESH_TOKEN, string OPENID)
        {
            try
            {
               

                // Response.Write("获得用户信息REFRESH_TOKEN:" + REFRESH_TOKEN + "||OPENID:" + OPENID);
                string Str = GetJson("https://api.weixin.qq.com/sns/userinfo?access_token=" + REFRESH_TOKEN + "&openid=" + OPENID);        
                // OAuthUser OAuthUser_Model = JsonHelper.ParseFromJson(Str);
                OAuthUser OAuthUser_Model = JsonConvert.DeserializeObject<OAuthUser>(Str);
        
                return OAuthUser_Model;
            }
            catch (Exception ex)
            {
                NLogger.Debug("ERROR:" + ex.ToString());
                return null;
            }
        }
        protected string GetJson(string url)
        {
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            wc.Encoding = System.Text.Encoding.UTF8;
            string returnText = wc.DownloadString(url);

            if (returnText.Contains("errcode"))
            {
                //可能发生错误
            }
            //Response.Write(returnText);
            return returnText;
        }
    }



    public class OAuth_Token
    {
        public OAuth_Token()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        //access_token    网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同
        //expires_in    access_token接口调用凭证超时时间，单位（秒）
        //refresh_token    用户刷新access_token
        //openid    用户唯一标识，请注意，在未关注公众号时，用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID
        //scope    用户授权的作用域，使用逗号（,）分隔
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
    }

    public class OAuthUser
    {
        public OAuthUser()
        { }
        #region 数据库字段
        private string _openID;
        private string _searchText;
        private string _nickname;
        private string _sex;
        private string _province;
        private string _city;
        private string _country;
        private string _headimgUrl;
        private string[] _privilege;
        #endregion

        #region 字段属性
        /// 
        /// 用户的唯一标识
        /// 
        public string openid
        {
            set { _openID = value; }
            get { return _openID; }
        }
        /// 
        /// 
        /// 
        public string SearchText
        {
            set { _searchText = value; }
            get { return _searchText; }
        }
        /// 
        /// 用户昵称 
        /// 
        public string nickname
        {
            set { _nickname = value; }
            get { return _nickname; }
        }
        /// 
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知 
        /// 
        public string sex
        {
            set { _sex = value; }
            get { return _sex; }
        }
        /// 
        /// 用户个人资料填写的省份
        /// 
        public string province
        {
            set { _province = value; }
            get { return _province; }
        }
        /// 
        /// 普通用户个人资料填写的城市 
        /// 
        public string city
        {
            set { _city = value; }
            get { return _city; }
        }
        /// 
        /// 国家，如中国为CN 
        /// 
        public string country
        {
            set { _country = value; }
            get { return _country; }
        }
        /// 
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        /// 
        public string headimgurl
        {
            set { _headimgUrl = value; }
            get { return _headimgUrl; }
        }
        /// 
        /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）其实这个格式称不上JSON，只是个单纯数组
        /// 
        public string[] privilege
        {
            set { _privilege = value; }
            get { return _privilege; }
        }
        #endregion
    }
















    /// <summary>
    /// 获取当前定位后，返回城市，区域  ID  和名称
    /// </summary>
    /// <returns></returns>
    //public async Task<ActionResult> GetCity(string cityName,string countyName)
    //{
    //    //var a = Request["cityName"];
    //    //var b = Request["countyName"];
    //    //var t = Request["id"];
    //    //var d = Request["count"];

    //    long cityId = 0;
    //    long countyId = 0;
    //    // 如果城市名称不为空，说明是根据定位发来信息
    //    if (!String.IsNullOrWhiteSpace(cityName))
    //    {
    //        var findCityT = await _cityRepository.FindAsync(c => c.Name.Contains(cityName));
    //        if (findCityT != null)
    //        {
    //            cityId = findCityT.Id;
    //        }
    //    }
    //    //如果区域名称不为空，说明是根据定位发来信息
    //    if (!String.IsNullOrWhiteSpace(countyName))
    //    {
    //        var findCountyT = await _countyRepository.FindAsync(c => c.Name.Contains(countyName));
    //        if (findCountyT != null)
    //        {
    //            countyId = findCountyT.Id;
    //        }
    //    }


    //    //ViewBag.cityId = cityId;
    //    //ViewBag.countyId = countyId;
    //    //ViewBag.cityName = cityName;
    //    //ViewBag.countyName = countyName;


    //    return Json(new { cityId = cityId, countyId = countyId, cityName= cityName, countyName= countyName }, JsonRequestBehavior.AllowGet);
    //}





}