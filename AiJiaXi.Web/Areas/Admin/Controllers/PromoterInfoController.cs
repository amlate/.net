using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Repositories.Interface;
using Webdiyer.WebControls.Mvc;
using Project.Domain.Entities.PromoterManager;
using Project.Domain.Enums;
using Microsoft.AspNet.Identity;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Repositories.Impl;
using Project.Web.Filters;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    //推广员后台-我的推广信息
    public class PromoterInfoController : Controller
    {
        private IRepository<PromoterInfo> _promoterInfoRepository;//推广信息
        private ApplicationUserManager _userManager;//用户信息
        private IRepository<Order> _orderRepository;//订单表
        private IRepository<ApplicationUser> _applicationUserRepository;//用户表啊[系统的]

        public PromoterInfoController(IRepository<ApplicationUser> applicationUserRepository, IRepository<Order> orderRepository, ApplicationUserManager userManager, IRepository<PromoterInfo> promoterInfoRepository)
        {
            _promoterInfoRepository = promoterInfoRepository;
            _userManager=userManager;
            _orderRepository = orderRepository;
            _applicationUserRepository = applicationUserRepository;

           

        }




        public ActionResult Index(int id = 1)
        {

     

           //登录【后台】用户信息
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);

            string MyPhone = user.PhoneNumber;//获取当前登录用户手机号
                                              //string MyPhone = "13566665555";//获取当前登录用户手机号


            //获取【公众号或APP】端用户信息  用户类型为 用户
            var withdraU = _applicationUserRepository.Find(item => true && item.UserType == UserType.User && item.PhoneNumber == MyPhone);
            if (withdraU == null)
            {
                return Content("请联系商家确认登录用户信息中【移动电话】是否和公众号/APP中登录手机号一致！");
                // return JavaScript("layer.alert('请联系商家确认登录用户信息中【移动电话】是否和公众号/APP中登录手机号一致！');");
            }


            ViewBag.MyPhone = MyPhone;
            var model = _promoterInfoRepository.FindList<PromoterInfo>(item => true && item.MyPhone== MyPhone).ToList().OrderByDescending(t=>DateTime.Parse(t.FollowDate)).AsQueryable().ToPagedList(id, 15);
            foreach (var item in model)
            {
                //获取【公众号或APP】端用户信息  用户类型为 用户
                var withdraUser = _applicationUserRepository.Find(g => true && g.UserType == UserType.User && g.PhoneNumber == item.FriendsPhone);
                if (withdraUser != null)
                {
                    //成为好友时间
                    DateTime followDate = Convert.ToDateTime(item.FollowDate);
                    //消费金额[状态为成功]
                    var xfje = _orderRepository.FindList<Order>(t => true && t.RiseTime >= followDate && t.OrderStatus == OrderStatus.Succeed && t.ApplicationUser.Id == withdraUser.Id).ToList().Sum(t => t.Fact);

                    item.ConsumptionAmount = xfje.ToString();
                    item.MyAmount = (xfje /10).ToString();
                }
               


            }

            if (Request.IsAjaxRequest())
                return PartialView("_PromoterInfoSearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult SearchPost(string FriendsPhone, int id = 1)
        {
            return PromoterInfoSearchResult(FriendsPhone, id);
        }
        private ActionResult PromoterInfoSearchResult(string FriendsPhone, int id = 1)
        {
            //登录【后台】用户信息
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            string MyPhone = user.PhoneNumber;//获取当前登录用户手机号

            var predicate = PredicateBuilder.True<PromoterInfo>();
            predicate = predicate.And(item => item.MyPhone == MyPhone);

            if (!string.IsNullOrWhiteSpace(FriendsPhone))
            {
                predicate = predicate.And(item => item.FriendsPhone.Contains(FriendsPhone));
            }

            var model = _promoterInfoRepository.FindList<PromoterInfo>(predicate).ToList().OrderByDescending(t=>DateTime.Parse(t.FollowDate)).AsQueryable().ToPagedList(id, 15);
            foreach (var item in model)
            {
                //获取【公众号或APP】端用户信息  用户类型为 用户
                var withdraUser = _applicationUserRepository.Find(g => true && g.UserType == UserType.User && g.PhoneNumber == item.FriendsPhone);
                if (withdraUser != null)
                {
                    //成为好友时间
                    DateTime followDate = Convert.ToDateTime(item.FollowDate);
                    //消费金额[状态为成功]
                    var xfje = _orderRepository.FindList<Order>(t => true && t.RiseTime >= followDate && t.OrderStatus == OrderStatus.Succeed && t.ApplicationUser.Id == withdraUser.Id).ToList().Sum(t => t.Fact);

                    item.ConsumptionAmount = xfje.ToString();
                    item.MyAmount = (xfje / 10).ToString();
                }



            }

            if (Request.IsAjaxRequest())
                return PartialView("_PromoterInfoSearchResult", model);
            return View(model);
        }

        
      
    }
}