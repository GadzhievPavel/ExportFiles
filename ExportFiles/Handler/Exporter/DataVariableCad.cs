﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.References.Files;

namespace ExportFiles.Handler.Exporter
{
    public class DataVariableCad:IEnumerable<string>
    {
        private Dictionary<string, object> data;

        public DataVariableCad()
        {
            data = new Dictionary<string, object>();
        }

        public void Add(string key, object value)
        {
            data[key] = value;
        }

        public object this[string key]
        {
            get { return data[key]; }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return data.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}