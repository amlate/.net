using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities;
using AiJiaXi.Domain.Repositories.Interface;
using Webdiyer.WebControls.Mvc;
using AiJiaXi.Domain.Entities.PromoterManager;
using AiJiaXi.Web.Filters;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    //总后台-我的推广信息 所有好友列表
    public class FrendPromoterInfoController : Controller
    {
        private IRepository<PromoterInfo> _promoterInfoRepository;

        public FrendPromoterInfoController(IRepository<PromoterInfo> promoterInfoRepository)
        {
            _promoterInfoRepository = promoterInfoRepository;
        }

        public ActionResult Index(string MyPhone, int id = 1)
        {
            var model = _promoterInfoRepository.FindList<PromoterInfo>(item => true && item.MyPhone == MyPhone).ToList().OrderByDescending(t=>DateTime.Parse(t.FollowDate)).AsQueryable().ToPagedList(id, 15);

            ViewBag.MyPhone = MyPhone;
            if (Request.IsAjaxRequest())
                return PartialView("_FrendPromoterInfoSearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult SearchPost(string FriendsPhone, string MyPhone, int id = 1)
        {
            return FrendPromoterInfoSearchResult(FriendsPhone, MyPhone, id);
        }
        private ActionResult FrendPromoterInfoSearchResult(string FriendsPhone, string MyPhone, int id = 1)
        {
            var predicate = PredicateBuilder.True<PromoterInfo>();

            if (!string.IsNullOrWhiteSpace(MyPhone))//推广员手机号，肯定不为空的
            {
                predicate = predicate.And(item => item.MyPhone.Contains(MyPhone));
            }
            if (!string.IsNullOrWhiteSpace(FriendsPhone))// 好友手机号
            {
                predicate = predicate.And(item => item.FriendsPhone.Contains(FriendsPhone));
            }
            var model = _promoterInfoRepository.FindList<PromoterInfo>(predicate).ToList().OrderByDescending(t=>DateTime.Parse(t.FollowDate)).AsQueryable().ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_FrendPromoterInfoSearchResult", model);
            return View(model);
        }

        /// <summary>
        /// 处理按钮方法
        /// </summary>
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public async Task<ActionResult> Delete(long Id)
        {
            if (Id == 0)
            {
                return JavaScript("layer.alert('数据不存在！');");
            }          
            else
            {
                var entity = _promoterInfoRepository.Find(item => item.Id == Id);               
                if (this.TryUpdateModel(entity))
                {
                    var result = await _promoterInfoRepository.DeleteAsync(entity);
                    if (!result)
                    {
                        return JavaScript("layer.alert('操作失败！');");
                    }
                }
            }

            return JavaScript("layer.alert('操作成功！',function(){location.href=window.location.href});");
        }
    }
}