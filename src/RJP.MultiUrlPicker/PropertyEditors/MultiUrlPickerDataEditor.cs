namespace RJP.MultiUrlPicker.PropertyEditors
{
    using System;

    using ClientDependency.Core;

    using Umbraco.Core.Logging;
    using Umbraco.Core.PropertyEditors;
    using Umbraco.Core.Services;
    using Umbraco.Web.PropertyEditors;
    using Umbraco.Web.PublishedCache;

    [PropertyEditorAsset(ClientDependencyType.Javascript, "~/App_Plugins/RJP.MultiUrlPicker/MultiUrlPicker.js")]
    [DataEditor("RJP.MultiUrlPicker", "Multi Url Picker", "~/App_Plugins/RJP.MultiUrlPicker/MultiUrlPicker.html",
        ValueType = "JSON", Group ="pickers", Icon = "icon-link")]
    public class MultiUrlPickerPropertyEditor : DataEditor
    {
        private readonly IEntityService _entityService;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        public MultiUrlPickerPropertyEditor(ILogger logger, IEntityService entityService, IPublishedSnapshotAccessor publishedSnapshotAccessor) : base(logger, EditorType.PropertyValue|EditorType.MacroParameter)
        {
            _entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
            _publishedSnapshotAccessor = publishedSnapshotAccessor ?? throw new ArgumentNullException(nameof(publishedSnapshotAccessor));
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new MultiUrlPickerConfigurationEditor();

        protected override IDataValueEditor CreateValueEditor() => new MultiUrlPickerDataValueEditor(_entityService, _publishedSnapshotAccessor, Logger, Attribute);
    }
}
