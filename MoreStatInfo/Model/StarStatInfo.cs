using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreStatInfo.Model
{
    public class StarStatInfo
    {
        public PlanetStatInfo[] planetStatInfos;
        public long[] Powerenergyinfoshow;

        /// <summary>
        /// 产品信息计数，第一个索引是物品id 第二个索引 0是实时产出，1是实时消耗 2是理论产量，3是需求产量 4是生产者数量，5是消费者数量 6是总计，7是本地提供，8是本地需求，9是本地仓储
        /// </summary>
        public long[][] ProductCount;

        public bool[] ProductExist;

        public StarData starData;

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

        public void Collect(StarData starData)
        {
            this.starData = starData;
            planetStatInfos = new PlanetStatInfo[starData.planetCount];
            for (int i = 0; i < planetStatInfos.Length; i++)
            {
                var pd = starData.planets[i];
                planetStatInfos[i] = new PlanetStatInfo();
                planetStatInfos[i].Init();
                planetStatInfos[i].Collect(pd);
            }
            Collect();
        }

        public void Collect()
        {
            ClearAll();
            for (int i = 0; i < planetStatInfos.Length; i++)
            {
                PlanetStatInfo planetstatinfo = planetStatInfos[i];
                planetstatinfo.Collect();
                for (int j = 0; j < ItemProto.itemIds.Length; j++)
                {
                    var itemId = ItemProto.itemIds[j];
                    for (int k = 0; k < 10; k++)
                    {
                        ProductCount[itemId][k] += planetstatinfo.ProductCount[itemId][k];
                    }
                    if (!ProductExist[itemId])
                    {
                        ProductExist[itemId] = ProductExist[itemId] || planetstatinfo.ProductExist[itemId];
                    }
                }
                for (int j = 0; j < 5; j++)
                {
                    Powerenergyinfoshow[j] += planetstatinfo.Powerenergyinfoshow[j];
                }
            }
        }

        public void Init()
        {
            ProductCount = new long[12000][];
            ProductExist = new bool[12000];
            Powerenergyinfoshow = new long[5];
            for (int i = 0; i < ItemProto.itemIds.Length; i++)
            {
                var itemId = ItemProto.itemIds[i];
                ProductCount[itemId] = new long[10];
            }
        }
    }
}