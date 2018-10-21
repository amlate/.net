using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Entities.Location;
using Project.Domain.Entities.Orders;
using Project.Domain.Enums;
using Project.Domain.JsonModel;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Project.Web.Provider;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductClassesController : Controller
    {

        private IRepository<OrderItemClass> _orderItemClassRepository;
        private IRepository<City> _cityRepository;
        private IRepository<Agency> _agencyRepository;
        private ApplicationUserManager _userManager;

        public ProductClassesController(IRepository<OrderItemClass> orderItemClassRepository,
            ApplicationUserManager userManager, IRepository<City> cityRepository, IRepository<Agency> agencyRepository)
        {
            _orderItemClassRepository = orderItemClassRepository;
            _userManager = userManager;
            _cityRepository = cityRepository;
            _agencyRepository = agencyRepository;
        }
        
        public async Task<ActionResult> Index(int id = 1)
        {
            var cities = _cityRepository.FindList<City>(item => true);
            ViewBag.Cities = new SelectList(cities, "Id", "Name");

            var predicate = PredicateBuilder.True<OrderItemClass>().And(item => item.IsValid);
            
            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                var cityId = user.Agency.CityId;
                predicate = predicate.And(item => item.CityId == cityId);
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("无权查看！");
            }
            
            var model = this._orderItemClassRepository.FindList<OrderItemClass>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderItemClassSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderItemClassSearchPost(string name, string city, int id = 1)
        {
            return OrderItemClassSearchResult(name, city, id);
        }

        private ActionResult OrderItemClassSearchResult(string name, string city, int id = 1)
        {
            var predicate = PredicateBuilder.True<OrderItemClass>().And(item => item.IsValid);
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var cityId = user.Agency.CityId;
                predicate = predicate.And(item => item.CityId == cityId);
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                if (city.IsInt())
                {
                    var cityId = int.Parse(city);
                    predicate = predicate.And(item => item.CityId == cityId);
                }
            }

            var model = this._orderItemClassRepository.FindList<OrderItemClass>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderItemClassSearchResult", model);
            return View(model);
        }

        public ActionResult Manage(long id = 0)
        {
            var cityIds = _agencyRepository.FindList<Agency>(item => true).Select(item => item.CityId).Distinct().ToArray();
            var cities = _cityRepository.FindList<City>(item => cityIds.Contains(item.Id));

            ViewBag.Cities = new SelectList(cities, "Id", "Name");

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员无权管理产品类别");
            }

            if (id == 0)
            {
                ViewBag.Second = "新增产品类别";
                return View(new OrderItemClass());
            }

            var entity = _orderItemClassRepository.Find(item => item.Id == id);
            ViewBag.Second = "编辑产品类别";

            return entity == null ? View(new OrderItemClass()) : View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(OrderItemClass model, HttpPostedFileBase c_icon, HttpPostedFileBase c_icon_hover)
        {
            string c_iconName = c_icon == null ? string.Empty : FileUploadHelper.ProcessUpload(c_icon, Server.MapPath("~/Upload"));
            string c_icon_hoverName = c_icon_hover == null ? string.Empty : FileUploadHelper.ProcessUpload(c_icon_hover, Server.MapPath("~/Upload"));

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员无权管理产品类别");
            }

            var entity = _orderItemClassRepository.Find(item => item.Id == model.Id);
            if (entity == null)
            {
                if (!string.IsNullOrWhiteSpace(c_iconName))
                {
                    model.IconUrl = c_iconName;
                }

                if (!string.IsNullOrWhiteSpace(c_icon_hoverName))
                {
                    model.HoverIconUrl = c_icon_hoverName;
                }

                model.IsValid = true;
                model.AddTime = DateTime.Now;
                model.AddUser = user.UserName;
                await _orderItemClassRepository.AddAsync(model);
            }
            else
            {
                if (this.TryUpdateModel(entity))
                {
                    if (!string.IsNullOrWhiteSpace(c_iconName))
                    {
                        entity.IconUrl = c_iconName;
                    }

                    if (!string.IsNullOrWhiteSpace(c_icon_hoverName))
                    {
                        entity.HoverIconUrl = c_icon_hoverName;
                    }
                    entity.ModifyTime = DateTime.Now;
                    entity.ModifyUser = user.UserName;
                    await _orderItemClassRepository.UpdateAsync(entity);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Recycle(long id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员无权删除管理产品类别");
            }

            var entity = _orderItemClassRepository.Find(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该产品类别不存在，删除失败！');");
            }

            entity.IsValid = false;
            var result = await _orderItemClassRepository.UpdateAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model = this._orderItemClassRepository.FindList<OrderItemClass>(item => item.IsValid).OrderBy(a => a.Id).ToPagedList(1, 15);

            return PartialView("_OrderItemClassSearchResult", model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(long id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员无权删除管理产品类别");
            }

            var entity = _orderItemClassRepository.Find(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该产品类别不存在，删除失败！');");
            }

            var result = await _orderItemClassRepository.DeleteAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model = this._orderItemClassRepository.FindList<OrderItemClass>(item => item.IsValid).OrderBy(a => a.Id).ToPagedList(1, 15);

            return PartialView("_OrderItemClassSearchResult", model);
        }

        public ActionResult Counties(string id)
        {
            var counties =
                LocationHelper.GetCounties(int.Parse(id))
                    .Select(item => new Select2Item() {value = item.Id.ToString(), text = item.Name});

            return Json(counties, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Post(FormCollection form, List<string> value)
        {
            var name = form["name"];
            var pk = form["pk"];
            var entity = _orderItemClassRepository.Find(item => item.Id.ToString() == pk);
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (string.Equals("counties", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.Counties = (value == null || (!value.Any())) ? String.Empty : value.Aggregate(string.Empty, (cur, next) => cur + "," + next).Trim(',');

            }

            var result = await _orderItemClassRepository.UpdateAsync(entity);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}