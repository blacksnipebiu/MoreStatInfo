using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreStatInfo.Patchs
{
    internal class MoreStatInfoGameLogic
    {
        public static Action OnGameBegin;
        public static Action OnGameEnd;

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(GlobalObject), "OnApplicationQuit")]
        //public static void GameLoaderCreateLoaderPrefix()
        //{
        //    OnGameEnd?.Invoke();
        //    Debug.Log("MoreStatInfo End");
        //}

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "Import")]
        public static void GameLoaderCreateLoaderPostfix()
        {
            OnGameBegin?.Invoke();
            Console.WriteLine("MoreStatInfo Start1");
        }
    }
}
