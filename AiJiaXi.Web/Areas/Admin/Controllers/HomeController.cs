using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Domain.ViewModels.Admin;
using AiJiaXi.Web.Filters;
using Microsoft.AspNet.Identity;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        private IRepository<JoinApplication> _joinApplicationRepository;
        private IRepository<Order> _orderRepository;

        public HomeController(ApplicationUserManager userManager, IRepository<JoinApplication> joinApplicationRepository,
            IRepository<Order> orderRepository)
        {
            _userManager = userManager;
            _joinApplicationRepository = joinApplicationRepository;
            _orderRepository = orderRepository;
        }

        // GET: Admin/Home
        public async Task<ActionResult> Index()
        {
            var userId = this.User.Identity.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            switch (user.UserType)
            {
                case UserType.Admin:
                    ViewBag.Joins = _joinApplicationRepository.Count(item => item.FeedbackStatus == FeedbackStatus.UnDealed);
                    ViewBag.Users = _userManager.Users.Count(item => item.UserType == UserType.User);
                    ViewBag.Completes = _orderRepository.Count(item => item.OrderStatus == OrderStatus.Succeed);
                    ViewBag.Turnover =
                        _orderRepository.FindList<Order>(item => item.OrderStatus == OrderStatus.Succeed).ToList()
                            .Sum(item => item.Fact);

                    return View("Summary");
                case UserType.Agency:
                    ViewBag.Going =
                        _orderRepository.Count(
                            item =>
                                item.OrderStatus > OrderStatus.ToPay && item.OrderStatus < OrderStatus.Succeed &&
                                item.AgencyId == user.AgencyId);
                    ViewBag.Completes = _orderRepository.Count(item => item.OrderStatus == OrderStatus.Succeed && item.AgencyId == user.AgencyId);
                    ViewBag.Turnover =
                        _orderRepository.FindList<Order>(item => item.OrderStatus == OrderStatus.Succeed && item.AgencyId == user.AgencyId).ToList()
                            .Sum(item => item.Fact);

                    var complaints = _orderRepository.Count(item => item.ComplaintType > ComplaintType.None && item.AgencyId == user.AgencyId);
                    var orders = _orderRepository.Count(item => item.AgencyId == user.AgencyId);
                    ViewBag.Complaints = $"投诉率：{((float)complaints/orders).ToString("p")}, 接到投诉：{complaints} 单";
                    return View("AgencySummary");
                case UserType.Withdrawals:
                    return RedirectToAction("Index", "PromoterInfo");
            }

            return View();
        }


        public ActionResult MonthSummary()
        {
            return PartialView("_MonthSummary", SummaryViewModel.Get(DateTime.Now.Year, DateTime.Now.Month));
        }
    }
}