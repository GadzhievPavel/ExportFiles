//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TFlex.DOCs.References.Configurations
{
	using System;
	using TFlex.DOCs.Model.References;
	using TFlex.DOCs.Model.Structure;
	using TFlex.DOCs.Model.References.Links;
	using TFlex.DOCs.Model.Classes;
	using TFlex.DOCs.Model.Parameters;
	
	
	/// <summary>
	/// Представляет объект "Параметр" справочника "Конфигурационный справочник" или порождённый от него
	/// </summary>
	public partial class ParameterReferenceObject
	{
		
		internal ParameterReferenceObject(ConfigurationsReference reference) : 
				base(reference)
		{
		}
		
		/// <summary>
		/// Возвращает параметр "Наименование"
		/// </summary>
		public StringParameter Name
		{
			get
			{
				return ((StringParameter)(this[FieldKeys.Name]));
			}
		}
		
		/// <summary>
		/// Возвращает параметр "Описание"
		/// </summary>
		public StringParameter Description
		{
			get
			{
				return ((StringParameter)(this[FieldKeys.Description]));
			}
		}
	}
}
