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
    }
}
