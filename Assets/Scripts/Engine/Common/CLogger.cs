using Unity.Burst;
using UnityEngine;

namespace YAPCG.Engine.Common
{
    public static class CLogger
    {
        static class Prefixes
        {
            public const string 
                Loading = "Loading",
                Loaded = "Loaded",
                Duplication = "Duplication",
                Info = "Info";
        }

        [BurstDiscard]
        public static void LogLoading(Object c, string m, params object[] o) => Log(LogType.Log, c, Prefixes.Loading, m, o);
        [BurstDiscard]
        public static void LogLoaded(Object c, string m, params object[] o) => Log(LogType.Log, c, Prefixes.Loaded, m, o);

        
        public static void WarningDuplication(Object context, string message, params object[] objects) =>
            Log(LogType.Warning, context, Prefixes.Duplication, message, objects);

        public static void LogInfo (string message, params object[] objects) => Log(LogType.Log, Prefixes.Info, message, objects);
        public static void Warning(string message, params object[] objects) => Warning(null, message, objects);
        
        
        public static void Warning(Object context, string message, params object[] objects) =>
            Log(LogType.Warning, context, "Warning", message, objects); 

        
        
        #region Simple
        private static string FormatMessage(string prefix, string message) => $"[{prefix}] {message}";
        

        [BurstDiscard]
        private static void Log (LogType type, string prefix, string message, params object[] objects) => Debug.LogFormat(type, LogOption.None, null, FormatMessage(prefix, message), objects);
        [BurstDiscard]
        private static void Log (LogType type, Object context, string prefix, string message, params object[] objects) => Debug.LogFormat(type, LogOption.None, context, FormatMessage(prefix, message), objects);
        #endregion

    }
}