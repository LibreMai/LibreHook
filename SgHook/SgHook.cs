using MelonLoader;
using System;

[assembly: MelonInfo(typeof(SgHook.SgHook), SgHook.BuildInfo.Name, SgHook.BuildInfo.Version, SgHook.BuildInfo.Author)]
namespace SgHook
{
    public static class BuildInfo
    {
        public const string Name = "SgHook";
        public const string Description = "Plugin for WMDX2023";
        public const string Author = "Wat";
        public const string Company = "nop";
        public const string Version = "1.30.0";
    }
    public class SgHook : MelonMod
    {
        private static HarmonyLib.Harmony h0;
        public static HarmonyLib.Harmony H0 => h0;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg(" _________  ");
            MelonLogger.Msg("|  _/ \\_  | ");
            MelonLogger.Msg("| `.   ,' | ");
            MelonLogger.Msg("|__/,^.\\__| ");
            MelonLogger.Msg("");
            MelonLogger.Msg("love from vietnam");
            MelonLogger.Msg("=========================================");

            MelonLogger.Msg("Welcome to maimai!");
            MelonLogger.Msg("Reading ConfigFile...");
            Config.InitConfigFile();
            MelonLogger.Msg("CNMod Config:" + Config.Conf.greeting);
            h0 = this.HarmonyInstance;
            OnProgramStart.Run();
            MelonLogger.Msg("=========================================");
            if (!Metric.getTitle()) return;
            MelonLogger.Msg("");
            MelonLogger.Msg("=========================================");
            MelonLogger.Msg("Registing moudles...");
            // reflect get all class under namespace CNMod.Modules
            var modules = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            foreach (var module in modules)
            {
                if (module.Namespace == "SgHook.Modules")
                {
                    try
                    {
                        var moduleInstance = System.Activator.CreateInstance(module) as ISgHookBase;
                        moduleInstance.InitConfig();
                        if (moduleInstance.IsEnabled())
                        {
                            moduleInstance.Run();
                            MelonLogger.Msg("Registed module: " + moduleInstance);
                        }
                    }
                    catch (NullReferenceException _) { }
                    catch (MissingMethodException _)
                    {
                        // TODO: Check CameraHook Transpiler
                    }
                    catch (System.Exception e)
                    {
                        MelonLogger.Error("Error when registing module: " + module);
                        MelonLogger.Error(e.ToString());
                    }
                }
            }
            MelonLogger.Msg("=========================================");
            // new thread
            new System.Threading.Thread(() =>
            {
                Toast.Job();
                MelonLogger.Msg("Toast Registed");
            }).Start();
        }
    }
}
    