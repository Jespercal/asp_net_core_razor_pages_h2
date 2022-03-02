using ContactListWebpage.DAL;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ContactListWebpage.Models
{
    public class Contact
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "missing_name", ErrorMessageResourceType = typeof(ContactListWebpage.Resources.SharedResource))]
        public string Name { get; set; }
        [XmlIgnore]
        public List<ContactInfo> Infos { get; set; }
        [XmlIgnore]
        public List<ContactInfoMap> InfosMap => ContactInfoMap.CreateList(this, Infos);
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }

        public Contact()
        {
            Infos = new List<ContactInfo>();
        }

        public void TryAddInfo(ContactInfoTemplate temp)
        {
            if (temp.IsSelected && Infos.Count(dat => dat.InfoType.Id == temp.InfoType.Id) <= 0)
            {
                ContactInfo contactInfo = new ContactInfo();
                contactInfo.Contact = this;
                contactInfo.InfoType = temp.InfoType;
                contactInfo.Value = temp.Value;
                Infos.Add(contactInfo);
            }
            else if (temp.IsSelected && Infos.Count(dat => dat.InfoType.Id == temp.InfoType.Id) > 0)
            {
                int index = Infos.FindIndex(dat => dat.InfoType.Id == temp.InfoType.Id);
                Infos[index].Value = temp.Value;
            }
        }
    }
}
