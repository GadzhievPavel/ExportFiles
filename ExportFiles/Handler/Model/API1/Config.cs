using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.Plugins;
using TFlex.DOCs.References.Configurations;

namespace ExportFiles.Handler.Model.API1
{
    public class Config
    {
        private Dictionary<string, DataReferenceObject> dict;
        public Config(Dictionary<string, DataReferenceObject> valuePairs)
        {
            dict = valuePairs;
        }

        public dynamic Get(string key)
        {
            dict.TryGetValue(key, out DataReferenceObject val);
            if(val is null)
            {
                throw new ArgumentNullException($"ключ {key} отсутствует в конфиге");
            }
            return val.GetValue();
        }

        public dynamic this[string key]
        {
            get
            {
                return Get(key);
            }
        }
    }
}
