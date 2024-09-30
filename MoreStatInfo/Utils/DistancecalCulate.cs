using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreStatInfo.Utils
{
    internal class DistancecalCulate
    {
        public static double PlayerToPlanetDis(PlanetData pd)
        {
            VectorLF3 vectorLF2 = pd.uPosition - GameMain.mainPlayer.uPosition;
            double num2 = vectorLF2.magnitude - pd.realRadius - 60;
            return num2 < 0 ? 0 : num2;
        }

        public static string PlayerToPlanetDisTranslate(PlanetData pd)
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
    }
}