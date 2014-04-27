using Newtonsoft.Json.Linq;

using umbraco;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace RJP.MultiUrlPicker.Models
{

    public class Link
    {
        private readonly JToken _linkItem;
        private string _name;
        private int? _id;
        private string _url;
        private string _target;
        private bool? _deleted;

        public Link(JToken linkItem)
        {
            _linkItem = linkItem;
        }

        private IPublishedContent PublishedItem
        {
            get
            {
                if (Id == null) return null;
                if (UmbracoContext.Current == null) return null;
                var umbHelper = new UmbracoHelper(UmbracoContext.Current);

                var id = (int)Id;
                var objType = uQuery.GetUmbracoObjectType(id);

                if  (objType == uQuery.UmbracoObjectType.Document)
                {
                    return umbHelper.TypedContent(id);
                }
                if (objType == uQuery.UmbracoObjectType.Media)
                {
                    return umbHelper.TypedMedia(id);
                }
                return null;
            }
        }

        public int? Id
        {
            get
            {
                if (_id == null)
                {
                    _id = _linkItem.Value<int?>("id");
                }
                return _id;         
            } 

        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    _name = _linkItem.Value<string>("name");
                }
                return _name;
            }
        }

        internal bool Deleted
        {
            get
            {
                if (_deleted == null)
                {
                    if (Id != null)
                    {
                        _deleted = PublishedItem == null;
                    }
                    else
                    {
                        _deleted = false;
                    }     
                }
                return (bool)_deleted;
            }
        }

        public string Url
        {
            get
            {
                if (string.IsNullOrEmpty(_url))
                {
                    _url = PublishedItem != null ? PublishedItem.Url : _linkItem.Value<string>("url");
                }
                return _url;
            }
        }

        public string Target
        {
            get
            {
                if (string.IsNullOrEmpty(_target))
                {
                    _target = _linkItem.Value<string>("target");
                }
                return _target == string.Empty ? null : _target;
            }
        }
    }
}
