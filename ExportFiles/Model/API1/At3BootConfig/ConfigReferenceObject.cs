namespace TFlex.DOCs.References.Configurations
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
        private HashSet<DataReferenceObject> parametersHashSet = new HashSet<DataReferenceObject>();
        /// <summary>
        /// ������� ��� ��������� ������������
        /// </summary>
        /// <returns></returns>
        public Config getParameters()
        {
        }
