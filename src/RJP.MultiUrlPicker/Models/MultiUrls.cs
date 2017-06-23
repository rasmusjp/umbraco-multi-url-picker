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

        internal MultiUrls(JArray propertyData, string requestingUrl)
        {
            _propertyData = propertyData.ToString();

            Initialize(propertyData, requestingUrl);
        }

        public MultiUrls(string propertyData, string requestingUrl)
        {
            _propertyData = propertyData;

            if (!string.IsNullOrEmpty(propertyData))
            {
                var relatedLinks = JsonConvert.DeserializeObject<JArray>(propertyData);
                Initialize(relatedLinks, requestingUrl);
            }
        }

        private void Initialize(JArray data, string requestingUrl)
        {
            foreach (var item in data)
            {
                var newLink = new Link(item);
                if (!newLink.Deleted)
                {
                    _multiUrls.Add(newLink);
                }
                else
                {
                    LogHelper.Warn<MultiUrls>(
                        string.Format("MultiUrlPicker value converter skipped a link as the node has been unpublished/deleted (Udi: {0}, Request: {1})", newLink.Udi, requestingUrl));
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
