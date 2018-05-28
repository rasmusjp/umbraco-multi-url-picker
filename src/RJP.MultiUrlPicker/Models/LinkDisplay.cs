namespace RJP.MultiUrlPicker.Models
{
    using System.Runtime.Serialization;

    using Umbraco.Core;

    [DataContract]
    internal class LinkDisplay
    {
        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "isMedia")]
        public bool IsMedia { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "published")]
        public bool Published { get; set; }

        [DataMember(Name = "querystring")]
        public string Querystring { get; set; }

        [DataMember(Name = "target")]
        public string Target { get; set; }

        [DataMember(Name = "trashed")]
        public bool Trashed { get; set; }

        [DataMember(Name = "udi")]
        public GuidUdi Udi { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
