using Project.Common.Helpers;
using Project.Domain.Entities.PromoterManager;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;
using Project.Domain.Repositories.Interface;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Web.Filters;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    //用户二维码页面
    public class QrCodeController : Controller
    {
        private ApplicationUserManager _userManager;//用户信息
        private IRepository<UserAccount> _userAccountRepository;//用户表啊[自定义的]


        public QrCodeController(IRepository<UserAccount> userAccountRepository, ApplicationUserManager userManager)
        {
            _userManager = userManager;
            _userAccountRepository = userAccountRepository;

        }

        public async Task<ActionResult> Index()
        {
            //登录用户信息
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);

            string path = "";
            string titleMs = "您目前没有生成过二维码";//描述
            //自定义用户信息
            var model = _userAccountRepository.Find(item => true && item.Id == user.Id);
            if (model != null)
            {
                path = model.CommissionUrl;
                if (!String.IsNullOrWhiteSpace(path))
                {
                    titleMs = "您已经生成过二维码，请右键保存图片，进行推广使用!";
                }
            }
            ViewBag.titleMs = titleMs;
            ViewBag.path = path;
            return View();
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Create()
        {
            //得到access_token
            string access_token = await getAccessToken();
            if (!String.IsNullOrWhiteSpace(access_token))
            {
                string path = "";
                //得到Ticket
                TicketReturn TicketMod = getTicket(access_token);
                if (!String.IsNullOrWhiteSpace(TicketMod.ticket))
                {
                    //得到二维码URL
                    path = await getQrCode(TicketMod.ticket);
                    if (String.IsNullOrWhiteSpace(path))
                    {
                        return JavaScript("layer.alert('获取二维码URL为空，请联系管理员！');");
                    }
                }
                else
                {
                    return JavaScript("layer.alert('ticket数据为空，请联系管理员！');");
                }            


                
                //登录用户信息
                var userId = User.Identity.GetUserId();
                var user = this._userManager.FindById(userId);

                //自定义用户信息【将二维码地址存到，用户类型的URL中】
                //下面是推广员对应的用户的MODEL
                var model = _userAccountRepository.Find(item => true && item.ApplicationUser.UserType==UserType.User && item.ApplicationUser.PhoneNumber==user.PhoneNumber);         

                if (model != null)
                {
                    model.CommissionUrl = path;
                    var result = await _userAccountRepository.UpdateAsync(model);
                    if (result == false)
                    {
                        return JavaScript("layer.alert('保存二维码失败！');");
                    }
                }
                //推广员MODEL
                var model1 = _userAccountRepository.Find(item => true && item.Id == userId);
                if (model1 != null)
                {
                    model1.CommissionUrl = path;
                    var result1 = await _userAccountRepository.UpdateAsync(model1);
                    if (result1 == false)
                    {
                        return JavaScript("layer.alert('保存二维码失败！');");
                    }
                }
            }
            else
            {
                return JavaScript("layer.alert('access_token数据为空，请联系管理员！');");
            }
            
            return JavaScript("layer.alert('二维码已生成并成功保存二维码！',function(){location.href=window.location.href});");
        }


        /// <summary>
        /// 创建自定义菜单 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> CreateMenu()
        {
            //得到access_token
            string access_token = await getAccessToken();
            if (!String.IsNullOrWhiteSpace(access_token))
            {

                string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + access_token;
                //菜单MODEL 
                Menu menu = new Menu();
                //主菜单1
                button button1 = new button();
                button1.type = "view";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                button1.name = "我要洗衣";

                string zhUrl = "http://dl-aijiaxi.com/OAuthRedirectUrl/Index?reurl=baiximeIndex";         
                string urlPar = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx653820407bca1a42&redirect_uri="+ HttpUtility.UrlEncode(zhUrl) + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";

                button1.url = urlPar;            
      
                //主菜单2
                button button2 = new button();
                button2.name = "我要加盟";

                sub_button sb1_0 = new sub_button();
                sb1_0.type = "view";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                sb1_0.name = "我的二维码";
                string sb1_0Url = "http://dl-aijiaxi.com/OAuthRedirectUrl/Index?reurl=ErCodeIndex";
                string sb1_0Par = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx653820407bca1a42&redirect_uri=" + HttpUtility.UrlEncode(sb1_0Url) + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                sb1_0.url = sb1_0Par;


                sub_button sb2_1 = new sub_button();
                sb2_1.type = "click";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                sb2_1.name = "下载APP";
                sb2_1.key = "OnDownApp";

                sub_button sb1_1 = new sub_button();
                sb1_1.type = "view";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                sb1_1.name = "我要加盟";
                string sb1_Url = "http://dl-aijiaxi.com/OAuthRedirectUrl/Index?reurl=FrontPageJoin";
                string sb1_Par = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx653820407bca1a42&redirect_uri=" + HttpUtility.UrlEncode(sb1_Url) + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                sb1_1.url = sb1_Par;

          
                

                ICollection<sub_button> sub_buttonLi = new List<sub_button>();        
                sub_buttonLi.Add(sb1_1);
                sub_buttonLi.Add(sb2_1);
                sub_buttonLi.Add(sb1_0);
    
                button2.sub_button = sub_buttonLi;

                //主菜单3
                button button3 = new button();
                button3.name = "服务中心";

                sub_button sb3_1 = new sub_button();
                sb3_1.type = "view";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                sb3_1.name = "洗涤展示";
                string sb3_1_Url = "http://dl-aijiaxi.com/OAuthRedirectUrl/Index?reurl=FrontPageExhibition";
                string sb3_1_Par = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx653820407bca1a42&redirect_uri=" + HttpUtility.UrlEncode(sb3_1_Url) + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                sb3_1.url = sb3_1_Par;


                //sub_button sb1 = new sub_button();
                //sb1.type = "view";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                //sb1.name = "个人中心";
                //string sb1Url = "http://dl-aijiaxi.com/OAuthRedirectUrl/Index?reurl=FrontPageUserInfo";
                //string sb1urlPar = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx653820407bca1a42&redirect_uri=" + HttpUtility.UrlEncode(sb1Url) + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";

                //sb1.url = sb1urlPar;

                

                sub_button sb2 = new sub_button();
                sb2.type = "view";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                sb2.name = "我要抽奖";
                string sb2Url = "http://dl-aijiaxi.com/OAuthRedirectUrl/Index?reurl=FrontPageTurntable";
                string sb2urlPar = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx653820407bca1a42&redirect_uri=" + HttpUtility.UrlEncode(sb2Url) + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                sb2.url = sb2urlPar;


                sub_button sb3 = new sub_button();
                sb3.type = "view";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                sb3.name = "服务流程";
                string sb3Url = "http://dl-aijiaxi.com/OAuthRedirectUrl/Index?reurl=FrontPageServiceFlow";
                string sb3urlPar = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx653820407bca1a42&redirect_uri=" + HttpUtility.UrlEncode(sb3Url) + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                sb3.url = sb3urlPar;

                


                sub_button sb4 = new sub_button();
                sb4.type = "view";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                sb4.name = "合作伙伴";
                string sb4Url = "http://dl-aijiaxi.com/OAuthRedirectUrl/Index?reurl=FrontPagePartner";
                string sb4urlPar = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx653820407bca1a42&redirect_uri=" + HttpUtility.UrlEncode(sb4Url) + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                sb4.url = sb4urlPar;


                sub_button sb5 = new sub_button();
                sb5.type = "click";//用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
                sb5.name = "在线客服";
                sb5.key = "OnlineService";

                ICollection<sub_button> sub_buttonLi1 = new List<sub_button>();
                sub_buttonLi1.Add(sb3_1);
              //  sub_buttonLi1.Add(sb2);
                sub_buttonLi1.Add(sb3);
                sub_buttonLi1.Add(sb4);
                sub_buttonLi1.Add(sb5);

                button3.sub_button = sub_buttonLi1;


                ICollection <button> buttons= new List<button>();
                buttons.Add(button1);
                buttons.Add(button2);
                buttons.Add(button3);

                menu.button = buttons;

                var s = HttpHelper.PostJson<Menu>(url, menu);

                //主要使用里面的两个字段，{"errcode":0,"errmsg":"ok"}
                accessToken tr = JsonConvert.DeserializeObject<accessToken>(s);




               // var json= { "button":{ {"type":"view","name":"我要洗衣","key":null,"url":"http://www.soso.com/","sub_button":null},{"type":null,"name":"服务中心","key":null,"url":null,"sub_button":{ {"type":"view","name":"我要加盟","key":null,"url":"http://www.soso.com/","media_id":null},{"type":"view","name":"APP下载","key":null,"url":"http://www.soso.com/","media_id":null}]},{"type":null,"name":null,"key":null,"url":null,"sub_button":[{"type":"click","name":"在线客服","key":"OnlineService","url":"http://www.soso.com/","media_id":null},{"type":"click","name":"在线客服","key":"OnlineService","url":"http://www.soso.com/","media_id":null},{"type":null,"name":null,"key":null,"url":null,"media_id":null},{"type":null,"name":null,"key":null,"url":null,"media_id":null},{"type":null,"name":null,"key":null,"url":null,"media_id":null}}}}};

            }
            else
            {
                return JavaScript("layer.alert('access_token数据为空，请联系管理员！');");
            }

            return JavaScript("layer.alert('二维码已生成并成功保存二维码！',function(){location.href=window.location.href});");
        }


        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        private async Task<string> getAccessToken()
        {
            //取得access_token  MODEL
            HttpClient client = new HttpClient();
            var result = await client.GetStringAsync("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wx653820407bca1a42&secret=c17aa5110fdd94f5b48caf54fe354e67");
            var model = JsonConvert.DeserializeObject<accessToken>(result);
        
            return model.access_token;
        }

        /// <summary>
        /// 获取ticket
        /// </summary>
        /// <returns></returns>
        private TicketReturn getTicket(string access_token)
        {

            string url = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + access_token;
            TicketParam tp = new TicketParam();
            tp.action_name = "QR_LIMIT_STR_SCENE";

            scene sc = new scene();
            //登录用户信息
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);

            sc.scene_str = user.PhoneNumber;//推广参数

            action_info ai = new action_info();
            ai.scene = sc;

            tp.action_info = ai;


            var s = HttpHelper.PostJson<TicketParam>(url, tp);

            TicketReturn tr = JsonConvert.DeserializeObject<TicketReturn>(s);


            return tr;
        }

        /// <summary>
        /// 获取二维码图片
        /// </summary>
        /// <returns></returns>
        private async Task<string> getQrCode(string ticket)
        {
            //获取二维码图片
            HttpClient client = new HttpClient();
            string url = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + HttpUtility.UrlEncode(ticket);
            // var result = await client.GetStreamAsync(url);

            //var model = JsonConvert.DeserializeObject<accessToken>(result);

            //http://www.2cto.com/weixin/201501/367411.html


            string strpath = "";

            //获取二维码图片保存在本地
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

            req.Method = "GET";

            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                strpath = myResponse.ResponseUri.ToString();


                //以下为保存到本地
                WebClient mywebclient = new WebClient();
                try
                {
                    //mywebclient.DownloadFile(strpath, "d:\\1.png");
                }
                catch (Exception)
                {
                    throw new Exception("获取二维码图片失败！");
                }
            }



            //获取 二维码图片流

            //try
            //{

            //    Byte[] bytes = client.GetByteArrayAsync(url).Result;
            //    var v= new MemoryStream(bytes);


            //}
            //catch
            //{
            //    throw new Exception("获取二维码图片失败！");
            //}



            return strpath;
        }



    }
}