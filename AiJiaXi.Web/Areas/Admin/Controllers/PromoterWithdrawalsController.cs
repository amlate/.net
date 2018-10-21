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
using Project.Domain.Enums;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.UserProfile;
using Project.Web.Filters;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    //总后台-推广员提现记录
    public class PromoterWithdrawalsController : Controller
    {
        private IRepository<Withdrawals> _withdrawalsRepository;
        private IRepository<Order> _orderRepository;//订单表
        private IRepository<ApplicationUser> _applicationUserRepository;//用户表啊[系统的]
        private IRepository<UserAccount> _userAccountRepository;//用户表啊[自定义的]

        public PromoterWithdrawalsController(IRepository<Withdrawals> withdrawalsRepository, IRepository<Order> orderRepository, IRepository<ApplicationUser> applicationUserRepository, IRepository<UserAccount> userAccountRepository)
        {
            _withdrawalsRepository = withdrawalsRepository;
            _orderRepository = orderRepository;
            _applicationUserRepository = applicationUserRepository;
            _userAccountRepository = userAccountRepository;
        }

        public ActionResult Index(int id = 1)
        {
            var model = _withdrawalsRepository.FindList<Withdrawals>(item => true).OrderByDescending(t => t.ApplyDate).ToPagedList(id, 15);
            foreach (var item in model)
            {
                //获取【公众号或APP】端用户信息  用户类型为 用户
                var withdraUser = _applicationUserRepository.Find(a => true && a.UserType == UserType.User && a.PhoneNumber == item.Phone);
                if (withdraUser != null)
                {
                    //预提现申请日期
                    DateTime preWithDate = Convert.ToDateTime(item.PreApplyDate);
                    item.AlreadyConsumed = _orderRepository.FindList<Order>(t => true && t.RiseTime >= preWithDate && t.OrderStatus == OrderStatus.Succeed && t.ApplicationUser.Id == withdraUser.Id).ToList().Sum(t => t.Fact).ToString();
                }
                else
                {
                    item.AlreadyConsumed = "推广员平台手机号码与公众号/APP不匹配";
                }

            }


            if (Request.IsAjaxRequest())
                return PartialView("_PromoterWithdrawalsSearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult SearchPost(string Phone, int id = 1)
        {
            return PromoterWithdrawalsSearchResult(Phone, id);
        }
        private ActionResult PromoterWithdrawalsSearchResult(string Phone, int id = 1)
        {
            var predicate = PredicateBuilder.True<Withdrawals>();

            if (!string.IsNullOrWhiteSpace(Phone))
            {
                predicate = predicate.And(item => item.Phone.Contains(Phone));
            }

            var model = _withdrawalsRepository.FindList<Withdrawals>(predicate).OrderByDescending(t => t.ApplyDate).ToPagedList(id, 15);

            foreach (var item in model)
            {
                //获取【公众号或APP】端用户信息  用户类型为 用户
                var withdraUser = _applicationUserRepository.Find(a => true && a.UserType == UserType.User && a.PhoneNumber == item.Phone);
                if (withdraUser != null)
                {
                    //预提现申请日期
                    DateTime preWithDate = Convert.ToDateTime(item.PreApplyDate);
                    item.AlreadyConsumed = _orderRepository.FindList<Order>(t => true && t.RiseTime >= preWithDate && t.OrderStatus == OrderStatus.Succeed && t.ApplicationUser.Id == withdraUser.Id).ToList().Sum(t => t.Fact).ToString();
                }
                else
                {
                    item.AlreadyConsumed = "推广员平台手机号码与公众号/APP不匹配";
                }

            }
            if (Request.IsAjaxRequest())
                return PartialView("_PromoterWithdrawalsSearchResult", model);
            return View(model);
        }

        /// <summary>
        /// 处理按钮方法
        /// </summary>
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public async Task<ActionResult> Transfer(long Id, WithdrawalsStatus TempState)
        {
            if (Id == 0)
            {
                return JavaScript("layer.alert('数据不存在！');");
            }
            else if (TempState == WithdrawalsStatus.Success|| TempState == WithdrawalsStatus.Failed)
            {
                return JavaScript("layer.alert('已经处理过，不可继续操作！');");
            }
            else
            {
                var entity = _withdrawalsRepository.Find(item => item.Id == Id);

                //entity.State = WithdrawalsStatus.Success;//转帐成功
                //entity.HandleDate = DateTime.Now.ToString();
                if (this.TryUpdateModel(entity))//把前台提交来的值直接替换上。[如果通过后台赋值，需要把上面两句代码打开，把当前这行注释掉]
                {
                    var result = await _withdrawalsRepository.UpdateAsync(entity);
                    if (!result)
                    {
                        return JavaScript("layer.alert('操作失败！');");
                    }
                }
            }

            return JavaScript("layer.alert('操作成功！',function(){location.href=window.location.href});");
        }

        /// <summary>
        /// 取消提现方法
        /// </summary>
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public async Task<ActionResult> CancelWidth(long Id, WithdrawalsStatus TempState)
        {
            if (Id == 0)
            {
                return JavaScript("layer.alert('数据不存在！');");
            }
            //只要状态不是等待转帐都提示
            else if (TempState != WithdrawalsStatus.UnDeaked)
            {
                return JavaScript("layer.alert('只有状态为【等待商家转帐】时才可操作！');");
            }
            else
            {
                var entity = _withdrawalsRepository.Find(item => item.Id == Id);
                //entity.State = WithdrawalsStatus.Success;//转帐成功
                //entity.HandleDate = DateTime.Now.ToString();
                if (this.TryUpdateModel(entity))//把前台提交来的值直接替换上。[如果通过后台赋值，需要把上面两句代码打开，把当前这行注释掉]
                {
                    var result = await _withdrawalsRepository.UpdateAsync(entity);
                    if (!result)
                    {
                        return JavaScript("layer.alert('取消提现数据操作失败！');");
                    }

                    //获取【公众号或APP】端用户信息  用户类型为 用户
                    var withdraUser = _applicationUserRepository.Find(a => true && a.UserType == UserType.User && a.PhoneNumber == entity.Phone);
                    if (withdraUser != null)
                    {
                        //当前用户余额
                        var yhModel = _userAccountRepository.Find(item => true && item.Id == withdraUser.Id);
                        //用户余额-提现金额
                        yhModel.CommissionMoney = yhModel.CommissionMoney + decimal.Parse(entity.Amount);
                        var result1 = _userAccountRepository.UpdateAsync(yhModel);
                        if (result1 == null)
                        {
                            return JavaScript("layer.alert('用户余额扣除失败！');");
                        }
                    }
                   

                  

                }
            }

            return JavaScript("layer.alert('操作成功！',function(){location.href=window.location.href});");
        }
    }
}