using System;
using System.Linq;
using System.Web.Mvc;
using Project.Domain.Entities.Logs;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;
using ZhiYuan.IAR.Repository.EF;

namespace Project.Web.Filters
{
    public class OperatonLogAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在执行操作方法之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var area = filterContext.RouteData.DataTokens["area"]== null ? string.Empty : filterContext.RouteData.DataTokens["area"].ToString();
            var cname = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();

            // 只记录后台的操作日志
            if (!string.Equals(area, "Admin", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            // 只记录后台的操作日志
            if (string.Equals(cname, "OperationLog", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            var module =
                ContextFactory.GetCurrentContext().Navbars.FirstOrDefault(item => item.Controller == cname);

            string moduleName = cname;

            if (module != null)
            {
                moduleName = module.NameOption;
            }
            else
            {
                return;
            }

            var action =
               ContextFactory.GetCurrentContext().Navbars.FirstOrDefault(item => item.Controller == cname && item.Action == actionName);

            string actionDisplay = actionName;

            if (action != null)
            {
                actionDisplay = action.NameOption;
            }
            else
            {
                return;
            }

            var str = filterContext.ActionParameters.Aggregate(
                string.Empty,
                (current, actionParameter) => current + (actionParameter.Key + ":" + actionParameter.Value));

            IRepository<OperationLog> _operationlogRepository = new Repository<OperationLog>();
            var log = new OperationLog()
                          {
                              Module = moduleName,
                              Action = $"{actionDisplay}({str})",
                              RiseTime = DateTime.Now,
                              UserIP =
                                  filterContext.RequestContext.HttpContext.Request.UserHostAddress == "::1"
                                      ? "127.0.0.1"
                                      : filterContext.RequestContext.HttpContext.Request.UserHostAddress,
                              UserName = filterContext.HttpContext.User.Identity.Name
                          };

            _operationlogRepository.Add(log);

        }

        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}