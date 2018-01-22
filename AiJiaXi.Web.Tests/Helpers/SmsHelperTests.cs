using System;
using AiJiaXi.Common.Helpers;
using AiJiaXi.Domain.Entities.Configs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AiJiaXi.Web.Tests.Helpers
{
    [TestClass()]
    public class SmsHelperTests
    {
        //http://114.113.154.5:8080/Home/Index  报备网址


        [TestMethod()]
        public void SendTest()
        {
          
            var config = new SmsConfig()
            {
                EntId = string.Empty,
                Account = "AA00250",
                Password = "AA0025058",
                ExtNo = String.Empty,
                SmsSendUrl = "http://dx.ipyy.net/sms.aspx"
            };

            var result = SmsHelper.Send(config, "短信验证码为：123456 【白洗么】", true, "13322232003");

            Assert.AreEqual(result.Result, true);
        }
    }
}
