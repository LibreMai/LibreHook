using HarmonyLib;
using Main;
using Manager;
using Manager.Operation;
using MelonLoader;
using Monitor;
using Net;
using System;
using System.Reflection;

namespace SgHook.Modules
{
    public class BypassCheck : ISgHookBase
    {
        private static BypassCheckConfig config;
        public void InitConfig()
        {
            config = Config.Conf.bypassCheck;
        }

        public bool IsEnabled()
        {
            return config.enable;
        }
        public void Run()
        {
            {
                MethodInfo originalPrepareTimer = typeof(ProcessManager).GetMethod("PrepareTimer");
                MethodInfo prefixPrepareTimer = typeof(BypassCheck).GetMethod("Prefix_PrepareTimer");
                SgHook.H0.Patch(originalPrepareTimer, prefix: new HarmonyMethod(prefixPrepareTimer));
            }
            {
                var originalCalcSpecialNum = typeof(Manager.GameManager).GetMethod("CalcSpecialNum");
                var prefix = typeof(BypassCheck).GetMethod("Prefix_CalcSpecialNum");
                SgHook.H0.Patch(originalCalcSpecialNum, prefix: new HarmonyMethod(prefix));
            }
            if (config.disableIniFileClear) {
                var origin = typeof(MAI2System.IniFile).GetMethod("clear", BindingFlags.Public | BindingFlags.Instance);
                var prefix = typeof(BypassCheck).GetMethod("Prefix_IniFileClear");
                SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
            }
            if(config.forceAsServer) {
                {
                    var origin = typeof(AMDaemon.LanInstall).GetProperty("IsServer").GetMethod;
                    var prefix = typeof(BypassCheck).GetMethod("Prefix_IsServer");
                    SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
                }
                {
                    var origin = typeof(AMDaemon.Network).GetProperty("IsLanAvailable").GetMethod;
                    var prefix = typeof(BypassCheck).GetMethod("Prefix_IsLanAvailable");
                    SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));

                    MelonLogger.Msg("Bypass IsLanAvailable succeed(This msg only shows on load)");
                }
            }

