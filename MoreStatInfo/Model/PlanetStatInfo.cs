using MoreStatInfo.Model;
using MoreStatInfo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreStatInfo
{
    public struct PlanetStatInfo
    {
        public bool Existfactory;
        public bool IsUsed;
        public PlanetData pd;
        public int[] planetsveinSpotsSketch;
        public long[] Powerenergyinfoshow;

        /// <summary>
        /// 产品信息计数，第一个索引是物品id 第二个索引 0是实时产出，1是实时消耗 2是理论产量，3是需求产量 4是生产者数量，5是消费者数量 6是总计，7是本地提供，8是本地需求，9是本地仓储
        /// </summary>
        public long[][] ProductCount;

        public int productCursor;
        public bool[] ProductExist;
        public Dictionary<int, float> TheoryProductDiction;
        public Dictionary<int, float> TheoryRequireDiction;
        public bool Lackofelectricity => Powerenergyinfoshow[0] + Powerenergyinfoshow[3] > Powerenergyinfoshow[1];

        public void ClearAll()
        {
            productCursor = 0;
            Array.Clear(Powerenergyinfoshow, 0, 5);
            Array.Clear(ProductExist, 0, 12000);
            Existfactory = false;
            for (int i = 0; i < ItemProto.itemIds.Length; i++)
            {
                var itemId = ItemProto.itemIds[i];
                Array.Clear(ProductCount[itemId], 0, 10);
                TheoryProductDiction[itemId] = 0;
                TheoryRequireDiction[itemId] = 0;
            }
        }

        public void Collect()
        {
            if (pd.factory == null)
            {
                return;
            }
            ClearAll();
            FactoryProductionStat factoryProduction = GameMain.data.statistics.production.factoryStatPool[pd.factory.index];
            if (factoryProduction == null) return;
            int[] itemIds = ItemProto.itemIds;
            for (int i = 0; i < itemIds.Length; i++)
            {
                int itemId = itemIds[i];
                int productPoolIndex = factoryProduction.productIndices[itemId];
                if (productPoolIndex == 0)
                {
                    continue;
                }
                productCursor++;
                ProductExist[itemId] = true;
                ProductCount[itemId][0] += factoryProduction.productPool[productPoolIndex].total[1];
                ProductCount[itemId][1] += factoryProduction.productPool[productPoolIndex].total[8];
            }
            FactorySystem fs = pd.factory.factorySystem;
            Existfactory = fs.factory.entityCount > 0;
            MinerPoolCollect(fs);
            FractionatorPoolCollect(fs);
            SiloPoolCollect(fs);
            EjectorPoolCollect(fs);
            AssemblerPoolCollect(fs);
            LabPoolCollect(fs);
            PowerGeneratorComponentCollect(fs);
            PowerExchangerComponentCollect(fs);
            StorageInfoCollect(fs);
            StationComponentCollect(fs);
            PowerenergyinfoshowCollect(factoryProduction);
            for (int i = 0; i < ItemProto.itemIds.Length; i++)
            {
                int itemId = ItemProto.itemIds[i];
                ProductCount[itemId][2] = (long)TheoryProductDiction[itemId];
                ProductCount[itemId][3] = (long)TheoryRequireDiction[itemId];
            }
        }

        /// <summary>
        /// 只执行一次
        /// </summary>
        public void Init()
        {
            IsUsed = true;
            TheoryProductDiction = new Dictionary<int, float>();
            TheoryRequireDiction = new Dictionary<int, float>();
            Powerenergyinfoshow = new long[5];
            ProductCount = new long[12000][];
            ProductExist = new bool[12000];
            for (int i = 0; i < ItemProto.itemIds.Length; i++)
            {
                var itemId = ItemProto.itemIds[i];
                ProductCount[itemId] = new long[10];
                TheoryProductDiction.Add(itemId, 0);
                TheoryRequireDiction.Add(itemId, 0);
            }
        }

        internal void Collect(PlanetData pd)
        {
            this.pd = pd;
            VeinInfoCal();
        }

        /// <summary>
        /// 组装机数据采集
        /// </summary>
        /// <param name="fs"></param>
        void AssemblerPoolCollect(FactorySystem fs)
        {
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
                    ProductCount[rp.Items[i]][5]++;

                    if (ac.extraSpeed == 0)
                        TheoryRequireDiction[rp.Items[i]] += rp.ItemCounts[i] * 9.0f * ac.speedOverride / (rp.TimeSpend * 25.0f);
                    else
                        TheoryRequireDiction[rp.Items[i]] += rp.ItemCounts[i] * 9.0f * ac.speed / (rp.TimeSpend * 25.0f);
                }
                for (int i = 0; i < rp.Results.Length; i++)
                {
                    ProductCount[rp.Results[i]][4]++;
                    if (ac.extraSpeed == 0)
                        TheoryProductDiction[rp.Results[i]] += rp.ResultCounts[i] * 9.0f * ac.speedOverride / (rp.TimeSpend * 25.0f);
                    else
                        TheoryProductDiction[rp.Results[i]] += rp.ResultCounts[i] * 9.0f * (ac.speed + ac.extraSpeed / 10) / (rp.TimeSpend * 25.0f);
                }
            }
        }

        /// <summary>
        /// 太阳帆发射器
        /// </summary>
        /// <param name="fs"></param>
        void EjectorPoolCollect(FactorySystem fs)
        {
            foreach (EjectorComponent ec in fs.ejectorPool)
            {
                if (ec.id <= 0 || ec.entityId <= 0)
                {
                    continue;
                }
                ProductCount[1501][5]++;
                TheoryRequireDiction[1501] += 20;
            }
        }

        /// <summary>
        /// 分馏塔
        /// </summary>
        /// <param name="fs"></param>
        void FractionatorPoolCollect(FactorySystem fs)
        {
            foreach (FractionatorComponent fc in fs.fractionatorPool)
            {
                if (fc.id <= 0 || fc.entityId <= 0)
                {
                    continue;
                }
                ProductCount[1121][4]++;
                ProductCount[1120][5]++;
                float num = Mathf.Clamp(Mathf.Min(fc.fluidInputCargoCount, 30) * (int)(fc.fluidInputCount / (double)fc.fluidInputCargoCount + 0.5) * 60, 0, 7200) * (1 + fc.extraIncProduceProb) / 100;
                TheoryProductDiction[1121] += num;
                TheoryRequireDiction[1120] += num;
            }
        }

        /// <summary>
        /// 研究站数据采集
        /// </summary>
        /// <param name="fs"></param>
        void LabPoolCollect(FactorySystem fs)
        {
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
                    ProductCount[rp.Items[i]][5]++;

                    if (flag)
                    {
                        TheoryRequireDiction[rp.Items[i]] += rp.ItemCounts[i] * num * 1f / (1f + lc.extraSpeed * 1f / (lc.speed * 10f));
                    }
                    else
                    {
                        TheoryRequireDiction[rp.Items[i]] += rp.ItemCounts[i] * num;
                    }
                }
                for (int i = 0; i < rp.Results.Length; i++)
                {
                    ProductCount[rp.Results[i]][4]++;

                    TheoryProductDiction[rp.Results[i]] += num * rp.ResultCounts[i];
                }
            }
        }

        /// <summary>
        /// 矿机数据采集
        /// </summary>
        /// <param name="fs"></param>
        void MinerPoolCollect(FactorySystem fs)
        {
            foreach (MinerComponent mc in fs.minerPool)
            {
                if (mc.id <= 0 || mc.entityId <= 0)
                {
                    continue;
                }
                if (mc.type == EMinerType.Water && pd.waterItemId > 0)
                {
                    TheoryProductDiction[pd.waterItemId] += (long)(50 * GameMain.history.miningSpeedScale);
                }
                else if (mc.type == EMinerType.Vein && mc.veinCount > 0)
                {
                    int itemId = 0;
                    for (int i = 0; i < mc.veins.Length; i++)
                    {
                        if (mc.veins[i] == 0 || mc.veins[i] >= fs.factory.veinPool.Length) continue;
                        VeinData vd = fs.factory.veinPool[mc.veins[i]];
                        if (vd.productId == 0) continue;
                        itemId = vd.productId;
                        break;
                    }
                    ProductCount[itemId][4]++;
                    bool isveincollector = pd.factory.entityPool[mc.entityId].stationId > 0;
                    TheoryProductDiction[itemId] += (long)(30 * (isveincollector ? 2 : 1) * mc.veinCount * GameMain.history.miningSpeedScale * mc.speed) / 10000;
                }
                else if (mc.type == EMinerType.Oil && mc.veinCount > 0)
                {
                    for (int i = 0; i < mc.veins.Length; i++)
                    {
                        if (mc.veins[i] == 0 || mc.veins[i] >= fs.factory.veinPool.Length) continue;
                        VeinData vd = fs.factory.veinPool[mc.veins[i]];
                        ProductCount[1007][4]++;
                        TheoryProductDiction[1007] += (long)(vd.amount * GameMain.history.miningSpeedScale * VeinData.oilSpeedMultiplier * 60);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 发电信息收集
        /// </summary>
        /// <param name="factoryProduction"></param>
        void PowerenergyinfoshowCollect(FactoryProductionStat factoryProduction)
        {
            if (factoryProduction.powerPool != null && factoryProduction.powerPool.Length > 0)
            {
                Powerenergyinfoshow[0] = factoryProduction.powerPool[0].total[0] / 10;
                Powerenergyinfoshow[1] = factoryProduction.powerPool[1].total[0] / 10;
                Powerenergyinfoshow[2] = factoryProduction.energyConsumption;
                Powerenergyinfoshow[3] = factoryProduction.powerPool[3].total[0] / 10;
                Powerenergyinfoshow[4] = factoryProduction.powerPool[2].total[0] / 10;
            }
        }

        /// <summary>
        /// 能量枢纽信息采集
        /// </summary>
        /// <param name="fs"></param>
        void PowerExchangerComponentCollect(FactorySystem fs)
        {
            foreach (PowerExchangerComponent pec in fs.factory.powerSystem.excPool)
            {
                if (pec.id <= 0 || pec.targetState == 0)
                {
                    continue;
                }
                int product = pec.targetState == 1 ? 2207 : 2206;
                int requireitem = 4413 - product;

                ProductCount[product][4]++;
                ProductCount[requireitem][5]++;
                TheoryProductDiction[product] += 10;
                TheoryRequireDiction[requireitem] += 10;
            }
        }

        /// <summary>
        /// 发电设备信息采集
        /// </summary>
        /// <param name="fs"></param>
        void PowerGeneratorComponentCollect(FactorySystem fs)
        {
            float sum = 0;
            foreach (PowerGeneratorComponent pgc in fs.factory.powerSystem.genPool)
            {
                if (!pgc.gamma)
                {
                    continue;
                }
                float eta = 1f - GameMain.history.solarEnergyLossRate;
                sum += pgc.EtaCurrent_Gamma(eta) * pgc.RequiresCurrent_Gamma(eta);
                ProductCount[1208][4]++;
            }
            if (sum > 0)
            {
                TheoryProductDiction[1208] = (long)(sum * 3.0f / 1000000.0f);
            }
        }

        /// <summary>
        /// 火箭发射井
        /// </summary>
        /// <param name="fs"></param>
        void SiloPoolCollect(FactorySystem fs)
        {
            foreach (SiloComponent sc in fs.siloPool)
            {
                if (sc.id <= 0 || sc.entityId <= 0)
                {
                    continue;
                }
                ProductCount[1503][5]++;
                TheoryRequireDiction[1503] += 5;
            }
        }

        /// <summary>
        /// 物流站信息采集
        /// </summary>
        /// <param name="fs"></param>
        void StationComponentCollect(FactorySystem fs)
        {
            int tempi = 0;
            foreach (StationComponent sc in fs.factory.transport.stationPool)
            {
                if (sc != null && sc.entityId > 0)
                {
                    tempi++;
                    for (int i = 0; i < sc.storage.Length; i++)
                    {
                        int itemId = sc.storage[i].itemId;
                        if (itemId <= 0)
                        {
                            continue;
                        }
                        ProductCount[itemId][6] += sc.storage[i].count;
                        if (!AllStatInfo.RemoteorLocal)
                        {
                            if (sc.storage[i].localLogic == ELogisticStorage.Supply)
                                ProductCount[itemId][7] += sc.storage[i].count;
                            else if (sc.storage[i].localLogic == ELogisticStorage.Demand)
                                ProductCount[itemId][8] += sc.storage[i].count;
                            else
                                ProductCount[itemId][9] += sc.storage[i].count;
                        }
                        else if (AllStatInfo.RemoteorLocal && (sc.isCollector || sc.isStellar))
                        {
                            if (sc.storage[i].remoteLogic == ELogisticStorage.Supply)
                                ProductCount[itemId][7] += sc.storage[i].count;
                            else if (sc.storage[i].remoteLogic == ELogisticStorage.Demand)
                                ProductCount[itemId][8] += sc.storage[i].count;
                            else
                                ProductCount[itemId][9] += sc.storage[i].count;
                        }
                    }
                    float miningSpeedScale = GameMain.history.miningSpeedScale;
                    int pdId = pd.id;
                    string scName = fs.factory.ReadExtraInfoOnEntity(sc.entityId);

                    //星球矿机产量适配
                    //if (string.IsNullOrEmpty(scName) && sc.isStellar && (scName.Equals("Station_miner") || scName.Equals("星球矿机")))
                    //{
                    //    for (int i = 0; i < 5; i++)
                    //    {
                    //        int itemId = sc.storage[i].itemId;
                    //        if (itemId <= 0)
                    //        {
                    //            continue;
                    //        }
                    //        ProducerDiction[itemId]++;
                    //        if (pd.waterItemId == itemId)
                    //            TheoryProductDiction[itemId] += (long)(1800 * miningSpeedScale);
                    //        else
                    //            TheoryProductDiction[itemId] += itemId != 1007 ? (int)(getVeinnumber(itemId, pdId) * miningSpeedScale) / 2 * 60 : (int)(getVeinnumber(itemId, pdId) * miningSpeedScale) * 60;
                    //    }
                    //}
                    if (sc.collectionPerTick != null && sc.isCollector)
                    {
                        PrefabDesc prefabDesc = LDB.items.Select(ItemProto.stationCollectorId).prefabDesc;
                        double collectorsWorkCost = prefabDesc.workEnergyPerTick * 60.0;
                        collectorsWorkCost /= prefabDesc.stationCollectSpeed;
                        double gasTotalHeat = pd.gasTotalHeat;
                        float collectSpeedRate = gasTotalHeat - collectorsWorkCost <= 0.0 ? 1f : (float)((miningSpeedScale * gasTotalHeat - collectorsWorkCost) / (gasTotalHeat - collectorsWorkCost));
                        for (int index = 0; index < sc.collectionIds.Length; ++index)
                        {
                            int itemId = sc.storage[index].itemId;
                            TheoryProductDiction[itemId] += (long)(sc.collectionPerTick[index] * collectSpeedRate * 3600);
                            ProductCount[itemId][4]++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 仓库和储液罐信息采集
        /// </summary>
        /// <param name="fs"></param>
        void StorageInfoCollect(FactorySystem fs)
        {
            if (fs.storage != null)
            {
                if (fs.storage.storagePool != null)
                {
                    foreach (StorageComponent sc in fs.storage.storagePool)
                    {
                        if (sc == null || sc.entityId <= 0 || sc.isEmpty)
                        {
                            continue;
                        }
                        for (int i = 0; i < sc.grids.Length; i++)
                        {
                            int itemId;
                            if ((itemId = sc.grids[i].itemId) <= 0)
                            {
                                continue;
                            }
                            ProductCount[itemId][6] += sc.grids[i].count;
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
                        ProductCount[itemId][6] += tc.fluidCount;
                    }
                }
            }
        }

        void VeinInfoCal()
        {
            planetsveinSpotsSketch = VeinSearch.veinSpotsSketch(pd);
        }
    }

    public static class PlanetStatInfoExtension
    {
        public static bool ExistFactory(this PlanetData pd)
        {
            return MoreStatInfo.allstatinfo.planetstatinfoDic[pd.id].Existfactory;
        }

        public static PlanetStatInfo GetPlanetStatInfo(this PlanetData pd)
        {
            return MoreStatInfo.allstatinfo.planetstatinfoDic[pd.id];
        }
    }
}