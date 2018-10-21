using System.Web;
using System.Web.Mvc;
using Project.Web.Filters;

namespace Project.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new OperatonLogAttribute());
        }   
    }
}
