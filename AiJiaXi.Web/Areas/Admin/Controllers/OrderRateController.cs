using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.JsonModel;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class OrderRateController : Controller
    {
        private IRepository<OrderRate> _orderRateRepository;
        private ApplicationUserManager _userManager;
        private IRepository<Agency> _agencyRepository;

        public OrderRateController(IRepository<OrderRate> orderRateRepository, ApplicationUserManager userManager, IRepository<Agency> agencyRepository)
        {
            _orderRateRepository = orderRateRepository;
            _userManager = userManager;
            _agencyRepository = agencyRepository;
        }

        // GET: Admin/OrderRate
        public async Task<ActionResult> Index(string orderId,int id = 1)
        {
            var predicate = PredicateBuilder.True<OrderRate>();

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.Order.AgencyId == agencyId);
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            var model = this._orderRateRepository.FindList<OrderRate>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderRateSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderRateSearchPost(string orderNo, string agency, string status, int id = 1)
        {
            return OrderRateSearchResult(orderNo, agency, status, id);
        }

        private ActionResult OrderRateSearchResult(string orderNo, string agency, string status, int id = 1)
        {
            var predicate = PredicateBuilder.True<OrderRate>();
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.Order.AgencyId == agencyId);
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                predicate = predicate.And(item => item.Order.OrderNo == orderNo);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                switch (status)
                {
                    case "-1":
                        break;
                    case "0":
                        predicate = predicate.And(item => item.IsApproval == false);
                        break;
                    case "1":
                        predicate = predicate.And(item => item.IsApproval == true);
                        break;
                    default:
                        throw new Exception($"错误的评论状态({status})！");
                }
            }

            var model = this._orderRateRepository.FindList<OrderRate>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderRateSearchResult", model);
            return View(model);
        }

        public ActionResult Detail(long id = 0)
        {
            var model = _orderRateRepository.Find(item => item.Id == id);
            if (model == null)
            {
                throw new Exception("非法参数！");
            }

            return View(model);
        }

        public ActionResult Approvals()
        {
            var select2Items = new List<Select2Item>()
            {
                new Select2Item() {value = "true", text = "审核通过"},
                new Select2Item() {value = "false", text = "审核不通过"}
            };

            return Json(select2Items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Post(FormCollection form)
        {
            var name = form["name"];
            var pk = form["pk"];
            var value = form["value"];
            var entity = _orderRateRepository.Find(item => item.Id.ToString() == pk);
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (string.Equals("approval", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.IsApproval = bool.Parse(value);
            }

            var result = await _orderRateRepository.UpdateAsync(entity);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id = 0)
        {
            var predicate = PredicateBuilder.True<OrderRate>();
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.Order.AgencyId == agencyId);
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            var entity = await _orderRateRepository.FindAsync(item => item.Id == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该评论不存在，删除失败！');");
            }
            
            var result = await _orderRateRepository.DeleteAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }


            var model = this._orderRateRepository.FindList<OrderRate>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            return PartialView("_OrderRateSearchResult", model);
        }
    }
}