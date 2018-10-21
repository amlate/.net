using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
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
    public class EventAwardController : Controller
    {
        private IRepository<EventAward> _eventAwardRepository;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public EventAwardController(IRepository<EventAward> eventAwardRepository, ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            _eventAwardRepository = eventAwardRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/EventAward
        public async Task<ActionResult> Index(string eventId, int id = 1)
        {
            var predicate = PredicateBuilder.True<EventAward>().And(item => item.EventPrize.EventId.ToString() == eventId);

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.EventPrize.Event.Agencies.Select(m => m.Id).Contains(agencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            var model = this._eventAwardRepository.FindList<EventAward>(predicate).OrderByDescending(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_EventAwardsResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult EventAwardsSearchPost(string name, int id = 1)
        {
            return EventAwardsSearchResult( name, id);
        }

        private ActionResult EventAwardsSearchResult(string name, int id = 1)
        {
            var predicate = PredicateBuilder.True<EventAward>();
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.EventPrize.Event.Agencies.Select(m => m.Id).Contains(agencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }


            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.User.UserName.Contains(name));
            }

            var model = this._eventAwardRepository.FindList<OrderItemClass>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_EventAwardsResult", model);
            return View(model);
        }

        public ActionResult Flags()
        {
            var select2Items = new List<Select2Item>()
            {
                new Select2Item() {value = "true", text = "已发货"},
                new Select2Item() {value = "false", text = "未发货"}
            };

            return Json(select2Items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Post(FormCollection form)
        {
            var name = form["name"];
            var pk = form["pk"];
            var value = form["value"];
            var entity = _eventAwardRepository.Find(item => item.Id.ToString() == pk);
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (string.Equals("flag", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.Flag = bool.Parse(value);
            }

            var result = await _eventAwardRepository.UpdateAsync(entity);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}