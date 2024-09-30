using MoreStatInfo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreStatInfo.Model
{
    public struct AllStatInfo
    {
        public static bool RemoteorLocal;
        public PanelMode CurPanelMode;
        public GalaxyData galaxyData;
        public PlanetStatInfo[] planetstatinfoDic;
        public long[] Powerenergyinfoshow;

        /// <summary>
        /// 产品信息计数，第一个索引是物品id 第二个索引 0是实时产出，1是实时消耗 2是理论产量，3是需求产量 4是生产者数量，5是消费者数量 6是总计，7是本地提供，8是本地需求，9是本地仓储
        /// </summary>
        public long[][] ProductCount;

        public bool[] ProductExist;
        public StarStatInfo[] starStatInfos;
        public int TargetPlanetId;
        public List<int> TargetPlanetIds;

        public void CalculateProductCount()
        {
            ClearAll();
            PlanetStatInfo planetStatinfo;
            switch (CurPanelMode)
            {
                case PanelMode.GalaxyStatInfo:
                    for (int i = 0; i < starStatInfos.Length; i++)
                    {
                        var starStatinfo = starStatInfos[i];
                        for (int j = 0; j < ItemProto.itemIds.Length; j++)
                        {
                            var itemId = ItemProto.itemIds[j];
                            for (int k = 0; k < 10; k++)
                            {
                                ProductCount[itemId][k] += starStatinfo.ProductCount[itemId][k];
                            }
                            if (!ProductExist[itemId])
                            {
                                ProductExist[itemId] = ProductExist[itemId] || starStatinfo.ProductExist[itemId];
                            }
                        }
                        for (int j = 0; j < 5; j++)
                        {
                            Powerenergyinfoshow[j] += starStatinfo.Powerenergyinfoshow[j];
                        }
                    }
                    break;

                case PanelMode.MultiplePlanetStatInfo:
                    for (int i = 0; i < TargetPlanetIds.Count; i++)
                    {
                        int planetId = TargetPlanetIds[i];
                        if (!planetstatinfoDic[planetId].IsUsed)
                        {
                            continue;
                        }
                        planetStatinfo = planetstatinfoDic[planetId];
                        for (int j = 0; j < ItemProto.itemIds.Length; j++)
                        {
                            var itemId = ItemProto.itemIds[j];
                            for (int k = 0; k < 10; k++)
                            {
                                ProductCount[itemId][k] += planetStatinfo.ProductCount[itemId][k];
                            }
                            if (!ProductExist[itemId])
                            {
                                ProductExist[itemId] = ProductExist[itemId] || planetStatinfo.ProductExist[itemId];
                            }
                        }
                        for (int j = 0; j < 5; j++)
                        {
                            Powerenergyinfoshow[j] += planetStatinfo.Powerenergyinfoshow[j];
                        }
                    }
                    break;

                case PanelMode.TargetSinglePlanetStatInfo:
                    if (!planetstatinfoDic[TargetPlanetId].IsUsed)
                    {
                        break;
                    }
                    planetStatinfo = planetstatinfoDic[TargetPlanetId];
                    for (int j = 0; j < ItemProto.itemIds.Length; j++)
                    {
                        var itemId = ItemProto.itemIds[j];
                        for (int k = 0; k < 10; k++)
                        {
                            ProductCount[itemId][k] += planetStatinfo.ProductCount[itemId][k];
                        }
                        if (!ProductExist[itemId])
                        {
                            ProductExist[itemId] = ProductExist[itemId] || planetStatinfo.ProductExist[itemId];
                        }
                    }
                    Array.Copy(planetStatinfo.Powerenergyinfoshow, Powerenergyinfoshow, 5);
                    break;
            }
        }

        public void ClearAll()
        {
            Array.Clear(Powerenergyinfoshow, 0, 5);
            Array.Clear(ProductExist, 0, 12000);
            for (int i = 0; i < ItemProto.itemIds.Length; i++)
            {
                var itemId = ItemProto.itemIds[i];
                Array.Clear(ProductCount[itemId], 0, 10);
            }
        }

        public void Collect(GalaxyData galaxyData)
        {
            Array.Clear(planetstatinfoDic, 0, planetstatinfoDic.Length);
            this.galaxyData = galaxyData;
            starStatInfos = new StarStatInfo[galaxyData.starCount];
            for (int i = 0; i < starStatInfos.Length; i++)
            {
                var sd = galaxyData.stars[i];
                starStatInfos[i] = new StarStatInfo();
                starStatInfos[i].Init();
                starStatInfos[i].Collect(sd);
                for (int j = 0; j < starStatInfos[i].planetStatInfos.Length; j++)
                {
                    ref PlanetStatInfo planetstatinfo = ref starStatInfos[i].planetStatInfos[j];
                    if (planetstatinfo.pd.id >= planetstatinfoDic.Length)
                    {
                        Array.Resize(ref planetstatinfoDic, planetstatinfoDic.Length + 100);
                    }
                    planetstatinfoDic[planetstatinfo.pd.id] = planetstatinfo;
                }
            }
        }

        public void Collect()
        {
            switch (CurPanelMode)
            {
                case PanelMode.GalaxyStatInfo:
                    for (int i = 0; i < starStatInfos.Length; i++)
                    {
                        ref StarStatInfo starStatinfo = ref starStatInfos[i];
                        starStatinfo.Collect();
                    }
                    break;

                case PanelMode.MultiplePlanetStatInfo:
                    for (int i = 0; i < TargetPlanetIds.Count; i++)
                    {
                        int planetId = TargetPlanetIds[i];
                        if (!planetstatinfoDic[planetId].IsUsed)
                        {
                            continue;
                        }
                        ref PlanetStatInfo planetStatinfo = ref planetstatinfoDic[planetId];
                        planetStatinfo.Collect();
                    }
                    break;

                case PanelMode.TargetSinglePlanetStatInfo:
                    if (!planetstatinfoDic[TargetPlanetId].IsUsed)
                    {
                        break;
                    }
                    ref PlanetStatInfo planetStatinfo2 = ref planetstatinfoDic[TargetPlanetId];
                    planetStatinfo2.Collect();
                    break;
            }
        }

        public void Init()
        {
            ProductCount = new long[12000][];
            ProductExist = new bool[12000];
            Powerenergyinfoshow = new long[5];
            TargetPlanetIds = new List<int>();
            planetstatinfoDic = new PlanetStatInfo[100];
            for (int i = 0; i < 12000; i++)
            {
                //var itemId = ItemProto.itemIds[i];
                ProductCount[i] = new long[10];
            }
        }

        /// <summary>
        /// 刷新所有
        /// </summary>
        public void RefreshProperty(PanelMode panelMode)
        {
            CurPanelMode = PanelMode.GalaxyStatInfo;
            Collect();
            CurPanelMode = panelMode;
            CalculateProductCount();
        }
    }
}