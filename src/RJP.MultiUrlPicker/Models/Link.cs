namespace RJP.MultiUrlPicker.Models
{
    using Umbraco.Core;

    public class Link
    {
        public string Name { get; set; }
        public string Target { get; set; }
        public LinkType Type { get; set; }
        public Udi Udi { get; set; }
        public string Url { get; set; }
    }
}
