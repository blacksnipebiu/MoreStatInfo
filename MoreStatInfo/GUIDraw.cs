using MoreStatInfo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static MoreStatInfo.MoreStatInfo;

namespace MoreStatInfo
{
    internal class GUIDraw
    {
        public string[] columnsbuttonstr = new string[11] { "物品", "实时产量", "实时消耗", "理论产量", "需求产量", "生产者", "消费者", "总计", "本地提供", "本地需求", "本地仓储" };
        public int cursortcolumnindex;
        public int cursortrule;
        public int heightdis;
        public float MainWindow_x = 200;
        public float MainWindow_y = 200;
        public float MainWindowHeight = 700;
        public float MainWindowWidth = 1150;
        public bool OnGUIInited;
        public float PanelMaskWidth;
        public bool[] Productsearchcondition;
        public int switchheight;
        public int switchwitdh;

        #region 样式

        private GUIStyle buttonstyleblue = null;
        private GUIStyle buttonstyleyellow = null;
        private GUILayoutOption[] doubleiconoptions;
        private GUIStyle emptyStyle;
        private GUILayoutOption[] iconoptions;
        private Texture2D mytexture;
        private GUIStyle normalPlanetButtonStyle;
        private GUIStyle selectedPlanetButtonStyle;
        private GUIStyle styleboldblue;
        private GUIStyle styleitemname = null;
        private GUIStyle stylenormalblue;
        private GUIStyle stylesmallblue;
        private GUIStyle stylesmallyellow;
        private GUIStyle styleyellow;
        private GUIStyle whiteStyle;

        #endregion 样式

        private static GameObject ui_morestatinfopanel;
        private bool _filtercondition;
        private bool _multplanetproduct;
        private bool _PlanetorSum;
        private bool _refreshfactoryinfo;
        private bool _refreshPlanetinfo;
        private bool _RemoteorLocal;
        private bool _ShowGUIWindow;
        private bool _sortbyPlanetDataDis;
        private bool _sortbypointproduct;
        private bool _sortbyVeinCount;
        private bool _TGMKinttostringMode;
        private int baseSize;
        private bool bottomscaling;
        private bool building = true;
        private int[] ColumnWidth;
        private bool compound = true;
        private bool comsumeimittoggle;
        private int comsumelowerlimit;
        private string comsumelowerlimitstr = "0";
        private bool dropdownbutton;
        private Dictionary<int, int> factoryinfoshow = new Dictionary<int, int>();
        private List<int> iteminfoshow = new List<int>();
        private int ItemInfoShowIndex = 1;
        private bool leftscaling;
        private float MainWindow_x_move = 200;
        private float MainWindow_y_move = 200;
        private bool moving;
        private bool noreachneedproduce;
        private bool noreachtheoryproduce;
        private bool onlyseechose;
        private int pdselectinfoindex = 1;
        private Vector2 pdselectscrollPosition;
        private List<int> planetinfoshow = new List<int>();
        private List<int> planetsproductinfoshow = new List<int>();
        private int pointPlanetId;
        private StringBuilder[] Powerenergyinfoshow;
        private bool producelimittoggle;
        private int producelowerlimit;
        private string producelowerlimitstr = "0";
        private bool rawmaterial = true;
        private bool RefreshBaseSize;
        private bool rightscaling;
        private StringBuilder sb;
        private Vector2 scrollPosition;
        private bool[] searchcondition_bool = new bool[40];
        private string searchcondition_TypeName;
        private bool secondrawmaterial = true;
        private int selectitemId;
        private bool StopRefresh;
        private float temp_MainWindow_x = 10;
        private float temp_MainWindow_y = 200;
        private bool theorycomsumelimittoggle;

        private int theorycomsumelowerlimit;

        private string theorycomsumelowerlimitstr = "0";

        private bool theorynoreachneedproduce;

        private bool theoryproducelimittoggle;

        private int theoryproducelowerlimit;
        private string theoryproducelowerlimitstr = "0";

        private int unloadPlanetNum;

        public GUIDraw(int baseFontSize)
        {
            BaseSize = baseFontSize;
            Powerenergyinfoshow = new StringBuilder[5];
            var morestatinfopanel = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("MoreStatInfo.morestatinfopanel")).LoadAsset<GameObject>("MoreStatInfoPanel");

            ui_morestatinfopanel = UnityEngine.Object.Instantiate(morestatinfopanel, UIRoot.instance.overlayCanvas.transform);
        }

        public int BaseSize
        {
            get => baseSize;
            set
            {
                if (baseSize == value)
                {
                    return;
                }
                baseSize = value;
                MoreStatInfo.scale.Value = value;
                RefreshBaseSize = true;
                heightdis = value * 2;
            }
        }

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
                columnsbuttonstr[8] = !RemoteorLocal ? "本地提供".GetTranslate() : "远程提供".GetTranslate();
                columnsbuttonstr[9] = !RemoteorLocal ? "本地需求".GetTranslate() : "远程需求".GetTranslate();
                columnsbuttonstr[10] = !RemoteorLocal ? "本地仓储".GetTranslate() : "远程仓储".GetTranslate();
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
        /// 显示面板
        /// </summary>
        public bool ShowGUIWindow
        {
            get => _ShowGUIWindow;
            set
            {
                _ShowGUIWindow = value;
                ui_morestatinfopanel.SetActive(value);
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
                    foreach (var itemId in ItemProto.itemIds)
                    {
                        Productsearchcondition[itemId] = false;
                    }
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
            }
        }

