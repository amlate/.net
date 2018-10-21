using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Entities.Location;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Webdiyer.WebControls.Mvc;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class LocationController : Controller
    {
        private IRepository<Province> _provinceRepository;
        private IRepository<City> _cityRepository;
        private IRepository<County> _countyRepository;
        private IRepository<Agency> _agencyRepository;

        public LocationController(IRepository<Province> provinceRepository, IRepository<City> cityRepository,
            IRepository<County> countyRepository, IRepository<Agency> agencyRepository)
        {
            _provinceRepository = provinceRepository;
            _cityRepository = cityRepository;
            _countyRepository = countyRepository;
            _agencyRepository = agencyRepository;
        }

        public JsonResult Provinces()
        {
            var provinces = _provinceRepository.FindList<Province>(item => true).ToList();

            return Json(provinces, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Cities(int id)
        {
            var cities = LocationHelper.GetCities(id);

            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Counties(int id)
        {
            var counties = LocationHelper.GetCounties(id);

            return Json(counties, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int id = 1)
        {
            var provinces = LocationHelper.GetProvinces(true);
            var counties = LocationHelper.GetCounties(true);
            var model = new Tuple<IEnumerable<Province>, IEnumerable<County>>(provinces, counties);
            
            var provinces_s = _provinceRepository.FindList<Province>(item => true).ToList();
            ViewBag.Provinces = new SelectList(provinces_s, "Id", "Name");

            if (Request.IsAjaxRequest())
                return PartialView("_LocationSearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult LocationSearchPost(string province, int id = 1)
        {
            return LocationSearchPostResult(province, id);
        }
        private ActionResult LocationSearchPostResult(string province, int id = 1)
        {
            var provinces = LocationHelper.GetProvinces(true);
            var counties = LocationHelper.GetCounties(true);

            if (!string.IsNullOrWhiteSpace(province))
            {
                provinces = provinces.Where(item => item.Id.ToString() == province);
            }

            var model = new Tuple<IEnumerable<Province>, IEnumerable<County>>(provinces, counties); 
            if (Request.IsAjaxRequest())
                return PartialView("_LocationSearchResult", model);
            return View(model);
        }
    }
}