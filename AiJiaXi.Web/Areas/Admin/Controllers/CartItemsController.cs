using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class CartItemsController : Controller
    {
        private IRepository<Order> _orderRepository;

        public CartItemsController(IRepository<Order> orderRepository)
        {
            this._orderRepository = orderRepository;
        }


        // GET: Admin/CartItems
        public ActionResult Index(long id)
        {
            var order = _orderRepository.Find(item => item.Id == id);

            return View(order);
        }
    }
}