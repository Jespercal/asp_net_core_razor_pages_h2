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
            return _dataHandler.CreateContact( contact );
        }
        public bool DeleteContact(int id)
        {
            return _dataHandler.DeleteContact( id );
        }
        public Contact UpdateContact(int id, Contact contact)
        {
            return _dataHandler.UpdateContact( id, contact );
        }
        public Contact GetContact( int id )
        {
            return _dataHandler.GetContact( id );
        }
        public List<Contact> ListContacts()
        {
            return _dataHandler.ListContacts();
        }
        #endregion

        #region CRUD for InfoTypes
        public InfoType CreateInfoType(string name)
        {
            throw new NotImplementedException();
        }
        public InfoType CreateInfoType( InfoType infoType )
        {
            return _dataHandler.CreateInfoType( infoType );
        }
        public bool DeleteInfoType(int id)
        {
            return _dataHandler.DeleteInfoType( id );
        }
        public InfoType UpdateInfoType(int id, InfoType infoType)
        {
            return _dataHandler.UpdateInfoType( id, infoType );
        }
        public InfoType GetInfoType(int id)
        {
            return _dataHandler.GetInfoType( id );
        }
        public List<InfoType> ListInfoTypes()
        {
            return _dataHandler.ListInfoTypes();
        }
        #endregion

        #region CRUD for ContactFavorites
        public FavoriteContact CreateFavorite( IdentityUser user, Contact contact )
        {
            return _dataHandler.CreateFavorite( user, contact );
        }
        public FavoriteContact CreateFavorite(IdentityUser user, int contactId )
        {
            return _dataHandler.CreateFavorite(user, contactId);
        }
        public bool DeleteFavorite( IdentityUser user, Contact contact )
        {
            return _dataHandler.DeleteFavorite( user, contact );
        }
        public bool DeleteFavorite(IdentityUser user, int contactId)
        {
            return _dataHandler.DeleteFavorite(user, contactId);
        }
        public List<Contact> ListFavorites( IdentityUser user )
        {
            return _dataHandler.ListFavorites( user );
        }
        #endregion

        // Save changes to data
        public void SaveChanges()
        {
            _dataHandler.Save();
        }
    }
}
