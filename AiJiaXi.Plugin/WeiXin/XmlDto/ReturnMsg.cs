namespace AiJiaXi.Plugin.WeiXin.XmlDto
{
    public class ReturnMsg
    {
        public string Code { get; set; }

        public string Msg { get; set; }

        public string Appid { get; set; }

        public string MchId { get; set; }

        public string DeviceInfo { get; set; }

        public string NonceStr { get; set; }

        public string Sign { get; set; }

        public string ResultCode { get; set; }

        public string ErrCode { get; set; }

        public string ErrCodeDes { get; set; }

        public string TradeType { get; set; }

        public string PrepayId { get; set; }

        public string CodeUrl { get; set; }
    }
}