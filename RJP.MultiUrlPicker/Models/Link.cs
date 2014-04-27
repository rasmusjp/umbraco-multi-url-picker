using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private IPublishedContent ContentItem
        {
            get
            {
                if (this.Id == null) return null;
                if (UmbracoContext.Current == null) return null;
                var umbHelper = new UmbracoHelper(UmbracoContext.Current);
                return umbHelper.TypedContent((int)this.Id);
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
                    if (this.Id != null)
                    {
                        _deleted = ContentItem == null;
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
                    if (this.ContentItem != null)
                    {
                        _url = ContentItem.Url;
                    }
                    else
                    {
                        _url = _linkItem.Value<string>("url");
                    }
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
