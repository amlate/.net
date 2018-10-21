using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Project.Domain.Entities;
using Project.Domain.Enums;
using Project.Domain.ViewModels.Admin;
using Project.Web.Filters;
using Microsoft.AspNet.Identity;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SummaryController : Controller
    {
        private ApplicationUserManager _userManager;

        public SummaryController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        // GET: Admin/Summary
        public ActionResult Turnnover(string year, string month)
        {
            int year_int = DateTime.Now.Year;
            int month_int = DateTime.Now.Month;

            if ((!string.IsNullOrWhiteSpace(year)) && year.IsInt())
            {
                year_int = int.Parse(year);
            }

            if ((!string.IsNullOrWhiteSpace(month)) && month.IsInt())
            {
                month_int = int.Parse(month);
            }

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);

            Agency agency = null;

            if (user.UserType == UserType.Agency)
            {
                agency = user.Agency;
            }

            ViewBag.Date = $"{year_int}年{month_int}月";
            var model = SummaryViewModel.Get(year_int, month_int, agency);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_TurnoverPartial", model);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult TurnnoverSearchPost(string year, string month)
        {
            int year_int = DateTime.Now.Year;
            int month_int = DateTime.Now.Month;

            if ((!string.IsNullOrWhiteSpace(year)) && year.IsInt())
            {
                year_int = int.Parse(year);
            }

            if ((!string.IsNullOrWhiteSpace(month)) && month.IsInt())
            {
                month_int = int.Parse(month);
            }
            
            ViewBag.Date = $"{year_int}年{month_int}月";
            var model = SummaryViewModel.Get(year_int, month_int);
            return PartialView("_TurnoverPartial", model);
        }
    }
}