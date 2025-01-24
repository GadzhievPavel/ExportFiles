using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.References;
using TFlex.DOCs.Model.References.Links.Extensions;
using TFlex.DOCs.References.Configurations;

namespace ExportFiles.Handler.Model.API1
{
    public partial class RefReferenceObject : ParameterReferenceObject
    {
        internal RefReferenceObject(ConfigurationsReference reference) : base(reference) { }

        /// <summary>
        /// Возвращает объект на который ведет связь
        /// </summary>
        public ReferenceObject RefObject
        {
            get
            {
                var objects = this.GetObjects(RelationsLink.RefObject);
                if (objects != null)
                {
                    return objects.FirstOrDefault();
                }
                return null;
            }
        }
    }
}
