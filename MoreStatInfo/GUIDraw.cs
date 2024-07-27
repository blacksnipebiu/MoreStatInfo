using System;
using System.Reflection;
using UnityEngine;

namespace MoreStatInfo
{
    internal class GUIDraw
    {
        private bool firstDraw;
        private bool RefreshBaseSize;
        private static GameObject ui_morestatinfopanel;
        public static Texture2D mytexture;
        public static GUIStyle normalPlanetButtonStyle;
        public static GUIStyle selectedPlanetButtonStyle;
        public static GUIStyle stylenormalblue;
        public static GUIStyle styleboldblue;
        public static GUIStyle styleyellow;
        public static GUIStyle styleitemname = null;
        public static GUIStyle buttonstyleyellow = null;
        public static GUIStyle buttonstyleblue = null;
        private int baseSize;
        public static int heightdis;
        public static int switchwitdh;
        public static int switchheight;
        private static float temp_MainWindow_x = 10;
        private static float temp_MainWindow_y = 200;
        private static float MainWindow_x_move = 200;
        private static float MainWindow_y_move = 200;
        private static float MainWindow_x = 200;
        public static float MainWindow_y = 200;
        public static int cursortrule;
        public static int cursortcolumnindex;
        public static string[] columnsbuttonstr = new string[11] { "物品", "实时产量", "实时消耗", "理论产量", "需求产量", "生产者", "消费者", "总计", "本地提供", "本地需求", "本地仓储" };
        public static float MainWindowWidth = 1150;
        public static float MainWindowHeight = 700;
        private static bool moving;
        private static bool leftscaling;
        private static bool rightscaling;
        private static bool bottomscaling;

        private bool _ShowGUIWindow;
        /// <summary>
        /// 显示面板
        /// </summary>
        public bool ShowGUIWindow
        {
            get => _ShowGUIWindow;
            set
            {
                _ShowGUIWindow = value;
                ui_morestatinfopanel.SetActive(value);
                if (value)
                {
                    firstDraw = true;
                }
            }
        }
        public int BaseSize
        {
            get => baseSize;
            set
            {
                baseSize = value;
                MoreStatInfo.scale.Value = value;
                RefreshBaseSize = true;
                heightdis = value * 2;
            }
        }

        public GUIStyle emptyStyle;
        public GUIStyle whiteStyle;

        public GUIDraw()
        {
            Init();
        }

        private void Init()
        {
            var morestatinfopanel = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("MoreStatInfo.morestatinfopanel")).LoadAsset<GameObject>("MoreStatInfoPanel");
            ui_morestatinfopanel = UnityEngine.Object.Instantiate(morestatinfopanel, UIRoot.instance.overlayCanvas.transform);
            mytexture = new Texture2D(10, 10);
            for (int i = 0; i < mytexture.width; i++)
                for (int j = 0; j < mytexture.height; j++)
                    mytexture.SetPixel(i, j, new Color(0, 0, 0, 1));
            mytexture.Apply();

