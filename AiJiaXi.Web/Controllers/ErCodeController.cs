using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.Repositories.Interface;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AiJiaXi.Web.Controllers
{
    public class ErCodeController : Controller
    {
        private ApplicationUserManager _userManager;//用户信息
        private IRepository<UserAccount> _userAccountRepository;//用户表啊[自定义的]


        public ErCodeController(IRepository<UserAccount> userAccountRepository, ApplicationUserManager userManager)
        {
            _userManager = userManager;
            _userAccountRepository = userAccountRepository;

        }

        /// <summary>
        /// 显示出推广二维码
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index(string OpenId)
        {
            //登录用户信息
            //var userId = User.Identity.GetUserId();
            //var user = this._userManager.FindById(userId);
            //var phone = user.PhoneNumber;

            string state = "我的二维码";
            string url = "";
            //获取推广员  用户类型为 推广员
            //var withdraUser = _userAccountRepository.Find(g => true && g.ApplicationUser.UserType == UserType.Withdrawals && g.ApplicationUser.PhoneNumber == phone);
            var withdraUser = _userAccountRepository.Find(g => true && g.OpenId== OpenId && g.ApplicationUser.UserType == UserType.User);
            if (withdraUser != null)
            {
                if (String.IsNullOrWhiteSpace(withdraUser.CommissionUrl))
                {
                    state = "如果您不是推广员请联系商家进行咨询，如果您是推广员，请到管理平台生成二维码！";
                }
                else
                {
                    url = withdraUser.CommissionUrl;
                }
            }
            else
            {
                state = "该功能只有注册成功并成为推广员后才可使用！";
            }
            ViewBag.state = state;
            ViewBag.url = url;

            return View();
        }

    }
}