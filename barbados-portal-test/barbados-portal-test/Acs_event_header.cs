using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace barbados_portal_test
{
	[XmlRoot(ElementName = "patch")]
	public class Patch
	{
		[XmlAttribute(AttributeName = "length")]
		public string Length { get; set; }
		[XmlAttribute(AttributeName = "offset")]
		public string Offset { get; set; }
	}

	[XmlRoot(ElementName = "full")]
	public class Full
	{
		[XmlAttribute(AttributeName = "length")]
		public string Length { get; set; }
		[XmlAttribute(AttributeName = "offset")]
		public string Offset { get; set; }
	}

	[XmlRoot(ElementName = "overview")]
	public class Overview
	{
		[XmlAttribute(AttributeName = "length")]
		public string Length { get; set; }
		[XmlAttribute(AttributeName = "offset")]
		public string Offset { get; set; }
	}

	[XmlRoot(ElementName = "acs_event_header")]
	public class Acs_event_header
	{
		[XmlElement(ElementName = "lpr")]
		public string Lpr { get; set; }
		[XmlElement(ElementName = "patch")]
		public Patch Patch { get; set; }
		[XmlElement(ElementName = "full")]
		public Full Full { get; set; }
		[XmlElement(ElementName = "overview")]
		public Overview Overview { get; set; }

		

		
	}
}
