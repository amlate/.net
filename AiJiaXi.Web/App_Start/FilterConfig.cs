using System.Web;
using System.Web.Mvc;
using AiJiaXi.Web.Filters;

namespace AiJiaXi.Web
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
