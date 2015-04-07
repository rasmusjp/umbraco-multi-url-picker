namespace RJP.MultiUrlPicker.Courier
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Umbraco.Courier.Core;
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
                            link.id = ExecutionContext.DatabasePersistence.GetUniqueId(
                                (int)link.id,
                                link.isMedia != null ? NodeObjectTypes.Media : NodeObjectTypes.Document);
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
                                link.isMedia != null ? NodeObjectTypes.Media : NodeObjectTypes.Document);
                        }
                    }
                    propertyData.Value = links.ToString();
                }
            }
        }
    }
}