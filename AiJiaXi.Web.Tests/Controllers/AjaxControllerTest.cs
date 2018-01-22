using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Entities.Location;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.Repositories.Impl;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AiJiaXi.Web.Tests.Controllers
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
