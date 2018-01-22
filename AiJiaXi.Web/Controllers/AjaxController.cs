using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Entities.Location;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.JsonModel;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Areas.Admin.Controllers;
using AiJiaXi.Web.Provider;
using AiJiaXi.Domain.Entities.PromoterManager;
using Microsoft.AspNet.Identity;
using WeiPay;
using AiJiaXi.Domain.Entities.Configs;
using AiJiaXi.Common;

namespace AiJiaXi.Web.Controllers
{
    public class AjaxController : Controller
    {
        private readonly IRepository<Event> _evenRepository;
        private readonly IRepository<OrderStep> _orderStepRepository;
        private readonly IRepository<UserAccount> _userAccount;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<ApplicationUser> _applicationUser;
        private readonly IRepository<UserLocation> _userLocationRepository;
        private readonly IRepository<Feedback> _feedbackRepository;
        private readonly IRepository<EventAward> _eventAwardRepository;
        private readonly IRepository<Voucher> _voucheRepository;

        private readonly IRepository<UserAddress> _userAddressRepository;

        private readonly IRepository<County> _countyRepository;

        private readonly IRepository<Order> _ordeRepository;

        private readonly IRepository<AccountRecord> _accountRecordRepository;

        private readonly IRepository<JoinApplication> _joinRepository;

        private readonly IRepository<CartItem> _cartitemRepository;

        private readonly FrontPageController _frontPage;

        private IRepository<SmsConfig> _smsConfigRepository; //发送短信

        private IRepository<PromoterInfo> _promoterInfoRepository; //推广信息表

        /// <summary>
        /// 产品
        /// </summary>
        private readonly IRepository<OrderItem> _orderitemRepository;

        private readonly IRepository<Agency> _agencyRepository;

        public AjaxController(IRepository<PromoterInfo> promoterInfoRepository, IRepository<SmsConfig> smsConfigRepository, IRepository<UserAccount> userAccount, IRepository<ApplicationUser> applicationUser, IRepository<Feedback> feedbackRepository, IRepository<Voucher> voucheRepository, IRepository<UserAddress> userAddressRepository, IRepository<County> countyRepository, IRepository<Order> ordeRepository, IRepository<AccountRecord> accountRecordRepository, FrontPageController frontPage, IRepository<JoinApplication> joinRepository, IRepository<CartItem> cartitemRepository, IRepository<OrderItem> orderitemRepository, IRepository<Agency> agencyRepository, IRepository<OrderStep> orderStepRepository, IRepository<Event> evenRepository, IRepository<City> cityRepository, IRepository<UserLocation> userLocationRepository, IRepository<EventAward> eventAwardRepository)
        {
            _userAccount = userAccount;
            _applicationUser = applicationUser;
            _feedbackRepository = feedbackRepository;
            _voucheRepository = voucheRepository;
            _userAddressRepository = userAddressRepository;
            _countyRepository = countyRepository;
            _ordeRepository = ordeRepository;
            _accountRecordRepository = accountRecordRepository;
            _frontPage = frontPage;
            _joinRepository = joinRepository;
            _cartitemRepository = cartitemRepository;
            _orderitemRepository = orderitemRepository;
            _agencyRepository = agencyRepository;
            _orderStepRepository = orderStepRepository;
            _evenRepository = evenRepository;
            _cityRepository = cityRepository;
            _userLocationRepository = userLocationRepository;
            _eventAwardRepository = eventAwardRepository;

            _smsConfigRepository = smsConfigRepository;
            _promoterInfoRepository = promoterInfoRepository;
        }

