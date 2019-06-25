using System;
using System.Runtime.CompilerServices;

namespace CMSC495Team3ServerApp.Logging
{
    public interface ILogger
    {
        void Debug(string content, [CallerFilePath] string callerName = "", [CallerMemberName] string memberName = "");

        void Info(string content, [CallerFilePath] string callerName = "", [CallerMemberName] string memberName = "");

        void Error(string content, [CallerFilePath] string callerName = "", [CallerMemberName] string memberName = "");

        void Error(string content, Exception exception, [CallerFilePath] string callerName = "",
            [CallerMemberName] string memberName = "");

        void Fatal(string content, [CallerFilePath] string callerName = "", [CallerMemberName] string memberName = "");

        void Fatal(string content, Exception exception, [CallerFilePath] string callerName = "",
            [CallerMemberName] string memberName = "");
    }
}