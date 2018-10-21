using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Domain.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ZhiYuan.IAR.Repository.EF;

namespace Project.Web.Filters
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            }
            set { _roleManager = value; }
        }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var area = filterContext.RouteData.DataTokens["area"].ToString();
            string returnUrl = filterContext.HttpContext.Request.Path;
            string param = string.IsNullOrWhiteSpace(returnUrl)
                ? string.Empty
                : string.Format("?ReturnUrl={0}", HttpUtility.UrlEncode(returnUrl));
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.HttpContext.Response.Redirect(string.Format("/Admin/Purview/Login{0}", param), true);
                return;
            }

            if (!string.Equals(area, "Admin", StringComparison.CurrentCultureIgnoreCase))
            {
                filterContext.HttpContext.Response.Redirect(string.Format("/Admin/Purview/Login{0}", param), true);
                filterContext.RequestContext.HttpContext.Response.End();
                return;
            }

            var controller = filterContext.RouteData.Values["controller"].ToString();
            var action = filterContext.RouteData.Values["action"].ToString();
            var isAllowed = this.IsAllowed(controller, action, filterContext);

            if (!isAllowed)
            {
                filterContext.HttpContext.Response.Redirect(string.Format("/Admin/Purview/Login{0}", param), true);
                filterContext.RequestContext.HttpContext.Response.End();
            }
        }

        private bool IsAllowed(string controllerName, string actionName, AuthorizationContext filterContext)
        {
            string userId = filterContext.HttpContext.User.Identity.GetUserId();
            var user = UserManager.FindById(userId);

            if (user.IsFrozen)
            {
                return false;
            }

            if (user.UserType == UserType.User)
            {
                return false;
            }

            if (user.UserType == UserType.Admin || user.UserType == UserType.Agency || user.UserType == UserType.Withdrawals || user.UserType == UserType.Washers)
            {
                if (user.UserType == UserType.Agency)
                {
                    if (!user.Agency.IsValid)
                    {
                        return false;
                    }
                }

                if (user.UserType == UserType.Withdrawals)
                {
                    if (string.Equals("home", controllerName, StringComparison.CurrentCultureIgnoreCase) && string.Equals("index", actionName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }

                    return string.Equals("PromoterInfo", controllerName, StringComparison.CurrentCultureIgnoreCase) ||
                           string.Equals("Withdrawals", controllerName, StringComparison.CurrentCultureIgnoreCase) ||
                           string.Equals("QrCode", controllerName, StringComparison.CurrentCultureIgnoreCase);
                }

                if (string.Equals("home", controllerName, StringComparison.CurrentCultureIgnoreCase) && string.Equals("index", actionName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            if (UserManager.IsInRole(userId, "系统管理员"))
            {
                return true;
            }

            var navbar =
                ContextFactory.GetCurrentContext()
                    .Navbars.FirstOrDefault(item => item.Controller == controllerName && item.Action == actionName);

            if (navbar == null)
            {
                navbar =
                    ContextFactory.GetCurrentContext().Navbars.FirstOrDefault(
                        item => item.Controller == controllerName && item.IsParent);
            }

            // 无规则
            if (navbar == null)
            {
                return true;
            }

            var actionRoles = navbar.ApplicationRoles.ToList();
            // 没有角色则没有权限
            if (actionRoles.Count == 0)
            {
                return false;
            }

            var roles = actionRoles.Select(item => item.Name).ToArray();
            var rolesHas = this.UserManager.GetRoles(userId);
            return rolesHas.Any(item => roles.Contains(item));
        }
    }
}