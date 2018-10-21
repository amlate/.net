using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.Location;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;
using Project.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Project.Web.Tests.Controllers
{
    [TestClass]
    public class AjaxControllerTest
    {

        private readonly IRepository<Order> _ordeRepository = new Repository<Order>();

        [TestMethod]
        public async Task setCartProduct()
        {
            var order = new Order
            {
                OrderNo = OrderHelper.GenerateOrderNumber(),
                UserAddressId = 1,
                ApplicationUserId = "00000000-0000-0000-0000-000000000000",
                Appointment = DateTime.Now.AddDays(1),
                TotalPrice = 20,
                Fact = 18,
                Freight = 10,
                RiseTime = DateTime.Now,
                OrderStatus = OrderStatus.ToPay,
                AgencyId = 1,

            };
            for (var i = 0; i < 1; i++)
            {
                var cart = new CartItem
                {
                    OrderItemId = 1,
                    Nums = 2
                };
                order.CartItems.Add(cart);
            }
            var result = await _ordeRepository.AddAsync(order);

            // Assert
            Assert.IsNotNull(result);
        }

    }
}
