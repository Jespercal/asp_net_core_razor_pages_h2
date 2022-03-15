using ContactListWebpage.Models;
using Microsoft.AspNetCore.Identity;
using System.Xml.Serialization;

namespace ContactListWebpage.DAL
{
    public class DataHandler : IDataHandler
    {
        private static readonly DataHandler _dataHandlerInstance = new DataHandler();
        public List<Contact> Contacts { get; set; }
        public List<InfoType> InfoTypes { get; set; }
        public List<FavoriteContact> FavoriteContacts { get; set; }
        public List<ContactInfo> _contactInfos { get; set; }
        public static DataHandler GetInstance() => _dataHandlerInstance;
        public static DataHandler Instance => _dataHandlerInstance;

        private DataHandler()
        {
            Contacts = new List<Contact>();
            InfoTypes = new List<InfoType>();
            _contactInfos = new List<ContactInfo>();
            FavoriteContacts = new List<FavoriteContact>();
            Load();
        }

        public void Save()
        {
            XmlSerializer serializer2 = new XmlSerializer(typeof(List<Contact>));
            using (Stream writer2 = new FileStream("contacts.xml", FileMode.Create))
            {
                serializer2.Serialize(writer2, Contacts);
            }
            XmlSerializer serializer3 = new XmlSerializer(typeof(List<InfoType>));
            using (Stream writer3 = new FileStream("infotypes.xml", FileMode.Create))
            {
                serializer3.Serialize(writer3, InfoTypes);
            }

            _contactInfos.Clear();
            foreach (Contact contact in Contacts)
            {
                _contactInfos.AddRange(contact.Infos);
            }
            XmlSerializer serializer4 = new XmlSerializer(typeof(List<ContactInfo>));
            using (Stream writer4 = new FileStream("contactInfos.xml", FileMode.Create))
            {
                serializer4.Serialize(writer4, _contactInfos);
            }

            XmlSerializer serializer5 = new XmlSerializer(typeof(List<FavoriteContact>));
            using (Stream writer5 = new FileStream("favorites.xml", FileMode.Create))
            {
                serializer5.Serialize(writer5, FavoriteContacts);
            }
        }

