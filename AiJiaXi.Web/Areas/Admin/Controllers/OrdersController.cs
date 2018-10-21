using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Entities.Location;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;
using Project.Domain.Helpers;
using Project.Domain.JsonModel;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class OrdersController : Controller
    {
        private IRepository<Order> _orderRepository;
        private ApplicationUserManager _userManager;
        private IRepository<Agency> _agencyRepository;
        private IRepository<City> _cityRepository;
        private IRepository<UserAccount> _userAccountRepository;

        public OrdersController(IRepository<Order> orderRepository, ApplicationUserManager userManager,
            IRepository<Agency> agencyRepository, IRepository<City> cityRepository,
            IRepository<UserAccount> userAccountRepository)
        {
            ViewBag.IsAgency = false;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _agencyRepository = agencyRepository;
            _cityRepository = cityRepository;
            _userAccountRepository = userAccountRepository;
        }

        // GET: Admin/Orders
        public ActionResult Index(int id = 1)
        {
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid);
            var predicate = PredicateBuilder.True<Order>().And(item => item.OrderStatus != OrderStatus.ToConfirm);

            var userId = User.Identity.GetUserId();
            var user =  this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
                agencies = agencies.Where(item => manages.Contains(item.Id));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }
            ViewBag.Agencies = new SelectList(agencies.ToList(), "Id", "Name");

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderSearchPost(string agency, string status, string orderNo, int id = 1)
        {
            return OrderSearchResult(agency, status, orderNo, id);
        }

        private  ActionResult OrderSearchResult(string agency, string status, string orderNo, int id = 11)
        {
            var predicate = PredicateBuilder.True<Order>().And(item => item.OrderStatus != OrderStatus.ToConfirm);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            if (!string.IsNullOrWhiteSpace(agency))
            {
                predicate = predicate.And(item => item.AgencyId.ToString() == agency);
            }

            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                predicate = predicate.And(item => item.OrderNo == orderNo);
            }

            if ((!string.IsNullOrWhiteSpace(status)) && status != "-1")
            {
                var os = (OrderStatus)Enum.Parse(typeof(OrderStatus), status, true);
                predicate = predicate.And(item => item.OrderStatus == os);

            }

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderSearchResult", model);
            return View(model);
        }

        #region 待支付订单

        public ActionResult Topay(int id = 1)
        {
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid);
            var predicate = PredicateBuilder.True<Order>().And(item => item.OrderStatus == OrderStatus.ToPay);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
                agencies = agencies.Where(item => manages.Contains(item.Id));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }
            ViewBag.Agencies = new SelectList(agencies.ToList(), "Id", "Name");

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderToPaySearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderToPaySearchPost(string agency, string status, string orderNo, int id = 1)
        {
            return OrderToPaySearchResult(agency, status, orderNo, id);
        }

        private ActionResult OrderToPaySearchResult(string agency, string status, string orderNo, int id = 1)
        {
            var predicate = PredicateBuilder.True<Order>().And(item => item.OrderStatus == OrderStatus.ToPay);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            if (!string.IsNullOrWhiteSpace(agency))
            {
                predicate = predicate.And(item => item.AgencyId.ToString() == agency);
            }

            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                predicate = predicate.And(item => item.OrderNo == orderNo);
            }

            if ((!string.IsNullOrWhiteSpace(status)) && status != "-1")
            {
                var os = (OrderStatus)Enum.Parse(typeof(OrderStatus), status, true);
                predicate = predicate.And(item => item.OrderStatus == os);
            }

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderGoingSearchResult", model);
            return View(model);
        }

        #endregion


        #region 进行中订单
        public ActionResult Going(int id = 1)
        {
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid);
            var predicate =
                PredicateBuilder.True<Order>()
                    .And(
                        item =>
                            item.OrderStatus == OrderStatus.ToPatch ||
                            item.OrderStatus == OrderStatus.Patched || item.OrderStatus == OrderStatus.InService ||
                            item.OrderStatus == OrderStatus.ToDispatch || item.OrderStatus == OrderStatus.Dispatching ||
                            item.OrderStatus == OrderStatus.Dispatched);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
                agencies = agencies.Where(item => manages.Contains(item.Id));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }
            ViewBag.Agencies = new SelectList(agencies.ToList(), "Id", "Name");

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderGoingSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderGoingSearchPost(string agency, string status, string orderNo, int id = 1)
        {
            return OrderGoingSearchResult(agency, status, orderNo, id);
        }

        private ActionResult OrderGoingSearchResult(string agency, string status, string orderNo, int id = 1)
        {
            var predicate =
                PredicateBuilder.True<Order>()
                    .And(
                        item =>
                            item.OrderStatus == OrderStatus.ToPatch ||
                            item.OrderStatus == OrderStatus.Patched || item.OrderStatus == OrderStatus.InService ||
                            item.OrderStatus == OrderStatus.ToDispatch || item.OrderStatus == OrderStatus.Dispatching ||
                            item.OrderStatus == OrderStatus.Dispatched);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }


            if (!string.IsNullOrWhiteSpace(agency))
            {
                predicate = predicate.And(item => item.AgencyId.ToString() == agency);
            }

            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                predicate = predicate.And(item => item.OrderNo == orderNo);
            }

            if ((!string.IsNullOrWhiteSpace(status)) && status != "-1" )
            {
                var os = (OrderStatus)Enum.Parse(typeof(OrderStatus), status, true);
                predicate = predicate.And(item => item.OrderStatus == os);

            }

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderGoingSearchResult", model);
            return View(model);
        }

        #endregion

        #region 已完成订单

        public ActionResult Complete(int id = 1)
        {
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid);
            var predicate = PredicateBuilder.True<Order>().And(item => item.OrderStatus == OrderStatus.Succeed);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
                agencies = agencies.Where(item => manages.Contains(item.Id));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }
            ViewBag.Agencies = new SelectList(agencies.ToList(), "Id", "Name");

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.CompleteTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderComplteSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderComplteSearchPost(string agency, string status, string orderNo, int id = 1)
        {
            return OrderComplteSearchResult(agency, status, orderNo, id);
        }

        private ActionResult OrderComplteSearchResult(string agency, string status, string orderNo, int id = 1)
        {
            var predicate = PredicateBuilder.True<Order>().And(item => item.OrderStatus == OrderStatus.Succeed);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            if (!string.IsNullOrWhiteSpace(agency))
            {
                predicate = predicate.And(item => item.AgencyId.ToString() == agency);
            }

            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                predicate = predicate.And(item => item.OrderNo == orderNo);
            }

            if ((!string.IsNullOrWhiteSpace(status)) && status != "-1")
            {
                var os = (OrderStatus)Enum.Parse(typeof(OrderStatus), status, true);
                predicate = predicate.And(item => item.OrderStatus == os);

            }

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.CompleteTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderComplteSearchResult", model);
            return View(model);
        }
        #endregion

        #region 已取消订单

        public  ActionResult Cancel(int id = 1)
        {
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid);
            var predicate = PredicateBuilder.True<Order>().And(item => item.OrderStatus == OrderStatus.Cancelled);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
                agencies = agencies.Where(item => manages.Contains(item.Id));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }
            ViewBag.Agencies = new SelectList(agencies.ToList(), "Id", "Name");

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderCancelSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderCancelSearchPost(string agency, string status, string orderNo, int id = 1)
        {
            return OrderCancelSearchResult(agency, status, orderNo, id);
        }

        private ActionResult OrderCancelSearchResult(string agency, string status, string orderNo, int id = 1)
        {
            var predicate = PredicateBuilder.True<Order>().And(item => item.OrderStatus == OrderStatus.Cancelled);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            if (!string.IsNullOrWhiteSpace(agency))
            {
                predicate = predicate.And(item => item.AgencyId.ToString() == agency);
            }

            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                predicate = predicate.And(item => item.OrderNo == orderNo);
            }

            if ((!string.IsNullOrWhiteSpace(status)) && status != "-1")
            {
                var os = (OrderStatus)Enum.Parse(typeof(OrderStatus), status, true);
                predicate = predicate.And(item => item.OrderStatus == os);

            }

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderCancelSearchResult", model);
            return View(model);
        }

        #endregion

         #region 退款订单

        public ActionResult Refund(int id = 1)
        {
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid);
            var predicate =
                PredicateBuilder.True<Order>()
                    .And(
                        item =>
                            item.OrderStatus == OrderStatus.ToRefund || item.OrderStatus == OrderStatus.Refundding ||
                            item.OrderStatus == OrderStatus.Refunded);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
                agencies = agencies.Where(item => manages.Contains(item.Id));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }
            ViewBag.Agencies = new SelectList(agencies.ToList(), "Id", "Name");

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderRefundSearchResult", model);
            return View(model);
        }


        [HttpPost]
        public ActionResult OrderRefundSearchPost(string agency, string status, string orderNo, int id = 1)
        {
            return OrderRefundSearchResult(agency, status, orderNo, id);
        }

        private ActionResult OrderRefundSearchResult(string agency, string status, string orderNo, int id = 1)
        {
            var predicate =
                PredicateBuilder.True<Order>()
                    .And(
                        item =>
                            item.OrderStatus == OrderStatus.ToRefund || item.OrderStatus == OrderStatus.Refundding ||
                            item.OrderStatus == OrderStatus.Refunded);

            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType == UserType.Agency)
            {
                var agencyId = user.Agency.Id;
                predicate = predicate.And(item => item.AgencyId == agencyId);
                ViewBag.IsAgency = true;
            }
            else if (user.UserType == UserType.Washers)
            {
                var manages = user.Remark.Split(',').Select(long.Parse);
                predicate = predicate.And(item => manages.Contains(item.AgencyId));
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            if (!string.IsNullOrWhiteSpace(agency))
            {
                predicate = predicate.And(item => item.AgencyId.ToString() == agency);
            }

            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                predicate = predicate.And(item => item.OrderNo == orderNo);
            }

            if ((!string.IsNullOrWhiteSpace(status)) && status != "-1")
            {
                var os = (OrderStatus)Enum.Parse(typeof(OrderStatus), status, true);
                predicate = predicate.And(item => item.OrderStatus == os);

            }

            var model = this._orderRepository.FindList<Order>(predicate).OrderByDescending(a => a.RiseTime).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_OrderRefundSearchResult", model);
            return View(model);
        }

        #endregion


        /// <summary>
        /// 未支付订单可以取消
        /// </summary>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        public async Task<ActionResult> CancelOrder(long id)
        {
            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            ViewBag.StepTitle = "订单取消原因";
            ViewBag.ActionName = "CancelOrder";
            return PartialView("_OrderStep", order);
        }

        /// <summary>
        /// 未支付订单可以取消
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelOrder(FormCollection collection, long id)
        {
            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            string script = string.Empty;
            // 未支付订单可以取消
            if (order.OrderStatus == OrderStatus.ToPay)
            {
                order.OrderStatus = OrderStatus.Cancelled;
                order.OrderSteps.Add(new OrderStep()
                {
                    OrderStatus = order.OrderStatus,
                    RiseTime = DateTime.Now,
                    OperationUser = user.UserName,
                    UserType = user.UserType,
                    Note = collection["note"]
                });
                var result = await _orderRepository.UpdateAsync(order);
                
                script = result ? "layer.alert('操作成功！', function(){$('#OrderModal').modal('hide'))" : "layer.alert('操作失败，请联系管理员！'";

            }
            return RedirectToAction("Cancel");
        }

        public async Task<ActionResult> ConfirmPayed(long id)
        {
            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            ViewBag.StepTitle = "确认支付说明";
            ViewBag.ActionName = "ConfirmPayed";
            return PartialView("_OrderStep", order);
        }

        /// <summary>
        /// 确认收到订单款项（手动确认，客户端应有确认的步骤）
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmPayed(FormCollection collection, long id)
        {
            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            // 特殊情况下有管理员进行订单支付确认
            string script = String.Empty;
            order.OrderStatus = OrderStatus.ToPatch;
            order.OrderSteps.Add(new OrderStep()
            {
                OrderStatus = order.OrderStatus,
                RiseTime = DateTime.Now,
                OperationUser = user.UserName,
                UserType = user.UserType,
                Note = collection["note"]
            });
            var result = await _orderRepository.UpdateAsync(order);

            script = result ? "layer.alert('操作成功！', function(){$('#OrderModal').modal('hide'))" : "layer.alert('操作失败，请联系管理员！'";

            return RedirectToAction("Going");
        }

        public async Task<ActionResult> Refunding(long id)
        {


            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            ViewBag.StepTitle = "进入退款程序";
            ViewBag.ActionName = "Refunding";
            return PartialView("_OrderStep", order);
        }

        /// <summary>
        /// 待退款订单进入 退款程序
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Refunding(FormCollection collection, long id)
        {
            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            // 将订单状态改为待退款
            string script = string.Empty;
            order.OrderStatus = OrderStatus.Refundding;
            order.OrderSteps.Add(new OrderStep()
            {
                OrderStatus = order.OrderStatus,
                RiseTime = DateTime.Now,
                OperationUser = user.UserName,
                UserType = user.UserType,
                Note = collection["note"]
            });
            var result = await _orderRepository.UpdateAsync(order);


            script = result ? "layer.alert('操作成功！', function(){$('#OrderModal').modal('hide'))" : "layer.alert('操作失败，请联系管理员！'";

            return RedirectToAction("Refund");
        }

        public async Task<ActionResult> Refunded(long id)
        {


            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            ViewBag.StepTitle = "退款完成情况说明";
            ViewBag.ActionName = "Refunded";
            return PartialView("_OrderStep", order);
        }

        /// <summary>
        /// 退款中的订单确认退款完成
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Refunded(FormCollection collection, long id)
        {
            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }
            // 将订单状态改为待退款
            order.OrderStatus = OrderStatus.Refunded;
            order.OrderSteps.Add(new OrderStep()
            {
                OrderStatus = order.OrderStatus,
                RiseTime = DateTime.Now,
                OperationUser = user.UserName,
                UserType = user.UserType,
                Note = collection["note"]
            });
            var result = await _orderRepository.UpdateAsync(order);
            string script = String.Empty;
            script = result ? "layer.alert('操作成功！', function(){$('#OrderModal').modal('hide'))" : "layer.alert('操作失败，请联系管理员！'";

            return RedirectToAction("Refund");
        }

        public async Task<ActionResult> NextStep(long id)
        {
            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            ViewBag.StepTitle = "订单流转说明";
            ViewBag.ActionName = "NextStep";
            return PartialView("_OrderStep", order);
        }

        /// <summary>
        /// 进行中订单进行下一步处理
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NextStep(FormCollection collection, long id)
        {
            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            // 将订单状态改为待退款
            string script = String.Empty;
            var status = (int) order.OrderStatus;
            if (status >= 1 && status <= 5)
            {
                order.OrderStatus = (OrderStatus)(status + 1);

                // 已完成订单添加 完成时间
                if (order.OrderStatus == OrderStatus.Succeed)
                {
                    order.CompleteTime = DateTime.Now;
                }

                order.OrderSteps.Add(new OrderStep()
                {
                    OrderStatus = order.OrderStatus,
                    RiseTime = DateTime.Now,
                    OperationUser = user.UserName,
                    UserType = user.UserType,
                    Note = collection["note"]
                });
                var result = await _orderRepository.UpdateAsync(order);
            }

            return RedirectToAction("Going");
        }

        public async Task<ActionResult> GoRefund(long id)
        {


            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            ViewBag.StepTitle = "退款原因说明";
            ViewBag.ActionName = "GoRefund";
            return PartialView("_OrderStep", order);
        }

        /// <summary>
        /// 进行中的订单不能取消，提交退款申请（此步骤前台考虑也要有）
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="id">订单id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GoRefund(FormCollection collection, long id)
        {
            var order = await _orderRepository.FindAsync(item => item.Id == id);
            if (order == null)
            {
                throw new Exception("不存在的订单！");
            }

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
                if (user.AgencyId != order.AgencyId)
                {
                    throw new Exception("代理商只能管理自己的订单！");
                }
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            // 将订单状态改为待退款
            order.OrderStatus = OrderStatus.ToRefund;
            order.OrderSteps.Add(new OrderStep()
            {
                OrderStatus = order.OrderStatus,
                RiseTime = DateTime.Now,
                OperationUser = user.UserName,
                UserType = user.UserType,
                Note = collection["note"]
            });
            var result = await _orderRepository.UpdateAsync(order);

            string script = String.Empty;
            script = result ? "layer.alert('操作成功！', function(){$('#OrderModal').modal('hide'))" : "layer.alert('操作失败，请联系管理员！'";

            return RedirectToAction("Refund");
        }

        public ActionResult Batch(string id)
        {
            var ids = id.TrimEnd(',').Split(',').Select(long.Parse);
            var model = _orderRepository.FindList<Order>(item => ids.Contains(item.Id)).ToList();
            return PartialView("_BatchOrderStep", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BatchOrderSteps(FormCollection form)
        {
            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            if (user.UserType == UserType.Agency)
            {
            }
            else if (user.UserType == UserType.User || user.UserType == UserType.Withdrawals)
            {
                throw new Exception("推广员和前台用户无权执行此操作！");
            }

            var orderIds = form["orders"].TrimEnd(',').Split(',').Select(long.Parse);
            var status = form["status"];
            var note = form["note"];


            var order_status = (OrderStatus) Enum.Parse(typeof (OrderStatus), status, true);
            _orderRepository.Update(item => orderIds.Contains(item.Id), u => new Order()
            {
                OrderStatus = order_status
            });

            var list = orderIds.Select(item => new OrderStep()
            {
                OrderStatus = order_status,
                RiseTime = DateTime.Now,
                OperationUser = user.UserName,
                UserType = user.UserType,
                Note = note,
                OrderId = item
            });
            
            SqlBulkHelper.SqlBulkCopy(list, "AiJiaXi__OrderStep", new []{ "Employee", "EmployeeId", "Order" });

            return RedirectToAction("Going");
        }

        public ActionResult ComplaintTypes()
        {
            var select2Items = (from object value in Enum.GetValues(typeof(ComplaintType))
                                select new Select2Item() { value = value.ToString(), text = value.GetDescription() }).ToList();

            return Json(select2Items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Post(FormCollection form)
        {
            var name = form["name"];
            var pk = form["pk"];
            var value = form["value"];
            var entity = _orderRepository.Find(item => item.Id.ToString() == pk);
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (string.Equals("complaint", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.ComplaintType = (ComplaintType)Enum.Parse(typeof(ComplaintType), value, true);
            }

            if (string.Equals("note", name, StringComparison.CurrentCultureIgnoreCase))
            {
                entity.ComplaintNote = value;
            }

            var result = await _orderRepository.UpdateAsync(entity);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}