using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using AiJiaXi.Domain.Entities.UserProfile;
using AiJiaXi.Domain.Enums;
using AiJiaXi.Domain.Repositories.Interface;
using AiJiaXi.Web.Filters;
using Microsoft.AspNet.Identity;

namespace AiJiaXi.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UserAccountController : Controller
    {
        private IRepository<UserAccount> _userAccountRepository;

        public UserAccountController(IRepository<UserAccount> userAccountRepository)
        {
            this._userAccountRepository = userAccountRepository;
        }

        public async Task<ActionResult> Index(string id)
        {
            var entity = await _userAccountRepository.FindAsync(item => item.Id == id);

            return View(entity);
        }

        [HttpPost]
        public async Task<ActionResult> ModifyAccount(string id, string name, string value)
        {
            var account = _userAccountRepository.Find(item => item.Id == id);
            if (account.AccountRecords == null)
            {
                account.AccountRecords = new List<AccountRecord>();
            }
            var accountRecord = new AccountRecord();
            if (name == "balance")
            {
                decimal val = 0;
                var flag = decimal.TryParse(value, out val);
                if (flag)
                {
                    bool isAdd = account.Balance > val;
                    decimal tradeMoney = isAdd ? account.Balance - val : val - account.Balance;
                    account.Balance = val;
                    accountRecord.TradeType = isAdd ? TradeType.Consume : TradeType.Recharge;
                    accountRecord.RiseTime = DateTime.Now;
                    accountRecord.ResultType = ResultType.Succeedd;
                    accountRecord.TradeMoney = tradeMoney;
                    accountRecord.AccountBallance = val;
                    accountRecord.TradeId = Guid.NewGuid().ToString();
                    accountRecord.Note = $"管理员（{this.User.Identity.GetUserName()}）调整了您的账户余额！";
                }
            }

            if (name == "score")
            {
                int val = 0;
                var flag = int.TryParse(value, out val);
                if (flag)
                {
                    bool isAdd = account.Score > val;
                    int tradeScore = isAdd ? account.Score - val : val - account.Score;
                    account.Score = val;
                    accountRecord.TradeType = isAdd ? TradeType.ScoreConsume : TradeType.ScoreIncome;
                    accountRecord.RiseTime = DateTime.Now;
                    accountRecord.ResultType = ResultType.Succeedd;
                    accountRecord.TradeScore = tradeScore;
                    accountRecord.ScoreBalance = val;
                    accountRecord.TradeId = Guid.NewGuid().ToString();
                    accountRecord.Note = $"管理员（{this.User.Identity.GetUserName()}）调整了您的账户账户积分！";
                }
            }
            account.AccountRecords.Add(accountRecord);
            await _userAccountRepository.UpdateAsync(account);

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}