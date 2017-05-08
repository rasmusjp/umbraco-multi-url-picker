namespace RJP.MultiUrlPicker
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Core.Services;

    using Models;

    public class MultiUrlPickerValueConverter : PropertyValueConverterBase, IPropertyValueConverterMeta
    {
        private static int _maxNumberOfItems;

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("RJP.MultiUrlPicker");
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (string.IsNullOrEmpty(source?.ToString()))
            {
                return null;
            }
            return JArray.Parse(source.ToString());
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            bool isMultiple = IsMultipleDataType(propertyType.DataTypeId);
            if (source == null)
            {
                return isMultiple ? new MultiUrls() : null;
            }

            var urls = new MultiUrls((JArray)source);
            if(isMultiple)
            {
                if(_maxNumberOfItems > 0)
                {
                    return urls.Take(_maxNumberOfItems);
                }
                return urls;
            }
            return urls.FirstOrDefault();
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            if (IsMultipleDataType(propertyType.DataTypeId))
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

        private static bool IsMultipleDataType(int dataTypeId)
        {
            IDataTypeService dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            IDictionary<string, PreValue> preValues = dataTypeService
                .GetPreValuesCollectionByDataTypeId(dataTypeId)
                .PreValuesAsDictionary;

            PreValue maxNumberOfItems;
            if (preValues.TryGetValue("maxNumberOfItems", out maxNumberOfItems) &&
                int.TryParse(maxNumberOfItems.Value, out _maxNumberOfItems))
            {
                PreValue versionPreValue;
                Version version;
                // for backwards compatibility, always return true if version
                // is less than 2.0.0
                if (preValues.TryGetValue("version", out versionPreValue) &&
                    Version.TryParse(versionPreValue.Value, out version)
                    && version >= new Version(2, 0, 0))
                {
                    return _maxNumberOfItems != 1;
                }
            }
            return true;
        }
    }
}