            styleboldblue = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 20
            };
            styleboldblue.normal.textColor = new Color32(167, 255, 255, 255);
            stylenormalblue = new GUIStyle
            {
                fontStyle = FontStyle.Normal,
                fontSize = 20
            };
            stylenormalblue.normal.textColor = new Color32(167, 255, 255, 255);
            styleyellow = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 20
            };
            styleyellow.normal.textColor = new Color32(240, 191, 103, 255);
        }

        public void Draw()
        {
            if (firstDraw)
            {
                firstDraw = false;
                BaseSize = GUI.skin.label.fontSize;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                int t = (int)(Input.GetAxis("Mouse Wheel") * 10);
                int temp = BaseSize + t;
                if (Input.GetKeyDown(KeyCode.UpArrow)) { temp++; }
                if (Input.GetKeyDown(KeyCode.DownArrow)) { temp--; }
                temp = Math.Max(5, Math.Min(temp, 35));
                BaseSize = temp;
            }
            switchwitdh = MoreStatInfo.IsEnglish ? 7 * heightdis : 4 * heightdis;
            switchheight = heightdis * 10;
            if (styleitemname == null)
            {
                emptyStyle = new GUIStyle();
                whiteStyle = new GUIStyle();
                whiteStyle.normal.background = Texture2D.whiteTexture;
                styleitemname = new GUIStyle(GUI.skin.label);
                styleitemname.normal.textColor = Color.white;
                buttonstyleblue = new GUIStyle(GUI.skin.button);
                buttonstyleblue.normal.textColor = styleboldblue.normal.textColor;
                buttonstyleyellow = new GUIStyle(GUI.skin.button);
                buttonstyleyellow.normal.textColor = styleyellow.normal.textColor;
                normalPlanetButtonStyle = new GUIStyle(GUI.skin.button);
                selectedPlanetButtonStyle = new GUIStyle(GUI.skin.button);
                selectedPlanetButtonStyle.normal.textColor = new Color32(215, 186, 245, 255);
            }
            if (RefreshBaseSize)
            {
                RefreshBaseSize = false;
                GUI.skin.label.fontSize = BaseSize;
                GUI.skin.button.fontSize = BaseSize;
                GUI.skin.toggle.fontSize = BaseSize;
                GUI.skin.textField.fontSize = BaseSize;
                GUI.skin.textArea.fontSize = BaseSize;
                buttonstyleblue.fontSize = BaseSize;
                buttonstyleyellow.fontSize = BaseSize;
            }

            UIPanelSet();
        }


        public static void UIPanelSet()
        {
            var rt = ui_morestatinfopanel.GetComponent<RectTransform>();
            var Canvasrt = UIRoot.instance.overlayCanvas.GetComponent<RectTransform>();
            float CanvaswidthMultiple = Canvasrt.sizeDelta.x * 1.0f / Screen.width;
            float CanvasheightMultiple = Canvasrt.sizeDelta.y * 1.0f / Screen.height;
            rt.sizeDelta = new Vector2(CanvaswidthMultiple * MainWindowWidth, CanvasheightMultiple * MainWindowHeight);
            rt.localPosition = new Vector2(-Canvasrt.sizeDelta.x / 2 + MainWindow_x * CanvaswidthMultiple, Canvasrt.sizeDelta.y / 2 - MainWindow_y * CanvasheightMultiple - rt.sizeDelta.y);
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
        public static Rect AddRect(int x, ref int y, int width, int height)
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
        public static Rect AddRect(float x, ref float y, float width, float height)
        {
            Rect rect = new Rect(x, y, width, height);
            y += height;
            return rect;
        }

        public static Rect GetMainWindowRect()
        {
            return new Rect(MainWindow_x, MainWindow_y, MainWindowWidth, MainWindowHeight);
        }

        public static Rect GetSwitchWindowRect()
        {
            return new Rect(MainWindow_x - switchwitdh, MainWindow_y, switchwitdh, switchheight);
        }

        public static Rect GetPlanetWindowRect()
        {
            return new Rect(MainWindow_x + MainWindowWidth, MainWindow_y, heightdis * 8 + 10, heightdis * 21 + 30);
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
        #region 窗口操作
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
        public static void MoveWindow()
        {
            if (leftscaling || rightscaling || bottomscaling) return;
            Vector2 temp = Input.mousePosition;
            if (temp.x > MainWindow_x && temp.x < MainWindow_x + MainWindowWidth && Screen.height - temp.y > MainWindow_y && Screen.height - temp.y < MainWindow_y + 20)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!moving)
                    {
                        MainWindow_x_move = MainWindow_x;
                        MainWindow_y_move = MainWindow_y;
                        temp_MainWindow_x = temp.x;
                        temp_MainWindow_y = Screen.height - temp.y;
                    }
                    moving = true;
                    MainWindow_x = MainWindow_x_move + temp.x - temp_MainWindow_x;
                    MainWindow_y = MainWindow_y_move + (Screen.height - temp.y) - temp_MainWindow_y;
                }
                else
                {
                    moving = false;
                    temp_MainWindow_x = MainWindow_x;
                    temp_MainWindow_y = MainWindow_y;
                }
            }
            else if (moving)
            {
                moving = false;
                MainWindow_x = MainWindow_x_move + temp.x - temp_MainWindow_x;
                MainWindow_y = MainWindow_y_move + (Screen.height - temp.y) - temp_MainWindow_y;
            }
            MainWindow_y = Math.Max(10, Math.Min(Screen.height - 10, MainWindow_y));
            MainWindow_x = Math.Max(10, Math.Min(Screen.width - 10, MainWindow_x));
        }

        /// <summary>
        /// 改变窗口大小
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="window_x"></param>
        /// <param name="window_y"></param>
        public static void Scaling_Window()
        {
            Vector2 temp = Input.mousePosition;
            float x = MainWindowWidth;
            float y = MainWindowHeight;
            if (Input.GetMouseButton(0))
            {
                if ((temp.x + 10 > MainWindow_x && temp.x - 10 < MainWindow_x) && (Screen.height - temp.y >= MainWindow_y && Screen.height - temp.y <= MainWindow_y + y) || leftscaling)
                {
                    x -= temp.x - MainWindow_x;
                    MainWindow_x = temp.x;
                    leftscaling = true;
                    rightscaling = false;
                }
                if ((temp.x + 10 > MainWindow_x + x && temp.x - 10 < MainWindow_x + x) && (Screen.height - temp.y >= MainWindow_y && Screen.height - temp.y <= MainWindow_y + y) || rightscaling)
                {
                    x += temp.x - MainWindow_x - x;
                    rightscaling = true;
                    leftscaling = false;
                }
                if ((Screen.height - temp.y + 10 > y + MainWindow_y && Screen.height - temp.y - 10 < y + MainWindow_y) && (temp.x >= MainWindow_x && temp.x <= MainWindow_x + x) || bottomscaling)
                {
                    y += Screen.height - temp.y - (MainWindow_y + y);
                    bottomscaling = true;
                }
                if (rightscaling || leftscaling)
                {
                    if ((Screen.height - temp.y + 10 > MainWindow_y && Screen.height - temp.y - 10 < MainWindow_y) && (temp.x >= MainWindow_x && temp.x <= MainWindow_x + x))
                    {
                        y -= Screen.height - temp.y - MainWindow_y;
                        MainWindow_y = Screen.height - temp.y;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                rightscaling = false;
                leftscaling = false;
                bottomscaling = false;
            }
            MainWindowWidth = x;
            MainWindowHeight = y;
        }
        #endregion
    }
}
