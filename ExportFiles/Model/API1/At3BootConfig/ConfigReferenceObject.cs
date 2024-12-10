namespace TFlex.DOCs.References.Configurations{
    using System;
    using TFlex.DOCs.Model.References;
    using TFlex.DOCs.Model.Structure;
    using TFlex.DOCs.Model.References.Links;
    using TFlex.DOCs.Model.Classes;
    using TFlex.DOCs.Model.Parameters;
    using System.Collections.Generic;
    using System.Linq;
    using ExportFiles.Handler.Model.API1;

    public partial class ConfigReferenceObject : ConfigurationsReferenceObject
    {
        private HashSet<DataReferenceObject> parametersHashSet = new HashSet<DataReferenceObject>();        private Dictionary<string, DataReferenceObject> parametersDictionary = new Dictionary<string, DataReferenceObject>();        public DataReferenceObject GetParam(string parameterName)        {            var param = new DataReferenceObject(this.Reference);            if (parametersDictionary.TryGetValue(parameterName, out param))            {                return param;            }            else { return null; }        }        public DataReferenceObject this[string parameterName]        {            get { return GetParam(parameterName); }        }        private void BuildSetParameters(ConfigurationsReferenceObject obj)        {            foreach (var child in obj.Children.Cast<DataReferenceObject>())            {                parametersHashSet.Add(child);                BuildSetParameters(child);            }        }        private void BuildDictinoryParameters()        {            foreach (var child in parametersHashSet)            {                parametersDictionary.Add(child.Name, child);            }        }        private void LoadParameters()        {            BuildSetParameters(this);            BuildDictinoryParameters();        }
        /// <summary>
        /// Вернуть все параметры конфигурации
        /// </summary>
        /// <returns></returns>
        public Config getParameters()
        {            LoadParameters();            return new Config(parametersDictionary);
        }    }
}