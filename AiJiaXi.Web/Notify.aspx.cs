using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Project.Common.Helpers;
using Project.Web.Controllers;
using WeiPay;
using Project.Domain.Entities;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.UserProfile;
using Project.Domain.Enums;
using Project.Domain.Repositories.Interface;
using ZhiYuan.IAR.Repository.EF;

namespace Project.Web
{
    public partial class Notify : System.Web.UI.Page
    {
        private readonly IRepository<UserAccount> _userAccount;

        private readonly IRepository<AccountRecord> _accountRecordRepository;



        protected async void Page_Load(object sender, EventArgs e)
        {
            //LogUtil.WriteLog("============ 支付回调页面 ===============");
            //创建ResponseHandler实例
            ResponseHandler resHandler = new ResponseHandler(Context);
            //创建返回给微信的ResponseHandler实例
            RequestHandler toWeixinServer = new RequestHandler(null);
            resHandler.setKey(PayConfig.AppKey);
            //判断签名
            try
            {

                string error = "";
                if (resHandler.isWXsign(out error))
                {
                    #region 协议参数=====================================
                    //--------------协议参数--------------------------------------------------------
                    //SUCCESS/FAIL此字段是通信标识，非交易标识，交易是否成功需要查
                    string return_code = resHandler.getParameter("return_code");
                    //返回信息，如非空，为错误原因签名失败参数格式校验错误
                    string return_msg = resHandler.getParameter("return_msg");
                    //微信分配的公众账号 ID
                    string appid = resHandler.getParameter("appid");

                    //以下字段在 return_code 为 SUCCESS 的时候有返回--------------------------------
                    //微信支付分配的商户号
                    string mch_id = resHandler.getParameter("mch_id");
                    //微信支付分配的终端设备号
                    string device_info = resHandler.getParameter("device_info");
                    //微信分配的公众账号 ID
                    string nonce_str = resHandler.getParameter("nonce_str");
                    //业务结果 SUCCESS/FAIL
                    string result_code = resHandler.getParameter("result_code");
                    //错误代码 
                    string err_code = resHandler.getParameter("err_code");
                    //结果信息描述
                    string err_code_des = resHandler.getParameter("err_code_des");

                    //以下字段在 return_code 和 result_code 都为 SUCCESS 的时候有返回---------------
                    //-------------业务参数---------------------------------------------------------
                    //用户在商户 appid 下的唯一标识
                    string openid = resHandler.getParameter("openid");
                    //用户是否关注公众账号，Y-关注，N-未关注，仅在公众账号类型支付有效
                    // string is_subscribe = resHandler.getParameter("is_subscribe");
                    //JSAPI、NATIVE、MICROPAY、APP
                    string trade_type = resHandler.getParameter("trade_type");
                    //银行类型，采用字符串类型的银行标识
                    string bank_type = resHandler.getParameter("bank_type");
                    //订单总金额，单位为分
                    string total_fee = resHandler.getParameter("total_fee");
                    //货币类型，符合 ISO 4217 标准的三位字母代码，默认人民币：CNY
                    string fee_type = resHandler.getParameter("fee_type");
                    //微信支付订单号
                    string transaction_id = resHandler.getParameter("transaction_id");
                    //商户系统的订单号，与请求一致。
                    string out_trade_no = resHandler.getParameter("out_trade_no");
                    //商家数据包，原样返回
                    string attach = resHandler.getParameter("attach");
                    //支 付 完 成 时 间 ， 格 式 为yyyyMMddhhmmss，如 2009 年12 月27日 9点 10分 10 秒表示为 20091227091010。时区为 GMT+8 beijing。该时间取自微信支付服务器
                    string time_end = resHandler.getParameter("time_end");

                    #endregion
                    //支付成功
                    if (!out_trade_no.Equals("") && return_code.Equals("SUCCESS") && result_code.Equals("SUCCESS"))
                    {
                       
                        toWeixinServer.setParameter("return_code", "SUCCESS");

                        /**
                         *  这里输入用户逻辑操作，比如更新订单的支付状态
                         * 
                         * **/
                        #region 充值
                        //查询订单号是否在缓存中，如果在说明订单已经操作过了，就不用再操作了
                        
                        if (Cache.Get(out_trade_no)==null)
                        {
                            LogUtil.WriteLog("Notify 页面  支付成功，支付信息：商家订单号：" + out_trade_no + "、支付金额(分)：" + total_fee + "、自定义参数：" + attach);
                            //如果交易成功，那么将值插入到缓存中
                            bool rechargeResult = await UpdateRechargeRecord(out_trade_no);

                            if (rechargeResult)
                            {
                                Cache.Insert(out_trade_no, out_trade_no, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20));
                            }
                           // LogUtil.WriteLog("============ 单次支付结束 ===============");
                        }


                        #endregion

                       
                       
                        string data = toWeixinServer.parseXML();
                         string returnMsg = HttpUtil.Send(data, "https://api.mch.weixin.qq.com/pay/unifiedorder");
                         LogUtil.WriteLog("Notify 页面  发送给微信服务器的返回信息：" + returnMsg);
                        return;
                    }
                    else
                    {
                        LogUtil.WriteLog("Notify 页面  支付失败，支付信息   total_fee= " + total_fee + "、err_code_des=" + err_code_des + "、result_code=" + result_code);
                        toWeixinServer.setParameter("return_code", "FAIL");
                        toWeixinServer.setParameter("return_msg", "交易失败");
                        string data = toWeixinServer.parseXML();
                        // string returnMsg = HttpUtil.Send(data, "https://api.mch.weixin.qq.com/pay/unifiedorder");
                        // LogUtil.WriteLog("Notify 页面  发送给微信服务器的返回信息：" + returnMsg);
                    }

                }
                else
                {
                    LogUtil.WriteLog("Notify 页面  isWXsign= false ，错误信息：" + error);
                }

                Response.End();
            }
            catch (Exception ee)
            {
                LogUtil.WriteLog("Notify 页面  发送异常错误：" + ee.Message);
            }
        }

