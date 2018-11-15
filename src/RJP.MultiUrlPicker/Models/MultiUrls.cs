namespace RJP.MultiUrlPicker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Umbraco.Core.Logging;

    [Obsolete("Use IEnumerable<Link> instead")]
    public class MultiUrls : IEnumerable<Link>
    {
        private readonly string _propertyData;
        private readonly List<Link> _multiUrls = new List<Link>();

        internal MultiUrls()
        {
        }

        internal MultiUrls(JArray propertyData)
        {
            _propertyData = propertyData.ToString();

            Initialize(propertyData);
        }

        public MultiUrls(string propertyData)
        {
            _propertyData = propertyData;

            if (!string.IsNullOrEmpty(propertyData))
            {
                var relatedLinks = JsonConvert.DeserializeObject<JArray>(propertyData);
                Initialize(relatedLinks);
            }
        }

        private void Initialize(JArray data)
        {
            foreach (var item in data)
            {
                var newLink = new Link(item);
                if (!newLink.Deleted)
                {
                    _multiUrls.Add(new Link(item));
                }
                else
                {
                    LogHelper.Info<MultiUrls>("Skipped link as the node has been unpublished/deleted (UDI: {0})", () => newLink.Udi);
                }
            }
        }

        public string PropertyData
        {
            get
            {
                return _propertyData;
            }
        }

        // Although this method seems unnecessary it makes .Any() available in Dynamics
        public bool Any()
        {
            return Enumerable.Any(this);
        }

        public IEnumerator<Link> GetEnumerator()
        {
            return _multiUrls.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
