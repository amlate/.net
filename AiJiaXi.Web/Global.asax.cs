using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Project.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            OrderAutoDealTaskConfig.RegisterTasks();

            // 在应用程序启动时运行的代码 这里设置【10分钟】间隔 122400000 300000
            System.Timers.Timer myTimer = new System.Timers.Timer(600000);//修改时间间隔
                                                                             //关联事件
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(AutoExec);
            myTimer.AutoReset = true;
            myTimer.Enabled = true;

        }

        /// <summary>
        /// 定时执行的代码
        /// </summary>
        private void AutoExec(object sender, System.Timers.ElapsedEventArgs e)
        {
            string url = "http://localhost/Weixin/Index";
            System.Net.HttpWebRequest myHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            System.Net.HttpWebResponse myHttpWebResponse = (System.Net.HttpWebResponse)myHttpWebRequest.GetResponse();
            System.IO.Stream receiveStream = myHttpWebResponse.GetResponseStream();//得到回写的字节流

        }
        protected void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码
            //如果出错，删除下面代码
            //下面的代码是关键，可解决IIS应用程序池自动回收的问题
            System.Threading.Thread.Sleep(1000);
            ////这里设置你的web地址，可以随便指向你的任意一个aspx页面甚至不存在的页面，目的是要激发Application_Start
            ////string url = "http://www.xxxxx.com";
            string url = "http://localhost/Weixin/Index";
            System.Net.HttpWebRequest myHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            System.Net.HttpWebResponse myHttpWebResponse = (System.Net.HttpWebResponse)myHttpWebRequest.GetResponse();
            System.IO.Stream receiveStream = myHttpWebResponse.GetResponseStream();//得到回写的字节流

            //在此添加其它代码
        }
    }
}
