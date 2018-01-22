﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.Logs;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;
using Webdiyer.WebControls.Mvc;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class LoginLogController : Controller
    {
        private IRepository<LoginLog> _loginLogRepository;

        public LoginLogController(IRepository<LoginLog> loginLogRepository)
        {
            _loginLogRepository = loginLogRepository;
        }

        public ActionResult Index(int id = 1)
        {
            var model = _loginLogRepository.FindList<LoginLog>(item => true).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_LoginLogSearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult LoginLogSearchPost(string username, string ip, int id = 1)
        {
            return LoginLogSearchPostResult(username, ip, id);
        }
        private ActionResult LoginLogSearchPostResult(string username, string ip, int id = 1)
        {
            var predicate = PredicateBuilder.True<LoginLog>();

            if (!string.IsNullOrWhiteSpace(username))
            {
                predicate = predicate.And(item => item.LoginUserName.Contains(username));
            }

            if (!string.IsNullOrWhiteSpace(ip))
            {
                predicate = predicate.And(item => item.LoginIp == ip);
            }

            var model = _loginLogRepository.FindList<LoginLog>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_LoginLogSearchResult", model);
            return View(model);
        }
    }
}