            if(config.forceIgnoreError)
            {
                var origin = typeof(Main.GameMain).GetMethod("IsError",BindingFlags.NonPublic | BindingFlags.Instance);
                var prefix = typeof(BypassCheck).GetMethod("Prefix_IsError");
                SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));

                MelonLogger.Msg("Bypass IsError succeed(This msg only shows on load)");
                {
                    var originSet = typeof(AMDaemon.Error).GetMethod("Set", new System.Type[] { typeof(int), typeof(int) });
                    var prefixSet = typeof(BypassCheck).GetMethod("Prefix_ErrorSet");
                    SgHook.H0.Patch(originSet, prefix: new HarmonyMethod(prefixSet));
                }
            }
            if(config.keepCredits >=0)
            {
                {
                    var origin = typeof(AMDaemon.CreditUnit).GetProperty("Credit", BindingFlags.Public | BindingFlags.Instance).GetMethod;
                    var prefix = typeof(BypassCheck).GetMethod("Prefix_Credit");
                    SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
                }

                {
                    var origin = typeof(Manager.Credit).GetMethod("IsGameCostEnough", BindingFlags.Public | BindingFlags.Instance);
                    var prefix = typeof(BypassCheck).GetMethod("Prefix_IsGameCostEnough");
                    SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
                }
            }
            if(config.forceEnableQuickRetry)
            {
                var origin = typeof(QuickRetry).GetMethod("IsQuickRetryEnable", BindingFlags.NonPublic | BindingFlags.Instance);
                var prefix = typeof(BypassCheck).GetMethod("Prefix_IsQuickRetryEnable");
                SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
            }
            if(config.fixQuickRetryFastLateBug)
            {
                var originTypo = typeof(GamePlayManager).GetMethod("SetQuickRetryFrag", BindingFlags.Public| BindingFlags.Instance);
                var originEng = typeof(GamePlayManager).GetMethod("SetQuickRetryFlag", BindingFlags.Public | BindingFlags.Instance);

                var postfix = typeof(BypassCheck).GetMethod("Postfix_SetQuickRetryFlag");
                if(originEng != null)
                {
                    SgHook.H0.Patch(originEng, postfix: new HarmonyMethod(postfix));
                }
                if(originTypo != null)
                {
                    SgHook.H0.Patch(originTypo, postfix: new HarmonyMethod(postfix));
                }
            }
            if(config.disableMaintenance)
            {
                var originGetRemainingMinutes = typeof(MaintenanceTimer).GetMethod("GetRemainingMinutes", BindingFlags.Public | BindingFlags.Instance);
                var prefixGetRemainingMinutes = typeof(BypassCheck).GetMethod("Prefix_GetRemainingMinutes");
                SgHook.H0.Patch(originGetRemainingMinutes, prefix: new HarmonyMethod(prefixGetRemainingMinutes));

                var originIsShowRemainingMinutes = typeof(MaintenanceTimer).GetMethod("IsShowRemainingMinutes", BindingFlags.Public | BindingFlags.Instance);
                var prefixIsShowRemainingMinutes = typeof(BypassCheck).GetMethod("Prefix_IsShowRemainingMinutes");
                SgHook.H0.Patch(originIsShowRemainingMinutes, prefix: new HarmonyMethod(prefixIsShowRemainingMinutes));

                var originIsClosed = typeof(MaintenanceTimer).GetMethod("IsClosed", BindingFlags.Public | BindingFlags.Instance);
                var prefixIsClosed = typeof(BypassCheck).GetMethod("Prefix_IsClosed");
                SgHook.H0.Patch(originIsClosed, prefix: new HarmonyMethod(prefixIsClosed));
            }

            if(config.disableAutoReboot)
            {
                var originIsAutoRebootNeeded = typeof(MaintenanceTimer).GetMethod("IsAutoRebootNeeded", BindingFlags.Public | BindingFlags.Instance);
                var prefixIsAutoRebootNeeded = typeof(BypassCheck).GetMethod("Prefix_IsAutoRebootNeeded");
                SgHook.H0.Patch(originIsAutoRebootNeeded, prefix: new HarmonyMethod(prefixIsAutoRebootNeeded));
            }
            if(config.rewriteTitleRouterConfigLog > 0)
            {
                var origin = typeof(GameMain).GetMethod("GetRewrittenTitleRouterConfigLog", BindingFlags.Instance | BindingFlags.NonPublic);
                var prefix = typeof(BypassCheck).GetMethod("Prefix_GetRewrittenTitleRouterConfigLog");
                SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
            }
            if(config.bypassCheckServerHash)
            {
                var origin = typeof(NetHttpClient).GetMethod("CheckServerHash", BindingFlags.Static | BindingFlags.Public);
                var prefix = typeof(BypassCheck).GetMethod("Prefix_CheckServerHash");
                SgHook.H0.Patch(origin , prefix: new HarmonyMethod(prefix));
            }
        }
        public static bool Prefix_GetRewrittenTitleRouterConfigLog(ref int __result)
        {
            __result = config.rewriteTitleRouterConfigLog;
            return true;
        }
        public static void Postfix_SetQuickRetryFlag()
        {
            for (int i=0; i<4; i++)
            {
                uint fix = 0;
                var p = MAI2.Util.Singleton<Manager.GamePlayManager>.Instance.GetGameScore(i);
                p.GetType().GetProperty("Fast", BindingFlags.Public| BindingFlags.NonPublic| BindingFlags.Instance).SetValue(p, fix, null);
                p.GetType().GetProperty("Late", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(p, fix, null);
            }
        }
        public static bool Prefix_IsQuickRetryEnable(ref bool __result)
        {
            __result = true;
            return false;
        }
        public static bool Prefix_IsAutoRebootNeeded(ref bool __result)
        {
            __result = false;
            return false;
        }
        public static bool Prefix_GetRemainingMinutes(ref int __result)
        {
            __result = 60;
            return false;
        }
        public static bool Prefix_IsShowRemainingMinutes(ref bool __result)
        {
            __result = false;
            return false;
        }
        public static bool Prefix_IsClosed(ref bool __result)
        {
            __result = false;
            return false;
        }
        public static void Prefix_PrepareTimer(ref int second, int delayCount, bool isEntry, Action both, bool isVisible, ProcessManager __instance)
        {
            if(config.waitTime > 0)
            {
                second = config.waitTime;
            }
        }

        public static bool Prefix_CalcSpecialNum(ref int __result)
        {
            MelonLogger.Msg("Bypass environment check succeed!!");
            __result = config.magicNumber;
            return false;
        }
        public static bool Prefix_IniFileClear(ref MAI2System.IniFile __instance)
        {
            MelonLogger.Msg("Bypass ini file clear succeed!!");
            return false;
        }

        public static bool Prefix_IsServer(ref bool __result)
        {
            MelonLogger.Msg("Bypass IsServer succeed!!");
            __result = true;
            return false;
        }
        public static bool Prefix_IsLanAvailable(ref bool __result)
        {
            __result = false;
            return false;
        }
        public static bool Prefix_ErrorSet(ref bool __result)
        {
            MelonLogger.Msg("Bypass ErrorSet succeed!!");
            __result = false;
            return false;
        }
        public static bool Prefix_IsError(ref bool __result)
        {
            __result = false;
            return false;
        }
        public static bool Prefix_Credit(ref uint __result)
        {
            __result = config.keepCredits;
            return false;
        }
        public static bool Prefix_IsGameCostEnough(ref bool __result)
        {
            __result = true;
            return false;
        }
        public static bool Prefix_CheckServerHash(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
