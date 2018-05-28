namespace RJP.MultiUrlPicker.PropertyEditors.ValueConverters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Web.PublishedCache;

    using Models;

    public class MultiUrlPickerValueConverter : PropertyValueConverterBase
    {
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
        private readonly ProfilingLogger _proflog;

        public MultiUrlPickerValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor, ProfilingLogger proflog)
        {
            _publishedSnapshotAccessor = publishedSnapshotAccessor ?? throw new ArgumentNullException(nameof(publishedSnapshotAccessor));
            _proflog = proflog ?? throw new ArgumentNullException(nameof(proflog));
        }
        public override bool IsConverter(PublishedPropertyType propertyType) => propertyType.EditorAlias.Equals("RJP.MultiUrlPicker");

        public override Type GetPropertyValueType(PublishedPropertyType propertyType) =>
            propertyType.DataType.ConfigurationAs<MultiUrlPickerConfiguration>().MaxNumberOfItems == 1 ?
                typeof(Link) :
                typeof(IEnumerable<Link>);

        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

        public override object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview) => source?.ToString();

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            using (_proflog.DebugDuration<MultiUrlPickerValueConverter>($"ConvertPropertyToLinks ({propertyType.DataType.Id})"))
            {
                int? maxNumber = propertyType.DataType.ConfigurationAs<MultiUrlPickerConfiguration>().MaxNumberOfItems;

                if (inter == null)
                {
                    return maxNumber == 1 ? null : Enumerable.Empty<Link>();
                }

                var links = new List<Link>();
                var dtos = JsonConvert.DeserializeObject<IEnumerable<LinkDto>>((string)inter);

                foreach (var dto in dtos)
                {
                    LinkType type = LinkType.External;
                    string url = dto.Url;

                    if (dto.Udi != null)
                    {
                        type = dto.Udi.EntityType == Constants.UdiEntityType.Media
                            ? LinkType.Media
                            : LinkType.Content;

                        var content = type == LinkType.Media ?
                             _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(preview, dto.Udi.Guid) :
                             _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(preview, dto.Udi.Guid);

                        if (content == null)
                        {
                            continue;
                        }
                        url = content.Url;
                    }

                    links.Add(
                        new Link
                        {
                            Name = dto.Name,
                            Target = dto.Target,
                            Type = type,
                            Udi = dto.Udi,
                            Url = url + dto.Querystring,
                        }
                    );
                }

                if (maxNumber == 1) return links.FirstOrDefault();
                if (maxNumber > 0) return links.Take(maxNumber.Value);
                return links;
            }
        }
    }
}
