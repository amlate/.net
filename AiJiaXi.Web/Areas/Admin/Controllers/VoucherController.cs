using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;
using Project.Domain.Helpers;
using Project.Domain.JsonModel;
using Project.Domain.Repositories.Interface;
using Project.Web.Filters;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.Helpers;
using Webdiyer.WebControls.Mvc;
using WebGrease.Css.Extensions;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class VoucherController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IRepository<Agency> _agencyRepository;
        private IRepository<Event> _eventRepository;
        private IRepository<Voucher> _voucherRepository;

        public VoucherController(ApplicationUserManager userManager, ApplicationRoleManager roleManager,
            IRepository<Voucher> voucherRepository, IRepository<Agency> agencyRepository,
            IRepository<Event> eventRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _voucherRepository = voucherRepository;
            _agencyRepository = agencyRepository;
            _eventRepository = eventRepository;
        }

        // GET: Admin/Voucher
        public async Task<ActionResult> Index(string eventId, int id = 1)
        {
            var predicate = PredicateBuilder.True<Voucher>();

            if (!eventId.Is<long>())
            {
                throw new Exception("非法参数");
            }

            var eId = long.Parse(eventId);
            predicate = predicate.And(item => item.EventId == eId);

            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            switch (user.UserType)
            {
                case UserType.Agency:
                    break;
                case UserType.User:
                case UserType.Withdrawals:
                    throw new Exception("该会员无权查看！");
            }
            var model = this._voucherRepository.FindList<Voucher>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            ViewBag.EventId = eventId;
            if (Request.IsAjaxRequest())
                return PartialView("_VouchersSearchResult", model);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> VouchersSearchPost(string name, int id = 1)
        {
            return await VouchersSearchResult(name, id);
        }

        private async Task<ActionResult> VouchersSearchResult(string name, int id = 1)
        {
            var predicate = PredicateBuilder.True<Voucher>();
            var userId = User.Identity.GetUserId();
            var user = await this._userManager.FindByIdAsync(userId);
            switch (user.UserType)
            {
                case UserType.Agency:
                    break;
                case UserType.User:
                case UserType.Withdrawals:
                    throw new Exception("该会员无权查看！");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            var model = this._voucherRepository.FindList<Voucher>(predicate).OrderBy(a => a.Id).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_VouchersSearchResult", model);
            return View(model);
        }

        public ActionResult Generate()
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("非管理员不能生成代金券！");
            }
            
            ViewBag.Second = "生成非活动类代金券";
            var agencies = _agencyRepository.FindList<Agency>(item => item.IsValid).ToList();
            var events =
                _eventRepository.FindList<Event>(
                    item =>
                        item.EndTime >= DateTime.Now &&
                        (item.EventType == EventType.Voucher || item.EventType == EventType.LuckyDraw || item.EventType == EventType.Register)).ToList();
            ViewBag.Agencies = new SelectList(agencies, "Id", "Name");
            ViewBag.Events = new SelectList(events, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate(FormCollection form)
        {
            if (string.IsNullOrWhiteSpace(form["EventId"]))
            {
                throw new Exception("无效的活动！");
            }

            var nums_str = form["nums"];
            if (!nums_str.IsInt())
            {
                throw new Exception("生成代金券数目错误，应为整数数字！");
            }
            int nums = int.Parse(nums_str);

            var vouchers = new List<Voucher>();
            var temp = new Voucher();
            if (TryUpdateModel(temp, null, form.AllKeys, new string[] {"nums"}))
            {
                for (int i = 0; i < nums; i++)
                {
                    var entity = new Voucher
                    {
                        Id = Guid.NewGuid(),
                        VoucherNo = VoucherHelper.GenerateVoucherNumber(vouchers),
                        Name = temp.Name,
                        Desc = temp.Desc,
                        PriceToUse = temp.PriceToUse,
                        Amount = temp.Amount,
                        StartTime = temp.StartTime,
                        EndTime = temp.EndTime,
                        IsOccu = false,
                        EventId = temp.EventId,
                        AgencyId = temp.AgencyId,
                    };
                    vouchers.Add(entity);
                }
                
                SqlBulkHelper.SqlBulkCopy(vouchers, "AiJiaXi__Voucher", new []{ "Event", "UserAccount", "UserAccountId", "Agency" });
                
                
            }

            return RedirectToAction("Index", new { eventId = temp.EventId});
        }

        public ActionResult Users(string query)
        {
            if (query == null)
            {
                query = String.Empty;
            }
            var users = _userManager.Users.Where(item => item.UserType == UserType.User && item.UserName.Contains(query)).ToList();
            var selet2List = users.Select(item => new Select2Item() {id = item.Id, text = item.UserName});

            return Json(selet2List, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Occup(FormCollection form)
        {
            var voucherId = form["pk"];
            var value = form["value"];
            var voucher = _voucherRepository.Find(item => item.Id.ToString() == voucherId);
            if (voucher ==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                voucher.UserAccount = null;
            }
            else
            {
                voucher.UserAccountId = value;
            }

            var result = await _voucherRepository.UpdateAsync(voucher);
            if (!result)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            if (user.UserType != UserType.Admin)
            {
                throw new Exception("当前用户没有执行此操作的权限!");
            }

            var entity = _voucherRepository.Find(item => item.Id.ToString() == id);

            if (entity == null)
            {
                return JavaScript("layer.alert('该优惠券不存在，删除失败！');");
            }

            var result = await _voucherRepository.DeleteAsync(entity);
            if (!result)
            {
                return JavaScript("layer.alert('删除过程中发生异常，删除失败！');");
            }

            var model =
                this._voucherRepository.FindList<Agency>(item => true)
                    .OrderBy(a => a.Id)
                    .ToPagedList(1, 15);

            return PartialView("_VouchersSearchResult", model);
        }

        public ActionResult Download(string eventId)
        {
            var curEvent = _eventRepository.Find(item => item.Id.ToString() == eventId);
            if (curEvent == null)
            {
                return null;
            }

            var vouchers = curEvent.Vouchers;

            if (!vouchers.Any())
            {
                return null;
            }

            var dir = Server.MapPath("~/Download/VoucherNo");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var fileName = $"代金券-{curEvent.Name}-{DateTime.Now.Ticks}.txt";

            var sb = new StringBuilder(String.Empty);
            vouchers.ForEach(item => sb.AppendLine($"代金券兑换码：{item.VoucherNo}, 代金券金额：{item.Amount.ToString("C")}"));

            string path = dir.CombinePath(fileName);
            System.IO.File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

            return File(dir.CombinePath(fileName), "text/plain");
        }
    }
}