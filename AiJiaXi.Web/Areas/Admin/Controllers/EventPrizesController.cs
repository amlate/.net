using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;
using AiJiaXi.Web.Provider;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class EventPrizesController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IRepository<Event> _eventRepository;
        private IRepository<EventPrize> _eventPrizeRepository;

        public EventPrizesController(IRepository<EventPrize> eventPrizeRepository, ApplicationUserManager userManager, ApplicationRoleManager roleManager, IRepository<Event> eventRepository)
        {
            _eventPrizeRepository = eventPrizeRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _eventRepository = eventRepository;
        }

        // GET: Admin/EventPrizesDefault
        public async Task<ActionResult> Index(string eventId, int id = 1)
        {
            var predicate = PredicateBuilder.True<EventPrize>().And(item => item.EventId.ToString() == eventId);

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id; 
                predicate = predicate.And(item => item.Event.Agencies.Select(m => m.Id).Contains(agencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            var model = this._eventPrizeRepository.FindList<EventPrize>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_EventPrizesResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult EventPrizesSearchPost(int id = 1)
        {
            return EventPrizesSearchResult(id);
        }

        private ActionResult EventPrizesSearchResult(int id = 1)
        {
            var predicate = PredicateBuilder.True<EventPrize>();
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.Event.Agencies.Select(m => m.Id).Contains(agencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            var model = this._eventPrizeRepository.FindList<EventPrize>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_EventPrizesResult", model);
            return View(model);
        }

        public ActionResult Manage(long id = 0)
        {
            var events =
                _eventRepository.FindList<Event>(
                    item =>
                        item.EndTime >= DateTime.Now &&
                        item.EventType == EventType.LuckyDraw).ToList();
            ViewBag.Events = new SelectList(events, "Id", "Name");

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员不能添加奖品！");
            }

            ViewBag.Second = "奖品设置";

            var entity = _eventPrizeRepository.Find(item => item.Id == id);
            if (entity == null)
            {
                return View(new EventPrize());
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Manage(FormCollection form, HttpPostedFileBase prize_img, long id = 0)
        {
            if (!Directory.Exists(Server.MapPath("~/Upload/Prizes")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Upload/Prizes"));
            }

            string prize_imgName = prize_img == null ? string.Empty : FileUploadHelper.ProcessUpload(prize_img, Server.MapPath("~/Upload/Prizes"));
            var entity = _eventPrizeRepository.Find(item => item.Id == id);
            if (entity == null)
            {
                entity = new EventPrize();
                if (this.TryUpdateModel(entity, null, form.AllKeys, new[] { "Desc" }))
                {
                    // entity.Desc = HttpUtility.HtmlEncode(form["Desc"]);
                    entity.Desc = string.Empty;
                    if (!string.IsNullOrWhiteSpace(prize_imgName))
                    {
                        entity.Images = prize_imgName;
                    }
                    await _eventPrizeRepository.AddAsync(entity);
                }
            }
            else
            {
                if (this.TryUpdateModel(entity, null, form.AllKeys, new[] { "Desc" }))
                {
                    // entity.Desc = HttpUtility.HtmlEncode(form["Desc"]);
                    entity.Desc = string.Empty;
                    if (!string.IsNullOrWhiteSpace(prize_imgName))
                    {
                        entity.Images = prize_imgName;
                    }
                    await _eventPrizeRepository.UpdateAsync(entity);
                }
            }
            return RedirectToAction("Index", new {eventId = entity.EventId});
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

            var entity = _eventPrizeRepository.Find(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该优惠券不存在，删除失败！');");
            }

            var result = await _eventPrizeRepository.DeleteAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model =
                this._eventPrizeRepository.FindList<EventPrize>(item => true)
                    .OrderBy(a => a.Id)
                    .ToPagedList(1, 15);

            return PartialView("_EventPrizesResult", model);
        }
    }
}