using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;
using Webdiyer.WebControls.Mvc;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class AccountRecordController : Controller
    {
        private IRepository<AccountRecord> _accountRecordRepository;

        public AccountRecordController(IRepository<AccountRecord> accountRecordRepository)
        {
            _accountRecordRepository = accountRecordRepository;
        }

        // GET: Admin/AccountRecord
        public ActionResult Index(string userId, int id = 1)
        {
            var model =
                _accountRecordRepository.FindList<AccountRecord>(item => item.UserAccountId == userId)
                    .OrderByDescending(item => item.RiseTime)
                    .ToPagedList(id, 5);



            return PartialView("_AccountRecords", model);
        }
    }
}