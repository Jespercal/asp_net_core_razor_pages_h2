using ContactListWebpage.Models;
using Microsoft.AspNetCore.Identity;

namespace ContactListWebpage.DAL
{
    public interface IDataHandler
    {
        public List<Contact> Contacts { get; set; }
        public List<InfoType> InfoTypes { get; set; }
        public List<FavoriteContact> FavoriteContacts { get; set; }
        public List<ContactInfo> _contactInfos { get; set; }

        public void Save();
        public void Load();
    }
}
