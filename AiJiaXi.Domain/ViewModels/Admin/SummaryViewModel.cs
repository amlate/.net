using System;
using System.Collections.Generic;
using System.Linq;
using Project.Domain.Entities;
using Project.Domain.Entities.Orders;
using Project.Domain.Enums;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;

namespace Project.Domain.ViewModels.Admin
{
    public class SummaryViewModel
    {
        private static IRepository<Agency> _agencyRepository = new Repository<Agency>();
        private static IRepository<Order> _orderRepository = new Repository<Order>();

        public string Agency { get; set; }

        public int Goings { get; set; }

        public int Completes { get; set; }

        public int Negative { get; set; }

        public int Favourite { get; set; }

        public int Complaint { get; set; }

        public string Turnover { get; set; }

        public string Total { get; set; }

        public int TotalOrdersCount { get; set; }

        public static List<SummaryViewModel> Get(int year, int month, Agency agency = null)
        {
            var agencies = new List<Agency>();
            if (agency != null)
            {
                agencies.Add(agency);
            }
            else
            {
                agencies = _agencyRepository.FindList<Agency>(item => item.IsValid).ToList();
            }

            var orders_c =
                _orderRepository.FindList<Order>(
                    item => item.CompleteTime != null && item.CompleteTime.Value.Year == year && item.CompleteTime.Value.Month == month).ToList();

            var orders_g =
                _orderRepository.FindList<Order>(
                    item => item.RiseTime.Year == year && item.RiseTime.Month == month).ToList();

            var list = new List<SummaryViewModel>();
            foreach (var ag in agencies)
            {
                list.Add(new SummaryViewModel()
                {
                    Agency = ag.Name,
                    Goings = orders_g.Count(item => item.OrderStatus > OrderStatus.ToPay && item.OrderStatus < OrderStatus.Succeed && item.AgencyId == ag.Id),
                    Completes = orders_c.Count(item => item.OrderStatus == OrderStatus.Succeed && item.AgencyId == ag.Id),
                    Negative = orders_c.Count(item => item.OrderRate != null && item.OrderRate.Stars <= 3 && item.AgencyId == ag.Id),
                    Favourite = orders_c.Count(item => item.OrderRate != null && item.OrderRate.Stars > 3 && item.AgencyId == ag.Id),
                    Turnover = orders_c.Where(item => item.OrderStatus == OrderStatus.Succeed && item.AgencyId == ag.Id).Sum(item => item.Fact).ToString("c"),
                    Complaint = orders_g.Count(item => item.ComplaintType > ComplaintType.None && item.AgencyId == ag.Id),
                    Total = orders_c.Where(item => item.OrderStatus == OrderStatus.Succeed && item.AgencyId == ag.Id).Sum(item => item.TotalPrice).ToString("c"),
                    TotalOrdersCount = orders_g.Count(item => item.AgencyId == ag.Id)
                });
            } 

            return list;
        }
    }
}