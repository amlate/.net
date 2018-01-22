using System;
using System.Collections.Generic;
using System.Linq;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.Location;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Entities.PromoterManager;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.Repositories.Impl;
using AiJiaXi.Domain.Repositories.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AiJiaXi.Web.Tests.Controllers
{
    [TestClass()]
    public class FrontPageControllerTests
    {
        [TestMethod()]
        public void AddUserTest()
        {
            IRepository<UserLocation> _userLocationRepository = new Repository<UserLocation>();
            IRepository<County> _countyRepository = new Repository<County>();
            IRepository<City> _cityRepository = new Repository<City>();
            IRepository<Agency> _agencyRepository = new Repository<Agency>();
            IRepository<Event> _eventRepository = new Repository<Event>();
            IRepository<Voucher> _voucheRepository = new Repository<Voucher>();
            int count = 0;
            var userLocation = _userLocationRepository.Find(u => u.FromUserName == "oFZzeshY4KsD1tKYdBFs9csUA95Y");
            if (userLocation != null)
            {
                // 判断当前城市是否有注册赠送优惠券的活动
                var findCity = _cityRepository.Find(c => c.Name == userLocation.LocationCityName);
                //获取当前城市的所有区,通过区域去查询代理商，看代理商们是否有对应的活动，
                if (findCity != null)
                {
                    var vouchers = new List<Voucher>();
                    var countyIdList = _countyRepository.FindList<City>(c => c.Id - findCity.Id < 100).Select(c => c.Id);
                    var agencyList = _agencyRepository.FindList<Agency>(a => countyIdList.Contains(a.CountyId)).ToList();
                    foreach (var item in agencyList)
                    {
                        var findEvent = _eventRepository.Find(
                              e => e.Flag && e.EventType == EventType.Register && e.ApplyStatus == ApplyStatus.Reviewed && e.StartTime < DateTime.Now && e.EndTime > DateTime.Now && e.Agencies.Contains(item));
                        if (findEvent != null)
                        {//如果包含相应的活动，那么去优惠券里查询相应的活动，找对应的id,useraccountid为空的一个分配给注册者
                            var findVoucher = _voucheRepository.Find(
                                  v => v.EventId == findEvent.Id && string.IsNullOrEmpty(v.UserAccountId));
                            if (findVoucher != null)
                            {
                                count++;
                                findVoucher.UserAccount = null;
                                findVoucher.UserAccountId = null;
                                findVoucher.IsOccu = true;
                                _voucheRepository.UpdateAsync(findVoucher);
                            }
                        }
                    }

                }
            }
            Assert.Fail();

        }
    }
}