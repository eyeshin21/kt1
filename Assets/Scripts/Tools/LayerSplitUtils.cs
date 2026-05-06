using System.Collections.Generic;
using UnityEngine;

namespace Tool
{
    public static class LayerSplitUtils
    {
        /// <summary>
        /// Cắt danh sách steps thành nhiều layer theo layerCounts.
        /// - Nếu thiếu step cho layer tiếp theo -> dừng (không crash).
        /// - Layer count = 0 -> trả list rỗng.
        /// </summary>
        public static List<RandomColorStepLayer> SplitByLayers(
         List<RandomColorStep> steps,
         IList<int> layerCounts)
        {
            var result = new List<RandomColorStepLayer>();
            if (steps == null || layerCounts == null) return result;

            int index = 0;

            for (int i = 0; i < layerCounts.Count; i++)
            {
                int count = Mathf.Max(0, layerCounts[i]);

                var layer = new RandomColorStepLayer();

                if (count == 0)
                {
                    result.Add(layer);
                    continue;
                }

                // thiếu step thì dừng (đúng “cách 1”)
                if (index + count > steps.Count) break;

                layer.steps.AddRange(steps.GetRange(index, count));
                result.Add(layer);

                index += count;
            }

            return result;
        }
    }
}
