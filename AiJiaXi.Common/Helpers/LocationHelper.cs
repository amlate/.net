using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Project.Domain.Entities;
using Project.Domain.Entities.Location;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;

namespace Project.Common.Helpers
{
    public class LocationHelper
    {
        private static IRepository<Province> _provinceRepository = new Repository<Province>();
        private static IRepository<City> _cityRepository = new Repository<City>();
        private static IRepository<County> _countyRepository = new Repository<County>();
        private static IRepository<Agency> _agencyReposity = new Repository<Agency>();

        public static string GetCityByCode(int id)
        {
            var city = _cityRepository.Find(item => item.Id == id);
            if (city == null)
            {
                return string.Empty;
            }

            return city.Name;
        }

        public static IEnumerable<Province> GetProvinces(bool status)
        {

            var provinces =
                _agencyReposity.FindList<Agency>(item => true)
                    .Select(item => item.CountyId)
                    .Distinct()
                    .ToArray()
                    .Select(item => int.Parse(string.Format("{0}0000", item.ToString().Substring(0, 2)))).ToArray();

            return status
                ? _provinceRepository.FindList<Province>(item => provinces.Contains(item.Id)).ToList()
                : _provinceRepository.FindList<Province>(item => !provinces.Contains(item.Id)).ToList();
        }

        public static Province GetProvince(int id)
        {
            return _provinceRepository.Find(item => item.Id == id);
        }

        public static IEnumerable<Province> GetProvince(bool status)
        {
            var provinces =
                _agencyReposity.FindList<Agency>(item => true)
                    .Select(item => item.CountyId)
                    .Distinct()
                    .ToArray()
                    .Select(item => int.Parse(string.Format("{0}00", item.ToString().Substring(0, 2)))).ToArray();

            return status
                ? _provinceRepository.FindList<Province>(item => provinces.Contains(item.Id)).ToList()
                : _provinceRepository.FindList<Province>(item => !provinces.Contains(item.Id)).ToList();
        }

        public static IEnumerable<City> GetCities(bool status)
        {
            var cities =
                _agencyReposity.FindList<Agency>(item => true)
                    .Select(item => item.CountyId)
                    .Distinct()
                    .ToArray()
                    .Select(item => int.Parse(string.Format("{0}00", item.ToString().Substring(0, 4)))).ToArray();

            return status
                ? _cityRepository.FindList<City>(item => cities.Contains(item.Id)).ToList()
                : _cityRepository.FindList<City>(item => !cities.Contains(item.Id)).ToList();
        }

        public static IEnumerable<City> GetCities(int proince)
        {
            string sheng_id = proince.ToString(CultureInfo.InvariantCulture).Substring(0, 2);
            return _cityRepository.FindList<City>(item => item.Id.ToString().StartsWith(sheng_id));
        }

        public static City GetCity(int id)
        {
            return _cityRepository.Find(item => item.Id == id);
        }

        public static IEnumerable<County> GetCounties(bool status)
        {
            var counties = _agencyReposity.FindList<Agency>(item => true).Select(item => item.CountyId).Distinct().ToArray();

            return status
                ? _countyRepository.FindList<County>(item => counties.Contains(item.Id)).ToList()
                : _countyRepository.FindList<County>(item => !counties.Contains(item.Id)).ToList();

        }

        public static IEnumerable<County> GetCounties(int city)
        {
            string shi_id = city.ToString(CultureInfo.InvariantCulture).Substring(0, 4);
            return _countyRepository.FindList<County>(item => item.Id.ToString().StartsWith(shi_id));
        }

        public static County GetCounty(int id)
        {
            return _countyRepository.Find(item => item.Id == id);
        }
    }
}