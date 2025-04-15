using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace SgHook.Modules
{
    public class InputRestore : ISgHookBase
    {
        private static InputRestoreConfig config;
        public void InitConfig()
        {
            config = Config.Conf.inputRestore;
        }
        public bool IsEnabled()
        {
            return config.enable;
        }
        public void Run()
        {
            if (config.restoreDebugInput)
            {
                var originalGetKey = typeof(DebugInput).GetMethod("GetKey", BindingFlags.Public | BindingFlags.Static);
                var prefixGetKey = typeof(InputRestore).GetMethod("Prefix_GetKey", BindingFlags.Public | BindingFlags.Static);
                SgHook.H0.Patch(originalGetKey, prefix: new HarmonyMethod(prefixGetKey));

                var originalGetKeyDown = typeof(DebugInput).GetMethod("GetKeyDown", BindingFlags.Public | BindingFlags.Static);
                var prefixGetKeyDown = typeof(InputRestore).GetMethod("Prefix_GetKeyDown", BindingFlags.Public | BindingFlags.Static);
                SgHook.H0.Patch(originalGetKeyDown, prefix: new HarmonyMethod(prefixGetKeyDown));

                var originalGetKeyUp = typeof(DebugInput).GetMethod("GetKeyUp", BindingFlags.Public | BindingFlags.Static);
                var prefixGetKeyUp = typeof(InputRestore).GetMethod("Prefix_GetKeyUp", BindingFlags.Public | BindingFlags.Static);
                SgHook.H0.Patch(originalGetKeyUp, prefix: new HarmonyMethod(prefixGetKeyUp));

                var originalGetButton = typeof(DebugInput).GetMethod("GetButton", BindingFlags.Public | BindingFlags.Static);
                var prefixGetButton = typeof(InputRestore).GetMethod("Prefix_GetButton", BindingFlags.Public | BindingFlags.Static);
                SgHook.H0.Patch(originalGetButton, prefix: new HarmonyMethod(prefixGetButton));

                var originalGetButtonDown = typeof(DebugInput).GetMethod("GetButtonDown", BindingFlags.Public | BindingFlags.Static);
                var prefixGetButtonDown = typeof(InputRestore).GetMethod("Prefix_GetButtonDown", BindingFlags.Public | BindingFlags.Static);
                SgHook.H0.Patch(originalGetButtonDown, prefix: new HarmonyMethod(prefixGetButtonDown));

                var originalGetButtonUp = typeof(DebugInput).GetMethod("GetButtonUp", BindingFlags.Public | BindingFlags.Static);
                var prefixGetButtonUp = typeof(InputRestore).GetMethod("Prefix_GetButtonUp", BindingFlags.Public | BindingFlags.Static);
                SgHook.H0.Patch(originalGetButtonUp, prefix: new HarmonyMethod(prefixGetButtonUp));

                var originalGetMouseButton = typeof(DebugInput).GetMethod("GetMouseButton", BindingFlags.Public | BindingFlags.Static);
                var prefixGetMouseButton = typeof(InputRestore).GetMethod("Prefix_GetMouseButton", BindingFlags.Public | BindingFlags.Static);
                SgHook.H0.Patch(originalGetMouseButton, prefix: new HarmonyMethod(prefixGetMouseButton));

                var originalGetMouseButtonDown = typeof(DebugInput).GetMethod("GetMouseButtonDown", BindingFlags.Public | BindingFlags.Static);
                var prefixGetMouseButtonDown = typeof(InputRestore).GetMethod("Prefix_GetMouseButtonDown", BindingFlags.Public | BindingFlags.Static);
                SgHook.H0.Patch(originalGetMouseButtonDown, prefix: new HarmonyMethod(prefixGetMouseButtonDown));

                var originalGetMouseButtonUp = typeof(DebugInput).GetMethod("GetMouseButtonUp", BindingFlags.Public | BindingFlags.Static);
                var prefixGetMouseButtonUp = typeof(InputRestore).GetMethod("Prefix_GetMouseButtonUp", BindingFlags.Public | BindingFlags.Static);
                SgHook.H0.Patch(originalGetMouseButtonUp, prefix: new HarmonyMethod(prefixGetMouseButtonUp));

            }
        }
        public static bool Prefix_GetKey(ref bool __result, KeyCode name)
        {
            __result = Input.GetKey(name);
            return false;
        }
        public static bool Prefix_GetKeyDown(ref bool __result, KeyCode name)
        {
            __result = Input.GetKeyDown(name);
            return false;
        }
        public static bool Prefix_GetKeyUp(ref bool __result, KeyCode name)
        {
            __result = Input.GetKeyUp(name);
            return false;
        }
        public static bool Prefix_GetButton(ref bool __result, string name)
        {
            __result = Input.GetButton(name);
            return false;
        }
        public static bool Prefix_GetButtonDown(ref bool __result, string name)
        {
            __result = Input.GetButtonDown(name);
            return false;
        }
        public static bool Prefix_GetButtonUp(ref bool __result, string name)
        {
            __result = Input.GetButtonUp(name);
            return false;
        }
        public static bool Prefix_GetMouseButton(ref bool __result, int button)
        {

            __result = Input.GetMouseButton(button);
            return false;
        }
        public static bool Prefix_GetMouseButtonDown(ref bool __result, int button)
        {
            __result = Input.GetMouseButtonDown(button);
            return false;
        }
        public static bool Prefix_GetMouseButtonUp(ref bool __result, int button)
        {
            __result = Input.GetMouseButtonUp(button);
            return false;
        }
    }
}
