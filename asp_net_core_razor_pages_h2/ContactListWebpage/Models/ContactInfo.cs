using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ContactListWebpage.Models
{
    [Serializable]
    public class ContactInfo
    {
        [XmlIgnore]
        public Contact Contact { get; set; }
        [XmlIgnore]
        private int? _contactid;
        public int? ContactId
        {
            get { return Contact != null ? Contact.Id : _contactid; }
            set { _contactid = value; }
        }
        [XmlIgnore]
        public InfoType InfoType { get; set; }
        [XmlIgnore]
        private int? _infotypeid;
        public int? InfoTypeId
        {
            get { return InfoType != null ? InfoType.Id : _infotypeid; }
            set { _infotypeid = value; }
        }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
