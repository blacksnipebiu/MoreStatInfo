using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MoreStatInfo.Model;
using MoreStatInfo.Patchs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using AllStatInfo = MoreStatInfo.Model.AllStatInfo;
using Debug = UnityEngine.Debug;

namespace MoreStatInfo
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class MoreStatInfo : BaseUnityPlugin
    {
        public const string GUID = "cn.blacksnipe.dsp.MoreStatInfo";
        public const string NAME = "MoreStatInfo";
        public const string VERSION = "2.0.1";
        public static Harmony harmony;
        public static AllStatInfo allstatinfo;
        public static bool IsEnglish;
        public static bool OneSecondElapsed;
        public static ConfigEntry<int> scale;
        private static GUIDraw guiDraw;
        private static ConfigEntry<KeyboardShortcut> QuickKey;
        private bool firstStart = true;
        private bool IsFirstMainMenu;
        public static bool GameLoaded;

        IEnumerator OneSecondCheck()
        {
            while (true)
            {
                OneSecondElapsed = true;
                yield return new WaitForSeconds(1f);
            }
        }

        void OnGUI()
        {
            if (!IsFirstMainMenu)
            {
                return;
            }
            guiDraw.Draw();
        }

        void Start()
        {
            MoreStatInfoGameLogic.OnGameBegin += () =>
            {
                GameLoaded = true;
                Console.WriteLine("GameLoaded" + GameLoaded);
            };
            MoreStatInfoGameLogic.OnGameEnd += () =>
            {
                GameLoaded = false;
                Console.WriteLine("GameLoaded" + GameLoaded);
            };
            harmony = new Harmony(GUID);
            harmony.PatchAll(typeof(MoreStatInfoGameLogic));
            scale = Config.Bind("大小适配", "scale", 16);
            QuickKey = Config.Bind("打开窗口快捷键", "Key", new KeyboardShortcut(KeyCode.Alpha3, KeyCode.LeftAlt));

            MoreStatInfoTranslate.regallTranslate();
            Debug.Log("MoreStatInfo Start");
            StartCoroutine(OneSecondCheck());
        }

        void Update()
        {
            if (!GameMain.instance)
                return;
            if (!IsFirstMainMenu && UIRoot.instance?.uiMainMenu?.active == true && ItemProto.itemIds != null)
            {
                IsFirstMainMenu = true;
                guiDraw = new GUIDraw(scale.Value);
                allstatinfo = new AllStatInfo();
                allstatinfo.Init();
            }
            if (!GameMain.instance.running)
            {
                firstStart = false;
                //guiDraw.ShowGUIWindow = false;
            }
            else if (!firstStart)
            {
                firstStart = true;
                if (guiDraw.OnGUIInited)
                {
                    guiDraw.Reset();
                }
                allstatinfo.Collect(GameMain.galaxy);
            }
            else
            {
                if (QuickKey.Value.IsDown())
                {
                    guiDraw.ShowGUIWindow = !guiDraw.ShowGUIWindow;
                }
            }

        }

    }
}