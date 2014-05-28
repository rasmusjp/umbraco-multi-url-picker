namespace RJP.MultiUrlPicker.Courier
{
    #region Usings

    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Umbraco.Courier.Core;
    using Umbraco.Courier.DataResolvers;
    using Umbraco.Courier.ItemProviders;

    #endregion

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
                            link.id = PersistenceManager.Default.GetUniqueId(
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
                            link.id = PersistenceManager.Default.GetNodeId(
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