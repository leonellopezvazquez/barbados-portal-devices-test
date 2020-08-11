using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace barbados_portal_test
{

    [XmlRoot(ElementName = "FTP")]
    public class FTP
    {
        [XmlElement(ElementName = "IN")]
        public string IN { get; set; }
        [XmlElement(ElementName = "OUT")]

        public string INTER { get; set; }
        [XmlElement(ElementName = "INTER")]
        public string OUT { get; set; }
        [XmlElement(ElementName = "CROPPED")]
        public string CROPPED { get; set; }
        [XmlElement(ElementName = "EVENTIN")]
        public string EventIn { get; set; }
        [XmlElement(ElementName = "EVENTOUT")]
        public string EventOut { get; set; }
        [XmlElement(ElementName = "ISLOCAL")]
        public string IsLocal { get; set; }
        [XmlElement(ElementName = "OCR")]
        public string OCR { get; set; }

        [XmlElement(ElementName = "ASUR01")]
        public string IdCamera1 { get; set; }

        [XmlElement(ElementName = "ASUR02")]
        public string IdCamera2 { get; set; }

        [XmlElement(ElementName = "ASUR03")]
        public string IdCamera3 { get; set; }
    }

    [XmlRoot(ElementName = "S3")]
    public class S3
    {
        [XmlElement(ElementName = "REGION")]
        public string REGION { get; set; }
        [XmlElement(ElementName = "BUCKIN")]
        public string BUCKIN { get; set; }
        [XmlElement(ElementName = "BUCKOUT")]
        public string BUCKOUT { get; set; }
        [XmlElement(ElementName = "ACCESS")]
        public string ACCESS { get; set; }
        [XmlElement(ElementName = "SECRET")]
        public string SECRET { get; set; }
    }


    [XmlRoot(ElementName = "DB")]
    public class DB
    {
        [XmlElement(ElementName = "IP")]
        public string IP { get; set; }
        [XmlElement(ElementName = "database")]
        public string Database { get; set; }
        [XmlElement(ElementName = "USER")]
        public string USER { get; set; }
        [XmlElement(ElementName = "PASSWORD")]
        public string PASSWORD { get; set; }
        [XmlElement(ElementName = "TIME")]
        public string Interval { get; set; }
    }





    [XmlRoot(ElementName = "CONFIG")]
    public class ConfigObj
    {
        [XmlElement(ElementName = "FTP")]
        public FTP FTP { get; set; }
        [XmlElement(ElementName = "S3")]
        public S3 S3 { get; set; }
        [XmlElement(ElementName = "DB")]
        public DB DB { get; set; }

        [XmlElement(ElementName = "INI")]
        public INI INI { get; set; }
        [XmlElement(ElementName = "MATCH")]
        public MATCH MATCH { get; set; }
    }

    public class MATCH
    {
        [XmlElement(ElementName = "MinPlateConfidence")]
        public int MinPlateConfidence { get; set; }

        [XmlElement(ElementName = "Speed")]
        public string Speed { get; set; }

        [XmlElement(ElementName = "PlateWeigth")]
        public int PlateWeigth { get; set; }

        [XmlElement(ElementName = "MantisWeigth")]
        public int MantisWeigth { get; set; }

        [XmlElement(ElementName = "Distance")]
        public int Distance { get; set; }

    }



    public class INI
    {
        [XmlElement(ElementName = "Logo")]
        public string Logo { get; set; }
    }

}
