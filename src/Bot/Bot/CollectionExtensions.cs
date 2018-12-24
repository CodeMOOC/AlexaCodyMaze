using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot {

    public static class CollectionExtensions {

        public static T SafeGet<K, T>(this IDictionary<K, object> d, K key, T defaultValue) {
            if(d == null) {
                return defaultValue;
            }

            if(!d.ContainsKey(key)) {
                return defaultValue;
            }

            object v = d[key];
            if(v is T) {
                return (T)v;
            }
            else {
                return defaultValue;
            }
        }

    }

}
