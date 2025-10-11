using MoreStatInfo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreStatInfo.Utils
{
    internal class PlanetLoad
    {
        public static List<string> PlanetType = new List<string>();
        public static int unloadPlanetNum = 0;

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