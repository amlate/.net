using System.Linq;
using Project.Domain.Entities;
using Project.Domain.Entities.Location;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;

namespace Project.Common.Helpers
{
    public class AgencyHelper
    {
        private static IRepository<Agency> _repository = new Repository<Agency>();
        public static string GetAgencyName(string ids)
        {
            var idarray = ids.Split(',').Select(long.Parse).Select(item=>_repository.Find(m => m.Id == item).Name);

            return idarray.Aggregate(string.Empty, (current, item) => current + "," + item).Trim(',');
        }
    }
}