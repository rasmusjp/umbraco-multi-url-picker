namespace RJP.MultiUrlPicker
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;

    using Models;

    [PropertyValueType(typeof(IEnumerable<Link>))]
    [PropertyValueCache(PropertyCacheValue.Source, PropertyCacheLevel.Content)]
    [PropertyValueCache(PropertyCacheValue.Object, PropertyCacheLevel.ContentCache)]
    [PropertyValueCache(PropertyCacheValue.XPath, PropertyCacheLevel.ContentCache)]
    public class MultiUrlPickerValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("RJP.MultiUrlPicker");
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null)
            {
                return null;
            }
            return JArray.Parse(source.ToString());
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null)
            {
                return new MultiUrls();
            }

            return new MultiUrls((JArray)source);
        }
    }
}
