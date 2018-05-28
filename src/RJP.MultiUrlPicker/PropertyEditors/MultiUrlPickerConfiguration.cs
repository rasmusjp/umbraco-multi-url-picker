namespace RJP.MultiUrlPicker.PropertyEditors
{
    using Umbraco.Core.PropertyEditors;

    public class MultiUrlPickerConfiguration
    {
        [ConfigurationField("minNumberOfItems", "Min number of items", "number")]
        public int? MinNumberOfItems { get; set; }

        [ConfigurationField("maxNumberOfItems", "Max number of items", "number")]
        public int? MaxNumberOfItems { get; set; }
    }
}
