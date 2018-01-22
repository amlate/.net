using System.Runtime.Remoting.Messaging;
using AiJiaXi.Domain;

namespace ZhiYuan.IAR.Repository.EF
{
    public class ContextFactory
    {
        /// <summary>
        /// 获取当前数据上下文
        /// </summary>
        /// <returns></returns>
        public static ApplicationDbContext GetCurrentContext()
        {
            ApplicationDbContext _nContext = CallContext.GetData("DefaultConnection") as ApplicationDbContext;
            if (_nContext == null)
            {
                _nContext = new ApplicationDbContext();
                CallContext.SetData("DefaultConnection", _nContext);
            }
            return _nContext;
        }  
    }
}