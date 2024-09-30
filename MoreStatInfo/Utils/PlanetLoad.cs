using MoreStatInfo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreStatInfo.Utils
{
    internal class PlanetLoad
    {
        public static List<string> PlanetType = new List<string>();
        public static int unloadPlanetNum = 0;

        /// <summary>
        /// 加载全部星球
        /// </summary>
        public static void LoadAllPlanet()
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

        public static void LoadPlanetTypeForGalaxy(GalaxyData galaxyData)
        {
            PlanetType.Clear();
            foreach (StarData sd in galaxyData.stars)
            {
                foreach (PlanetData pd in sd.planets)
                {
                    if (PlanetType.Contains(pd.typeString)) continue;
                    PlanetType.Add(pd.typeString);
                }
            }
        }
    }
}