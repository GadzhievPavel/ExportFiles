using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            dict.TryGetValue(key, out var value);
            return value.GetValue();
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
