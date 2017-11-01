namespace RJP.MultiUrlPicker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ClientDependency.Core;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.Editors;
    using Umbraco.Core.Models.EntityBase;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.PropertyEditors;

    using Models;

    using Constants = Umbraco.Core.Constants;

    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/RJP.MultiUrlPicker/MultiUrlPicker.js")]
    [PropertyEditor("RJP.MultiUrlPicker", "Multi Url Picker", "JSON",
        "~/App_Plugins/RJP.MultiUrlPicker/MultiUrlPicker.html",
        Group ="pickers", Icon = "icon-link", IsParameterEditor = true)]
    public class MultiUrlPickerPropertyEditor : PropertyEditor
    {
        private IDictionary<string, object> _defaultPreValues;

        public MultiUrlPickerPropertyEditor()
        {
            _defaultPreValues = new Dictionary<string, object>
            {
                {"version", Information.Version.ToString(3)},
            };
        }

        public override IDictionary<string, object> DefaultPreValues
        {
            get { return _defaultPreValues; }
            set { _defaultPreValues = value; }
        }

        protected override PreValueEditor CreatePreValueEditor()
        {
            return new MultiUrlPickerPreValueEditor();
        }

        protected override PropertyValueEditor CreateValueEditor()
        {
            return new MultiUrlPickerPropertyValueEditor(base.CreateValueEditor());
        }

        private class MultiUrlPickerPreValueEditor : PreValueEditor
        {
            [PreValueField("minNumberOfItems", "Min number of items", "number")]
            public int? MinNumberOfItems { get; set; }

            [PreValueField("maxNumberOfItems", "Max number of items", "number")]
            public int? MaxNumberOfItems { get; set; }

            [PreValueField("version", "Multi Url Picker version", "hidden", HideLabel = true)]
            public string Version { get; set; }

            public override IDictionary<string, object> ConvertDbToEditor(IDictionary<string, object> defaultPreVals, PreValueCollection persistedPreVals)
            {
                // if there isn't a version stored set it to 0 for backwards compatibility
                if(!persistedPreVals.PreValuesAsDictionary.ContainsKey("version"))
                {
                    persistedPreVals.PreValuesAsDictionary["version"] = new PreValue("0");
                }

                return base.ConvertDbToEditor(defaultPreVals, persistedPreVals);
            }
        }

        private class MultiUrlPickerPropertyValueEditor : PropertyValueEditorWrapper
        {
            public MultiUrlPickerPropertyValueEditor(PropertyValueEditor wrapped) : base(wrapped)
            {
            }

            public override object ConvertDbToEditor(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
            {
                string value = property.Value?.ToString();

                if(string.IsNullOrEmpty(value))
                {
                    return Enumerable.Empty<object>();
                }

                try
                {
                    ServiceContext services = ApplicationContext.Current.Services;
                    IEntityService entityService = services.EntityService;
                    IContentTypeService contentTypeService = services.ContentTypeService;

                    var links = JsonConvert.DeserializeObject<List<LinkDto>>(value);

                    var documentLinks = links.FindAll(link =>
                        link.Id.HasValue && false == link.IsMedia.GetValueOrDefault() ||
                        link.Udi != null && link.Udi.EntityType == Constants.UdiEntityType.Document
                    );

                    var mediaLinks = links.FindAll(link =>
                        link.Id.HasValue && true == link.IsMedia.GetValueOrDefault() ||
                        link.Udi != null && link.Udi.EntityType == Constants.UdiEntityType.Media
                    );

                    List<IUmbracoEntity> entities = new List<IUmbracoEntity>();
                    if (documentLinks.Count > 0)
                    {
                        if(documentLinks[0].Id.HasValue)
                        {
                            entities.AddRange(
                                entityService.GetAll(UmbracoObjectTypes.Document, documentLinks.Select(link => link.Id.Value).ToArray())
                            );
                        }
                        else
                        {
                            entities.AddRange(
                                entityService.GetAll(UmbracoObjectTypes.Document, documentLinks.Select(link => link.Udi.Guid).ToArray())
                            );
                        }
                    }

                    if(mediaLinks.Count > 0)
                    {
                        if (mediaLinks[0].Id.HasValue)
                        {
                            entities.AddRange(
                                entityService.GetAll(UmbracoObjectTypes.Media, mediaLinks.Select(link => link.Id.Value).ToArray())
                            );
                        }
                        else
                        {
                            entities.AddRange(
                                entityService.GetAll(UmbracoObjectTypes.Media, mediaLinks.Select(link => link.Udi.Guid).ToArray())
                            );
                        }
                    }

                    var result = new List<LinkDisplay>();
                    foreach(LinkDto dto in links)
                    {
                        if (dto.Id.HasValue || dto.Udi != null)
                        {
                            IUmbracoEntity entity = entities.Find(e => e.Key == dto.Udi?.Guid || e.Id == dto.Id);
                            if (entity == null)
                            {
                                continue;
                            }

                            string entityType = Equals(entity.AdditionalData["NodeObjectTypeId"], Constants.ObjectTypes.MediaGuid) ?
                                Constants.UdiEntityType.Media :
                                Constants.UdiEntityType.Document;

                            var udi = new GuidUdi(entityType, entity.Key);

                            string contentTypeAlias = entity.AdditionalData["ContentTypeAlias"] as string;
                            string icon;
                            bool isMedia = false;
                            bool published = Equals(entity.AdditionalData["IsPublished"], true);
                            string url = dto.Url;

                            if(string.IsNullOrEmpty(contentTypeAlias))
                            {
                                continue;
                            }

                            if (udi.EntityType == Constants.UdiEntityType.Document)
                            {
                                IContentType contentType = contentTypeService.GetContentType(contentTypeAlias);

                                if (contentType == null)
                                {
                                    continue;
                                }

                                icon = contentType.Icon;

                                if (UmbracoContext.Current != null)
                                {
                                    url = UmbracoContext.Current.UrlProvider.GetUrl(entity.Id);
                                }
                            }
                            else
                            {
                                IMediaType mediaType = contentTypeService.GetMediaType(contentTypeAlias);

                                if (mediaType == null)
                                {
                                    continue;
                                }

                                icon = mediaType.Icon;
                                isMedia = true;
                                published = true;

                                if (UmbracoContext.Current != null)
                                {
                                    url = UmbracoContext.Current.MediaCache.GetById(entity.Id)?.Url;
                                }
                            }

                            result.Add(new LinkDisplay
                            {
                                Icon = icon,
                                Id = entity.Id,
                                IsMedia = isMedia,
                                Name = dto.Name,
                                Target = dto.Target,
                                Published = published,
                                Udi = udi,
                                Url = url,
                                Querystring = dto.Querystring
                            });
                        }
                        else
                        {
                            result.Add(new LinkDisplay
                            {
                                Icon = "icon-link",
                                Name = dto.Name,
                                Published = true,
                                Target = dto.Target,
                                Url = dto.Url,
                                Querystring = dto.Querystring
                            });
                        }
                    }
                    return result;
                }
                catch(Exception ex)
                {
                    ApplicationContext.Current.ProfilingLogger.Logger.Error<MultiUrlPickerPropertyValueEditor>("Error getting links", ex);
                }

                return base.ConvertDbToEditor(property, propertyType, dataTypeService);
            }

            public override object ConvertEditorToDb(ContentPropertyData editorValue, object currentValue)
            {
                string value = editorValue.Value?.ToString();

                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                try
                {
                    return JsonConvert.SerializeObject(
                        from link in JsonConvert.DeserializeObject<List<LinkDisplay>>(value)
                        select new LinkDto
                        {
                            Name = link.Name,
                            Target = link.Target,
                            Udi = link.Udi,
                            Url = link.Udi == null ? link.Url : null, // only save the url for external links
                            Querystring = link.Querystring
                        },
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                }
                catch (Exception ex)
                {
                    ApplicationContext.Current.ProfilingLogger.Logger.Error<MultiUrlPickerPropertyValueEditor>("Error saving links", ex);
                }
                return base.ConvertEditorToDb(editorValue, currentValue);
            }
        }
    }
}
