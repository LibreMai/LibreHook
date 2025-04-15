#if OVERRIDEPOWERON
using AMDaemon.Allnet;
using HarmonyLib;
using MelonLoader;
using System.Reflection;

namespace SgHook.Modules
{
    public class OverridePowerOn : ISgHookBase
    {
        private static OverridePowerOnConfig config;
        public void InitConfig()
        {
            config = Config.Conf.overridePowerOn;
        }
        public bool IsEnabled()
        {
            return config.enable;
        }

        public void Run()
        {
  
            if (config.overrideAllNetIsGood)
            {
                var originalAllNetIsGood = typeof(AMDaemon.Allnet.Auth).GetProperty("IsGood").GetMethod;
                var prefix = typeof(OverridePowerOn).GetMethod("Prefix_OverrideAllNetIsGood");
                SgHook.H0.Patch(originalAllNetIsGood, prefix: new HarmonyMethod(prefix));

            }
            if (config.overrideNetworkTest)
            {
                var originalNetworkTest = typeof(AMDaemon.NetworkTestInfo).GetMethod("GetResult", BindingFlags.Instance);
                var prefix = typeof(OverridePowerOn).GetMethod("Prefix_GetResult");
                SgHook.H0.Patch(originalNetworkTest, prefix: new HarmonyMethod(prefix));
            }
            if (config.dumpPowerOnProcess)
            {
                MelonLogger.Msg("Dumping PowerOnProcess");
                MemoryDump.CreateDump(MemoryDump.DumpType.MiniDumpWithFullMemory, "PowerOnProcess");
            }
        }




        public static bool Prefix_OverrideAllNetIsGood(ref bool __result)
        {
           __result = true;
            return false;
        }

        public static bool Prefix_GetResult(ref AMDaemon.TestResult __result)
        {
           __result = AMDaemon.TestResult.Good;
            return false;
        }

    }
}
#endif