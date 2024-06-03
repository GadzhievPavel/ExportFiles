using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Класс с набором Guid
/// </summary>
public static class Guids
{
    /// <summary>
    /// Guid-ы справочника "Документы"
    /// </summary>
    public static class DocumentsReference
    {
        /// <summary>
        /// Типы справочника "Документы"
        /// </summary>
        public static class Classes
        {
            /// <summary>
            /// Тип "Объект состава изделия"
            /// </summary>
            public static readonly Guid objectProductComposition = new Guid("f89e9648-c8a0-43f8-82bb-015cfe1486a4");
        }

        public static class Links
        {
            public static readonly Guid Files = new Guid("9eda1479-c00d-4d28-bd8e-0d592c873303");

            public static readonly Guid ОсновнойМатериал = new Guid("2167290d-faa1-4c55-a5cb-32bcd205502a");
        }
    }

    /// <summary>
    /// Guid-ы справочника "Изменения"
    /// </summary>
    public static class ChangeReference
    {
        /// <summary> Guid справочника 'Изменения' </summary>
        public static readonly Guid Изменения = new Guid("c9a4bb1b-cacb-4f2d-b61a-265f1bfc7fb9");

        public static class Parameter
        {
            public static readonly Guid НомерИзменения = new Guid("91486563-d044-4045-814b-3432b67812f1");
        }
        public static class Links
        {
            /// <summary> Guid связи 'Рабочие Файлы' справочника 'Файлы' </summary>
            public static readonly Guid ИзмененияРабочиеФайлы = new Guid("6b65a575-3ca4-4fb0-9bfc-4d1655c2d83e");
            /// <summary>
            /// Guid связи "ОбъектЭСИ" от Изменения к справочнику ЭСИ
            /// </summary>
            public static readonly Guid ОбъектЭСИ = new Guid("7898c148-6434-494a-bb27-f19f31a5baa2");
        }
    }

    /// <summary>
    /// Guid-ы справочника "Извещения об Изменениях"
    /// </summary>
    public static class NoticeModificationReference
    {
        /// <summary>
        /// Guid справочника 'Извещения об Изменениях'
        /// </summary>
        public static readonly Guid ИзвещенияОбИзменениях = new Guid("4853c5ce-6b8a-48ac-94bf-e80cdbcb4c1b");

        public static class Parameter
        {
            public static readonly Guid ОбозначениеИзвещенияОбИзменения = new Guid("b03c9129-7ac3-46f5-bf7d-fdd88ef1ff9a");
        }

        public static class Links
        {
            /// <summary>
            /// Guid связи "Изменения" от ИИ к объекту изменение
            /// </summary>
            public static readonly Guid ИзвещенияИзменения = new Guid("5e46670a-400c-4e36-bb37-d4d651bdf692");
        }
    }

    public static class MaterialReference
    {
        public static class Parameter
        {
            public static readonly Guid СводноеНаименование = new Guid("23cfeee6-57f3-4a1e-9cf0-9040fed0e90c");
        }
    }

    /// <summary>
    /// Guid ы справочника номенклатура
    /// </summary>
    public static class NomenclatureReference
    {
        /// <summary>
        /// Guid типа "Сборочный чертеж"
        /// </summary>
        public static readonly Guid TypeAssemblyDrawing = new Guid("00542618-0248-435b-b308-928cb7131655");
    }
}