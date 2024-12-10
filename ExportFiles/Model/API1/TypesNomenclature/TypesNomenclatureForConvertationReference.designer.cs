//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TFlex.DOCs.References.TypesNomenclatureForConvertation
{
	using System;
	using TFlex.DOCs.Model.References;
	using TFlex.DOCs.Model.Structure;
	using TFlex.DOCs.Model.Classes;
	using TFlex.DOCs.Model;
	
	
	/// <summary>
	/// Представляет описание справочника "Типы конвертируемых объектов ЭСИ"
	/// </summary>
	public partial class TypesNomenclatureForConvertationReference
	{
		
		/// <summary>
		/// Представляет уникальный идентификатор (GUID) справочника "Типы конвертируемых объектов ЭСИ"
		/// </summary>
		public static readonly System.Guid ReferenceId = new Guid("fcf00da8-06aa-4c57-86a9-522ef0a752b0");
		
		/// <summary>
		/// Инициализирует новый экземпляр TypesNomenclatureForConvertationReference для работы с объектами справочника "Типы конвертируемых объектов ЭСИ"
		/// </summary>
		public TypesNomenclatureForConvertationReference(TFlex.DOCs.Model.ServerConnection connection) : 
				base(connection, TypesNomenclatureForConvertationReference.ReferenceId)
		{
		}
		
		private TypesNomenclatureForConvertationReference(ParameterGroup masterGroup) : 
				base(masterGroup)
		{
		}
		
		/// <summary>
		/// Возвращает типы объектов справочника "Типы конвертируемых объектов ЭСИ"
		/// </summary>
		public new TypesNomenclatureForConvertationTypes Classes
		{
			get
			{
				return ((TypesNomenclatureForConvertationTypes)(base.Classes));
			}
		}
		
		protected override TypesNomenclatureForConvertationReferenceObject CreateReferenceObjectForClass(ClassObject classObject)
		{
			TypesNomenclatureForConvertationType type = classObject as TypesNomenclatureForConvertationType;
			if ((type == null))
			{
				return null;
			}
			if (type.IsTypesNomenclatureForConvertationReferenceObject)
			{
				return new TypesNomenclatureForConvertationReferenceObject(this);
			}
			return new TypesNomenclatureForConvertationReferenceObject(this);
		}
		
		public partial class Factory : SpecialReferenceFactory<TypesNomenclatureForConvertationReference, TypesNomenclatureForConvertationTypes>
		{
			
			internal Factory()
			{
			}
			
			public override TypesNomenclatureForConvertationReference CreateReference(ParameterGroup masterGroup)
			{
				return new TypesNomenclatureForConvertationReference(masterGroup);
			}
			
			public override TypesNomenclatureForConvertationTypes CreateClassTree(ParameterGroup masterGroup)
			{
				return new TypesNomenclatureForConvertationTypes(masterGroup);
			}
		}
	}
}
