using MoreStatInfo.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreStatInfo.Utils
{
    internal class OutputDataUtil
    {
        /// <summary>
        /// 导出工厂信息
        /// </summary>
        public static void OutputFactoryBuildInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var factorybuildinfoshow = new Dictionary<int, int>();
            stringBuilder.AppendLine("建筑信息");
            foreach (StarData sd in GameMain.galaxy.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    if (pd.factory == null) continue;
                    foreach (EntityData entity in pd.factory.entityPool)
                    {
                        int protoId = entity.protoId;
                        if (protoId <= 0) continue;
                        if (!factorybuildinfoshow.ContainsKey(protoId)) factorybuildinfoshow.Add(protoId, 0);
                        factorybuildinfoshow[protoId]++;
                    }
                }
            }
            stringBuilder.AppendLine("建筑ID,建筑名称,建筑数量");
            foreach (var wap in factorybuildinfoshow)
            {
                stringBuilder.AppendLine($"{wap.Key},{ItemProto.itemProtoById[wap.Key].name},{wap.Value}");
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("电量信息");
            stringBuilder.AppendLine($"{"发电性能".GetTranslate()},{MoreStatInfo.allstatinfo.Powerenergyinfoshow[0] + "W"}");
            stringBuilder.AppendLine($"{"耗电需求".GetTranslate()},{MoreStatInfo.allstatinfo.Powerenergyinfoshow[1] + "W"}");
            stringBuilder.AppendLine($"{"总耗能".GetTranslate()},{MoreStatInfo.allstatinfo.Powerenergyinfoshow[2] + "J"}");
            stringBuilder.AppendLine($"{"放电功率".GetTranslate()},{MoreStatInfo.allstatinfo.Powerenergyinfoshow[3] + "W"}");
            stringBuilder.AppendLine($"{"充电功率".GetTranslate()},{MoreStatInfo.allstatinfo.Powerenergyinfoshow[4] + "W"}");

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("CPU");

            AppendCpuStatEntry(ECpuWorkEntry.Total, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.LogicTick, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Universe, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.PrepareTick, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Icarus, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.LogisticsSchedule, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.PlanetFactory, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.PowerSystem, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.ConstructionSystem, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.LogisticsTransport, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Facilities, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Inserters, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.CargoPaths, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.CargoTrafficMisc, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.CargoPresent, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.StorageSystem, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.GroundCombatUnits, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.GroundDefense, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.GroundDarkFog, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.DigitalSystem, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.DysonSphere, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.SpaceSector, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.SpaceCombatUnits, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.CreationSystem, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.SpaceDarkFog, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.SkillSystem, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Trash, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Statistics, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.WarningSystem, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.PhysicsLogic, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.AudioLogic, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Scenario, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.EndTick, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Update, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.UIUpdate, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.RendererUpdate, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.LateUpdate, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.UILateUpdate, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.RendererLateUpdate, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.DrawingLateUpdate, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.GameEngine0, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Wreckage, stringBuilder);
            AppendCpuStatEntry(ECpuWorkEntry.Profiler, stringBuilder);

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("GPU");
            stringBuilder.AppendLine("," + "物件数量".Translate() + "," + "顶点数量".Translate());
            AppendGpuStatEntry(EGpuWorkEntry.Total, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Universe, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Cloud, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Player, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Creation, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.MechaDrone, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Vegetation, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Vein, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Facility, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Preview, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Belt, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Cargo, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Minimap, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.LogisticCourier, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.LogisticDrone, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.LogisticShip, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Trash, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.DysonSwarm, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.DysonNode, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.DysonFrame, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.DysonShell, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.DysonBullet, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.DysonRocket, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Craft, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Enemy, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.DFCarrier, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.DFAnt, stringBuilder);
            AppendGpuStatEntry(EGpuWorkEntry.Skill, stringBuilder);

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Data(单位Byte)");
            stringBuilder.Append("IO耗时".Translate() + ",");
            stringBuilder.Append(PerformanceMonitor.saveTimeCost.ToString("0.00") + "秒存储");
            stringBuilder.Append(',');
            stringBuilder.Append(PerformanceMonitor.loadTimeCost.ToString("0.00") + "秒读取");
            stringBuilder.Append("\r\n");
            AppendDataStatEntry(ESaveDataEntry.Total, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Header, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Preference, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.History, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Player, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Creation, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Planet, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Factory, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Entity, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Ruin, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.BeltAndCargo, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Storage, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.PowerSystem, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Construction, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Facility, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Miner, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Inserter, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Assembler, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Fractionator, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Ejector, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Silo, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Lab, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Transport, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Platform, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.DysonSphere, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.DysonSwarm, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.DysonBullet, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.DysonNode, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.DysonFrame, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.DysonShell, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.DysonRocket, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Trash, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Digital, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Statistics, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Combat, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Defense, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Skill, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.Craft, stringBuilder);
            AppendDataStatEntry(ESaveDataEntry.DFRelay, stringBuilder);
            File.WriteAllText(Path.Combine(GameConfig.gameSaveFolder, "统计面板".GetTranslate() + "_工厂信息_" + DateTime.Now.ToString("yyyyMMdd-HH_mm_ss") + ".csv"), stringBuilder.ToString());
            Application.OpenURL(GameConfig.gameSaveFolder);
        }

        public static void OutputItemInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine("物品ID,物品名称,实时产出(/s),实时消耗(/s),理论产量(/s),需求产量(/s),生产者数量,消费者数量,仓库总量");
            for (int i = 0; i < ItemProto.itemIds.Length; i++)
            {
                var itemId = ItemProto.itemIds[i];
                if (ItemProto.itemProtoById[itemId] == null) continue;
                sb.Append(itemId + ",");
                sb.Append(ItemProto.itemProtoById[itemId].name + ",");
                for (int j = 0; j <= 6; j++)
                {
                    sb.Append(MoreStatInfo.allstatinfo.ProductCount[itemId][j] + ",");
                }
                sb.AppendLine();
            }
            File.WriteAllText(Path.Combine(GameConfig.gameSaveFolder, "统计面板".GetTranslate() + "_产物信息_" + DateTime.Now.ToString("yyyyMMdd-HH_mm_ss") + ".csv"), sb.ToString());
            Application.OpenURL(GameConfig.gameSaveFolder);
        }

        private static void AppendCpuStatEntry(ECpuWorkEntry entry, StringBuilder label)
        {
            double num = PerformanceMonitor.timeCostsShowing[(int)entry];
            if (num <= -1.0)
            {
                return;
            }
            int num2 = PerformanceMonitor.cpuWorkLevels[(int)entry];
            if (num2 < 0)
            {
                int num3 = (int)PerformanceMonitor.cpuWorkParents[(int)entry];
                if (num3 > 0)
                {
                    num2 = PerformanceMonitor.cpuWorkLevels[num3] + 1;
                }
            }
            switch (num2)
            {
                case 2:
                    label.Append("  > " + PerformanceMonitor.cpuWorkNames[(int)entry].Translate());
                    break;

                case 3:
                    label.Append("    - " + PerformanceMonitor.cpuWorkNames[(int)entry].Translate());
                    break;

                default:
                    label.Append(PerformanceMonitor.cpuWorkNames[(int)entry].Translate());
                    break;
            }
            label.Append(',');
            double num4 = PerformanceMonitor.timeCostsShowing[1];
            label.AppendFormat("{0:0.000} ms", num * 1000.0);
            label.Append(',');
            if (entry == ECpuWorkEntry.Total)
            {
                label.AppendFormat("理论逻辑帧".Translate(), 1.0 / (num + (double)Mathf.Min((float)num, 0.0018f)));
                return;
            }
            if (num4 < 1E-08)
            {
                label.AppendLine("0.0 %");
                return;
            }
            label.AppendFormat("{0:0.0} %\r\n", num / num4 * 100.0);
        }

        private static void AppendDataStatEntry(ESaveDataEntry entry, StringBuilder label)
        {
            long num = PerformanceMonitor.dataLengths[(int)entry];
            if (num <= 0L)
            {
                return;
            }
            int num2 = PerformanceMonitor.dataLenLevels[(int)entry];
            if (num2 < 0)
            {
                int num3 = (int)PerformanceMonitor.dataLenParents[(int)entry];
                if (num3 > 0)
                {
                    num2 = PerformanceMonitor.dataLenLevels[num3] + 1;
                }
            }
            switch (num2)
            {
                case 1:
                    label.Append("  > " + PerformanceMonitor.dataLenNames[(int)entry].Translate());
                    break;

                case 2:
                    label.Append("    - " + PerformanceMonitor.dataLenNames[(int)entry].Translate());
                    break;

                case 3:
                    label.Append("      - " + PerformanceMonitor.dataLenNames[(int)entry].Translate());
                    break;

                default:
                    label.Append(PerformanceMonitor.dataLenNames[(int)entry].Translate());
                    break;
            }
            label.Append(',');
            long num4 = PerformanceMonitor.dataLengths[1];
            label.Append(num + ",");
            if (entry == ESaveDataEntry.Total)
            {
                label.AppendLine();
                return;
            }
            if (num4 == 0L)
            {
                label.AppendLine("0.00 %");
                return;
            }
            label.AppendFormat("{0:0.00} %\r\n", (double)num / num4 * 100.0);
        }

        private static void AppendGpuStatEntry(EGpuWorkEntry entry, StringBuilder label)
        {
            int num = PerformanceMonitor.objectCountsShowing[(int)entry];
            int num2 = PerformanceMonitor.vertexCountsShowing[(int)entry];
            if (num2 > 0 || num > 0)
            {
                label.Append(PerformanceMonitor.gpuWorkNames[(int)entry].Translate());
                label.Append(",");
                label.Append(num);
                label.Append(",");
                label.Append(num2);
                label.AppendLine();
            }
        }
    }
}