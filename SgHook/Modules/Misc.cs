using HarmonyLib;
using Manager;
using MelonLoader;
using Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Fx;
using Monitor;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace SgHook.Modules
{
    public class Misc : ISgHookBase
    {

        private static MiscConfig config;
        public void InitConfig()
        {
            config = Config.Conf.misc;
        }

        public bool IsEnabled()
        {
            return config.enable;
        }

        public void Run()
        {
            if (config.reloadKey != "")
            {
                var origin = typeof(CommonProcess).GetMethod("OnUpdate", BindingFlags.Instance | BindingFlags.Public);
                var postfix = typeof(Misc).GetMethod("Postfix_OnUpdate");
                SgHook.H0.Patch(origin, postfix: new HarmonyMethod(postfix));
            }
            if (config.eventOverride)
            {
                var origin = typeof(EventManager).GetMethod("IsOpenEvent", BindingFlags.Instance);
                var prefix = typeof(Misc).GetMethod("Prefix_IsOpenEvent");
                SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
            }

            if (Config.Conf.onProgramStart.singlePlayer & config.fixParticle)
            {
                var SetUpParticle_origin =
                    typeof(TapCEffect).GetMethod("SetUpParticle", BindingFlags.Public | BindingFlags.Instance);
                var SetUpParticle_prefix = typeof(Misc).GetMethod("Prefix_SetUpParticle");
                var SetUpParticle_postfix = typeof(Misc).GetMethod("Postfix_SetUpParticle");

                var FX_Controler_Play_origin =
                    typeof(FX_Controler).GetMethod("Play", BindingFlags.Public | BindingFlags.Instance);
                var FX_Controler_Play_prefix = typeof(Misc).GetMethod("Prefix_FX_Controler_Play");
                SgHook.H0.Patch(SetUpParticle_origin, prefix: new HarmonyMethod(SetUpParticle_prefix),
                    postfix: new HarmonyMethod(SetUpParticle_postfix));
                SgHook.H0.Patch(FX_Controler_Play_origin, prefix: new HarmonyMethod(FX_Controler_Play_prefix));
            }

        }
        public static bool Prefix_IsOpenEvent(ref bool __result)
        {
            __result = true;
            return false;
        }
        public static void Postfix_OnUpdate()
        {
            
            KeyCode key = (KeyCode)Enum.Parse(typeof(KeyCode), config.reloadKey);
            if (DebugInput.GetKeyDown(key))
            {
                var modules = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
                try
                {
                    Config.InitConfigFile();
                    foreach (var module in modules)
                    {
                        if (module.Namespace == "CNHook.Modules")
                        {
                            try
                            {
                                var moduleInstance = System.Activator.CreateInstance(module) as ISgHookBase;
                                moduleInstance.InitConfig();

                                Toast.Create($"Module {module} Config reloaded");
                            }
                            catch (System.Exception e)
                            {
                                MelonLogger.Error("Error when registing module: " + module);
                                MelonLogger.Error(e.ToString());
                            }
                        }
                    }
                }
                catch(Exception e) {
                    Toast.Create("Config reload err");
                    MelonLogger.Msg(e.Message);
                }
            }
        }

        private static bool particle_flag = false;

        public static bool Prefix_SetUpParticle(int monitorIndex)
        {
            particle_flag = true;
            return true;
        }

        public static void Postfix_SetUpParticle()
        {
            particle_flag = false;
        }

        public static bool Prefix_FX_Controler_Play(FX_Controler __instance)
        {
            if (particle_flag)
            {
                ParticleSystemRenderer[] compments = __instance.GetComponentsInChildren<ParticleSystemRenderer>(true);
                foreach (var c in compments)
                {
                    c.maxParticleSize = 1f;
                }
            }
            return true;
        }
            
    }
}