        // GET: Ajax
        public async Task<ActionResult> GetMobileCode()
        {
            string openId = Request.Form["openId"];
            string tel = Request.Form["tel"];
            var isExist = await _applicationUser.FindAsync(a => a.PhoneNumber == tel);
            if (isExist == null)
            {

            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户反馈
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> UserFeedback()
        {
            string openid = Request.Form["openid"];
            string content = Request.Form["value"];
            var isExist = await _applicationUser.FindAsync(a => a.OpenId == openid);
            if (isExist == null)
            {
                return Json(new LoginReturn() { Valid = false, Msg = "用户不存在" }, JsonRequestBehavior.DenyGet);
            }
            Feedback feedback = new Feedback
            {
                Content = content,
                DealStatus = DealStatus.Dealing,
                RiseTime = DateTime.Now,
                UserId = isExist.Id,
                UserName = isExist.UserName
            };
            await _feedbackRepository.AddAsync(feedback);
            return Json(new LoginReturn() { Valid = true, Msg = "反馈成功！" }, JsonRequestBehavior.DenyGet);
        }


        /// <summary>
        /// 删除地址(逻辑删除)
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> DelAddress()
        {
            string openid = Request.Form["openid"];
            long id = Convert.ToInt64(Request.Form["id"]);
            var findUser = await _applicationUser.FindAsync(a => a.OpenId == openid);
            if (findUser == null)
            {
                return Json(new LoginReturn() { Valid = false, Msg = "用户不存在" }, JsonRequestBehavior.DenyGet);
            }
            var findOrder = await _userAddressRepository.FindAsync(u => u.Id == id);
            if (findOrder == null)
            {
                return Json(new LoginReturn() { Valid = false, Msg = "地址不存在" }, JsonRequestBehavior.DenyGet);
            }
            findOrder.IsShow = false;
            var userResult = await _userAddressRepository.UpdateAsync(findOrder);
            if (!userResult)
            {
                return Json(new LoginReturn() { Valid = false, Msg = "删除失败" }, JsonRequestBehavior.DenyGet);
            }
            //int count = await _userAddressRepository.DeleteAsync(u => u.Id == id);
            return Json(new LoginReturn() { Valid = true, Msg = "操作成功" }, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 添加或修改地址
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ActionResult> AddOrEditAddress(string type)
        {
            long id = Convert.ToInt64(Request.Form["id"]);
            string contactPhoneNum = Request.Form["contactPhoneNum"];
            string contact = Request.Form["contact"];
            string addr = Request.Form["addr"];
            string isDefault = Request.Form["isDefault"];
            string applicationuserId = Request.Form["applicationuserId"];
            long countyId = Convert.ToInt64(Request.Form["countyId"]);
            var findCounty = await _countyRepository.FindAsync(c => c.Id == countyId);
            // addr = findCounty.Note + addr;
            UserAddress returnAddress;

            if (type == "add")
            {
                UserAddress address = new UserAddress
                {
                    ApplicationUserId = applicationuserId,
                    Addr = findCounty.Note + addr,
                    Note = countyId.ToString(),
                    Contact = contact,
                    ContactPhoneNum = contactPhoneNum,
                    Gender = Gender.Male,
                    IsShow = true,
                    IsDefault = isDefault != "0"
                };
                if (address.IsDefault)
                {
                    var findDefult = await _userAddressRepository.FindAsync(u => u.IsDefault == true);
                    if (findDefult != null)
                    {
                        findDefult.IsDefault = false;
                        await _userAddressRepository.UpdateAsync(findDefult);
                    }
                }
                returnAddress = await _userAddressRepository.AddAsync(address);
            }
            else
            {
                var findAddress = await _userAddressRepository.FindAsync(u => u.Id == id);
                findAddress.Addr = findCounty.Note + addr;
                findAddress.Note = countyId.ToString();
                findAddress.Contact = contact;
                findAddress.ContactPhoneNum = contactPhoneNum;
                findAddress.IsDefault = isDefault != "0";
                if (findAddress.IsDefault)
                {
                    var findDefult = await _userAddressRepository.FindAsync(u => u.IsDefault == true && u.Id != findAddress.Id);
                    if (findDefult != null)
                    {
                        findDefult.IsDefault = false;
                        await _userAddressRepository.UpdateAsync(findDefult);
                    }
                }
                await _userAddressRepository.UpdateAsync(findAddress);
                returnAddress = findAddress;
            }
            return Json(new { Valid = true, Msg = "", Address = returnAddress }, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 获取地区列表,开通地区的列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCounty(long cityid)
        {
            var countyIdList = _agencyRepository.FindList<Agency>(a => a.CityId == cityid&&a.IsValid).Select(a => a.CountyId);
            //var countyList = _countyRepository.FindList<County>(c => c.Id > cityid && c.Id < (cityid + 100));
            var countyList = _countyRepository.FindList<County>(c => countyIdList.Contains(c.Id));
            string str = string.Empty;
            int count = 0;
            foreach (var item in countyList)
            {
                if (count == 0)
                {
                    str += @" <option selected='selected' value=" + item.Id + ">" + item.Name + "</option>";
                }
                else
                {
                    str += @" <option value=" + item.Id + ">" + item.Name + "</option>";
                }
                count++;
            }
            return Json(new { Valid = true, list = str }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetOrderInfo()
        {
            try
            {
                string id = Request.Form["orderId"];
                var findOrder = await _ordeRepository.FindAsync(o => o.OrderNo == id);
                if (findOrder == null)
                {
                    return Json(new { Valid = false }, JsonRequestBehavior.DenyGet);
                }
                string xiaci = "<tbody><tr><th width='32px'>编号</th><th width='30%'>物件</th><th>瑕疵</th></tr>";
                List<CartItem> cartList = findOrder.CartItems.ToList();
                int cartCount = 0;
                List<string> picList = new List<string>();
                for (int i = 0; i < cartList.Count; i++)
                {
                    //string note = string.Empty;
                    //var firstOrDefault = cartList[i].OrderImages.FirstOrDefault();
                    //if (firstOrDefault != null) { note = firstOrDefault.Note; }
                    //for (int j = 0; j < cartList[i].Nums; j++)
                    //{

                    //    cartCount++;
                    //    if (i % 2 == 0)
                    //    {
                    //        xiaci += @"<tr class='grey'><td class='gre'>" + cartCount + "</td><td>" + cartList[i].OrderItem.Name +(j+1)+ "</td><td>" + note + "</td></tr>";
                    //    }
                    //    else
                    //    {
                    //        xiaci += @"<tr><td class='gre'>" + cartCount + "</td><td>" + cartList[i].OrderItem.Name +(j+1)+ "</td><td>" + note + "</td></tr>";
                    //    }
                    //}
                    int itemCount = 0;
                    foreach (var item in cartList[i].OrderImages)
                    {

                        if (string.IsNullOrEmpty(item.Url))
                        {
                            picList.Add(item.Url);
                        }
                        itemCount++;
                        cartCount++;
                        if (cartCount % 2 == 0)
                        {
                            xiaci += @"<tr class='grey'><td class='gre'>" + cartCount + "</td><td>" + cartList[i].OrderItem.Name + itemCount + "</td><td>" + item.Note + "</td></tr>";
                        }
                        else
                        {
                            xiaci += @"<tr><td class='gre'>" + cartCount + "</td><td>" + cartList[i].OrderItem.Name + itemCount + "</td><td>" + item.Note + "</td></tr>";
                        }
                    }
                }
                xiaci += @" </tbody>";

                string img = string.Empty;
                foreach (var item in picList)
                {
                    img += @"<img src=" + item + ">";
                }

                string wuliu = string.Empty;
                List<OrderStep> orderstate = findOrder.OrderSteps.ToList();
                if (orderstate.Count() != 0)
                {
                    // orderstate = orderstate.OrderBy(o => o.OrderStatus).ToList();
                    orderstate = orderstate.OrderBy(o => o.RiseTime).ToList();
                }
                for (int i=orderstate.Count()-1; i >=0; i--)
                {
                    OrderStatus state = orderstate[i].OrderStatus;
                    if (i == orderstate.Count() - 1)
                    {
                        wuliu += @"<li class='first grey'><i></i><b></b><p><span>" + orderstate[i].RiseTime.Day.ToString() + "</span>" + orderstate[i].RiseTime.DayOfWeek.ToString() + "<br>" + orderstate[i].RiseTime.ToString("yyyy-MM HH:mm") + "</p><s>" + EnumExtensions.GetDescription(state) + "</s></li>";
                    }
                    //else if (state == OrderStatus.Dispatching)
                    //{
                    //    wuliu += @"<li class='first grey'><i></i><b></b><p><span>" + orderstate[i].RiseTime.Day.ToString() + "</span>" + orderstate[i].RiseTime.DayOfWeek.ToString() + "<br>" + orderstate[i].RiseTime.ToString("yyyy-MM HH:mm") + "</p><s>" + EnumExtensions.GetDescription(state) + "</s><span class='pai'>" + orderstate[i].Employee.Name + "<br>" + orderstate[i].Employee.Phone + "</span></li>";
                    //}
                    else if (i == orderstate.Count() - 2)
                    {
                        wuliu += @"<li class='first'><i></i><b></b><p><span>" + orderstate[i].RiseTime.Day.ToString() + "</span>" + orderstate[i].RiseTime.DayOfWeek.ToString() + "<br>" + orderstate[i].RiseTime.ToString("yyyy-MM HH:mm") + "</p><s>" + EnumExtensions.GetDescription(state) + "</s></li>";
                    }
                    else
                    {

                        wuliu += @"<li><i></i><b></b><p><span>" + orderstate[i].RiseTime.Day.ToString() + "</span>" + orderstate[i].RiseTime.DayOfWeek.ToString() + "<br>" + orderstate[i].RiseTime.ToString("yyyy-MM hh:mm") + "</p><s>" + EnumExtensions.GetDescription(state) + "</s></li>";
                    }
                }

                //优惠券信息 [同时查出，满足多少金额可用优惠券]
                var voucherList = _voucheRepository.FindList<Voucher>(v => v.IsUsed == false && v.UserAccountId == findOrder.ApplicationUserId && findOrder.TotalPrice >= v.PriceToUse && v.Agency.CountyId == findOrder.Agency.CountyId && v.EndTime > DateTime.Now).ToList();

                var voucherViewModels = voucherList.Select(item => new VoucherViewModel()
                {
                    Id = item.Id.ToString(),
                    Amount = item.Amount.ToString(),
                    Desc = item.Desc,
                    Name = item.Name,
                    EndTime = item.EndTime.SimpleDateFull()
                });
                return Json(new
                {
                    Valid = true,
                    Msg = new Order
                    {
                        Id = findOrder.Id,
                        OrderNo = findOrder.OrderNo,
                        TotalPrice = findOrder.TotalPrice,
                        Freight = findOrder.Freight,
                        Fact = findOrder.Fact,
                        OrderStatus = findOrder.OrderStatus
                    },
                    Other = new
                    {
                        openid = findOrder.ApplicationUser.OpenId,
                        itemTotal = findOrder.CartItems.Count,
                        assetFee = findOrder.TotalPrice + findOrder.Freight - findOrder.Fact,
                        UserAddress = findOrder.UserAddress.Addr,
                        Contact = findOrder.UserAddress.Contact,
                        RiseTime = Convert.ToDateTime(findOrder.RiseTime).ToString("yyyy-MM-dd HH:mm"),
                        xiaciHtml = xiaci,
                        dateMsg = "",
                        wuliu = wuliu,
                        img = img,
                        VoucherAmount = String.IsNullOrEmpty(findOrder.VoucherId.ToString()) ? "" : findOrder.Voucher.Amount.ToString(),
                        voucherList = voucherViewModels
                    }
                }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(new { Valid = false, Msg = ex.ToString() }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 生成订单
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GenerateOrder(string openid)
        {
            //var useraccount= await _frontPage.UserUnknown(openid);

            var order = new Order
            {
                OrderNo = System.Guid.NewGuid().ToString(),
                UserAddressId = 1,
                ApplicationUserId = "00000000-0000-0000-0000-000000000000",
                Appointment = DateTime.Now.AddDays(1),
                TotalPrice = 20,
                Fact = 18,
                Freight = 10,
                RiseTime = DateTime.Now,
                OrderStatus = OrderStatus.ToPay,
                AgencyId = 1,

            };
            var result = await _ordeRepository.AddAsync(order);
            return Json(new { Valid = true, Msg = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> CancelOrder()
        {
            try
            {
                string openid = Request.Form["openid"];
                string orderNo = Request.Form["orderId"];
                var findUser = await _frontPage.UserUnknown(openid);
                if (findUser == null)
                {
                    return Json(new { Valid = false, Msg = "用户不存在！" }, JsonRequestBehavior.DenyGet);
                }
                var order = await _ordeRepository.FindAsync(o => o.OrderNo == orderNo);
                if (order == null)
                {
                    return Json(new { Valid = false, Msg = "订单不存在！" }, JsonRequestBehavior.DenyGet);
                }
                if (order.OrderStatus == OrderStatus.ToPay)
                {
                    order.OrderStatus = OrderStatus.Cancelled;
                    OrderStep orderStep = new OrderStep()
                    {
                        OrderStatus = OrderStatus.Cancelled,
                        RiseTime = DateTime.Now,
                        OrderId = order.Id
                    };
                    await _orderStepRepository.AddAsync(orderStep);
                }
                else if (order.OrderStatus == OrderStatus.Patched || order.OrderStatus == OrderStatus.ToPatch || order.OrderStatus == OrderStatus.InService)
                {
                    order.OrderStatus = OrderStatus.ToRefund;
                    OrderStep orderStep = new OrderStep()
                    {
                        OrderStatus = OrderStatus.ToRefund,
                        RiseTime = DateTime.Now,
                        OrderId = order.Id
                    };
                    await _orderStepRepository.AddAsync(orderStep);
                }
                else
                {
                    return Json(new { Valid = false, Msg = "订单状态已不能修改！" }, JsonRequestBehavior.DenyGet);
                }
                bool success = await _ordeRepository.UpdateAsync(order);
                return Json(new { Valid = success }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(new { Valid = false, Msg = ex.ToString() }, JsonRequestBehavior.DenyGet);
            }

        }

        /// <summary>
        /// 加盟
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Join()
        {
            string name = Request.Form["name"];
            string phone = Request.Form["phone"];
            string email = Request.Form["email"];
            string type = Request.Form["type"];
            string city = Request.Form["city"];
            JoinApplication join = new JoinApplication
            {
                Name = name,
                Mobile = phone,
                Email = email,
                JoinType = type,
                Area = city,
                RiseTime = DateTime.Now
            };
            var newJoin = await _joinRepository.AddAsync(join);
            if (newJoin == null)
            {
                return Json(new { Valid = false }, JsonRequestBehavior.DenyGet);
            }
            return Json(new { Valid = true }, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 生成订单
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> setCartProduct()
        {
            int Count = int.Parse(Request.Form["Count"]);
            string OpenId = Request.Form["OpenId"];
            string CountyId = Request.Form["CountyId"];
            string UserId = "";
            long AgencyId = 0;
            var userModel = _applicationUser.Find(t => t.OpenId == OpenId);
            var AgencyMod = _agencyRepository.Find(t => t.CountyId.ToString() == CountyId&&t.IsValid);
            if (AgencyMod != null)
            {
                AgencyId = AgencyMod.Id;
            }
            if (userModel != null)
            {                
                //判断帐号是否冻结
                if (userModel.IsFrozen == true)//冻结
                {
                    return Json(new { result = false, IsFrozen = true }, JsonRequestBehavior.AllowGet);
                }

                UserId = userModel.Id;//用户主键ID
            }
            //var userId = this.HttpContext.User.Identity.GetUserId();
            if (Count > 0)
            {
                var order = new Order
                {
                    OrderNo = OrderHelper.GenerateOrderNumber(),
                    //UserAddressId = 1,
                    ApplicationUserId = UserId,
                    Appointment = Convert.ToDateTime("2000-1-1"),
                    //TotalPrice = 20,
                    Fact = 0,
                    Freight = 0,
                    RiseTime = Convert.ToDateTime("2000-1-1"),
                    OrderStatus = OrderStatus.ToConfirm,
                    AgencyId = AgencyId

                };

                //LC操作
                if (order.CartItems == null)
                {
                    order.CartItems = new List<CartItem>();
                }

                for (var i = 0; i < Count; i++)
                {
                    try
                    {
                        var cart = new CartItem
                        {
                            OrderItemId = Convert.ToInt64(Request.Form["Shopping[" + i + "][Id]"].Replace("pro_", "")),
                            Nums = Convert.ToInt32(Request.Form["Shopping[" + i + "][Count]"])
                        };
                        order.CartItems.Add(cart);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                var Ids = order.CartItems.Select(a => a.OrderItemId).ToArray();
                var OrderItem = _orderitemRepository.FindList<OrderItem>(a => Ids.Contains(a.Id)).ToList();
                foreach (var item in order.CartItems)
                {
                    order.TotalPrice += item.Nums * OrderItem.Find(a => a.Id == item.OrderItemId).Price;
                }
                order.Fact = order.TotalPrice;
                var orderresult = _ordeRepository.Add(order);

                return Json(new { result = true, id = orderresult.Id }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 修改订单信息
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> UpdateCartProduct()
        {
            long OrderId = Int64.Parse(Request.Form["OrderId"]);

            var OrderItemId = Convert.ToInt64(Request.Form["ProductJson[0][Id]"].Replace("pro_", ""));
            var Nums = Convert.ToInt32(Request.Form["ProductJson[0][Count]"]);

            var order = _ordeRepository.Find(a => a.Id == OrderId);

            order.CartItems.FirstOrDefault(a => a.OrderItemId == OrderItemId).Nums = Nums;

            var Ids = order.CartItems.Select(a => a.OrderItemId).ToArray();
            var OrderItem = _orderitemRepository.FindList<OrderItem>(a => Ids.Contains(a.Id)).ToList();
            order.TotalPrice = 0;
            foreach (var item in order.CartItems)
            {
                order.TotalPrice += item.Nums * OrderItem.Find(a => a.Id == item.OrderItemId).Price;
            }
            var orderresult = _ordeRepository.Update(order);
            return Json(new { result = orderresult }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 修改订单信息
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetCartProduct()
        {
            long OrderId = Int64.Parse(Request.Form["OrderId"]);

            var order = _ordeRepository.Find(a => a.Id == OrderId);
            if (order != null)
            {
                return Json(new { result = true, Total = order.TotalPrice }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 修改订单信息
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> OrderConfirm()
        {
            long OrderId = Int64.Parse(Request.Form["OrderId"]);
            long UserAddressId = Int64.Parse(Request.Form["UserAddressId"]);
            DateTime Appointment = Convert.ToDateTime(Request.Form["Appointment"]);
            string AppointmentTime = Request.Form["AppointmentTime"];

            var order = _ordeRepository.Find(a => a.Id == OrderId);
            order.OrderStatus = OrderStatus.ToPay;
            order.RiseTime = DateTime.Now;
            order.UserAddressId = UserAddressId;
            order.Appointment = Appointment;
            order.AppointmentTime = AppointmentTime;

            var orderresult = _ordeRepository.Update(order);


            //删除没用的999状态的订单
            // var orderDel = _ordeRepository.Find(a => a.OrderStatus== OrderStatus.ToConfirm);
            int delVal = _ordeRepository.Delete(t => t.OrderStatus == OrderStatus.ToConfirm);
            OrderStep orderStep = new OrderStep()
            {
                OrderStatus = OrderStatus.ToPay,
                RiseTime =order.RiseTime,
                OrderId = order.Id
                };
            await _orderStepRepository.AddAsync(orderStep);
            return Json(new { result = orderresult, OrderNo = order.OrderNo }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Test()
        {
            int Count = 1;
            var userId = this.HttpContext.User.Identity.GetUserId();
            if (Count > 0)
            {
                var order = new Order
                {
                    OrderNo = OrderHelper.GenerateOrderNumber(),
                    UserAddressId = 1,
                    ApplicationUserId = userId,
                    Appointment = DateTime.Now.AddDays(1),
                    TotalPrice = 20,
                    Fact = 18,
                    Freight = 10,
                    RiseTime = DateTime.Now,
                    OrderStatus = OrderStatus.ToPay,
                    AgencyId = 1,

                };
                order.CartItems = new List<CartItem>();
                for (var i = 0; i < Count; i++)
                {
                    var cart = new CartItem
                    {
                        OrderItemId = 1,
                        Nums = 2
                    };
                    order.CartItems.Add(cart);
                }
                var result = await _ordeRepository.AddAsync(order);
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
        }



        private static string _prepayId = ""; //预支付ID
        private static string _sign = "";     //为了获取预支付ID的签名
        private static string _paySign = "";  //进行支付需要的签名
        private static string _package = "";  //进行支付需要的包
        private static string _timeStamp = ""; //时间戳 程序生成 无需填写
        private static string _nonceStr = ""; //随机字符串  程序生成 无需填写
        /// <summary>
        /// 微信支付生成预支付页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GoWeixinPay()
        {
            try
            {
                string userOpenId = Request.Form["openid"];
                //string UserOpenId, string Body, string OrderSN, string TotalFee, string Attach
                string body = Request.Form["body"];
                string orderSn = OrderHelper.GenerateOrderNumber();

                string totalFee = Convert.ToInt32(Convert.ToDecimal(Request.Form["totalFee"]) * 100).ToString();

                string attach = string.Empty;
                #region 支付操作============================


                #region 基本参数===========================
                //时间戳  //时间戳 程序生成 无需填写
                _timeStamp = WeiPay.TenpayUtil.getTimestamp();
                //随机字符串  程序生成 无需填写
                _nonceStr = WeiPay.TenpayUtil.getNoncestr();

                //创建支付应答对象

                var packageReqHandler = new RequestHandler();
                //初始化
                packageReqHandler.init();

                //设置package订单参数  具体参数列表请参考官方pdf文档，请勿随意设置
                packageReqHandler.setParameter("body", body); //商品信息描述 127字符
                packageReqHandler.setParameter("appid", PayConfig.AppId);
                packageReqHandler.setParameter("mch_id", PayConfig.MchId);
                packageReqHandler.setParameter("nonce_str", _nonceStr.ToLower());
                packageReqHandler.setParameter("notify_url", PayConfig.NotifyUrl);
                packageReqHandler.setParameter("openid", userOpenId);
                packageReqHandler.setParameter("out_trade_no", orderSn); //商家订单号 商户自己订单号
                packageReqHandler.setParameter("spbill_create_ip", System.Web.HttpContext.Current.Request.UserHostAddress); //用户的公网ip，不是商户服务器IP
                packageReqHandler.setParameter("total_fee", totalFee); //商品金额,以分为单位(money * 100).ToString()
                packageReqHandler.setParameter("trade_type", "JSAPI");
                if (!string.IsNullOrEmpty(attach))
                    packageReqHandler.setParameter("attach", attach);//自定义参数 127字符

                #endregion

                #region sign===============================
                _sign = packageReqHandler.CreateMd5Sign("key", PayConfig.AppKey);

                #endregion

                #region 获取package包======================
                packageReqHandler.setParameter("sign", _sign);

                string data = packageReqHandler.parseXML();

                string prepayXml = HttpUtil.Send(data, "https://api.mch.weixin.qq.com/pay/unifiedorder");


                //获取预支付ID
                var xdoc = new XmlDocument();
                xdoc.LoadXml(prepayXml);
                XmlNode xn = xdoc.SelectSingleNode("xml");
                XmlNodeList xnl = xn.ChildNodes;

                if (xnl.Count > 7)
                {
                    _prepayId = xnl[7].InnerText;
                    _package = string.Format("prepay_id={0}", _prepayId);

                }
                #endregion

                #region 设置支付参数 输出页面  该部分参数请勿随意修改 ==============
                var paySignReqHandler = new RequestHandler();
                paySignReqHandler.setParameter("appId", PayConfig.AppId);
                paySignReqHandler.setParameter("timeStamp", _timeStamp);
                paySignReqHandler.setParameter("nonceStr", _nonceStr);
                paySignReqHandler.setParameter("package", _package);
                paySignReqHandler.setParameter("signType", "MD5");
                _paySign = paySignReqHandler.CreateMd5Sign("key", PayConfig.AppKey);
                string[] str = new string[] { "chooseWXPay" };
                return Json(new
                {
                    isSuccess = true,
                    a = xnl[1].InnerText,
                    p = xnl[0].InnerText,
                    z = prepayXml,
                    other = new
                    {
                        debug = "true",
                        appId = PayConfig.AppId,     //公众号名称，由商户传入     
                        timeStamp = _timeStamp,         //时间戳，自1970年以来的秒数     
                        nonceStr = _nonceStr, //随机串     
                        package = _package,
                        signType = "MD5",         //微信签名方式：     
                        paySign = _paySign, //微信签名 
                        jsApiList = str,
                        orderNumber = orderSn,
                        prepayId = _prepayId
                    }
                }, JsonRequestBehavior.DenyGet);
                #endregion
                #endregion
            }
            catch (Exception ex)
            {

                return Json(new { isSuccess = false, s = ex.ToString() }, JsonRequestBehavior.DenyGet);
            }
        }


        /// <summary>
        /// 生成充值订单
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RechargeRecord()
        {
            try
            {
                int countyId = Convert.ToInt32(Request.Form["countyId"]);
                string userid = Request.Form["userid"];
                var findUser = await _userAccount.FindAsync(u => u.Id == userid);
                string orderid = Request.Form["orderid"];
                decimal tradeMoney = Convert.ToDecimal(Request.Form["tradeMoney"]);
                string orderSn = Request.Form["orderSn"];
                if (!string.IsNullOrEmpty(orderSn))
                {//如果传orderSn那说明不是充值，是订单支付
                    AccountRecord accountRecord = new AccountRecord
                    {
                        UserAccountId = userid,
                        TradeType = TradeType.Consume,
                        RiseTime = DateTime.Now,
                        ResultType = ResultType.Failed,
                        TradeMoney = tradeMoney,
                        AccountBallance = findUser.Balance,
                        TradeScore = 0,
                        ScoreBalance = 0,
                        TradeId = orderid,
                        Note = orderSn
                    };
                    await _accountRecordRepository.AddAsync(accountRecord);
                }
                else
                {
                    AccountRecord accountRecord = new AccountRecord
                    {
                        UserAccountId = userid,
                        TradeType = TradeType.Recharge,
                        RiseTime = DateTime.Now,
                        ResultType = ResultType.Failed,
                        TradeMoney = tradeMoney,
                        AccountBallance = findUser.Balance,
                        TradeScore = 0,
                        ScoreBalance = 0,
                        TradeId = orderid,
                        Note = ""
                    };
                    //通过判断充值所在区域去寻找代理商来判断是否有充值活动，如果有充值活动，那么用Note来记录event的id
                    //不需要判断对应的区域了，但是要判断是否符合城市，从UserLocation表里取用户的地理信息，判断与当前城市是否相同，
                    //如果相同那么再去查找是否有相应的优惠
                    //var findAgency = await _agencyRepository.FindAsync(a => a.CountyId == countyId&&a.IsValid);
                    var currentCity= await _cityRepository.FindAsync(c => countyId - c.Id < 100);
                    var findUserLocation= await _userLocationRepository.FindAsync(u => u.FromUserName == findUser.OpenId);
                    if (currentCity.Name==findUserLocation.LocationCityName)
                    {
                        //将活动优惠的充值金额从大到小排列(需要满足的条件为1该区域的代理（不用了e.AgencyId == findAgency.Id && ），2是充值活动，3活动还在进行中（起止时间，Flag）4,审核通过)
                        var eventList = _evenRepository.FindList<Event>(e => e.EventType == EventType.Recharge && e.StartTime < DateTime.Now && e.EndTime > DateTime.Now && e.Flag && e.ApplyStatus == ApplyStatus.Reviewed).OrderByDescending(a => a.PriceTo);
                        //如果有活动那么挑选充值金额是否满足活动要求，如果满足活动要求那么判断是否充值次数大于活动的次数限制，如果超过限制，那么寻找其他优惠
                        foreach (var item in eventList)
                        {
                            if (tradeMoney >= item.PriceTo)
                            {//满足优惠条件
                                int totalCount = _accountRecordRepository.FindList<AccountRecord>(a => a.Note == item.Id.ToString()&&a.ResultType==ResultType.Succeedd).Count();
                                //满足优惠次数,默认如果为0的话不限制次数
                                if (item.Nums==0||item.Nums > totalCount)
                                {
                                    accountRecord.TradeMoney = tradeMoney + item.BenefitPrice;
                                    accountRecord.Note = item.Id.ToString();
                                    break;
                                }
                            }
                        }
                    }

                    await _accountRecordRepository.AddAsync(accountRecord);
                }
                return Json(new { Valid = true }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(new { Valid = false, Msg = ex.ToString() }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 更新充值订单
        /// 将微信的订单id存储到note中，通过服务器的回调修改状态
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> UpdateRechargeRecord(string wxOrderid)
        {
            var findOrder = await _accountRecordRepository.FindAsync(a => a.Note == wxOrderid);
            if (findOrder == null)
            {
                return false;
            }
            var findUser = await _userAccount.FindAsync(u => u.Id == findOrder.UserAccountId);
            if (findUser == null)
            {
                return false;
            }
            findOrder.TradeType = TradeType.Recharge;
            var accountResult = await _accountRecordRepository.UpdateAsync(findOrder);
            if (!accountResult)
            {
                return false;
            }
            findUser.Balance = findOrder.TradeMoney;
            findUser.Score = findUser.Score + Convert.ToInt32(findOrder.TradeMoney);
            bool userResult = await _userAccount.UpdateAsync(findUser);
            return userResult;
        }

        /// <summary>
        /// 兑换优惠券
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ExchangeVoucher()
        {
            try
            {
                string openid = Request.Form["openid"];
                string code = Request.Form["code"];
                var findUser = await _frontPage.UserUnknown(openid);
                if (findUser == null)
                {
                    return Json(new { Valid = false, Msg = "用户不存在！" }, JsonRequestBehavior.AllowGet);
                }
                var findVoucher = await _voucheRepository.FindAsync(v => v.VoucherNo == code && v.IsOccu == false && v.IsUsed == false);
                if (findVoucher == null)
                {
                    return Json(new { Valid = false, Msg = "优惠券不存在！" }, JsonRequestBehavior.AllowGet);
                }
                int maxCount = _voucheRepository.FindList<Voucher>(v => v.EventId == findVoucher.EventId && v.UserAccountId == findUser.Id).Count();
                if (maxCount >= findVoucher.Event.UseMaxVoucherNum && findVoucher.Event.UseMaxVoucherNum != 0)
                {
                    return Json(new { Valid = false, Msg = "已到达此类优惠券兑换上线，不能继续兑换" }, JsonRequestBehavior.AllowGet);
                }
                findVoucher.UserAccount = findUser;
                findVoucher.UserAccountId = findUser.Id;
                findVoucher.IsOccu = true;
                var result = await _voucheRepository.UpdateAsync(findVoucher);
                if (result)
                {
                    return Json(new { Valid = true, Msg = "兑换成功!", amount = findVoucher.Amount.ToString(), Desc = findVoucher.Desc, EndTime = findVoucher.EndTime.ToString(), Range = findVoucher.Agency.County.Name }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Valid = true, Msg = "兑换失败！" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Valid = false, Msg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 订单支付（线下和余额）
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> UpdataOrderState()
        {
            string type = Request.Form["type"];
            string orderNo = Request.Form["orderId"];
            //查找订单
            var findOrder = await _ordeRepository.FindAsync(o => o.OrderNo == orderNo && o.OrderStatus == OrderStatus.ToPay);
            if (findOrder == null)
            {
                return Json(new { Valid = false, Msg = "订单异常" }, JsonRequestBehavior.DenyGet);
            }
            //查找用户
            var findUser = await _userAccount.FindAsync(u => u.Id == findOrder.ApplicationUserId);
            if (findUser == null)
            {
                return Json(new { Valid = false, Msg = "用户异常" }, JsonRequestBehavior.DenyGet);
            }
            if (type == "other")//线下支付,更改订单状态，添加订单步骤
            {
                findOrder.OrderStatus = OrderStatus.ToPatch;
                findOrder.PayType = PayType.Cod;
                var orderResult = await _ordeRepository.UpdateAsync(findOrder);
                if (!orderResult)
                {
                    return Json(new { Valid = false, Msg = "订单系统异常！" }, JsonRequestBehavior.DenyGet);
                }
            }
            else//余额支付
            {
                //可用余额
                decimal avaiableMoney = findUser.Balance - findUser.FrozenMoney;
                if (findOrder.Fact > avaiableMoney)
                {
                    return Json(new { Valid = false, Msg = "余额不足请先充值" }, JsonRequestBehavior.DenyGet);
                }
                //需要操作的步骤有添加交易信息，更改订单状态，修改用户余额和积分，添加订单步骤
                AccountRecord accountRecord = new AccountRecord()
                {
                    UserAccountId = findUser.Id,
                    TradeType = TradeType.Consume,
                    RiseTime = DateTime.Now,
                    ResultType = ResultType.Succeedd,
                    TradeMoney = findOrder.Fact,
                    TradeScore = 0,
                    AccountBallance = findUser.Balance - findOrder.Fact,
                    TradeId = String.Empty,
                    ScoreBalance = findUser.Score,
                    Note = findOrder.OrderNo
                };
                var recordResutl = await _accountRecordRepository.AddAsync(accountRecord);
                if (recordResutl == null)
                {
                    return Json(new { Valid = false, Msg = "交易信息系统异常" }, JsonRequestBehavior.DenyGet);
                }
                findOrder.OrderStatus = OrderStatus.ToPatch;
                findOrder.PayType = PayType.Transfer;
                var orderResult = await _ordeRepository.UpdateAsync(findOrder);
                if (!orderResult)
                {
                    return Json(new { Valid = false, Msg = "订单系统异常" }, JsonRequestBehavior.DenyGet);
                }
                findUser.Balance = findUser.Balance - findOrder.Fact;
                //findUser.Score = (int)findUser.Score + (int)findOrder.Fact;
                var userResult = await _userAccount.UpdateAsync(findUser);
                if (!userResult)
                {
                    return Json(new { Valid = false, Msg = "交易失败！" }, JsonRequestBehavior.DenyGet);
                }
            }
            OrderStep orderStep = new OrderStep()
            {
                OrderStatus = OrderStatus.ToPatch,
                RiseTime = DateTime.Now,
                OrderId = findOrder.Id
            };
            await _orderStepRepository.AddAsync(orderStep);


            #region 发短信通知代理商可取件
            // 发送手机验证码       
            var config = await _smsConfigRepository.FindAsync(item => true);
            // var result = await SmsHelper.Send(config, string.Format("您的验证码为：{0} 【白洗么】", code), true, mobile);
            // var result = await SmsHelper.Send(config, string.Format("代理商您好，您有一笔新的订单需要处理，订单信息如下：联系人：{0}，联系方式：{1}，地址：{2}，总价：{3}，取件时间：{4}【白洗么】 ", findOrder.UserAddress.Contact, findOrder.UserAddress.ContactPhoneNum, findOrder.UserAddress.Addr,findOrder.Fact.ToString()), true, findOrder.Agency.ContactMobile);
            //var result = await SmsHelper.Send(config, string.Format("代理商您好，您有一笔新的订单需要处理，订单信息如下：联系人：{0}，联系方式：{1}，地址：{2}，总价：{3}，取件时间：{4}【白洗么】 ", findOrder.UserAddress.Contact, findOrder.UserAddress.ContactPhoneNum, findOrder.UserAddress.Addr, findOrder.Fact, findOrder.Appointment.ToString("yyyy-MM-dd") + "  " + findOrder.AppointmentTime), true, findOrder.Agency.ContactMobile);


            
            string orderInfo = getOrderInfo(findOrder.CartItems.ToList());

            NLogger.Trace("哈哈哈订单信息:"+ orderInfo);
            var result = await SmsHelper.Send(config, string.Format("订单信息如下：联系人：{0},联系方式：{1},地址：{2},总价：{3}, 品类：（{4}），取件时间：{5}【白洗么】 ", findOrder.UserAddress.Contact, findOrder.UserAddress.ContactPhoneNum, findOrder.UserAddress.Addr, findOrder.Fact,orderInfo,findOrder.Appointment.ToString("yyyy-MM-dd") + "  " + findOrder.AppointmentTime), true, findOrder.Agency.ContactMobile);
            if (!result)
            {
                // todo 验证码发送失败处理代码               
                // return "手机验证码发送失败，请稍后再试！";
            }

            #endregion

            #region 支付后优惠券作废，状态变成已使用
            var VoucherModel = _voucheRepository.Find(t => t.Id == findOrder.VoucherId);
            if (VoucherModel != null)
            {
                //已使用
                VoucherModel.IsUsed = true;
                _voucheRepository.Update(VoucherModel);
            }

            #endregion

            return Json(new { Valid = true, Msg = "交易成功！", openid = findUser.OpenId }, JsonRequestBehavior.DenyGet);
        }

        public  string testa()
        {
            var findOrder = _ordeRepository.Find(o => o.Id==1231);
   
            string orderInfo = getOrderInfo(findOrder.CartItems.ToList());


            return orderInfo;

        }

        /// <summary>
        /// 获取订单信息发短信时用
        /// </summary>
        /// <returns></returns>
        private string getOrderInfo(IList<CartItem> ci)
        {

            //var classItem = ci.Select(p => p.OrderItem.ItemClass.Name).Distinct().ToList(); ;

            string orderInfo = "";
            //var orderInfoStr = new List<string>();
            try
            {
                if (ci.Count > 0)
                {
                    //foreach (var classIt in classItem)
                    //{
                    //    var items = ci.Where(item => item.OrderItem.ItemClass.Name == classIt).Select(item => $"{item.OrderItem.Name}:{item.OrderItem.Nums}").ToList();
                    //    orderInfoStr.Add($"{classIt}({string.Join(",", items)})");
                    //}

                    //orderInfo = string.Join(",", orderInfoStr);


                    foreach (var item in ci)
                    {
                       
                             orderInfo += item.OrderItem.ItemClass.Name+"-"+ item.OrderItem.Name + item.Nums.ToString() + ",";
                  

                    }
                }
            }
            catch (Exception ex) {
                return "";
            }      
            return orderInfo.TrimEnd(',');
        }
        /// <summary>
        /// 确认收货
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ConfirmOrder()
        {
            try
            {
                var orderId = Request.Form["orderId"];
                var findOrder = await _ordeRepository.FindAsync(o => o.OrderNo == orderId);
                if (findOrder == null)
                {
                    return Json(new { Valid = false, Msg = "订单不存在" }, JsonRequestBehavior.DenyGet);
                }
                if (findOrder.OrderStatus != OrderStatus.Dispatched)
                {
                    return Json(new { Valid = false, Msg = "还不能签收" }, JsonRequestBehavior.DenyGet);
                }
                var findUser = findOrder.ApplicationUser.UserAccount;
                findUser.Score = findUser.Score + (int)findOrder.Fact;
                await _userAccount.UpdateAsync(findUser);
                findOrder.OrderStatus = OrderStatus.Succeed;
                findOrder.CompleteTime = DateTime.Now;
                var orderResult = await _ordeRepository.UpdateAsync(findOrder);
                if (!orderResult)
                {
                    return Json(new { Valid = false, Msg = "确认收货失败" }, JsonRequestBehavior.DenyGet);
                }
                OrderStep orderStep = new OrderStep()
                {
                    OrderStatus = OrderStatus.Succeed,
                    RiseTime = DateTime.Now,
                    OrderId = findOrder.Id
                };
                await _orderStepRepository.AddAsync(orderStep);

                NLogger.Error("确认收货findOrder.ApplicationUser.PhoneNumber:"+ findOrder.ApplicationUser.PhoneNumber);
                #region 计算及更新推广员提成金额
                //根据下线手机号找到上线手机号
                var PromoterInfoModel = _promoterInfoRepository.Find(t => t.FriendsPhone == findOrder.ApplicationUser.PhoneNumber);
                if (PromoterInfoModel != null)
                {
                    //推广人手机号
                    var myPhone = PromoterInfoModel.MyPhone;

                    NLogger.Error("确认收货myPhone:" + myPhone);
                    //根据手机号找出推广人用户表信息
                    var myUserAccountModel = _userAccount.Find(t => t.ApplicationUser.PhoneNumber == myPhone && t.ApplicationUser.UserType == UserType.User);
                    if (myUserAccountModel != null)
                    {
                        NLogger.Error("确认收货findOrder.Fact:" + findOrder.Fact.ToString());
                        //余额加上实际支付金额的百分之十
                        //myUserAccountModel.Balance = myUserAccountModel.Balance + (findOrder.Fact * decimal.Parse("0.1"));
                        myUserAccountModel.CommissionMoney = myUserAccountModel.CommissionMoney + (findOrder.Fact * decimal.Parse("0.1"));
                        _userAccount.Update(myUserAccountModel);

                        NLogger.Error("确认收货myUserAccountModel.CommissionMoney:" + myUserAccountModel.CommissionMoney.ToString());
                    }


                    // Balance
                }


                #endregion

                #region 判断是否可以跳转到大转盘,如果有抽奖活动，活动进行中，区域一致

                bool isJump = false;
                List<Event> luckList = _evenRepository.FindList<Event>(
                    e =>
                        e.Flag && e.EventType == EventType.LuckyDraw && e.StartTime < DateTime.Now && e.EndTime > DateTime.Now &&
                        e.Agencies.Select(a => a.CountyId).Contains(findOrder.Agency.CountyId)).OrderBy(e => e.PriceTo).ToList();
                if (luckList.Count > 0)
                {
                    var prizeList = (from item in luckList from itemEventPrize in item.EventPrizes where findOrder.Fact >= item.PriceTo && itemEventPrize.Nums - itemEventPrize.EventAwards.Count > 0 select itemEventPrize).ToList();
                    if (prizeList.Count > 0)
                    {
                        isJump = true;
                    }
                }
                #endregion
                return Json(new { Valid = true, Msg = "确认收货成功", openid = findOrder.ApplicationUser.OpenId, IsJump = isJump, countyId = findOrder.Agency.CountyId }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                NLogger.Error($"确认收货出错:\n{ex.StackTrace}");
                return null;
            }
         }

        /// <summary>
        /// 获取转盘信息
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetTurnTableInfo(string openId, string orderNo)
        {
            var findUser = await _userAccount.FindAsync(u => u.OpenId == openId);
            if (findUser == null)
            {
                return null;
            }
            var findOrder = await _ordeRepository.FindAsync(o => o.OrderNo == orderNo);
            if (findOrder == null)
            {
                return null;
            }
            List<Event> luckList = _evenRepository.FindList<Event>(
                e =>
                    e.Flag && e.EventType == EventType.LuckyDraw && e.StartTime < DateTime.Now && e.EndTime > DateTime.Now &&
                    e.Agencies.Select(a => a.CountyId).Contains(findOrder.Agency.CountyId)).OrderByDescending(e=>e.PriceTo).ToList();
            //var prizeList = (from item in luckList from itemEventPrize in item.EventPrizes where findOrder.Fact>=item.PriceTo&& itemEventPrize.Nums - itemEventPrize.EventAwards.Count > 0 select itemEventPrize).ToList();
            List<EventPrize> prizeList=new List<EventPrize>();
            int count = 0;
            foreach (var item in luckList)
            {
                foreach (var itemEventPrize in item.EventPrizes)
                {
                    if (findOrder.Fact >= item.PriceTo && itemEventPrize.Nums - itemEventPrize.EventAwards.Count > 0)
                    {
                        count++;
                        prizeList.Add(itemEventPrize);
                    }
                }
                if (count>0)
                {
                    break;
                }
            }
            ViewBag.Order = findOrder;
            List<string> restaraunts = new List<string>();
            List<string> weight = new List<string>();
            List<string> colors = new List<string>();
            List<string> prizeId = new List<string>();
            List<bool> log = new List<bool>();
            for (int i = 0; i < prizeList.Count; i++)
            {
                restaraunts.Add(prizeList[i].Name);
                weight.Add(prizeList[i].Weight.ToString());
                colors.Add(i % 2 == 0 ? "#FFF4D6" : "#FFFFFF");
                prizeId.Add(prizeList[i].Id.ToString());
                log.Add(prizeList[i].IsLog);
            }
            return Json(new {restaraunts = restaraunts, weight= weight, colors= colors, prizeIds= prizeId,log= log },JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加中奖信息
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> AddAwardInfo()
        {
            string openId = Request.Form["openId"];
            string eventprizeId = Request.Form["eventprizeId"];
            var findUser = await _userAccount.FindAsync(u => u.OpenId == openId);
            if (findUser == null)
            {
                return null;
            }
            long userAddressId=0 ;
            var findAddressDefult= await _userAddressRepository.FindAsync(u => u.IsShow && u.IsDefault && u.ApplicationUserId == findUser.Id);
            if (findAddressDefult==null)
            {
               var findAddress= await _userAddressRepository.FindAsync(u => u.IsShow&& u.ApplicationUserId == findUser.Id);
                if (findAddress!=null)
                {
                    userAddressId = findAddress.Id;
                }
            }
            else
            {
                userAddressId= findAddressDefult.Id;
            }
            EventAward eventAward = new EventAward()
            {
                UserId = findUser.Id,
                EventPrizeId = Convert.ToInt64(eventprizeId),
                Flag = false,
                AddressId = userAddressId
            };
            var newAward = await _eventAwardRepository.AddAsync(eventAward);
            return Json(new {award= newAward }, JsonRequestBehavior.AllowGet);
        }
    }
}