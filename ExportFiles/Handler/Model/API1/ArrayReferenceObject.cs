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

    public partial class ArrayReferenceObject : DataReferenceObject
    {
        private HashSet<DataReferenceObject> parametersHashSet = new HashSet<DataReferenceObject>();        private Dictionary<string, DataReferenceObject> parametersDictionary = new Dictionary<string, DataReferenceObject>();

        /// <summary>
        /// Добавить в словарь параметр
        /// </summary>
        /// <param name="obj"></param>
        private void BuildSetParameters(ConfigurationsReferenceObject obj)        {            foreach (var child in obj.Children.Cast<DataReferenceObject>())            {                parametersHashSet.Add(child);                BuildSetParameters(child);            }        }        /// <summary>
        /// Собрать словарь параметров
        /// </summary>        private void BuildDictinoryParameters()        {            foreach (var child in parametersHashSet)            {                parametersDictionary.Add(child.Name, child);            }        }        /// <summary>
        /// Сформировать массив
        /// </summary>        private void LoadParameters()        {            BuildSetParameters(this);            BuildDictinoryParameters();        }

        /// <summary>
        /// Вернуть все параметры конфигурации
        /// </summary>
        /// <returns></returns>
        public Config getParameters()
        {            LoadParameters();            return new Config(parametersDictionary);
        }
    }}