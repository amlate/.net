using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Common;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Entities.Orders;
using Project.Domain.Enums;
using Project.Domain.JsonModel;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class EventsController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IRepository<Agency> _agencyRepository;
        private IRepository<Event> _eventRepository;

        public EventsController(ApplicationUserManager userManager, ApplicationRoleManager roleManager,
            IRepository<Agency> agencyRepository, IRepository<Event> eventRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _agencyRepository = agencyRepository;
            _eventRepository = eventRepository;
        }


        // GET: Admin/Event
        public async Task<ActionResult> Index(int id = 1)
        {
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid).ToList();
            ViewBag.Agencies = new SelectList(agencies, "Id", "Name");
            var predicate = PredicateBuilder.True<Event>();

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.Agencies.Select(m => m.Id).Contains(agencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            var model = this._eventRepository.FindList<Event>(predicate).OrderByDescending(a => a.EndTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_EventSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult EventsSearchPost(string eventType, string name, string startTime, string endTime, string status, int id = 1)
        {
            return EventsSearchResult(eventType,name, startTime, endTime, status, id);
        }

        private ActionResult EventsSearchResult(string eventType, string name, string startTime, string endTime, string status, int id = 1)
        {
            var predicate = PredicateBuilder.True<Event>();
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.Agencies.Select(m => m.Id).Contains(agencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                switch (status)
                {
                    case "0":
                        predicate = predicate.And(item => item.StartTime > DateTime.Now);
                        break;
                    case "1":
                        predicate = predicate.And(item => item.StartTime <= DateTime.Now && item.EndTime > DateTime.Now);
                        break;
                    case "2":
                        predicate = predicate.And(item => item.EndTime <= DateTime.Now);
                        break;
                }
            }


            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime dt;
                bool flag = DateTime.TryParse(startTime, out dt);
                if (flag)
                {
                    predicate = predicate.And(item => item.StartTime >= dt);
                }
                else
                {
                    throw new Exception($"错误的时间格式({startTime})！");
                }
            }

            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime dt;
                bool flag = DateTime.TryParse(endTime, out dt);
                if (flag)
                {
                    predicate = predicate.And(item => item.StartTime <= dt);
                }
                else
                {
                    throw new Exception($"错误的时间格式({endTime})！");
                }
            }

            if (!string.IsNullOrWhiteSpace(eventType))
            {
                EventType etype;
                bool flag = Enum.TryParse(eventType, true, out etype);
                if (flag)
                {
                    predicate = predicate.And(item => item.EventType == etype);
                }
                else
                {
                    throw new Exception($"错误的活动类型({eventType})！");
                }
            }

            var model = this._eventRepository.FindList<OrderItemClass>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_EventSearchResult", model);
            return View(model);
        }

        public ActionResult Manage(long id = 0)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员不能发布活动！");
            }

            ViewBag.Second = "发布活动";

            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid).ToList();
            ViewBag.Agencies = agencies;

            var entity = _eventRepository.Find(item => item.Id == id);
            if (entity == null)
            {
                return View(new Event() {StartTime = DateTime.Now, EndTime = DateTime.Now});
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Manage(FormCollection form, long id = 0)
        {
            var entity = _eventRepository.Find(item => item.Id == id);
            if (entity == null)
            {
                entity = new Event();
                if (this.TryUpdateModel(entity, null,form.AllKeys, new [] { "Desc" , "agencies" }))
                {
                    var agencies = form["agencies"].Split(',');
                    if (agencies.Any())
                    {
                        var list =
                            _agencyRepository.FindList<Agency>(item => agencies.Contains(item.Id.ToString())).ToList();

                        entity.Agencies = new List<Agency>();
                        entity.Agencies.AddRange(list);
                    }
                    entity.Desc = HttpUtility.HtmlEncode(form["Desc"]);
                    await _eventRepository.AddAsync(entity);
                }   
            }
            else
            {
                if (this.TryUpdateModel(entity, null, form.AllKeys, new[] { "Desc", "agencies" }))
                {
                    if (entity.Agencies == null)
                    {
                        entity.Agencies = new List<Agency>();
                    }
                    entity.Agencies.Clear();
                    var agencies = form["agencies"].Split(',');
                    if (agencies.Any())
                    {
                        entity.Agencies.AddRange(_agencyRepository.FindList<Agency>(item => agencies.Contains(item.Id.ToString())).ToList());
                    }
                    entity.Desc = HttpUtility.HtmlEncode(form["Desc"]);
                    await _eventRepository.UpdateAsync(entity);
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

            var entity = _eventRepository.Find(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该优惠券不存在，删除失败！');");
            }

            var result = await _eventRepository.DeleteAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model =
                this._eventRepository.FindList<Event>(item => true)
                    .OrderBy(a => a.Id)
                    .ToPagedList(1, 15);

            return PartialView("_EventSearchResult", model);
        }

        public ActionResult ApplyStatus()
        {
            var select2Items = (from object value in Enum.GetValues(typeof (ApplyStatus))
                select new Select2Item() {value = value.ToString(), text = value.GetDescription()}).ToList();

            return Json(select2Items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Flags()
        {
            var select2Items = new List<Select2Item>()
            {
                new Select2Item() {value = "true", text = "进行中"},
                new Select2Item() {value = "false", text = "尚未开启"}
            };

            return Json(select2Items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Post(FormCollection form)
        {
            var name = form["name"];
            var pk = form["pk"];
            var value = form["value"];
            var entity = _eventRepository.Find(item => item.Id.ToString() == pk);
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (string.Equals("status", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.ApplyStatus = (ApplyStatus)Enum.Parse(typeof(ApplyStatus), value, true);
            }

            if (string.Equals("flag", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.Flag = bool.Parse(value);
            }

            var result = await _eventRepository.UpdateAsync(entity);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}