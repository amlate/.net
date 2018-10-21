using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Plugin;
using Project.Plugin.WeiXin;
using Project.Domain;
using Project.Domain.Entities;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.Location;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;
using ZhiYuan.IAR.Repository.EF;
using Project.Common;
using System.Net.Http;
using Newtonsoft.Json;
using Project.Domain.Entities.PromoterManager;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace Project.Web.Controllers
{
    public class BaiximeController : Controller
    {

        private readonly IRepository<Agency> _agencyRepository;

        private readonly IRepository<City> _cityRepository;

        private readonly IRepository<County> _countyRepository;

        private readonly IRepository<ApplicationUser> _applicationRepository;
        /// <summary>
        /// 产品类别
        /// </summary>
        private readonly IRepository<OrderItemClass> _orderitemclassRepository;

        /// <summary>
        /// 产品
        /// </summary>
        private readonly IRepository<OrderItem> _orderitemRepository;

        //获得用户当前位置
        private readonly IRepository<UserLocation> _userLocationRepository;


        /// <summary>
        /// 订单详细
        /// </summary>
        private readonly IRepository<CartItem> _cartitemRepository;

        /// <summary>
        /// 订单信息
        /// </summary>
        private readonly IRepository<Order> _orderRepository;

        private readonly IRepository<UserAddress> _useraddressRepository;

        public BaiximeController(IRepository<ApplicationUser> applicationRepository, IRepository<UserLocation> userLocationRepository, IRepository<Agency> agencyRepository, IRepository<City> cityRepository, IRepository<County> countyRepository, IRepository<OrderItemClass> orderitemclassRepository, IRepository<OrderItem> orderitemRepository, IRepository<CartItem> cartitemRepository, IRepository<Order> orderRepository, IRepository<UserAddress> useraddressRepository)
        {
            _cityRepository = cityRepository;
            _countyRepository = countyRepository;
            _orderitemclassRepository = orderitemclassRepository;
            _orderitemRepository = orderitemRepository;
            _agencyRepository = agencyRepository;
            _userLocationRepository = userLocationRepository;
            _cartitemRepository = cartitemRepository;
            _orderRepository = orderRepository;
            _useraddressRepository = useraddressRepository;
            _applicationRepository = applicationRepository;
        }


        // GET: FrontPage
        /// <summary>
        /// 首页信息商品展示页
        /// </summary>
        ///<param name="type">1代表传入的是ID，2代表传入的是名称</param>
        /// <param name="cityIdParm">城市[ID/名称]，</param>
        /// <param name="countyIdParm">区域[ID/名称]，</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string type, string cityIdParm, string countyIdParm, string OpenId)
        {
            //if (!WeiXinVerification())
            //{
            //    return RedirectToAction("IsNotWeiXin");
            //}

            //LC注释掉以下名称
            //cityName = string.IsNullOrEmpty(cityName) ? "大连市" : cityName;
            //countyName = string.IsNullOrEmpty(countyName) ? "西岗区" : countyName;


            //获取OpenId用于查询条件用【注意】
            //string OpenId = String.IsNullOrWhiteSpace(Request.QueryString["OpenId"])?"":Request.QueryString["OpenId"];
            ViewBag.OpenId = OpenId;
            NLogger.Debug("Index_OpenId:" + OpenId);


            #region LC增加以下内容
            //判断用户是否注册过
            string login = "";
            var userModel = _applicationRepository.Find(t => t.OpenId == OpenId);
            if (userModel != null)
            {
                login = "1";
            }
            ViewBag.login = login;


            long cityId = 0;
            long countyId = 0;

            //如果为空说明是第一次请求，没有任何操作
            if (String.IsNullOrWhiteSpace(type))
            {
                //cityId = 210200;
                //countyId = 210202;
            }
            else
            {
                if (type == "1")//传入是ID
                {
                    cityId = String.IsNullOrWhiteSpace(cityIdParm) ? 0 : long.Parse(cityIdParm);
                    countyId = String.IsNullOrWhiteSpace(countyIdParm) ? 0 : long.Parse(countyIdParm);

                }
                if (type == "2")//传入是名称[定位来的]
                {
                    // 如果城市名称不为空，说明是根据定位发来信息
                    if (!String.IsNullOrWhiteSpace(cityIdParm))
                    {
                        var findCityT = await _cityRepository.FindAsync(c => c.Name.Contains(cityIdParm));
                        if (findCityT != null)
                        {
                            cityId = findCityT.Id;
                        }
                    }
                    //如果区域名称不为空，说明是根据定位发来信息
                    if (!String.IsNullOrWhiteSpace(countyIdParm))
                    {
                        var findCountyT = await _countyRepository.FindAsync(c => c.Name.Contains(countyIdParm));
                        if (findCountyT != null)
                        {
                            countyId = findCountyT.Id;
                        }
                    }
                }

            }
            #endregion



            ViewBag.data = _cityRepository.FindList<City>(c => true);
            var findCity = await _cityRepository.FindAsync(c => c.Id == cityId);


            #region LC增加以下内容
            var findCounty = await _countyRepository.FindAsync(c => c.Id == countyId);
            ViewBag.cityId = cityId.ToString();//城市ID
            ViewBag.countyId = countyId.ToString();//区域ID

            if (findCity != null)
            {
                ViewBag.cityName = findCity.Name;
            }
            else
            {
                ViewBag.cityName = "定位中";
            }
            if (findCounty != null)
            {
                ViewBag.countyName = findCounty.Name;
            }
            else
            {
                ViewBag.countyName = "定位中";
            }


            if (type == "1")//正常选择城市
            {
                //更新定位数据
                var userLocationModel = _userLocationRepository.Find(t => t.FromUserName == OpenId);
                if (userLocationModel != null)
                {
                    userLocationModel.ToUserName = "999";//999代表已经选择过城市                   
                    userLocationModel.CityName = findCity.Name;
                    userLocationModel.CountyName = findCounty.Name;
                    _userLocationRepository.Update(userLocationModel);

                }
                else
                {
                    //保存数据
                    UserLocation ul = new UserLocation();
                    ul.FromUserName = OpenId;//OpenId
                    ul.ToUserName = "999";
                    ul.CreateTime = DateTime.Now;
                    ul.CityName = findCity.Name;
                    ul.CountyName = findCounty.Name;

                    _userLocationRepository.Add(ul);
                }
            }


            #endregion


            //CityId是7.4日LC增加城市条件
            //var Class = _orderitemclassRepository.FindList<OrderItemClass>(c => true && c.CityId == cityId).OrderBy(a => a.Order).ToList();
            //服务范围
            var agencyLi = _agencyRepository.FindList<Agency>(t => t.IsValid).Distinct().OrderBy(k => k.CityId);
            ViewBag.Range = agencyLi;

            //查出当前开通的区域ID          
            var CountyLi = agencyLi.Select(t => t.CountyId);
             

            var Class = _orderitemclassRepository.FindList<OrderItemClass>(c => true && c.IsValid == true &&c.CityId==cityId && CountyLi.Contains((int)countyId) && c.Counties.Contains(countyId.ToString())).OrderBy(a => a.Order ).ToList();
            ViewBag.Class = Class;
            var CIds = Class.Select(a => a.Id).ToArray();

            var OrderItem = _orderitemRepository.FindList<OrderItem>(a => CIds.Contains(a.ItemClassId) && a.IsValid == true).GroupBy(a => a.ItemClassId).ToList();
            List<IGrouping<long, OrderItem>> OrderItemList = new List<IGrouping<long, OrderItem>>();
            foreach (var cid in CIds)
            {
                OrderItemList.Add(OrderItem.Find(a => a.Key == cid));
            }
            ViewBag.OrderItem = OrderItemList;

          

            NLogger.Debug("Index_165:");
            return View(findCity);
        }

        /// <summary>
        /// 生成订单
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GenerateOrder(long id,string countyId)
        {
            ViewBag.CountyId = countyId;
            ViewBag.OrderId = id;
            var order = _orderRepository.Find(a => a.Id == id);
            ViewBag.Order = order;
            ViewBag.CartItem = order.CartItems.ToList();
            var Ids = order.CartItems.Select(a => a.OrderItemId).ToArray();
            var list = _orderitemRepository.FindList<OrderItem>(a => Ids.Contains(a.Id)).ToList();
            
            ViewBag.AddressList = _useraddressRepository.FindList<UserAddress>(u => u.ApplicationUserId == order.ApplicationUserId&&u.IsShow&&u.Note== countyId).ToList();
            return View(list);
        }
    }
}