        /// <summary>
        /// 改变排序顺序
        /// </summary>
        /// <param name="columnnum"></param>
        public void ChangeSort(int columnnum)
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

        public void Draw()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                int t = (int)(Input.GetAxis("Mouse Wheel") * 10);
                int temp = BaseSize + t;
                if (Input.GetKeyDown(KeyCode.UpArrow)) { temp++; }
                if (Input.GetKeyDown(KeyCode.DownArrow)) { temp--; }
                temp = Math.Max(5, Math.Min(temp, 35));
                BaseSize = temp;
            }
            if (!OnGUIInited)
            {
                OnGUIInited = true;
                Init();
            }
            if (RefreshBaseSize)
            {
                RefreshBaseSize = false;
                RefreshFontSize();
            }

            PanelMaskWidth = MainWindowWidth;
            SwitchWindowShowFun();
            MainWindowShowFun();
            if (PlanetorSum || RefreshPlanetinfo)
            {
                PanelMaskWidth += heightdis * 7;
                PlanetWindowShowFun();
            }
            UIPanelSet();
            if (!StopRefresh && OneSecondElapsed)
            {
                allstatinfo.TargetPlanetIds = planetsproductinfoshow;
                allstatinfo.TargetPlanetId = pointPlanetId;
                var panelMode = PanelMode.GalaxyStatInfo;
                if (PlanetorSum)
                {
                    if (Multplanetproduct)
                    {
                        panelMode = PanelMode.MultiplePlanetStatInfo;
                    }
                    else
                    {
                        panelMode = PanelMode.TargetSinglePlanetStatInfo;
                    }
                }

                allstatinfo.RefreshProperty(panelMode);
                SortItemIndex(allstatinfo.ProductCount, allstatinfo.ProductExist, cursortcolumnindex, cursortrule);
                RefreshPlanet();
            }
            if (OneSecondElapsed)
            {
                OneSecondElapsed = false;
            }
        }

        /// <summary>
        /// 刷新字体大小
        /// </summary>
        public void RefreshFontSize()
        {
            GUI.skin.label.fontSize = BaseSize;
            GUI.skin.button.fontSize = BaseSize;
            GUI.skin.toggle.fontSize = BaseSize;
            GUI.skin.textField.fontSize = BaseSize;
            GUI.skin.textArea.fontSize = BaseSize;
            buttonstyleblue.fontSize = BaseSize;
            buttonstyleyellow.fontSize = BaseSize;
            selectedPlanetButtonStyle.fontSize = BaseSize;
            normalPlanetButtonStyle.fontSize = BaseSize;
            styleboldblue.fontSize = BaseSize + 5;
            stylesmallblue.fontSize = BaseSize;
            styleyellow.fontSize = BaseSize + 5;
            stylesmallyellow.fontSize = BaseSize;
            styleitemname.fontSize = BaseSize;
            switchwitdh = IsEnglish ? 7 * heightdis : 4 * heightdis;
            switchheight = heightdis * 10;

            iconoptions = new[] { GUILayout.Width(heightdis), GUILayout.Height(heightdis) };
            doubleiconoptions = new[] { GUILayout.Width(2 * heightdis), GUILayout.Height(2 * heightdis) };
        }

        /// <summary>
        /// 根据规则排序后的结果
        /// </summary>
        /// <param name="itemsmap"></param>
        /// <param name="columnnum"></param>
        /// <param name="sortrules"></param>
        /// <returns></returns>
        public void SortItemIndex(long[][] itemsmap, bool[] productexist, int columnnum, int sortrules)
        {
            iteminfoshow.Clear();
            for (int i = 0; i < ItemProto.itemIds.Length; i++)
            {
                var itemId = ItemProto.itemIds[i];
                if (!productexist[itemId])
                {
                    continue;
                }
                if (itemId == 1030 || itemId == 1031) continue;
                if ((itemId >= 1000 && itemId < 1030 || itemId == 1208) && !rawmaterial) continue;
                if ((itemId > 1100 && itemId < 1200) && !secondrawmaterial) continue;
                ItemProto item = ItemProto.itemProtoById[itemId];
                if (item == null) continue;
                if (item.CanBuild && !building) continue;
                if (item.recipes.Count > 0 && (itemId != 1003 && !(itemId > 1100 && itemId < 1200)) && item.BuildIndex == 0 && !compound) continue;
                if (onlyseechose && !Productsearchcondition[itemId]) continue;
                if (noreachtheoryproduce && (!productexist[itemId] || itemsmap[itemId][0] >= itemsmap[itemId][2])) continue;
                if (noreachneedproduce && (!productexist[itemId] || itemsmap[itemId][0] >= itemsmap[itemId][3])) continue;
                if (theorynoreachneedproduce && (!productexist[itemId] || itemsmap[itemId][2] >= itemsmap[itemId][3])) continue;
                if (producelimittoggle && (!productexist[itemId] || itemsmap[itemId][0] <= producelowerlimit)) continue;
                if (comsumeimittoggle && (!productexist[itemId] || itemsmap[itemId][1] <= comsumelowerlimit)) continue;
                if (theoryproducelimittoggle && (!productexist[itemId] || itemsmap[itemId][2] <= theoryproducelowerlimit)) continue;
                if (theorycomsumelimittoggle && (!productexist[itemId] || itemsmap[itemId][3] <= theorycomsumelowerlimit)) continue;
                iteminfoshow.Add(itemId);
            }
            if (sortrules == 0)
            {
                return;
            }
            if (columnnum == 0)
            {
                if (sortrules == 1)
                {
                    iteminfoshow.Reverse();
                }
                return;
            }
            columnnum--;
            int itemsmapcount = iteminfoshow.Count;
            for (int i = 0; i < itemsmapcount; i++)
            {
                for (int j = i + 1; j < itemsmapcount; j++)
                {
                    var secondItemId = iteminfoshow[j];
                    if (sortrules == 1)
                    {
                        if (itemsmap[iteminfoshow[i]][columnnum] > itemsmap[secondItemId][columnnum])
                        {
                            (iteminfoshow[i], iteminfoshow[j]) = (iteminfoshow[j], iteminfoshow[i]);
                        }
                    }
                    else
                    {
                        if (itemsmap[iteminfoshow[i]][columnnum] < itemsmap[secondItemId][columnnum])
                        {
                            (iteminfoshow[i], iteminfoshow[j]) = (iteminfoshow[j], iteminfoshow[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据物品ID来判断条件
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="tempDiction"></param>
        /// <returns></returns>
        private bool CheckItemConditions(int itemId, long[][] tempDiction)
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
            return Productsearchcondition.Select((isSelected, itemId) => new { isSelected, itemId })
                    .Where(selectedItem => selectedItem.isSelected)
                    .All(selectedItem => allstatinfo.planetstatinfoDic[pdid].ProductExist[selectedItem.itemId] && CheckItemConditions(selectedItem.itemId, allstatinfo.planetstatinfoDic[pdid].ProductCount));
        }

        // 定义ClampString方法，将输入字符串的长度限制在指定的范围内
        private string ClampString(string str, int minLength, int maxLength)
        {
            if (str.Length < minLength)
            {
                return new string('0', minLength);
            }
            else return str.Length > maxLength ? str.Substring(0, maxLength) : str;
        }

        private void Init()
        {
            switchwitdh = IsEnglish ? 7 * heightdis : 4 * heightdis;
            switchheight = heightdis * 10;
            for (int i = 0; i < 5; i++)
            {
                if (i == 2)
                {
                    Powerenergyinfoshow[i] = new StringBuilder("          J", 16);
                }
                else
                {
                    Powerenergyinfoshow[i] = new StringBuilder("          W", 16);
                }
            }
            sb = new StringBuilder("                ", 16);
            ColumnWidth = new int[11];
            factoryinfoshow = new Dictionary<int, int>();
            Productsearchcondition = new bool[12000];
            _TGMKinttostringMode = true;

            var blueColor = new Color32(167, 255, 255, 255);
            var yellowColor = new Color32(240, 191, 103, 255);

            emptyStyle = new GUIStyle();
            whiteStyle = new GUIStyle();
            whiteStyle.normal.background = Texture2D.whiteTexture;
            styleitemname = new GUIStyle();
            styleitemname.normal.textColor = Color.white;
            buttonstyleblue = new GUIStyle(GUI.skin.button);
            buttonstyleyellow = new GUIStyle(GUI.skin.button);
            normalPlanetButtonStyle = new GUIStyle(GUI.skin.button);
            selectedPlanetButtonStyle = new GUIStyle(GUI.skin.button);
            selectedPlanetButtonStyle.normal.textColor = new Color32(215, 186, 245, 255);

            buttonstyleblue.normal.textColor = blueColor;
            buttonstyleyellow.normal.textColor = yellowColor;

            buttonstyleblue.margin = new RectOffset();
            buttonstyleyellow.margin = new RectOffset();
            normalPlanetButtonStyle.margin = new RectOffset();
            selectedPlanetButtonStyle.margin = new RectOffset();
            mytexture = new Texture2D(10, 10);
            for (int i = 0; i < mytexture.width; i++)
                for (int j = 0; j < mytexture.height; j++)
                    mytexture.SetPixel(i, j, new Color(0, 0, 0, 1));
            mytexture.Apply();

            styleboldblue = new GUIStyle { fontStyle = FontStyle.Bold, fontSize = 20 };
            stylesmallblue = new GUIStyle { fontStyle = FontStyle.Normal, fontSize = 15 };
            stylenormalblue = new GUIStyle { fontStyle = FontStyle.Normal, fontSize = 20 };

            stylesmallblue.normal.textColor = blueColor;
            styleboldblue.normal.textColor = blueColor;
            stylenormalblue.normal.textColor = blueColor;

            styleyellow = new GUIStyle { fontStyle = FontStyle.Bold, fontSize = 20 };
            stylesmallyellow = new GUIStyle { fontStyle = FontStyle.Normal, fontSize = 15 };

            styleyellow.normal.textColor = yellowColor;
            stylesmallyellow.normal.textColor = yellowColor;

            iconoptions = new[] { GUILayout.Width(heightdis), GUILayout.Height(heightdis) };
            doubleiconoptions = new[] { GUILayout.Width(2 * heightdis), GUILayout.Height(2 * heightdis) };
        }

        /// <summary>
        /// 刷新目标星球工厂
        /// </summary>
        /// <param name="pdid"></param>
        private void RefreshFactory(int pdid)
        {
            factoryinfoshow.Clear();
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
            planetinfoshow.Clear();
            unloadPlanetNum = 0;
            foreach (StarData sd in GameMain.galaxy.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    unloadPlanetNum += pd.data == null && pd.factory == null ? 1 : 0;
                    if (PlanetorSum)
                    {
                        if (!RefreshPlanetinfo && pd.GetPlanetStatInfo().productCursor == 0)
                        {
                            continue;
                        }
                        if (Refreshfactoryinfo && pd.factory != null && !pd.ExistFactory())
                        {
                            continue;
                        }
                    }
                    if (Filtercondition || Refreshfactoryinfo)
                    {
                        if (!pd.ExistFactory()) continue;
                        if (!CheckProductCondition(pd.id)) continue;
                    }
                    bool flag = true;
                    for (int i = 1; i <= 30; i++)
                    {
                        if (searchcondition_bool[i])
                        {
                            if (i <= 14 && pd.GetPlanetStatInfo().planetsveinSpotsSketch[i] == 0) { flag = false; break; }
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
                            if (i == 26 && !pd.ExistFactory()) { flag = false; break; }
                            if (i == 27 && pd.ExistFactory()) { flag = false; break; }
                            if (i == 28 && allstatinfo.planetstatinfoDic[pd.id].Lackofelectricity) { flag = false; break; }
                            if (i == 29 && pd.data == null) { flag = false; break; }
                            if (i == 30 && pd.data != null) { flag = false; break; }
                        }
                    }

                    if (!string.IsNullOrEmpty(searchcondition_TypeName) && pd.typeString != searchcondition_TypeName) flag = false;
                    if (flag)
                        planetinfoshow.Add(pd.id);
                }
            }
            if (_sortbyPlanetDataDis)
            {
                planetinfoshow.Sort((a, b) =>
                {
                    double result = DistancecalCulate.PlayerToPlanetDis(GameMain.galaxy.PlanetById(a)) - DistancecalCulate.PlayerToPlanetDis(GameMain.galaxy.PlanetById(b));
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
                    int count1 = allstatinfo.planetstatinfoDic[a].planetsveinSpotsSketch.Where((n, index) => searchcondition_bool[index]).Sum();
                    int count2 = allstatinfo.planetstatinfoDic[b].planetsveinSpotsSketch.Where((n, index) => searchcondition_bool[index]).Sum();
                    if (count2 - count1 > 0)
                    {
                        return 1;
                    }
                    else if (count2 - count1 == 0)
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
                int pointitem = 0;
                for (int i = 0; i < ItemProto.itemIds.Length; i++)
                {
                    var itemId = ItemProto.itemIds[i];
                    if (Productsearchcondition[itemId])
                    {
                        pointitem = itemId;
                        break;
                    }
                }
                if (pointitem == 0) return;
                var planetShowCount = planetinfoshow.Count;
                for (int i = 0; i < planetShowCount; i++)
                {
                    int curPlanetId = planetinfoshow[i];

                    for (int j = i + 1; j < planetShowCount; j++)
                    {
                        int comparePlanetId = planetinfoshow[j];
                        if (allstatinfo.planetstatinfoDic[curPlanetId].ProductCount[pointitem][0] < allstatinfo.planetstatinfoDic[comparePlanetId].ProductCount[pointitem][0])
                        {
                            (planetinfoshow[i], planetinfoshow[j]) = (planetinfoshow[j], planetinfoshow[i]);
                        }
                    }
                }
            }
        }

        #region UI

        /// <summary>
        /// 工厂信息
        /// </summary>
        public void FactoryInfoPanel()
        {
            GUILayout.BeginVertical();
            var totalNum = factoryinfoshow.Count;
            int columnNum = (int)((MainWindowWidth - 20) / (heightdis * 3));
            int lines = totalNum / columnNum + ((totalNum % columnNum) > 0 ? 1 : 0);

            for (int i = 0; i < lines; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < columnNum; j++)
                {
                    int index = i * columnNum + j;
                    if (index == totalNum) break;
                    var wap = factoryinfoshow.ElementAt(index);
                    ItemProto item = ItemProto.itemProtoById[wap.Key];
                    GUILayout.BeginVertical();
                    if (j % 2 == 0)
                    {
                        GUILayout.Button(item.iconSprite.texture, emptyStyle, doubleiconoptions);
                        GUILayout.Label(wap.Value + "", stylesmallblue);
                    }
                    else
                    {
                        GUILayout.Label(wap.Value + "", stylesmallblue);
                        GUILayout.Button(item.iconSprite.texture, emptyStyle, doubleiconoptions);
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(heightdis);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(50);
            }
            var powerlabelStyle = styleboldblue;
            for (int i = 0; i < 5; i++)
            {
                if (TGMKinttostringMode)
                {
                    StringBuilderUtility.WriteKMG(Powerenergyinfoshow[i], 9, allstatinfo.Powerenergyinfoshow[i]);
                }
                else
                {
                    powerlabelStyle = stylesmallblue;
                }
            }
            GUILayout.Label("发电性能".GetTranslate() + ":" + (TGMKinttostringMode ? Powerenergyinfoshow[0].ToString() : allstatinfo.Powerenergyinfoshow[0] + "W"), powerlabelStyle);
            GUILayout.Label("耗电需求".GetTranslate() + ":" + (TGMKinttostringMode ? Powerenergyinfoshow[1].ToString() : allstatinfo.Powerenergyinfoshow[1] + "W"), powerlabelStyle);
            GUILayout.Label("总耗能".GetTranslate() + ":" + (TGMKinttostringMode ? Powerenergyinfoshow[2].ToString() : allstatinfo.Powerenergyinfoshow[2] + "J"), powerlabelStyle);
            GUILayout.Label("放电功率".GetTranslate() + ":" + (TGMKinttostringMode ? Powerenergyinfoshow[3].ToString() : allstatinfo.Powerenergyinfoshow[3] + "W"), powerlabelStyle);
            GUILayout.Label("充电功率".GetTranslate() + ":" + (TGMKinttostringMode ? Powerenergyinfoshow[4].ToString() : allstatinfo.Powerenergyinfoshow[4] + "W"), powerlabelStyle);

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 主面板
        /// </summary>
        /// <param name="id"></param>
        public void MainWindow(int id)
        {
            try
            {
                GUILayout.BeginArea(new Rect(10, 20, MainWindowWidth - 20, MainWindowHeight - 30));
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                GUILayout.BeginVertical();
                if (Refreshfactoryinfo)
                {
                    FactoryInfoPanel();
                }
                else if (RefreshPlanetinfo)
                {
                    PlanetInfoPanel();
                }
                else
                {
                    StatInfoPanel();
                }

                if (Filtercondition)
                {
                    FilterUI();
                }
                GUILayout.EndVertical();

                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            catch { }
        }

        /// <summary>
        /// 星球信息面板
        /// </summary>
        public void PlanetInfoPanel()
        {
            GUILayout.BeginHorizontal();
            //筛选条件列表
            {
                //星球属性筛选条件
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("附属气态星产物".GetTranslate());
                    for (int i = 15; i <= 17; i++)
                        searchcondition_bool[i] = GUILayout.Toggle(searchcondition_bool[i], i.GetSearchchinese());

                    GUILayout.Label("星球特殊性".GetTranslate());
                    for (int i = 18; i <= 25; i++)
                        searchcondition_bool[i] = GUILayout.Toggle(searchcondition_bool[i], i.GetSearchchinese());

                    if (searchcondition_bool[26] != GUILayout.Toggle(searchcondition_bool[26], "具有工厂".GetTranslate()))
                    {
                        searchcondition_bool[26] = !searchcondition_bool[26];
                        if (searchcondition_bool[26])
                            searchcondition_bool[27] = false;
                    }
                    if (searchcondition_bool[27] != GUILayout.Toggle(searchcondition_bool[27], "不具有工厂".GetTranslate()))
                    {
                        searchcondition_bool[27] = !searchcondition_bool[27];
                        if (searchcondition_bool[27])
                            searchcondition_bool[26] = false;
                    }
                    if (searchcondition_bool[28] != GUILayout.Toggle(searchcondition_bool[28], "电力不足".GetTranslate()))
                    {
                        searchcondition_bool[28] = !searchcondition_bool[28];
                        if (searchcondition_bool[28])
                            searchcondition_bool[26] = true;
                    }
                    if (searchcondition_bool[29] != GUILayout.Toggle(searchcondition_bool[29], "已加载星球".GetTranslate()))
                    {
                        searchcondition_bool[29] = !searchcondition_bool[29];
                        if (searchcondition_bool[29])
                            searchcondition_bool[30] = false;
                    }
                    if (searchcondition_bool[30] != GUILayout.Toggle(searchcondition_bool[30], "未加载星球".GetTranslate()))
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
                    GUILayout.Label("目标星球矿物".GetTranslate());
                    for (int i = 1; i <= 14; i++)
                        searchcondition_bool[i] = GUILayout.Toggle(searchcondition_bool[i], i.GetSearchchinese());
                    _sortbyPlanetDataDis = GUILayout.Toggle(_sortbyPlanetDataDis, "依照离玩家距离排序".GetTranslate());
                    _sortbyVeinCount = GUILayout.Toggle(_sortbyVeinCount, "依矿脉数量排序".GetTranslate());
                    if (GUILayout.Button("取消所有条件".GetTranslate()))
                    {
                        for (int i = searchcondition_bool.Length - 1; i >= 0; i--)
                        {
                            searchcondition_bool[i] = false;
                        }
                    }
                    GUILayout.Label("未加载星球".GetTranslate() + $"{unloadPlanetNum}");
                    if (GUILayout.Button("加载全部星球".GetTranslate()))
                    {
                        PlanetLoad.LoadAllPlanet();
                    }
                    if (PlanetModelingManager.calPlanetReqList.Count > 0)
                    {
                        GUILayout.Label($"{PlanetModelingManager.calPlanetReqList.Count}" + "个星球加载中".GetTranslate());
                        GUILayout.Label("请勿切换存档".GetTranslate());
                    }
                    GUILayout.EndVertical();
                }

                //星球类型筛选条件
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("星球类型".GetTranslate());
                    if (GUILayout.Button(string.IsNullOrEmpty(searchcondition_TypeName) ? "未选择".GetTranslate() : searchcondition_TypeName))
                    {
                        dropdownbutton = !dropdownbutton;
                    }
                    if (dropdownbutton)
                    {
                        if (GUILayout.Button("取消选择"))
                        {
                            dropdownbutton = !dropdownbutton;
                            searchcondition_TypeName = string.Empty;
                        }
                        for (int i = 0; i < PlanetLoad.PlanetType.Count; i++)
                        {
                            var planettype = PlanetLoad.PlanetType[i];
                            if (GUILayout.Button(planettype))
                            {
                                dropdownbutton = !dropdownbutton;
                                searchcondition_TypeName = planettype;
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
                GUILayout.Label("行星信息".GetTranslate() + ":", styleboldblue);
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
                    string waterType = pd.waterItemId.GetItemText();
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("海洋类型".GetTranslate() + ":", stylenormalblue);
                    GUILayout.Space(20);
                    GUILayout.Label(waterType, stylenormalblue);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("星球类型".GetTranslate() + ":", stylenormalblue);
                    GUILayout.Space(20);
                    GUILayout.Label(pd.typeString, stylenormalblue);
                    GUILayout.EndHorizontal();
                    if (pd.singularityString.Length > 0)
                    {
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("星球特殊性".GetTranslate() + ": ", stylenormalblue);
                        GUILayout.Space(20);
                        GUILayout.Label(pd.singularityString, stylenormalblue);
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Label("工厂状态".GetTranslate() + ":", stylenormalblue);
                GUILayout.Space(20);
                GUILayout.Label(pd.ExistFactory() ? "有".GetTranslate() : "无".GetTranslate(), stylenormalblue);
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Label("距离玩家".GetTranslate() + ":", stylenormalblue);
                GUILayout.Space(20);
                GUILayout.Label(DistancecalCulate.PlayerToPlanetDisTranslate(pd), stylenormalblue);
                GUILayout.EndHorizontal();
                if (GUILayout.Button("方向指引".GetTranslate()))
                {
                    GameMain.mainPlayer.navigation._indicatorAstroId = pd.id;
                }
                GUILayout.Space(20);
                if (pd.gasItems != null)
                {
                    for (int i = 0; i < pd.gasItems.Length; i++)
                    {
                        ItemProto item = ItemProto.itemProtoById[pd.gasItems[i]];
                        GUILayout.BeginHorizontal();
                        GUILayout.Button(item.iconSprite.texture, emptyStyle, iconoptions);
                        GUILayout.Label(item.name + ":" + string.Format("{0:N2}", pd.gasSpeeds[i]), stylenormalblue);
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    VeinSearch.CalcVeinAmounts(pd);
                    int[] planetveinSpotsSketch = VeinSearch.veinSpotsSketch(pd);
                    for (int k = 1; planetveinSpotsSketch != null && k <= 14; k++)
                    {
                        if (planetveinSpotsSketch[k] == 0) continue;
                        long i = VeinSearch.veinAmounts[k];

                        var item = ItemProto.itemProtoById[LDB.veins.Select(k).MiningItem];
                        GUILayout.BeginHorizontal();
                        GUILayout.Button(item.iconSprite.texture, emptyStyle, iconoptions);
                        StringBuilderUtility.WriteKMG(sb, 9, i);
                        GUILayout.Label(item.name + sb.ToString() + ("(" + planetveinSpotsSketch[k] + ")"), stylenormalblue);
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
                        GUILayout.Label("围绕行星".GetTranslate() + ":", styleboldblue);
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
                            GUILayout.Button(item.iconSprite.texture, emptyStyle, iconoptions);
                            GUILayout.Label(item.name + ":" + string.Format("{0:N2}", pd.orbitAroundPlanet.gasSpeeds[i]), stylenormalblue);
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(heightdis);
                    }

                    {
                        GUILayout.Label("恒星信息".GetTranslate() + ":", styleboldblue);
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
                        GUILayout.Label("恒星亮度".GetTranslate() + ":" + sd.dysonLumino + "", stylenormalblue);
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
        }

        /// <summary>
        /// 产物统计面板
        /// </summary>
        public void StatInfoPanel()
        {
            // 获取生产数据字典
            //var tempDiction = Multplanetproduct
            //    ? multPlanetProduce
            //    : PlanetorSum && PlanetProductCount != null && PlanetProductCount.ContainsKey(pointPlanetId)
            //        ? GameMain.galaxy.PlanetById(pointPlanetId).GetPlanetStatInfo()
            //        : SumProduce;
            GUILayout.BeginVertical();

            // 设置列宽度
            ColumnWidth[0] = IsEnglish ? heightdis * 8 : heightdis * 4;
            for (var i = 1; i <= 10; i++)
            {
                if (i == 5 || i == 6)
                {
                    ColumnWidth[i] = TGMKinttostringMode ? heightdis * 4 : heightdis * 2;
                }
                else
                {
                    ColumnWidth[i] = heightdis * 4;
                }
            }

            // 绘制列按钮
            GUILayout.BeginHorizontal();
            GUILayout.Space(heightdis);
            for (int i = 0; i <= 10; i++)
            {
                var buttonStyle = i % 2 == 0 ? buttonstyleyellow : buttonstyleblue;
                if (GUILayout.Button(columnsbuttonstr[i].GetTranslate(), buttonStyle, GUILayout.Width(ColumnWidth[i])))
                    ChangeSort(i);
            }
            GUILayout.EndHorizontal();

            // 绘制物品信息
            foreach (var itemInfo in iteminfoshow.Skip((ItemInfoShowIndex - 1) * 20).Take(20))
            {
                GUILayout.BeginHorizontal();
                var itemID = itemInfo;
                var item = ItemProto.itemProtoById[itemID];
                if (item == null) continue;

                GUILayout.Button(item.iconSprite.texture, emptyStyle, iconoptions);
                GUILayout.Label(item.name, styleitemname, GUILayout.Width(ColumnWidth[0]));
                for (int j = 0; j < 10; j++)
                {
                    var labelStyle = j % 2 == 0 ? styleboldblue : styleyellow;
                    if (TGMKinttostringMode)
                    {
                        StringBuilderUtility.WriteKMG(sb, 9, allstatinfo.ProductCount[itemID][j]);
                    }
                    else
                    {
                        labelStyle = j % 2 == 0 ? stylesmallblue : stylesmallyellow;
                    }
                    string number = TGMKinttostringMode ? sb.ToString() : allstatinfo.ProductCount[itemID][j].ToString();
                    GUILayout.Label(number, labelStyle, GUILayout.Width(ColumnWidth[j + 1]), GUILayout.Height(heightdis));
                }
                GUILayout.EndHorizontal();
            }

            // 更新和绘制分页按钮
            int indexnum = (int)(iteminfoshow.Count / 20.5f) + 1;
            ItemInfoShowIndex = Math.Min(ItemInfoShowIndex, indexnum);

            GUILayout.BeginHorizontal();
            for (int i = 1; i <= indexnum && indexnum > 1; i++)
            {
                var style = GUI.skin.button;
                if (i == ItemInfoShowIndex)
                {
                    style = selectedPlanetButtonStyle;
                }
                if (GUILayout.Button(i + "", style, iconoptions))
                    ItemInfoShowIndex = i;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 背景板防穿透
        /// </summary>
        public void UIPanelSet()
        {
            var rt = ui_morestatinfopanel.GetComponent<RectTransform>();
            var Canvasrt = UIRoot.instance.overlayCanvas.GetComponent<RectTransform>();
            float CanvaswidthMultiple = Canvasrt.sizeDelta.x * 1.0f / Screen.width;
            float CanvasheightMultiple = Canvasrt.sizeDelta.y * 1.0f / Screen.height;
            rt.sizeDelta = new Vector2(CanvaswidthMultiple * PanelMaskWidth, CanvasheightMultiple * MainWindowHeight);
            rt.localPosition = new Vector2(-Canvasrt.sizeDelta.x / 2 + MainWindow_x * CanvaswidthMultiple, Canvasrt.sizeDelta.y / 2 - MainWindow_y * CanvasheightMultiple - rt.sizeDelta.y);
        }

        /// <summary>
        /// 筛选条件UI
        /// </summary>
        /// <param name="tempheight1"></param>
        private void FilterUI()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("产物筛选".GetTranslate());
            rawmaterial = GUILayout.Toggle(rawmaterial, "一级原料".GetTranslate());
            secondrawmaterial = GUILayout.Toggle(secondrawmaterial, "二级原料".GetTranslate());
            building = GUILayout.Toggle(building, "建筑".GetTranslate());
            compound = GUILayout.Toggle(compound, "合成材料".GetTranslate());
            onlyseechose = GUILayout.Toggle(onlyseechose, "只看目标产物".GetTranslate());
            SortbyPointProduct = GUILayout.Toggle(SortbyPointProduct, "依目标产物排序".GetTranslate());
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            noreachtheoryproduce = GUILayout.Toggle(noreachtheoryproduce, "实时产量<理论产量".GetTranslate());
            noreachneedproduce = GUILayout.Toggle(noreachneedproduce, "实时产量<需求产量".GetTranslate());
            theorynoreachneedproduce = GUILayout.Toggle(theorynoreachneedproduce, "理论产量<需求产量".GetTranslate());

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // 定义需要创建的Toggle和TextArea的标签和变量
            var toggleValues = new bool[] { producelimittoggle, comsumeimittoggle, theorycomsumelimittoggle, theoryproducelimittoggle };
            string[] textAreas = new string[] { producelowerlimitstr, comsumelowerlimitstr, theorycomsumelowerlimitstr, theoryproducelowerlimitstr };

            GUILayout.BeginHorizontal();

            toggleValues[0] = GUILayout.Toggle(producelimittoggle, "实时产量".GetTranslate());
            textAreas[0] = GUILayout.TextArea(producelowerlimitstr + "/min");

            toggleValues[1] = GUILayout.Toggle(comsumeimittoggle, "实时消耗".GetTranslate());
            textAreas[1] = GUILayout.TextArea(comsumelowerlimitstr + "/min");

            toggleValues[2] = GUILayout.Toggle(theorycomsumelimittoggle, "需求产量".GetTranslate());
            textAreas[2] = GUILayout.TextArea(theorycomsumelowerlimitstr + "/min");

            toggleValues[3] = GUILayout.Toggle(theoryproducelimittoggle, "理论产量".GetTranslate());
            textAreas[3] = GUILayout.TextArea(theoryproducelowerlimitstr + "/min");
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

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

            if (!SequenceCompare.SequenceEqual(toggleValues, new bool[] { producelimittoggle, comsumeimittoggle, theorycomsumelimittoggle, theoryproducelimittoggle }))
            {
                producelimittoggle = toggleValues[0];
                comsumeimittoggle = toggleValues[1];
                theorycomsumelimittoggle = toggleValues[2];
                theoryproducelimittoggle = toggleValues[3];
            }

            var showSelectItems = new List<int>();
            foreach (var itemid in ItemProto.itemIds)
            {
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

                var item = ItemProto.itemProtoById[itemid];
                if (item == null)
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

                showSelectItems.Add(itemid);
            }

            int colnum = (int)MainWindowWidth / heightdis - 2;
            int totalNum = showSelectItems.Count;
            int lines = totalNum / colnum + ((totalNum % colnum) > 0 ? 1 : 0);
            for (int i = 0; i < lines; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < colnum; j++)
                {
                    int index = i * colnum + j;
                    if (index == totalNum) break;
                    var itemid = showSelectItems[index];
                    var item = ItemProto.itemProtoById[itemid];
                    GUIStyle style = emptyStyle;

                    if (Productsearchcondition[itemid])
                    {
                        style = whiteStyle;
                    }

                    if (GUILayout.Button(item.iconSprite.texture, style, iconoptions))
                    {
                        Productsearchcondition[itemid] = !Productsearchcondition[itemid];

                        if (SortbyPointProduct)
                        {
                            if (Productsearchcondition[itemid])
                            {
                                Array.Clear(Productsearchcondition, 0, 12000);
                                Productsearchcondition[itemid] = true;
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 主面板GUI逻辑
        /// </summary>
        private void MainWindowShowFun()
        {
            IsEnglish = Localization.CurrentLanguage.glyph == 0;
            MoveWindow();
            Scaling_Window();
            GUI.DrawTexture(GUI.Window(20210821, new Rect(MainWindow_x, MainWindow_y, MainWindowWidth, MainWindowHeight), MainWindow, "统计面板".GetTranslate() + "(" + VERSION + ")" + "ps:ctrl+↑↓"), mytexture);
        }

        /// <summary>
        /// 星球面板
        /// </summary>
        /// <param name="id"></param>
        private void PlanetWindow(int id)
        {
            try
            {
                GUILayout.BeginVertical();
                int pageItemNumber = (int)(MainWindowHeight / (heightdis + 1) - 2);
                for (int i = (pdselectinfoindex - 1) * pageItemNumber; i < pdselectinfoindex * pageItemNumber && i < planetinfoshow.Count; i++)
                {
                    GUIStyle style = normalPlanetButtonStyle;
                    if (!Multplanetproduct)
                    {
                        if (pointPlanetId == planetinfoshow[i])
                            style = selectedPlanetButtonStyle;
                        if (GUILayout.Button(GameMain.galaxy.PlanetById(planetinfoshow[i]).displayName, style, GUILayout.Height(heightdis)))
                        {
                            pointPlanetId = planetinfoshow[i];
                            RefreshFactory(pointPlanetId);
                        }
                    }
                    else
                    {
                        if (planetsproductinfoshow.Contains(planetinfoshow[i]))
                            style = selectedPlanetButtonStyle;
                        if (GUILayout.Button(GameMain.galaxy.PlanetById(planetinfoshow[i]).displayName, style, GUILayout.Height(heightdis)))
                        {
                            if (planetsproductinfoshow.Contains(planetinfoshow[i]))
                                planetsproductinfoshow.Remove(planetinfoshow[i]);
                            else
                                planetsproductinfoshow.Add(planetinfoshow[i]);
                        }
                    }
                }

                int indexnum = planetinfoshow.Count / pageItemNumber + (planetinfoshow.Count % pageItemNumber == 0 ? 0 : 1);
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                for (int i = pdselectinfoindex > 3 ? pdselectinfoindex - 2 : 1; i <= indexnum; i++)
                {
                    if (GUILayout.Button(i.ToString(), i == pdselectinfoindex ? selectedPlanetButtonStyle : normalPlanetButtonStyle, iconoptions))
                        pdselectinfoindex = i;
                }
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            catch { }
        }

        /// <summary>
        /// 星球面板GUI逻辑
        /// </summary>
        private void PlanetWindowShowFun()
        {
            GUI.DrawTexture(GUI.Window(202108213, new Rect(MainWindow_x + MainWindowWidth, MainWindow_y, heightdis * 7, MainWindowHeight), PlanetWindow, ""), mytexture);
        }

        /// <summary>
        /// 菜单面板
        /// </summary>
        /// <param name="id"></param>
        private void SwitchWindow(int id)
        {
            GUILayout.BeginArea(new Rect(10, 20, switchwitdh, switchheight));
            RemoteorLocal = GUILayout.Toggle(RemoteorLocal, "本地/远程".GetTranslate());
            PlanetorSum = GUILayout.Toggle(PlanetorSum, "全部/星球".GetTranslate());
            StopRefresh = GUILayout.Toggle(StopRefresh, "停止刷新".GetTranslate());
            TGMKinttostringMode = GUILayout.Toggle(TGMKinttostringMode, "单位转化".GetTranslate());
            Filtercondition = GUILayout.Toggle(Filtercondition, "筛选条件".GetTranslate());
            Refreshfactoryinfo = GUILayout.Toggle(Refreshfactoryinfo, "工厂信息".GetTranslate());
            RefreshPlanetinfo = GUILayout.Toggle(RefreshPlanetinfo, "星球信息".GetTranslate());
            Multplanetproduct = GUILayout.Toggle(Multplanetproduct, "多选星球".GetTranslate());

            GUILayout.EndArea();
        }

        /// <summary>
        /// 菜单面板GUI逻辑
        /// </summary>
        private void SwitchWindowShowFun()
        {
            GUI.DrawTexture(GUI.Window(202108212, new Rect(MainWindow_x - switchwitdh, MainWindow_y, switchwitdh, switchheight), SwitchWindow, ""), mytexture);
        }

        #endregion UI

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
        public void MoveWindow()
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
        public void Scaling_Window()
        {
            float x = MainWindowWidth;
            float y = MainWindowHeight;
            if (Input.GetMouseButton(0))
            {
                Vector2 temp = Input.mousePosition;
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
                if ((Screen.height - temp.y + 2 > y + MainWindow_y && Screen.height - temp.y - 2 < y + MainWindow_y) && (temp.x >= MainWindow_x && temp.x <= MainWindow_x + x) || bottomscaling)
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

        internal void Reset()
        {
            Array.Clear(Productsearchcondition, 0, 12000);
            PlanetLoad.LoadPlanetTypeForGalaxy(GameMain.galaxy);
            VeinSearch.Clear();
        }

        #endregion 窗口操作
    }
}