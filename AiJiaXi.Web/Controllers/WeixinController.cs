/*----------------------------------------------------------------
    Copyright (C) 2016 Senparc
    文件名：WeixinController.cs
    文件功能描述：用于处理微信回调的信息
    创建标识：Senparc - 20150312
----------------------------------------------------------------*/

using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Web.Mvc;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.MvcExtension;

namespace AiJiaXi.Web.Controllers
{
    using Common;
    using Domain.Entities.IdentityModel;
    using Domain.Entities.PromoterManager;
    using Domain.Repositories.Impl;
    using Domain.Repositories.Interface;
    using MessageHandle;
    using Newtonsoft.Json.Linq;
    using Senparc.Weixin.Context;   // using Senparc.Weixin.MP.Sample.CustomMessageHandler;
    using Senparc.Weixin.Entities;
    using Senparc.Weixin.MP.Entities;
    using Senparc.Weixin.MP.MessageHandlers;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Xml.Linq;
    public partial class WeixinController : Controller
    {
        //所有事件接收的网址:http://www.cnblogs.com/fengwenit/p/4527059.html

        //public static readonly string Token = WebConfigurationManager.AppSettings["WeixinToken"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        //public static readonly string EncodingAESKey = WebConfigurationManager.AppSettings["WeixinEncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        //public static readonly string AppId = WebConfigurationManager.AppSettings["WeixinAppId"];//与微信公众账号后台的AppId设置保持一致，区分大小写。


        public static readonly string Token = "baiximo";//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = "FXBVrzyEEqAbLFpcnBB23nAtiCJQfrYrsEe9S7FGXhR";//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = "wx653820407bca1a42";//与微信公众账号后台的AppId设置保持一致，区分大小写。


        readonly Func<string> _getRandomFileName = () => DateTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

        public WeixinController()
        {

        }


        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://sdk.weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Get(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {


                return Content("failed:" + postModel.Signature + "," + Senparc.Weixin.MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }


        /// <summary>
        /// 返回MODEL
        /// </summary>
        public class returnModel
        {

            public string ToUserName { get; set; }


            public string FromUserName { get; set; }


            public string CreateTime { get; set; }


            public string MsgType { get; set; }

            //跳转地址
            //public object sub_button { get; set; }

            //
            public string media_id { get; set; }
        }



        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Post(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = AppId;//根据自己后台的设置保持一致

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = -1;

            var logPath = Server.MapPath(string.Format("~/App_Data/MP/{0}/", DateTime.Now.ToString("yyyy-MM-dd")));
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.InputStream, maxRecordCount);



            try
            {
                //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                messageHandler.RequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                if (messageHandler.UsingEcryptMessage)
                {
                    messageHandler.EcryptRequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_Ecrypt_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                }

                /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                messageHandler.OmitRepeatedMessage = true;


                //执行微信处理过程
                messageHandler.Execute();

                //测试时可开启，帮助跟踪数据

                //if (messageHandler.ResponseDocument == null)
                //{
                //    throw new Exception(messageHandler.RequestDocument.ToString());
                //}

                if (messageHandler.ResponseDocument != null)
                {
                    messageHandler.ResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                }

                if (messageHandler.UsingEcryptMessage)
                {
                    //记录加密后的响应信息
                    messageHandler.FinalResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_Final_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                }


                //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                //return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
                return new WeixinResult(messageHandler);//v0.8+
            }
            catch (Exception ex)
            {
                using (TextWriter tw = new StreamWriter(Server.MapPath("~/App_Data/Error_" + _getRandomFileName() + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.StackTrace);
                    }

                    tw.Flush();
                    tw.Close();
                }
                return Content("");
            }
        }

        /**
* 生成消息转发到多客服  构造xml发起客服请求,触发客服可以使用自定义菜单或者关键字
* @param touser
* @param fromuser
* @return
*/
        public String CreateRelayCustomMsg(String touser, String fromuser)
        {
            StringBuilder relayCustomMsg = new StringBuilder();
            relayCustomMsg.Append("<xml>");
            relayCustomMsg.Append("<ToUserName><![CDATA[" + touser + "]]></ToUserName>");
            relayCustomMsg.Append("<FromUserName><![CDATA[" + fromuser + "]]></FromUserName>");
            relayCustomMsg.Append("<CreateTime>1399197672</CreateTime>");
            relayCustomMsg.Append("<MsgType><![CDATA[transfer_customer_service]]></MsgType>");
            relayCustomMsg.Append("</xml>");
            return relayCustomMsg.ToString();
        }

