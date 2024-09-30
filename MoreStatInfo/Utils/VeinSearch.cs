using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreStatInfo.Utils
{
    internal class VeinSearch
    {
        public static long[] veinAmounts = new long[64];
        private static HashSet<int> CalcVeinAmountHashSet = new HashSet<int>();
        private static Dictionary<int, int[]> PlanetVeinCountPool = new Dictionary<int, int[]>();

        public static void CalcVeinAmounts(PlanetData pd)
        {
            pd.CalcVeinAmounts(ref veinAmounts, CalcVeinAmountHashSet, UIRoot.instance.uiGame.veinAmountDisplayFilter);
        }

        public static void Clear()
        {
            PlanetVeinCountPool.Clear();
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
            if (pd.gasItems != null) return 0;
            VeinSearch.CalcVeinAmounts(pd);
            if (evt == EVeinType.Oil)
            {
                int collectspeed = (int)(VeinSearch.veinAmounts[7] * VeinData.oilSpeedMultiplier + 0.5);
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
    }
}