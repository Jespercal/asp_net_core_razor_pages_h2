using ContactListWebpage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;

namespace ContactListWebpage.DAL
{
    public class DataHandlerDB : IDataHandler
    {
        private static readonly DataHandlerDB _dataHandlerInstance = new DataHandlerDB();
        public List<Contact> Contacts { get; set; }
        public List<InfoType> InfoTypes { get; set; }
        public List<FavoriteContact> FavoriteContacts { get; set; }
        public List<ContactInfo> _contactInfos { get; set; }
        public static DataHandlerDB GetInstance() => _dataHandlerInstance;
        public static DataHandlerDB Instance => _dataHandlerInstance;
        private bool _isSetup;
        private string connectionString = "";

        public Queue<SPBuilder> RunSPOnSave { get; set; } = new Queue<SPBuilder>();

        private DataHandlerDB()
        {
            connectionString = WebApplication.CreateBuilder().Configuration.GetConnectionString("DefaultConnection");
            Contacts = new List<Contact>();
            InfoTypes = new List<InfoType>();
            _contactInfos = new List<ContactInfo>();
            FavoriteContacts = new List<FavoriteContact>();
            Load();
        }

        public void RunOnSave( SPBuilder builder )
        {
            RunSPOnSave.Enqueue(builder);
            //builder.Execute();
        }

        public void Save()
        {
            while(RunSPOnSave.Count > 0)
            {
                RunSPOnSave.Dequeue().Execute();
            }
        }

        public void Load()
        {
            if (!_isSetup) Setup();

            {
                Contacts = new List<Contact>();
                SqlHelper.ExecuteReader(connectionString, "ListContacts", System.Data.CommandType.StoredProcedure, (reader) =>
                {
                    while (reader.Read())
                    {
                        Contact contact = new Contact()
                        {
                            Id = (int)(Int64)reader[0],
                            Name = (string)reader[1],
                            CreatedAt = (DateTime)reader[2]
                        };
                        if (reader[3].GetType() != typeof(DBNull))
                        {
                            contact.UpdatedAt = (DateTime)reader[3];
                        }
                        Contacts.Add(contact);
                    }
                }, new SqlParameter[] { });
            }

            {
                InfoTypes = new List<InfoType>();
                SqlHelper.ExecuteReader(connectionString, "ListInfoTypes", System.Data.CommandType.StoredProcedure, (reader) =>
                {
                    while (reader.Read())
                    {
                        InfoType infotype = new InfoType()
                        {
                            Id = (int)(Int64)reader[0],
                            Name = (string)reader[1],
                            Formatting = reader[2].GetType() != typeof(DBNull) ? (string)reader[2] : null,
                            Example = reader[3].GetType() != typeof(DBNull) ? (string)reader[3] : null,
                            Link = reader[4].GetType() != typeof(DBNull) ? (string)reader[4] : null
                        };
                        InfoTypes.Add(infotype);
                    }
                }, new SqlParameter[] { });
            }

            {
                _contactInfos = new List<ContactInfo>();
                SqlHelper.ExecuteReader(connectionString, "ListContactInfos", System.Data.CommandType.StoredProcedure, (reader) =>
                {
                    while (reader.Read())
                    {
                        ContactInfo contactinfo = new ContactInfo()
                        {
                            ContactId = (int)(Int64)reader[0],
                            InfoTypeId = (int)(Int64)reader[1],
                            Value = (string)reader[2]
                        };
                        _contactInfos.Add(contactinfo);
                    }
                }, new SqlParameter[] { });
            }
            for (int index = 0; index < _contactInfos.Count; index++)
            {
                _contactInfos[index].Contact = Contacts.Count(dat => dat.Id == _contactInfos[index].ContactId) > 0 ? Contacts.Find(dat => dat.Id == _contactInfos[index].ContactId) : null;
                _contactInfos[index].InfoType = InfoTypes.Count(dat => dat.Id == _contactInfos[index].InfoTypeId) > 0 ? InfoTypes.Find(dat => dat.Id == _contactInfos[index].InfoTypeId) : null;
                if (_contactInfos[index].Contact != null)
                {
                    _contactInfos[index].Contact.Infos.Add(_contactInfos[index]);
                }
            }

            {
                FavoriteContacts = new List<FavoriteContact>();
                SqlHelper.ExecuteReader(connectionString, "ListFavorites", System.Data.CommandType.StoredProcedure, (reader) =>
                {
                    while (reader.Read())
                    {
                        FavoriteContact favorite = new FavoriteContact()
                        {
                            UserSid = (string)reader[0],
                            ContactId = (int)(Int64)reader[1],
                        };
                        FavoriteContacts.Add(favorite);
                    }
                }, new SqlParameter[] { });
                for (int index = 0; index < FavoriteContacts.Count; index++)
                {
                    FavoriteContacts[index].Contact = Contacts.Count(dat => dat.Id == FavoriteContacts[index].ContactId) > 0 ? Contacts.Find(dat => dat.Id == FavoriteContacts[index].ContactId) : null;
                }
            }
        }

