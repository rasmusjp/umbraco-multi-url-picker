namespace RJP.MultiUrlPicker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Newtonsoft.Json.Linq;
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Core.Services;

    public class MultiUrlPickerValueConverter : PropertyValueConverterBase, IPropertyValueConverterMeta
    {
        private readonly IDataTypeService _dataTypeService;

        public MultiUrlPickerValueConverter()
            : this(ApplicationContext.Current.Services.DataTypeService)
        { }

        public MultiUrlPickerValueConverter(IDataTypeService dataTypeService)
        {
            _dataTypeService = dataTypeService;
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("RJP.MultiUrlPicker");
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            var sourceStr = source?.ToString();
            if (string.IsNullOrWhiteSpace(sourceStr))
            {
                return null;
            }

            if (sourceStr.Trim().StartsWith("["))
            {
                try
                {
                    return JArray.Parse(sourceStr);
                }
                catch (Exception ex)
                {
                    LogHelper.Error<MultiUrlPickerValueConverter>("Error parsing JSON", ex);
                }
            }

            return null;
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            bool isMultiple = IsMultipleDataType(propertyType.DataTypeId, out int maxNumberOfItems);
            if (source == null)
            {
                return isMultiple ? new MultiUrls() : null;
            }

            var urls = new MultiUrls((JArray)source);
            if (isMultiple)
            {
                if (maxNumberOfItems > 0)
                {
                    return urls.Take(maxNumberOfItems);
                }

                return urls;
            }

            return urls.FirstOrDefault();
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            if (IsMultipleDataType(propertyType.DataTypeId, out int maxNumberOfItems))
            {
                return typeof(IEnumerable<Link>);
            }

            return typeof(Link);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            switch (cacheValue)
            {
                case PropertyCacheValue.Source:
                    return PropertyCacheLevel.Content;
                case PropertyCacheValue.Object:
                case PropertyCacheValue.XPath:
                    return PropertyCacheLevel.ContentCache;
            }

            return PropertyCacheLevel.None;
        }

        private bool IsMultipleDataType(int dataTypeId, out int maxNumberOfItems)
        {
            var preValues = _dataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId).FormatAsDictionary();

            if (preValues.TryGetValue("maxNumberOfItems", out PreValue maxNumberOfItemsPreValue) &&
                int.TryParse(maxNumberOfItemsPreValue.Value, out maxNumberOfItems))
            {
                // For backwards compatibility, always return true if version is less than 2.0.0
                if (preValues.TryGetValue("version", out PreValue versionPreValue) &&
                    Version.TryParse(versionPreValue.Value, out Version version) &&
                    version >= new Version(2, 0, 0))
                {
                    return maxNumberOfItems != 1;
                }
            }

            maxNumberOfItems = 0;
            return true;
        }
    }
}
