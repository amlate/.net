using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Entities.Logs;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Webdiyer.WebControls.Mvc;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class OperationLogController : Controller
    {
        private IRepository<OperationLog> _operationLogRepository;

        public OperationLogController(IRepository<OperationLog> operationLogRepository)
        {
            _operationLogRepository = operationLogRepository;
        }

        public ActionResult Index(int id = 1)
        {
            var model = _operationLogRepository.FindList<OperationLog>(item => true).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OperationLogSearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult OperationLogSearchPost(string username, string ip, int id = 1)
        {
            return OperationLogSearchPostResult(username, ip, id);
        }
        private ActionResult OperationLogSearchPostResult(string username, string ip, int id = 1)
        {
            var predicate = PredicateBuilder.True<OperationLog>();

            if (!string.IsNullOrWhiteSpace(username))
            {
                predicate = predicate.And(item => item.UserName.Contains(username));
            }

            if (!string.IsNullOrWhiteSpace(ip))
            {
                predicate = predicate.And(item => item.UserIP == ip);
            }

            var model = _operationLogRepository.FindList<OperationLog>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OperationLogSearchResult", model);
            return View(model);
        }
    }
}