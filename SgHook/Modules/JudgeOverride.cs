using Fx;
using IO;
using Manager;
using Manager.UserDatas;
using Monitor;
using Monitor.TestMode.SubSequence;
using System.Reflection;
using System.Threading;
using UnityEngine;
using static Manager.CriAtomUserExtension;

namespace SgHook.Modules
{
    public class JudgeOverride: ISgHookBase
    {
        private static JudgeOverrideConfig config;
        public void InitConfig()
        {
            config = Config.Conf.judgeOverride;
        }
        public bool IsEnabled()
        {
            return config.enable;
        }
        public void Run()
        {
            {
                var origin = typeof(UserOption).GetMethod("GetAdjustMSec");
                var prefix = typeof(JudgeOverride).GetMethod("OffsetA");
                SgHook.H0.Patch(origin, prefix: new HarmonyLib.HarmonyMethod(prefix));
            }
            {
                var origin = typeof(UserOption).GetMethod("GetJudgeTimingFrame");
                var prefix = typeof(JudgeOverride).GetMethod("OffsetB");
                SgHook.H0.Patch(origin, prefix: new HarmonyLib.HarmonyMethod(prefix));
            }
            {
                var origin = typeof(SoundManager).GetMethod("Play", BindingFlags.Static | BindingFlags.Public);
                var prefix = typeof(JudgeOverride).GetMethod("MusicVolumnOverride");
                SgHook.H0.Patch(origin, prefix: new HarmonyLib.HarmonyMethod(prefix));
            }
            if (config.vSyncCount > 0) {
                QualitySettings.vSyncCount = config.vSyncCount;
            }
            if(config.particleRate<=2.0)
            {
                var origin = typeof(TapCEffect).GetMethod("SetUpParticle");
                var postfix = typeof(JudgeOverride).GetMethod("Postfix_SetUpParticle");
                SgHook.H0.Patch(origin, postfix: new HarmonyLib.HarmonyMethod(postfix));
            }
            if(config.wasapiExclusive)
            {
                var origin = typeof(CriAtomUserExtension).GetMethod("SetAudioClientShareMode", BindingFlags.Instance | BindingFlags.Public);
                var prefix = typeof(JudgeOverride).GetMethod("Prefix_SetAudioClientShareMode");
                SgHook.H0.Patch(origin, prefix: new HarmonyLib.HarmonyMethod(prefix));
            }
            if (config.touchPanelDelay > 0)
            {
                var origin = typeof(NewTouchPanel).GetMethod("Recv", BindingFlags.NonPublic | BindingFlags.Instance);
                var prefix = typeof(JudgeOverride).GetMethod("Recv_Prefix");
                SgHook.H0.Patch(origin, prefix: new HarmonyLib.HarmonyMethod(prefix));
            }
        }
        public static void Recv_Prefix()
        {
            Thread.Sleep(config.touchPanelDelay);
        }
        public static bool Prefix_SetAudioClientShareMode(ref AudioClientShareMode mode)
        {
            mode = AudioClientShareMode.Exclusive;
            return true;
        }
        public static void Postfix_SetUpParticle(TapCEffect __instance, FX_Mai2_Note_Color ____particleControler)
        {
            ParticleSystemRenderer[] componentsInChildren = ____particleControler.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].maxParticleSize = config.particleRate;
            }
        }
        public static bool OffsetA(ref float __result, UserOption __instance)
	    {
		    __result = (float)(__instance.AdjustTiming - 20 + 36 ) / 10f * 16.666666f + config.offsetA * 16.666666f;
		    return false;
	    }

	    public static bool OffsetB(ref float __result, UserOption __instance)
	    {
		    __result = (float)(__instance.JudgeTiming - 20 ) / 10f + config.offsetB;
		    return false;
	    }

        public static bool MusicVolumnOverride(SoundManager.AcbID acbID, ref float volume)
        {
            if (acbID == SoundManager.AcbID.Music)
            {
                volume = config.musicVolume;
            }
            return true;
        }

   }
}
