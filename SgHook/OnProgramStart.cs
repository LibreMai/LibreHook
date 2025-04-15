using HarmonyLib;
using IO;
using Main;
using MelonLoader;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SgHook
{

    public class OnProgramStart
    {
        private static OnProgramStartConfig config;

        public static void Run()
        {
            try
            {
                config = Config.Conf.onProgramStart;
                if (config.enable)
                {
                    if (config.probeAllSensitive) ProbeAllSenSitive();
                    if (config.singlePlayer) SinglePlayer();
                } 
            } catch (Exception e)
            {
                MelonLogger.Msg(e.ToString());
            }
        }

        public static void SinglePlayer()
        {
            try
            {
                MethodInfo origin = typeof(GameMainObject).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo postfix = typeof(OnProgramStart).GetMethod("Postfix_SinglePlayer", BindingFlags.Static | BindingFlags.Public);
                SgHook.H0.Patch(origin, postfix: new HarmonyMethod(postfix));

                MethodInfo MeshButton_Awake_origin =
                    typeof(MeshButton).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo MeshButton_Awake_postfix =
                    typeof(OnProgramStart).GetMethod(nameof(Postfix_MeshButton_Awake));
                SgHook.H0.Patch(MeshButton_Awake_origin, postfix: new HarmonyMethod(MeshButton_Awake_postfix));
                
                MelonLogger.Msg("Single Player Mode Hooked");
            }
            catch (Exception e)
            {
                MelonLogger.Msg(e.ToString());
            }
        }

        public static void Postfix_SinglePlayer(GameMainObject __instance)
        {
            Vector3 position = Camera.main.gameObject.transform.position;
            Camera.main.gameObject.transform.position = new Vector3(position.x - 540f, position.y, position.z);
            ((Transform)typeof(GameMainObject).GetField("rightMonitor", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance)).transform.localScale = Vector3.zero;
        }

        public static void Postfix_MeshButton_Awake(MeshButton __instance)
        {
            CustomGraphic customGraphic = __instance.targetGraphic as CustomGraphic;
            FieldInfo vertexArrayFieldInfo = AccessTools.Field(typeof(MeshButton), "vertexArray");
            Vector2[] vertexArray = (Vector2[])vertexArrayFieldInfo.GetValue(__instance);
            for (int i = 0; i < customGraphic.vertex.Count; i++)
            {
                vertexArray[i] = RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector2(__instance.transform.position.x + customGraphic.vertex[i].x + 540f, __instance.transform.position.y + customGraphic.vertex[i].y));
            }
        }

        public static void ProbeAllSenSitive()
        {
            try
            {
                MelonLogger.Warning("Try to get QRImage.Key");
                FieldInfo keyField = typeof(QRImage).GetField("Key", BindingFlags.NonPublic | BindingFlags.Static);
                byte[] keyValue = (byte[])keyField.GetValue(null);
                var hexString = string.Join("", keyValue.Select(b => b.ToString("X2")));
#if SENSITIVE
                MelonLogger.Msg(hexString);
#endif
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.ToString());
            }
            try
            {
                MelonLogger.Warning("Try to get CipherAES.AesKey");
                Assembly assembly = Assembly.Load("Assembly-CSharp");
                Type cipherAESType = assembly.GetType("Net.CipherAES");
                FieldInfo aesKeyFieldInfo = cipherAESType.GetField("AesKey", BindingFlags.NonPublic | BindingFlags.Static);
                string aesKeyValue = aesKeyFieldInfo.GetValue(null) as string;
#if SENSITIVE
                MelonLogger.Msg(aesKeyValue);
#endif
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.ToString());
            }
            try
            {
                MelonLogger.Warning("Try to get CipherAES.AesIV");
                Assembly assembly = Assembly.Load("Assembly-CSharp");
                Type cipherAESType = assembly.GetType("Net.CipherAES");
                FieldInfo aesIVFieldInfo = cipherAESType.GetField("AesIV", BindingFlags.NonPublic | BindingFlags.Static);
                string aesIVValue = aesIVFieldInfo.GetValue(null) as string;
#if SENSITIVE
                MelonLogger.Msg(aesIVValue);
#endif
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.ToString());
            }
            try
            {
                MelonLogger.Warning("Try to get Net.Packet.Packet.ObfuscateParam");
                FieldInfo param = typeof(Net.Packet.Packet).GetField("ObfuscateParam", BindingFlags.NonPublic | BindingFlags.Static);
                string str = param.GetValue(null) as string;
#if SENSITIVE
                MelonLogger.Msg(str);
#endif
            }
            catch (Exception e)
            {
                MelonLogger.Error(e.ToString());
            }
        }
    }
}
