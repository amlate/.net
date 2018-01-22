using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Entities.Logs;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.JsonModel;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Controllers;
using AiJiaXi.Web.Filters;
using AiJiaXi.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using LoginViewModel = AiJiaXi.Domain.ViewModels.Admin.LoginViewModel;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class PurviewController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IRepository<JoinApplication> _joinApplicationRepository;

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            set { _roleManager = value; }
        }

        public PurviewController(IRepository<LoginLog> loginLogRepository, IRepository<JoinApplication> joinApplicationRepository)
        {
            _loginLogRepository = loginLogRepository;
            _joinApplicationRepository = joinApplicationRepository;
        }

        private IRepository<Navbar> _navbarRepository;
        private IRepository<LoginLog> _loginLogRepository;
        private IRepository<ApplicationRole> _roleRepository;

        public PurviewController(ApplicationSignInManager signInManager, ApplicationUserManager userManager,
            ApplicationRoleManager roleManager, IRepository<Navbar> navbarRepository, IRepository<ApplicationRole> roleRepository, IRepository<LoginLog> loginLogRepository, IRepository<JoinApplication> joinApplicationRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            this._navbarRepository = navbarRepository;
            _roleRepository = roleRepository;
            _loginLogRepository = loginLogRepository;
            _joinApplicationRepository = joinApplicationRepository;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult VerificationCode()
        {
            string verificationCode = SecurityHelper.CreateVerificationText(5);
            Bitmap _img = SecurityHelper.CreateVerificationImage(verificationCode);
            _img.Save(this.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            this.TempData["VerificationCode"] = verificationCode.ToUpper();
            return null;
        }

        // GET: Admin/Account
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "Admin/Home/Index")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(LoginViewModel model, string returnUrl = "/Admin/Home/Index")
        {
            var loginReturn = new LoginReturn();

            if (TempData["VerificationCode"] == null || TempData["VerificationCode"].ToString() != model.VerificationCode.ToUpper())
            {

                loginReturn.Valid = false;
                loginReturn.Msg = "验证码输入错误！";
                return Json(loginReturn);
            }

            if (!ModelState.IsValid)
            {
                loginReturn.Valid = false;
                loginReturn.Msg = "不合法的字段值，请重新输入！";
                return Json(loginReturn, JsonRequestBehavior.AllowGet);
            }

            // 这不会计入到为执行帐户锁定而统计的登录失败次数中
            // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    try
                    {
                        var vlog = new LoginLog()
                        {
                            LoginDesc = (int)LoginStatus.Success,
                            LoginIp = this.Request.UserHostAddress == "::1" ? "127.0.01" : this.Request.UserHostAddress,
                            LoginUserName = model.UserName,
                            RiseTime = DateTime.Now
                        };

                        await _loginLogRepository.AddAsync(vlog);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }

                    loginReturn.Valid = true;
                    loginReturn.Msg = "登陆成功！";
                    loginReturn.ReturnUrl = HttpUtility.UrlDecode(returnUrl);
                    return Json(loginReturn);
                case SignInStatus.LockedOut:
                    loginReturn.Valid = false;
                    loginReturn.Msg = "账户已锁定 ！";
                    return Json(loginReturn);
                case SignInStatus.Failure:
                default:
                    try
                    {
                        var vlog = new LoginLog()
                        {
                            LoginDesc = (int)LoginStatus.Failed,
                            LoginIp = this.Request.UserHostAddress == "::1" ? "127.0.01" : this.Request.UserHostAddress,
                            LoginUserName = model.UserName,
                            RiseTime = DateTime.Now
                        };

                        await _loginLogRepository.AddAsync(vlog);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }

                    loginReturn.Valid = false;
                    loginReturn.Msg = "用户名不存在或密码错误！";
                    return Json(loginReturn);
            };
        }

        [AllowAnonymous]
        public async Task<string> Initial(string id)
        {
            if (id != "leonleon")
            {
                return "别乱玩！！！";
            }

            var user = new ApplicationUser()
            {
                UserName = "admin",
                RealName = "徐良",
                Email = "leobert_hsu@outlook.com",
                PhoneNumber = "18642650146",
                Dept = "产品研发部",
                Position = "软件开发工程师",
                AddTime = DateTime.Now,
                Remark = "系统最高权限者",
                HeadAppearUrl = string.Empty,
                EmailConfirmed = true,
                UserType = UserType.Admin,
                PhoneNumberConfirmed = true
            };

            var result = await UserManager.CreateAsync(user, "zhiyuan@2015888");
            if (result.Succeeded)
            {
                var role = new ApplicationRole() { Name = "系统管理员", Description = "系统最高权限" };
                var roleResult = await RoleManager.CreateAsync(role);
                if (roleResult.Succeeded)
                {
                    var userId = UserManager.FindByName("admin").Id;
                    await UserManager.AddToRoleAsync(userId, "系统管理员");
                    return "初始化成功";
                }
                else
                {
                    return roleResult.Errors.ToString();
                }
            }
            else
            {
                return result.Errors.ToString();
            }
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Purview");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        public async Task<ActionResult> NaviPartial()
        {
            var userId = this.HttpContext.User.Identity.GetUserId();

            if (this.UserManager.IsInRole(userId, "系统管理员"))
            {
                IEnumerable<Navbar> list = this.GetNaviMenu();
                return this.PartialView("_NaviPartial", list.ToList());
            }

            IEnumerable<Navbar> menuList = new List<Navbar>();

            // var membership = RepositoryFactory.IsmsMembershipRepository.Find(item => item.UserId == userId);

            menuList = UserManager.GetRoles(userId).Select(this.GetNaviMenu).Aggregate(menuList, (current, list) => current.Union(list));

            return this.PartialView("_NaviPartial", menuList.ToList());
        }

        private List<Navbar> GetNaviMenu(string roleName)
        {
            return _roleRepository.Find(item => item.Name == roleName).Navbars.Where(item => true).ToList();
        }

        private List<Navbar> GetNaviMenu()
        {
            return
                _navbarRepository.FindList<Navbar>(
                    item => true,
                    "Order",
                    true).ToList();
        }

        public ActionResult WrapperPartial()
        {
            bool isAdmin = false;
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);

            if (user.UserType == UserType.Admin)
            {
                isAdmin = true;
            }


            ViewBag.IsAdmin = isAdmin;
            var joins =
                _joinApplicationRepository.FindList<JoinApplication>(
                    item => item.FeedbackStatus == FeedbackStatus.UnDealed)
                    .OrderByDescending(item => item.RiseTime)
                    .ToList();
            ViewBag.Joins = joins.Take(5).ToList();
            ViewBag.Count = joins.Count;
            return PartialView("_WrapperPartial");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", "Home");
            }
            AddErrors(result);
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}