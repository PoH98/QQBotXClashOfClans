using System.Collections.Generic;
using System.Xml.Serialization;

namespace Native.Csharp.App.Model
{
	[XmlRoot(ElementName = "posts")]
	public class Posts
	{
		[XmlElement(ElementName = "post")]
		public List<Post> Post { get; set; }
		[XmlAttribute(AttributeName = "count")]
		public string Count { get; set; }
		[XmlAttribute(AttributeName = "offset")]
		public string Offset { get; set; }
	}
}
