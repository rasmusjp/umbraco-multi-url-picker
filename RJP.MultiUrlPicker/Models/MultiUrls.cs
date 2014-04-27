using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Logging;

namespace RJP.MultiUrlPicker.Models
{
    public class MultiUrls : List<Link>
    {
        private readonly string _propertyData;

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
                        this.Add(new Link(item));
                    }
                    else
                    {
                        LogHelper.Warn<MultiUrls>(
                            string.Format("MultiUrlPicker value converter skipped a link as the node is deleted (Id: {0}), ", newLink.Id));
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

        public bool Any()
        {
            return Enumerable.Any(this);          
        }
    }
}
