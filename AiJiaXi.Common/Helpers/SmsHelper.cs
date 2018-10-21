using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Project.Domain.Entities.Configs;

namespace Project.Common.Helpers
{
    public class SmsHelper
    {
        public static async Task<bool> Send(SmsConfig config, string content, bool isNow, params string[] mobile)
        {
            string sendTime = isNow ? string.Empty : DateTime.Now.SimpleDateFull();
            /*StringBuilder body = new StringBuilder(string.Empty);


            body.AppendFormat(
                "{0}?action=send&userid=&account={1}&password={2}&mobile={3}&content={4}&sendTime={5}&extno=",
                config.SmsSendUrl, config.Account, config.Password,
                mobile.Aggregate(string.Empty, (current, item) => current + ("," + item)), content, sendTime);*/

            List<KeyValuePair<string, string>> formBody = new List<KeyValuePair<string, string>>();
            formBody.Add(new KeyValuePair<string, string>("action", "send"));
            formBody.Add(new KeyValuePair<string, string>("userid", string.Empty));
            formBody.Add(new KeyValuePair<string, string>("account", config.Account));
            formBody.Add(new KeyValuePair<string, string>("password", config.Password));
            formBody.Add(new KeyValuePair<string, string>("mobile", mobile.Aggregate(string.Empty, (current, item) => current + ("," + item))));
            formBody.Add(new KeyValuePair<string, string>("content", content));
            formBody.Add(new KeyValuePair<string, string>("sendTime", sendTime));
            formBody.Add(new KeyValuePair<string, string>("extno", String.Empty));

            var result = await HttpHelper.PostAsync(config.SmsSendUrl, formBody);
            NLogger.Debug(result);

            var xml = XDocument.Parse(result);
            var root = xml.Root;
            string returnStatus = root.Element("returnstatus").Value;
            string message = root.Element("message").Value;
            string remainpoint = root.Element("remainpoint").Value;
            string taskID = root.Element("taskID").Value;
            string successCounts = root.Element("successCounts").Value;

            StringBuilder log = new StringBuilder();
            log.Append("-----------------------------------------------------------------------");
            log.AppendLine(string.Format("短信发送内容：{0}", content));
            log.AppendLine(string.Format("短信发送状态：{0}", returnStatus));
            log.AppendLine(string.Format("返回信息：{0}", message));
            log.AppendLine(string.Format("返回余额：{0}", remainpoint));
            log.AppendLine(string.Format("返回本次任务的序列ID：{0}", taskID));
            log.AppendLine(string.Format("成功短信数：{0}", successCounts));
            log.Append("-----------------------------------------------------------------------");
            NLogger.Trace(log.ToString());

            return string.Equals(returnStatus, "Success", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}