        public void Setup()
        {
            #region Tables
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].Contacts') AND type in (N'U'))
                                                        BEGIN
                                                            CREATE TABLE Contacts(Id bigint NOT NULL, Name varchar(200) NOT NULL, CreatedAt datetime NOT NULL DEFAULT GETDATE(), UpdatedAt datetime, PRIMARY KEY (Id));
                                                        END;");

            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].FavoriteContacts') AND type in (N'U'))
                                                        BEGIN
                                                            CREATE TABLE FavoriteContacts(UserSid nvarchar(256) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id), ContactId bigint NOT NULL FOREIGN KEY REFERENCES Contacts(Id), PRIMARY KEY (UserSid,ContactId));
                                                        END;");

            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].ContactInfos') AND type in (N'U'))
                                                        BEGIN
                                                            CREATE TABLE ContactInfos(ContactId bigint NOT NULL FOREIGN KEY REFERENCES Contacts(Id), InfoTypeId bigint NOT NULL, Value varchar(200) NOT NULL, CreatedAt datetime NOT NULL DEFAULT GETDATE(), PRIMARY KEY (ContactId,InfoTypeId));
                                                        END;");

            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].InfoTypes') AND type in (N'U'))
                                                        BEGIN
                                                            CREATE TABLE InfoTypes(Id bigint NOT NULL, Name varchar(200) NOT NULL, Formatting varchar(200), Example varchar(200), Link varchar(200), PRIMARY KEY (Id));
                                                        END;");
            #endregion

            #region Contacts
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpsertContact') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE UpsertContact @Id bigint, @Name varchar(200)
                                                            AS
                                                            BEGIN
                                                                BEGIN TRANSACTION Upsert
                                                                UPDATE Contacts set Name = @Name, UpdatedAt = GETDATE() WHERE Id = @Id;
                                                                IF @@ROWCOUNT = 0
                                                                BEGIN
                                                                    INSERT INTO Contacts (Id,Name) VALUES (@Id,@Name);
                                                                END
                                                                COMMIT TRANSACTION Upsert
                                                            END');
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CreateContact') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE CreateContact @Id bigint, @Name varchar(200)
                                                            AS
                                                            INSERT INTO Contacts (Id,Name) VALUES (@Id,@Name);')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpdateContact') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE UpdateContact @Id bigint, @Name varchar(200)
                                                            AS
                                                            UPDATE Contacts set Name = @Name, UpdatedAt = GETDATE() WHERE Id = @Id;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'DeleteContact') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE DeleteContact @Id bigint
                                                            AS
                                                            DELETE FROM Contacts WHERE Id = @Id;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'GetContact') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE GetContact @Id bigint
                                                            AS
                                                            SELECT * FROM Contacts WHERE Id = @Id;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ListContacts') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE ListContacts
                                                            AS
                                                            SELECT * FROM Contacts')
                                                        END;");
            #endregion

            #region ContactInfo
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpsertContactInfo') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE UpsertContactInfo @ContactId bigint, @InfoTypeId bigint, @Value varchar(200)
                                                            AS
                                                            BEGIN
                                                                BEGIN TRANSACTION Upsert
                                                                UPDATE ContactInfos set Value = @Value WHERE ContactId = @ContactId AND InfoTypeId = @InfoTypeId;
                                                                IF @@ROWCOUNT = 0
                                                                BEGIN
                                                                    INSERT INTO ContactInfos (ContactId, InfoTypeId, Value) VALUES (@ContactId, @InfoTypeId, @Value);
                                                                END
                                                                COMMIT TRANSACTION Upsert
                                                            END');
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CreateContactInfo') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE CreateContactInfo @ContactId bigint, @InfoTypeId bigint, @Value varchar(200)
                                                            AS
                                                            INSERT INTO ContactInfos (ContactId, InfoTypeId, Value) VALUES (@ContactId, @InfoTypeId, @Value);')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpdateContactInfo') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE UpdateContactInfo @ContactId bigint, @InfoTypeId bigint, @Value varchar(200)
                                                            AS
                                                            UPDATE ContactInfos set Value = @Value WHERE ContactId = @ContactId AND InfoTypeId = @InfoTypeId;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'DeleteContactInfos') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE DeleteContactInfos @ContactId bigint
                                                            AS
                                                            DELETE FROM ContactInfos WHERE ContactId = @ContactId;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'DeleteContactInfo') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE DeleteContactInfo @ContactId bigint, @InfoTypeId bigint
                                                            AS
                                                            DELETE FROM ContactInfos WHERE ContactId = @ContactId AND InfoTypeId = @InfoTypeId;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'GetContactInfo') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE GetContactInfo @ContactId bigint, @InfoTypeId bigint
                                                            AS
                                                            SELECT * FROM ContactInfos WHERE ContactId = @ContactId AND InfoTypeId = @InfoTypeId;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ListContactInfos') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE ListContactInfos
                                                            AS
                                                            SELECT * FROM ContactInfos')
                                                        END;");
            #endregion

            #region InfoTypes
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpsertInfoType') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE UpsertInfoType @Id bigint, @Name varchar(200), @Formatting varchar(200), @Example varchar(200), @Link varchar(200)
                                                            AS
                                                            BEGIN
                                                                BEGIN TRANSACTION Upsert
                                                                UPDATE InfoTypes set Name = @Name, Formatting = @Formatting, Example = @Example, Link = @Link WHERE Id = @Id;
                                                                IF @@ROWCOUNT = 0
                                                                BEGIN
                                                                    INSERT INTO InfoTypes (Id, Name, Formatting, Example, Link) VALUES (@Id, @Name, @Formatting, @Example, @Link)
                                                                END
                                                                COMMIT TRANSACTION Upsert
                                                            END');
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CreateInfoType') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE (CreateInfoType @Id bigint, @Name varchar(200), @Formatting varchar(200), @Example varchar(200), @Link varchar(200))
                                                            AS
                                                            BEGIN
                                                                INSERT INTO [InfoTypes] (Id, Name, Formatting, Example, Link) VALUES (@Id, @Name, @Formatting, @Example, @Link);
                                                            END');
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpdateInfoType') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE UpdateInfoType @Id bigint, @Name varchar(200), @Formatting varchar(200), @Example varchar(200), @Link varchar(200)
                                                            AS
                                                            UPDATE InfoTypes set Name = @Name, Formatting = @Formatting, Example = @Example, Link = @Link WHERE Id = @Id;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'DeleteInfoType') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE DeleteInfoType @Id bigint
                                                            AS
                                                            DELETE FROM InfoTypes WHERE Id = @Id;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'GetInfoType') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE GetInfoType @Id bigint
                                                            AS
                                                            SELECT * FROM InfoTypes WHERE Id = @Id;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ListInfoTypes') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE ListInfoTypes
                                                            AS
                                                            SELECT * FROM InfoTypes')
                                                        END;");
            #endregion

            #region Favorites
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CreateFavorite') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE CreateFavorite @UserSid nvarchar(450), @ContactId bigint
                                                            AS
                                                            INSERT INTO FavoriteContacts (UserSid,ContactId) VALUES (@UserSid,@ContactId);')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'DeleteFavorite') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE DeleteFavorite @UserSid nvarchar(450), @ContactId bigint
                                                            AS
                                                            DELETE FROM FavoriteContacts WHERE UserSid = @UserSid AND ContactId = @ContactId;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'GetFavorite') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE GetFavorite @UserSid nvarchar(450), @ContactId bigint
                                                            AS
                                                            SELECT * FROM FavoriteContacts WHERE UserSid = @UserSid AND ContactId = @ContactId;')
                                                        END;");
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ListFavorites') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE ListFavorites
                                                            AS
                                                            SELECT * FROM FavoriteContacts')
                                                        END;");
            #endregion

            _isSetup = true;
        }


        #region CRUD for Contacts
        public Contact CreateContact(Contact contact)
        {
            contact.Id = (Contacts.Count > 0 ? Contacts.Max(dat => dat.Id) : 0) + 1;
            Contacts.Add(contact);

            RunOnSave(new SPBuilder("CreateContact").Add("Id", contact.Id).Add("Name",contact.Name));

            return contact;
        }
        public bool DeleteContact(int id)
        {
            if (id == null || id == 0) return false;
            if (Contacts.Count(dat => dat.Id == id) > 0)
            {
                Contacts.RemoveAll(dat => dat.Id == id);
                RunOnSave(new SPBuilder("DeleteContact").Add("Id",id));
                return true;
            }
            return false;
        }
        public Contact UpdateContact(int id, Contact contact)
        {
            int index = Contacts.FindIndex(dat => dat.Id == id);
            Contacts[index] = contact;
            Contacts[index].UpdatedAt = DateTime.UtcNow;

            RunOnSave(new SPBuilder("UpdateContact").Add("Id", id).Add("Name",contact.Name));//.Execute();
            RunOnSave(new SPBuilder("DeleteContactInfos").Add("ContactId", id));//.Execute();
            Contacts[index].Infos.ForEach(info =>
            {
                RunOnSave(new SPBuilder("UpsertContactInfo").Add("ContactId", id).Add("InfoTypeId", info.InfoType.Id).Add("Value", info.Value));//.Execute();
            });

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
        public InfoType CreateInfoType(InfoType infoType)
        {
            infoType.Id = (InfoTypes.Count > 0 ? InfoTypes.Max(dat => dat.Id) : 0) + 1;
            InfoTypes.Add(infoType);

            RunOnSave(new SPBuilder("UpsertInfoType")
                .Add("Id", infoType.Id)
                .Add("Name", infoType.Name)
                .Add("Formatting", infoType.Formatting != null ? infoType.Formatting : DBNull.Value)
                .Add("Example", infoType.Example != null ? infoType.Example : DBNull.Value)
                .Add("Link", infoType.Link != null ? infoType.Link : DBNull.Value));
                //.Execute();

            return infoType;
        }
        public bool DeleteInfoType(int id)
        {
            if (id == null || id == 0) return false;
            if (InfoTypes.Count(dat => dat.Id == id) > 0)
            {
                InfoTypes.RemoveAll(dat => dat.Id == id);

                RunOnSave(new SPBuilder("DeleteInfoType").Add("Id", id));//.Execute();

                return true;
            }
            return false;
        }
        public InfoType UpdateInfoType(int id, InfoType infoType)
        {
            int index = InfoTypes.FindIndex(dat => dat.Id == id);
            InfoTypes[index] = infoType;

            RunOnSave(new SPBuilder("UpsertInfoType")
                .Add("Id", id)
                .Add("Name", infoType.Name)
                .Add("Formatting", infoType.Formatting != null ? infoType.Formatting : DBNull.Value)
                .Add("Example", infoType.Example != null ? infoType.Example : DBNull.Value)
                .Add("Link", infoType.Link != null ? infoType.Link : DBNull.Value));
                //.Execute();

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

            RunOnSave(new SPBuilder("CreateFavorite")
                .Add("UserSid", favorite.UserSid)
                .Add("ContactId", favorite.ContactId.Value));
                //.Execute();

            return favorite;
        }
        public FavoriteContact CreateFavorite(IdentityUser user, int contactId)
        {
            if (user == null || contactId == null) return null;
            if (FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.ContactId == contactId) > 0) return null;
            Contact contact = GetContact(contactId);

            FavoriteContact favorite = new FavoriteContact() { UserSid = user.Id, Contact = contact };
            FavoriteContacts.Add(favorite);

            RunOnSave(new SPBuilder("CreateFavorite")
                .Add("UserSid", favorite.UserSid)
                .Add("ContactId", contactId));
                //.Execute();

            return favorite;
        }
        public bool DeleteFavorite(IdentityUser user, Contact contact)
        {
            if (user == null || contact == null) return false;

            if (FavoriteContacts.Count(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id) > 0)
            {
                FavoriteContacts.Remove(FavoriteContacts.Find(dat => dat.UserSid == user.Id && dat.Contact.Id == contact.Id));

                RunOnSave(new SPBuilder("DeleteFavorite").Add("UserSid", user.Id).Add("ContactId", contact.Id));//.Execute();

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

                RunOnSave(new SPBuilder("DeleteFavorite").Add("UserSid", user.Id).Add("ContactId", contactId));//.Execute();

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
