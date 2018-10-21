using System.ComponentModel;
using Newtonsoft.Json;

namespace Project.Domain.JsonModel
{
    public class LoginReturn
    {
        [JsonProperty(PropertyName = "valid")]
        public bool Valid { get; set; }

        [JsonProperty(PropertyName = "msg")]
        public string Msg { get; set; }

        [JsonProperty(PropertyName = "returnUrl")]
        [DefaultValue("/admin/home/index")]
        public string ReturnUrl { get; set; }
    }
}