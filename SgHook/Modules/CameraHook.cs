using HarmonyLib;
using Manager;
using MelonLoader;
using UnityEngine;
using System.Reflection;
using ZXing;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using ZXing.Multi.QrCode;

namespace SgHook.Modules
{
    public class CameraHook : ISgHookBase
    {
        private static CameraHookConfig config;
        public void InitConfig()
        {
            config = Config.Conf.cameraHook;
        }
        public bool IsEnabled()
        {
            return config.enable;
        }
        public void Run()
        {
            //启用玩家摄像头扫码
            if (config.playerPhotoCameraScanQR)
            {
                var GetTexture= typeof(CameraManager).GetMethod(nameof(CameraManager.GetTexture), BindingFlags.Public | BindingFlags.Static);
                var _GetTexturePrefix = typeof(CameraHook).GetMethod(nameof(GetTexturePrefix));
                SgHook.H0.Patch(GetTexture, new HarmonyMethod(_GetTexturePrefix), null, null, null);
            }
            //取消所有WebCamTexture的初始化限制
            if (config.skipWebCamTextureInitLimit) { 
                var WebCamTextureInit = typeof(WebCamTexture).GetConstructor(new System.Type[] { typeof(string), typeof(int), typeof(int), typeof(int) });
                var _WebCamTexturePrefix= typeof(CameraHook).GetMethod(nameof(WebCamTexturePrefix));
                SgHook.H0.Patch(WebCamTextureInit, new HarmonyMethod(_WebCamTexturePrefix), null, null, null);
            }
            //修复二维码扫描识别错位
            if (config.fixQRCodeScanResLimit)
            {
                var GetDecodeStringsMethod = typeof(ChimeDevice).GetMethod(nameof(ChimeDevice.GetDecodeStrings), BindingFlags.Public | BindingFlags.Instance);
                var _GetDecodeStringsMethodPrefix = typeof(CameraHook).GetMethod(nameof(GetDecodeStringsMethodPrefix));
                SgHook.H0.Patch(GetDecodeStringsMethod, new HarmonyMethod(_GetDecodeStringsMethodPrefix), null, null, null);
            }

            if (config.hookCameraInitialize)
            {
                var CameraInitialize = typeof(CameraManager).GetMethod("CameraInitialize",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                var _CameraInitializeTrans = typeof(CameraHook).GetMethod(nameof(CameraInitializeTranspiler));
                SgHook.H0.Patch(CameraInitialize, null, null, transpiler: new HarmonyMethod(_CameraInitializeTrans),
                    null);
                if (config.showCamerasList)
                {
                    MelonLogger.Msg("======================================");
                    MelonLogger.Msg("WebCam List:");
                    for (int i = 0; i < WebCamTexture.devices.Length; i++)
                    {
                        MelonLogger.Msg($"{i} {WebCamTexture.devices[i].name}");
                    }
                    MelonLogger.Msg("======================================");
                }
            }
        }
        public static bool GetTexturePrefix(CameraManager.CameraTypeEnum type,ref WebCamTexture __result)
        {
            if (type==CameraManager.CameraTypeEnum.Chime)
            {
                __result = CameraManager.GetTexture(CameraManager.CameraTypeEnum.Photo);
                return false;
            }
            return true;
        }
        public static bool WebCamTexturePrefix(ref WebCamTexture __instance,  string deviceName, int requestedWidth, int requestedHeight, int requestedFPS)
        {
            var overrideInit=typeof(WebCamTexture).GetMethod("Internal_CreateWebCamTexture", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new System.Type[] { typeof(WebCamTexture), typeof(string), typeof(int), typeof(int), typeof(int) }, null);
            overrideInit.Invoke(__instance, new object[] {__instance,deviceName,0,0,0});
            return false;
        }

        class _InternalCamTpe
        {
            public Color32LuminanceSource color32LuminanceSource;
            public Color32[] color32s;
        }
        static Dictionary<WebCamTexture, _InternalCamTpe> keyValuePairs = new Dictionary<WebCamTexture, _InternalCamTpe>();
        public static void GetDecodeStringsMethodPrefix(ref Color32[] ____colors,ref WebCamTexture ____texture,ref Color32LuminanceSource ____sourceCameraID)
        {
            if (____texture == null )
            {
                return;
            }
            if (!keyValuePairs.ContainsKey(____texture))
            {
                keyValuePairs.Add(____texture,new _InternalCamTpe()
                {
                    color32s=new Color32[____texture.width * ____texture.height],
                    color32LuminanceSource= new Color32LuminanceSource(____texture.width, ____texture.height)
                });
            }
            ____colors = keyValuePairs[____texture].color32s;
            ____sourceCameraID = keyValuePairs[____texture].color32LuminanceSource;
            ____sourceCameraID.SetPixels(____colors);
        }

        public static IEnumerable<CodeInstruction> CameraInitializeTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo trans =
                typeof(CameraHook).GetMethod(nameof(CameraInitializeTrans), BindingFlags.Public | BindingFlags.Static);
            return new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Call, trans),
                new CodeInstruction(OpCodes.Ret)
            };
        }
        

        public static IEnumerator CameraInitializeTrans(CameraManager cm)
        {
            cm.Message = "カメラを探しています";
            if (WebCamTexture.devices.Length == 0)
            {
                cm.Message = "接続されているカメラが見つかりませんでした";
                CameraManager.IsReady = true;
                MelonLogger.Warning("No WebCamDevice found!");
                yield break;
            }
            yield return new WaitForSeconds(1f);
            int camIndex = 0;
            //fields and methods
            FieldInfo webcamtexField = AccessTools.Field(typeof(CameraManager), "_webcamtex");
            WebCamTexture[] _webcamtex = (WebCamTexture[])webcamtexField.GetValue(cm);
            FieldInfo cameraTypesField = AccessTools.Field(typeof(CameraManager), "_cameraTypes");
            CameraManager.CameraTypeEnum[] _cameraTypes = (CameraManager.CameraTypeEnum[])cameraTypesField.GetValue(cm);
            WebCamTexture photoWCT = null,chimeWCT = null,availableWCT = null;
            
            
            //try to init specified photo WCT
            if (config.specifyCameraPhoto >= 0)
            {
                WebCamTexture wct = new WebCamTexture(WebCamTexture.devices[config.specifyCameraPhoto].name, 0, 0, 0);
                cm.Message = "カメラチェック:" + wct.deviceName + " (Photo)";
                Stopwatch timer = new Stopwatch();
                wct.Play();
                bool isCamTimeout = false;
                timer.Restart();
                while (!wct.isPlaying)
                {
                    if (timer.ElapsedMilliseconds >= config.selfCheckMilliSeconds)
                    {
                        isCamTimeout = true;
                        break;
                    }
                    yield return new WaitForSeconds(1f);
                }
                wct.Stop();
                if (!isCamTimeout)
                {
                    photoWCT = wct;
                    MelonLogger.Msg($"Photo WebCam found: {wct.deviceName}");
                }
            }
            
            //try to init specified chime WCT
            if (config.specifyCameraChime >= 0)
            {
                WebCamTexture wct = new WebCamTexture(WebCamTexture.devices[config.specifyCameraChime].name, 0, 0, 0);
                cm.Message = "カメラチェック:" + wct.deviceName + " (Photo)";
                Stopwatch timer = new Stopwatch();
                wct.Play();
                bool isCamTimeout = false;
                timer.Restart();
                while (!wct.isPlaying)
                {
                    if (timer.ElapsedMilliseconds >= config.selfCheckMilliSeconds)
                    {
                        isCamTimeout = true;
                        break;
                    }
                    yield return new WaitForSeconds(1f);
                }
                wct.Stop();
                if (!isCamTimeout)
                {
                    chimeWCT = wct;
                    MelonLogger.Msg($"Chime WebCam found: {wct.deviceName}");
                }
            }

            if (photoWCT == null || chimeWCT == null)
            {
                MelonLogger.Msg("Finding available WebCam...");
                foreach (var device in WebCamTexture.devices)
                {
                    cm.Message = "カメラチェック:" + device.name;
                    WebCamTexture wct = new WebCamTexture(device.name, 0, 0, 0);
                    Stopwatch timer = new Stopwatch();
                    wct.Play();
                    bool isCamTimeout = false;
                    timer.Restart();
                    while (!wct.isPlaying)
                    {
                        if (timer.ElapsedMilliseconds >= config.selfCheckMilliSeconds)
                        {
                            isCamTimeout = true;
                            break;
                        }
                        yield return new WaitForSeconds(1f);
                    }
                    wct.Stop();
                    if (!isCamTimeout)
                    {
                        availableWCT = wct;
                        MelonLogger.Msg($"Available WebCam found: {wct.deviceName}");
                        break;
                    }
                }
                if (availableWCT == null)
                {
                    cm.Message = "カメラは見つけたけど再生に失敗";
                    for (int i = 0; i < 2; i++)
                    {
                        _cameraTypes[i] = CameraManager.CameraTypeEnum.Unknown;
                    }
                    MelonLogger.Warning("No available WebCam found!");
                    MelonLogger.Warning("Cameras Initialize failed!");
                    CameraManager.IsReady = true;
                    yield break;
                }
                if (photoWCT == null) photoWCT = availableWCT;
                if (chimeWCT == null) photoWCT = availableWCT;
                
            }
            
            camIndex = 0;
            _webcamtex[camIndex] = photoWCT;
            _cameraTypes[camIndex] = CameraManager.CameraTypeEnum.Photo;
            cm.isAvailableCamera[(int)_cameraTypes[camIndex]] = true;
            cm.cameraProcMode[(int)_cameraTypes[camIndex]] = CameraManager.CameraProcEnum.Good;
            CameraManager.DeviceId[(int)_cameraTypes[camIndex]] = camIndex;
            camIndex = 1;
            _webcamtex[camIndex] = chimeWCT;
            _cameraTypes[camIndex] = CameraManager.CameraTypeEnum.Chime;
            cm.isAvailableCamera[(int)_cameraTypes[camIndex]] = true;
            cm.cameraProcMode[(int)_cameraTypes[camIndex]] = CameraManager.CameraProcEnum.Good;
            CameraManager.DeviceId[(int)_cameraTypes[camIndex]] = camIndex;
            CameraManager.IsReady = true;
            yield break;
        }
    }   
}
