using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Umbraco.Core.Logging;

namespace RJP.MultiUrlPicker.Models
{
    public class MultiUrls : IEnumerable<Link>
    {
        private readonly string _propertyData;
        private readonly List<Link> _multiUrls = new List<Link>(); 

        public MultiUrls(string propertyData)
        {
            _propertyData = propertyData;

            if (!string.IsNullOrEmpty(propertyData))
            {
                var relatedLinks = JsonConvert.DeserializeObject<JArray>(propertyData);
                foreach (var item in relatedLinks)
                {
                    var newLink = new Link(item);
                    if (!newLink.Deleted)
                    {
                        _multiUrls.Add(new Link(item));
                    }
                    else
                    {
                        LogHelper.Warn<MultiUrls>(
                            string.Format("MultiUrlPicker value converter skipped a link as the node has been upublished/deleted (Id: {0}), ", newLink.Id));
                    }
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
