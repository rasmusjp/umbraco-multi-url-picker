namespace RJP.MultiUrlPicker.Courier
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Umbraco.Courier.Core;
    using Umbraco.Courier.Core.Logging;
    using Umbraco.Courier.DataResolvers;
    using Umbraco.Courier.ItemProviders;

    public class MultiUrlPickerDataResolverProvider : PropertyDataResolverProvider
    {
        public override string EditorAlias
        {
            get
            {
                return "RJP.MultiUrlPicker";
            }
        }

        public override void PackagingProperty(Item item, ContentProperty propertyData)
        {
            if (propertyData.Value != null)
            {
                var links = JsonConvert.DeserializeObject<JArray>(propertyData.Value.ToString());
                if (links != null)
                {
                    foreach (dynamic link in links)
                    {
                        if (link.id != null)
                        {
                            var objectTypeId = link.isMedia != null
                                ? UmbracoNodeObjectTypeIds.Media
                                : UmbracoNodeObjectTypeIds.Document;
                            var itemProviderId = link.isMedia != null
                                ? ItemProviderIds.mediaItemProviderGuid
                                : ItemProviderIds.documentItemProviderGuid;

                            link.id = ExecutionContext.DatabasePersistence.GetUniqueId((int)link.id, objectTypeId);
                            item.Dependencies.Add(link.id.ToString(), itemProviderId);
                        }
                        else if (link.isMedia != null)
                        {
                            try
                            {
                                var mediaId = ExecutionContext.DatabasePersistence.GetUniqueIdFromMediaFile(link.url);
                                item.Dependencies.Add(mediaId.ToString(), ItemProviderIds.mediaItemProviderGuid);
                            }
                            catch (Exception e)
                            {
                                CourierLogHelper.Error<MultiUrlPickerDataResolverProvider>(string.Format("Error setting media-item dependency, name={0}, url={1}", link.name, link.url), e);
                            }
                        }
                    }
                    propertyData.Value = links.ToString();
                }
            }
        }

        public override void ExtractingProperty(Item item, ContentProperty propertyData)
        {
            if (propertyData.Value != null)
            {
                var links = JsonConvert.DeserializeObject<JArray>(propertyData.Value.ToString());
                if (links != null)
                {
                    foreach (dynamic link in links)
                    {
                        if (link.id != null)
                        {
                            link.id = ExecutionContext.DatabasePersistence.GetNodeId(
                                (Guid)link.id,
                                link.isMedia != null ? UmbracoNodeObjectTypeIds.Media : UmbracoNodeObjectTypeIds.Document);
                        }
                    }
                    propertyData.Value = links;
                }
            }
        }
    }
}