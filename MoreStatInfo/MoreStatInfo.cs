using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using static MoreStatInfo.GUIDraw;

namespace MoreStatInfo
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class MoreStatInfo : BaseUnityPlugin
    {
        public const string GUID = "cn.blacksnipe.dsp.MoreStatInfo";
        public const string NAME = "MoreStatInfo";
        public const string VERSION = "1.4.7";

        public static bool IsEnglish;
        public static bool OneSecondElapsed;
        public static ConfigEntry<int> scale;
        private static GUIDraw guiDraw;
        private static List<ItemProto> ItemList = new List<ItemProto>();
        private static Dictionary<int, Dictionary<int, long[]>> PlanetProduce = new Dictionary<int, Dictionary<int, long[]>>();
        private static Dictionary<int, int[]> PlanetVeinCountPool = new Dictionary<int, int[]>();
        private static Dictionary<int, Dictionary<int, int>> productIndices = new Dictionary<int, Dictionary<int, int>>();
        private static Dictionary<int, bool> Productsearchcondition = new Dictionary<int, bool>();
        private static ConfigEntry<KeyboardShortcut> ShowCounter1;
        private static Dictionary<int, long[]> SumProduce = new Dictionary<int, long[]>();
        private bool _filtercondition;
        private bool _multplanetproduct;
        private bool _PlanetorSum;
        private bool _refreshfactoryinfo;
        private bool _refreshPlanetinfo;
        private bool _RemoteorLocal;
        private bool _sortbyPlanetDataDis;
        private bool _sortbypointproduct;
        private bool _sortbyVeinCount;
        private bool _TGMKinttostringMode;
        private bool building = true;
        private bool compound = true;
        private bool comsumeimittoggle;
        private int comsumelowerlimit;
        private string comsumelowerlimitstr = "0";
        private bool dropdownbutton;
        private Dictionary<int, int> factoryinfoshow = new Dictionary<int, int>();
        private bool firstStart = true;
        private List<int> iteminfoshow = new List<int>();
        private int ItemInfoShowIndex = 1;
        private Dictionary<int, long[]> multPlanetProduce = new Dictionary<int, long[]>();
        private bool noreachneedproduce;
        private bool noreachtheoryproduce;
        private bool onlyseechose;
        private int pdselectinfoindex = 1;
        private Vector2 pdselectscrollPosition;
        private List<int> planetinfoshow = new List<int>();
        private List<int> planetsproductinfoshow = new List<int>();
        private List<string> PlanetType = new List<string>();
        private int pointPlanetId;
        private Dictionary<int, long[]> powerenergyinfoshow = new Dictionary<int, long[]>();
        private bool producelimittoggle;
        private int producelowerlimit;
        private string producelowerlimitstr = "0";
        private bool rawmaterial = true;
        private Vector2 scrollPosition;
        private bool[] searchcondition_bool = new bool[40];
        private string searchcondition_TypeName = "";
        private bool secondrawmaterial = true;
        private int selectitemId;
        private bool StopRefresh;
        private long[] sumpowerinfoshow = new long[5];
        private bool theorycomsumelimittoggle;
        private int theorycomsumelowerlimit;
        private string theorycomsumelowerlimitstr = "0";
        private bool theorynoreachneedproduce;
        private bool theoryproducelimittoggle;
        private int theoryproducelowerlimit;
        private string theoryproducelowerlimitstr = "0";
        private int unloadPlanetNum;
        private int windowmaxheight = 630;
        private int windowmaxwidth = 700;

        /// <summary>
        /// 筛选条件
        /// </summary>
        public bool Filtercondition
        {
            get => _filtercondition;
            set
            {
                if (value == _filtercondition) return;
                _filtercondition = value;
                if (value)
                {
                    RefreshPlanetinfo = false;
                    Refreshfactoryinfo = false;
                }
            }
        }

        /// <summary>
        /// 多选星球
        /// </summary>
        public bool Multplanetproduct
        {
            get => _multplanetproduct;
            set
            {
                if (value == _multplanetproduct) return;
                _multplanetproduct = value;
                if (value)
                {
                    Refreshfactoryinfo = false;
                    RefreshPlanetinfo = false;
                    PlanetorSum = true;
                }
            }
        }

        /// <summary>
        /// 全部/星球
        /// </summary>
        public bool PlanetorSum
        {
            get => _PlanetorSum;
            set
            {
                if (value == _PlanetorSum) return;
                _PlanetorSum = value;
                if (!value)
                {
                    pointPlanetId = 0;
                }
                RefreshProductStat();
            }
        }

        /// <summary>
        /// 工厂信息
        /// </summary>
        public bool Refreshfactoryinfo
        {
            get => _refreshfactoryinfo;
            set
            {
                if (value == _refreshfactoryinfo) return;
                _refreshfactoryinfo = value;
                if (value)
                {
                    Filtercondition = false;
                    RefreshPlanetinfo = false;
                    Multplanetproduct = false;
                    RefreshFactory(pointPlanetId);
                }
            }
        }

        /// <summary>
        /// 星球信息
        /// </summary>
        public bool RefreshPlanetinfo
        {
            get => _refreshPlanetinfo;
            set
            {
                if (value == _refreshPlanetinfo) return;
                _refreshPlanetinfo = value;
                if (value)
                {
                    Refreshfactoryinfo = false;
                    Filtercondition = false;
                    Multplanetproduct = false;
                }
            }
        }

        /// <summary>
        /// 本地/远程
        /// </summary>
        public bool RemoteorLocal
        {
            get => _RemoteorLocal;
            set
            {
                if (value == _RemoteorLocal) return;
                _RemoteorLocal = value;
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
        }

        /// <summary>
        /// 依目标产物排序
        /// </summary>
        public bool SortbyPointProduct
        {
            get => _sortbypointproduct;
            set
            {
                if (value == _sortbypointproduct) return;
                _sortbypointproduct = value;
                if (value)
                {
                    for (int j = 0; j < ItemList.Count; j++)
                        Productsearchcondition[ItemList[j].ID] = false;
                    if (selectitemId > 0)
                        Productsearchcondition[selectitemId] = true;
                }
            }
        }

        /// <summary>
        /// 单位转化
        /// </summary>
        public bool TGMKinttostringMode
        {
            get => _TGMKinttostringMode;
            set
            {
                if (value == _TGMKinttostringMode) return;
                _TGMKinttostringMode = value;
                if (TGMKinttostringMode)
                {
                    styleboldblue.fontSize = 20;
                    styleyellow.fontSize = 20;
                }
                else
                {
                    styleboldblue.fontSize = 15;
                    styleyellow.fontSize = 15;
                }
            }
        }

        /// <summary>
        /// 获取目标星球的矿物数据
        /// </summary>
        /// <param name="planet"></param>
        /// <returns></returns>
        public static int[] veinSpotsSketch(PlanetData planet)
        {
            if (PlanetVeinCountPool.ContainsKey(planet.id))
            {
                return PlanetVeinCountPool[planet.id];
            }
            if (planet.factory != null)
            {
                int[] result = new int[20];
                foreach (VeinData vd in planet.factory.veinPool)
                {
                    if (vd.id == 0) continue;
                    result[(int)vd.type]++;
                }
                PlanetVeinCountPool.Add(planet.id, result);
                return result;
            }
            else if (planet.data?.veinPool != null)
            {
                var rawdata = planet.data;
                int[] result = new int[20];
                foreach (VeinData vd in rawdata.veinPool)
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
                switch (type)
                {
                    case EStarType.MainSeqStar:
                        switch (spectr)
                        {
                            case ESpectrType.M:
                                p = 2.5f;
                                break;

                            case ESpectrType.K:
                                p = 1f;
                                break;

                            case ESpectrType.G:
                                p = 0.7f;
                                break;

                            case ESpectrType.F:
                                p = 0.6f;
                                break;

                            case ESpectrType.A:
                                p = 1f;
                                break;

                            case ESpectrType.B:
                                p = 0.4f;
                                break;

                            case ESpectrType.O:
                                p = 1.6f;
                                break;
                        }
                        break;

                    case EStarType.GiantStar:
                        p = 2.5f;
                        break;

                    case EStarType.WhiteDwarf:
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
                        break;

                    case EStarType.NeutronStar:
                        p = 4.5f;
                        array[14]++;
                        int num5 = 1;
                        while (num5 < 12 && dotNet35Random.NextDouble() < 0.6499999761581421)
                        {
                            array[14]++;
                            num5++;
                        }
                        break;

                    case EStarType.BlackHole:
                        p = 5f;
                        array[14]++;
                        int num6 = 1;
                        while (num6 < 12 && dotNet35Random.NextDouble() < 0.6499999761581421)
                        {
                            array[14]++;
                            num6++;
                        }
                        break;
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
                PlanetVeinCountPool.Add(planet.id, array);
                return array;
            }
            return null;
        }

        /// <summary>
        /// 主面板
        /// </summary>
        /// <param name="id"></param>
        public void MainWindow(int id)
        {
            GUILayout.BeginArea(new Rect(10, 20, MainWindowWidth, MainWindowHeight));
            int x = 0;
            int y = 0;
            scrollPosition = GUI.BeginScrollView(new Rect(0, 0, MainWindowWidth - 20, MainWindowHeight - 30), scrollPosition, new Rect(0, 0, windowmaxwidth, windowmaxheight));

            if (Refreshfactoryinfo && factoryinfoshow != null && factoryinfoshow.Count > 0)
            {
                foreach (KeyValuePair<int, int> wap in factoryinfoshow)
                {
                    ItemProto item = LDB.items.Select(wap.Key);
                    GUI.Button(new Rect(x, y, heightdis * 2, heightdis * 2), item.iconSprite.texture, guiDraw.emptyStyle);
                    GUI.Label(AddRect(ref x, y + heightdis * 2, heightdis * 2, heightdis), wap.Value + "", styleboldblue);
                    if (x > MainWindowWidth - 10 - heightdis * 2)
                    {
                        x = 0;
                        y += heightdis * 4;
                    }
                }
                y += heightdis * 4;
                string[] PowerInfo = new string[5]
                {
                    "发电性能".getTranslate() + ":",
                    "耗电需求".getTranslate() + ":",
                    "总耗能".getTranslate() + ":",
                    "放电功率".getTranslate() + ":",
                    "充电功率".getTranslate() + ":"
                };
                for (int i = 0; i < 5; i++)
                {
                    string unit = i == 2 ? "J" : "W";
                    double num = (RefreshPlanetinfo || PlanetorSum) && pointPlanetId > 0 && powerenergyinfoshow.ContainsKey(pointPlanetId) ? powerenergyinfoshow[pointPlanetId][i] : sumpowerinfoshow[i];
                    PowerInfo[i] += TGMKinttostringMode ? TGMKinttostring(num, unit) : Threeinttostring(num) + unit;
                    GUI.Label(AddRect(10, ref y, 200, heightdis), PowerInfo[i], styleboldblue);
                }
                windowmaxwidth = (int)MainWindowWidth - 10;
            }
            else if (RefreshPlanetinfo)
            {
                var iconoptions = new[] { GUILayout.Width(heightdis), GUILayout.Height(heightdis) };
                GUILayout.BeginHorizontal();
                //筛选条件列表
                {
                    //星球属性筛选条件
                    {
                        GUILayout.BeginVertical();
                        GUILayout.Label("附属气态星产物".getTranslate());
                        for (int i = 15; i <= 17; i++)
                            searchcondition_bool[i] = GUILayout.Toggle(searchcondition_bool[i], searchchineseTranslate(i));

                        GUILayout.Label("星球特殊性".getTranslate());
                        for (int i = 18; i <= 25; i++)
                            searchcondition_bool[i] = GUILayout.Toggle(searchcondition_bool[i], searchchineseTranslate(i));

                        if (searchcondition_bool[26] != GUILayout.Toggle(searchcondition_bool[26], "具有工厂".getTranslate()))
                        {
                            searchcondition_bool[26] = !searchcondition_bool[26];
                            if (searchcondition_bool[26])
                                searchcondition_bool[27] = false;
                        }
                        if (searchcondition_bool[27] != GUILayout.Toggle(searchcondition_bool[27], "不具有工厂".getTranslate()))
                        {
                            searchcondition_bool[27] = !searchcondition_bool[27];
                            if (searchcondition_bool[27])
                                searchcondition_bool[26] = false;
                        }
                        if (searchcondition_bool[28] != GUILayout.Toggle(searchcondition_bool[28], "电力不足".getTranslate()))
                        {
                            searchcondition_bool[28] = !searchcondition_bool[28];
                            if (searchcondition_bool[28])
                                searchcondition_bool[26] = true;
                        }
                        if (searchcondition_bool[29] != GUILayout.Toggle(searchcondition_bool[29], "已加载星球".getTranslate()))
                        {
                            searchcondition_bool[29] = !searchcondition_bool[29];
                            if (searchcondition_bool[29])
                                searchcondition_bool[30] = false;
                        }
                        if (searchcondition_bool[30] != GUILayout.Toggle(searchcondition_bool[30], "未加载星球".getTranslate()))
                        {
                            searchcondition_bool[30] = !searchcondition_bool[30];
                            if (searchcondition_bool[30])
                                searchcondition_bool[29] = false;
                        }
                        GUILayout.EndVertical();
                    }

                    //矿物筛选条件
                    {
                        GUILayout.BeginVertical();
                        GUILayout.Label("目标星球矿物".getTranslate());
                        for (int i = 1; i <= 14; i++)
                            searchcondition_bool[i] = GUILayout.Toggle(searchcondition_bool[i], searchchineseTranslate(i));
                        _sortbyPlanetDataDis = GUILayout.Toggle(_sortbyPlanetDataDis, "依照离玩家距离排序".getTranslate());
                        _sortbyVeinCount = GUILayout.Toggle(_sortbyVeinCount, "依矿脉数量排序".getTranslate());
                        if (GUILayout.Button("取消所有条件".getTranslate()))
                        {
                            for (int i = searchcondition_bool.Length - 1; i >= 0; i--)
                            {
                                searchcondition_bool[i] = false;
                            }
                        }
                        GUILayout.Label("未加载星球".getTranslate() + $"{unloadPlanetNum}");
                        if (GUILayout.Button("加载全部星球".getTranslate()))
                        {
                            LoadAllPlanet();
                        }
                        if (PlanetModelingManager.calPlanetReqList.Count > 0)
                        {
                            GUILayout.Label($"{PlanetModelingManager.calPlanetReqList.Count}" + "个星球加载中".getTranslate());
                            GUILayout.Label("请勿切换存档".getTranslate());
                        }
                        GUILayout.EndVertical();
                    }

                    //星球类型筛选条件
                    {
                        GUILayout.BeginVertical();
                        GUILayout.Label("星球类型".getTranslate());
                        if (GUILayout.Button(searchcondition_TypeName == "" ? "未选择".getTranslate() : searchcondition_TypeName))
                        {
                            dropdownbutton = !dropdownbutton;
                        }
                        if (dropdownbutton)
                        {
                            if (GUILayout.Button("取消选择"))
                            {
                                dropdownbutton = !dropdownbutton;
                                searchcondition_TypeName = "";
                            }
                            for (int i = 0; i < PlanetType.Count; i++)
                            {
                                if (GUILayout.Button(PlanetType[i]))
                                {
                                    dropdownbutton = !dropdownbutton;
                                    searchcondition_TypeName = PlanetType[i];
                                }
                            }
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.Space(heightdis);

                //星球信息
                PlanetData pd;
                if (pointPlanetId > 0 && (pd = GameMain.galaxy.PlanetById(pointPlanetId)) != null)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("行星信息".getTranslate() + ":", styleboldblue);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.BeginVertical();
                    string pdname = GUILayout.TextArea(pd.displayName);
                    if (!pdname.Equals(pd.displayName))
                    {
                        pd.overrideName = pdname;
                        pd.NotifyOnDisplayNameChange();
                    }
                    if (pd.gasItems == null)
                    {
                        string waterType = GetWaterTypeText(pd.waterItemId);
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("海洋类型".getTranslate() + ":", stylenormalblue);
                        GUILayout.Space(20);
                        GUILayout.Label(waterType, stylenormalblue);
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("星球类型".getTranslate() + ":", stylenormalblue);
                        GUILayout.Space(20);
                        GUILayout.Label(pd.typeString, stylenormalblue);
                        GUILayout.EndHorizontal();
                        if (pd.singularityString.Length > 0)
                        {
                            GUILayout.Space(5);
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("星球特殊性".getTranslate() + ": ", stylenormalblue);
                            GUILayout.Space(20);
                            GUILayout.Label(pd.singularityString, stylenormalblue);
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("工厂状态".getTranslate() + ":", stylenormalblue);
                    GUILayout.Space(20);
                    GUILayout.Label(PlanetProduce.ContainsKey(pd.id) ? "有".getTranslate() : "无".getTranslate(), stylenormalblue);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("距离玩家".getTranslate() + ":", stylenormalblue);
                    GUILayout.Space(20);
                    GUILayout.Label(PlayerToPlanetDisTranslate(pd), stylenormalblue);
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("方向指引".getTranslate()))
                    {
                        GameMain.mainPlayer.navigation._indicatorAstroId = pd.id;
                    }
                    GUILayout.Space(20);
                    if (pd.gasItems != null)
                    {
                        for (int i = 0; i < pd.gasItems.Length; i++)
                        {
                            ItemProto item = LDB.items.Select(pd.gasItems[i]);
                            GUILayout.BeginHorizontal();
                            GUILayout.Button(item.iconSprite.texture, guiDraw.emptyStyle, iconoptions);
                            GUILayout.Label(item.name + ":" + string.Format("{0:N2}", pd.gasSpeeds[i]), stylenormalblue);
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        long[] veinAmounts = new long[64];
                        pd.CalcVeinAmounts(ref veinAmounts, new HashSet<int>(), UIRoot.instance.uiGame.veinAmountDisplayFilter);
                        int[] planetveinSpotsSketch = veinSpotsSketch(pd);
                        for (int k = 1; planetveinSpotsSketch != null && k <= 14; k++)
                        {
                            if (planetveinSpotsSketch[k] == 0) continue;
                            long i = veinAmounts[k];

                            var item = LDB.items.Select(LDB.veins.Select(k).MiningItem);
                            GUILayout.BeginHorizontal();
                            GUILayout.Button(item.iconSprite.texture, guiDraw.emptyStyle, iconoptions);
                            GUILayout.Label(item.name + TGMKinttostring(i) + ("(" + planetveinSpotsSketch[k] + ")"), stylenormalblue);
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    GUILayout.Space(20);

                    GUILayout.BeginVertical();

                    {
                        GUILayout.BeginVertical();
                        if (pd.orbitAroundPlanet != null)
                        {
                            GUILayout.Label("围绕行星".getTranslate() + ":", styleboldblue);
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();
                            pdname = GUILayout.TextArea(pd.orbitAroundPlanet.displayName);
                            if (!pdname.Equals(pd.orbitAroundPlanet.displayName))
                            {
                                pd.orbitAroundPlanet.overrideName = pdname;
                                pd.orbitAroundPlanet.NotifyOnDisplayNameChange();
                            }
                            for (int i = 0; i < pd.orbitAroundPlanet.gasItems.Length; i++)
                            {
                                ItemProto item = LDB.items.Select(pd.orbitAroundPlanet.gasItems[i]);
                                GUILayout.BeginHorizontal();
                                GUILayout.Button(item.iconSprite.texture, guiDraw.emptyStyle, iconoptions);
                                GUILayout.Label(item.name + ":" + string.Format("{0:N2}", pd.orbitAroundPlanet.gasSpeeds[i]), stylenormalblue);
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                            GUILayout.Space(heightdis);
                        }

                        {
                            GUILayout.Label("恒星信息".getTranslate() + ":", styleboldblue);
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();
                            StarData sd = pd.star;
                            string starname = GUILayout.TextArea(sd.displayName);
                            if (!starname.Equals(sd.displayName))
                            {
                                sd.overrideName = starname;
                                sd.NotifyOnDisplayNameChange();
                            }
                            GUILayout.Space(5);
                            GUILayout.Label(sd.typeString, stylenormalblue);
                            GUILayout.Space(5);
                            GUILayout.Label("恒星亮度".getTranslate() + ":" + sd.dysonLumino + "", stylenormalblue);
                            GUILayout.Space(5);
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }

                    GUILayout.EndVertical();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                windowmaxwidth = (int)MainWindowWidth - 30;
            }
            else
            {
                // 获取生产数据字典
                var tempDiction = Multplanetproduct
                    ? multPlanetProduce
                    : PlanetorSum && PlanetProduce != null && PlanetProduce.ContainsKey(pointPlanetId)
                        ? PlanetProduce[pointPlanetId]
                        : SumProduce;

                // 设置列宽度
                int[] ColumnWidth = new int[11]
                {
                    IsEnglish ? heightdis * 8 : heightdis * 4,
                    heightdis * 4,
                    heightdis * 4,
                    heightdis * 4,
                    heightdis * 4,
                    TGMKinttostringMode ? heightdis * 4 : heightdis * 2,
                    TGMKinttostringMode ? heightdis * 4 : heightdis * 2,
                    heightdis * 4,
                    heightdis * 4,
                    heightdis * 4,
                    heightdis * 4,
                };

                // 计算窗口最大宽度
                windowmaxwidth = ColumnWidth.Sum();

                // 绘制列按钮
                for (int i = 0; i <= 10; i++)
                {
                    var buttonStyle = i % 2 == 0 ? buttonstyleyellow : buttonstyleblue;
                    if (GUI.Button(AddRect(ref x, y, ColumnWidth[i], heightdis), columnsbuttonstr[i].getTranslate(), buttonStyle))
                        ChangeSort(i);
                }

                x = 0;
                y += heightdis;

                // 绘制物品信息
                foreach (var itemInfo in iteminfoshow.Skip((ItemInfoShowIndex - 1) * 20).Take(20))
                {
                    if (!tempDiction.ContainsKey(itemInfo)) continue;

                    var item = LDB.items.Select(itemInfo);
                    var itemID = item.ID;

                    GUI.Button(AddRect(ref x, y, heightdis, heightdis), item.iconSprite.texture, guiDraw.emptyStyle);
                    GUI.Label(AddRect(ref x, y, ColumnWidth[0] - heightdis, heightdis), item.name, styleitemname);
                    for (int j = 0; j < 10; j++)
                    {
                        var labelStyle = j % 2 == 0 ? styleboldblue : styleyellow;
                        string number = TGMKinttostringMode ? TGMKinttostring(tempDiction[itemID][j]) : tempDiction[itemID][j].ToString();
                        GUI.Label(AddRect(ref x, y, ColumnWidth[j + 1], heightdis), number, labelStyle);
                    }

                    x = 0;
                    y += heightdis;
                }

                // 更新和绘制分页按钮
                int indexnum = (int)(iteminfoshow.Count / 20.5f) + 1;
                ItemInfoShowIndex = Math.Min(ItemInfoShowIndex, indexnum);

                x = 0;
                for (int i = 1; i <= indexnum && indexnum > 1; i++)
                {
                    if (i == ItemInfoShowIndex)
                    {
                        GUIStyle style = new GUIStyle(GUI.skin.button) { normal = { textColor = new Color32(215, 186, 245, 255) } };
                        if (GUI.Button(AddRect(ref x, y, heightdis, heightdis), i + "", style))
                            ItemInfoShowIndex = i;
                    }
                    else
                    {
                        if (GUI.Button(AddRect(ref x, y, heightdis, heightdis), i + ""))
                            ItemInfoShowIndex = i;
                    }
                }
                y += heightdis;
            }

            if (Filtercondition)
            {
                FilterUI(ref y);
            }

            windowmaxheight = y + heightdis;
            GUI.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// 根据规则排序后的结果
        /// </summary>
        /// <param name="itemsmap"></param>
        /// <param name="columnnum"></param>
        /// <param name="sortrules"></param>
        /// <returns></returns>
        public Dictionary<int, long[]> sortItem(Dictionary<int, long[]> itemsmap, int columnnum, int sortrules)
        {
            if (sortrules == 0) return itemsmap;
            Dictionary<int, long[]> result = new Dictionary<int, long[]>();
            if (columnnum == 0)
            {
                List<int> itemids = new List<int>(itemsmap.Keys);
                itemids.Sort();
                if (sortrules == -1)
                    itemids.Reverse();
                for (int i = 0; i < itemids.Count; i++)
                    result.Add(itemids[i], itemsmap[itemids[i]]);

                return result;
            }
            columnnum--;
            Dictionary<int, long[]> tempitemsmap = new Dictionary<int, long[]>(itemsmap);
            List<int> temp = new List<int>();
            long max;
            int tempid;
            int itemsmapcount = itemsmap.Count;
            for (int i = 0; i < itemsmapcount; i++)
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

        /// <summary>
        /// 数量与单位之间的转化
        /// </summary>
        /// <param name="num"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public string TGMKinttostring(double num, string unit = "")
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
            if (t == 0) return coreEnergyCap + unit;
            if (t == 1) return coreEnergyCap + "K" + unit;
            if (t == 2) return coreEnergyCap + "M" + unit;
            if (t == 3) return coreEnergyCap + "G" + unit;
            if (t == 4) return coreEnergyCap + "T" + unit;
            if (t == 5) return coreEnergyCap + "P" + unit;
            if (t == 6) return coreEnergyCap + "E" + unit;

            return "";
        }

        /// <summary>
        /// 每隔三位数加符号","
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
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
                    result = (temp % 1000).ToString().PadLeft(3, '0') + "," + result;
                temp /= 1000;
                if (temp > 1000)
                {
                    result = (temp % 1000).ToString().PadLeft(3, '0') + "," + result;
                    temp /= 1000;
                }
                else break;
            }
            if (temp > 0) result = temp + "," + result;
            return result;
        }

        /// <summary>
        /// 根据物品ID来判断条件
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="tempDiction"></param>
        /// <returns></returns>
        private bool CheckItemConditions(int itemId, Dictionary<int, long[]> tempDiction)
        {
            if ((itemId >= 1000 && itemId < 1030 || itemId == 1208) && !rawmaterial) return false;
            if ((itemId > 1100 && itemId < 1200) && !secondrawmaterial) return false;
            if (LDB.items.Select(itemId).CanBuild && !building) return false;
            if (LDB.items.Select(itemId).recipes.Count > 0 && (itemId != 1003 && !(itemId > 1100 && itemId < 1200)) && LDB.items.Select(itemId).BuildIndex == 0 && !compound) return false;
            if (noreachtheoryproduce && (tempDiction[itemId][0] >= tempDiction[itemId][2])) return false;
            if (noreachneedproduce && (tempDiction[itemId][0] >= tempDiction[itemId][3])) return false;
            if (theorynoreachneedproduce && (tempDiction[itemId][2] >= tempDiction[itemId][3])) return false;
            if (producelimittoggle && (tempDiction[itemId][0] <= producelowerlimit)) return false;
            if (comsumeimittoggle && (tempDiction[itemId][1] <= comsumelowerlimit)) return false;
            if (theoryproducelimittoggle && (tempDiction[itemId][2] <= theoryproducelowerlimit)) return false;
            if (theorycomsumelimittoggle && (tempDiction[itemId][3] <= theorycomsumelowerlimit)) return false;

            return true;
        }

        /// <summary>
        /// 根据星球判断产物条件
        /// </summary>
        /// <param name="pdid"></param>
        /// <returns></returns>
        private bool CheckProductCondition(int pdid)
        {
            return Productsearchcondition
                    .Where(wap => wap.Value)
                    .All(wap => PlanetProduce[pdid].Keys.Contains(wap.Key) && CheckItemConditions(wap.Key, PlanetProduce[pdid]));
        }

        // 定义ClampString方法，将输入字符串的长度限制在指定的范围内
        private string ClampString(string str, int minLength, int maxLength)
        {
            str = str.Length < minLength ? new string('0', minLength) : str;
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        /// <summary>
        /// 筛选条件UI
        /// </summary>
        /// <param name="tempheight1"></param>
        private void FilterUI(ref int y)
        {
            float x = 0;

            GUIStyle toggleStyle = GUI.skin.toggle;
            GUI.Label(AddRect(ref x, y, toggleStyle.CalcSize(new GUIContent("产物筛选".getTranslate())).x, heightdis), "产物筛选".getTranslate());
            string[] toggleLabels = new string[]
            {
                "一级原料",
                "二级原料",
                "建筑",
                "合成材料",
                "只看目标产物",
                "依目标产物排序",
                "实时产量<理论产量",
                "实时产量<需求产量",
                "理论产量<需求产量"
            };

            bool[] toggleValues = new bool[]
            {
                rawmaterial,
                secondrawmaterial,
                building,
                compound,
                onlyseechose,
                SortbyPointProduct,
                noreachtheoryproduce,
                noreachneedproduce,
                theorynoreachneedproduce
            };

            for (int i = 0; i < toggleLabels.Length; i++)
            {
                var showname = toggleLabels[i].getTranslate();
                float width = toggleStyle.CalcSize(new GUIContent(showname)).x;
                if (x + width + 5 > windowmaxwidth || i == 6)
                {
                    x = 0;
                    y += heightdis;
                }
                toggleValues[i] = GUI.Toggle(AddRect(ref x, y, width + 5, heightdis), toggleValues[i], showname);
            }

            // 更新变量
            rawmaterial = toggleValues[0];
            secondrawmaterial = toggleValues[1];
            building = toggleValues[2];
            compound = toggleValues[3];
            onlyseechose = toggleValues[4];
            SortbyPointProduct = toggleValues[5];
            noreachtheoryproduce = toggleValues[6];
            noreachneedproduce = toggleValues[7];
            theorynoreachneedproduce = toggleValues[8];

            x = 0;
            y += heightdis;

            // 定义需要创建的Toggle和TextArea的标签和变量
            toggleLabels = new string[] { "实时产量", "实时消耗", "需求产量", "理论产量" };
            toggleValues = new bool[] { producelimittoggle, comsumeimittoggle, theorycomsumelimittoggle, theoryproducelimittoggle };
            string[] textAreas = new string[] { producelowerlimitstr, comsumelowerlimitstr, theorycomsumelowerlimitstr, theoryproducelowerlimitstr };

            // 循环创建Toggle和TextArea，并将其存储在数组中
            for (int i = 0; i < toggleLabels.Length; i++)
            {
                var showname = toggleLabels[i].getTranslate() + ":";
                bool toggleValue = toggleValues[i];
                string textArea = textAreas[i];

                // 计算Toggle文本像素大小
                Vector2 labelSize = toggleStyle.CalcSize(new GUIContent(showname));

                // 创建Toggle
                toggleValues[i] = GUI.Toggle(AddRect(ref x, y, labelSize.x + 5, heightdis), toggleValue, showname, toggleStyle);

                // 创建TextArea
                textAreas[i] = GUI.TextArea(AddRect(ref x, y, heightdis * 4, heightdis), textArea + "/min");
            }

            // 更新变量
            producelowerlimitstr = Regex.Replace(textAreas[0], @"[^0-9]", "");
            comsumelowerlimitstr = Regex.Replace(textAreas[1], @"[^0-9]", "");
            theorycomsumelowerlimitstr = Regex.Replace(textAreas[2], @"[^0-9]", "");
            theoryproducelowerlimitstr = Regex.Replace(textAreas[3], @"[^0-9]", "");

            producelowerlimitstr = ClampString(producelowerlimitstr, 0, 9);
            comsumelowerlimitstr = ClampString(comsumelowerlimitstr, 0, 9);
            theorycomsumelowerlimitstr = ClampString(theorycomsumelowerlimitstr, 0, 9);
            theoryproducelowerlimitstr = ClampString(theoryproducelowerlimitstr, 0, 9);

            int.TryParse(comsumelowerlimitstr, out comsumelowerlimit);
            int.TryParse(theorycomsumelowerlimitstr, out theorycomsumelowerlimit);
            int.TryParse(theoryproducelowerlimitstr, out theoryproducelowerlimit);
            int.TryParse(producelowerlimitstr, out producelowerlimit);

            if (!SequenceEqual(toggleValues, new bool[] { producelimittoggle, comsumeimittoggle, theorycomsumelimittoggle, theoryproducelimittoggle }))
            {
                producelimittoggle = toggleValues[0];
                comsumeimittoggle = toggleValues[1];
                theorycomsumelimittoggle = toggleValues[2];
                theoryproducelimittoggle = toggleValues[3];
            }

            x = 0;
            y += heightdis;
            foreach (var item in ItemList)
            {
                int itemid = item.ID;
                if (!Productsearchcondition.ContainsKey(itemid))
                {
                    Productsearchcondition.Add(itemid, false);
                }

                if (itemid == 1030 || itemid == 1031)
                {
                    continue;
                }

                if ((itemid >= 1000 && itemid < 1030 || itemid == 1208) && !rawmaterial)
                {
                    continue;
                }

                if ((itemid > 1100 && itemid < 1200) && !secondrawmaterial)
                {
                    continue;
                }

                if (item.CanBuild && !building)
                {
                    continue;
                }

                if (item.recipes.Count > 0 && (itemid != 1003 && !(itemid > 1100 && itemid < 1200)) && item.BuildIndex == 0 && !compound)
                {
                    continue;
                }

                GUIStyle style = guiDraw.emptyStyle;

                if (Productsearchcondition[itemid])
                {
                    style = guiDraw.whiteStyle;
                }

                if (GUI.Button(AddRect(ref x, y, heightdis, heightdis), item.iconSprite.texture, style))
                {
                    Productsearchcondition[itemid] = !Productsearchcondition[itemid];

                    if (SortbyPointProduct)
                    {
                        if (Productsearchcondition[itemid])
                        {
                            selectitemId = itemid;

                            var keys = Productsearchcondition.Keys.ToList();
                            foreach (var key in keys)
                            {
                                Productsearchcondition[key] = false;
                            }

                            Productsearchcondition[itemid] = true;
                        }
                    }
                }

                if (x > MainWindowWidth - heightdis * 2)
                {
                    y += heightdis;
                    x = 0;
                }
            }
        }

        /// <summary>
        /// 获取目标星球目标矿物簇数
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="pdid"></param>
        /// <returns></returns>
        private int getVeinnumber(int itemid, int pdid)
        {
            int number = 0;
            EVeinType evt = LDB.veins.GetVeinTypeByItemId(itemid);
            PlanetData pd = GameMain.galaxy.PlanetById(pdid);
            long[] veinAmounts = new long[64];
            pd.CalcVeinAmounts(ref veinAmounts, new HashSet<int>(), UIRoot.instance.uiGame.veinAmountDisplayFilter);
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
                    return planetveinSpotsSketch[(int)LDB.veins.GetVeinTypeByItemId(itemid)];
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

        private string GetWaterTypeText(int waterItemId)
        {
            string waterType = "无".getTranslate();
            if (waterItemId < 0)
            {
                switch (waterItemId)
                {
                    case -2:
                        waterType = "冰".Translate();
                        break;

                    case -1:
                        waterType = "熔岩".Translate();
                        break;

                    default:
                        waterType = "未知".Translate();
                        break;
                }
            }
            else
            {
                var wateritem = LDB.items.Select(waterItemId);
                if (wateritem != null)
                    waterType = wateritem.name;
            }
            return waterType;
        }

        /// <summary>
        /// 加载全部星球
        /// </summary>
        private void LoadAllPlanet()
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

        /// <summary>
        /// 主面板GUI逻辑
        /// </summary>
        private void MainWindowShowFun()
        {
            IsEnglish = Localization.CurrentLanguage.glyph == 0;
            MoveWindow();
            Scaling_Window();
            GUI.DrawTexture(GUI.Window(20210821, GetMainWindowRect(), MainWindow, "统计面板".getTranslate() + "(" + VERSION + ")" + "ps:ctrl+↑↓"), mytexture);
        }

        void OnGUI()
        {
            if (!guiDraw.ShowGUIWindow)
            {
                return;
            }
            guiDraw.Draw();
            RefreshAll();
            MainWindowShowFun();
            SwitchWindowShowFun();
            RefreshPlanet();
            PlanetWindowShowFun();
            if (OneSecondElapsed)
            {
                OneSecondElapsed = false;
            }
        }

        /// <summary>
        /// 星球面板
        /// </summary>
        /// <param name="id"></param>
        private void PlanetWindow(int id)
        {
            GUILayout.BeginArea(new Rect(0, 20, heightdis * 8 + 10, heightdis * 21));

            for (int i = (pdselectinfoindex - 1) * 20; i < pdselectinfoindex * 20 && i < planetinfoshow.Count; i++)
            {
                GUIStyle style = normalPlanetButtonStyle;
                if (!Multplanetproduct)
                {
                    if (pointPlanetId == planetinfoshow[i])
                        style = selectedPlanetButtonStyle;
                    if (GUI.Button(new Rect(5, (i - (pdselectinfoindex - 1) * 20) * heightdis, heightdis * 8, heightdis), GameMain.galaxy.PlanetById(planetinfoshow[i]).displayName, style))
                    {
                        pointPlanetId = planetinfoshow[i];
                        RefreshFactory(pointPlanetId);
                    }
                }
                else
                {
                    if (planetsproductinfoshow.Contains(planetinfoshow[i]))
                        style = selectedPlanetButtonStyle;
                    if (GUI.Button(new Rect(5, (i - (pdselectinfoindex - 1) * heightdis) * heightdis, heightdis * 8, heightdis), GameMain.galaxy.PlanetById(planetinfoshow[i]).displayName, style))
                    {
                        if (planetsproductinfoshow.Contains(planetinfoshow[i]))
                            planetsproductinfoshow.Remove(planetinfoshow[i]);
                        else
                            planetsproductinfoshow.Add(planetinfoshow[i]);
                    }
                }
            }

            int indexnum = planetinfoshow.Count / 20 + (planetinfoshow.Count % 20 == 0 ? 0 : 1);
            int tempwidth = 0;
            for (int i = pdselectinfoindex > 3 ? pdselectinfoindex - 2 : 1; i <= indexnum; i++)
            {
                if (GUI.Button(new Rect(heightdis * tempwidth++, 20 * heightdis, heightdis, heightdis), i + "", i == pdselectinfoindex ? selectedPlanetButtonStyle : normalPlanetButtonStyle))
                    pdselectinfoindex = i;
            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// 星球面板GUI逻辑
        /// </summary>
        private void PlanetWindowShowFun()
        {
            if (PlanetorSum || RefreshPlanetinfo)
            {
                GUI.DrawTexture(GUI.Window(202108213, GetPlanetWindowRect(), PlanetWindow, ""), mytexture);
            }
        }

        private double PlayerToPlanetDis(PlanetData pd)
        {
            VectorLF3 vectorLF2 = pd.uPosition - GameMain.mainPlayer.uPosition;
            double num2 = vectorLF2.magnitude - pd.realRadius - 60;
            return num2 < 0 ? 0 : num2;
        }

        private string PlayerToPlanetDisTranslate(PlanetData pd)
        {
            double distanceNum = PlayerToPlanetDis(pd);
            double magnitude = distanceNum + pd.realRadius + 60;
            string format = "{0:0} m";
            if (magnitude >= 24000000.0)
            {
                distanceNum /= 2400000.0;
                format = "{0:0.000} LY";
            }
            else if (magnitude >= 2400000.0)
            {
                distanceNum /= 2400000.0;
                format = "{0:0.0000} LY";
            }
            else if (magnitude >= 2000.0)
            {
                distanceNum /= 40000.0;
                format = "{0:0.000} AU";
            }
            return string.Format(format, distanceNum);
        }

        /// <summary>
        /// 刷新所有
        /// </summary>
        private void RefreshAll()
        {
            if (StopRefresh || !OneSecondElapsed)
            {
                return;
            }
            RefreshProductStat();
            Dictionary<int, long[]> tempDiction = SumProduce;
            if (PlanetorSum && PlanetProduce != null && PlanetProduce.Count > 0)
            {
                if (Multplanetproduct)
                {
                    multPlanetProduce = new Dictionary<int, long[]>();
                    for (int i = 0; i < planetsproductinfoshow.Count; i++)
                    {
                        if (!PlanetProduce.ContainsKey(planetsproductinfoshow[i])) continue;
                        Dictionary<int, long[]> tempDiction1 = PlanetProduce[planetsproductinfoshow[i]];
                        foreach (KeyValuePair<int, long[]> wap in tempDiction1)
                        {
                            if (multPlanetProduce.ContainsKey(wap.Key))
                            {
                                for (int j = 0; j < 10; j++)
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
                    multPlanetProduce = sortItem(multPlanetProduce, cursortcolumnindex, cursortrule);
                    tempDiction = multPlanetProduce;
                }
                else if (PlanetProduce.ContainsKey(pointPlanetId))
                {
                    PlanetProduce[pointPlanetId] = sortItem(PlanetProduce[pointPlanetId], cursortcolumnindex, cursortrule);
                    tempDiction = PlanetProduce[pointPlanetId];
                }
            }
            tempDiction = sortItem(tempDiction, cursortcolumnindex, cursortrule);
            var temp = new List<int>(tempDiction.Keys);
            iteminfoshow = new List<int>();
            for (int i = 0; i < temp.Count; i++)
            {
                int itemId = temp[i];
                ItemProto item = LDB.items.Select(itemId);
                if (LDB.items.Select(itemId) == null) continue;

                if (itemId == 1030 || itemId == 1031) continue;
                if ((itemId >= 1000 && itemId < 1030 || itemId == 1208) && !rawmaterial) continue;
                if ((itemId > 1100 && itemId < 1200) && !secondrawmaterial) continue;
                if (item.CanBuild && !building) continue;
                if (item.recipes.Count > 0 && (itemId != 1003 && !(itemId > 1100 && itemId < 1200)) && item.BuildIndex == 0 && !compound) continue;
                if (onlyseechose && !Productsearchcondition[itemId]) continue;
                if (noreachtheoryproduce && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][0] >= tempDiction[itemId][2])) continue;
                if (noreachneedproduce && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][0] >= tempDiction[itemId][3])) continue;
                if (theorynoreachneedproduce && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][2] >= tempDiction[itemId][3])) continue;
                if (producelimittoggle && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][0] <= producelowerlimit)) continue;
                if (comsumeimittoggle && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][1] <= comsumelowerlimit)) continue;
                if (theoryproducelimittoggle && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][2] <= theoryproducelowerlimit)) continue;
                if (theorycomsumelimittoggle && (!tempDiction.ContainsKey(itemId) || tempDiction[itemId][3] <= theorycomsumelowerlimit)) continue;
                iteminfoshow.Add(itemId);
            }
        }

        /// <summary>
        /// 刷新目标星球工厂
        /// </summary>
        /// <param name="pdid"></param>
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

        /// <summary>
        /// 刷新星球信息
        /// </summary>
        private void RefreshPlanet()
        {
            if (!OneSecondElapsed || GameMain.galaxy == null) return;
            planetinfoshow.Clear();
            unloadPlanetNum = 0;
            foreach (StarData sd in GameMain.galaxy.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    unloadPlanetNum += pd.data == null && pd.factory == null ? 1 : 0;
                    if (PlanetorSum)
                    {
                        if (!RefreshPlanetinfo && (!PlanetProduce.ContainsKey(pd.id) || PlanetProduce[pd.id].Count == 0))
                        {
                            continue;
                        }
                        if (Refreshfactoryinfo && pd.factory != null)
                        {
                            if (!pd.factory.entityPool.Any(entity => entity.protoId > 0)) continue;
                        }
                    }
                    if (Filtercondition || Refreshfactoryinfo)
                    {
                        if (!PlanetProduce.ContainsKey(pd.id) || PlanetProduce[pd.id].Count == 0) continue;
                        if (!CheckProductCondition(pd.id)) continue;
                    }

                    bool flag = true;
                    long[] veinAmounts = new long[64];
                    pd.CalcVeinAmounts(ref veinAmounts, new HashSet<int>(), UIRoot.instance.uiGame.veinAmountDisplayFilter);
                    for (int i = 1; i <= 30; i++)
                    {
                        if (searchcondition_bool[i])
                        {
                            if (i <= 14)
                            {
                                int[] planetsveinSpotsSketch = veinSpotsSketch(pd);
                                if (planetsveinSpotsSketch == null || planetsveinSpotsSketch[i] == 0)
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
                            if (i == 28 && (!powerenergyinfoshow.ContainsKey(pd.id) || powerenergyinfoshow[pd.id][0] + powerenergyinfoshow[pd.id][3] > powerenergyinfoshow[pd.id][1])) { flag = false; break; }
                            if (i == 29 && pd.data == null) { flag = false; break; }
                            if (i == 30 && pd.data != null) { flag = false; break; }
                        }
                    }

                    if (searchcondition_TypeName != "" && pd.typeString != searchcondition_TypeName) flag = false;
                    if (flag)
                        planetinfoshow.Add(pd.id);
                }
            }
            if (_sortbyPlanetDataDis)
            {
                planetinfoshow.Sort((a, b) =>
                {
                    double result = PlayerToPlanetDis(GameMain.galaxy.PlanetById(a)) - PlayerToPlanetDis(GameMain.galaxy.PlanetById(b));
                    if (result > 0)
                    {
                        return 1;
                    }
                    else if (result == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                });
            }
            if (_sortbyVeinCount)
            {
                planetinfoshow.Sort((a, b) =>
                {
                    var pd = GameMain.galaxy.PlanetById(a);
                    var pd2 = GameMain.galaxy.PlanetById(b);
                    int count1 = 0;
                    int count2 = 0;
                    for (int i = 1; i <= 14; i++)
                    {
                        if (!searchcondition_bool[i])
                        {
                            continue;
                        }
                        if (i <= 14)
                        {
                            int[] planetsveinSpotsSketch = veinSpotsSketch(pd);
                            if (planetsveinSpotsSketch != null)
                            {
                                count1 += planetsveinSpotsSketch[i];
                            }
                            planetsveinSpotsSketch = veinSpotsSketch(pd2);
                            if (planetsveinSpotsSketch != null)
                            {
                                count2 += planetsveinSpotsSketch[i];
                            }
                        }
                    }
                    int result = count2 - count1;
                    if (result > 0)
                    {
                        return 1;
                    }
                    else if (result == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                });
            }

            if (SortbyPointProduct && Filtercondition)
            {
                List<long> itemproduce = new List<long>();
                int pointitem = 0;
                foreach (KeyValuePair<int, bool> wap in Productsearchcondition)
                {
                    if (wap.Value)
                    {
                        pointitem = wap.Key;
                        break;
                    }
                }
                if (pointitem == 0) return;
                for (int i = 0; i < planetinfoshow.Count; i++)
                {
                    itemproduce.Add(PlanetProduce[planetinfoshow[i]][pointitem][0]);
                }
                for (int i = 0; i < itemproduce.Count; i++)
                {
                    int max = i;
                    for (int j = i + 1; j < itemproduce.Count; j++)
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

        /// <summary>
        /// 刷新产物统计信息
        /// </summary>
        private void RefreshProductStat()
        {
            var PlanetProductDiction = new Dictionary<int, Dictionary<int, float>>();
            var PlanetRequireDiction = new Dictionary<int, Dictionary<int, float>>();
            var PlanetComsumerDiction = new Dictionary<int, Dictionary<int, long>>();
            var PlanetProducerDiction = new Dictionary<int, Dictionary<int, long>>();
            PlanetProduce = new Dictionary<int, Dictionary<int, long[]>>();
            productIndices = new Dictionary<int, Dictionary<int, int>>();
            SumProduce = new Dictionary<int, long[]>();
            powerenergyinfoshow = new Dictionary<int, long[]>();
            sumpowerinfoshow = new long[6];
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
                                {
                                    productIndices[pd.id].Add(i, ip.ID);
                                    break;
                                }
                        if (!productIndices[pd.id].ContainsKey(i))
                        {
                            continue;
                        }
                        int itemId = productIndices[pd.id][i];
                        if (PlanetorSum || RefreshPlanetinfo)
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

                    FactorySystem fs = pd.factory.factorySystem;

                    foreach (MinerComponent mc in fs.minerPool)
                    {
                        if (mc.id <= 0 || mc.entityId <= 0)
                        {
                            continue;
                        }
                        if (mc.type == EMinerType.Water && pd.waterItemId > 0)
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
                            PlanetProductDiction[pd.id][index] += (long)(30 * (isveincollector ? 2 : 1) * mc.veinCount * GameMain.history.miningSpeedScale * mc.speed) / 10000;
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
                    foreach (FractionatorComponent fc in fs.fractionatorPool)
                    {
                        if (fc.id <= 0 || fc.entityId <= 0)
                        {
                            continue;
                        }
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
                    foreach (SiloComponent sc in fs.siloPool)
                    {
                        if (sc.id <= 0 || sc.entityId <= 0)
                        {
                            continue;
                        }
                        if (!PlanetRequireDiction[pd.id].ContainsKey(1503))
                            PlanetRequireDiction[pd.id][1503] = 0;
                        if (!PlanetComsumerDiction[pd.id].ContainsKey(1503))
                            PlanetComsumerDiction[pd.id][1503] = 0;
                        PlanetComsumerDiction[pd.id][1503]++;
                        PlanetRequireDiction[pd.id][1503] += 5;
                    }
                    foreach (EjectorComponent ec in fs.ejectorPool)
                    {
                        if (ec.id <= 0 || ec.entityId <= 0)
                        {
                            continue;
                        }
                        if (!PlanetRequireDiction[pd.id].ContainsKey(1501))
                            PlanetRequireDiction[pd.id][1501] = 0;
                        if (!PlanetComsumerDiction[pd.id].ContainsKey(1501))
                            PlanetComsumerDiction[pd.id][1501] = 0;
                        PlanetComsumerDiction[pd.id][1501]++;
                        PlanetRequireDiction[pd.id][1501] += 20;
                    }

                    foreach (AssemblerComponent ac in fs.assemblerPool)
                    {
                        if (ac.id <= 0 || ac.entityId <= 0)
                        {
                            continue;
                        }
                        RecipeProto rp;
                        if ((rp = LDB.recipes.Select(ac.recipeId)) == null)
                        {
                            continue;
                        }
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
                                PlanetRequireDiction[pd.id][rp.Items[i]] += rp.ItemCounts[i] * 9.0f * ac.speed / (rp.TimeSpend * 25.0f);
                        }
                        for (int i = 0; i < rp.Results.Length; i++)
                        {
                            if (!PlanetProductDiction[pd.id].ContainsKey(rp.Results[i]))
                                PlanetProductDiction[pd.id].Add(rp.Results[i], 0);
                            if (!PlanetProducerDiction[pd.id].ContainsKey(rp.Results[i]))
                                PlanetProducerDiction[pd.id][rp.Results[i]] = 0;
                            PlanetProducerDiction[pd.id][rp.Results[i]]++;
                            if (ac.extraSpeed == 0)
                                PlanetProductDiction[pd.id][rp.Results[i]] += rp.ResultCounts[i] * 9.0f * ac.speedOverride / (rp.TimeSpend * 25.0f);
                            else
                                PlanetProductDiction[pd.id][rp.Results[i]] += rp.ResultCounts[i] * 9.0f * (ac.speed + ac.extraSpeed / 10) / (rp.TimeSpend * 25.0f);
                        }
                    }
                    foreach (LabComponent lc in fs.labPool)
                    {
                        if (lc.id <= 0 || lc.entityId <= 0)
                        {
                            continue;
                        }
                        RecipeProto rp;
                        if ((rp = LDB.recipes.Select(lc.recipeId)) == null)
                        {
                            continue;
                        }
                        bool flag = lc.productive && !lc.forceAccMode;
                        float num = lc.speedOverride * (1 + lc.extraSpeed * 0.1f / lc.speedOverride) * 3600 / lc.timeSpend;
                        for (int i = 0; i < rp.Items.Length; i++)
                        {
                            if (!PlanetRequireDiction[pd.id].ContainsKey(rp.Items[i]))
                                PlanetRequireDiction[pd.id].Add(rp.Items[i], 0);
                            if (!PlanetComsumerDiction[pd.id].ContainsKey(rp.Items[i]))
                                PlanetComsumerDiction[pd.id][rp.Items[i]] = 0;
                            PlanetComsumerDiction[pd.id][rp.Items[i]]++;

                            if (flag)
                            {
                                PlanetRequireDiction[pd.id][rp.Items[i]] += rp.ItemCounts[i] * num * 1f / (1f + lc.extraSpeed * 1f / (lc.speed * 10f));
                            }
                            else
                            {
                                PlanetRequireDiction[pd.id][rp.Items[i]] += rp.ItemCounts[i] * num;
                            }
                        }
                        for (int i = 0; i < rp.Results.Length; i++)
                        {
                            if (!PlanetProductDiction[pd.id].ContainsKey(rp.Results[i]))
                                PlanetProductDiction[pd.id].Add(rp.Results[i], 0);
                            if (!PlanetProducerDiction[pd.id].ContainsKey(rp.Results[i]))
                                PlanetProducerDiction[pd.id][rp.Results[i]] = 0;
                            PlanetProducerDiction[pd.id][rp.Results[i]]++;

                            PlanetProductDiction[pd.id][rp.Results[i]] += num;
                        }
                    }
                    float sum = 0;
                    foreach (PowerGeneratorComponent pgc in fs.factory.powerSystem.genPool)
                    {
                        if (!pgc.gamma)
                        {
                            continue;
                        }
                        float eta = 1f - GameMain.history.solarEnergyLossRate;
                        sum += pgc.EtaCurrent_Gamma(eta) * pgc.RequiresCurrent_Gamma(eta);
                        if (!PlanetProducerDiction[pd.id].ContainsKey(1208))
                            PlanetProducerDiction[pd.id][1208] = 0;
                        PlanetProducerDiction[pd.id][1208]++;
                    }
                    if (sum > 0)
                    {
                        if (!PlanetProductDiction[pd.id].ContainsKey(1208))
                            PlanetProductDiction[pd.id][1208] = 0;
                        PlanetProductDiction[pd.id][1208] = (long)(sum * 3.0f / 1000000.0f);
                    }
                    foreach (PowerExchangerComponent pec in fs.factory.powerSystem.excPool)
                    {
                        if (pec.id <= 0 || pec.targetState == 0)
                        {
                            continue;
                        }
                        int product = pec.targetState == 1 ? 2207 : 2206;
                        int requireitem = 4413 - product;

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

                    //if(powerenergyinfoshow.ContainsKey(0))
                    //    powerenergyinfoshow.Add(0, 0);
                    //powerenergyinfoshow[0] = factoryProduction.energyConsumption;
                    if (factoryProduction.powerPool != null && factoryProduction.powerPool.Length > 0)
                    {
                        powerenergyinfoshow[pd.id][0] = factoryProduction.powerPool[0].total[0] / 10;
                        powerenergyinfoshow[pd.id][1] = factoryProduction.powerPool[1].total[0] / 10;
                        powerenergyinfoshow[pd.id][2] = factoryProduction.energyConsumption;
                        powerenergyinfoshow[pd.id][3] = factoryProduction.powerPool[3].total[0] / 10;
                        powerenergyinfoshow[pd.id][4] = factoryProduction.powerPool[2].total[0] / 10;
                        sumpowerinfoshow[0] += factoryProduction.powerPool[0].total[0] / 10;
                        sumpowerinfoshow[1] += factoryProduction.powerPool[1].total[0] / 10;
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
                                if (localsc == null || localsc.entityId <= 0 || localsc.isEmpty)
                                {
                                    continue;
                                }
                                for (int i = 0; i < localsc.grids.Length; i++)
                                {
                                    int itemId;
                                    if ((itemId = localsc.grids[i].itemId) <= 0)
                                    {
                                        continue;
                                    }
                                    if (PlanetorSum || RefreshPlanetinfo)
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
                        if (fs.storage.tankPool != null)
                        {
                            foreach (TankComponent tc in fs.storage.tankPool)
                            {
                                if (tc.id <= 0 || tc.fluidId <= 0 || tc.fluidCount <= 0)
                                {
                                    continue;
                                }
                                int itemId = tc.fluidId;
                                if (PlanetorSum || RefreshPlanetinfo)
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
                                    if (PlanetorSum || RefreshPlanetinfo)
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
                            string scName = fs.factory.ReadExtraInfoOnEntity(sc.entityId);
                            if (string.IsNullOrEmpty(scName) && sc.isStellar && (scName.Equals("Station_miner") || scName.Equals("星球矿机")))
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
                                            PlanetProductDiction[pdId][itemId] += itemId != 1007 ? (int)(getVeinnumber(itemId, pdId) * miningSpeedScale) / 2 * 60 : (int)(getVeinnumber(itemId, pdId) * miningSpeedScale) * 60;
                                    }
                                }
                            }
                            if (sc.collectionPerTick != null && sc.isCollector)
                            {
                                PrefabDesc prefabDesc = LDB.items.Select(ItemProto.stationCollectorId).prefabDesc;
                                double collectorsWorkCost = prefabDesc.workEnergyPerTick * 60.0;
                                collectorsWorkCost /= prefabDesc.stationCollectSpeed;
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
                if ((PlanetorSum || RefreshPlanetinfo) && !PlanetProduce.ContainsKey(wap.Key))
                    PlanetProduce.Add(wap.Key, new Dictionary<int, long[]>());
                foreach (KeyValuePair<int, float> wap1 in wap.Value)
                {
                    if (PlanetorSum || RefreshPlanetinfo)
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
                if ((PlanetorSum || RefreshPlanetinfo) && !PlanetProduce.ContainsKey(wap.Key))
                    PlanetProduce.Add(wap.Key, new Dictionary<int, long[]>());
                foreach (KeyValuePair<int, float> wap1 in wap.Value)
                {
                    if (PlanetorSum || RefreshPlanetinfo)
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
                if ((PlanetorSum || RefreshPlanetinfo) && !PlanetProduce.ContainsKey(wap.Key))
                    PlanetProduce.Add(wap.Key, new Dictionary<int, long[]>());
                foreach (KeyValuePair<int, long> wap1 in wap.Value)
                {
                    if (PlanetorSum || RefreshPlanetinfo)
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
                if ((PlanetorSum || RefreshPlanetinfo) && !PlanetProduce.ContainsKey(wap.Key))
                    PlanetProduce.Add(wap.Key, new Dictionary<int, long[]>());
                foreach (KeyValuePair<int, long> wap1 in wap.Value)
                {
                    if (PlanetorSum || RefreshPlanetinfo)
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

        private string searchchineseTranslate(int i)
        {
            if (0 < i && i < 15) return LDB.items.Select(LDB.veins.Select(i).MiningItem).name;
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

        // 定义SequenceEqual方法，用于比较两个bool类型的数组是否相等
        private bool SequenceEqual(bool[] array1, bool[] array2)
        {
            if (array1 == null || array2 == null)
            {
                return array1 == null && array2 == null;
            }
            if (array1.Length != array2.Length)
            {
                return false;
            }
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }

        void Start()
        {
            _TGMKinttostringMode = true;
            scale = Config.Bind("大小适配", "scale", 16);
            ShowCounter1 = Config.Bind("打开窗口快捷键", "Key", new KeyboardShortcut(KeyCode.Alpha3, KeyCode.LeftAlt));
            guiDraw = new GUIDraw();
            scrollPosition[0] = 0;
            pdselectscrollPosition[0] = 0;
            MoreStatInfoTranslate.regallTranslate();
            Debug.Log("MoreStatInfo Start");
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    OneSecondElapsed = true;
                    Thread.Sleep(1000);
                }
            });
        }

        /// <summary>
        /// 菜单面板
        /// </summary>
        /// <param name="id"></param>
        private void SwitchWindow(int id)
        {
            GUILayout.BeginArea(new Rect(10, 20, switchwitdh, switchheight));
            int x = 0, y = 0;
            RemoteorLocal = GUI.Toggle(AddRect(x, ref y, switchwitdh, heightdis), RemoteorLocal, "本地/远程".getTranslate());
            PlanetorSum = GUI.Toggle(AddRect(x, ref y, switchwitdh, heightdis), PlanetorSum, "全部/星球".getTranslate());
            StopRefresh = GUI.Toggle(AddRect(x, ref y, switchwitdh, heightdis), StopRefresh, "停止刷新".getTranslate());
            TGMKinttostringMode = GUI.Toggle(AddRect(x, ref y, switchwitdh, heightdis), TGMKinttostringMode, "单位转化".getTranslate());
            Filtercondition = GUI.Toggle(AddRect(x, ref y, switchwitdh, heightdis), Filtercondition, "筛选条件".getTranslate());
            Refreshfactoryinfo = GUI.Toggle(AddRect(x, ref y, switchwitdh, heightdis), Refreshfactoryinfo, "工厂信息".getTranslate());
            RefreshPlanetinfo = GUI.Toggle(AddRect(x, ref y, switchwitdh, heightdis), RefreshPlanetinfo, "星球信息".getTranslate());
            Multplanetproduct = GUI.Toggle(AddRect(x, ref y, switchwitdh, heightdis), Multplanetproduct, "多选星球".getTranslate());
            GUILayout.EndArea();
        }

        /// <summary>
        /// 菜单面板GUI逻辑
        /// </summary>
        private void SwitchWindowShowFun()
        {
            GUI.DrawTexture(GUI.Window(202108212, GetSwitchWindowRect(), SwitchWindow, ""), mytexture);
        }

        void Update()
        {
            if (GameMain.instance == null)
                return;
            if (!GameMain.instance.running)
            {
                firstStart = false;
                ItemList = new List<ItemProto>(LDB.items.dataArray);
                PlanetVeinCountPool.Clear();
                while (PlanetModelingManager.calPlanetReqList.Count > 0)
                {
                    PlanetModelingManager.calPlanetReqList.Dequeue();
                }
            }
            else if (!firstStart)
            {
                firstStart = true;
                Productsearchcondition = new Dictionary<int, bool>();
                for (int i = 0; i < ItemList.Count; i++)
                {
                    if (!Productsearchcondition.ContainsKey(ItemList[i].ID))
                        Productsearchcondition.Add(ItemList[i].ID, false);
                }
                foreach (StarData sd in GameMain.galaxy.stars)
                {
                    foreach (PlanetData pd in sd.planets)
                    {
                        if (PlanetType.Contains(pd.typeString)) continue;
                        PlanetType.Add(pd.typeString);
                    }
                }
            }
            else
            {
                if (ShowCounter1.Value.IsDown())
                {
                    guiDraw.ShowGUIWindow = !guiDraw.ShowGUIWindow;
                }
            }
        }
    }
}