        public void Load()
        {
            if (File.Exists("infotypes.xml") && File.ReadAllText("infotypes.xml") != "")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<InfoType>));
                using (Stream reader = new FileStream("infotypes.xml", FileMode.Open))
                {
                    InfoTypes = (List<InfoType>)serializer.Deserialize(reader);
                }
            }
            if (File.Exists("contacts.xml") && File.ReadAllText("contacts.xml") != "")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Contact>));
                using (Stream reader = new FileStream("contacts.xml", FileMode.Open))
                {
                    Contacts = (List<Contact>)serializer.Deserialize(reader);
                }
            }
            if (File.Exists("contactInfos.xml") && File.ReadAllText("contactInfos.xml") != "")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ContactInfo>));
                using (Stream reader = new FileStream("contactInfos.xml", FileMode.Open))
                {
                    _contactInfos = (List<ContactInfo>)serializer.Deserialize(reader);
                }
            }

            for(int index = 0; index < _contactInfos.Count;index++)
            {
                _contactInfos[index].Contact = Contacts.Count(dat => dat.Id == _contactInfos[index].ContactId) > 0 ? Contacts.Find(dat => dat.Id == _contactInfos[index].ContactId) : null;
                _contactInfos[index].InfoType = InfoTypes.Count(dat => dat.Id == _contactInfos[index].InfoTypeId) > 0 ? InfoTypes.Find(dat => dat.Id == _contactInfos[index].InfoTypeId) : null;
                if(_contactInfos[index].Contact != null)
                {
                    _contactInfos[index].Contact.Infos.Add(_contactInfos[index]);
                }
            }

            if (File.Exists("favorites.xml") && File.ReadAllText("favorites.xml") != "")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<FavoriteContact>));
                using (Stream reader = new FileStream("favorites.xml", FileMode.Open))
                {
                    FavoriteContacts = (List<FavoriteContact>)serializer.Deserialize(reader);
                    for(int index = 0;index < FavoriteContacts.Count;index++)
                    {
                        FavoriteContacts[index].Contact = Contacts.Count(dat => dat.Id == FavoriteContacts[index].ContactId) > 0 ? Contacts.Find(dat => dat.Id == FavoriteContacts[index].ContactId) : null;
                    }
                }
            }
        }

        #region CRUD for Contacts
        public Contact CreateContact(string name, List<ContactInfo> infos)
        {
            throw new NotImplementedException();
        }
        public Contact CreateContact(Contact contact)
        {
            contact.Id = (Contacts.Count > 0 ? Contacts.Max(dat => dat.Id) : 0) + 1;
            Contacts.Add(contact);
            return contact;
        }
        public bool DeleteContact(int id)
        {
            if (id == null || id == 0) return false;
            if (Contacts.Count(dat => dat.Id == id) > 0)
            {
                Contacts.RemoveAll(dat => dat.Id == id);
                return true;
            }
            return false;
        }
        public Contact UpdateContact(int id, Contact contact)
        {
            int index = Contacts.FindIndex(dat => dat.Id == id);
            Contacts[index] = contact;
            Contacts[index].UpdatedAt = DateTime.UtcNow;
            return Contacts[index];
        }
        public Contact GetContact(int id)
        {
            if (id != null && id != 0 && Contacts.Count(dat => dat.Id == id) > 0)
            {
                Contact contact = Contacts.Find(dat => dat.Id == id);
                for (int index = 0; index < contact.Infos.Count; index++)
                {
                    contact.Infos[index].InfoType = InfoTypes.Find(dat2 => dat2.Id == contact.Infos[index].InfoTypeId);
                }
                return Contacts.Find(dat => dat.Id == id);
            }
            return null;
        }
        public List<Contact> ListContacts()
        {
            return Contacts.Select(dat => GetContact(dat.Id)).ToList();
        }
        #endregion

        #region CRUD for InfoTypes
        public InfoType CreateInfoType(string name)
        {
            throw new NotImplementedException();
        }
        public InfoType CreateInfoType(InfoType infoType)
        {
            infoType.Id = (InfoTypes.Count > 0 ? InfoTypes.Max(dat => dat.Id) : 0) + 1;
            InfoTypes.Add(infoType);
            return infoType;
        }
        public bool DeleteInfoType(int id)
        {
            if (id == null || id == 0) return false;
            if (InfoTypes.Count(dat => dat.Id == id) > 0)
            {
                InfoTypes.RemoveAll(dat => dat.Id == id);
                return true;
            }
            return false;
        }
        public InfoType UpdateInfoType(int id, InfoType infoType)
        {
            int index = InfoTypes.FindIndex(dat => dat.Id == id);
            InfoTypes[index] = infoType;
            return InfoTypes[index];
        }
        public InfoType GetInfoType(int id)
        {
            if (id != null && id != 0 && InfoTypes.Count(dat => dat.Id == id) > 0)
            {
                return InfoTypes.Find(dat => dat.Id == id);
            }
            return null;
        }
        public List<InfoType> ListInfoTypes()
        {
            return InfoTypes.ToList();
        }
        #endregion

        #region CRUD for ContactFavorites
        public FavoriteContact CreateFavorite(IdentityUser user, Contact contact)
        {
            if (user == null || contact == null) return null;
            if (FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.ContactId == contact.Id) > 0) return null;

            FavoriteContact favorite = new FavoriteContact() { UserSid = user.Id, Contact = contact };
            FavoriteContacts.Add(favorite);

            return favorite;
        }
        public FavoriteContact CreateFavorite(IdentityUser user, int contactId)
        {
            if (user == null || contactId == null) return null;
            if (FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.ContactId == contactId) > 0) return null;
            Contact contact = GetContact(contactId);

            FavoriteContact favorite = new FavoriteContact() { UserSid = user.Id, Contact = contact };
            FavoriteContacts.Add(favorite);

            return favorite;
        }
        public bool DeleteFavorite(IdentityUser user, Contact contact)
        {
            if (user == null || contact == null) return false;

            if (FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id) > 0)
            {
                FavoriteContacts.Remove(FavoriteContacts.Find(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id));
                return true;
            }
            return false;
        }
        public bool DeleteFavorite(IdentityUser user, int contactId)
        {
            if (user == null || contactId == null) return false;
            Contact contact = GetContact(contactId);

            if (contact != null && FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id) > 0)
            {
                FavoriteContacts.Remove(FavoriteContacts.Find(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id));
                return true;
            }
            return false;
        }
        public List<Contact> ListFavorites(IdentityUser user)
        {
            if (user == null) return new List<Contact>();

            List<FavoriteContact> favorites = FavoriteContacts.Where(dat => dat.UserSid == user.Id).ToList();

            return Contacts.Where(dat => favorites.Count(dat2 => dat2.ContactId == dat.Id) > 0).ToList();
        }
        #endregion
    }
}
