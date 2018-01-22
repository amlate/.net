using System.ComponentModel;

namespace AiJiaXi.Domain.Enums
{
    public enum UserType
    {
        [Description("管理员")] Admin,

        [Description("代理商")] Agency,

        [Description("用户")] User,

        [Description("推广员")]Withdrawals,

        [Description("洗涤公司")]
        Washers
    }

    public enum Gender
    {
        [Description("先生")] Male,

        [Description("女士")] Female
    }

    public enum OrderImageType
    {
        [Description("瑕疵")]
        // 瑕疵图片
        BeforeWashing,

        [Description("洗涤后")]
        // 晒单图片
        Washed
    }

    public enum PayType
    {
        [Description("线上支付")]
        Online,

        [Description("取衣时付款")]
        Cod,

        [Description("转账")]
        Transfer
    }

    public enum OrderStatus
    {
        [Description("待支付")]//前台提交订单
        ToPay = 0,

        [Description("支付完成，衣物待取")]//前台支付完成
        ToPatch = 1,

        [Description("揽件成功")]
        Patched = 2,

        [Description("到达工厂")]
        InService = 3,

        [Description("洗涤完成")]
        ToDispatch = 4,

        [Description("送返中")]
        Dispatching = 5,

        [Description("已送达，客户签收")]
        Dispatched = 6,

        [Description("已成功")] //前台确认收货更新完成时间：CompleteTime
        Succeed = 7,

        [Description("已取消")]//前台取消订单
        Cancelled = 8,

        [Description("申请退款")]//前台申请退款
        ToRefund = 9,

        [Description("退款中")]
        Refundding = 10,

        [Description("已退款")]
        Refunded = 11,

        [Description("待确认")]
        ToConfirm = 99
    }

    public enum DealStatus
    {
        [Description("处理中")]
        Dealing,

        [Description("已处理")]
        Dealed,

        [Description("已忽略")]
        Ignored
    }
    public enum AgencyType
    {
        [Description("城市代理")] City,

        [Description("区县代理")] County
    }

    public enum TradeType
    {
        

        [Description("支出")] Consume,

        [Description("收入")] Recharge,

        [Description("积分支出")]
        ScoreConsume,

        [Description("积分收入")]
        ScoreIncome
    }

    public enum ResultType
    {
        [Description("成功")]
        Succeedd,

        [Description("失败")]
        Failed
    }

    public enum InServiceState
    {
        [Description("在职")] InService,

        [Description("离职")] OffService
    }

    public enum EventType
    {
        // [Description("运费优惠")] Freight,


        [Description("价格优惠")] Benefit,

        [Description("代金券")] Voucher,

        [Description("抽奖")] LuckyDraw,

        [Description("充值优惠")]
        Recharge,
        [Description("注册送代金券")]
        Register,

        //    ,

        //[Description("抢先优惠")]
        //Forestall
    }

    public enum ApplyStatus
    {
        [Description("待审核")] ToReview,

        [Description("审核通过")] Reviewed,

        [Description("不通过")] Failed
    }


    public enum LoginStatus
    {
        [Description("成功")] Success,

        [Description("失败")] Failed,

        [Description("验证码错误")] ValidateCodeFailed,

        [Description("用户名不存在")] UserNameFailed,

        [Description("密码错误")] PasswordFailed
    }

    public enum FeedbackStatus
    {
        [Description("未处理")]
        UnDealed,

        [Description("联系中")]
        Contacting,

        [Description("已联系")]
        Contacted,

        [Description("已忽略")]
        Ignored
    }

    //提现状态
    public enum WithdrawalsStatus
    {
        [Description("等待商家转帐")]
        UnDeaked,

        [Description("提现成功")]
        Success,

        [Description("提现失败")]
        Failed,

        [Description("预提现申请中")]
        PreWithdrawals,

    }

    //预提现状态
    public enum PreWithdrawalsStatus
    {
        [Description("空闲")]
        Free,

        [Description("可提现")]
        CanWithdrawals,
    }

    /// <summary>
    /// 投诉类型
    /// </summary>
    public enum ComplaintType
    {
        [Description("无投诉")]
        None,

        [Description("服务态度")]
        Attitude,

        [Description("取件投诉")]
        Patch,

        [Description("送返投诉")]
        Depatch,

        [Description("洗涤问题")]
        Washing,

        [Description("其它问题")]
        Other = 999
    }

    /// <summary>
    /// 前台活动判断用类型
    /// </summary>
    public enum ClientEventType
    {
        //大转盘
        [Description("无活动")]
        None,

        //58元洗三件
        [Description("商品活动")]
        Goods,

        //满70减10元
        [Description("价格优惠")]
        Benefit,

        //充值100送20
        [Description("充值优惠")]
        Recharge,

        //大转盘
        [Description("抽奖")]
        LuckyDraw



    }
}