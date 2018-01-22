using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.Location;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.JsonModel;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;
using AiJiaXi.Web.Provider;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class AgencyController : Controller
    {
        private IRepository<Agency> _agencyRepository;
        private IRepository<City> _cityRepository;
        private ApplicationUserManager _userManager;

        public AgencyController(IRepository<Agency> agencyRepository, IRepository<City> cityRepository, ApplicationUserManager userManager)
        {
            _agencyRepository = agencyRepository;
            _cityRepository = cityRepository;
            _userManager = userManager;
        }

        public ActionResult Index(int id = 1)
        {
            var cityIds = _agencyRepository.FindList<Agency>(item => true).Select(item => item.CityId).Distinct().ToArray();
            var cities = _cityRepository.FindList<City>(item => cityIds.Contains(item.Id));

            ViewBag.Cities = new SelectList(cities, "Id", "Name");

            var model = _agencyRepository.FindList<Agency>(item => true).OrderByDescending(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_AgencySearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult AgencySearchPost(string name, string city, string county, int id = 1)
        {
            return AgencySearchPostResult(name, city, county, id);
        }
        private ActionResult AgencySearchPostResult(string name, string city, string county, int id = 1)
        {
            var predicate = PredicateBuilder.True<Agency>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                if (city.IsInt())
                {
                    int cityId_int = int.Parse(city);
                    predicate = predicate.And(item => item.CityId == cityId_int);
                }
            }

            if (!string.IsNullOrWhiteSpace(county))
            {
                predicate = predicate.And(item => item.County.Name.Contains(county));
            }
                
            var model = _agencyRepository.FindList<Agency>(predicate).OrderByDescending(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_AgencySearchResult", model);
            return View(model);
        }

        public ActionResult Manage(string classId, long id = 0)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("当前用户没有执行此操作的权限！");
            }
            if(id == 0)
            {
                ViewBag.Second = "新增代理商";
                return View(new Agency());
            }
            var entity = _agencyRepository.Find(item => item.Id == id);
            ViewBag.Second = "编辑代理商";

            return entity == null ? View(new Agency()) : View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(HttpPostedFileBase img_map, Agency model)
        {
            if (!Directory.Exists(Server.MapPath("~/Upload/rangemap")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Upload/rangemap"));
            }

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("当前用户没有执行此操作的权限");
            }
            string img_mapFileName = img_map == null ? string.Empty : FileUploadHelper.Process(img_map, Server.MapPath("~/Upload/rangemap"), model.Name);

            var entity = _agencyRepository.Find(item => item.Id == model.Id);
            if (entity == null)
            {
                var exists = _agencyRepository.Exist(item => item.IsValid && item.CountyId == model.CountyId);
                if (exists)
                {
                    ModelState.AddModelError("", "该地区已有签约代理商负责！");
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(img_mapFileName))
                {
                    model.RangeMap = img_mapFileName;
                }
                
                await _agencyRepository.AddAsync(model);
            }
            else
            {
                if (this.TryUpdateModel(entity))
                {
                    if (!string.IsNullOrWhiteSpace(img_mapFileName))
                    {
                        entity.RangeMap =img_mapFileName;
                    }
                    
                    await _agencyRepository.UpdateAsync(entity);
                }
            }

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<ActionResult> Delete(long id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("当前用户没有执行此操作的权限!");
            }

            var entity = _agencyRepository.Find(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该代理商不存在，删除失败！');");
            }

            entity.IsValid = false;
            var result = await _agencyRepository.UpdateAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model =
                this._agencyRepository.FindList<Agency>(item => true)
                    .OrderBy(a => a.Id)
                    .ToPagedList(1, 15);

            return PartialView("_AgencySearchResult", model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult Valid()
        {

            var select2Items = new List<Select2Item>()
            {
                new Select2Item() {value = "true", text = "已签约"},
                new Select2Item() {value = "false", text = "禁用"}
            };

            return Json(select2Items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Post(FormCollection form)
        {

            var name = form["name"];
            var pk = form["pk"];
            var value = form["value"];
            var entity = _agencyRepository.Find(item => item.Id.ToString() == pk);
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (string.Equals("valid", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.IsValid = bool.Parse(value);
            }

            _agencyRepository.Update(item => item.CountyId == entity.CountyId, u => new Agency() { IsValid = false});
            var result = await _agencyRepository.UpdateAsync(entity);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}