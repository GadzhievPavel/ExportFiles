//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TFlex.DOCs.References.TypesConvertableObjectsNomenclature
{
	using System;
	using TFlex.DOCs.Model.References;
	using TFlex.DOCs.Model.Classes;
	
	
	/// <summary>
	/// Представляет описание типа объекта справочника "Типы конвертируемых объектов ЭСИ"
	/// </summary>
	public partial class TypesConvertableObjectsNomenclatureType
	{
		
		internal TypesConvertableObjectsNomenclatureType(TypesConvertableObjectsNomenclatureTypes owner) : 
				base(owner)
		{
		}
		
		/// <summary>
		/// Возвращает типы объектов справочника "Типы конвертируемых объектов ЭСИ"
		/// </summary>
		public new TypesConvertableObjectsNomenclatureTypes Classes
		{
			get
			{
				return ((TypesConvertableObjectsNomenclatureTypes)(base.Classes));
			}
		}
		
		/// <summary>
		/// Возвращает true, если текущий экземпляр описывает тип "Тип конвертируемого объекта ЭСИ" или порождён от него
		/// </summary>
		public bool IsTypesConvertableObjectsNomenclatureReferenceObject
		{
			get
			{
				return IsInherit(TypesConvertableObjectsNomenclatureTypes.Keys.TypesConvertableObjectsNomenclatureReferenceObject);
			}
		}
	}
}