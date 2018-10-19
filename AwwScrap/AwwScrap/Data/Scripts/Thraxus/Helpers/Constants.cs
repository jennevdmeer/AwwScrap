using Sandbox.ModAPI;

namespace AwwScrap.Helpers
{
    public static class Constants
    {
        public static bool IsServer => MyAPIGateway.Multiplayer.IsServer;

        public static bool DebugMode { get; } = false;

        public static bool EnableProfilingLog { get; } = true;

        public static bool EnableGeneralLog { get; } = true;

        public static string DebugLogName { get; } = "AwwScrap_Debug";

        public static string ProfilingLogName { get; } = "AwwScrap_Profiling";

        public static string GeneralLogName { get; } = "AwwScrap_General";

        public const int TicksPerSecond = 60;

        public const int TicksPerMinute = TicksPerSecond * 60;

        public const int DefaultLocalMessageDisplayTime = 5000;

        public const int DefaultServerMessageDisplayTime = 10000;

        #region Networking

        public static ushort NetworkId => 16760;

        public const string ServerCommsPrefix = "ASServerMessage";

        #endregion
    }
}
