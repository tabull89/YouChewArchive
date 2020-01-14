using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive.Classes
{
    public class MemberFilter
    {
        public int MemberId { get; set; }
        public Dictionary<Type, List<int>> ExcludedMap { get; set; } = new Dictionary<Type, List<int>>();
        public int? minDate { get; set; }
        public int? maxDate { get; set; }

        public List<int> GetExcludedIds<T>()
        {
            Type type = typeof(T);

            List<int> ret;

            if(!ExcludedMap.TryGetValue(type, out ret))
            {
                ret = new List<int>();
            }

            return ret;
        }

        public void AddExcludedIds<T>(params int[] ids)
        {
            AddExcludedIds<T>(ids.ToList());
        }

        public void AddExcludedIds<T>(List<int> ids)
        {
            List<int> excludedIds;
            Type type = typeof(T);

            if(!ExcludedMap.TryGetValue(type, out excludedIds))
            {
                excludedIds = new List<int>(ids);

                ExcludedMap.Add(type, excludedIds);
            }
            else
            {
                ExcludedMap[type].AddRange(ids);
            }
        }

        
    }
}
