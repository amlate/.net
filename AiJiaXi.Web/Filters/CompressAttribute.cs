using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace AiJiaXi.Web.Filters
{
    /// <summary>
    /// 压缩Http响应，提高访问效率
    /// </summary>
    public class CompressAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在执行操作方法之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 获得并解析Http编码
            var preferredEncoding = this.GetPreferedEncoding(filterContext.HttpContext.Request);

            // 压缩响应
            var response = filterContext.HttpContext.Response;

            response.AppendHeader("Content-encoding", preferredEncoding.ToString());

            if (preferredEncoding == CompressScheme.Gzip)
            {
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }

            if (preferredEncoding== CompressScheme.Deflate)
            {
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }

            return;
        }

        private CompressScheme GetPreferedEncoding(HttpRequestBase request)
        {
            var acceptableEncoding = request.Headers["Accept-Encoding"].ToLower();
            // acceptableEncoding = SortEncodings(acceptableEncoding);
            if (acceptableEncoding.Contains("gzip"))
            {
                return CompressScheme.Gzip;
            }

            if (acceptableEncoding.Contains("deflate"))
            {
                return CompressScheme.Deflate;
            }

            return CompressScheme.Identity;
        }

        enum CompressScheme
        {
            Gzip = 0,
            Deflate = 1,
            Identity =2
        }
    }
}