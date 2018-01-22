using System.Linq;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Entities.Location;
using AiJiaXi.Domain.Repositories.Impl;
using AiJiaXi.Domain.Repositories.Interface;

namespace AiJiaXi.Common.Helpers
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