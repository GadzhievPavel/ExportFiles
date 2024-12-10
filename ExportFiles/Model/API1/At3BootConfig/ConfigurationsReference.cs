namespace TFlex.DOCs.References.Configurations{
    using System;
    using TFlex.DOCs.Model.References;
    using TFlex.DOCs.Model.Structure;
    using TFlex.DOCs.Model.Classes;
    using TFlex.DOCs.Model;
    using TFlex.DOCs.Model.Search;
    using System.Linq;

    public partial class ConfigurationsReference : SpecialReference<ConfigurationsReferenceObject>
    {
        public partial class Factory
        {
        }

        /// <summary>
        /// Поиск конфигурации
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigReferenceObject FindConfig(string name)        {            return Find(                Filter.Parse($"[Наименование] = '{name}' И [Тип] = 'Конфигурация'",                this.ParameterGroup)).FirstOrDefault() as ConfigReferenceObject;        }
    }}