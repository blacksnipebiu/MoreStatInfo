using System.Collections.Generic;

namespace MoreStatInfo
{
    public static class MoreStatInfoTranslate
    {
        private static Dictionary<string, string> TranslateDict = new Dictionary<string, string>();

        public static string GetItemText(this int itemId)
        {
            string waterType = "无".GetTranslate();
            if (itemId < 0)
            {
                switch (itemId)
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
                var wateritem = LDB.items.Select(itemId);
                if (wateritem != null)
                    waterType = wateritem.name;
            }
            return waterType;
        }

        public static string GetSearchchinese(this int Index)
        {
            if (0 < Index && Index < 15) return ItemProto.itemProtoById[LDB.veins.Select(Index).MiningItem].name;
            if (Index == 15) return "可燃冰".GetTranslate();
            if (Index == 16) return "氢".GetTranslate();
            if (Index == 17) return "重氢".GetTranslate();
            if (Index == 18) return "水".GetTranslate();
            if (Index == 19) return "硫酸".GetTranslate();
            if (Index == 20) return "潮汐锁定".GetTranslate();
            if (Index == 21) return "轨道共振1:2".GetTranslate();
            if (Index == 22) return "轨道共振1:4".GetTranslate();
            if (Index == 23) return "横躺自传".GetTranslate();
            if (Index == 24) return "反向自传".GetTranslate();
            if (Index == 25) return "多卫星".GetTranslate();
            return "";
        }

        public static string GetTranslate(this string s) => MoreStatInfo.IsEnglish && TranslateDict.ContainsKey(s) && TranslateDict[s].Length > 0 ? TranslateDict[s] : s;

        public static void regallTranslate()
        {
            TranslateDict.Clear();
            TranslateDict.Add("放电功率", "Discharging power");
            TranslateDict.Add("充电功率", "Charging power");
            TranslateDict.Add("统计面板", "Stat Pannel");
            TranslateDict.Add("可燃冰", "Fire ice");
            TranslateDict.Add("氢", "Hydrogen");
            TranslateDict.Add("重氢", "Deuterium");
            TranslateDict.Add("水", "Water");
            TranslateDict.Add("硫酸", "Sulfuric acid");
            TranslateDict.Add("潮汐锁定", "Tidal locking perpetual day and night");
            TranslateDict.Add("轨道共振1:2", "Orbital resonance 1:2");
            TranslateDict.Add("轨道共振1:4", "Orbital resonance 1:4");
            TranslateDict.Add("距离玩家", "Distance from player");
            TranslateDict.Add("横躺自传", "Horizontal rotation");
            TranslateDict.Add("反向自传", "Reverse rotation");
            TranslateDict.Add("多卫星", "Satellite");
            TranslateDict.Add("工厂状态", "Factory Status");
            TranslateDict.Add("有", "Existed");
            TranslateDict.Add("电力不足", "Insufficient power");
            TranslateDict.Add("产物筛选", "Item filter");
            TranslateDict.Add("一级原料", "rawmaterial");
            TranslateDict.Add("二级原料", "secondrawmaterial");
            TranslateDict.Add("建筑", "Building");
            TranslateDict.Add("合成材料", "Compound");
            TranslateDict.Add("只看目标产物", "only show point item");
            TranslateDict.Add("依目标产物排序", "Sort by point item's prod.");
            TranslateDict.Add("实时产量<理论产量", "prod.<Theoretical prod.");
            TranslateDict.Add("实时产量<需求产量", "prod.<Theoretical cons.");
            TranslateDict.Add("理论产量<需求产量", "Theoretical prod.<Theoretical cons.");
            TranslateDict.Add("发电性能", "Generation capacity");
            TranslateDict.Add("耗电需求", "Consumption demand");
            TranslateDict.Add("总耗能", "Total energy consumption");
            TranslateDict.Add("物品", "ItemName");
            TranslateDict.Add("无", "None");
            TranslateDict.Add("生产者", "Producer");
            TranslateDict.Add("消费者", "Consumer");
            TranslateDict.Add("总计", "Total");
            TranslateDict.Add("本地提供", "LocalProvide");
            TranslateDict.Add("本地需求", "LocalDemand");
            TranslateDict.Add("本地仓储", "LocalStore");
            TranslateDict.Add("远程提供", "RemoteProvide");
            TranslateDict.Add("远程需求", "RemoteDemand");
            TranslateDict.Add("远程仓储", "RemoteStore");
            TranslateDict.Add("实时产量", "Prodcution");
            TranslateDict.Add("实时消耗", "Consumption");
            TranslateDict.Add("需求产量", "Theoretical cons.");
            TranslateDict.Add("理论产量", "Theoretical prod.");
            TranslateDict.Add("围绕行星", "Orbit Around Planet");
            TranslateDict.Add("恒星亮度", "Star lumination");
            TranslateDict.Add("恒星信息", "Star information");
            TranslateDict.Add("星球类型", "Planet type");
            TranslateDict.Add("海洋类型", "Ocean type");
            TranslateDict.Add("行星信息", "Planet information");
            TranslateDict.Add("方向指引", "Indicator");
            TranslateDict.Add("目标星球矿物", "VeinType");
            TranslateDict.Add("附属气态星产物", "GasType");
            TranslateDict.Add("星球特殊性", "PlanetSingularity");

            TranslateDict.Add("本地/远程", "Local/Remote");
            TranslateDict.Add("全部/星球", "Total/Planets");
            TranslateDict.Add("停止刷新", "StopRefresh");
            TranslateDict.Add("单位转化", "Unit conversion");
            TranslateDict.Add("筛选条件", "FilterCondition");
            TranslateDict.Add("工厂信息", "FactoryInfo");
            TranslateDict.Add("星球信息", "PlanetInfo");
            TranslateDict.Add("多选星球", "Multiple select planets");
            TranslateDict.Add("防止穿透", "Close White Panel");
            TranslateDict.Add("加载全部星球", "Load All Planets");
            TranslateDict.Add("已加载星球", "Loaded Planets");
            TranslateDict.Add("未加载星球", "Unloaded Planets");
            TranslateDict.Add("个星球加载中", "Unloaded Planets");
            TranslateDict.Add("未选择", "UnSelected");
        }
    }
}