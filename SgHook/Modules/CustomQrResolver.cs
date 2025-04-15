using HarmonyLib;
using MelonLoader;
using System;
using System.Reflection;

namespace SgHook.Modules
{
    public class CustomQrResolver : ISgHookBase
    {
        public void InitConfig()
        {
        }
        public bool IsEnabled()
        {
            return false;
        }
        public void Run()
        {
            {
                Assembly assembly = Assembly.Load("ChimeLib.NET");
                Type CCommGetUserDataType = assembly.GetType("ChimeLib.NET.COMM.CCommGetUserData");
                var origin = CCommGetUserDataType.GetMethod("Create", BindingFlags.Instance | BindingFlags.NonPublic);
                var prefix = typeof(CustomQrResolver).GetMethod("Prefix_Create");
                SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
            }
        }
        public static void Prefix_Create(string strGameID, string strChipID, string strCommonKey, string strQRData)
        {
            MelonLogger.Msg($"strGameID:{strGameID} strChipID:{strChipID} strCommonKey:{strCommonKey}, strQRData:{strQRData}");
        }
    }
}
