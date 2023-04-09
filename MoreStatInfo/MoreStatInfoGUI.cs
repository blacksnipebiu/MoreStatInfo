using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreStatInfo
{
    internal class MoreStatInfoGUI
    {
        private static GameObject ui_morestatinfopanel;
        public static Texture2D mytexture;
        public static GUIStyle styleblue;
        public static GUIStyle styleyellow;
        public static GUIStyle styleitemname = null;
        public static GUIStyle buttonstyleyellow = null;
        public static GUIStyle buttonstyleblue = null;
        public static int heightdis;
        public static int switchwitdh;
        public static int switchheight;
        private static float temp_window_x = 10;
        private static float temp_window_y = 200;
        private static float window_x_move = 200;
        private static float window_y_move = 200;
        private static float window_x = 200;
        public static float window_y = 200;
        public static int cursortrule;
        public static int cursortcolumnindex;
        public static string[] columnsbuttonstr = new string[11] { "物品", "实时产量", "实时消耗", "理论产量", "需求产量", "生产者", "消费者", "总计", "本地提供", "本地需求", "本地仓储" };
        public static float window_width = 1150;
        public static float window_height = 700;
        private static bool moving;
        private static bool leftscaling;
        private static bool rightscaling;
        private static bool topscaling;
        private static bool bottomscaling;

        private static bool _ShowGUIWindow;
        /// <summary>
        /// 显示面板
        /// </summary>
        public static bool ShowGUIWindow
        {
            get => _ShowGUIWindow;
            set
            {
                _ShowGUIWindow = value;
                if (!MoreStatInfo.CloseUIpanel.Value)
                {
                    ui_morestatinfopanel.SetActive(value);
                }
            }
        }

        public static void Init()
        {
            GameObject morestatinfopanel=null;
            morestatinfopanel = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("MoreStatInfo.morestatinfopanel")).LoadAsset<GameObject>("MoreStatInfoPanel");
            if (morestatinfopanel != null)
            {
                ui_morestatinfopanel = UnityEngine.Object.Instantiate(morestatinfopanel, UIRoot.instance.overlayCanvas.transform);
            }
            MoreStatInfo.CloseUIpanel.SettingChanged += new EventHandler((o, e) =>
            {
                ui_morestatinfopanel.SetActive(!MoreStatInfo.CloseUIpanel.Value);
            });
            mytexture = new Texture2D(10, 10);
            for (int i = 0; i < mytexture.width; i++)
                for (int j = 0; j < mytexture.height; j++)
                    mytexture.SetPixel(i, j, new Color(0, 0, 0, 1));
            mytexture.Apply();

            styleblue = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 20
            };
            styleblue.normal.textColor = new Color32(167, 255, 255, 255);
            styleyellow = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 20
            };
            styleyellow.normal.textColor = new Color32(240, 191, 103, 255);
        }

        public static void MoreStatInfoPanelFun()
        {
            var rt = ui_morestatinfopanel.GetComponent<RectTransform>();
            //1150,700,495,403,79,-791
            //1150,700,495,403,-465,-563
            //1150,700,1268,1003,308,-1163
            rt.sizeDelta = new Vector2(window_width, window_height);
            rt.localPosition = new Vector2(-Screen.width / 2 + window_x, Screen.height / 2 - window_y - window_height);
        }

        /// <summary>
        /// 返回一个Rect，并根据type修改x或y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static Rect AddRect(int x, ref int y, Vector2 Size)
        {
            Rect rect = new Rect(new Vector2(x, y), Size);
            y += (int)Size.y;
            return rect;
        }

        /// <summary>
        /// 返回一个Rect，并根据type修改x或y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static Rect AddRect(ref int x, int y, Vector2 Size)
        {
            Rect rect = new Rect(new Vector2(x, y), Size);
            x += (int)Size.x;
            return rect;
        }

        /// <summary>
        /// 返回一个Rect，并根据引用修改
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect AddRect(ref int x, int y, float width, float height)
        {
            Rect rect = new Rect(x, y, width, height);
            x += (int)width;
            return rect;
        }

        /// <summary>
        /// 返回一个Rect，并根据引用修改
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect AddRect(ref int x, int y, int width, int height)
        {
            Rect rect = new Rect(x, y, width, height);
            x += width;
            return rect;
        }

        /// <summary>
        /// 返回一个Rect，并根据引用修改
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect AddRect(int x,ref int y, int width, int height)
        {
            Rect rect = new Rect(x, y, width, height);
            y += height;
            return rect;
        }

        /// <summary>
        /// 返回一个Rect，并根据引用修改
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect AddRect(ref float x, float y, float width, float height)
        {
            Rect rect = new Rect(x, y, width, height);
            x += width;
            return rect;
        }

        /// <summary>
        /// 返回一个Rect，并根据引用修改
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect AddRect(float x,ref float y,float width,float height)
        {
            Rect rect = new Rect(x,y,width,height);
            y += height;
            return rect;
        }

        public static Rect GetMainWindowRect()
        {
            return new Rect(window_x, window_y, window_width, window_height);
        }

        public static Rect GetSwitchWindowRect()
        {
            return new Rect(window_x - switchwitdh, window_y, switchwitdh, switchheight);
        }

        public static Rect GetPlanetWindowRect()
        {
            return new Rect(window_x + window_width, window_y, heightdis * 8 + 10, heightdis * 21 + 30);
        }

        public static bool IsScalingWindow()
        {
            return leftscaling || rightscaling || topscaling || bottomscaling;
        }

        /// <summary>
        /// 改变排序顺序
        /// </summary>
        /// <param name="columnnum"></param>
        public static void ChangeSort(int columnnum)
        {
            if (cursortrule != 0)
                columnsbuttonstr[cursortcolumnindex] = columnsbuttonstr[cursortcolumnindex].Substring(0, columnsbuttonstr[cursortcolumnindex].Length - 1);
            if (columnnum != cursortcolumnindex)
                cursortrule = 1;
            else
            {
                cursortrule++;
                if (cursortrule > 1) cursortrule = -1;
            }
            cursortcolumnindex = columnnum;

            switch (cursortrule)
            {
                case 1:
                    columnsbuttonstr[columnnum] += "↑"; break;
                case -1:
                    columnsbuttonstr[columnnum] += "↓"; break;
                case 0:
                    cursortcolumnindex = 0; break;
            }
        }

        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="window_x"></param>
        /// <param name="window_y"></param>
        /// <param name="window_x_move"></param>
        /// <param name="window_y_move"></param>
        /// <param name="moving"></param>
        /// <param name="temp_window_x"></param>
        /// <param name="temp_window_y"></param>
        /// <param name="window_width"></param>
        public static void moveWindow()
        {
            Vector2 temp = Input.mousePosition;
            if (temp.x > window_x && temp.x < window_x + window_width && Screen.height - temp.y > window_y && Screen.height - temp.y < window_y + 20)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!moving)
                    {
                        window_x_move = window_x;
                        window_y_move = window_y;
                        temp_window_x = temp.x;
                        temp_window_y = Screen.height - temp.y;
                    }
                    moving = true;
                    window_x = window_x_move + temp.x - temp_window_x;
                    window_y = window_y_move + (Screen.height - temp.y) - temp_window_y;
                }
                else
                {
                    moving = false;
                    temp_window_x = window_x;
                    temp_window_y = window_y;
                }
            }
            else if (moving)
            {
                moving = false;
                window_x = window_x_move + temp.x - temp_window_x;
                window_y = window_y_move + (Screen.height - temp.y) - temp_window_y;
            }
        }

        /// <summary>
        /// 改变窗口大小
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="window_x"></param>
        /// <param name="window_y"></param>
        public static void scaling_window()
        {
            Vector2 temp = Input.mousePosition;
            float x = window_width;
            float y = window_height;
            if (Input.GetMouseButton(0))
            {
                if ((temp.x + 10 > window_x && temp.x - 10 < window_x) && (Screen.height - temp.y >= window_y && Screen.height - temp.y <= window_y + y) || leftscaling)
                {
                    x -= temp.x - window_x;
                    window_x = temp.x;
                    leftscaling = true;
                    rightscaling = false;
                }
                if ((temp.x + 10 > window_x + x && temp.x - 10 < window_x + x) && (Screen.height - temp.y >= window_y && Screen.height - temp.y <= window_y + y) || rightscaling)
                {
                    x += temp.x - window_x - x;
                    rightscaling = true;
                    leftscaling = false;
                }
                if ((Screen.height - temp.y + 10 > y + window_y && Screen.height - temp.y - 10 < y + window_y) && (temp.x >= window_x && temp.x <= window_x + x) || bottomscaling)
                {
                    y += Screen.height - temp.y - (window_y + y);
                    bottomscaling = true;
                }
                if (rightscaling || leftscaling)
                {
                    if ((Screen.height - temp.y + 10 > window_y && Screen.height - temp.y - 10 < window_y) && (temp.x >= window_x && temp.x <= window_x + x) || topscaling)
                    {
                        y -= Screen.height - temp.y - window_y;
                        window_y = Screen.height - temp.y;
                        topscaling = true;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                rightscaling = false;
                leftscaling = false;
                bottomscaling = false;
                topscaling = false;
            }
            window_width = x;
            window_height = y;
        }

    }
}
