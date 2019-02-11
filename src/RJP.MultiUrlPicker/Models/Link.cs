namespace RJP.MultiUrlPicker.Models
{
    using System;

    using Newtonsoft.Json.Linq;

    using Umbraco.Core.Models;
    using Umbraco.Core;

    using Umbraco.Web;
    using Umbraco.Web.Extensions;
    using Umbraco.Core.Models.PublishedContent;

    public class Link
    {
        private readonly JToken _linkItem;
        private readonly Lazy<UmbracoHelper> _umbracoHelper;
        private bool _publishedContentInitialized = false;
        private string _name;
        private string _url;
        private string _target;
        private bool? _deleted;
        private LinkType? _linkType;
        private IPublishedContent _content;
        private Udi _udi;
        private int? _id;

        public Link(JToken linkItem, Lazy<UmbracoHelper> umbracoHelper = null)
        {
            _linkItem = linkItem;
            _umbracoHelper = umbracoHelper ?? new Lazy<UmbracoHelper>(() => new UmbracoHelper(UmbracoContext.Current));
        }

        private IPublishedContent PublishedContent
        {
            get
            {
                InitPublishedContent();
                return _content;
            }
        }

        [Obsolete("Use Udi instead")]
        public int? Id
        {
            get
            {
                if (_id == null)
                {
                    _id = _linkItem.Value<int?>("id");
                    if (!_id.HasValue)
                    {
                        InitPublishedContent();
                    }
                }
                return _id;
            }

        }

        public Udi Udi
        {
            get
            {
                if (_udi == null)
                {
                    if (!Udi.TryParse(_linkItem.Value<string>("udi"), out _udi))
                    {
                        InitPublishedContent();
                    }
                }
                return _udi;
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
                    if (Id.HasValue || Udi != null)
                    {
                        _deleted = PublishedContent == null;
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
                    _url = PublishedContent?.Url ?? _linkItem.Value<string>("url");

                    var qs = _linkItem.Value<string>("querystring");
                    if (!string.IsNullOrWhiteSpace(qs))
                    {
                        _url += qs;
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

        public LinkType Type
        {
            get
            {
                if (_linkType == null)
                {
                    if (Udi != null)
                    {
                        if (Udi.EntityType == Constants.UdiEntityType.Media)
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

        private void InitPublishedContent()
        {
            if (!_publishedContentInitialized)
            {
                _publishedContentInitialized = true;

                if (UmbracoContext.Current == null)
                {
                    return;
                }

                if (Udi.TryParse(_linkItem.Value<string>("udi"), out _udi) && _udi is GuidUdi guidUdi)
                {
                    var umbracoType = Constants.UdiEntityType.ToUmbracoObjectType(_udi.EntityType);
                    if (umbracoType == UmbracoObjectTypes.Media)
                    {
                        var entityService = ApplicationContext.Current.Services.EntityService;
                        var mediaAttempt = entityService.GetIdForKey(guidUdi.Guid, umbracoType);
                        if (mediaAttempt.Success)
                        {
                            _content = _umbracoHelper.Value.TypedMedia(mediaAttempt.Result);
                        }
                    }
                    else
                    {
                        _content = _umbracoHelper.Value.TypedContent(guidUdi.Guid);
                    }
                    _id = _content?.Id;
                }
                else
                {
                    // there were no Udi so let's try the legacy way
                    _id = _linkItem.Value<int?>("id");
                    if (_id.HasValue)
                    {
                        if (_linkItem.Value<bool>("isMedia"))
                        {
                            _content = _umbracoHelper.Value.TypedMedia(_id.Value);
                        }
                        else
                        {
                            _content = _umbracoHelper.Value.TypedContent(_id.Value);
                        }

                        SetUdi();
                    }
                }
            }
        }

        private void SetUdi()
        {
            if (_content != null && _udi == null)
            {
                Guid? key = _content.GetKey();
                if (key == Guid.Empty)
                {
                    // if the key is Guid.Empty the model might be created by the ModelsBuilder,
                    // if so it, by default, derives from PublishedContentModel.
                    // By calling UnWrap() we get the original content, which probably implements
                    // IPublishedContentWithKey, so we can get the key
                    key = (_content as PublishedContentWrapped)?.Unwrap().GetKey();
                }

                if (key.HasValue && key != Guid.Empty)
                {
                    string udiType = _content.ItemType == PublishedItemType.Media ?
                        Constants.UdiEntityType.Media :
                        Constants.UdiEntityType.Document;

                    _udi = Udi.Create(udiType, key.Value);
                }
            }
        }
    }
}
