using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UserAddressController : Controller
    {
        private IRepository<UserAddress> _userAddressRepository;

        public UserAddressController(IRepository<UserAddress> userAddressRepository)
        {
            _userAddressRepository = userAddressRepository;
        }

        // GET: Admin/UserAddress
        public ActionResult Index(string id)
        {
            var model = _userAddressRepository.FindList<UserAddress>(item => item.ApplicationUserId == id).ToList();

            return View(model);
        }
    }
}