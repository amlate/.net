using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Plugin;
using Project.Plugin.WeiXin;
using Project.Domain;
using Project.Domain.Entities;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.Location;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;
using ZhiYuan.IAR.Repository.EF;
using Project.Common.Helpers;
using Project.Domain.Entities.PromoterManager;
using Project.Domain.Entities.Configs;
using Project.Web.Provider;
using Microsoft.AspNet.Identity;
using Project.Common;

namespace Project.Web.Controllers
{
    public class FrontPageController : Controller
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<EventAward> _eventAwardRepository;
        private readonly IRepository<County> _countyRepository;

        private readonly IRepository<Agency> _agencyRepository;

        private readonly IRepository<ApplicationUser> _applicationRepository;

        private readonly IRepository<UserAccount> _useraccountRepository;

        private readonly IRepository<Voucher> _voucheRepository;

        private readonly IRepository<UserAddress> _useraddressRepository;

        private readonly IRepository<AccountRecord> _accountrecordRepository;

        private readonly IRepository<Order> _ordeRepository;

        private readonly IRepository<OrderRate> _orderRateRepository;

        private ApplicationUserManager _userManager;

        private readonly IRepository<BizPartner> _bizPartneRepository;


        //private ApplicationUserManager _userManager;//用户信息   
        private IRepository<Withdrawals> _withdrawalsRepository;//提现申请   
        private Repository<SmsConfig> _smsConfigRepository; //发送短信
        private IRepository<PromoterInfo> _promoterInfoRepository;//推广信息

        //获得用户当前位置
        private readonly IRepository<UserLocation> _userLocationRepository;
        public FrontPageController(IRepository<UserLocation> userLocationRepository, ApplicationUserManager userManager, Repository<SmsConfig> smsConfigRepository, IRepository<PromoterInfo> promoterInfoRepository, IRepository<Withdrawals> withdrawalsRepository, IRepository<City> cityRepository, IRepository<Agency> agencyRepository, IRepository<ApplicationUser> applicationRepository, IRepository<UserAccount> useraccountRepository, IRepository<Voucher> voucheRepository, IRepository<UserAddress> useraddressRepository, IRepository<County> countyRepository, IRepository<AccountRecord> accountrecordRepository, IRepository<Order> ordeRepository, IRepository<OrderRate> orderRateRepository, IRepository<BizPartner> bizPartneRepository, IRepository<Event> eventRepository, IRepository<EventAward> eventAwardRepository)
        {
            _cityRepository = cityRepository;
            _agencyRepository = agencyRepository;
            _applicationRepository = applicationRepository;
            _useraccountRepository = useraccountRepository;
            _voucheRepository = voucheRepository;
            _useraddressRepository = useraddressRepository;
            _countyRepository = countyRepository;
            _accountrecordRepository = accountrecordRepository;
            _ordeRepository = ordeRepository;
            _orderRateRepository = orderRateRepository;
            _bizPartneRepository = bizPartneRepository;
            _eventRepository = eventRepository;
            _eventAwardRepository = eventAwardRepository;
            _smsConfigRepository = smsConfigRepository;
            _promoterInfoRepository = promoterInfoRepository;
            _withdrawalsRepository = withdrawalsRepository;
            _userManager = userManager;
            _userLocationRepository = userLocationRepository;
        }

        /// <summary>
        /// 添加地址
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> AddAddress(string openid)
        {
            var findUser = await _applicationRepository.FindAsync(a => a.OpenId == openid);
            if (findUser == null)
            {
                return null;
            }
            ViewBag.AddressList = _useraddressRepository.FindList<UserAddress>(u => u.ApplicationUserId == findUser.Id && u.IsShow);
            List<int> cityidList = _agencyRepository.FindList<int>(a => a.IsValid).Select(a => a.CityId).ToList();
            var cityList = _cityRepository.FindList<City>(c => cityidList.Contains(c.Id)).ToList();
            ViewBag.CityList = cityList;
            int cityListCount = cityList.Count();
            if (cityListCount > 0)
            {
                int cityid = cityList[0].Id;
                ViewBag.CountyList = _countyRepository.FindList<County>(c => c.Id > cityid && c.Id < (cityid + 100));
            }
            return View(findUser);
        }

