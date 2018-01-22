using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Repositories.Impl;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;
using Microsoft.AspNet.Identity.Owin;
using Webdiyer.WebControls.Mvc;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class NavbarsController : Controller
    {
        private IRepository<Navbar> _navbarRepository;
        private IRepository<ApplicationRole> _appliocationRoleRepository;
        private ApplicationUserManager _userManager;

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

        public NavbarsController(IRepository<Navbar> navbarRepository, ApplicationUserManager userManager, IRepository<ApplicationRole> appliocationRoleRepository)
        {
            _navbarRepository = navbarRepository;
            _userManager = userManager;
            _appliocationRoleRepository = appliocationRoleRepository;
        }

        // GET: Admin/Navbars
        public ActionResult Index(int id = 1)
        {
            PagedList<Navbar> model =
                _navbarRepository.FindList<Navbar>(item => true).OrderBy(a => a.Order).ToPagedList(id, 50);

            if (Request.IsAjaxRequest())
                return PartialView("_NavbarList", model);
            return View(model);
        }

        public ActionResult Manage(int id = 0)
        {
            var parents = _navbarRepository.FindList<Navbar>(item => item.IsParent).ToList();
            ViewData["NavParents"] = new SelectList(parents, "Id", "NameOption");

            if (id == 0)
            {
                ViewBag.Second = "新增菜单";
                return View(new Navbar());
            }

            var entity = _navbarRepository.Find(item => item.Id == id);
            ViewBag.Second = "编辑菜单";

            return entity == null ? View(new Navbar()) : View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(Navbar navbar)
        {
            navbar.IsParent = navbar.ParentId == 0;

            if (navbar.Id == 0)
            {
                navbar.Status = true;
                await _navbarRepository.AddAsync(navbar);
            }
            else
            {
                var entity = _navbarRepository.Find(item => item.Id == navbar.Id);
                if (this.TryUpdateModel(entity))
                {
                    await _navbarRepository.UpdateAsync(entity);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id = 0)
        {
            var entity = await _navbarRepository.FindAsync(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该菜单不存在，删除失败！');");
            }

            if (await _navbarRepository.ExistAsync(item => item.ParentId == id))
            {
                return JavaScript("layer.alert('该菜单下有子菜单，删除失败！');");
            }

            var result = await _navbarRepository.DeleteAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            PagedList<Navbar> models =
                _navbarRepository.FindList<Navbar>(item => true).OrderBy(a => a.Order).ToPagedList(id, 50);


            return PartialView("_NavbarList", models);
        }
    }
}