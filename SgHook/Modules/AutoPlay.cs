#if AUTOPLAY
using DB;
using Fx;
using HarmonyLib;
using Manager;
using Manager.UserDatas;
using MelonLoader;
using Monitor;
using Process;
using System;
using System.Reflection;
using UnityEngine;
using static Manager.GameManager;

namespace SgHook.Modules
{
    public class AutoPlay : ISgHookBase
    {
        private static AutoPlayConfig config;
        public static bool IsAutoPlaying = false;
        public void InitConfig()
        {
            config = Config.Conf.autoPlay;
        }
        public bool IsEnabled()
        {
            return config.enable;
        }
        public void Run()
        {
            {
                var origin = typeof(CommonProcess).GetMethod("OnUpdate", BindingFlags.Instance| BindingFlags.Public);
                var postfix = typeof(AutoPlay).GetMethod("Postfix_OnUpdate");
                SgHook.H0.Patch(origin, postfix: new HarmonyMethod(postfix));
            }
            {
                var origin = typeof(ResultProcess).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.Public);
                var prefix = typeof(AutoPlay).GetMethod("Prefix_FixResult");
                SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
            }
            {
                var origin = typeof(GameManager).GetMethod("AutoJudge");
                var prefix = typeof(AutoPlay).GetMethod("Prefix_AutoJudge");
                SgHook.H0.Patch(origin, prefix: new HarmonyMethod(prefix));
            }
        }

        public static void Prefix_FixResult()
        {

            GameManager.AutoPlay = GameManager.AutoPlayMode.None;
            if (!IsAutoPlaying) return;

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    var score = MAI2.Util.Singleton<Manager.GamePlayManager>.Instance.GetGameScore(i);
                    uint total = score.Fast + score.Late;
                    uint fixedFast = total * config.fastRate / 100;
                    uint fixedLate = total - fixedFast;

                    Toast.Create($"Fixed: Fast {fixedFast} / Late {fixedLate}");

                    score.GetType().GetProperty("Fast", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(score, fixedFast, null);
                    score.GetType().GetProperty("Late", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(score, fixedLate, null);
                    
                    uint[,] _resultList = (uint[,])score.GetType().GetField("_resultList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(score);
                    var userOption = score.UserOption;
                    
                    if (userOption.DispJudge.IsCritical()) {
                        var slide = _resultList[2, 5] + _resultList[2, 6] + _resultList[2, 8] + _resultList[2, 9] + _resultList[2, 7];
                        _resultList[2, 5] = 0;
                        _resultList[2, 6] = 0;
                        _resultList[2, 8] = 0;
                        _resultList[2, 9] = 0;
                        _resultList[2, 7] = slide;
                        Toast.Create($"Critical Slide: {slide}");
                        var touch = _resultList[4,5] + _resultList[4,6] + _resultList[4,8] + _resultList[4,9] + _resultList[4,7];
                        _resultList[4,5] = 0;
                        _resultList[4,6] = 0;
                        _resultList[4,8] = 0;
                        _resultList[4,9] = 0;
                        _resultList[4,7] = touch;
                        Toast.Create($"Critical Touch: {touch}");
                    }

                    score.GetType().GetField("_resultList", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(score, _resultList);

                } catch(Exception _) { 
                    MelonLogger.Msg($"Player: {i} not found.");
                }
            }
        }
        public static bool Prefix_AutoJudge(ref NoteJudge.ETiming __result)
        {
            if(GameManager.AutoPlay == AutoPlayMode.Perfect)
            {
                var random = UnityEngine.Random.Range(1, 99);
                if (random <= config.perfect2ndRate)
                {
                    __result = NoteJudge.ETiming.LatePerfect2nd;
                } else
                {
                    __result = NoteJudge.ETiming.LatePerfect;
                }
                return false;
            }
            return true;
        }
        public static void Postfix_OnUpdate()
        {

            if (config.switchKey!="")
            {
                KeyCode key = (KeyCode)Enum.Parse(typeof(KeyCode), config.switchKey);
                if (DebugInput.GetKeyDown(key))
                {
                    if (IsAutoPlaying)
                    {
                        IsAutoPlaying = false;
                        GameManager.AutoPlay = GameManager.AutoPlayMode.None;
                        Toast.Create("AutoPlay: OFF");
                    } else {
                        IsAutoPlaying = true;
                        Toast.Create("AutoPlay: ON");
                    }
                }
            }
            else
            {
                IsAutoPlaying = true;
            }
            if(!IsAutoPlaying)
            {
                return;
            }
            IsAutoPlaying = true;
            var random = UnityEngine.Random.Range(1, 99);
            var randomGreat = UnityEngine.Random.Range(1, 99);
            var randomGood = UnityEngine.Random.Range(1, 99);

            var criticalRandom = UnityEngine.Random.Range(1, 99);
            if (0 < random && random < config.perfectRate)
            {
                if (0 < criticalRandom && criticalRandom < config.criticalRate)
                {

                    GameManager.AutoPlay = GameManager.AutoPlayMode.Critical;
                }
                else
                {
                    GameManager.AutoPlay = GameManager.AutoPlayMode.Perfect;
                }
                return;
            } else if(0 < randomGreat && randomGreat < config.greatRate)
            {
                GameManager.AutoPlay = GameManager.AutoPlayMode.Great;
                return;
            } else if(0 < randomGood && randomGood < config.goodRate)
            {

                GameManager.AutoPlay = GameManager.AutoPlayMode.Good;
                return;
            }
            GameManager.AutoPlay = GameManager.AutoPlayMode.None;
        }
    }
}
#endif