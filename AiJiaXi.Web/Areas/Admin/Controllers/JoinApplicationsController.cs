using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Enums;
using Project.Domain.JsonModel;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Webdiyer.WebControls.Mvc;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class JoinApplicationsController : Controller
    {
        private IRepository<JoinApplication> _joinApplicationRepository;

        public JoinApplicationsController(IRepository<JoinApplication> joinApplicationRepository)
        {
            _joinApplicationRepository = joinApplicationRepository;
        }

        public ActionResult Index(int id = 1)
        {
            var model = _joinApplicationRepository.FindList<JoinApplication>(item => true).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_JoinApplicationList", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult JoinApplicationSearchPost(string name, string mobile, string area, int id = 1)
        {
            return JoinApplicationPostResult(name, mobile, area, id);
        }
        private ActionResult JoinApplicationPostResult(string name, string mobile, string area, int id = 1)
        {
            var predicate = PredicateBuilder.True<JoinApplication>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(mobile))
            {
                predicate = predicate.And(item => item.Mobile == mobile);
            }

            if (!string.IsNullOrWhiteSpace(area))
            {
                predicate = predicate.And(item => item.Area.Contains(area));
            }

            var model = _joinApplicationRepository.FindList<JoinApplication>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_JoinApplicationList", model);
            return View(model);
        }

        public async Task<ActionResult> Delete(long id = 0)
        {
            var entity = await _joinApplicationRepository.FindAsync(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('不存在的申请记录，删除失败！');");
            }

            var result = await _joinApplicationRepository.DeleteAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }


            var model = _joinApplicationRepository.FindList<JoinApplication>(item => true).OrderByDescending(a => a.RiseTime).ToPagedList(1, 15);

            return PartialView("_JoinApplicationList", model);
        }

        [HttpPost]
        public async Task<ActionResult> Post(FormCollection form)
        {

            var name = form["name"];
            var pk = form["pk"];
            var value = form["value"];
            var entity = _joinApplicationRepository.Find(item => item.Id.ToString() == pk);
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (string.Equals("status", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.FeedbackStatus = (FeedbackStatus)Enum.Parse(typeof(FeedbackStatus), value, true);
            }
            

            var result = await _joinApplicationRepository.UpdateAsync(entity);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }

        public ActionResult FeedbackStatus()
        {
            var select2Items = (from object value in Enum.GetValues(typeof(FeedbackStatus))
                                select new Select2Item() { value = value.ToString(), text = value.GetDescription() }).ToList();

            return Json(select2Items, JsonRequestBehavior.AllowGet);
        }
    }
}