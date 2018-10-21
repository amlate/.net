using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Domain.Entities.Configs;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ConfigController : Controller
    {
        private IRepository<SmsConfig> _smsConfigRepository;

        private IRepository<EmailConfig> _emailRepository;

        public ConfigController(IRepository<SmsConfig> smsConfigRepository, IRepository<EmailConfig> emailRepository)
        {
            _smsConfigRepository = smsConfigRepository;
            _emailRepository = emailRepository;
        }

        // GET: Admin/SysConfig
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SmsConfig()
        {
            var config = _smsConfigRepository.Find(item => true);

            return PartialView("_SmsConfigPartial", config);
        }

        public ActionResult EmailConfig()
        {
            var config = _emailRepository.Find(item => true);

            return PartialView("_EmailConfigPartial", config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage()
        {
            var smsConfig = _smsConfigRepository.Find(item => true);
            var emailConfig = _emailRepository.Find(item => true);

            if (this.TryUpdateModel(smsConfig) && this.TryUpdateModel(emailConfig))
            {
                await _smsConfigRepository.UpdateAsync(smsConfig);
                await _emailRepository.UpdateAsync(emailConfig);

                return JavaScript("layer.alert('保存配置成功！');");
            }

            return JavaScript("layer.alert('保存配置失败！');");
        }
    }
}