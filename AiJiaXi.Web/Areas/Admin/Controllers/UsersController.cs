using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Common.Helpers;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.Logs;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UsersController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public UsersController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ActionResult Index(int id = 1)
        {
            var model = this._userManager.Users.Where(item => item.UserType == UserType.User && !item.IsFrozen).OrderByDescending(a => a.AddTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_UsersSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult UsersSearchPost(string username, int id = 1)
        {
            return UsersSearchResult(username, id);
        }

        private ActionResult UsersSearchResult(string username,int id = 1)
        {
            var predicate = PredicateBuilder.True<ApplicationUser>();

            predicate = predicate.And(item => item.UserType == UserType.User && !item.IsFrozen);

            if (!string.IsNullOrWhiteSpace(username))
            {
                predicate = predicate.And(item => item.UserName.Contains(username));
            }

            var model = this._userManager.Users.Where(predicate).OrderByDescending(a => a.AddTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_UsersSearchResult", model);
            return View(model);
        }

        public ActionResult Frozen(int id = 1)
        {

            var model = this._userManager.Users.Where(item => item.UserType == UserType.User && item.IsFrozen).OrderByDescending(a => a.AddTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_FrozenSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult FrozenSearchPost(string username, int id = 1)
        {
            return FrozenSearchResult(username, id);
        }

        private ActionResult FrozenSearchResult(string username, int id = 1)
        {
            var predicate = PredicateBuilder.True<ApplicationUser>();

            predicate = predicate.And(item => item.UserType == UserType.User && item.IsFrozen);

            if (!string.IsNullOrWhiteSpace(username))
            {
                predicate = predicate.And(item => item.UserName.Contains(username));
            }

            var model = this._userManager.Users.Where(predicate).OrderByDescending(a => a.AddTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_FrozenSearchResult", model);
            return View(model);
        }

        public ActionResult Manage(string id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("无相应权限");
            }

            var roles = _roleManager.Roles.ToList();
            var selectRoles = new SelectList(roles, "Name", "Name");
            ViewBag.Roles = selectRoles;

            var entity = this._userManager.FindById(id);
            if (entity == null)
            {
                ViewBag.Second = "新增用户";
                return View(new ApplicationUser() {UserType = UserType.User});
            }

            ViewBag.Second = "编辑用户";

            return View(entity);
        }

        [HttpPost]
        public async Task<ActionResult> Manage(string id, string role, FormCollection collection)
        {

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("无相应权限");
            }

            var entity = await this._userManager.FindByIdAsync(id);
            if (entity == null)
            {
                entity = new ApplicationUser();
                if (this.TryUpdateModel(entity, "", collection.AllKeys, new string[] { "UserType" }))
                {
                    entity.UserType = UserType.User;
                    entity.IsFrozen = false;
                    entity.AddTime = DateTime.Now;
                    entity.EmailConfirmed = true;
                    entity.PhoneNumberConfirmed = true;
                    entity.UserAccount = new UserAccount();
                    // todo 初始密码移动到配置文件
                    var result = await _userManager.CreateAsync(entity, "baixime@2015888");
                    if (!result.Succeeded)
                    {

                        AddErrors(result);
                        return View(entity);
                    }
                }
            }
            else
            {
                if (this.TryUpdateModel(entity, "", collection.AllKeys, new string[] { "UserType" }))
                {
                    entity.UserType = UserType.User;
                    var result = await _userManager.UpdateAsync(entity);
                    if (result.Succeeded)
                    {
                    }
                }
            }

            return RedirectToAction("Index", "Users");
        }

        public async Task<ActionResult> Lock(string id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("无相应权限");
            }

            var entity = await this._userManager.FindByIdAsync(id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该用户不存在，冻结会员失败！');");
            }

            entity.IsFrozen = true;
            var result = await _userManager.UpdateAsync(entity);
            if (!result.Succeeded)
            {
                return JavaScript("layer.alert('冻结会员失败！');");
            }


            return JavaScript("layer.alert('成功冻结会员，该账号将暂时无法使用！', function(){ window.location.reload();});");
        }

        public async Task<ActionResult> ResetPwd(string id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("无相应权限");
            }

            var entity = await this._userManager.FindByIdAsync(id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该用户不存在，重置密码失败！');");
            }

            string code = await _userManager.GeneratePasswordResetTokenAsync(userId);
            string pwd = "baixime@2016";
            var result = await _userManager.ResetPasswordAsync(id, code, "");
            if (!result.Succeeded)
            {
                return JavaScript("layer.alert('重置密码失败过程中发生异常，重置密码失败！');");
            }


            return JavaScript("layer.alert('成功重置密码，初始密码为：" + pwd + "}！', function(){ window.location.reload();});");
        }

        public async Task<ActionResult> Delete(string id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("无相应权限");
            }

            var entity = await this._userManager.FindByIdAsync(id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该用户不存在，删除失败！');");
            }

            var result = await _userManager.DeleteAsync(entity);
            if (!result.Succeeded)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model = this._userManager.Users.Where(item => item.UserType == UserType.User && !item.IsFrozen).OrderByDescending(a => a.AddTime).ToPagedList(1, 15);

            return PartialView("_UsersSearchResult", model);
        }

        public async Task<ActionResult> Active(string id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("无相应权限");
            }

            var entity = await this._userManager.FindByIdAsync(id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该用户不存在，激活会员失败！');");
            }

            entity.IsFrozen = false;
            var result = await _userManager.UpdateAsync(entity);
            if (!result.Succeeded)
            {
                return JavaScript("layer.alert('激活会员失败！');");
            }


            return JavaScript("layer.alert('成功激活会员，该账号已恢复使用！', function(){ window.location.reload();});");
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