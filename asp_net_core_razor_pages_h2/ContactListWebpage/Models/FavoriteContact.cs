using System.Xml.Serialization;

namespace ContactListWebpage.Models
{
    public class FavoriteContact
    {
        public string UserSid { get; set; }
        [XmlIgnore]
        public Contact Contact { get; set; }
        [XmlIgnore]
        private int? _contactid;
        public int? ContactId
        {
            get { return Contact != null ? Contact.Id : _contactid; }
            set { _contactid = value; }
        }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
