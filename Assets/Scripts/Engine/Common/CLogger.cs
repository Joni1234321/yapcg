using UnityEngine;

namespace YAPCG.Engine.Common
{
    public static class CLogger
    {


        public static void Loading(string message, params object[] objects) => 
            Log(LogType.Log, "Loading", message, objects);
        public static void Loading(Object context, string message, params object[] objects) => 
            Log(LogType.Log, context, "Loading", message, objects);

        public static void Warning(string message, params object[] objects) =>
            Log(LogType.Warning, "Warning", message, objects); 
        
        
        #region Simple
        private static string FormatMessage(string prefix, string message) => $"[{prefix}] {message}";

        public static void Log (object obj) => Debug.Log(obj);
        public static void Log (object obj, Object context) => Debug.Log(obj, context);
        public static void Log (string message, params object[] objects) => Debug.LogFormat(message, objects);

        public static void Log (LogType type, string prefix, string message, params object[] objects) => Debug.LogFormat(type, LogOption.None, null, FormatMessage(prefix, message), objects);
        public static void Log (LogType type, Object context, string prefix, string message, params object[] objects) => Debug.LogFormat(type, LogOption.None, context, FormatMessage(prefix, message), objects);
        
        public static void Log (LogType type, LogOption options, string prefix, string message, params object[] objects) => Debug.LogFormat(type, options, null, FormatMessage(prefix, message), objects);
        #endregion

    }
}