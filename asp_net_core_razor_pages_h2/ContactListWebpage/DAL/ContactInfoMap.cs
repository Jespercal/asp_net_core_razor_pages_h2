using ContactListWebpage.Models;

namespace ContactListWebpage.DAL
{
    public class ContactInfoMap
    {
        public int ContactId { get; set; }
        public int InfoTypeId { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public static ContactInfoMap CreateFrom( Contact contact, ContactInfo info )
        {
            return new() { ContactId = contact.Id, InfoTypeId = info.InfoType.Id, Value = info.Value, CreatedAt = info.CreatedAt };
        }

        public static List<ContactInfoMap> CreateList(Contact contact, List<ContactInfo> infos)
        {
            List<ContactInfoMap> list = new List<ContactInfoMap>();
            foreach (ContactInfo info in infos)
            {
                list.Add(CreateFrom(contact, info));
            }
            return list;
        }
    }
}
