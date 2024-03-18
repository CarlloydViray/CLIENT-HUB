using System.Diagnostics;

namespace BPOI_HUB.model.core
{
    public class DictionaryOps
    {
        public static void DisplayData(Dictionary<string, decimal> d)
        {

            foreach (string key in d.Keys)
            {
                if (d[key] != 0)
                {
                    Debug.WriteLine(key + " : " + d[key]);
                }
            }
            Debug.WriteLine("");

        }

        public static void IncrementDict(ref Dictionary<string, decimal> d, string key, decimal amount)
        {

            if (d.ContainsKey(key))
            {
                d[key] += amount;
            }
            else
            {
                d[key] = amount;
            }


        }
    }
}
