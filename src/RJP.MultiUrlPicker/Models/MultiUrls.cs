namespace RJP.MultiUrlPicker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Umbraco.Core.Logging;
    using Umbraco.Web;

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
                    LogHelper.Warn<MultiUrls>("MultiUrlPicker value converter skipped a link, because the node has been upublished/deleted (ID: {0}, URL: {1})", () => newLink.Id, () => UmbracoContext.Current?.PublishedContentRequest?.Uri);
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
