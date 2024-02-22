using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.Plugins;
using TFlex.DOCs.Model.References;
using TFlex.PdmFramework.Resolve;

namespace ExportFiles
{
    public class PluginLibrary : IPluginLibrary
    {
        public void OnCreatingReferenceVisualRepresentation(Reference reference, IModelVisualRepresentation VisualRepresentation)
        {
        }

        public void OnCreatingVisualRepresentation(IModelVisualRepresentation VisualRepresentation)
        {
        }

        public void RegisterPlugin()
        {
            AssemblyResolver.Instance.AddDirectory(@"C:\Program Files (x86)\T-FLEX DOCs 17\Program");
        }
    }
}
