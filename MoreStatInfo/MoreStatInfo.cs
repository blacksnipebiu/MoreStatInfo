using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MoreStatInfo
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class MoreStatInfo : BaseUnityPlugin
    {
        public const string GUID = "cn.blacksnipe.dsp.MoreStatInfo";
        public const string NAME = "MoreStatInfo";
        public const string VERSION = "1.3.4";
        private const string GAME_PROCESS = "DSPGAME.exe";
        private int unloadPlanetNum = 0;
        private bool[] searchcondition_bool = new bool[40];
        private string searchcondition_TypeName = "";
        private int pointPlanetId = 0;
        private int producelowerlimit = 0;
        private string producelowerlimitstr = "0";
        private int comsumelowerlimit = 0;
        private string comsumelowerlimitstr = "0";
        private int theoryproducelowerlimit = 0;
        private string theoryproducelowerlimitstr = "0";
        private int theorycomsumelowerlimit = 0;
        private string theorycomsumelowerlimitstr = "0";
        private int maxheight;
        private int windowmaxheight = 630;
        private int selectitemId = 0;
        private static Dictionary<int, bool> Productsearchcondition = new Dictionary<int, bool>();
        private bool dropdownbutton = false;
        private bool firstStart = true;
        private bool moving = false;
        private bool leftscaling = false;
        private bool rightscaling = false;
        private bool topscaling = false;
        private bool bottomscaling = false;
        private bool showwindow = false;
        private bool StopRefresh = false;
        private bool firstopen = true;

        private bool producelimittoggle = false;
        private bool comsumeimittoggle = false;
        private bool theoryproducelimittoggle = false;
        private bool theorycomsumelimittoggle = false;
        private bool sortbypointproduct = false;
        private bool rawmaterial = true;
        private bool secondrawmaterial = true;
        private bool building = true;
        private bool compound = true;
        private bool onlyseechose = false;
        private bool noreachtheoryproduce = false;
        private bool noreachneedproduce = false;
        private bool refreshfactoryinfo = false;
        private bool theorynoreachneedproduce = false;


        private Vector2 scrollPosition;
        private Vector2 pdselectscrollPosition;
        private GUIStyle styleblue=new GUIStyle();
        private GUIStyle styleyellow=new GUIStyle();
        private GUIStyle styleitemname=null;
        private GUIStyle buttonstyleyellow=null;
        private GUIStyle buttonstyleblue=null;
        private float curtime;
        private float itemrefreshlasttime;
        private float planetrefreshlasttime;
        private float window_x_move = 200;
        private float window_y_move = 200;
        private bool changescale = false;
        private float temp_window_x = 10;
        private float temp_window_y = 200;
        private float window_x = 200;
        private float window_y = 200;
        private float window_width = 1150;
        private float window_height = 700;
        private List<int> iteminfoshow = new List<int>();
        private List<int> planetinfoshow = new List<int>();
        private List<int> planetsproductinfoshow = new List<int>();
        private List<string> PlanetType=new List<string>();
        private static List<ItemProto> ItemList = new List<ItemProto>();
        private static Dictionary<int, long[]> SumProduce = new Dictionary<int, long[]>();
        private Dictionary<int, int> factoryinfoshow = new Dictionary<int, int>();
        private long[] sumpowerinfoshow = new long[5];
        private Dictionary<int, long[]> powerenergyinfoshow = new Dictionary<int, long[]>();
        private Dictionary<int, long[]> multPlanetProduce = new Dictionary<int, long[]>();
        private static Dictionary<int, Dictionary<int, long[]>> PlanetProduce = new Dictionary<int, Dictionary<int, long[]>>();
        private static Dictionary<int, Dictionary<int, int>> productIndices = new Dictionary<int, Dictionary<int, int>>();
        private static Dictionary<int, Dictionary<int, float>> PlanetRequireDiction = new Dictionary<int, Dictionary<int, float>>();
        private static Dictionary<int, Dictionary<int, float>> PlanetProductDiction = new Dictionary<int, Dictionary<int, float>>();
        private static Dictionary<int, Dictionary<int, long>> PlanetComsumerDiction = new Dictionary<int, Dictionary<int, long>>();
        private static Dictionary<int, Dictionary<int, long>> PlanetProducerDiction = new Dictionary<int, Dictionary<int, long>>();
        private int stationinfoindex = 1;
        private int pdselectinfoindex = 1;
        private bool multplanetproduct = false;
        private bool refreshPlanetinfo = false;
        private bool RemoteorLocal = false;
        private bool PlanetorSum = false;
        private bool TGMKinttostringMode = true;
        private bool temp = false;
        private Texture2D mytexture;
        private bool filtercondition = false;
        private int cursortcolumnindex =0;
        private int cursortrule = 0;
        private KeyboardShortcut tempShowWindow;
        private bool ChangeQuickKey = false;
        private bool ChangingQuickKey = false;
        private static ConfigEntry<int> scale;
        private static ConfigEntry<KeyboardShortcut> ShowCounter1;
        private static ConfigEntry<Boolean> CloseUIpanel;
        private string[] columnsbuttonstr = new string[11] {"物品", "实时产量", "实时消耗", "理论产量", "需求产量", "生产者", "消费者", "总计", "本地提供", "本地需求", "本地仓储"};
        private GameObject morestatinfopanel;
        private GameObject ui_morestatinfopanel;
        void Start()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("MoreStatInfo.morestatinfopanel"));
            //AssetBundle assetBundle = AssetBundle.LoadFromFile("E:/game/game1/New Unity Project (4)/AssetBundles/StandaloneWindows64/panel");
            morestatinfopanel = assetBundle.LoadAsset<GameObject>("MoreStatInfoPanel");
            scale = Config.Bind("大小适配", "scale", 16);
            ShowCounter1 = Config.Bind("打开窗口快捷键", "Key", new BepInEx.Configuration.KeyboardShortcut(KeyCode.Alpha3, KeyCode.LeftAlt));
            CloseUIpanel = Config.Bind("关闭白色面板","closePanel" ,true);
            tempShowWindow = ShowCounter1.Value;
            itemrefreshlasttime = Time.time;
            planetrefreshlasttime = Time.time;
            maxheight = Screen.height;
            scrollPosition[0] = 0;
            pdselectscrollPosition[0] = 0;
            mytexture = new Texture2D(10, 10);
            for (int i = 0; i < mytexture.width; i++)
                for (int j = 0; j < mytexture.height; j++)
                    mytexture.SetPixel(i, j, new Color(0, 0, 0, 1));
            mytexture.Apply();
            
            styleblue.fontStyle = FontStyle.Bold;
            styleblue.fontSize = 20;
            styleblue.normal.textColor = new Color32(167, 255, 255, 255);
            styleyellow.fontStyle = FontStyle.Bold;
            styleyellow.fontSize = 20;
            styleyellow.normal.textColor = new Color32(240, 191, 103, 255);
            for (int i = 1; i < searchcondition_bool.Length; i++)
                searchcondition_bool[i] = false;
            MoreStatInfoTranslate.regallTranslate();
        }
        

        void Update()
        {
            ChangeQuickKeyMethod();
            curtime = Time.time;
            if (Input.GetKey(KeyCode.F9)&&Input.GetKeyDown(KeyCode.LeftShift))
            {
                temp = !temp;
                //GameSave.LoadCurrentGameInResource(18);
            }
            if (GameMain.instance != null)
            {
                if (!GameMain.instance.running)
                {
                    firstStart = false;
                    ItemList = new List<ItemProto>(LDB.items.dataArray);
                    while(PlanetModelingManager.calPlanetReqList.Count > 0)
                    {
                        PlanetModelingManager.calPlanetReqList.Dequeue();
                    }
                }
                if (GameMain.instance.running && !firstStart)
                {
                    Productsearchcondition = new Dictionary<int, bool>();
                    for (int i = 0; i < ItemList.Count; i++)
                        Productsearchcondition.Add(ItemList[i].ID, false);
                    foreach(StarData sd in GameMain.galaxy.stars)
                    {
                        foreach(PlanetData pd in sd.planets)
                        {
                            if (PlanetType.Contains(pd.typeString)) continue;
                            PlanetType.Add(pd.typeString);
                        }
                    }
                    //for (int i = 0; i < ItemList.Count; i++)
                    //{
                    //    ItemProto item = ItemList[i];
                    //    Debug.Log(item.name + " " + item.ID);
                    //}
                    firstStart = true;
                }
            }
            if (ShowCounter1.Value.IsDown() && !ChangingQuickKey)
            {
                showwindow = !showwindow;
                if (ui_morestatinfopanel == null)
                {
                    ui_morestatinfopanel = UnityEngine.Object.Instantiate<GameObject>(morestatinfopanel, UIRoot.instance.overlayCanvas.transform);
                }
                ui_morestatinfopanel.SetActive(showwindow && !CloseUIpanel.Value);
            }

            if (showwindow && Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) { scale.Value++; changescale = true; }
                if (Input.GetKeyDown(KeyCode.DownArrow)) { scale.Value--; changescale = true; }
                if (scale.Value < 5) scale.Value = 5;
                if (scale.Value > 35) scale.Value = 35;
            }
        }

        private void ChangeQuickKeyMethod()
        {
            if (ChangeQuickKey)
            {
                setQuickKey();
                ChangingQuickKey = true;
            }
            else if (!ChangeQuickKey && ChangingQuickKey)
            {
                ShowCounter1.Value = tempShowWindow;
                ChangingQuickKey = false;
            }
        }

        private void setQuickKey()
        {
            bool left = true;
            int[] result = new int[2];
            int num = 0;
            if (Input.GetKey(KeyCode.LeftShift) && left)
            {
                left = false;
                result[0] = 304;
            }
            if (Input.GetKey(KeyCode.LeftControl) && left)
            {
                left = false;
                result[0] = 306;
            }
            if (Input.GetKey(KeyCode.LeftAlt) && left)
            {
                left = false;
                result[0] = 308;
            }
            bool right = true;
            for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9 && right; i++)
            {
                if (Input.GetKey((KeyCode)i))
                {
                    result[1] = i;
                    right = false;
                    break;
                }
            }
            for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z && right; i++)
            {
                if (Input.GetKey((KeyCode)i))
                {
                    result[1] = i;
                    right = false;
                    break;
                }
            }
            for (int i = (int)KeyCode.F1; i <= (int)KeyCode.F10 && right; i++)
            {
                if (Input.GetKey((KeyCode)i))
                {
                    result[1] = i;
                    right = false;
                    break;
                }
            }
            if (left && right) num = 0;
            else if (!left && !right) num = 2;
            else num = 1;
            if (num == 2)
            {
                tempShowWindow = new KeyboardShortcut((KeyCode)result[1], (KeyCode)result[0]);
            }
            else if (num == 1)
            {
                int keynum = Math.Max(result[0], result[1]);
                tempShowWindow = new KeyboardShortcut((KeyCode)keynum);
            }
        }

        private void OnGUI()
        {
            int heightdis = GUI.skin.toggle.fontSize*2;
            if (styleitemname == null)
            {
                styleitemname = new GUIStyle(GUI.skin.label);
                styleitemname.normal.textColor = Color.white;
                buttonstyleblue = new GUIStyle(GUI.skin.button);
                buttonstyleblue.normal.textColor = styleblue.normal.textColor;
                buttonstyleyellow = new GUIStyle(GUI.skin.button);
                buttonstyleyellow.normal.textColor = styleyellow.normal.textColor;
            }

            if (changescale || firstopen)
            {
                changescale = false;
                firstopen = false;
                GUI.skin.label.fontSize = scale.Value;
                GUI.skin.button.fontSize = scale.Value;
                GUI.skin.toggle.fontSize = scale.Value;
                GUI.skin.textField.fontSize = scale.Value;
                GUI.skin.textArea.fontSize = scale.Value;
            }
            else if (!changescale && GUI.skin.toggle.fontSize != scale.Value)
            {
                scale.Value = GUI.skin.toggle.fontSize;
            }
            if (showwindow)
            {
                var rt = ui_morestatinfopanel.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(window_width, window_height);
                rt.localPosition = new Vector2(-Screen.width / 2 + window_x, Screen.height / 2 - window_y - window_height);
                if (curtime - itemrefreshlasttime > 1&&!StopRefresh)
                {
                    itemrefreshlasttime = curtime;
                    RefreshProductStat();
                    List<int> temp;
                    Dictionary<int, long[]> tempDiction=SumProduce;
                    if (PlanetorSum && PlanetProduce != null&& PlanetProduce.Count>0)
                    {
                        if (multplanetproduct)
                        {
                            multPlanetProduce = new Dictionary<int, long[]>();
                            if (planetsproductinfoshow.Count > 0)
                            {
                                for(int i = 0; i < planetsproductinfoshow.Count; i++)
                                {
                                    if (!PlanetProduce.ContainsKey(planetsproductinfoshow[i])) continue;
                                    Dictionary<int, long[]> tempDiction1 = PlanetProduce[planetsproductinfoshow[i]];
                                    foreach(KeyValuePair<int,long[]> wap in tempDiction1)
                                    {
                                        if (multPlanetProduce.ContainsKey(wap.Key))
                                        {
                                            for(int j = 0; j < 10; j++)
                                            {
                                                multPlanetProduce[wap.Key][j] += wap.Value[j];
                                            }
                                        }
                                        else
                                        {
                                            multPlanetProduce.Add(wap.Key, wap.Value);
                                        }
                                    }
                                }
                            }
                            multPlanetProduce = sortItem(multPlanetProduce, cursortcolumnindex, cursortrule);
                            temp = new List<int>(multPlanetProduce.Keys);
                            tempDiction = multPlanetProduce;
                        }
                        else if(PlanetProduce.ContainsKey(pointPlanetId))
                        {
                            PlanetProduce[pointPlanetId] = sortItem(PlanetProduce[pointPlanetId], cursortcolumnindex, cursortrule);
                            temp = new List<int>(PlanetProduce[pointPlanetId].Keys);
                            tempDiction = PlanetProduce[pointPlanetId];
                        }
                    }
                    tempDiction=sortItem(tempDiction, cursortcolumnindex, cursortrule);
                    temp = new List<int>(tempDiction.Keys);
                    iteminfoshow = new List<int>();
                    for (int i = 0; i < temp.Count; i++)
                    {
                        int itemId = temp[i];
                        ItemProto item = LDB.items.Select(itemId);
                        if (LDB.items.Select(itemId) == null) continue;

                        if ( itemId == 1030 || itemId == 1031) continue;
                        if ((itemId >= 1000 && itemId < 1030 || itemId == 1208) && !rawmaterial) continue;
                        if ((itemId > 1100 && itemId < 1200) && !secondrawmaterial) continue;
                        if (item.CanBuild && !building) continue;
                        if (item.recipes.Count > 0 && (itemId != 1003 && !(itemId > 1100 && itemId < 1200)) && item.BuildIndex == 0 && !compound) continue;
                        if (onlyseechose && !Productsearchcondition[itemId]) continue;
                        if (noreachtheoryproduce && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][0] >= tempDiction[itemId][2])) continue;
                        if (noreachneedproduce &&(!tempDiction.ContainsKey(itemId)|| tempDiction[itemId][0] >= tempDiction[itemId][3])) continue;
                        if (theorynoreachneedproduce && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][2] >= tempDiction[itemId][3])) continue;
                        if (producelimittoggle && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][0] <= producelowerlimit)) continue;
                        if (comsumeimittoggle && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][1] <= comsumelowerlimit)) continue;
                        if (theoryproducelimittoggle && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][2] <= theoryproducelowerlimit)) continue;
                        if (theorycomsumelimittoggle && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][3] <= theorycomsumelowerlimit)) continue;
                        iteminfoshow.Add(itemId);
                    }
                }
                Rect window = new Rect(window_x, window_y, window_width, window_height);
                Rect switchwindow = new Rect(window_x- (Localization.language != Language.zhCN ? 6 * heightdis : 3 * heightdis), window_y, Localization.language != Language.zhCN ? 6 * heightdis : 3 * heightdis, 400);
                if (leftscaling || rightscaling || topscaling || bottomscaling) { }
                else
                    moveWindow_xl_first(ref window_x, ref window_y, ref window_x_move, ref window_y_move, ref moving, ref temp_window_x, ref temp_window_y, window_width);
                scaling_window(window_width, window_height, ref window_x, ref window_y);
                window = GUI.Window(20210821, window, DoMyWindow1, "统计面板".getTranslate() + "(" + VERSION + ")" + "ps:ctrl+↑↓");
                switchwindow = GUI.Window(202108212, switchwindow, DoMyWindow2,"");
                if (PlanetorSum||refreshPlanetinfo)
                {
                    Rect pdselectpannel = new Rect(window_x + window_width, window_y, heightdis*8+10, heightdis*21+30);
                    pdselectpannel = GUI.Window(202108213, pdselectpannel, DoMyWindow3, "");
                    GUI.DrawTexture(pdselectpannel, mytexture);
                }
                GUI.DrawTexture(window, mytexture);
                GUI.DrawTexture(switchwindow, mytexture);
            }
        }
        public void DoMyWindow1(int winId)
        {
            int heightdis = GUI.skin.toggle.fontSize*2;
            int togglesize = GUI.skin.toggle.fontSize ;
            bool english = Localization.language != Language.zhCN ? true : false;
            styleblue.fontSize=togglesize;
            styleyellow.fontSize=togglesize;
            styleitemname.fontSize = togglesize;
            GUILayout.BeginArea(new Rect(10, 20, window_width, window_height));
            int tempheight = 0;
            int tempwidth = 0;



            scrollPosition = GUI.BeginScrollView(new Rect(0, 0, window_width - 20, window_height - 30), scrollPosition, new Rect(0, 0, (TGMKinttostringMode ? 45*heightdis:42*heightdis) + (english?4*heightdis:0), windowmaxheight), true, true);

            if (refreshfactoryinfo && factoryinfoshow!=null && factoryinfoshow.Count>0)
            {
                foreach (KeyValuePair<int, int> wap in factoryinfoshow)
                {
                    ItemProto item = LDB.items.Select(wap.Key);
                    GUI.Button(new Rect(120 * tempwidth, 20 + 100 * tempheight, 50, 50), item.iconSprite.texture, new GUIStyle());
                    GUI.Label(new Rect(120 * tempwidth++, 70 + 100 * tempheight, 80, 50), wap.Value + "", styleblue);
                    if (120 * (tempwidth +1) > window_width)
                    {
                        tempheight++;
                        tempwidth = 0;
                    }
                }
                tempheight++;
                if ((refreshPlanetinfo||PlanetorSum) && pointPlanetId>0 && powerenergyinfoshow.ContainsKey(pointPlanetId))
                {
                    GUI.Label(new Rect(10, 70 + 100 * tempheight, 200, 50), "发电性能".getTranslate()+":" + (TGMKinttostringMode ? TGMKinttostring(powerenergyinfoshow[pointPlanetId][0], "W") : Threeinttostring(powerenergyinfoshow[pointPlanetId][0]) + "W"), styleblue);
                    GUI.Label(new Rect(10, 105 + 100 * tempheight, 200, 50), "耗电需求".getTranslate() + ":" + (TGMKinttostringMode ? TGMKinttostring(powerenergyinfoshow[pointPlanetId][1], "W") : Threeinttostring(powerenergyinfoshow[pointPlanetId][1]) + "W"), styleblue);
                    GUI.Label(new Rect(10, 140 + 100 * tempheight, 200, 50), "总耗能".getTranslate() + ":" + (TGMKinttostringMode ? TGMKinttostring(powerenergyinfoshow[pointPlanetId][2], "J") : Threeinttostring(powerenergyinfoshow[pointPlanetId][2]) + "J"), styleblue);
                    GUI.Label(new Rect(210, 105 + 100 * tempheight, 200, 50), "放电功率".getTranslate() + ":" + (TGMKinttostringMode ? TGMKinttostring(powerenergyinfoshow[pointPlanetId][3], "W") : Threeinttostring(powerenergyinfoshow[pointPlanetId][3]) + "W"), styleblue);
                    GUI.Label(new Rect(210, 140 + 100 * tempheight, 200, 50), "充电功率".getTranslate() + ":" + (TGMKinttostringMode ? TGMKinttostring(powerenergyinfoshow[pointPlanetId][4], "W") : Threeinttostring(powerenergyinfoshow[pointPlanetId][4]) + "W"), styleblue);

                }
                else
                {
                    GUI.Label(new Rect(10, 70 + 100 * tempheight, 200, 50), "发电性能".getTranslate() + ":" + (TGMKinttostringMode?TGMKinttostring(sumpowerinfoshow[0],"W"): Threeinttostring(sumpowerinfoshow[0])+"W"), styleblue);
                    GUI.Label(new Rect(10, 105 + 100 * tempheight, 200, 50), "耗电需求".getTranslate() + ":" + (TGMKinttostringMode ? TGMKinttostring(sumpowerinfoshow[1], "W") : Threeinttostring(sumpowerinfoshow[1]) + "W"), styleblue);
                    GUI.Label(new Rect(10, 140 + 100 * tempheight, 200, 50), "总耗能".getTranslate() + ":" + (TGMKinttostringMode ? TGMKinttostring(sumpowerinfoshow[2], "J") : Threeinttostring(sumpowerinfoshow[2]) + "J"), styleblue);
                    GUI.Label(new Rect(210, 70 + 100 * tempheight, 200, 50), "放电功率".getTranslate() + ":" + (TGMKinttostringMode ? TGMKinttostring(sumpowerinfoshow[3], "W") : Threeinttostring(sumpowerinfoshow[3]) + "W"), styleblue);
                    GUI.Label(new Rect(210, 105 + 100 * tempheight, 200, 50), "充电功率".getTranslate() + ":" + (TGMKinttostringMode ? TGMKinttostring(sumpowerinfoshow[4], "W") : Threeinttostring(sumpowerinfoshow[4]) + "W"), styleblue);
                }
                
            }
            else if (refreshPlanetinfo)
            {
                GUILayout.BeginArea(new Rect(0, 10, (TGMKinttostringMode ? 45 * heightdis : 40 * heightdis) + (english ? 8 * heightdis : 0), heightdis * 30));
                int xpos = 0;
                int ypos = 0;
                {
                    int tempwidth1 = english ? heightdis * 8 : heightdis * 4;
                    GUI.Label(new Rect(0, 0, heightdis*8, heightdis), "附属气态星产物".getTranslate());
                    for (int i = 15; i <= 17; i++)
                        searchcondition_bool[i] = GUI.Toggle(new Rect(xpos, heightdis * (i - 14), tempwidth1, heightdis), searchcondition_bool[i], searchchineseTranslate(i));
                    GUI.Label(new Rect(0, heightdis * 4, heightdis * 8, heightdis), "星球特殊性".getTranslate());
                    for (int i = 18; i <= 25; i++)
                        searchcondition_bool[i] = GUI.Toggle(new Rect(xpos, heightdis * (i - 13), tempwidth1, heightdis), searchcondition_bool[i], searchchineseTranslate(i));
                    if(searchcondition_bool[26] != GUI.Toggle(new Rect(xpos, heightdis*13, tempwidth1, heightdis), searchcondition_bool[26], "具有工厂".getTranslate()))
                    {
                        searchcondition_bool[26] = !searchcondition_bool[26];
                        if (searchcondition_bool[26])
                            searchcondition_bool[27] = false;
                    }
                    if (searchcondition_bool[27] != GUI.Toggle(new Rect(xpos, heightdis * 14, tempwidth1, heightdis), searchcondition_bool[27], "不具有工厂".getTranslate()))
                    {
                        searchcondition_bool[27] = !searchcondition_bool[27];
                        if (searchcondition_bool[27])
                            searchcondition_bool[26] = false;
                    }
                    if (searchcondition_bool[28] != GUI.Toggle(new Rect(xpos, heightdis * 15, tempwidth1, heightdis), searchcondition_bool[28], "电力不足".getTranslate()))
                    {
                        searchcondition_bool[28] = !searchcondition_bool[28];
                        if (searchcondition_bool[28])
                            searchcondition_bool[26] = true;
                    }
                    if (searchcondition_bool[29] != GUI.Toggle(new Rect(xpos, heightdis * 16, tempwidth1, heightdis), searchcondition_bool[29], "星球已加载".getTranslate()))
                    {
                        searchcondition_bool[29] = !searchcondition_bool[29];
                        if (searchcondition_bool[29])
                            searchcondition_bool[30] = false;
                    }
                    if (searchcondition_bool[30] != GUI.Toggle(new Rect(xpos, heightdis * 17, tempwidth1, heightdis), searchcondition_bool[30], "星球未加载".getTranslate()))
                    {
                        searchcondition_bool[30] = !searchcondition_bool[30];
                        if (searchcondition_bool[30])
                            searchcondition_bool[29] = false;
                    }
                    xpos += tempwidth1;
                    GUI.Label(new Rect(xpos, 0, heightdis*4, heightdis), "目标星球矿物".getTranslate());
                    for (int i = 1; i <= 14; i++)
                        searchcondition_bool[i] = GUI.Toggle(new Rect(xpos, heightdis * i, xpos, heightdis), searchcondition_bool[i], searchchineseTranslate(i));
                    GUI.Label(new Rect(xpos, heightdis * 15, heightdis * 4, heightdis), "未加载星球".getTranslate() + $"{unloadPlanetNum}");
                    if (GUI.Button(new Rect(xpos, heightdis * 16, xpos, heightdis), "加载全部星球"))
                    {
                        foreach (StarData sd in GameMain.galaxy.stars)
                        {
                            foreach (PlanetData planet in sd.planets)
                            {
                                if (!planet.calculated && !planet.calculating && planet.data == null && !planet.loaded && !planet.loading && planet.factory == null)
                                {
                                    planet.calculating = true;
                                    PlanetModelingManager.calPlanetReqList.Enqueue(planet);
                                }
                            }
                        }
                    }
                    if (PlanetModelingManager.calPlanetReqList.Count > 0)
                    {
                        GUI.Label(new Rect(xpos, heightdis * 17, heightdis * 4, heightdis ), $"{PlanetModelingManager.calPlanetReqList.Count}" + "个星球加载中".getTranslate());
                        GUI.Label(new Rect(xpos, heightdis * 18, heightdis * 4, heightdis ), "请勿切换存档".getTranslate());
                    }
                    xpos += heightdis * 4;
                    GUI.Label(new Rect(xpos, 0, heightdis * 4, heightdis), "星球类型".getTranslate());
                    if (GUI.Button(new Rect(xpos, heightdis, heightdis * 4, heightdis), searchcondition_TypeName==""?"未选择": searchcondition_TypeName))
                    {
                        dropdownbutton = !dropdownbutton;
                        if (dropdownbutton)
                        {
                            window_height = heightdis * (PlanetType.Count + 6);
                        }
                        else
                        {
                            window_height = heightdis * 22;
                        }
                    }
                    if (dropdownbutton)
                    {
                        if (GUI.Button(new Rect(xpos, heightdis * 2, heightdis * 4, heightdis), "取消选择"))
                        {
                            dropdownbutton = !dropdownbutton;
                            searchcondition_TypeName = "";
                        }
                        for (int i = 0; i < PlanetType.Count; i++)
                        {
                            if (GUI.Button(new Rect(xpos, heightdis * (i + 3), heightdis * 4, heightdis), PlanetType[i]))
                            {
                                dropdownbutton = !dropdownbutton;
                                searchcondition_TypeName = PlanetType[i];
                            }
                        }
                    }
                    xpos += heightdis * 4;
                }
                PlanetData pd = null;
                if (pointPlanetId > 0)
                    pd = GameMain.galaxy.PlanetById(pointPlanetId);
                if (pd != null)
                {
                    GUILayout.BeginArea(new Rect(xpos + 10, 30, heightdis * 25, heightdis * 20));
                    {
                        string pdname = GUI.TextArea(new Rect(0, 0, heightdis * 7, heightdis), pd.displayName);
                        if (!pdname.Equals(pd.displayName))
                        {
                            pd.overrideName = pdname;
                            pd.NotifyOnDisplayNameChange();
                        }
                        tempheight = 1;
                        if (pd.gasItems != null)
                        {
                            for (int i = 0; i < pd.gasItems.Length; i++)
                            {
                                ItemProto item = LDB.items.Select(pd.gasItems[i]);
                                GUI.Button(new Rect(0, tempheight * heightdis, heightdis, heightdis), item.iconSprite.texture, new GUIStyle());
                                GUI.Label(new Rect(heightdis, tempheight++ * heightdis, heightdis * 4, heightdis), item.name + ":" + String.Format("{0:N2}", pd.gasSpeeds[i]), styleblue);
                            }
                        }
                        else
                        {
                            long[] veinAmounts = new long[64];
                            pd.CalcVeinAmounts(ref veinAmounts,new HashSet<int>(), UIRoot.instance.uiGame.veinAmountDisplayFilter);
                            int[] planetveinSpotsSketch = veinSpotsSketch(pd);
                            for (int k = 1; planetveinSpotsSketch!=null && k <= 14; k++)
                            {
                                if (planetveinSpotsSketch[k] == 0) continue;
                                long i = veinAmounts[k];
                                int veinid = int.Parse(VeintypechineseTranslate(k, 1));
                                GUI.Button(new Rect(0, tempheight * heightdis, heightdis, heightdis), LDB.items.Select(veinid).iconSprite.texture, new GUIStyle());
                                GUI.Label(new Rect(heightdis, tempheight++ * heightdis, heightdis * 4, heightdis), LDB.items.Select(veinid).name + TGMKinttostring(i) + ("(" + planetveinSpotsSketch[k] + ")"), styleblue);
                            }
                        }
                        GUILayout.BeginArea(new Rect(heightdis * 10, heightdis, heightdis * 15, heightdis * 20));
                        {
                            tempheight = 0;
                            int tempwidth1 = heightdis * 15;
                            if (pd.orbitAroundPlanet != null)
                            {
                                GUI.Label(new Rect(0, tempheight++ * heightdis, tempwidth1, heightdis), "围绕行星".getTranslate()+":", styleblue);
                                pdname = GUI.TextArea(new Rect(0, tempheight++ * heightdis, heightdis * 7, heightdis), pd.orbitAroundPlanet.displayName);
                                if (!pdname.Equals(pd.orbitAroundPlanet.displayName))
                                {
                                    pd.orbitAroundPlanet.overrideName = pdname;
                                    pd.orbitAroundPlanet.NotifyOnDisplayNameChange();
                                }
                                for (int i = 0; i < pd.orbitAroundPlanet.gasItems.Length; i++)
                                {
                                    ItemProto item = LDB.items.Select(pd.orbitAroundPlanet.gasItems[i]);
                                    GUI.Button(new Rect(0, tempheight * heightdis, heightdis, heightdis), item.iconSprite.texture, new GUIStyle());
                                    GUI.Label(new Rect(heightdis, tempheight++ * heightdis, tempwidth1, heightdis), item.name + ":" + String.Format("{0:N2}", pd.orbitAroundPlanet.gasSpeeds[i]), styleblue);
                                }
                            }
                            if (pd.gasItems == null)
                            {

                                string str = "无".getTranslate();
                                int waterItemId = pd.waterItemId;
                                if (waterItemId < 0)
                                {
                                    switch (waterItemId)
                                    {
                                        case -2:
                                            str = "冰".Translate();
                                            break;
                                        case -1:
                                            str = "熔岩".Translate();
                                            break;
                                        default:
                                            str = "未知".Translate();
                                            break;
                                    }
                                }
                                ItemProto wateritem = LDB.items.Select(pd.waterItemId);
                                if (wateritem != null)
                                    str = wateritem.name;
                                tempheight++;
                                GUI.Label(new Rect(0, tempheight++ * heightdis, tempwidth1, heightdis), "行星信息".getTranslate()+":", styleblue);
                                GUI.Label(new Rect(0, tempheight++ * heightdis, tempwidth1, heightdis), "海洋类型".getTranslate() + ":" + str, styleblue);
                                GUI.Label(new Rect(0, tempheight++ * heightdis, tempwidth1, heightdis), "星球类型".getTranslate() + ":" + pd.typeString, styleblue);
                                if (pd.singularityString.Length > 0)
                                    GUI.Label(new Rect(0, tempheight++ * heightdis, tempwidth1, heightdis), "星球特殊性".getTranslate() + ": " + pd.singularityString, styleblue);
                            }
                            GUI.Label(new Rect(0, tempheight++ * heightdis, tempwidth1, heightdis), PlanetProduce.ContainsKey(pd.id) ? "具有工厂".getTranslate() : "不具有工厂".getTranslate(), styleblue);
                            tempheight++;
                            StarData sd = pd.star;

                            GUI.Label(new Rect(0, tempheight++ * heightdis, tempwidth1, heightdis), "恒星信息".getTranslate() + ":", styleblue);
                            string starname = GUI.TextArea(new Rect(0, tempheight++ * heightdis, heightdis * 7, heightdis), sd.displayName);
                            if (!starname.Equals(sd.displayName))
                            {
                                sd.overrideName = starname;
                                sd.NotifyOnDisplayNameChange();
                            }
                            GUI.Label(new Rect(0, tempheight++ * heightdis, tempwidth1, heightdis), sd.typeString, styleblue);
                            GUI.Label(new Rect(0, tempheight++ * heightdis, tempwidth1, heightdis), "恒星亮度".getTranslate() + ":" + sd.dysonLumino + "", styleblue);
                            if (GUI.Button(new Rect(0, tempheight++ * heightdis, heightdis * 7, heightdis), "方向指引".getTranslate()))
                            {
                                GameMain.mainPlayer.navigation._indicatorAstroId = pd.id;
                            }
                        }
                        GUILayout.EndArea();
                    }

                    GUILayout.EndArea();
                }
                GUILayout.EndArea();
                

            }
            else
            {
                if (TGMKinttostringMode)
                {
                    buttonstyleblue.fontSize = GUI.skin.button.fontSize;
                    buttonstyleyellow.fontSize = GUI.skin.button.fontSize;
                    if (GUI.Button(new Rect(heightdis, 0, english ? heightdis * 8 : heightdis * 4, heightdis), columnsbuttonstr[0].getTranslate()))
                        changesort(0);
                    for (int i = 1; i <= 10; i++)
                        if (GUI.Button(new Rect(heightdis + heightdis * 4 * (english?i+1:i), 0,heightdis * 4, heightdis), columnsbuttonstr[i].getTranslate(), i % 2 == 0 ? buttonstyleyellow : buttonstyleblue))
                            changesort(i);
                    tempheight++;
                }
                else
                {
                    buttonstyleblue.fontSize = GUI.skin.label.fontSize;
                    buttonstyleyellow.fontSize = GUI.skin.label.fontSize;
                    tempwidth = heightdis;
                    if (GUI.Button(new Rect(tempwidth, 0, english? heightdis * 8:heightdis *4, heightdis), columnsbuttonstr[0].getTranslate())) changesort(0);
                    tempwidth += english ? heightdis * 8 : heightdis * 4;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis*4, heightdis), columnsbuttonstr[1].getTranslate(), buttonstyleblue)) changesort(1);
                    tempwidth += heightdis * 4;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis * 4, heightdis), columnsbuttonstr[2].getTranslate(), buttonstyleyellow)) changesort(2);
                    tempwidth += heightdis * 4;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis * 4, heightdis), columnsbuttonstr[3].getTranslate(), buttonstyleblue)) changesort(3);
                    tempwidth += heightdis * 4;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis * 4, heightdis), columnsbuttonstr[4].getTranslate(), buttonstyleyellow)) changesort(4);
                    tempwidth += heightdis * 4;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis * 2,  heightdis), columnsbuttonstr[5].getTranslate(), buttonstyleblue)) changesort(5);
                    tempwidth += heightdis * 2;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis * 2,  heightdis), columnsbuttonstr[6].getTranslate(), buttonstyleyellow)) changesort(6);
                    tempwidth += heightdis * 2;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis * 4, heightdis), columnsbuttonstr[7].getTranslate(), buttonstyleblue)) changesort(7);
                    tempwidth += heightdis * 4;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis * 4, heightdis), columnsbuttonstr[8].getTranslate(), buttonstyleyellow)) changesort(8);
                    tempwidth += heightdis * 4;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis * 4, heightdis), columnsbuttonstr[9].getTranslate(), buttonstyleblue)) changesort(9);
                    tempwidth += heightdis * 4;
                    if (GUI.Button(new Rect(tempwidth, 0, heightdis * 4, heightdis), columnsbuttonstr[10].getTranslate(), buttonstyleyellow)) changesort(10);
                    tempheight++;
                }


                Dictionary<int, long[]> tempDiction;
                if (multplanetproduct)
                {
                    tempDiction = multPlanetProduce;
                }
                else
                {
                    tempDiction = PlanetorSum && PlanetProduce != null && PlanetProduce.ContainsKey(pointPlanetId) ? PlanetProduce[pointPlanetId] : SumProduce;
                }
                if (TGMKinttostringMode)
                {
                    for (int i = (stationinfoindex - 1) * 20; i < stationinfoindex * 20 && i < iteminfoshow.Count; i++)
                    {
                        if (!tempDiction.ContainsKey(iteminfoshow[i])) continue;
                        ItemProto item = LDB.items.Select(iteminfoshow[i]);
                        GUI.Button(new Rect(0, heightdis * tempheight, heightdis, heightdis), item.iconSprite.texture, new GUIStyle());
                        GUI.Label(new Rect(heightdis , heightdis * tempheight, english ? heightdis * 8 : heightdis * 4, heightdis), item.name, styleitemname);
                        for (int j = 0; j < 10; j++)
                            GUI.Label(new Rect(heightdis + heightdis * 4 *(english ? j + 2: j+1), heightdis * tempheight, heightdis * 4, heightdis), TGMKinttostring(tempDiction[item.ID][j]), j % 2 == 0 ? styleblue : styleyellow);

                        tempheight++;
                    }
                }
                else
                {
                    for (int i = (stationinfoindex - 1) * 20; i < stationinfoindex * 20 && i < iteminfoshow.Count; i++)
                    {
                        if (!tempDiction.ContainsKey(iteminfoshow[i])) continue;
                        ItemProto item = LDB.items.Select(iteminfoshow[i]);
                        GUI.Button(new Rect(0, heightdis * tempheight, heightdis, heightdis), item.iconSprite.texture, new GUIStyle());
                        tempwidth = heightdis;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, english ? heightdis * 8 : heightdis * 4, heightdis), item.name, styleitemname);
                        tempwidth += english ? heightdis * 8 : heightdis * 4;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 4, heightdis), Threeinttostring(tempDiction[item.ID][0]), styleblue);
                        tempwidth += heightdis * 4;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 4, heightdis), Threeinttostring(tempDiction[item.ID][1]), styleyellow);
                        tempwidth += heightdis * 4;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 4, heightdis), Threeinttostring(tempDiction[item.ID][2]), styleblue);
                        tempwidth += heightdis * 4;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 4, heightdis), Threeinttostring(tempDiction[item.ID][3]), styleyellow);
                        tempwidth += heightdis * 4;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 2, heightdis), Threeinttostring(tempDiction[item.ID][4]), styleblue);
                        tempwidth += heightdis * 2;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 2, heightdis), Threeinttostring(tempDiction[item.ID][5]), styleyellow);
                        tempwidth += heightdis * 2;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 4, heightdis), Threeinttostring(tempDiction[item.ID][6]), styleblue);
                        tempwidth += heightdis * 4;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 4, heightdis), Threeinttostring(tempDiction[item.ID][7]), styleyellow);
                        tempwidth += heightdis * 4;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 4, heightdis), Threeinttostring(tempDiction[item.ID][8]), styleblue);
                        tempwidth += heightdis * 4;
                        GUI.Label(new Rect(tempwidth, heightdis * tempheight, heightdis * 4, heightdis), Threeinttostring(tempDiction[item.ID][9]), styleyellow);
                        tempheight++;
                    }
                }

                tempwidth = 0;
                int indexnum = (int)(iteminfoshow.Count / 20.5f) + 1;
                if (stationinfoindex > indexnum) stationinfoindex = 1;
                for (int i = 1; i <= indexnum &&indexnum>1; i++)
                {
                    if (i == stationinfoindex)
                    {
                        GUIStyle style = new GUIStyle(GUI.skin.button);
                        style.normal.textColor = new Color32(215, 186, 245, 255);
                        if (GUI.Button(new Rect(heightdis * tempwidth++, heightdis * 21, heightdis, heightdis), i + "", style))
                            stationinfoindex = i;
                    }
                    else
                    {
                        if (GUI.Button(new Rect(heightdis * tempwidth++, heightdis * 21, heightdis, heightdis), i + ""))
                            stationinfoindex = i;
                    }
                }
                tempheight++;
            }



            int tempheight1 = 0;
            if (filtercondition)
            {
                GUILayout.BeginArea(new Rect(0, heightdis*tempheight, window_width, 12* heightdis));
                tempheight1 = 0;
                GUI.Label(new Rect(0, 0, heightdis*3, heightdis), "产物筛选".getTranslate());
                rawmaterial = GUI.Toggle(new Rect(heightdis*3, 0, heightdis * 3, heightdis), rawmaterial, "一级原料".getTranslate());
                secondrawmaterial = GUI.Toggle(new Rect(heightdis * 6, 0, heightdis * 6, heightdis), secondrawmaterial, "二级原料".getTranslate());
                building = GUI.Toggle(new Rect(heightdis * 12, 0, heightdis * 3, heightdis), building, "建筑".getTranslate());
                compound = GUI.Toggle(new Rect(heightdis * 15, 0, heightdis * 3, heightdis), compound, "合成材料".getTranslate());
                onlyseechose = GUI.Toggle(new Rect(heightdis * 18, 0, heightdis*5, heightdis), onlyseechose, "只看目标产物".getTranslate());
                if (GUI.Toggle(new Rect(heightdis * 23, 0, heightdis * 7, heightdis), sortbypointproduct, "依目标产物排序".getTranslate()) != sortbypointproduct)
                {
                    sortbypointproduct = !sortbypointproduct;
                    if (sortbypointproduct)
                    {
                        for (int j = 0; j < ItemList.Count; j++)
                            Productsearchcondition[ItemList[j].ID] = false;
                        if (selectitemId > 0)
                            Productsearchcondition[selectitemId] = true;
                    }

                    changesearch();
                }
                tempheight1++;
                noreachtheoryproduce = GUI.Toggle(new Rect(0, heightdis*tempheight1, heightdis*6, heightdis), noreachtheoryproduce, "实时产量<理论产量".getTranslate());
                noreachneedproduce = GUI.Toggle(new Rect(heightdis * 6, heightdis * tempheight1, heightdis * 6, heightdis), noreachneedproduce, "实时产量<需求产量".getTranslate());
                theorynoreachneedproduce = GUI.Toggle(new Rect(heightdis*12, heightdis * tempheight1, heightdis*12, heightdis), theorynoreachneedproduce, "理论产量<需求产量".getTranslate());

                tempheight1++;
                bool temp1bool = GUI.Toggle(new Rect(0, heightdis * tempheight1, heightdis*4, heightdis), producelimittoggle, "实时产量".getTranslate());
                producelowerlimitstr = GUI.TextArea(new Rect(heightdis*4, heightdis * tempheight1, heightdis*4, heightdis), producelowerlimitstr + "/min");

                bool temp2bool = GUI.Toggle(new Rect(heightdis*8, heightdis * tempheight1, heightdis*4, heightdis), comsumeimittoggle, "实时消耗".getTranslate());
                comsumelowerlimitstr = GUI.TextArea(new Rect(heightdis * 12, heightdis * tempheight1, heightdis * 4, heightdis), comsumelowerlimitstr + "/min");

                bool temp3bool = GUI.Toggle(new Rect(heightdis * 16, heightdis * tempheight1, heightdis * 5, heightdis), theorycomsumelimittoggle, "需求产量".getTranslate());
                theorycomsumelowerlimitstr = GUI.TextArea(new Rect(heightdis * 21, heightdis * tempheight1, heightdis * 4, heightdis), theorycomsumelowerlimitstr + "/min");

                bool temp4bool = GUI.Toggle(new Rect(heightdis * 25, heightdis * tempheight1, heightdis * 4, heightdis), theoryproducelimittoggle, "理论产量".getTranslate());
                theoryproducelowerlimitstr = GUI.TextArea(new Rect(heightdis * 29, heightdis * tempheight1, heightdis * 4, heightdis), theoryproducelowerlimitstr + "/min");

                if (temp1bool != producelimittoggle || temp2bool != comsumeimittoggle || temp3bool != theorycomsumelimittoggle || temp4bool != theoryproducelimittoggle)
                {
                    producelimittoggle = temp1bool;
                    comsumeimittoggle = temp2bool;
                    theorycomsumelimittoggle = temp3bool;
                    theoryproducelimittoggle = temp4bool;
                    changesearch();
                }
                comsumelowerlimitstr = Regex.Replace(comsumelowerlimitstr, @"[^0-9]", "");
                if (comsumelowerlimitstr.Length == 0 || comsumelowerlimitstr.Length > 9) comsumelowerlimitstr = "0";

                theorycomsumelowerlimitstr = Regex.Replace(theorycomsumelowerlimitstr, @"[^0-9]", "");
                if (theorycomsumelowerlimitstr.Length == 0 || theorycomsumelowerlimitstr.Length > 9) theorycomsumelowerlimitstr = "0";

                theoryproducelowerlimitstr = Regex.Replace(theoryproducelowerlimitstr, @"[^0-9]", "");
                if (theoryproducelowerlimitstr.Length == 0 || theoryproducelowerlimitstr.Length > 9) theoryproducelowerlimitstr = "0";

                producelowerlimitstr = Regex.Replace(producelowerlimitstr, @"[^0-9]", "");
                if (producelowerlimitstr.Length == 0 || producelowerlimitstr.Length > 9) producelowerlimitstr = "0";

                int.TryParse(comsumelowerlimitstr, out comsumelowerlimit);
                int.TryParse(theorycomsumelowerlimitstr, out theorycomsumelowerlimit);
                int.TryParse(theoryproducelowerlimitstr, out theoryproducelowerlimit);
                int.TryParse(producelowerlimitstr, out producelowerlimit);


                tempheight1++;
                tempwidth = 0;
                for (int i = 0; i < ItemList.Count; i++)
                {
                    if (!Productsearchcondition.ContainsKey(ItemList[i].ID))
                        Productsearchcondition.Add(ItemList[i].ID, false);
                    int itemid = ItemList[i].ID;
                    if (itemid == 1030 || itemid == 1031) continue;
                    if ((itemid >= 1000 && itemid < 1030 || itemid == 1208) && !rawmaterial) continue;
                    if ((itemid > 1100 && itemid < 1200) && !secondrawmaterial) continue;
                    if (ItemList[i].CanBuild && !building) continue;
                    if (ItemList[i].recipes.Count > 0 && (itemid != 1003 && !(itemid > 1100 && itemid < 1200)) && ItemList[i].BuildIndex == 0 && !compound) continue;

                    int heightmode;

                    heightmode =heightdis;
                    GUIStyle style = new GUIStyle();
                    if (Productsearchcondition[itemid])
                        style.normal.background = Texture2D.whiteTexture;
                    if (GUI.Button(new Rect(tempwidth, heightdis * tempheight1, heightdis, heightdis), ItemList[i].iconSprite.texture, style))
                    {
                        Productsearchcondition[itemid] = !Productsearchcondition[itemid];
                        if (sortbypointproduct)
                        {
                            if (Productsearchcondition[itemid])
                            {
                                selectitemId = itemid;
                                for (int j = 0; j < ItemList.Count; j++)
                                {
                                    Productsearchcondition[itemid] = false;
                                }
                                Productsearchcondition[itemid] = true;
                            }
                        }
                        changesearch();
                    }
                    tempwidth += heightdis;
                    if (tempwidth > window_width-heightdis*2)
                    {
                        tempheight1++;
                        tempwidth = 0;
                    }
                }

                GUILayout.EndArea();
            }

            windowmaxheight = (tempheight + tempheight1+1) * heightdis;
            GUI.EndScrollView();
            GUILayout.EndArea();
        }

        private void DoMyWindow2(int id)
        {
            int lines = 1;
            int heightdis = GUI.skin.toggle.fontSize*2;
            int width = Localization.language != Language.zhCN ? 7 * heightdis : 4 * heightdis;
            GUILayout.BeginArea(new Rect(10, 0, width, 400));
            if (RemoteorLocal != GUI.Toggle(new Rect(0, heightdis *lines++, width, heightdis), RemoteorLocal, "本地/远程".getTranslate()))
            {
                RemoteorLocal = !RemoteorLocal;
                columnsbuttonstr[8] = !RemoteorLocal ? "本地提供".getTranslate() : "远程提供".getTranslate();
                columnsbuttonstr[9] = !RemoteorLocal ? "本地需求".getTranslate() : "远程需求".getTranslate();
                columnsbuttonstr[10] = !RemoteorLocal ? "本地仓储".getTranslate() : "远程仓储".getTranslate();
                if (cursortcolumnindex >= 8 && cursortcolumnindex <= 10)
                {
                    switch (cursortrule)
                    {
                        case 1:
                            columnsbuttonstr[cursortcolumnindex] += "↑"; break;
                        case -1:
                            columnsbuttonstr[cursortcolumnindex] += "↓"; break;
                        case 0:
                            columnsbuttonstr[cursortcolumnindex] += "-"; break;
                    }
                }
            }
            if (PlanetorSum != GUI.Toggle(new Rect(0, heightdis * lines++, width, heightdis), PlanetorSum, "全部/星球".getTranslate()))
            {
                PlanetorSum = !PlanetorSum;
                if (!PlanetorSum)
                {
                    pointPlanetId = 0;
                }
                RefreshProductStat();
            }
            StopRefresh = GUI.Toggle(new Rect(0, heightdis * lines++, width, heightdis), StopRefresh, "停止刷新".getTranslate());
            if (TGMKinttostringMode != GUI.Toggle(new Rect(0, heightdis * lines++, width, heightdis), TGMKinttostringMode, "单位转化".getTranslate()))
            {
                TGMKinttostringMode = !TGMKinttostringMode;
                if (TGMKinttostringMode)
                {
                    styleblue.fontSize = 20;
                    styleyellow.fontSize = 20;
                }
                else
                {
                    styleblue.fontSize = 15;
                    styleyellow.fontSize = 15;
                }
            }
            if (filtercondition != GUI.Toggle(new Rect(0, heightdis * lines++, width, heightdis), filtercondition, "筛选条件".getTranslate()))
            {
                filtercondition = !filtercondition;
                if (filtercondition)
                {
                    refreshPlanetinfo = false;
                }
            }
            if (refreshfactoryinfo != GUI.Toggle(new Rect(0, heightdis * lines++, width, heightdis), refreshfactoryinfo, "工厂信息".getTranslate()))
            {
                refreshfactoryinfo = !refreshfactoryinfo;
                if (refreshfactoryinfo)
                {
                    RefreshFactory(pointPlanetId);
                }
            }
            if ( refreshPlanetinfo != GUI.Toggle(new Rect(0, heightdis * lines++, width, heightdis), refreshPlanetinfo, "星球信息".getTranslate()))
            {
                refreshPlanetinfo = !refreshPlanetinfo;
                if (refreshPlanetinfo)
                {
                    refreshfactoryinfo = false;
                    filtercondition = false;
                }
            }
            if (multplanetproduct != GUI.Toggle(new Rect(0, heightdis * lines++, width, heightdis), multplanetproduct, "多选星球".getTranslate()))
            {
                multplanetproduct = !multplanetproduct;
                if (multplanetproduct)
                {
                    refreshfactoryinfo = false;
                    PlanetorSum = true;
                }
            }
            if (CloseUIpanel.Value != GUI.Toggle(new Rect(0, heightdis * lines++, width, heightdis), CloseUIpanel.Value, "关闭白边".getTranslate()))
            {
                CloseUIpanel.Value = !CloseUIpanel.Value;
                ui_morestatinfopanel.SetActive(!CloseUIpanel.Value);
            }
            GUILayout.EndArea();
        }
        
        private void DoMyWindow3(int id)
        {
            int heightdis = GUI.skin.toggle.fontSize*2;
            GUILayout.BeginArea(new Rect(0, 20, heightdis * 8+10, heightdis * 21));

            RefreshPlanet();
            for (int i = (pdselectinfoindex - 1) * 20; i < pdselectinfoindex * 20 && i < planetinfoshow.Count; i++)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (!multplanetproduct)
                {
                    if (pointPlanetId == planetinfoshow[i])
                        style.normal.textColor = new Color32(215, 186, 245, 255);
                    if (GUI.Button(new Rect(5, (i - (pdselectinfoindex - 1) * 20) * heightdis, heightdis*8, heightdis), GameMain.galaxy.PlanetById(planetinfoshow[i]).displayName, style))
                    {
                        pointPlanetId = planetinfoshow[i];
                        RefreshFactory(pointPlanetId);
                    }
                }
                else
                {
                    if(planetsproductinfoshow.Contains(planetinfoshow[i]))
                        style.normal.textColor = new Color32(215, 186, 245, 255);
                    if (GUI.Button(new Rect(5, (i - (pdselectinfoindex - 1) * heightdis) * heightdis, heightdis*8, heightdis), GameMain.galaxy.PlanetById(planetinfoshow[i]).displayName, style))
                    {
                        if (planetsproductinfoshow.Contains(planetinfoshow[i]))
                            planetsproductinfoshow.Remove(planetinfoshow[i]);
                        else
                            planetsproductinfoshow.Add(planetinfoshow[i]);
                        RefreshFactory(pointPlanetId);
                    }
                }
                
            }

            int indexnum = planetinfoshow.Count / 20 + 1;
            int tempwidth = 0;
            for (int i = pdselectinfoindex>3? pdselectinfoindex-2:1; i <= indexnum; i++)
            {
                if (i == pdselectinfoindex)
                {
                    GUIStyle style = new GUIStyle(GUI.skin.button);
                    style.normal.textColor = new Color32(215, 186, 245, 255);
                    if (GUI.Button(new Rect(heightdis * tempwidth++, 20*heightdis, heightdis, heightdis), i + "", style))
                        pdselectinfoindex = i;
                }
                else
                {
                    if (GUI.Button(new Rect(heightdis * tempwidth++, 20 * heightdis, heightdis, heightdis), i + ""))
                        pdselectinfoindex = i;
                }
            }
            GUILayout.EndArea();
        }

        public void moveWindow_xl_first(ref float x, ref float y, ref float x_move, ref float y_move, ref bool movewindow, ref float tempx, ref float tempy, float x_width)
        {
            Vector2 temp = Input.mousePosition;
            if (temp.x > x && temp.x < x + x_width && maxheight - temp.y > y && maxheight - temp.y < y + 20)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!movewindow)
                    {
                        x_move = x;
                        y_move = y;
                        tempx = temp.x;
                        tempy = maxheight - temp.y;
                    }
                    movewindow = true;
                    x = x_move + temp.x - tempx;
                    y = y_move + (maxheight - temp.y) - tempy;
                }
                else
                {
                    movewindow = false;
                    tempx = x;
                    tempy = y;
                }
            }
            else if (movewindow)
            {
                movewindow = false;
                x = x_move + temp.x - tempx;
                y = y_move + (maxheight - temp.y) - tempy;
            }
        }
        
        public void scaling_window(float x, float y, ref float x_move, ref float y_move)
        {
            Vector2 temp = Input.mousePosition;
            if (Input.GetMouseButton(0))
            {
                if ((temp.x + 10 > x_move && temp.x - 10 < x_move) && (maxheight - temp.y >= y_move && maxheight - temp.y <= y_move + y) || leftscaling)
                {
                    x -= temp.x - x_move;
                    x_move = temp.x;
                    leftscaling = true;
                    rightscaling = false;
                }
                if ((temp.x + 10 > x_move + x && temp.x - 10 < x_move + x) && (maxheight - temp.y >= y_move && maxheight - temp.y <= y_move + y) || rightscaling)
                {
                    x += temp.x - x_move - x;
                    rightscaling = true;
                    leftscaling = false;
                }
                if ((maxheight - temp.y + 10 > y + y_move && maxheight - temp.y - 10 < y + y_move) && (temp.x >= x_move && temp.x <= x_move + x) || bottomscaling)
                {
                    y += maxheight - temp.y - (y_move + y);
                    bottomscaling = true;
                }
                if (rightscaling || leftscaling)
                {
                    if ((maxheight - temp.y + 10 > y_move && maxheight - temp.y - 10 < y_move) && (temp.x >= x_move && temp.x <= x_move + x) || topscaling)
                    {
                        y -= maxheight - temp.y - y_move;
                        y_move = maxheight - temp.y;
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
        
        private void changesort(int columnnum)
        {
            if(cursortrule!=0)
                columnsbuttonstr[cursortcolumnindex] = columnsbuttonstr[cursortcolumnindex].Substring(0, columnsbuttonstr[cursortcolumnindex].Length-1);
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
                    columnsbuttonstr[columnnum] += "↑";break;
                case -1:
                    columnsbuttonstr[columnnum] += "↓"; break;
                case 0:
                    cursortcolumnindex = 0; break;
            }
        }
        
        public Dictionary<int, long[]> sortItem(Dictionary<int, long[]> itemsmap,int columnnum,int sortrules)
        {
            if (sortrules == 0) return itemsmap;
            Dictionary<int, long[]> result = new Dictionary<int, long[]>();
            if (columnnum == 0)
            {
                List<int> itemids = new List<int>(itemsmap.Keys);
                itemids.Sort();
                if (sortrules == -1)
                    itemids.Reverse();
                for(int i = 0; i < itemids.Count; i++)
                    result.Add(itemids[i], itemsmap[itemids[i]]);

                return result;
            }
            columnnum--;
            Dictionary<int, long[]> tempitemsmap = new Dictionary<int, long[]>(itemsmap);
            List<int> temp = new List<int>();
            long max;
            int tempid;
            int itemsmapcount = itemsmap.Count;
            for(int i = 0; i < itemsmapcount; i++)
            {
                max = -1;
                tempid = 0;
                foreach (KeyValuePair<int, long[]> wap in tempitemsmap)
                {
                    if (max < wap.Value[columnnum])
                    {
                        tempid = wap.Key;
                        max = wap.Value[columnnum];
                    }
                }
                temp.Add(tempid);
                tempitemsmap.Remove(tempid);
            }
            if (sortrules == 1)
                temp.Reverse();
            for (int i = 0; i < itemsmapcount; i++)
                result.Add(temp[i], itemsmap[temp[i]]);
            return result;
        }

        private void changesearch()
        {
            
        }

        private string searchchineseTranslate(int i)
        {
            if (i < 15) return LDB.ItemName(int.Parse(VeintypechineseTranslate(i, 1)));
            if (i == 15) return "可燃冰".getTranslate();
            if (i == 16) return "氢".getTranslate();
            if (i == 17) return "重氢".getTranslate();
            if (i == 18) return "水".getTranslate();
            if (i == 19) return "硫酸".getTranslate();
            if (i == 20) return "潮汐锁定".getTranslate();
            if (i == 21) return "轨道共振1:2".getTranslate();
            if (i == 22) return "轨道共振1:4".getTranslate();
            if (i == 23) return "横躺自传".getTranslate();
            if (i == 24) return "反向自传".getTranslate();
            if (i == 25) return "多卫星".getTranslate();
            return "";
        }

        private void RefreshProductStat()
        {
            PlanetProductDiction = new Dictionary<int, Dictionary<int, float>>();
            PlanetRequireDiction = new Dictionary<int, Dictionary<int, float>>();
            PlanetComsumerDiction = new Dictionary<int, Dictionary<int, long>>();
            PlanetProducerDiction = new Dictionary<int, Dictionary<int, long>>();
            PlanetProduce = new Dictionary<int, Dictionary<int, long[]>>();
            productIndices = new Dictionary<int, Dictionary<int, int>>();
            SumProduce = new Dictionary<int, long[]>();
            powerenergyinfoshow = new Dictionary<int, long[]>();
            sumpowerinfoshow = new long[6];
            Dictionary<int, long[]>  itemprovide = new Dictionary<int, long[]>();
            foreach (StarData sd in GameMain.galaxy.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    if (pd.factory == null) continue;
                    FactoryProductionStat factoryProduction = GameMain.data.statistics.production.factoryStatPool[pd.factory.index];
                    if (factoryProduction == null) continue;
                    productIndices[pd.id] = new Dictionary<int, int>();
                    PlanetProductDiction[pd.id] = new Dictionary<int, float>();
                    PlanetRequireDiction[pd.id] = new Dictionary<int, float>();
                    PlanetComsumerDiction[pd.id] = new Dictionary<int, long>();
                    PlanetProducerDiction[pd.id] = new Dictionary<int, long>();
                    PlanetProduce[pd.id] = new Dictionary<int, long[]>();
                    powerenergyinfoshow[pd.id] = new long[6];
                    for (int i = 1; i < factoryProduction.productCursor; i++)
                    {
                        if (!productIndices[pd.id].ContainsKey(i))
                            foreach (ItemProto ip in ItemList)
                                if (i == factoryProduction.productIndices[ip.ID])
                                    productIndices[pd.id].Add(i, ip.ID);
                        if (productIndices[pd.id].ContainsKey(i))
                        {
                            int itemId = productIndices[pd.id][i];
                            if (PlanetorSum||refreshPlanetinfo)
                            {
                                if (!PlanetProduce[pd.id].ContainsKey(itemId))
                                    PlanetProduce[pd.id].Add(itemId, new long[10]);
                                PlanetProduce[pd.id][itemId][0] += factoryProduction.productPool[i].total[1];
                                PlanetProduce[pd.id][itemId][1] += factoryProduction.productPool[i].total[8];
                            }
                            else
                            {
                                if (!SumProduce.ContainsKey(itemId))
                                    SumProduce[itemId] = new long[10];
                                SumProduce[itemId][0] += factoryProduction.productPool[i].total[1];
                                SumProduce[itemId][1] += factoryProduction.productPool[i].total[8];
                            }
                        }
                    }
                    
                    FactorySystem fs = pd.factory.factorySystem;

                    foreach (MinerComponent mc in fs.minerPool)
                    {
                        if (mc.id > 0 && mc.entityId > 0)
                        {
                            if (mc.type == EMinerType.Water&& pd.waterItemId > 0)
                            {
                                if (!PlanetProductDiction[pd.id].ContainsKey(pd.waterItemId))
                                    PlanetProductDiction[pd.id][pd.waterItemId] = 0;
                                if (!PlanetProducerDiction[pd.id].ContainsKey(pd.waterItemId))
                                    PlanetProducerDiction[pd.id][pd.waterItemId] = 0;
                                PlanetProductDiction[pd.id][pd.waterItemId] += (long)(50 * GameMain.history.miningSpeedScale);
                                PlanetProducerDiction[pd.id][pd.waterItemId]++;
                            }
                            else if (mc.type == EMinerType.Vein && mc.veinCount > 0)
                            {
                                int index = 0;
                                for (int i = 0; i < mc.veins.Length; i++)
                                {
                                    if (mc.veins[i] == 0 || mc.veins[i] >= fs.factory.veinPool.Length) continue;
                                    VeinData vd = fs.factory.veinPool[mc.veins[i]];
                                    if (vd.productId == 0) continue;
                                    index = vd.productId;
                                    break;
                                }
                                if (!PlanetProductDiction[pd.id].ContainsKey(index))
                                    PlanetProductDiction[pd.id][index] = 0;
                                if (!PlanetProducerDiction[pd.id].ContainsKey(index))
                                    PlanetProducerDiction[pd.id][index] = 0;
                                PlanetProducerDiction[pd.id][index]++;
                                bool isveincollector = pd.factory.entityPool[mc.entityId].stationId > 0;
                                PlanetProductDiction[pd.id][index] +=(long)( 30*(isveincollector?2:1)* mc.veinCount * GameMain.history.miningSpeedScale*mc.speed)/10000;
                            }
                            else if (mc.type == EMinerType.Oil && mc.veinCount > 0)
                            {
                                for (int i = 0; i < mc.veins.Length; i++)
                                {
                                    if (mc.veins[i] == 0 || mc.veins[i] >= fs.factory.veinPool.Length) continue;
                                    VeinData vd = fs.factory.veinPool[mc.veins[i]];
                                    if (!PlanetProductDiction[pd.id].ContainsKey(1007))
                                        PlanetProductDiction[pd.id][1007] = 0;
                                    if (!PlanetProducerDiction[pd.id].ContainsKey(1007))
                                        PlanetProducerDiction[pd.id][1007] = 0;
                                    PlanetProducerDiction[pd.id][1007]++;
                                    PlanetProductDiction[pd.id][1007] += (long)(vd.amount * GameMain.history.miningSpeedScale * VeinData.oilSpeedMultiplier * 60);
                                    break;
                                }

                            }
                        }
                    }
                    foreach (FractionatorComponent fc in fs.fractionatorPool)
                    {
                        if (fc.id > 0 && fc.entityId > 0)
                        {
                            if (!PlanetProductDiction[pd.id].ContainsKey(1121))
                                PlanetProductDiction[pd.id][1121] = 0;
                            if (!PlanetRequireDiction[pd.id].ContainsKey(1120))
                                PlanetRequireDiction[pd.id][1120] = 0;
                            if (!PlanetProducerDiction[pd.id].ContainsKey(1121))
                                PlanetProducerDiction[pd.id][1121] = 0;
                            if (!PlanetComsumerDiction[pd.id].ContainsKey(1120))
                                PlanetComsumerDiction[pd.id][1120] = 0;
                            PlanetProducerDiction[pd.id][1121]++;
                            PlanetComsumerDiction[pd.id][1120]++;
                            float num = Mathf.Clamp(Mathf.Min(fc.fluidInputCargoCount, 30) * (int)(fc.fluidInputCount / (double)fc.fluidInputCargoCount + 0.5) * 60, 0, 7200) * (1 + fc.extraIncProduceProb) / 100;
                            PlanetProductDiction[pd.id][1121] += num;
                            PlanetRequireDiction[pd.id][1120] += num;
                        }
                    }
                    foreach (SiloComponent sc in fs.siloPool)
                    {
                        if (sc.id > 0 && sc.entityId > 0)
                        {
                            if (!PlanetRequireDiction[pd.id].ContainsKey(1503))
                                PlanetRequireDiction[pd.id][1503] = 0;
                            if (!PlanetComsumerDiction[pd.id].ContainsKey(1503))
                                PlanetComsumerDiction[pd.id][1503] = 0;
                            PlanetComsumerDiction[pd.id][1503]++;
                            PlanetRequireDiction[pd.id][1503] += 5;
                        }
                    }
                    foreach (EjectorComponent ec in fs.ejectorPool)
                    {
                        if (ec.id > 0 && ec.entityId > 0)
                        {
                            if (!PlanetRequireDiction[pd.id].ContainsKey(1501))
                                PlanetRequireDiction[pd.id][1501] = 0;
                            if (!PlanetComsumerDiction[pd.id].ContainsKey(1501))
                                PlanetComsumerDiction[pd.id][1501] = 0;
                            PlanetComsumerDiction[pd.id][1501]++;
                            PlanetRequireDiction[pd.id][1501] += 20;
                        }
                    }

                    foreach (AssemblerComponent ac in fs.assemblerPool) 
                    {
                        if (ac.id > 0 && ac.entityId > 0)
                        {
                            RecipeProto rp = LDB.recipes.Select(ac.recipeId);
                            if (rp != null)
                            {
                                for (int i = 0; i < rp.Items.Length; i++)
                                {
                                    if (!PlanetRequireDiction[pd.id].ContainsKey(rp.Items[i]))
                                        PlanetRequireDiction[pd.id].Add(rp.Items[i], 0);
                                    if (!PlanetComsumerDiction[pd.id].ContainsKey(rp.Items[i]))
                                        PlanetComsumerDiction[pd.id][rp.Items[i]] = 0;
                                    PlanetComsumerDiction[pd.id][rp.Items[i]]++;

                                    if (ac.extraSpeed == 0)
                                        PlanetRequireDiction[pd.id][rp.Items[i]] += rp.ItemCounts[i] * 9.0f * ac.speedOverride / (rp.TimeSpend * 25.0f);
                                    else
                                        PlanetRequireDiction[pd.id][rp.Items[i]] += rp.ItemCounts[i] * 9.0f * ac.speed/ (rp.TimeSpend * 25.0f);
                                }
                                for (int i = 0; i < rp.Results.Length; i++)
                                {
                                    if (!PlanetProductDiction[pd.id].ContainsKey(rp.Results[i]))
                                        PlanetProductDiction[pd.id].Add(rp.Results[i], 0);
                                    if (!PlanetProducerDiction[pd.id].ContainsKey(rp.Results[i]))
                                        PlanetProducerDiction[pd.id][rp.Results[i]] = 0;
                                    PlanetProducerDiction[pd.id][rp.Results[i]]++;
                                    if(ac.extraSpeed==0)
                                        PlanetProductDiction[pd.id][rp.Results[i]] += rp.ResultCounts[i] * 9.0f * ac.speedOverride / (rp.TimeSpend * 25.0f);
                                    else
                                        PlanetProductDiction[pd.id][rp.Results[i]] += rp.ResultCounts[i] * 9.0f*(ac.speed+ac.extraSpeed / 10) / (rp.TimeSpend * 25.0f);
                                }
                            }

                        }
                    }
                    foreach (LabComponent lc in fs.labPool)
                    {
                        if (lc.id > 0 && lc.entityId > 0)
                        {
                            RecipeProto rp = LDB.recipes.Select(lc.recipeId);
                            if (rp != null)
                            {
                                for (int i = 0; i < rp.Items.Length; i++)
                                {

                                    if (!PlanetRequireDiction[pd.id].ContainsKey(rp.Items[i]))
                                        PlanetRequireDiction[pd.id].Add(rp.Items[i], 0);
                                    if (!PlanetComsumerDiction[pd.id].ContainsKey(rp.Items[i]))
                                        PlanetComsumerDiction[pd.id][rp.Items[i]] = 0;
                                    PlanetComsumerDiction[pd.id][rp.Items[i]]++;

                                    if (lc.extraSpeed == 0)
                                        PlanetRequireDiction[pd.id][rp.Items[i]] += rp.ItemCounts[i] * 3600.0f * lc.speedOverride / lc.speed / rp.TimeSpend;
                                    else
                                        PlanetRequireDiction[pd.id][rp.Items[i]] += rp.ItemCounts[i] * 3600.0f / rp.TimeSpend;
                                }
                                for (int i = 0; i < rp.Results.Length; i++)
                                {
                                    if (!PlanetProductDiction[pd.id].ContainsKey(rp.Results[i]))
                                        PlanetProductDiction[pd.id].Add(rp.Results[i], 0);
                                    if (!PlanetProducerDiction[pd.id].ContainsKey(rp.Results[i]))
                                        PlanetProducerDiction[pd.id][rp.Results[i]] = 0;
                                    PlanetProducerDiction[pd.id][rp.Results[i]]++;

                                    if (lc.extraSpeed == 0)
                                        PlanetProductDiction[pd.id][rp.Results[i]] += rp.ResultCounts[i] * 3600.0f*lc.speedOverride/lc.speed / rp.TimeSpend;
                                    else
                                        PlanetProductDiction[pd.id][rp.Results[i]] += rp.ResultCounts[i] * 36.0f * (100 + lc.extraSpeed * 10 / lc.speed) / rp.TimeSpend;
                                }
                            }
                        }
                    }
                    float sum = 0;
                    foreach (PowerGeneratorComponent pgc in fs.factory.powerSystem.genPool)
                    {
                        if (pgc.gamma)
                        {
                            float eta = 1f - GameMain.history.solarEnergyLossRate;
                            sum += pgc.EtaCurrent_Gamma(eta) * pgc.RequiresCurrent_Gamma(eta);
                            if (!PlanetProducerDiction[pd.id].ContainsKey(1208))
                                PlanetProducerDiction[pd.id][1208] = 0;
                            PlanetProducerDiction[pd.id][1208]++;
                        }
                    }
                    if (sum > 0)
                    {
                        if (!PlanetProductDiction[pd.id].ContainsKey(1208))
                            PlanetProductDiction[pd.id][1208] = 0;
                        PlanetProductDiction[pd.id][1208] = (long)(sum * 3.0f / 1000000.0f);
                    }
                    foreach (PowerExchangerComponent pec in fs.factory.powerSystem.excPool)
                    {
                        if (pec.id > 0 && pec.targetState!=0)
                        {
                            int product = pec.targetState == 1?2207:2206;
                            int requireitem = 4413- product;

                            if (!PlanetProductDiction[pd.id].ContainsKey(product))
                                PlanetProductDiction[pd.id][product] = 0;
                            if (!PlanetProducerDiction[pd.id].ContainsKey(product))
                                PlanetProducerDiction[pd.id][product] = 0;
                            if (!PlanetRequireDiction[pd.id].ContainsKey(requireitem))
                                PlanetRequireDiction[pd.id][requireitem] = 0;
                            if (!PlanetComsumerDiction[pd.id].ContainsKey(requireitem))
                                PlanetComsumerDiction[pd.id][requireitem] = 0;
                            PlanetProducerDiction[pd.id][product]++;
                            PlanetComsumerDiction[pd.id][requireitem]++;
                            PlanetProductDiction[pd.id][product] += 10;
                            PlanetRequireDiction[pd.id][requireitem] += 10;
                        }
                    }
                    
                    //if(powerenergyinfoshow.ContainsKey(0))
                    //    powerenergyinfoshow.Add(0, 0);
                    //powerenergyinfoshow[0] = factoryProduction.energyConsumption;
                    if(factoryProduction.powerPool != null && factoryProduction.powerPool.Length > 0)
                    {
                        powerenergyinfoshow[pd.id][0] = factoryProduction.powerPool[0].total[0] / 10;
                        powerenergyinfoshow[pd.id][1] = factoryProduction.powerPool[1].total[0] / 10;
                        powerenergyinfoshow[pd.id][2] = factoryProduction.energyConsumption;
                        powerenergyinfoshow[pd.id][3] = factoryProduction.powerPool[3].total[0] / 10;
                        powerenergyinfoshow[pd.id][4] = factoryProduction.powerPool[2].total[0] / 10;
                        sumpowerinfoshow[0]+= factoryProduction.powerPool[0].total[0] / 10;
                        sumpowerinfoshow[1] +=factoryProduction.powerPool[1].total[0] / 10;
                        sumpowerinfoshow[2] += factoryProduction.energyConsumption;
                        sumpowerinfoshow[3] = factoryProduction.powerPool[3].total[0] / 10;
                        sumpowerinfoshow[4] = factoryProduction.powerPool[2].total[0] / 10;
                    }
                    if (fs.storage != null)
                    {
                        if (fs.storage.storagePool != null)
                        {
                            foreach (StorageComponent localsc in fs.storage.storagePool)
                            {
                                if (localsc != null && localsc.entityId > 0 && !localsc.isEmpty)
                                {
                                    for (int i = 0; i < localsc.grids.Length; i++)
                                    {
                                        int itemId = localsc.grids[i].itemId;
                                        if (itemId > 0)
                                        {
                                            if (PlanetorSum || refreshPlanetinfo)
                                            {
                                                if (!PlanetProduce[pd.id].ContainsKey(itemId))
                                                    PlanetProduce[pd.id].Add(itemId, new long[10]);
                                                PlanetProduce[pd.id][itemId][6] += localsc.grids[i].count;
                                            }
                                            else
                                            {
                                                if (!SumProduce.ContainsKey(itemId))
                                                    SumProduce[itemId] = new long[10];
                                                SumProduce[itemId][6] += localsc.grids[i].count;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (fs.storage.tankPool!=null)
                        {
                            foreach (TankComponent tc in fs.storage.tankPool)
                            {
                                if (tc.id > 0 && tc.fluidId > 0 && tc.fluidCount > 0)
                                {
                                    int itemId = tc.fluidId;
                                    if (PlanetorSum || refreshPlanetinfo)
                                    {
                                        if (!PlanetProduce[pd.id].ContainsKey(itemId))
                                            PlanetProduce[pd.id].Add(itemId, new long[10]);
                                        PlanetProduce[pd.id][itemId][6] += tc.fluidCount;
                                    }
                                    else
                                    {
                                        if (!SumProduce.ContainsKey(itemId))
                                            SumProduce[itemId] = new long[10];
                                        SumProduce[itemId][6] += tc.fluidCount;
                                    }
                                }
                            }
                        }
                    }
                    int tempi = 0;
                    foreach (StationComponent sc in fs.factory.transport.stationPool)
                    {
                        if (sc != null && sc.entityId > 0)
                        {
                            tempi++;
                            for (int i = 0; i < sc.storage.Length; i++)
                            {
                                int itemId = sc.storage[i].itemId;
                                if (itemId > 0)
                                {
                                    if (PlanetorSum || refreshPlanetinfo)
                                    {
                                        if (!PlanetProduce[pd.id].ContainsKey(itemId))
                                            PlanetProduce[pd.id].Add(itemId, new long[10]);
                                        PlanetProduce[pd.id][itemId][6] += sc.storage[i].count;
                                        if (!RemoteorLocal)
                                        {
                                            if (sc.storage[i].localLogic == ELogisticStorage.Supply)
                                                PlanetProduce[pd.id][itemId][7] += sc.storage[i].count;
                                            else if (sc.storage[i].localLogic == ELogisticStorage.Demand)
                                                PlanetProduce[pd.id][itemId][8] += sc.storage[i].count;
                                            else
                                                PlanetProduce[pd.id][itemId][9] += sc.storage[i].count;
                                        }
                                        else if (RemoteorLocal && (sc.isCollector || sc.isStellar))
                                        {
                                            if (sc.storage[i].remoteLogic == ELogisticStorage.Supply)
                                                PlanetProduce[pd.id][itemId][7] += sc.storage[i].count;
                                            else if (sc.storage[i].remoteLogic == ELogisticStorage.Demand)
                                                PlanetProduce[pd.id][itemId][8] += sc.storage[i].count;
                                            else
                                                PlanetProduce[pd.id][itemId][9] += sc.storage[i].count;
                                        }
                                    }
                                    else
                                    {
                                        if (!SumProduce.ContainsKey(itemId))
                                            SumProduce[itemId] = new long[10];
                                        SumProduce[itemId][6] += sc.storage[i].count;
                                        if (!RemoteorLocal)
                                        {
                                            if (sc.storage[i].localLogic == ELogisticStorage.Supply)
                                                SumProduce[itemId][7] += sc.storage[i].count;
                                            else if (sc.storage[i].localLogic == ELogisticStorage.Demand)
                                                SumProduce[itemId][8] += sc.storage[i].count;
                                            else
                                                SumProduce[itemId][9] += sc.storage[i].count;
                                        }
                                        else if (RemoteorLocal && (sc.isCollector || sc.isStellar))
                                        {
                                            if (sc.storage[i].remoteLogic == ELogisticStorage.Supply)
                                                SumProduce[itemId][7] += sc.storage[i].count;
                                            else if (sc.storage[i].remoteLogic == ELogisticStorage.Demand)
                                                SumProduce[itemId][8] += sc.storage[i].count;
                                            else
                                                SumProduce[itemId][9] += sc.storage[i].count;
                                        }
                                    }

                                }
                            }
                            float miningSpeedScale = GameMain.history.miningSpeedScale;
                            int pdId = pd.id;
                            if (sc.name != null&&sc.isStellar && (sc.name.Equals("Station_miner")|| sc.name.Equals("星球矿机")))
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    int itemId = sc.storage[i].itemId;
                                    if (itemId > 0)
                                    {
                                        if (!PlanetProductDiction.ContainsKey(pdId))
                                            PlanetProductDiction[pdId] = new Dictionary<int, float>();
                                        if (!PlanetProductDiction[pdId].ContainsKey(itemId))
                                            PlanetProductDiction[pdId][itemId] = 0;
                                        if (!PlanetProducerDiction.ContainsKey(pdId))
                                            PlanetProducerDiction[pdId] = new Dictionary<int, long>();
                                        if (!PlanetProducerDiction[pdId].ContainsKey(itemId))
                                            PlanetProducerDiction[pdId][itemId] = 0;
                                        PlanetProducerDiction[pdId][itemId]++;
                                        if (pd.waterItemId == itemId)
                                            PlanetProductDiction[pdId][itemId] += (long)(1800 * miningSpeedScale);
                                        else
                                            PlanetProductDiction[pdId][itemId] += itemId!=1007?(int)(getVeinnumber(itemId, pdId) * miningSpeedScale) / 2 * 60: (int)(getVeinnumber(itemId, pdId) * miningSpeedScale)*60;
                                    }
                                }
                            }
                            if (sc.collectionPerTick != null&&sc.isCollector)
                            {
                                PrefabDesc prefabDesc = LDB.items.Select(ItemProto.stationCollectorId).prefabDesc;
                                double collectorsWorkCost = (double)prefabDesc.workEnergyPerTick * 60.0;
                                collectorsWorkCost /= (double)prefabDesc.stationCollectSpeed;
                                double gasTotalHeat = pd.gasTotalHeat;
                                float collectSpeedRate = gasTotalHeat - collectorsWorkCost <= 0.0 ? 1f : (float)((miningSpeedScale * gasTotalHeat - collectorsWorkCost) / (gasTotalHeat - collectorsWorkCost));
                                for (int index = 0; index < sc.collectionIds.Length; ++index)
                                {
                                    if (!PlanetProductDiction[pdId].ContainsKey(sc.storage[index].itemId))
                                        PlanetProductDiction[pdId][sc.storage[index].itemId] = 0;
                                    if (!PlanetProducerDiction.ContainsKey(pdId))
                                        PlanetProducerDiction[pdId] = new Dictionary<int, long>();
                                    if (!PlanetProducerDiction[pdId].ContainsKey(sc.storage[index].itemId))
                                        PlanetProducerDiction[pdId][sc.storage[index].itemId] = 0;
                                    PlanetProductDiction[pdId][sc.storage[index].itemId] += (long)(sc.collectionPerTick[index] * collectSpeedRate * 3600);
                                    PlanetProducerDiction[pdId][sc.storage[index].itemId]++;
                                }

                            }

                        }
                    }
                    

                }

            }
            foreach (KeyValuePair<int, Dictionary<int, float>> wap in PlanetProductDiction)
            {
                if ((PlanetorSum || refreshPlanetinfo) && !PlanetProduce.ContainsKey(wap.Key))
                    PlanetProduce.Add(wap.Key, new Dictionary<int, long[]>());
                foreach (KeyValuePair<int, float> wap1 in wap.Value)
                {
                    if (PlanetorSum || refreshPlanetinfo)
                    {
                        if (!PlanetProduce[wap.Key].ContainsKey(wap1.Key))
                            PlanetProduce[wap.Key].Add(wap1.Key, new long[10]);
                        PlanetProduce[wap.Key][wap1.Key][2] += (int)wap1.Value;
                    }
                    else
                    {
                        if (!SumProduce.ContainsKey(wap1.Key))
                            SumProduce[wap1.Key] = new long[10];
                        SumProduce[wap1.Key][2] += (long)wap1.Value;
                    }
                }
            }
            foreach (KeyValuePair<int, Dictionary<int, float>> wap in PlanetRequireDiction)
            {
                if ((PlanetorSum || refreshPlanetinfo) && !PlanetProduce.ContainsKey(wap.Key))
                    PlanetProduce.Add(wap.Key, new Dictionary<int, long[]>());
                foreach (KeyValuePair<int, float> wap1 in wap.Value)
                {
                    if (PlanetorSum || refreshPlanetinfo)
                    {
                        if (!PlanetProduce[wap.Key].ContainsKey(wap1.Key))
                            PlanetProduce[wap.Key].Add(wap1.Key, new long[10]);
                        PlanetProduce[wap.Key][wap1.Key][3] += (long)wap1.Value;
                    }
                    else
                    {
                        if (!SumProduce.ContainsKey(wap1.Key))
                            SumProduce[wap1.Key] = new long[10];
                        SumProduce[wap1.Key][3] += (int)wap1.Value;
                    }
                }
            }
            foreach (KeyValuePair<int, Dictionary<int, long>> wap in PlanetProducerDiction)
            {
                if ((PlanetorSum || refreshPlanetinfo) && !PlanetProduce.ContainsKey(wap.Key))
                    PlanetProduce.Add(wap.Key, new Dictionary<int, long[]>());
                foreach (KeyValuePair<int, long> wap1 in wap.Value)
                {
                    if (PlanetorSum || refreshPlanetinfo)
                    {
                        if (!PlanetProduce[wap.Key].ContainsKey(wap1.Key))
                            PlanetProduce[wap.Key].Add(wap1.Key, new long[10]);
                        PlanetProduce[wap.Key][wap1.Key][4] += wap1.Value;
                    }
                    else
                    {
                        if (!SumProduce.ContainsKey(wap1.Key))
                            SumProduce[wap1.Key] = new long[10];
                        SumProduce[wap1.Key][4] += wap1.Value;
                    }
                }
            }
            foreach (KeyValuePair<int, Dictionary<int, long>> wap in PlanetComsumerDiction)
            {
                if ((PlanetorSum || refreshPlanetinfo) && !PlanetProduce.ContainsKey(wap.Key))
                    PlanetProduce.Add(wap.Key, new Dictionary<int, long[]>());
                foreach (KeyValuePair<int, long> wap1 in wap.Value)
                {
                    if (PlanetorSum || refreshPlanetinfo)
                    {
                        if (!PlanetProduce[wap.Key].ContainsKey(wap1.Key))
                            PlanetProduce[wap.Key].Add(wap1.Key, new long[10]);
                        PlanetProduce[wap.Key][wap1.Key][5] += wap1.Value;
                    }
                    else
                    {
                        if (!SumProduce.ContainsKey(wap1.Key))
                            SumProduce[wap1.Key] = new long[10];
                        SumProduce[wap1.Key][5] += wap1.Value;
                    }
                    
                }
            }
            
        }

        private void RefreshPlanet()
        {
            if (curtime - planetrefreshlasttime > 1)
                planetrefreshlasttime = curtime;
            else
                return;
            if (GameMain.galaxy == null) return;
            planetinfoshow.Clear();
            unloadPlanetNum = 0;
            foreach (StarData sd in GameMain.galaxy.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    unloadPlanetNum += pd.data == null && pd.factory == null ? 1 : 0;
                    if (PlanetorSum && !refreshPlanetinfo && !PlanetProduce.ContainsKey(pd.id)) continue;
                    if(PlanetorSum || refreshfactoryinfo)
                    {
                        bool condition = false;
                        if (pd.factory != null)
                        {
                            foreach (EntityData entity in pd.factory.entityPool)
                            {
                                int protoId = entity.protoId;
                                if (protoId <= 0) continue;
                                condition = true;
                                break;
                            }
                        }
                        if (!condition) continue;
                    }
                    if (filtercondition||refreshfactoryinfo)
                    {
                        bool condition = true;
                        if (!PlanetProduce.ContainsKey(pd.id) || PlanetProduce[pd.id].Count == 0) continue;
                        
                        List<int> temp = new List<int>(PlanetProduce[pd.id].Keys);
                        Dictionary<int, long[]> tempDiction = PlanetProduce[pd.id];
                        foreach (KeyValuePair<int, bool> wap in Productsearchcondition)
                        {
                            if (!wap.Value) continue;
                            if (!temp.Contains(wap.Key))
                            {
                                condition = false;
                            }
                            else
                            {
                                int itemId = wap.Key;
                                if ((itemId >= 1000 && itemId < 1030 || itemId == 1208) && !rawmaterial) condition = false;
                                if ((itemId > 1100 && itemId < 1200) && !secondrawmaterial) condition = false;
                                if (LDB.items.Select(itemId).CanBuild && !building) condition = false;
                                if (LDB.items.Select(itemId).recipes.Count > 0 && (itemId != 1003 && !(itemId > 1100 && itemId < 1200)) && LDB.items.Select(itemId).BuildIndex == 0 && !compound) condition = false;
                                if (noreachtheoryproduce && (tempDiction[itemId][0] >= tempDiction[itemId][2])) condition = false; 
                                if (noreachneedproduce && (tempDiction[itemId][0] >= tempDiction[itemId][3])) condition = false;
                                if (theorynoreachneedproduce && (tempDiction[itemId][2] >= tempDiction[itemId][3])) condition = false;
                                if (producelimittoggle && (tempDiction[itemId][0] <= producelowerlimit)) condition = false;
                                if (comsumeimittoggle && (tempDiction[itemId][1] <= comsumelowerlimit)) condition = false;
                                if (theoryproducelimittoggle && (tempDiction[itemId][2] <= theoryproducelowerlimit)) condition = false;
                                if (theorycomsumelimittoggle && (tempDiction[itemId][3] <= theorycomsumelowerlimit)) condition = false;
                            }
                            if (!condition) break;
                        }
                        if (!condition) continue;
                    }

                    bool flag = true;
                    long[] veinAmounts = new long[64];
                    pd.CalcVeinAmounts(ref veinAmounts,new HashSet<int>(), UIRoot.instance.uiGame.veinAmountDisplayFilter);
                    for (int i = 1; i <= 30; i++)
                    {
                        if (searchcondition_bool[i])
                        {
                            if (i <= 14) 
                            {
                                int[] planetsveinSpotsSketch = veinSpotsSketch(pd);
                                if (planetsveinSpotsSketch==null||planetsveinSpotsSketch[i] == 0)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (i == 15 && ((pd.gasItems == null && pd.orbitAroundPlanet == null) || (pd.gasItems != null && (pd.gasItems[0] != 1011 && pd.gasItems[1] != 1011)) || (pd.orbitAroundPlanet != null && (pd.orbitAroundPlanet.gasItems[0] != 1011 && pd.orbitAroundPlanet.gasItems[1] != 1011)))) { flag = false; break; }
                            if (i == 16 && ((pd.gasItems == null && pd.orbitAroundPlanet == null) || (pd.gasItems != null && (pd.gasItems[0] != 1120 && pd.gasItems[1] != 1120)) || (pd.orbitAroundPlanet != null && (pd.orbitAroundPlanet.gasItems[0] != 1120 && pd.orbitAroundPlanet.gasItems[1] != 1120)))) { flag = false; break; }
                            if (i == 17 && ((pd.gasItems == null && pd.orbitAroundPlanet == null) || (pd.gasItems != null && (pd.gasItems[0] != 1121 && pd.gasItems[1] != 1121)) || (pd.orbitAroundPlanet != null && (pd.orbitAroundPlanet.gasItems[0] != 1121 && pd.orbitAroundPlanet.gasItems[1] != 1121)))) { flag = false; break; }
                            if (i == 18 && pd.waterItemId != 1000) { flag = false; break; }
                            if (i == 19 && pd.waterItemId != 1116) { flag = false; break; }
                            if (i == 20 && pd.singularity != EPlanetSingularity.TidalLocked) { flag = false; break; }
                            if (i == 21 && pd.singularity != EPlanetSingularity.TidalLocked2) { flag = false; break; }
                            if (i == 22 && pd.singularity != EPlanetSingularity.TidalLocked4) { flag = false; break; }
                            if (i == 23 && pd.singularity != EPlanetSingularity.LaySide) { flag = false; break; }
                            if (i == 24 && pd.singularity != EPlanetSingularity.ClockwiseRotate) { flag = false; break; }
                            if (i == 25 && pd.singularity != EPlanetSingularity.MultipleSatellites) { flag = false; break; }
                            if (i == 26 && (!PlanetProduce.ContainsKey(pd.id) || PlanetProduce[pd.id].Count == 0)) { flag = false; break; }
                            if (i == 27 && PlanetProduce.ContainsKey(pd.id)) { flag = false; break; }
                            if (i == 28 && (!powerenergyinfoshow.ContainsKey(pd.id)||powerenergyinfoshow[pd.id][0]+ powerenergyinfoshow[pd.id][3]> powerenergyinfoshow[pd.id][1])) { flag = false; break; }
                            if (i == 29 && pd.data == null) { flag = false;break; }
                            if (i == 30 && pd.data != null) { flag = false; break; }
                        }
                    }
                    if (searchcondition_TypeName != "" && pd.typeString != searchcondition_TypeName) flag = false;
                    if (flag)
                        planetinfoshow.Add(pd.id);
                }
            }

            if (sortbypointproduct && filtercondition)
            {
                List<long> itemproduce = new List<long>();
                int pointitem = 0;
                foreach(KeyValuePair<int,bool> wap in Productsearchcondition)
                {
                    if (wap.Value)
                    {
                        pointitem = wap.Key;
                        break;
                    }
                }
                if (pointitem == 0) return;
                for(int i = 0; i < planetinfoshow.Count; i++)
                {
                    itemproduce.Add(PlanetProduce[planetinfoshow[i]][pointitem][0]);
                }
                for(int i = 0; i < itemproduce.Count; i++)
                {
                    int max = i;
                    for(int j = i + 1; j < itemproduce.Count; j++)
                    {
                        if (itemproduce[max] < itemproduce[j])
                        {
                            max = j;
                        }
                    }
                    if (max != i)
                    {
                        int pdindex = planetinfoshow[i];
                        long itemcout = itemproduce[i];
                        itemproduce[i] = itemproduce[max];
                        planetinfoshow[i] = planetinfoshow[max];
                        itemproduce[max] = itemcout;
                        planetinfoshow[max] = pdindex;
                    }
                }
            }

        }

        public static int[] veinSpotsSketch(PlanetData planet)
        {
            if(planet.factory!= null)
            {
                int[] result = new int[20];
                foreach(VeinData vd in planet.factory.veinPool)
                {
                    if (vd.id == 0) continue;
                    result[(int)vd.type]++;
                }
                return result;
            }
            ThemeProto themeProto = LDB.themes.Select(planet.theme);
            if (themeProto != null)
            {
                DotNet35Random dotNet35Random = new DotNet35Random(planet.seed);
                dotNet35Random.Next();
                dotNet35Random.Next();
                dotNet35Random.Next();
                dotNet35Random.Next();
                dotNet35Random.Next();
                dotNet35Random.Next();
                VeinProto[] veinProtos = PlanetModelingManager.veinProtos;
                int[] array = new int[veinProtos.Length];
                if (themeProto.VeinSpot != null)
                {
                    Array.Copy(themeProto.VeinSpot, 0, array, 1, Math.Min(themeProto.VeinSpot.Length, array.Length - 1));
                }
                float p = 1f;
                ESpectrType spectr = planet.star.spectr;
                EStarType type = planet.star.type;
                if (type == EStarType.MainSeqStar)
                {
                    if (spectr == ESpectrType.M)
                    {
                        p = 2.5f;
                    }
                    else if (spectr == ESpectrType.K)
                    {
                        p = 1f;
                    }
                    else if (spectr == ESpectrType.G)
                    {
                        p = 0.7f;
                    }
                    else if (spectr == ESpectrType.F)
                    {
                        p = 0.6f;
                    }
                    else if (spectr == ESpectrType.A)
                    {
                        p = 1f;
                    }
                    else if (spectr == ESpectrType.B)
                    {
                        p = 0.4f;
                    }
                    else if (spectr == ESpectrType.O)
                    {
                        p = 1.6f;
                    }
                }
                else if (type == EStarType.GiantStar)
                {
                    p = 2.5f;
                }
                else if (type == EStarType.WhiteDwarf)
                {
                    p = 3.5f;
                    array[9] += 2;
                    int num2 = 1;
                    while (num2 < 12 && dotNet35Random.NextDouble() < 0.44999998807907104)
                    {
                        array[9]++;
                        num2++;
                    }
                    array[10] += 2;
                    int num3 = 1;
                    while (num3 < 12 && dotNet35Random.NextDouble() < 0.44999998807907104)
                    {
                        array[10]++;
                        num3++;
                    }
                    array[12]++;
                    int num4 = 1;
                    while (num4 < 12 && dotNet35Random.NextDouble() < 0.5)
                    {
                        array[12]++;
                        num4++;
                    }
                }
                else if (type == EStarType.NeutronStar)
                {
                    p = 4.5f;
                    array[14]++;
                    int num5 = 1;
                    while (num5 < 12 && dotNet35Random.NextDouble() < 0.6499999761581421)
                    {
                        array[14]++;
                        num5++;
                    }
                }
                else if (type == EStarType.BlackHole)
                {
                    p = 5f;
                    array[14]++;
                    int num6 = 1;
                    while (num6 < 12 && dotNet35Random.NextDouble() < 0.6499999761581421)
                    {
                        array[14]++;
                        num6++;
                    }
                }
                for (int i = 0; i < themeProto.RareVeins.Length; i++)
                {
                    int num7 = themeProto.RareVeins[i];
                    float num8 = (planet.star.index == 0) ? themeProto.RareSettings[i * 4] : themeProto.RareSettings[i * 4 + 1];
                    num8 = 1f - Mathf.Pow(1f - num8, p);
                    if (dotNet35Random.NextDouble() < (double)num8)
                    {
                        array[num7]++;
                        int num12 = 1;
                        while (num12 < 12 && dotNet35Random.NextDouble() < themeProto.RareSettings[i * 4 + 2])
                        {
                            array[num7]++;
                            num12++;
                        }
                    }
                }
                return array;
            }
            return null;
        }

        private void RefreshFactory(int pdid)
        {
            factoryinfoshow = new Dictionary<int, int>();
            if (pdid == 0)
            {
                foreach (StarData sd in GameMain.galaxy.stars)
                {
                    foreach (PlanetData pd in sd.planets)
                    {
                        if (pd.factory == null) continue;
                        foreach (EntityData entity in pd.factory.entityPool)
                        {
                            int protoId = entity.protoId;
                            if (protoId <= 0) continue;
                            if (!factoryinfoshow.ContainsKey(protoId)) factoryinfoshow.Add(protoId, 0);
                            factoryinfoshow[protoId]++;
                        }
                    }
                }
            }
            else
            {
                PlanetData pd = GameMain.galaxy.PlanetById(pdid);
                if (pd.factory != null)
                {
                    foreach (EntityData entity in pd.factory.entityPool)
                    {
                        int protoId = entity.protoId;
                        if (protoId <= 0) continue;
                        if (!factoryinfoshow.ContainsKey(protoId)) factoryinfoshow.Add(protoId, 0);
                        factoryinfoshow[protoId]++;
                    }
                }
            }
        }

        //数量与单位之间的转化
        public string TGMKinttostring(double num, string unit="")
        {
            double tempcoreEnergyCap = num;
            int t = 0;
            if (num < 1000)
                return num + unit;
            while (tempcoreEnergyCap >= 1000)
            {
                t += 1;
                tempcoreEnergyCap /= 1000;
            }
            string coreEnergyCap = string.Format("{0:N2}", tempcoreEnergyCap);
            if (t == 0) return coreEnergyCap  + unit;
            if (t == 1) return coreEnergyCap + "K" + unit;
            if (t == 2) return coreEnergyCap + "M" + unit;
            if (t == 3) return coreEnergyCap + "G" + unit;
            if (t == 4) return coreEnergyCap + "T" + unit;
            if (t == 5) return coreEnergyCap + "P" + unit;
            if (t == 6) return coreEnergyCap + "E" + unit;

            return "";
        }

        public string Threeinttostring(double num)
        {
            long temp = (long)num;
            string result = "";
            if (temp < 1000) return temp + "";
            while (temp >= 1000)
            {
                if (result.Length == 0)
                    result = (temp % 1000).ToString().PadLeft(3, '0') + result;
                else
                    result = (temp % 1000).ToString().PadLeft(3,'0') + "," + result;
                temp /= 1000;
                if (temp > 1000)
                {
                    result = (temp % 1000).ToString().PadLeft(3, '0') + "," + result;
                    temp /= 1000;
                }
                else break;
            }
            if(temp>0) result = temp + "," + result;
            return result;
        }

        private int getVeinnumber(int itemid, int pdid)
        {
            int number = 0;
            EVeinType evt = itemidtoVeintype(itemid);
            PlanetData pd = GameMain.galaxy.PlanetById(pdid);
            long[] veinAmounts = new long[64];
            pd.CalcVeinAmounts(ref veinAmounts,new HashSet<int>(), UIRoot.instance.uiGame.veinAmountDisplayFilter);
            if (pd.gasItems != null) return 0;
            if (evt == EVeinType.Oil && veinAmounts != null)
            {
                int collectspeed = (int)(veinAmounts[7] * VeinData.oilSpeedMultiplier + 0.5);
                if (collectspeed > 1) return collectspeed;
            }
            if (pd == null || evt == EVeinType.None)
            {
                return 0;
            }
            if (pd.factory == null)
            {
                int[] planetveinSpotsSketch = veinSpotsSketch(pd);
                if (planetveinSpotsSketch != null)
                    return planetveinSpotsSketch[(int)itemidtoVeintype(itemid)];
                return 0;
            }
            
            foreach (VeinData vd in pd.factory.veinPool)
            {
                if (vd.type == evt)
                {
                    number++;
                }
            }
            return number;
        }

        private EVeinType itemidtoVeintype(int itemid)
        {
            switch (itemid)
            {
                case 1001: return EVeinType.Iron;
                case 1002: return EVeinType.Copper;
                case 1003: return EVeinType.Silicium;
                case 1004: return EVeinType.Titanium;
                case 1005: return EVeinType.Stone;
                case 1006: return EVeinType.Coal;
                case 1007: return EVeinType.Oil;
                case 1011: return EVeinType.Fireice;
                case 1012: return EVeinType.Diamond;
                case 1013: return EVeinType.Fractal;
                case 1117: return EVeinType.Crysrub;
                case 1014: return EVeinType.Grat;
                case 1015: return EVeinType.Bamboo;
                case 1016: return EVeinType.Mag;
            }
            return EVeinType.None;
        }
      
        private string VeintypechineseTranslate(int i, int type)
        {
            if (type == 0)
            {
                if (i == 1) return "铁矿";
                if (i == 2) return "铜矿";
                if (i == 3) return "硅矿";
                if (i == 4) return "钛矿";
                if (i == 5) return "石矿";
                if (i == 6) return "碳矿";
                if (i == 7) return "原油涌泉";
                if (i == 8) return "可燃冰";
                if (i == 9) return "金伯利矿石";
                if (i == 10) return "分形硅石";
                if (i == 11) return "有机晶体";
                if (i == 12) return "光栅石";
                if (i == 13) return "刺笋结晶";
                if (i == 14) return "单极磁矿";
            }
            else if (type == 1)
            {
                if (i == 1) return "1001";
                if (i == 2) return "1002";
                if (i == 3) return "1003";
                if (i == 4) return "1004";
                if (i == 5) return "1005";
                if (i == 6) return "1006";
                if (i == 7) return "1007";
                if (i == 8) return "1011";
                if (i == 9) return "1012";
                if (i == 10) return "1013";
                if (i == 11) return "1117";
                if (i == 12) return "1014";
                if (i == 13) return "1015";
                if (i == 14) return "1016";
            }

            return "";
        }

    }
}
