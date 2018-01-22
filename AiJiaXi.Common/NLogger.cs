using NLog;

namespace AiJiaXi.Common
{
    /// <summary>
    /// 日志记录组件.
    /// </summary>
    public class NLogger
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 记录堆栈跟踪信息.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Trace(string message)
        {
            logger.Trace(message);
        }

        /// <summary>
        /// 记录调试信息.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Debug(string message)
        {
            logger.Debug(message);
        }

        /// <summary>
        /// 记录系统异常日志
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Error(string message)
        {
            logger.Error(message);
        }
    }
}