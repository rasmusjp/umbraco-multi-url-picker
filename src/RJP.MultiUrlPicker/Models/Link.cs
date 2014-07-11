using Newtonsoft.Json.Linq;

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
        private LinkType? _linkType;

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

                if  (Type == LinkType.Content)
                {
                    return umbHelper.TypedContent(Id.Value);
                }
                if (Type == LinkType.Media)
                {
                    return umbHelper.TypedMedia(Id.Value);
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

      public LinkType Type
      {
          get
          {
              if (_linkType == null)
              {
                  if (Id.HasValue)
                  {
                      if (_linkItem.Value<bool>("isMedia"))
                      {
                          _linkType = LinkType.Media;
                      }
                      else
                      {
                          _linkType = LinkType.Content;
                      }
                  }
                  else
                  {
                      _linkType = LinkType.External;
                  }
              }
              return _linkType.Value;
          }
      }
    }
}
