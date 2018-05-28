namespace RJP.MultiUrlPicker.PropertyEditors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.Editors;
    using Umbraco.Core.Models.Entities;
    using Umbraco.Core.Services;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Web.PublishedCache;

    using Models;

    using Constants = Umbraco.Core.Constants;

    public class MultiUrlPickerDataValueEditor : DataValueEditor
    {
        private readonly IEntityService _entityService;
        private readonly ILogger _logger;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        public MultiUrlPickerDataValueEditor(IEntityService entityService, IPublishedSnapshotAccessor publishedSnapshotAccessor, ILogger logger, DataEditorAttribute attribute) : base(attribute)
        {
            _entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
            _publishedSnapshotAccessor = publishedSnapshotAccessor ?? throw new ArgumentNullException(nameof(publishedSnapshotAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override object ToEditor(Property property, IDataTypeService dataTypeService, string culture = null, string segment = null)
        {
            string value = property.GetValue(culture, segment)?.ToString();

            if (string.IsNullOrEmpty(value))
            {
                return Enumerable.Empty<object>();
            }

            try
            {
                var links = JsonConvert.DeserializeObject<List<LinkDto>>(value);

                List<LinkDto> documentLinks = links.FindAll(link => link.Udi != null && link.Udi.EntityType == Constants.UdiEntityType.Document);
                List<LinkDto> mediaLinks = links.FindAll(link => link.Udi != null && link.Udi.EntityType == Constants.UdiEntityType.Media);

                List<IEntitySlim> entities = new List<IEntitySlim>();
                if (documentLinks.Count > 0)
                {
                    entities.AddRange(
                        _entityService.GetAll(UmbracoObjectTypes.Document, documentLinks.Select(link => link.Udi.Guid).ToArray())
                    );
                }

                if (mediaLinks.Count > 0)
                {
                    entities.AddRange(
                        _entityService.GetAll(UmbracoObjectTypes.Media, mediaLinks.Select(link => link.Udi.Guid).ToArray())
                    );
                }

                var result = new List<LinkDisplay>();
                foreach (LinkDto dto in links)
                {
                    GuidUdi udi = null;
                    string icon = "icon-link";
                    bool isMedia = false;
                    bool published = true;
                    bool trashed = false;
                    string url = dto.Url;

                    if (dto.Udi != null)
                    {
                        IUmbracoEntity entity = entities.Find(e => e.Key == dto.Udi.Guid);
                        if (entity == null)
                        {
                            continue;
                        }

                        if (entity is IDocumentEntitySlim documentEntity)
                        {
                            icon = documentEntity.ContentTypeIcon;
                            published = documentEntity.Published;
                            udi = new GuidUdi(Constants.UdiEntityType.Document, documentEntity.Key);
                            url = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(entity.Key)?.Url ?? "#";
                            trashed = documentEntity.Trashed;
                        }
                        else if(entity is IContentEntitySlim contentEntity)
                        {
                            icon = contentEntity.ContentTypeIcon;
                            isMedia = true;
                            published = false == contentEntity.Trashed;
                            udi = new GuidUdi(Constants.UdiEntityType.Media, contentEntity.Key);
                            url = _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(entity.Key)?.Url ?? "#";
                            trashed = contentEntity.Trashed;
                        }
                        else
                        {
                            // Not supported
                            continue;
                        }
                    }

                    result.Add(new LinkDisplay
                    {
                        Icon = icon,
                        IsMedia = isMedia,
                        Name = dto.Name,
                        Target = dto.Target,
                        Trashed = trashed,
                        Published = published,
                        Udi = udi,
                        Url = url,
                        Querystring = dto.Querystring
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error<MultiUrlPickerDataValueEditor>("Error getting links", ex);
            }

            return base.ToEditor(property, dataTypeService, culture, segment);
        }


        public override object FromEditor(ContentPropertyData editorValue, object currentValue)
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
                        Querystring = string.IsNullOrWhiteSpace(link.Querystring?.Trim()) ? null : link.Querystring.EnsureStartsWith('?')
                    },
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                _logger.Error<MultiUrlPickerDataValueEditor>("Error saving links", ex);
            }

            return base.FromEditor(editorValue, currentValue);
        }
    }
}
