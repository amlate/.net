using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.PromoterManager;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;
using Project.Domain.Helpers;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;
using Microsoft.Ajax.Utilities;
using Quartz;
using Quartz.Impl;
using Project.Common;

namespace Project.Web
{
    public class OrderAutoDealTaskConfig : IJob
    {
        private IRepository<Order> _orderRepository = new Repository<Order>();
        private IRepository<PromoterInfo> _promoterInfoRepository = new Repository<PromoterInfo>();
        private IRepository<UserAccount> _userAccountRepository = new Repository<UserAccount>();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                // 获得已签收订单列表
                var toConfirmOrders = _orderRepository.FindList<Order>(item => item.OrderStatus == OrderStatus.Dispatched).ToList();

                // 确认时间，便于有需求之后放到配置文件中
                var days = TimeSpan.FromDays(2);
                var ids = new List<long>();
                StringBuilder sb = new StringBuilder(string.Empty);
                var confirmed = new List<Order>();
                // 获得需要自动确认订单的订单列表
                foreach (var order in toConfirmOrders)
                {
                    var lastOrDefault = order.OrderSteps.LastOrDefault(item => true);

                    if (lastOrDefault != null && DateTime.Now - lastOrDefault.RiseTime  >= days)
                    {
                        ids.Add(order.Id);
                        sb.AppendLine(order.Id.ToString());
                        confirmed.Add(order);
                    }
                }

                NLogger.Error($"确认收货:\n{sb.ToString()}");

                if (!ids.Any())
                {
                    NLogger.Error($"没有需要确认收货得订单！");
                }
                _orderRepository.Update(item => ids.Contains(item.Id), u => new Order() { OrderStatus = OrderStatus.Succeed, CompleteTime = DateTime.Now});

                var stepList = ids.Select(item => new OrderStep()
                {
                    OrderId = item,
                    OrderStatus = OrderStatus.Succeed,
                    RiseTime = DateTime.Now,
                    OperationUser = "admin",
                    UserType = UserType.Admin,
                    Note = "后台自动确认订单"
                });

                SqlBulkHelper.SqlBulkCopy(stepList, "AiJiaXi__OrderStep", new[] { "Employee", "EmployeeId", "Order" });

                var temp = new List<PromoterInfo>();
                var account_temp = confirmed.Select(item => item.ApplicationUser.UserAccount).DistinctBy(item => item.Id).ToList();
                foreach (var confirmOrder in confirmed)
                {
                    PromoterInfo entity;
                    var account = account_temp.FirstOrDefault(item => item.Id == confirmOrder.ApplicationUser.Id);
                    if (account != null)
                    {
                        account.Score += (int)confirmOrder.Fact;
                        if (temp.Exists(t => t.FriendsPhone == confirmOrder.ApplicationUser.PhoneNumber))
                        {
                            entity = temp.FirstOrDefault(t => t.FriendsPhone == confirmOrder.ApplicationUser.PhoneNumber);
                        }
                        else if (_promoterInfoRepository.Exist(item => item.FriendsPhone == confirmOrder.ApplicationUser.PhoneNumber))
                        {
                            entity =
                                _promoterInfoRepository.Find(
                                    item => item.FriendsPhone == confirmOrder.ApplicationUser.PhoneNumber);
                        }
                        else
                        {
                            continue;
                        }


                        if (entity != null)
                        {
                            //推广人手机号
                            var myPhone = entity.MyPhone;
                            UserAccount c_account;

                            if (account_temp.Exists(t => t.ApplicationUser.PhoneNumber == myPhone && t.ApplicationUser.UserType == UserType.User))
                            {
                                c_account =
                                    account_temp.First(
                                        t =>
                                            t.ApplicationUser.PhoneNumber == myPhone &&
                                            t.ApplicationUser.UserType == UserType.User);
                            }
                            else if (_userAccountRepository.Exist(t => t.ApplicationUser.PhoneNumber == myPhone && t.ApplicationUser.UserType == UserType.User))
                            {
                                //根据手机号找出推广人用户表信息
                                c_account = _userAccountRepository.Find(t => t.ApplicationUser.PhoneNumber == myPhone && t.ApplicationUser.UserType == UserType.User);

                                account_temp.Add(c_account);
                            }
                            else
                            {
                                continue;
                            }

                            if (c_account != null)
                            {
                                //余额加上实际支付金额的百分之十
                                //myUserAccountModel.Balance = myUserAccountModel.Balance + (findOrder.Fact * decimal.Parse("0.1"));
                                c_account.CommissionMoney += (confirmOrder.Fact * decimal.Parse("0.1"));
                            }
                        }
                    }
                }

                if (account_temp.Any())
                {
                    account_temp.ForEach(item => _userAccountRepository.Update(item));
                }
            }
            catch (Exception ex)
            {
                NLogger.Error($"确认收货出错:\n{ex.StackTrace}");

            }
        }

        /// <summary>
        /// 注册调度任务
        /// </summary>
        public static void RegisterTasks()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<OrderAutoDealTaskConfig>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}