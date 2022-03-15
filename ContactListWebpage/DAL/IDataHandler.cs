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

        public Contact CreateContact(string name, List<ContactInfo> infos);
        public Contact CreateContact(Contact contact);
        public bool DeleteContact(int id);
        public Contact UpdateContact(int id, Contact contact);
        public Contact GetContact(int id);
        public List<Contact> ListContacts();

        public InfoType CreateInfoType(string name);
        public InfoType CreateInfoType(InfoType infoType);
        public bool DeleteInfoType(int id);
        public InfoType UpdateInfoType(int id, InfoType infoType);
        public InfoType GetInfoType(int id);
        public List<InfoType> ListInfoTypes();

        public FavoriteContact CreateFavorite(IdentityUser user, Contact contact);
        public FavoriteContact CreateFavorite(IdentityUser user, int contactId);
        public bool DeleteFavorite(IdentityUser user, Contact contact);
        public bool DeleteFavorite(IdentityUser user, int contactId);
        public List<Contact> ListFavorites(IdentityUser user);
    }
}
