using System;
using System.IO;
using System.Reflection;
using SimpleLogHelper;

namespace SendMessageSample
{
    internal static class LogHelper
    {
        private static readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        public static void AddLog(Exception e, MsgType type = MsgType.Error)
        {
            AddLog(e.Message, type);
        }

        public static void AddLog(string msg, MsgType type = MsgType.Information)
        {
            SimpleLogProxy.Instance.AddLog($"[{AssemblyName}]{'\t'}{msg}", type);
        }

        public static void InitLogPath()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Log\\");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, $"{AssemblyName}\\");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            SimpleLogProxy.Instance.SetLogPath(path);
        }

        public static void Stop()
        {
            SimpleLogProxy.Instance.Stop();
        }
    }
}
