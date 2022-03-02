using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using Microsoft.AspNetCore.Identity;

namespace ContactListWebpage.Repository
{
    public class MyRepository : IMyRepository
    {
        private IDataHandler _dataHandler;
        public MyRepository( IDataHandler dataHandler )
        {
            _dataHandler = dataHandler;
        }

        #region CRUD for Contacts
        public Contact CreateContact(string name, List<ContactInfo> infos)
        {
            throw new NotImplementedException();
        }
        public Contact CreateContact( Contact contact )
        {
            contact.Id = (_dataHandler.Contacts.Count > 0 ? _dataHandler.Contacts.Max(dat => dat.Id) : 0) + 1;
            _dataHandler.Contacts.Add( contact );
            return contact;
        }
        public bool DeleteContact(int id)
        {
            if (id == null || id == 0) return false;
            if(_dataHandler.Contacts.Count( dat => dat.Id == id) > 0)
            {
                _dataHandler.Contacts.RemoveAll(dat => dat.Id == id);
                return true;
            }
            return false;
        }
        public Contact UpdateContact(int id, Contact contact)
        {
            int index = _dataHandler.Contacts.FindIndex(dat => dat.Id == id);
            _dataHandler.Contacts[index] = contact;
            _dataHandler.Contacts[index].UpdatedAt = DateTime.UtcNow;
            return _dataHandler.Contacts[index];
        }
        public Contact GetContact( int id )
        {
            if (id != null && id != 0 && _dataHandler.Contacts.Count(dat => dat.Id == id) > 0)
            {
                Contact contact = _dataHandler.Contacts.Find(dat => dat.Id == id);
                for(int index = 0;index < contact.Infos.Count;index++)
                {
                    contact.Infos[index].InfoType = _dataHandler.InfoTypes.Find(dat2 => dat2.Id == contact.Infos[index].InfoTypeId);
                }
                return _dataHandler.Contacts.Find(dat => dat.Id == id);
            }
            return null;
        }
        public List<Contact> ListContacts()
        {
            return _dataHandler.Contacts.Select(dat => GetContact(dat.Id)).ToList();
        }
        #endregion

        #region CRUD for InfoTypes
        public InfoType CreateInfoType(string name)
        {
            throw new NotImplementedException();
        }
        public InfoType CreateInfoType( InfoType infoType )
        {
            infoType.Id = (_dataHandler.InfoTypes.Count > 0 ? _dataHandler.InfoTypes.Max(dat => dat.Id) : 0) + 1;
            _dataHandler.InfoTypes.Add(infoType);
            return infoType;
        }
        public bool DeleteInfoType(int id)
        {
            if (id == null || id == 0) return false;
            if (_dataHandler.InfoTypes.Count(dat => dat.Id == id) > 0)
            {
                _dataHandler.InfoTypes.RemoveAll(dat => dat.Id == id);
                return true;
            }
            return false;
        }
        public InfoType UpdateInfoType(int id, InfoType infoType)
        {
            int index = _dataHandler.InfoTypes.FindIndex(dat => dat.Id == id);
            _dataHandler.InfoTypes[index] = infoType;
            return _dataHandler.InfoTypes[index];
        }
        public InfoType GetInfoType(int id)
        {
            if (id != null && id != 0 && _dataHandler.InfoTypes.Count(dat => dat.Id == id) > 0)
            {
                return _dataHandler.InfoTypes.Find(dat => dat.Id == id);
            }
            return null;
        }
        public List<InfoType> ListInfoTypes()
        {
            return _dataHandler.InfoTypes.ToList();
        }
        #endregion

        #region CRUD for ContactFavorites
        public FavoriteContact CreateFavorite( IdentityUser user, Contact contact )
        {
            if (user == null || contact == null) return null;
            if (_dataHandler.FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.ContactId == contact.Id) > 0) return null;

            FavoriteContact favorite = new FavoriteContact() { UserSid = user.Id, Contact = contact };
            _dataHandler.FavoriteContacts.Add(favorite);

            return favorite;
        }
        public FavoriteContact CreateFavorite(IdentityUser user, int contactId )
        {
            if (user == null || contactId == null) return null;
            if (_dataHandler.FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.ContactId == contactId) > 0) return null;
            Contact contact = GetContact(contactId);

            FavoriteContact favorite = new FavoriteContact() { UserSid = user.Id, Contact = contact };
            _dataHandler.FavoriteContacts.Add(favorite);

            return favorite;
        }
        public bool DeleteFavorite( IdentityUser user, Contact contact )
        {
            if (user == null || contact == null) return false;

            if (_dataHandler.FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id) > 0)
            {
                _dataHandler.FavoriteContacts.Remove(_dataHandler.FavoriteContacts.Find(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id));
                return true;
            }
            return false;
        }
        public bool DeleteFavorite(IdentityUser user, int contactId)
        {
            if (user == null || contactId == null) return false;
            Contact contact = GetContact(contactId);

            if (contact != null && _dataHandler.FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id) > 0)
            {
                _dataHandler.FavoriteContacts.Remove(_dataHandler.FavoriteContacts.Find(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id));
                return true;
            }
            return false;
        }
        public List<Contact> ListFavorites( IdentityUser user )
        {
            if (user == null) return new List<Contact>();

            List<FavoriteContact> favorites = _dataHandler.FavoriteContacts.Where(dat => dat.UserSid == user.Id).ToList();

            return _dataHandler.Contacts.Where(dat => favorites.Count(dat2 => dat2.ContactId == dat.Id) > 0).ToList();
        }
        #endregion

        // Save changes to data
        public void SaveChanges()
        {
            _dataHandler.Save();
        }
    }
}
