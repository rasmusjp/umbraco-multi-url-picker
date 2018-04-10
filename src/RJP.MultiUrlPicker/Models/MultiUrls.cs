namespace RJP.MultiUrlPicker.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Umbraco.Core.Logging;

    [Obsolete("Use IEnumerable<Link> instead.")]
    public class MultiUrls : IEnumerable<Link>
    {
        private readonly List<Link> _multiUrls = new List<Link>();

        public MultiUrls(string propertyData)
        {
            PropertyData = propertyData;

            if (!string.IsNullOrEmpty(propertyData))
            {
                var relatedLinks = JsonConvert.DeserializeObject<JArray>(propertyData);
                Initialize(relatedLinks);
            }
        }

        internal MultiUrls()
        { }

        internal MultiUrls(JArray propertyData)
        {
            PropertyData = propertyData.ToString();

            Initialize(propertyData);
        }

        public string PropertyData { get; }

        // Although this method seems unnecessary it makes .Any() available in Dynamics
        public bool Any()
        {
            return Enumerable.Any(this);
        }

        public IEnumerator<Link> GetEnumerator()
        {
            return _multiUrls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
                    LogHelper.Warn<MultiUrls>("MultiUrlPicker value converter skipped a link as the node has been upublished/deleted (Id: {0}).", () => newLink.Id);
                }
            }
        }
    }
}