        /// <summary>
        /// 获取订单信息发短信时用
        /// </summary>
        /// <returns></returns>
        private string getOrderInfo(IList<CartItem> ci)
        {
            
            string orderInfo = "";          
            try
            {
                if (ci.Count > 0)
                {                    
                    foreach (var item in ci)
                    {

                        orderInfo += item.OrderItem.ItemClass.Name + "-" + item.OrderItem.Name + item.Nums.ToString() + ",";


                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return orderInfo.TrimEnd(',');
        }
        /// <summary>
        /// 更新订单状态,这里的订单状态应该分为2种情况，一种是充值的订单，还有一种是在线支付的订单，
        /// 充值的订单，用户的余额+交易金额，修改交易记录，
        /// 在线支付，修改订单状态（Orderd订单）用accountRecord的Note存储orderNo,生成步骤记录
        /// </summary>
        /// <param name="Orderid">交易记录订单id</param>
        /// <returns></returns>
        private async Task<bool> UpdateRechargeRecord(string Orderid)
        {
            try
            {
                var db = ContextFactory.GetCurrentContext();
                var findOrder = db.AccountRecords.FirstOrDefault(a => a.TradeId == Orderid);
                if (findOrder == null)
                {
                    LogUtil.WriteLog("Notify 页面  没有找到订单");
                    return false;
                }
                var findUser = db.UserAccounts.FirstOrDefault(u => u.Id == findOrder.UserAccountId);
                if (findUser == null)
                {
                    LogUtil.WriteLog("Notify 页面  用户不存在");
                    return false;
                }
                //如果订单交易成功，那么直接返回
                if (findOrder.ResultType == ResultType.Succeedd)
                {
                    LogUtil.WriteLog("Notify 页面  订单交易已成功");
                    return false;
                }
                findOrder.ResultType = ResultType.Succeedd;
                db.Entry(findOrder).State = System.Data.Entity.EntityState.Modified;
                if (findOrder.TradeType == TradeType.Recharge)
                {
                    findUser.Balance = findUser.Balance + findOrder.TradeMoney;
                    db.Entry(findUser).State = System.Data.Entity.EntityState.Modified;
                }
                else if (findOrder.TradeType == TradeType.Consume)
                {
                    //Order订单
                    var FindOrder = db.Orders.FirstOrDefault(o => o.OrderNo == findOrder.Note);
                    if (FindOrder == null)
                    {
                        return false;
                    }
                    FindOrder.OrderStatus = OrderStatus.ToPatch;
                    FindOrder.PayType = PayType.Online;
                    db.Entry(FindOrder).State = System.Data.Entity.EntityState.Modified;
                    //生成步骤记录
                    OrderStep orderStep = new OrderStep()
                    {
                        OrderStatus = OrderStatus.ToPatch,
                        RiseTime = DateTime.Now,
                        OrderId = FindOrder.Id
                    };
                    db.OrderSteps.Add(orderStep);
                    #region 发送短信
                    try
                    {
                        var findSms = db.SmsConfigs.FirstOrDefault();
                        if (findSms != null)
                        {
                            string orderInfo = getOrderInfo(FindOrder.CartItems.ToList());

                            await SmsHelper.Send(findSms, string.Format("订单信息如下：联系人：{0},联系方式：{1},地址：{2},总价：{3}, 品类：（{4}），取件时间：{5}【白洗么】 ", FindOrder.UserAddress.Contact, FindOrder.UserAddress.ContactPhoneNum, FindOrder.UserAddress.Addr, FindOrder.Fact, orderInfo, FindOrder.Appointment.ToString("yyyy-MM-dd") + "  " + FindOrder.AppointmentTime), true, FindOrder.Agency.ContactMobile);
                         // await SmsHelper.Send(findSms, string.Format("代理商您好，您有一笔新的订单需要处理，订单信息如下：联系人：{0}，联系方式：{1}，地址：{2}，总价：{3}，取件时间：{4}【白洗么】 ", FindOrder.UserAddress.Contact, FindOrder.UserAddress.ContactPhoneNum, FindOrder.UserAddress.Addr, FindOrder.Fact, FindOrder.Appointment.ToString("yyyy-MM-dd") + "  " + FindOrder.AppointmentTime), true, FindOrder.Agency.ContactMobile);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtil.WriteLog("Notify 页面  短信异常，异常信息："+ex.ToString());
                    }
                    #endregion
                    #region 优惠券作废

                    var voucherModel= db.Vouchers.FirstOrDefault(v => v.Id == FindOrder.VoucherId);
                    if (voucherModel!=null)
                    {
                        voucherModel.IsUsed = true;
                        db.Entry(voucherModel).State = System.Data.Entity.EntityState.Modified;
                    }
                    #endregion
                }
                //如果这里的优惠券作废操作失败了，不影响整体的逻辑，
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog("Notify 页面  交易异常信息：" + ex.ToString());
                return false;
            }
        }

    }
}