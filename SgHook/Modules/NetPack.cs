#if NETPACK
using HarmonyLib;
using Manager.Operation;
using MelonLoader;
using Net;
using Net.Packet;
using Net.VO;
using System;
using System.Reflection;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace SgHook.Modules
{
    public class NetPack: ISgHookBase
    {
        private static NetPackConfig config;

        public void InitConfig()
        {
            config = Config.Conf.netPack;
        }
        public bool IsEnabled()
        {
            return config.enable;
        }
        public void Run()
        {
            if (config.debugUrl)
            {
                var original = typeof(Net.Packet.Packet).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Instance);
                var postfix = typeof(NetPack).GetMethod("Postfix_NetPack");
                SgHook.H0.Patch(original, postfix: new HarmonyMethod(postfix));
            }

            if (config.debugRequest)
            {
                var original = typeof(Net.VO.NetQuery<VOSerializer, VOSerializer>).GetMethod("GetRequest");
                var postfix = typeof(NetPack).GetMethod("Postfix_GetRequest");
                SgHook.H0.Patch(original, postfix: new HarmonyMethod(postfix));
            }


            if(config.debugResponse)
            {
                var original = typeof(Net.VO.NetQuery<VOSerializer, VOSerializer>).GetMethod("SetResponse");
                var prefix = typeof(NetPack).GetMethod("Prefix_SetResponse");
                SgHook.H0.Patch(original, prefix: new HarmonyMethod(prefix));
            }
            if (config.overrideBaseUrl != "") {
                var original = typeof(Manager.OperationManager).GetMethod("GetBaseUri", BindingFlags.Public | BindingFlags.Instance);
                var prefix = typeof(NetPack).GetMethod("Prefix_GetBaseUri");
                SgHook.H0.Patch(original, prefix: new HarmonyMethod(prefix));
            }
            if(config.disableObEnc)
            {
                var originalOb = typeof(Packet).GetMethod("Obfuscator", BindingFlags.Static|BindingFlags.Public);
                var prefixOb = typeof(NetPack).GetMethod("Prefix_Obfuscator");
                SgHook.H0.Patch(originalOb, prefix: new HarmonyMethod(prefixOb));

                Assembly assembly = Assembly.LoadFrom("Sinmai_Data/Managed/Assembly-CSharp.dll");
                Type cipherAESType = assembly.CreateInstance("Net.CipherAES").GetType();

                MelonLogger.Msg(cipherAESType);
                var originalEncrypt = cipherAESType.GetMethod("Encrypt", BindingFlags.Public| BindingFlags.Static);
                var prefixEncrypt = typeof(NetPack).GetMethod("Prefix_Encrypt");
                SgHook.H0.Patch(originalEncrypt, prefix: new HarmonyMethod(prefixEncrypt));
                
                var originalDecrypt = cipherAESType.GetMethod("Decrypt", BindingFlags.Public | BindingFlags.Static);
                var prefixDecrypt = typeof(NetPack).GetMethod("Prefix_Decrypt");
                SgHook.H0.Patch(originalDecrypt, prefix: new HarmonyMethod(prefixDecrypt));
            }
            if(config.removeUrlRegex != "")
            {
                var original = typeof(NetHttpClient).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
                var prefix = typeof(NetPack).GetMethod("Prefix_NetHttpClient");
                SgHook.H0.Patch(original, prefix: new HarmonyMethod(prefix));
            }
        }

        public static bool Prefix_Encrypt(ref byte[] __result, byte[] data)
        {
            __result = data;
            return false;
        }
        public static bool Prefix_Decrypt(ref byte[] __result, byte[] encryptData)
        {
            __result = encryptData;
            return false;
        }
        public static bool Prefix_Obfuscator(ref string __result, string srcStr)
        {
            if(config.removeSuffixRegex != "")
            {
                Regex pattern = new Regex(config.removeSuffixRegex);
                srcStr = pattern.Replace(srcStr, "");
            }
            __result = srcStr;
            return false;
        }
        public static void Prefix_NetHttpClient(ref string url)
        {
            Regex pattern = new Regex(config.removeUrlRegex);
            url = pattern.Replace(url, "");
            MelonLogger.Msg("Url After Patching: " + url);

        }
        public static void Postfix_NetPack(Net.Packet.Packet __instance)
        {
            var BaseUrlField = typeof(Net.Packet.Packet).GetField("BaseUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            var BaseUrl = BaseUrlField.GetValue(__instance) as string;
            var Query = __instance.Query;
            Toast.Create(Query.Api);
            MelonLogger.Msg("userID: " + Query.UserId + " urlBeforePatching: " + BaseUrl + Query.Api);

        }
        public static void Postfix_GetRequest(NetQuery<VOSerializer, VOSerializer> __instance)
        {
            var RequestField = __instance.Request;
            MelonLogger.Msg("Request: " + RequestField.Serialize());
        }
        public static void Prefix_SetResponse(NetQuery<VOSerializer, VOSerializer> __instance, string str)
        {
            MelonLogger.Msg("Response: " + str);
        }
        public static bool Prefix_GetBaseUri(ref string __result)
        {
            __result = config.overrideBaseUrl;
            MelonLogger.Msg("Overridden: " + __result);
            return false;
        }

    }
}
#endif