namespace TFlex.DOCs.References.Configurations
{
    using System;
    using TFlex.DOCs.Model.References;
    using TFlex.DOCs.Model.Structure;
    using TFlex.DOCs.Model.References.Links;
    using TFlex.DOCs.Model.Classes;
    using TFlex.DOCs.Model.Parameters;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ConfigReferenceObject : ConfigurationsReferenceObject
    {
        private HashSet<ParameterReferenceObject> parametersHashSet = new HashSet<ParameterReferenceObject>();

        private Dictionary<string, ParameterReferenceObject> parametersDictionary = new Dictionary<string, ParameterReferenceObject>();

        public ParameterReferenceObject GetParam(string parameterName)
        {

            var param = new ParameterReferenceObject(this.Reference);
            if (parametersDictionary.TryGetValue(parameterName, out param))
            {
                return param;
            }
            else { return null; }
        }

        public ParameterReferenceObject this[string parameterName]
        {
            get { return GetParam(parameterName); }
        }

        private void BuildSetParameters(ConfigurationsReferenceObject obj)
        {
            foreach (var child in obj.Children.Cast<ParameterReferenceObject>())
            {
                parametersHashSet.Add(child);
                BuildSetParameters(child);
            }
        }

        private void BuildDictinoryParameters()
        {
            foreach (var child in parametersHashSet)
            {
                parametersDictionary.Add(child.Name, child);
            }
        }

        private void LoadParameters()
        {
            BuildSetParameters(this);
            BuildDictinoryParameters();
        }

        public Dictionary<String, ParameterReferenceObject> getParameters() {
            LoadParameters();
            return parametersDictionary; }
    }
}
