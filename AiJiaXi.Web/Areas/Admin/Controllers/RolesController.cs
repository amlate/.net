using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class RolesController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IRepository<ApplicationRole> _roleRepository;
        private IRepository<Navbar> _navbarRepository;

        public RolesController(ApplicationUserManager userManager, ApplicationRoleManager roleManager,
            IRepository<ApplicationRole> roleRepository, IRepository<Navbar> navbarRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _navbarRepository = navbarRepository;
        }   

        // GET: Admin/ApplicationRole
        public ActionResult Index(int id = 1)
        {
            var model = this._roleManager.Roles.Where(item => item.Name != "系统管理员").OrderByDescending(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_RolesSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult RolesSearchPost(string name, int id = 1)
        {
            return RolesSearchResult(name, id);
        }

        private ActionResult RolesSearchResult(string name, int id = 1)
        {
            var predicate = PredicateBuilder.True<ApplicationRole>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            var model = this._roleManager.Roles.Where(predicate).OrderByDescending(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_RolesSearchResult", model);
            return View(model);
        }

        public ActionResult Manage(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ViewBag.Second = "新增角色";
                return View(new ApplicationRole());
            }

            var entity = _roleManager.Roles.FirstOrDefault(item => item.Id == id);
            ViewBag.Second = "编辑菜单";

            return entity == null ? View(new ApplicationRole()) : View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ApplicationRole role)
        {
            var entity = _roleManager.FindById(role.Id);
            if (entity == null)
            {
                await _roleManager.CreateAsync(role);
            }
            else
            {
                if (this.TryUpdateModel(entity))
                {
                    await _roleManager.UpdateAsync(entity);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var entity = await _roleManager.FindByIdAsync(id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该角色不存在，删除失败！');");
            }

            var result = await _roleManager.DeleteAsync(entity);
            if (!result.Succeeded)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model = this._roleManager.Roles.OrderByDescending(a => a.Id).ToPagedList(1, 15);

            return PartialView("_RolesSearchResult", model);
        }

        public async Task<ActionResult> Details(string id)
        {
            var role = await _roleRepository.FindAsync(item => item.Id == id);

            if (role == null)
            {
                throw new Exception("不存在的角色Id");
            }

            ViewBag.RoleName = role.Name;
            ViewBag.RoleId = role.Id;
            ViewBag.HasNavbars = role.Navbars.Select(item => item.Id).ToList();

            var navbars = _navbarRepository.FindList<Navbar>(item => true).ToList();

            return View(navbars);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(List<int> purview)
        {
            string id = ControllerContext.RouteData.Values["id"] as string;
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("传入参数非法！");
            }
            var role = await _roleRepository.FindAsync(item => item.Id == id);
            
            var purviews = _navbarRepository.FindList<Navbar>(item => purview.Contains(item.Id)).ToList();

            role.Navbars.Clear();
            foreach (var item in purviews)
            {
                role.Navbars.Add(item);
            }

            var result = _roleRepository.Update(role);
            if (!result)
            {
                throw  new Exception("更新数据失败！");
            }

            return RedirectToAction("Index");
        }
    }
}