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
using Microsoft.AspNet.Identity;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.UserProfile;
using Project.Web.Filters;

namespace Project.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    //推广员后台-提现记录信息
    public class WithdrawalsController : Controller
    {
        private ApplicationUserManager _userManager;//用户信息
        //private IRepository<PreWithdrawals> _preWithdrawalsRepository;//预提现申请
        private IRepository<Withdrawals> _withdrawalsRepository;//提现申请
        private IRepository<Order> _orderRepository;//订单表
        private IRepository<ApplicationUser> _applicationUserRepository;//用户表啊[系统的]
        private IRepository<UserAccount> _userAccountRepository;//用户表啊[自定义的]

        public WithdrawalsController(IRepository<Withdrawals> withdrawalsRepository, ApplicationUserManager userManager, IRepository<Order> orderRepository, IRepository<ApplicationUser> applicationUserRepository, IRepository<UserAccount> userAccountRepository)
        {
            _withdrawalsRepository = withdrawalsRepository;
            //_preWithdrawalsRepository = preWithdrawalsRepository;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _applicationUserRepository = applicationUserRepository;
            _userAccountRepository=userAccountRepository;

          

        }

        public ActionResult Index(int id = 1)
        {
           //登录用户信息
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);

            string MyPhone = user.PhoneNumber;//获取当前登录用户手机号
            //string MyPhone = "13566665555";//获取当前登录用户手机号
            ViewBag.MyPhone = MyPhone;

            #region 计算我的余额，加上一些状态等
            //当前手机号必须有
            if (string.IsNullOrWhiteSpace(MyPhone))
            {
                return Content("请联系商家确认登录用户信息中【移动电话】是否正确录入！");
                //return JavaScript("layer.alert('请联系商家确认登录用户信息中【移动电话】是否正确录入');");
            }
             
         
            //获取【公众号或APP】端用户信息  用户类型为 用户
            var withdraUser = _applicationUserRepository.Find(item => true && item.UserType == UserType.User && item.PhoneNumber == MyPhone);
            if (withdraUser == null)
            {
                return Content("请联系商家确认登录用户信息中【移动电话】是否和公众号/APP中登录手机号一致！");
               // return JavaScript("layer.alert('请联系商家确认登录用户信息中【移动电话】是否和公众号/APP中登录手机号一致！');");
            }

            //当前用户余额【我的提成金额】
            var ye = _userAccountRepository.Find(item => true && item.Id == withdraUser.Id).CommissionMoney;

            //提现申请数据列表
            var model = _withdrawalsRepository.FindList<Withdrawals>(item => true && item.Phone == MyPhone).OrderByDescending(t => t.ApplyDate).ToPagedList(id, 15);

            ////提现金额
            //var txye = _withdrawalsRepository.FindList<Withdrawals>(item => true && item.Phone == MyPhone).ToList().Sum(t => Convert.ToInt32(t.Amount));
           
            //冻结金额 
            var djye = _withdrawalsRepository.FindList<Withdrawals>(item => true && item.Phone == MyPhone && item.State== WithdrawalsStatus.UnDeaked).ToList().Sum(t => Convert.ToInt32(t.Amount));

            ViewBag.FrendsCount = ye ;//我的余额
            ViewBag.djye = djye;
            #endregion

            #region 预提现信息显示
            //预提现信息
            string title= "您暂时未进行申请预提现！";
            var preWith = _withdrawalsRepository.Find(item => true && item.Phone== MyPhone && item.State== WithdrawalsStatus.PreWithdrawals);
            if (preWith!=null)
            {
                //预提现申请日期
                DateTime preWithDate = Convert.ToDateTime(preWith.PreApplyDate);
                //消费金额[状态为成功]
                var xfje = _orderRepository.FindList<Order>(item => true && item.RiseTime>= preWithDate && item.OrderStatus == OrderStatus.Succeed && item.ApplicationUser.Id == withdraUser.Id).ToList().Sum(t => t.Fact);


                title = "您预提现申请的时间为：" + preWith.PreApplyDate + "  目前您已经消费金额为：" + xfje.ToString() + "元  【可提现金额为：" + (xfje*2).ToString() + "元】";
            }
         
            ViewBag.preWith = title;
            #endregion

            if (Request.IsAjaxRequest())
                return PartialView("_WithdrawalsSearchResult", model);
            return View(model);
        }
        [HttpPost]
        public ActionResult SearchPost(string BankCard, int id = 1)
        {
            return WithdrawalsSearchResult(BankCard, id);
        }
        private ActionResult WithdrawalsSearchResult(string BankCard, int id = 1)
        {
            //登录用户信息
            var userId = User.Identity.GetUserId();
            var user = this._userManager.FindById(userId);
            string MyPhone = user.PhoneNumber;//获取当前登录用户手机号

            var predicate = PredicateBuilder.True<Withdrawals>();
            predicate = predicate.And(item => item.Phone == MyPhone);

            if (!string.IsNullOrWhiteSpace(BankCard))
            {
                predicate = predicate.And(item => item.Accounts.Contains(BankCard));
            }

            var model = _withdrawalsRepository.FindList<Withdrawals>(predicate).OrderByDescending(t => t.ApplyDate).ToPagedList(id, 15);
            if (Request.IsAjaxRequest())
                return PartialView("_WithdrawalsSearchResult", model);
            return View(model);
        }

        /// <summary>
        /// 预提现申请方法
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> PreWithdrawals(string Phone, string PreApplyDate)
        {
            //提现手机号，推广员手机号
            if (string.IsNullOrWhiteSpace(Phone))
            {
                return JavaScript("layer.alert('当前登录帐号手机号不存在，无法操作！');");
            }     
            else
            {
                var preWith = _withdrawalsRepository.Find(item => true && item.Phone == Phone && item.State == WithdrawalsStatus.PreWithdrawals);
                if (preWith != null)
                {
                    return JavaScript("layer.alert('您已经申请过预提现，请正式提现后再次申请！');");
                }

             
                //获取【公众号或APP】端用户信息  用户类型为 用户
                var withdraUser = _applicationUserRepository.Find(item => true && item.UserType == UserType.User && item.PhoneNumber == Phone);
                if (withdraUser == null)
                {
                    return JavaScript("layer.alert('请联系商家确认登录用户信息中【移动电话】是否和公众号/APP中登录手机号一致！');");
                }

                var yhye = _userAccountRepository.Find(item => true && item.Id == withdraUser.Id).CommissionMoney;
                if (yhye==0)
                {
                    return JavaScript("layer.alert('余额为0，不可预提现申请！');");
                }


                Withdrawals pw = new Domain.Entities.PromoterManager.Withdrawals();
                pw.Phone = Phone;
                pw.PreApplyDate = PreApplyDate;
                pw.State = WithdrawalsStatus.PreWithdrawals;

                var result = await _withdrawalsRepository.AddAsync(pw);
                if (result == null)
                {
                    return JavaScript("layer.alert('操作失败！');");
                }

               

            }

           return JavaScript("layer.alert('操作成功！',function(){location.href=window.location.href});");
           // return RedirectToAction("Index");
        }


        /// <summary>
        /// 提现申请方法
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Withdrawals(Withdrawals model)
        {
            //登录【后台】用户信息
            //var userId = User.Identity.GetUserId();
            //var user = this._userManager.FindById(userId);
            //string MyPhone = user.PhoneNumber;//登录后台的手机号

            //获取【公众号或APP】端用户信息  用户类型为 推广员
            var withdraUser = _applicationUserRepository.Find(item => true && item.UserType== UserType.User && item.PhoneNumber== model.Phone);
            if (withdraUser == null)
            {
                return JavaScript("layer.alert('请联系商家确认登录用户信息中【移动电话】是否和公众号/APP中登录手机号一致！');");
            }



            #region 验证提现金额

            //用户自定义表  MODEL
            var yhModel = new UserAccount();   
            if (string.IsNullOrWhiteSpace(model.Amount))
            {
                return JavaScript("layer.alert('可提现金额不能为空！');");
            }
            else
            {
                try
                {
                    int tx = Convert.ToInt32(model.Amount);
                    if (tx <= 0)
                    {
                        return JavaScript("layer.alert('请输入大于0的提现金额！');");
                    }
                    //消费金额[状态为成功]
                    var ye = _orderRepository.FindList<Order>(item => true && item.OrderStatus == OrderStatus.Succeed && item.ApplicationUser.Id == withdraUser.Id).ToList().Sum(t => t.Fact);
                    if (tx > (ye*2))
                    {
                        return JavaScript("layer.alert('提现金额不能大于您当前已消费金额2倍！');");
                    }


                    //当前用户提成余额
                     yhModel = _userAccountRepository.Find(item => true && item.Id == withdraUser.Id);
                    if (tx > yhModel.CommissionMoney)
                    {
                        return JavaScript("layer.alert('余额不足，不可提现！');");
                    }
                }
                catch (Exception ex)
                {
                    return JavaScript("layer.alert('请输入正确的提现金额！');");
                }
            }
            #endregion

            //判断是否有可提现的金额
            if (string.IsNullOrWhiteSpace(model.Bank))
            {
                return JavaScript("layer.alert('提现银行不能为空！');");
            }
            else if (string.IsNullOrWhiteSpace(model.Bank))
            {
                return JavaScript("layer.alert('提现银行不能为空！');");
            }
            else if (string.IsNullOrWhiteSpace(model.Name))
            {
                return JavaScript("layer.alert('银行卡姓名不能为空！');");
            }
            else if (string.IsNullOrWhiteSpace(model.Accounts))
            {
                return JavaScript("layer.alert('银行卡帐号不能为空！');");
            }
            

            //获取到预提现申请中的数据【正常情况只有一条】
            var entity = _withdrawalsRepository.Find(item =>item.Phone== model.Phone  && item.State==WithdrawalsStatus.PreWithdrawals);
            if (entity == null)
            {
                //验证预提现申请
                return JavaScript("layer.alert('请先进行预提现申请，成功消费后才可进行提现！');");
            }

            //将POST进来的数据，替换到表的数据上，省去了，一个字段一个字段赋值的流程
            if (this.TryUpdateModel(entity))
            {
                entity.ApplyDate = DateTime.Now.ToString();
                entity.State = WithdrawalsStatus.UnDeaked;

                var result=await _withdrawalsRepository.UpdateAsync(entity);
                if (result == null)
                {
                    return JavaScript("layer.alert('用户提现操作失败！');");
                }

                //用户余额-提现金额
                yhModel.CommissionMoney = yhModel.CommissionMoney - decimal.Parse(entity.Amount);
                var result1 = _userAccountRepository.UpdateAsync(yhModel);
                if (result1 == null)
                {
                    return JavaScript("layer.alert('用户余额扣除失败！');");
                }
            }
            

            //删除当前登录手机号的预提现数据
            //var entity = _preWithdrawalsRepository.Find(item => item.Phone == model.Phone);
            //var result1 = await _preWithdrawalsRepository.DeleteAsync(entity);
            //if (!result1)
            //{
            //    return JavaScript("layer.alert('操作失败！');");
            //}

            return JavaScript("layer.alert('操作成功！',function(){location.href=window.location.href});");
           // return RedirectToAction("Index");
        }


    }
}