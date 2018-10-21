namespace Project.Plugin.WeiXin.XmlDto
{
    public class PayDto
    {
        public string Appid { get; set; }

        public string MchId { get; set; }

        public string DeviceInfo { get; set; }

        public string NonceStr { get; set; }

        public string Sign { get; set; }

        public string Body { get; set; }

        public string Attach { get; set; }

        public string OutTradeNo { get; set; }

        public string FeeType { get; set; }

        public string TotalFee { get; set; }

        public string SpbillCreateIp { get; set; }

        public string TimeStrat { get; set; }

        public string TimeExpire { get; set; }

        public string GoodsTag { get; set; }

        public string NotifyUrl { get; set; }

        public string TradeType { get; set; }

        public string ProductId { get; set; }

        public string LimitPay { get; set; }

        public string OpenId { get; set; }
    }
}