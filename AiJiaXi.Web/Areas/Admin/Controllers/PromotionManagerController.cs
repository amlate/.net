using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project.Common.Helpers;
using Project.Domain.Entities;
using Project.Domain.Repositories.Interface;
using Webdiyer.WebControls.Mvc;
using Project.Domain.Entities.PromoterManager;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Enums;
using Project.Web.Filters;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    //总后台-推广员管理
    public class PromotionManagerController : Controller
    {
        private IRepository<Withdrawals> _withdrawalsRepository;
        private IRepository<ApplicationUser> _applicationUserRepository;//用户表啊[系统的]
        private IRepository<PromoterInfo> _promoterInfoRepository;//推广信息表

        public PromotionManagerController(IRepository<PromoterInfo> promoterInfoRepository,IRepository<ApplicationUser> applicationUserRepository, IRepository<Withdrawals> withdrawalsRepository)
        {
            _withdrawalsRepository = withdrawalsRepository;
            _applicationUserRepository = applicationUserRepository;
            _promoterInfoRepository=promoterInfoRepository;
        }

        public ActionResult Index(int id = 1)
        {
            //获取后台用户类型为推广员的
            var model = _applicationUserRepository.FindList<ApplicationUser>(item => true && item.UserType==UserType.Withdrawals && item.IsFrozen==false).OrderBy(t => t.AddTime).ToPagedList(id, 15);
            foreach (var item in model)
            {               
                item.FrendCount = _promoterInfoRepository.Count(t=>t.MyPhone==item.PhoneNumber).ToString();
            }
        


            if (Request.IsAjaxRequest())
                return PartialView("_PromotionManagerSearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult SearchPost(string MyPhone, int id = 1)
        {
            return PromotionManagerSearchResult(MyPhone, id);
        }
        private ActionResult PromotionManagerSearchResult(string MyPhone, int id = 1)
        {
            var predicate = PredicateBuilder.True<ApplicationUser>();
            predicate=predicate.And(item => item.UserType == UserType.Withdrawals && item.IsFrozen == false);

            if (!string.IsNullOrWhiteSpace(MyPhone))
            {
                predicate = predicate.And(item => item.PhoneNumber.Contains(MyPhone));
            }

            var model = _applicationUserRepository.FindList<ApplicationUser>(predicate).OrderBy(t => t.AddTime).ToPagedList(id, 15);
            foreach (var item in model)
            {
                item.FrendCount = _promoterInfoRepository.Count(t => t.MyPhone == item.PhoneNumber).ToString();
            }

            if (Request.IsAjaxRequest())
                return PartialView("_PromotionManagerSearchResult", model);
            return View(model);
        }


        /// <summary>
        /// 保存好友关系
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> SaveFrend(PromoterInfo model)
        {            
 
            if (string.IsNullOrWhiteSpace(model.MyPhone))
            {
                return JavaScript("layer.alert('推广员手机号不能为空！');");
            }
            else if (string.IsNullOrWhiteSpace(model.FriendsPhone))
            {
                return JavaScript("layer.alert('新好友不能为空！');");
            }
            else if (model.MyPhone== model.FriendsPhone)
            {
                return JavaScript("layer.alert('不能保存自己为自己的下线！');");
            }

            //判断用户表推广员
            var proModel = _applicationUserRepository.Find(t=>true && t.UserType==UserType.Withdrawals  && t.IsFrozen==false && t.PhoneNumber== model.MyPhone);
            if (proModel == null)
            {
                return JavaScript("layer.alert('您输入的推广员手机号不存在！');");
            }
            //判断用户表用户
            var proMode2 = _applicationUserRepository.Find(t => true && t.UserType == UserType.User && t.IsFrozen == false && t.PhoneNumber == model.FriendsPhone);
            if (proMode2 == null)
            {
                return JavaScript("layer.alert('您输入的好友手机号不存在！');");
            }
            var proModel1 = _promoterInfoRepository.Find(t => true && t.FriendsPhone == model.FriendsPhone);
            if (proModel1 != null)
            {
                return JavaScript("layer.alert('该手机号已经是其他人好友！');");
            }



            try
            {
                model.FollowDate = System.DateTime.Now.ToString();//关注时间         
                _promoterInfoRepository.Add(model);
                
            }
            catch (Exception ex)
            {
                return JavaScript("layer.alert('操作失败！');");
            }
           
            

            return JavaScript("layer.alert('操作成功！',function(){location.href=window.location.href});");
            // return RedirectToAction("Index");
        }



    }
}