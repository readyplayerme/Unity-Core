using UnityEngine;

namespace ReadyPlayerMe.Core
{
    /// <summary>
    ///     Used for logging in SDK. Currently logs to unity's debugger.
    ///     <remarks>
    ///         Logging is enabled by default, if required can be disabled.
    ///         <code> SDKLogging.logEnabled = false; </code>
    ///     </remarks>
    /// </summary>
    public static class SDKLogger
    {
        public static readonly Logger AvatarLoaderLogger = new Logger(new CustomLogHandler());

        public static void Log(string tag, object message)
        {
            AvatarLoaderLogger.Log(tag, message);
        }

        public static void LogWarning(string tag, object message)
        {
            AvatarLoaderLogger.LogWarning(tag, message);
        }
    }
}
