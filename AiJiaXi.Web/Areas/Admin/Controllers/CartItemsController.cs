using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Domain.Entities.Orders;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;

namespace Project.Web.Areas.Admin.Controllers
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