using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

using RJP.MultiUrlPicker.Models;

namespace RJP.MultiUrlPicker
{
    [PropertyValueType(typeof(MultiUrls))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    public class MultiUrlPickerValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("RJP.MultiUrlPicker");
        }
        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null) return null;
            var sourceString = source.ToString();

            return UmbracoContext.Current != null ? new MultiUrls(sourceString) : null;
        }
    }
}
