using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.JsonModel;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IRepository<Agency> _agencyRepository;
        private IRepository<UserAccount> _accountRepository;

        public AdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager,
            IRepository<Agency> agencyRepository, IRepository<UserAccount> accountRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _agencyRepository = agencyRepository;
            _accountRepository = accountRepository;
        }

        public ActionResult Index(string username, string userType, int id = 1)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("没有查看管理员列表的权限！");
            }

            var predicate = PredicateBuilder.True<ApplicationUser>();

            predicate = predicate.And(item => item.UserType != UserType.User);

            if (!string.IsNullOrWhiteSpace(username))
            {
                predicate = predicate.And(item => item.UserName.Contains(username));
            }

            if ((!string.IsNullOrWhiteSpace(userType)) && userType != "-1")
            {
                var os = (UserType)Enum.Parse(typeof(UserType), userType, true);
                predicate = predicate.And(item => item.UserType == os);

            }

            var model = this._userManager.Users.Where(predicate).OrderByDescending(a => a.AddTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_AdminSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult AdminSearchPost(string username, string userType, int id = 1)
        {
            return AdminSearchResult(username, userType, id);
        }

        private ActionResult AdminSearchResult(string username, string userType, int id = 1)
        {
            var predicate = PredicateBuilder.True<ApplicationUser>();

            predicate = predicate.And(item => item.UserType != UserType.User);

            if (!string.IsNullOrWhiteSpace(username))
            {
                predicate = predicate.And(item => item.UserName.Contains(username));
            }

            if ((!string.IsNullOrWhiteSpace(userType)) && userType != "-1")
            {
                var os = (UserType)Enum.Parse(typeof(UserType), userType, true);
                predicate = predicate.And(item => item.UserType == os);

            }

            var model = this._userManager.Users.Where(predicate).OrderByDescending(a => a.AddTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_AdminSearchResult", model);
            return View(model);
        }

        public ActionResult Agency(string id)
        {
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid).ToList();
            ViewBag.Agencies = new SelectList(agencies, "Id", "Name");

            return View("Manage", GetEntity((int)UserType.Agency, id));
        }

        public ActionResult Admin(string id)
        {
            return View("Manage", GetEntity((int)UserType.Admin, id));
        }

        public ActionResult Withdrawals(string id)
        {
            return View("Manage", GetEntity((int)UserType.Withdrawals, id));
        }

        public ActionResult Washers(string id)
        {
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid).ToList();
            ViewBag.Agencies = new SelectList(agencies, "Id", "Name");
            return View("Manage", GetEntity((int)UserType.Washers, id));
        }

        private ApplicationUser GetEntity(int type,string id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            var userType = (UserType)(type);

            if (userType == UserType.Withdrawals && user.UserType == UserType.Agency)
            {
                
            }else if (user.UserType != UserType.Admin)
            {
                throw new Exception("无相应权限");
            }



            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;

            var entity = this._userManager.FindById(id);
            if (entity == null)
            {
                entity = new ApplicationUser() { UserType = userType };

                ViewBag.Second = "新增用户";
            }
            else
            {
                ViewBag.Second = "编辑用户";
            }

            return entity;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(string id, string role, FormCollection collection)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);

            var entity = await this._userManager.FindByIdAsync(id);
            if (entity == null)
            {
                entity = new ApplicationUser();

                if (this.TryUpdateModel(entity))
                {
                    if (entity.UserType == UserType.Withdrawals && user.UserType == UserType.Agency)
                    {

                    }
                    else if (user.UserType != UserType.Admin)
                    {
                        throw new Exception("无相应权限");
                    }
                    //判断推广员手机号不能重复
                    if (entity.UserType == UserType.Withdrawals)
                    {
                        //先判断该推广员是否是用户，如果不是请先注册用户
                        var withUser = _userManager.Users.FirstOrDefault(t => t.UserType == UserType.User && t.PhoneNumber == entity.PhoneNumber);
                        if (withUser == null)
                        {
                            
                                ModelState.AddModelError("", "您录入的手机号目前未注册成为用户，如果想添加推广员必须先注册成用户！");
                                return View("Manage", GetEntity((int)entity.UserType, id));
                           
                        }


                        //判断推广员手机号不能重复
                        var withPhone = _userManager.Users.FirstOrDefault(t=>t.UserType== UserType.Withdrawals && t.PhoneNumber== entity.PhoneNumber);
                        if (withPhone != null)
                        {
                            //已经存在，但是冻结
                            if (withPhone.IsFrozen == true)
                            {
                                ModelState.AddModelError("", "该推广员手机号已存在,但已经被禁用，请进行启用操作！");
                                return View("Manage", GetEntity((int)entity.UserType, id));
                            }
                            else
                            {
                                ModelState.AddModelError("", "该手机号的用户已经是推广员！");
                                return View("Manage", GetEntity((int)entity.UserType, id));
                            }
                        }

                   
                    }

                 


                    entity.AddTime = DateTime.Now;
                    entity.EmailConfirmed = true;
                    entity.PhoneNumberConfirmed = true;
                    entity.Remark = collection["remark"];
                    entity.UserAccount = new UserAccount();
                    // todo 初始密码移动到配置文件
                    var result = await _userManager.CreateAsync(entity, "baixime@2015888");
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            var user_Id = _userManager.FindByName(entity.UserName).Id;
                            await _userManager.RemoveFromRolesAsync(user_Id, role);
                            await _userManager.AddToRolesAsync(user_Id, role);
                        }
                    }
                    else
                    {
                        AddErrors(result);
                        return View("Manage", GetEntity((int)entity.UserType, id));
                    }
                }
                
            }
            else
            {
                if (this.TryUpdateModel(entity, "", collection.AllKeys, new string[] { "UserType" }))
                {
                    entity.Remark = collection["remark"];
                    if (entity.UserType == UserType.Withdrawals && user.UserType == UserType.Agency)
                    {

                    }
                    else if (user.UserType != UserType.Admin)
                    {
                        throw new Exception("无相应权限");
                    }

                    var result = await _userManager.UpdateAsync(entity);
                    if (result.Succeeded)
                    {
                        await _userManager.RemoveFromRolesAsync(entity.Id, _userManager.GetRoles(entity.Id).ToArray());
                        await _userManager.AddToRolesAsync(entity.Id, role);
                    }
                    else
                    {
                        AddErrors(result);
                        return View("Manage", GetEntity((int)entity.UserType, id));
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("无相应权限");
            }
            var account = _accountRepository.Find(item => item.Id == id);

            var entity = await this._userManager.FindByIdAsync(id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该用户不存在，删除失败！');");
            }

            entity.IsFrozen = true;
            var result = await _userManager.UpdateAsync(entity);
            if (!result.Succeeded)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model = this._userManager.Users.Where(item => item.UserType != UserType.User).OrderByDescending(a => a.AddTime).ToPagedList(1, 15);

            return PartialView("_AdminSearchResult", model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        
       public ActionResult Frozen()
        {
            var select2Items = new List<Select2Item>()
            {
                new Select2Item() {value = "true", text = "已禁用"},
                new Select2Item() {value = "false", text = "启用"}
            };

            return Json(select2Items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Post(FormCollection form)
        {

            var name = form["name"];
            var pk = form["pk"];
            var value = form["value"];
            var entity = _userManager.FindById(pk);
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            

            if (string.Equals("frozen", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.IsFrozen = bool.Parse(value);
            }

            var result = await _userManager.UpdateAsync(entity);
            if (!result.Succeeded)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}