namespace TFlex.DOCs.References.Configurations{
    using System;
    using TFlex.DOCs.Model.References;
    using TFlex.DOCs.Model.Structure;
    using TFlex.DOCs.Model.References.Links;
    using TFlex.DOCs.Model.Classes;
    using TFlex.DOCs.Model.Parameters;


    public partial class DataReferenceObject : ConfigurationsReferenceObject
    {
        public dynamic GetValue()
        {
            if (this.Class.IsArray)
            {
                return this;
            }
            else if (this.Class.IsBool)
            {
                var b = this as BoolReferenceObject;
                return b.ValueBool.Value;
            }
            else if (this.Class.IsNumber)
            {
                var n = this as NumberReferenceObject;
                return n.ValueDouble.Value;
            }
            else if (this.Class.IsString)
            {
                var n = this as StringReferenceObject;
                return n.ValueString.Value;
            }
            else { return null; }

        }
    }}