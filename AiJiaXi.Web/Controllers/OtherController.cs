using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Plugin;
using AiJiaXi.Plugin.WeiXin;
using AiJiaXi.Domain;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.IdentityModel;
using AiJiaXi.Domain.Entities.Location;
using AiJiaXi.Domain.Entities.Orders;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Repositories.Impl;
using AiJiaXi.Domain.Repositories.Interface;
using ZhiYuan.IAR.Repository.EF;
using AiJiaXi.Domain.Entities.PromoterManager;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities.Configs;

namespace AiJiaXi.Web.Controllers
{

    //LC专用Controller
    public class OtherController : Controller
    {
        private readonly IRepository<City> _cityRepository;

        private readonly IRepository<County> _countyRepository;
        public OtherController(IRepository<City> cityRepository, IRepository<County> countyRepository)
        {
            _cityRepository = cityRepository;
            _countyRepository = countyRepository;
        }

        /// <summary>
        /// 获取当前定位后，返回城市，区域  ID  和名称
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetCity(string cityName,string countyName)
        {
            //var a = Request["cityName"];
            //var b = Request["countyName"];
            //var t = Request["id"];
            //var d = Request["count"];

            long cityId = 0;
            long countyId = 0;
            // 如果城市名称不为空，说明是根据定位发来信息
            if (!String.IsNullOrWhiteSpace(cityName))
            {
                var findCityT = await _cityRepository.FindAsync(c => c.Name.Contains(cityName));
                if (findCityT != null)
                {
                    cityId = findCityT.Id;
                }
            }
            //如果区域名称不为空，说明是根据定位发来信息
            if (!String.IsNullOrWhiteSpace(countyName))
            {
                var findCountyT = await _countyRepository.FindAsync(c => c.Name.Contains(countyName));
                if (findCountyT != null)
                {
                    countyId = findCountyT.Id;
                }
            }


            //ViewBag.cityId = cityId;
            //ViewBag.countyId = countyId;
            //ViewBag.cityName = cityName;
            //ViewBag.countyName = countyName;


            return Json(new { cityId = cityId, countyId = countyId, cityName= cityName, countyName= countyName }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 跳转专用，
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            //如果为空，那个提示用正确的方式打开
            //var findUser = _applicationUserRepository.Find(a => true);

            if (id == "1")// baixime/Index/
            {
                ViewBag.id = id;
            }
            else
            {
                ViewBag.id = "999";
            }

                return View();
        }
    }
}