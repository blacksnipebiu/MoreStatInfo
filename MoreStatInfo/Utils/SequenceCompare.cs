using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreStatInfo.Utils
{
    internal class SequenceCompare
    {
        // 定义SequenceEqual方法，用于比较两个bool类型的数组是否相等
        public static bool SequenceEqual(bool[] array1, bool[] array2)
        {
            if (array1 == null || array2 == null)
            {
                return array1 == null && array2 == null;
            }
            if (array1.Length != array2.Length)
            {
                return false;
            }
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}