        /// <summary>
        /// 优惠券（洗涤券）
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Coupon(string openid)
        {
            var findUser = await UserUnknown(openid);
            if (findUser == null)
            {
                return null;
            }
            ViewBag.voucherList = _voucheRepository.FindList<Voucher>(v => v.UserAccountId == findUser.Id && v.EndTime > DateTime.Now && v.IsUsed == false);
            return View(findUser);
        }

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> OrderList(string openid, string type, string orderId, string countyId)
        {
            ViewBag.CountyId = countyId;
            ViewBag.orderId = orderId;
            ViewBag.FromType = type;
            var findUseraccount = await UserUnknown(openid);
            if (findUseraccount == null)
            {
                return null;
            }

            ViewBag.orderList = _ordeRepository.FindList<Order>(o => o.OrderStatus != OrderStatus.ToConfirm && o.ApplicationUserId == findUseraccount.Id, "RiseTime", false);

            return View(findUseraccount);
        }


        /// <summary>
        /// 寻找UserAccount是否存在
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        internal async Task<UserAccount> UserUnknown(string openid)
        {
            var findUseraccount = await _useraccountRepository.FindAsync(u => u.OpenId == openid);

            return findUseraccount;
        }

        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="orderNo">订单编号</param>
        /// <param name="voucherId">优惠券表Id</param>
        /// <returns></returns>
        public async Task<ActionResult> Pay(string openid, string orderNo, string voucherId, string countyId)
        {
            ViewBag.CountyId = countyId;
            var finduser = await UserUnknown(openid);
            if (finduser == null)
            {
                return null;
            }
            ViewBag.user = finduser;
            var findOrder = await _ordeRepository.FindAsync(o => o.OrderStatus != OrderStatus.ToConfirm && o.OrderNo == orderNo && o.OrderStatus != OrderStatus.Cancelled);
            if (findOrder == null)
            {
                return null;
            }
            //LC追加
            else
            {
                decimal couponJe = 0;
                //优惠券表         
                var voucherModel = _voucheRepository.Find(t => t.Id.ToString() == voucherId);
                if (voucherModel != null)
                {
                    couponJe = voucherModel.Amount;//优惠券金额
                    findOrder.VoucherId = Guid.Parse(voucherId);//优惠券主键ID
                }
                else
                {
                    findOrder.VoucherId = null;
                }


                //判断优惠券不为空则将实际支付金额修改成这个               
                findOrder.Fact = findOrder.TotalPrice + findOrder.Freight - couponJe;
                //优惠券计算结束后，去相应的地区查找是否有满减活动，同样的要求有1、满减，2起止日期，3flag，5审核状态，6最大使用数量
                // todo 杨瑞 测试并查看有无必要修改这里逻辑

                List<Event> eventList = _eventRepository.FindList<Event>(
                      e =>
                          e.Agencies.Select(m => m.Id).Contains(findOrder.AgencyId) && e.EventType == EventType.Benefit &&
                          e.StartTime < DateTime.Now && e.EndTime > DateTime.Now && e.Flag &&
                          e.ApplyStatus == ApplyStatus.Reviewed).OrderByDescending(e => e.PriceTo).ToList();
                foreach (var item in eventList)
                {
                    if (findOrder.Fact >= item.PriceTo)
                    {
                        //查找是否满足优惠条件次数，如果满足则将Fact的金额减去优惠的金额
                        int resultCount = _ordeRepository.FindList<Order>(
                              o => o.ApplicationUserId == findOrder.ApplicationUserId && o.Events == item.Id.ToString() && o.OrderStatus != OrderStatus.Cancelled && o.OrderStatus != OrderStatus.Refundding && o.OrderStatus != OrderStatus.Refunded && o.OrderStatus != OrderStatus.ToConfirm && o.OrderStatus != OrderStatus.ToPay && o.OrderStatus != OrderStatus.ToRefund)
                              .Count();
                        //此处默认约定，如果为0不限制次数
                        if (resultCount < item.Nums || item.Nums == 0)
                        {
                            //支付金额如果为0，那么微信支付会有问题的
                            findOrder.Fact = findOrder.Fact - item.BenefitPrice;
                            break;
                        }
                    }
                }
                _ordeRepository.Update(findOrder);
            }
            return View(findOrder);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Recharge(string openid, string countyId)
        {
            ViewBag.countyId = countyId;
            var findUser = await _applicationRepository.FindAsync(a => a.OpenId == openid);
            if (findUser == null)
            {
                return null;
            }
            var findUseraccount = await _useraccountRepository.FindAsync(u => u.Id == findUser.Id);
            if (findUseraccount == null)
            {
                return null;
            }
            ViewBag.ConsumeList = _accountrecordRepository.FindList<AccountRecord>(a => a.UserAccountId == findUseraccount.Id && a.ResultType == ResultType.Succeedd);
            return View(findUseraccount);
        }

        /// <summary>
        ///  用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="cityName">城市名称</param>
        /// <param name="countyName">地区名称</param>
        /// <returns></returns>
        public async Task<ActionResult> UserInfo(string openId, string cityName, string countyName)
        {

            //获取OpenId用于查询条件用【注意】      
            ViewBag.OpenId = openId;

            #region LC增加以下内容
            long cityId = 0;
            long countyId = 0;


            if (!String.IsNullOrWhiteSpace(cityName) && !String.IsNullOrWhiteSpace(countyName))
            {
                //城市信息
                var findCityT = await _cityRepository.FindAsync(c => c.Name.Contains(cityName));
                if (findCityT != null)
                {
                    cityId = findCityT.Id;
                }
                //区域信息
                var findCountyT = await _countyRepository.FindAsync(c => c.Name.Contains(countyName));
                if (findCountyT != null)
                {
                    countyId = findCountyT.Id;
                }
                ViewBag.cityName = cityName;
                ViewBag.countyName = countyName;
            }
            else
            {
                ViewBag.cityName = "定位中";
                ViewBag.countyName = "定位中";
            }
            ViewBag.cityId = cityId.ToString();//城市ID
            ViewBag.countyId = countyId.ToString();//区域ID

            #endregion



            //如果为空，那个提示用正确的方式打开
            var findUser = await _applicationRepository.FindAsync(a => a.OpenId == openId);
            if (findUser == null)
            {
                //为空代表没有登录
                ViewBag.login = "";
                return Redirect("/Baixime/index");
            }

            ViewBag.user = await _useraccountRepository.FindAsync(u => u.Id == findUser.Id);
            if (ViewBag.user == null)
            {
                //出现异常
                //为空代表没有登录
                ViewBag.login = "";
                return Redirect("/Baixime/index");
            }


            ViewBag.VoucherCount = _voucheRepository.FindList<Voucher>(v => v.UserAccountId == findUser.Id && !v.IsUsed & v.EndTime > DateTime.Now && v.StartTime < DateTime.Now).Count();


            ViewBag.awardList = _eventAwardRepository.FindList<EventAward>(e => e.UserId == findUser.Id);
            //服务范围[当前界面选择的城市所有区域服务范围]
            ViewBag.Range = _agencyRepository.FindList<Agency>(t => t.IsValid && t.CityId == cityId);

            //评价信息[显示所有审核通过的，或者把自己没通过的也显示出来]
            ViewBag.OrderRate = _orderRateRepository.FindList<OrderRate>(a => a.IsApproval == true || a.Order.ApplicationUser.Id == findUser.Id, "RiseTime", false);

            //设置为1，代表已经登录
            ViewBag.login = "1";
            return View(findUser);
        }

        /// <summary>
        /// 当用户不用微信浏览器打开时跳转到这里
        /// </summary>
        /// <returns></returns>
        public ActionResult IsNotWeiXin()
        {
            return View();
        }

        private bool WeiXinVerification()
        {
            return HttpContext.Request.UserAgent != null && HttpContext.Request.UserAgent.ToLower().Contains("micromessenger");
        }

        /// <summary>
        /// 更换手机[原手机号]
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ReplaceTel(string openid)
        {
            var findUser = await _applicationRepository.FindAsync(a => a.OpenId == openid);
            //if (findUser == null)
            //{
            //    return null;
            //}         
            return View(findUser);
        }

        /// <summary>
        /// 更换手机[新手机号]
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ReplaceTelNew(string openid, string oldPhone)
        {
            var findUser = await _applicationRepository.FindAsync(a => a.OpenId == openid);
            //if (findUser == null)
            //{
            //    return null;
            //}
            ViewBag.oldPhone = oldPhone;

            return View(findUser);
        }

        //代理商您好，您有一笔新的订单需要处理，订单信息如下：联系人：@，联系方式：@，地址：@【白洗么】
        //http://114.113.154.5:8080/Home/Index
        /// <summary>
        /// 手机验证码
        /// </summary>
        /// <returns>mobile 手机号</returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> GetCode(string mobile, string token)
        {

            // 发送手机验证码
            string code = SecurityHelper.GenPhoneCode();
            this.TempData["PhoneCode"] = code;
            var config = await _smsConfigRepository.FindAsync(item => true);
            string state = "0";//成功

            if (token == "baiximo@123456")
            {
                var result = await SmsHelper.Send(config, string.Format("您的手机验证码为：{0}。工作人员不会向您索要，索要验证码的可能是骗子，如果非本人操作请忽略。 【白洗么】", code), true, mobile);
                if (!result)
                {
                    // todo 验证码发送失败处理代码
                    state = "-1";//失败
                                 // return "手机验证码发送失败，请稍后再试！";
                }
            }
            else
            {
                state = "-2";//认证失败
            }

            return Json(new { state = state, code = code, mobile = mobile }, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 换绑手机号
        /// </summary>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ReplacePhone(string OldPhone, string NewPhone)
        {
            string state = "0";//成功
            string message = "更换手机号成功！";
            //更新用户表中手机号[用户，或者推广员的情况下更新]
            var userModel = _applicationRepository.Find(t => t.PhoneNumber == OldPhone && (t.UserType == Domain.Enums.UserType.User || t.UserType == Domain.Enums.UserType.Withdrawals));
            if (userModel != null)
            {
                // userModel.PhoneNumber = NewPhone;             
                var result = _applicationRepository.Update(t => t.PhoneNumber == OldPhone && (t.UserType == Domain.Enums.UserType.User || t.UserType == Domain.Enums.UserType.Withdrawals), u => new ApplicationUser() { PhoneNumber = NewPhone });


                //var result = await _applicationRepository.UpdateAsync(userModel);
                if (result <= 0)
                {
                    state = "-1";
                    message = "更新用户表手机号失败！";
                    return Json(new { state = state, message = message }, JsonRequestBehavior.DenyGet);
                }

            }

            //更新推广信息表中，推广人手机号
            var proModel = _promoterInfoRepository.Find(t => t.MyPhone == OldPhone);
            if (proModel != null)
            {
                // proModel.MyPhone = NewPhone;
                var result = _promoterInfoRepository.Update(t => t.MyPhone == OldPhone, k => new PromoterInfo { MyPhone = NewPhone });
                if (result <= 0)
                {
                    state = "-1";
                    message = "更新推广信息我的手机号失败！";
                    return Json(new { state = state, message = message }, JsonRequestBehavior.DenyGet);

                }

            }

            //更新推广信息表中，好友手机号
            var proModel1 = _promoterInfoRepository.Find(t => t.FriendsPhone == OldPhone);
            if (proModel1 != null)
            {
                //  proModel1.FriendsPhone = NewPhone;
                var result = _promoterInfoRepository.Update(t => t.FriendsPhone == OldPhone, k => new PromoterInfo { FriendsPhone = NewPhone });
                if (result <= 0)
                {
                    state = "-1";
                    message = "更新推广信息好友手机号失败！";
                    return Json(new { state = state, message = message }, JsonRequestBehavior.DenyGet);

                }

            }

            //更新推提现信息中，提现人手机号
            var withModel = _withdrawalsRepository.Find(t => t.Phone == OldPhone);
            if (withModel != null)
            {
                //withModel.Phone = NewPhone;
                var result = _withdrawalsRepository.Update(t => t.Phone == OldPhone, k => new Withdrawals { Phone = NewPhone });
                if (result <= 0)
                {
                    state = "-1";
                    message = "更新提现信息我的手机号失败！";
                    return Json(new { state = state, message = message }, JsonRequestBehavior.DenyGet);

                }

            }

            return Json(new { state = state, message = message }, JsonRequestBehavior.DenyGet);
            //return JavaScript("layer.alert('操作成功！',function(){location.href=window.location.href});");
            // return RedirectToAction("Index");
        }

        /// <summary>
        /// 加盟
        /// </summary>
        /// <returns></returns>
        public ActionResult Join()
        {
            return View();
        }

        /// <summary>
        /// 订单评价
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public async Task<ActionResult> OrderGrade(string openid, string orderNo)
        {
            var useraccount = await UserUnknown(openid);
            if (useraccount == null)
            {
                return null;
            }
            ViewBag.userInfo = useraccount;
            var findOrder = await _ordeRepository.FindAsync(o => o.OrderNo == orderNo);
            if (findOrder == null)
            {
                return null;
            }
            return View(findOrder);
        }

        [HttpPost]
        public async Task<ActionResult> OrderGrade()
        {
            string openid = Request.Form["openid"];
            string orderid = Request.Form["orderid"];
            string content = Request.Form["content"];
            float starts = Convert.ToInt32(Request.Form["starts"]);
            var findOrder = await _ordeRepository.FindAsync(o => o.OrderNo == orderid);
            if (findOrder == null)
            {
                return null;
            }
            var findUser = await UserUnknown(openid);
            if (findUser == null)
            {
                return null;
            }
            IList<HttpPostedFileBase> fileList = Request.Files.GetMultiple("pic");
            string fileUrl = string.Empty;
            List<string> pictypeList = new List<string> { "image/jpeg", "image/jpg", "image/png", "image/bmp" };
            foreach (var item in fileList)
            {
                if (item.ContentLength != 0)
                {
                    if (!pictypeList.Contains(item.ContentType))
                    {
                        return Redirect("/frontpage/OrderGrade?openid=" + findUser.ApplicationUser.OpenId + "&orderNo=" + orderid); ;
                    }
                    fileUrl += "/Upload/" + FileUploadHelper.ProcessUpload(item, Server.MapPath("~/Upload")) + ",";
                }
            }
            OrderRate orderRate = new OrderRate
            {
                OrderComment = content,
                Stars = starts,
                ShareOrderImgUrls = fileUrl,
                RiseTime = DateTime.Now,
                IsApproval = false,
                Order = findOrder
            };
            var findRate = await _orderRateRepository.AddAsync(orderRate);
            if (findRate != null)
            {
                findOrder.OrderRateId = orderRate.Id;
                findOrder.OrderRate = orderRate;
                await _ordeRepository.UpdateAsync(findOrder);
                return Redirect("/frontpage/orderlist?openid=" + findUser.ApplicationUser.OpenId + "&type=0");
            }
            return Redirect("/frontpage/OrderGrade?openid=" + findUser.ApplicationUser.OpenId + "orderNo&=" + orderid); ;
        }


        /// <summary>
        /// 公众号会员注册
        /// </summary>
        /// <param name="openId">微信ID</param>
        /// <param name="Phone">手机号</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddUser(string OpenId, string Phone)
        {
            //return View(new ApplicationUser() { UserType = UserType.User });






            // string id, string role, FormCollection collection

            //var userId = User.Identity.GetUserId();
            //var user = this._userManager.FindById(userId);
            //if (user.UserType != UserType.Admin)
            //{
            //    throw new Exception("无相应权限");
            //}

            string state = "0";//成功





            //var entity = await this._userManager.FindByIdAsync(id);

            //根据OpenId或手机号  判断是否存在数据，，都是唯一的
            var entity = await this._applicationRepository.FindAsync(t => (t.OpenId == OpenId || t.PhoneNumber == Phone) && t.UserType == UserType.User);
            if (entity == null)
            {
                NLogger.Trace("空用户");

                //根据OpenId更新推广员手机号
                var proModel = _promoterInfoRepository.Find(t => true && t.FriendsWeiXinId == OpenId);
                if (proModel != null)
                {
                    //如果上线手机号和下线一致则返回，因为上下线不可成为好友
                    if (proModel.MyPhone == Phone)
                    {
                        //如果上下线相等则删除该关注
                        _promoterInfoRepository.Delete(proModel);
                    }
                    else  //更新该OpenId的手机号
                    {
                        var resu = _promoterInfoRepository.Update(t => t.FriendsWeiXinId == OpenId, u => new PromoterInfo() { FriendsPhone = Phone });
                        NLogger.Trace("resu：" + resu);
                        if (resu <= 0)
                        {
                            //失败
                        }
                    }

                }



                #region 生成会员卡号【不带4】
                var CardNumber = _useraccountRepository.FindList<UserAccount>(t => true).Max(k => k.CardNumber);
                if (!String.IsNullOrEmpty(CardNumber))
                {
                    //转换Int
                    int numb = Convert.ToInt32(CardNumber) + 1;
                    //不满10位前补0   如果有4，则换成5
                    CardNumber = numb.ToString().PadLeft(10, '0').Replace("4", "5");

                }
                else
                {
                    CardNumber = "0000010000";
                }
                string CardNumberNew = getKey(CardNumber);
                NLogger.Debug("CardNumberNew：" + CardNumberNew);
                #endregion

                entity = new ApplicationUser();
                //if (this.TryUpdateModel(entity, "", collection.AllKeys, new string[] { "UserType" }))
                //{

                entity.OpenId = OpenId;//微信ID 
                entity.PhoneNumber = Phone;//手机号码


                entity.RealName = Phone;//真实姓名

                entity.UserType = UserType.User;
                entity.IsFrozen = false;
                entity.AddTime = DateTime.Now;
                entity.EmailConfirmed = true;
                entity.PhoneNumberConfirmed = true;

                UserAccount ua = new UserAccount();
                ua.CardNumber = CardNumberNew;//会员卡号
                ua.OpenId = OpenId;
                entity.UserAccount = ua;


                entity.UserName = Phone + CardNumberNew;//用户姓名

                NLogger.Debug("entity.UserAccount：");
                // todo 初始密码移动到配置文件
                var result = await _userManager.CreateAsync(entity, "baixime@2015888");

                string sss = "";
                if (result.Errors != null)
                {
                    foreach (var ii in result.Errors)
                    {
                        sss += ii + "|";
                    }
                }
                NLogger.Debug("保存错误：" + sss);
                if (result.Succeeded)
                {
                    state = "0";
                }
                else
                {
                    state = "1";
                }
                //}
                //注册结束后去userLocation表中查找当前城市，，如果有的话将当前城市的
                //所有包含活动的区都绑定一张优惠券给注册用户
                try
                {
                    //IRepository<UserLocation> userLocationRepository = new Repository<UserLocation>();
                    var userLocation = await _userLocationRepository.FindAsync(u => u.FromUserName == OpenId);
                    if (userLocation != null)
                    {
                        // 判断当前城市是否有注册赠送优惠券的活动
                        //var findCity = await _cityRepository.FindAsync(c => c.Name == userLocation.LocationCityName);
                        var findCounty = await _countyRepository.FindAsync(c => c.Name == userLocation.LocationCountyName);
                        if (findCounty != null)
                        {
                            var findAgency = await _agencyRepository.FindAsync(a => a.CountyId == findCounty.Id);

                            if (findAgency != null)
                            {
                                var findEvent = await _eventRepository.FindAsync(
      e => e.Flag && e.EventType == EventType.Register && e.ApplyStatus == ApplyStatus.Reviewed && e.StartTime < DateTime.Now && e.EndTime > DateTime.Now && e.Agencies.Select(m => m.Id).Contains(findAgency.Id));
                                if (findEvent != null)
                                {//如果包含相应的活动，那么去优惠券里查询相应的活动，找对应的id,useraccountid为空的一个分配给注册者
                                 //IRepository<Voucher> voucheRepository = new Repository<Voucher>();
                                    var findVoucher = await _voucheRepository.FindAsync(
                                         v => v.EventId == findEvent.Id && string.IsNullOrEmpty(v.UserAccountId) && v.AgencyId == findAgency.Id);
                                    if (findVoucher != null)
                                    {
                                        // findVoucher.UserAccount = ua;
                                        findVoucher.UserAccountId = ua.Id;
                                        findVoucher.IsOccu = true;
                                        await _voucheRepository.UpdateAsync(findVoucher);
                                    }
                                }
                            }
                        }

                        #region 原始注册

                        //if (findCity != null)
                        //{
                        //    //IRepository<County> countyRepository = new Repository<County>();
                        //    //获取当前城市的所有区,通过区域去查询代理商，看代理商们是否有对应的活动，
                        //    var countyIdList = _countyRepository.FindList<int>(c => c.Id - findCity.Id < 100).Select(c => c.Id);
                        //    //IRepository<Agency> _agencyRepository = new Repository<Agency>();
                        //    var agencyList = _agencyRepository.FindList<Agency>(a => countyIdList.Contains(a.CountyId)).ToList();
                        //    foreach (var item in agencyList)
                        //    {
                        //        //IRepository<Event> eventRepository = new Repository<Event>();
                        //        var findEvent = await _eventRepository.FindAsync(
                        //              e => e.Flag && e.EventType == EventType.Register && e.ApplyStatus == ApplyStatus.Reviewed&&e.StartTime<DateTime.Now&&e.EndTime>DateTime.Now && e.Agencies.Select(m => m.Id).Contains(item.Id));
                        //        if (findEvent != null)
                        //        {//如果包含相应的活动，那么去优惠券里查询相应的活动，找对应的id,useraccountid为空的一个分配给注册者
                        //            //IRepository<Voucher> voucheRepository = new Repository<Voucher>();
                        //            var findVoucher= await _voucheRepository.FindAsync(
                        //                 v => v.EventId == findEvent.Id && string.IsNullOrEmpty(v.UserAccountId)&&v.AgencyId==item.Id);
                        //            if (findVoucher!=null)
                        //            {
                        //               // findVoucher.UserAccount = ua;
                        //                findVoucher.UserAccountId = ua.Id;
                        //                findVoucher.IsOccu = true;
                        //               await _voucheRepository.UpdateAsync(findVoucher);
                        //            }
                        //        }
                        //    }

                        //}

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Debug("CardNumberNew：" + ex.ToString());
                }

            }
            else
            {
                state = "999";
                //if (this.TryUpdateModel(entity, "", collection.AllKeys, new string[] { "UserType" }))
                //{
                //    entity.UserType = UserType.User;
                //    var result = await _userManager.UpdateAsync(entity);
                //    if (result.Succeeded)
                //    {
                //    }
                //}
            }

            return Json(new { state = state, IsFrozen = entity.IsFrozen, OpenId = entity.OpenId, PhoneNumber = entity.PhoneNumber, UserId = entity.Id }, JsonRequestBehavior.AllowGet);
            //return View(entity);
        }

        /// <summary>
        /// 生成会员卡号
        /// </summary>
        /// <returns></returns>
        public string getKey(string carId)
        {
            string CardNumber = "";

            //判断传入的卡号是否存在
            var userAccount = _useraccountRepository.Find(a => a.CardNumber == carId);
            if (userAccount == null)
            {
                //不带4的情况直接返回
                //if (carId.IndexOf("4") == -1)//判断是否带4
                //{
                //    return carId;
                //}
                //else
                //{
                //    //不满10位前补0   如果有4，则换成5
                //    CardNumber = carId.Replace("4", "5");
                //    return getKey(CardNumber);
                //}
                return carId;

            }
            else
            {
                //转换Int
                int numb = Convert.ToInt32(carId) + 1;
                //不满10位前补0   如果有4，则换成5
                CardNumber = numb.ToString().PadLeft(10, '0').Replace("4", "5");
                return getKey(CardNumber);
            }

        }

        /// <summary>
        /// 服务流程
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ServiceFlow(string OpenId)
        {

            return View();
        }

        /// <summary>
        /// 大转盘
        /// </summary>
        /// <param name="openId">用户的openid</param>
        /// <param name="orderNo">订单</param>
        /// <returns></returns>
        public async Task<ActionResult> Turntable(string openId, string orderNo, string countyId)
        {
            ViewBag.openid = openId;
            ViewBag.orderNo = orderNo;
            int county = Convert.ToInt32(countyId);
            var findCounty = await _countyRepository.FindAsync(c => c.Id == county);
            if (findCounty == null) throw new ArgumentNullException(nameof(findCounty));
            ViewBag.countyName = findCounty.Name;
            var findCity = await _cityRepository.FindAsync(c => county - c.Id < 100);
            if (findCity == null) throw new ArgumentNullException(nameof(findCity));
            ViewBag.cityName = findCity.Name;
            return View();
        }

        /// <summary>
        /// 合作伙伴
        /// </summary>
        /// <returns></returns>
        public ActionResult Partner()
        {
            ViewBag.partnerList = _bizPartneRepository.FindList<BizPartner>(b => b.IsShow == true).ToList();
            return View();
        }

        /// <summary>
        /// 合作伙伴详细页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> PartnerDetail(long id)
        {
            var findPartner = await _bizPartneRepository.FindAsync(b => b.Id == id);
            return View(findPartner);
        }


        /// <summary>
        /// 专门为支付准备的一个空白页
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PayMoney()
        {
            ViewBag.countyId = string.IsNullOrEmpty(Request.Form["countyId"]) ? "" : Request.Form["countyId"];
            string openid = Request.Form["openid"];
            ViewBag.userOpenId = openid;
            var user = await _applicationRepository.FindAsync(a => a.OpenId == openid);
            if (user == null)
            {
                return null;
            }
            ViewBag.userId = user.Id;
            ViewBag.body = Request.Form["body"];
            ViewBag.totalFee = Request.Form["totalFee"];
            string orderSn = Request.Form["orderSn"];
            if (!string.IsNullOrEmpty(orderSn))
            {
                var findOrder = await _ordeRepository.FindAsync(o => o.OrderNo == orderSn);
                if (findOrder == null)
                {
                    return null;
                }
                ViewBag.totalFee = findOrder.Fact;
            }
            ViewBag.orderSn = orderSn;
            return View();
        }


        /// <summary>
        /// 洗涤展示
        /// </summary>
        /// <returns></returns>
        public ActionResult Exhibition()
        {

            return View();
        }

        /// <summary>
        /// 抽奖
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Award(string openId)
        {
            var findUser = await UserUnknown(openId);
            if (findUser == null)
            {
                return null;
            }
            ViewBag.awardList = _eventAwardRepository.FindList<EventAward>(e => e.UserId == findUser.Id);
            ViewBag.OrderRate = _orderRateRepository.FindList<OrderRate>(a => a.IsApproval == true || a.Order.ApplicationUser.Id == findUser.Id, "RiseTime", false);
            return View(findUser);
        }
    }
}