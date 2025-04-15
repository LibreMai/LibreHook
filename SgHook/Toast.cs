using MAI2.Util;
using MAI2System;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
namespace SgHook
{
    public class Toast
    {
        public static ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        public static void Create(string content)
        {
            queue.Enqueue(content);
        }
        private static bool isTitle = false;

        public static void Job()
        {
            // DO NOT USE THIS!!
            // FIND GAMEOBJECT ONLY WORKS FINE IN MAIN THREAD!!
            while (true)
            {
                try 
                {
                    if (queue.Count == 0)
                    {
                        if (!isTitle)
                        {
                            UnityEngine.GameObject.Find("RomVersionText").GetComponent<TMPro.TextMeshProUGUI>().text = Singleton<SystemConfig>.Instance.config.displayVersionString;
                            isTitle = true;
                        }
                        Thread.Sleep(100);
                        continue;
                    }
                    var content = queue.TryDequeue(out string c) ? c : Singleton<SystemConfig>.Instance.config.displayVersionString;
                    UnityEngine.GameObject.Find("RomVersionText").GetComponent<TMPro.TextMeshProUGUI>().text = content;
                    isTitle = false;
                    Thread.Sleep(ToastDuration());
                    if (queue.Count == 0)
                    {
                        UnityEngine.GameObject.Find("RomVersionText").GetComponent<TMPro.TextMeshProUGUI>().text = Singleton<SystemConfig>.Instance.config.displayVersionString;
                    }
                }
                catch (Exception _) { }

                Thread.Sleep(100);
            }
        }
        public static int ToastDuration()
        {
            if(queue.Count == 0) return 200;

            var duration = (int)(Math.Pow(0.625, queue.Count - 2) * 200);
            if (duration <= 50) {
                return 50;
            }

            return duration;
        }
    }
}
