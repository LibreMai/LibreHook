using System;
using System.Threading;

namespace SgHook.Modules
{
    public class Useless: ISgHookBase
    {
        private static UselessConfig config;
        public void InitConfig()
        {
            config = Config.Conf.useless;
        }

        public bool IsEnabled()
        {
            return config.enable;
        }

        public void Run()
        { 
            try
            {
                new Thread(() =>
                {
                    while (config.hideDerakKumaHead)
                    {
                        Thread.Sleep(100);
                        try
                        {
                            UnityEngine.GameObject.Find("Derakkuma/Body/Head").SetActive(false);
                        }
                        catch (Exception _) { }
                    }
                }).Start();
            } catch (Exception _) { }
        }
    }   
}
