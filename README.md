Работа с документацией [ссылка](https://github.com/GadzhievPavel/ExportFiles/blob/master/ExportFiles/docs/index.md) 

# Назначение ExportFiles

Библиотека позволяет генерировать подлинники формата tif из grb файлов САПР T-FLEX CAD 17

Генерация подлинников производится на основе файлов, прикрепленных к номенклатуре типа **EngineeringDocumentObject**

## Пример 1
### Генерация подлинников для коллекции номенклатуры

* Указываем типы номенклатуры, для которых нужно проводить экспорта tif файлов
* Добавляем номенклатуру для экспорта
* Запускаем экспорт. Передаем **true** в метод **Export** если нужно создать новые файлы tit
<pre>
	<code>
	NomenclatureExport nomenclatureExport = new NomenclatureExport(Context.Connection);
        nomenclatureExport.AddEnabledClassObjectNomenclature("08309a17-4bee-47a5-b3c7-57a1850d55ea");///Деталь
        nomenclatureExport.AddEnabledClassObjectNomenclature("1cee5551-3a68-45de-9f33-2b4afdbf4a5c");///Сборочная единица
        nomenclatureExport.AddEnabledClassObjectNomenclature("7fa98498-c39c-44fc-bcaa-699b387f7f46");///изделие
        nomenclatureExport.AddEnabledClassObjectNomenclature("a8d65b57-b09d-42ef-9b7b-e9f95991053a");///спецификация

        foreach (ReferenceObject item in Объекты)
        {
            nomenclatureExport.AddNomenclature(item as NomenclatureObject);
        }
        nomenclatureExport.Export(true);
	</code>
</pre>

### Обновление подлинников для коллекции номенклатуры

* Указываем типы номенклатуры, для которых нужно проводить экспорта tif файлов
* Добавляем номенклатуру для экспорта
* Запускаем экспорт. Передаем **false** в метод **Export** если нужно обновить файлы tit
<pre>
	<code>
	NomenclatureExport nomenclatureExport = new NomenclatureExport(Context.Connection);
        nomenclatureExport.AddEnabledClassObjectNomenclature("08309a17-4bee-47a5-b3c7-57a1850d55ea");///Деталь
        nomenclatureExport.AddEnabledClassObjectNomenclature("1cee5551-3a68-45de-9f33-2b4afdbf4a5c");///Сборочная единица
        nomenclatureExport.AddEnabledClassObjectNomenclature("7fa98498-c39c-44fc-bcaa-699b387f7f46");///изделие
        nomenclatureExport.AddEnabledClassObjectNomenclature("a8d65b57-b09d-42ef-9b7b-e9f95991053a");///спецификация

        foreach (ReferenceObject item in Объекты)
        {
            nomenclatureExport.AddNomenclature(item as NomenclatureObject);
        }
        nomenclatureExport.Export(false);
	</code>
</pre>