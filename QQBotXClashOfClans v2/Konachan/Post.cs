using System.Xml;
using System.Xml.Serialization;

namespace Native.Csharp.App.Model
{
    [XmlRoot(ElementName = "post")]
    public class Post
    {
        [XmlAttribute]
        public uint id { get; set; }
        [XmlAttribute]
        public string tags { get; set; }
        [XmlAttribute]
        public string file_url { get; set; }
        [XmlAttribute]
        public string jpeg_url { get; set; }
        [XmlAttribute]
        public string rating { get; set; }
    }
}
