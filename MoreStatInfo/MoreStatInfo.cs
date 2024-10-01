using BepInEx;
using BepInEx.Configuration;
using MoreStatInfo.Model;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace MoreStatInfo
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class MoreStatInfo : BaseUnityPlugin
    {
        public const string GUID = "cn.blacksnipe.dsp.MoreStatInfo";
        public const string NAME = "MoreStatInfo";
        public const string VERSION = "1.5.4";
        public static AllStatInfo allstatinfo;
        public static bool IsEnglish;
        public static bool OneSecondElapsed;
        public static ConfigEntry<int> scale;
        private static GUIDraw guiDraw;
        private static ConfigEntry<KeyboardShortcut> QuickKey;
        private bool firstStart = true;
        private bool IsFirstMainMenu;

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
            if (IsFirstMainMenu && guiDraw.ShowGUIWindow)
            {
                guiDraw.Draw();
            }
        }

        void Start()
        {
            scale = Config.Bind("大小适配", "scale", 16);
            QuickKey = Config.Bind("打开窗口快捷键", "Key", new KeyboardShortcut(KeyCode.Alpha3, KeyCode.LeftAlt));

            MoreStatInfoTranslate.regallTranslate();
            Debug.Log("MoreStatInfo Start");
            StartCoroutine(OneSecondCheck());
        }

        void Update()
        {
            if (GameMain.instance == null)
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
                while (PlanetModelingManager.calPlanetReqList.Count > 0)
                {
                    PlanetModelingManager.calPlanetReqList.Dequeue();
                }
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