        /// <summary>
        /// 最简化的处理流程（不加密）
        /// </summary>
        [HttpPost]
        [ActionName("MiniPost")]
        public ActionResult MiniPost(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                //return Content("参数错误！");//v0.7-
                return new WeixinResult("参数错误！");//v0.8+
            }

            postModel.Token = Token;
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = AppId;//根据自己后台的设置保持一致

            //var messageHandler = new CustomMessageHandler(Request.InputStream, postModel, 10);
            var messageHandler = new CustomMessageHandler(Request.InputStream, 10);
            messageHandler.Execute();//执行微信处理过程

            //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
            return new FixWeixinBugWeixinResult(messageHandler);//v0.8+
            return new WeixinResult(messageHandler);//v0.8+
        }

        /*
         * v0.3.0之前的原始Post方法见：WeixinController_OldPost.cs
         *
         * 注意：虽然这里提倡使用CustomerMessageHandler的方法，但是MessageHandler基类最终还是基于OldPost的判断逻辑，
         * 因此如果需要深入了解Senparc.Weixin.MP内部处理消息的机制，可以查看WeixinController_OldPost.cs中的OldPost方法。
         * 目前为止OldPost依然有效，依然可用于生产。
         */

        /// <summary>
        /// 为测试并发性能而建
        /// </summary>
        /// <returns></returns>
        public ActionResult ForTest()
        {
            //异步并发测试（提供给单元测试使用）
            DateTime begin = DateTime.Now;
            int t1, t2, t3;
            System.Threading.ThreadPool.GetAvailableThreads(out t1, out t3);
            System.Threading.ThreadPool.GetMaxThreads(out t2, out t3);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.5));
            DateTime end = DateTime.Now;
            var thread = System.Threading.Thread.CurrentThread;
            var result = string.Format("TId:{0}\tApp:{1}\tBegin:{2:mm:ss,ffff}\tEnd:{3:mm:ss,ffff}\tTPool：{4}",
                    thread.ManagedThreadId,
                    HttpContext.ApplicationInstance.GetHashCode(),
                    begin,
                    end,
                    t2 - t1
                    );
            return Content(result);
        }


    }

    //自定义消息
    public partial class CustomMessageHandler : MessageHandler<MessageContext<Senparc.Weixin.MP.Entities.IRequestMessageBase, Senparc.Weixin.MP.Entities.IResponseMessageBase>>


    {



        // private IRepository<PromoterInfo> _promoterInfoRepository;


        public CustomMessageHandler(Stream inputStream, int maxRecordCount = 0)

            : base(inputStream, null, maxRecordCount)

        {

            WeixinContext.ExpireMinutes = 3;

        }

        public override void OnExecuting()

        {

            //测试MessageContext.StorageData

            if (CurrentMessageContext.StorageData == null)

            {

                CurrentMessageContext.StorageData = 0;

            }

            base.OnExecuting();

        }

        public override void OnExecuted()

        {

            base.OnExecuted();

            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;

        }


        /// <summary>
        /// 处理菜单点击的事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override Senparc.Weixin.MP.Entities.IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)

        {

            // 预处理文字或事件类型请求。

            // 这个请求是一个比较特殊的请求，通常用于统一处理来自文字或菜单按钮的同一个执行逻辑，

            // 会在执行OnTextRequest或OnEventRequest之前触发，具有以下一些特征：

            // 1、如果返回null，则继续执行OnTextRequest或OnEventRequest

            // 2、如果返回不为null，则终止执行OnTextRequest或OnEventRequest，返回最终ResponseMessage

            // 3、如果是事件，则会将RequestMessageEvent自动转为RequestMessageText类型，其中RequestMessageText.Content就是RequestMessageEvent.EventKey


            if (requestMessage.Content == "OneClick")

            {

                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();

                strongResponseMessage.Content = "小白在线客服代表为您服务、请在本页面聊天栏输入问题即可！";

                return strongResponseMessage;

            }

            return null;//返回null，则继续执行OnTextRequest或OnEventRequest

        }


        /// <summary>
        /// 自定义菜单点击按钮事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override Senparc.Weixin.MP.Entities.IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)

        {

            Senparc.Weixin.MP.Entities.IResponseMessageBase reponseMessage = null;

            //菜单点击，需要跟创建菜单时的Key匹配

            switch (requestMessage.EventKey)
            {
                case "OnlineService"://在线客服
                    {
                        //这个过程实际已经在OnTextOrEventRequest中完成，这里不会执行到。
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                        reponseMessage = strongResponseMessage;
                        strongResponseMessage.Content = "小白在线客服代表为您服务、请在本页面聊天栏输入问题即可！";

                    }

                    break;
                case "OnDownApp"://OnDownApp
                    {
                        //这个过程实际已经在OnTextOrEventRequest中完成，这里不会执行到。
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                        reponseMessage = strongResponseMessage;
                        strongResponseMessage.Content = "该功能暂未开通！";

                    }

                    break;



                default:

                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();

                        strongResponseMessage.Content = "您点击了按钮，EventKey：" + requestMessage.EventKey;
                        reponseMessage = strongResponseMessage;

                    }
                    break;
            }

            return reponseMessage;
        }


        /// <summary>
        /// DefaultResponseMessage必须重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override Senparc.Weixin.MP.Entities.IResponseMessageBase DefaultResponseMessage(Senparc.Weixin.MP.Entities.IRequestMessageBase requestMessage)

        {


            //所有没有被处理的消息会默认返回这里的结果
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

            // var str = CreateRelayCustomMsg(requestMessage.ToUserName, requestMessage.FromUserName).ToString();


            // responseMessage.Content = str;
            responseMessage.Content = "欢迎关注白洗么！";
            //return responseMessage;




            //  responseMessage.

            //return rm;
            return responseMessage;
        }



        /// <summary>

        /// 订阅（关注）事件【首次点关注时事件】

        /// </summary>

        /// <returns></returns>

        public override Senparc.Weixin.MP.Entities.IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)

        {

            //通过扫描关注【首次关注】

            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            IRepository<PromoterInfo> _promoterInfoRepository = new Domain.Repositories.Impl.Repository<PromoterInfo>();
            string st = "";
            string phone = requestMessage.EventKey.Replace("qrscene_", "");

            NLogger.Debug("关注："+ phone);
            NLogger.Debug("关注OpenId：" + requestMessage.FromUserName);
            if (!String.IsNullOrEmpty(phone) && !String.IsNullOrEmpty(requestMessage.FromUserName))
            {
                string FriendsPhone = "";

                //系统用户表  取出用户手机号
                IRepository<ApplicationUser> _applicationUserRepository = new Repository<ApplicationUser>();
                var userModel = _applicationUserRepository.Find(t => t.OpenId == requestMessage.FromUserName);
                if (userModel != null)
                {
                    FriendsPhone = userModel.PhoneNumber;
                }

                NLogger.Debug("FriendsPhone：" + FriendsPhone);
                //得到推广表数据 
                //var isExists = _promoterInfoRepository.Exist(item => true && item.MyPhone == phone && item.FriendsWeiXinId == requestMessage.FromUserName);
                //判断该好友的微信ID是否存在，因为下线ID是唯一的，只能是一个人的
                var isExists = _promoterInfoRepository.Exist(item => true && (item.FriendsWeiXinId == requestMessage.FromUserName || (FriendsPhone!="" && item.FriendsPhone == FriendsPhone)));
                NLogger.Debug("是否存在：" + isExists.ToString());
                if (isExists)
                {

                    //  st = "您已经是对方好友";
                }
                else
                {

                    //判断上下线是否手机号都是自己，是自己则就不保存
                    if (FriendsPhone == phone)
                    {
                        NLogger.Debug("首次关注，手机号存在："+ phone);

                    }
                    else
                    {

                        PromoterInfo pi = new PromoterInfo();
                        pi.FriendsWeiXinId = requestMessage.FromUserName;//好友OPEN ID
                        pi.FollowDate = System.DateTime.Now.ToString();//关注时间
                        pi.MyPhone = phone;


                        pi.FriendsPhone = FriendsPhone;  //好友手机号
                        _promoterInfoRepository.Add(pi);
                        NLogger.Debug("推广员保存成功：" );

                        //  st = "您成功成为对方好友";
                    }

                }
            }
            //st = "欢迎您关注白洗么在线洗衣，进入我要洗衣点击右上角即可注册。";
            responseMessage.Content = ConfigurationManager.AppSettings["WelcomeWord"];
            return responseMessage;
            // return null;

        }


        /// <summary>
        /// 扫描带参数二维码事件【已关注，再次扫描产生事件】
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override Senparc.Weixin.MP.Entities.IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            return null;
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            try
            {
                IRepository<PromoterInfo> _promoterInfoRepository = new Domain.Repositories.Impl.Repository<PromoterInfo>();
                string st = "";
                //qrscene_后面就是二维码生成的参数
                string phone = requestMessage.EventKey.Replace("qrscene_", "");
                if (!String.IsNullOrEmpty(phone) && !String.IsNullOrEmpty(requestMessage.FromUserName))
                {
                    string FriendsPhone = "";

                    //系统用户表  取出用户手机号
                    IRepository<ApplicationUser> _applicationUserRepository = new Repository<ApplicationUser>();
                    var userModel = _applicationUserRepository.Find(t => t.OpenId == requestMessage.FromUserName);
                    if (userModel != null)
                    {
                        FriendsPhone = userModel.PhoneNumber;
                    }


                    NLogger.Debug("533：" + phone+":"+ requestMessage.FromUserName);
                    //得到推广表数据 
                    var isExists = _promoterInfoRepository.Exist(item => true  && (item.FriendsWeiXinId == requestMessage.FromUserName || (FriendsPhone != "" && item.FriendsPhone== FriendsPhone)));
                    if (isExists)
                    {

                         st = "您已经是对方好友";
                    }
                    else
                    {
             

                        //判断上下线是否手机号都是自己，是自己则就不保存
                        if (FriendsPhone == phone)
                        {
                            NLogger.Debug("二次关注，手机号存在：" + phone);
                            return null;
                        }

                        NLogger.Debug("553：");

                        PromoterInfo pi = new PromoterInfo();
                        pi.FriendsWeiXinId = requestMessage.FromUserName;//好友OPEN ID
                        pi.FollowDate = System.DateTime.Now.ToString();//关注时间
                        pi.MyPhone = phone;

                        pi.FriendsPhone = FriendsPhone;  //好友手机号
                        _promoterInfoRepository.Add(pi);
                        NLogger.Debug("562：");
                        // st = "您成功成为对方好友";
                    }
                }

              //  responseMessage.Content = st;
            }
            catch (Exception ex)
            {
                NLogger.Debug("异常："+ ex.ToString());
            }
            //  return responseMessage;

            return null;
        }

        /// <summary>
        /// 转入多客服
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override Senparc.Weixin.MP.Entities.IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)

        {
            //var responseMessage1 = CreateResponseMessage<ResponseMessageText>();

            ////输入你好转入多客服
            //if (requestMessage.Content == "你好")
            //{
            return this.CreateResponseMessage<ResponseMessageTransfer_Customer_Service>();
            //}
            //responseMessage1.Content = "你不好";
            //return responseMessage1;
        }

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override Senparc.Weixin.MP.Entities.IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)

        {


            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息

            //var responseMessage = CreateResponseMessage<ResponseMessageText>();

            // responseMessage.Content = "这里写什么都无所谓，比如：上帝爱你！";
            //return responseMessage;//这里也可以返回null（需要注意写日志时候null的问题）



            //WebClient client = new WebClient();
            //            string url = "ht tp:/ /maps.go ogle.c om/maps/a pi/geocode/xml?latlng=39.910093,116.403945&language=zh-CN&sensor=false";
            //            client.Encoding = Encoding.UTF8;
            //            string responseTest = client.DownloadString(url);

            // Console.Write("{0}", responseTest);

            // Console.Read();

            //取得地址  MODEL

            //NLogger.Debug("FromUserName:" + requestMessage.CreateTime);
            //NLogger.Debug("FromUserName:" + requestMessage.FromUserName);
            //NLogger.Debug("Event:" + requestMessage.Event);
            //NLogger.Debug("Latitude:" + requestMessage.Latitude);
            //NLogger.Debug("Longitude:" + requestMessage.Longitude);
            //NLogger.Debug("Precision:" + requestMessage.Precision);


            #region 获取百度地图数据
            string url = "http://api.map.baidu.com/geocoder/v2/?ak=F83b9a554233a04b78240f7744bf94ca&callback=renderReverse&location="+ requestMessage.Latitude + ","+ requestMessage.Longitude + "&output=json&pois=1";

            NLogger.Debug("百度地图：" + url);
            HttpClient client = new HttpClient();            
            string result = client.GetStringAsync(url).Result;
            string res = result.Replace("renderReverse&&renderReverse(", "").Replace(")", "");
            // var json = JsonConvert.DeserializeObject(result);

            //获取URL解析完的JSON
            JObject jo = JObject.Parse(res);
            var j = jo.GetValue("result");
            //获取到上层JSON中的属性
            JObject jo1 = JObject.Parse(j.ToString());
            var j1 = jo1.GetValue("addressComponent");
            //获取到上层JSON中的属性
            JObject jvalue = JObject.Parse(j1.ToString());

            //已经得到最终的属性了，需要去括号。。
            var ProvinceName = jvalue.GetValue("province").ToString().Replace("{", "").Replace("}", "");//省份名
            var CityName = jvalue.GetValue("city").ToString().Replace("{", "").Replace("}", "");//城市名
            var CountyName = jvalue.GetValue("district").ToString().Replace("{", "").Replace("}", "");//区域名

            //string[] values = jo.Properties().Select(item => item.Value.ToString()).ToArray();            
            // var model = JsonConvert.DeserializeObject<BaiDuMap>(result);
            #endregion


            //获得用户当前位置
            IRepository<UserLocation> _userLocationRepository = new Domain.Repositories.Impl.Repository<UserLocation>();
            //得到定位表 
            var isExists = _userLocationRepository.Find(item => true && item.FromUserName == requestMessage.FromUserName);
            if (isExists!=null)
            {
                //第一个判断因为字段是后台的，为了赋值，所以加上的，判断当前城市和表中城市是否一致，如果不一致则重新定位，为的是只能选择当前城市的区域 或者  随便找个没用的字段，如果999，代表用户已经手动选择过地区，则不需要定位了
                if (String.IsNullOrWhiteSpace(isExists.LocationCityName) || CityName != isExists.LocationCityName || isExists.ToUserName != "999")
                {
                    isExists.ToUserName = requestMessage.ToUserName;
                    isExists.CreateTime = requestMessage.CreateTime;
                    isExists.Latitude = requestMessage.Latitude;
                    isExists.Longitude = requestMessage.Longitude;
                    isExists.Precision = requestMessage.Precision;
                    isExists.ProvinceName = ProvinceName;
                    isExists.CityName = CityName;
                    isExists.CountyName = CountyName;
                    isExists.LocationProvinceName = ProvinceName;
                    isExists.LocationCityName = CityName;
                    isExists.LocationCountyName = CountyName;
                    _userLocationRepository.Update(isExists);


                }
               

          

                NLogger.Debug("---------定位----------------");
                NLogger.Debug("CityName:" + CityName);
                NLogger.Debug("isExists.CityName:" + isExists.CityName);
                NLogger.Debug("isExists.ToUserName:" + isExists.ToUserName);
                NLogger.Debug("-------------------------");

            }
            else
            {

                //保存数据
                UserLocation ul = new UserLocation();
                ul.FromUserName = requestMessage.FromUserName;//OpenId
                ul.ToUserName = requestMessage.ToUserName;
                ul.CreateTime = requestMessage.CreateTime;
                ul.Latitude = requestMessage.Latitude;
                ul.Longitude = requestMessage.Longitude;
                ul.Precision = requestMessage.Precision;
                ul.ProvinceName = ProvinceName;
                ul.CityName = CityName;
                ul.CountyName = CountyName;
                ul.LocationProvinceName = ProvinceName;
                ul.LocationCityName = CityName;
                ul.LocationCountyName = CountyName;

                _userLocationRepository.Add(ul);

            }



            return null;
        }

    }


}