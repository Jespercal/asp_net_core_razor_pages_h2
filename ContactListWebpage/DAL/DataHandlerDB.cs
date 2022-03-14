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
        private string connectionString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-ContactListWebpage-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true";

        private DataHandlerDB()
        {
            Contacts = new List<Contact>();
            InfoTypes = new List<InfoType>();
            _contactInfos = new List<ContactInfo>();
            FavoriteContacts = new List<FavoriteContact>();
            Load();
        }

        public void Save()
        {
            return;
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
            if (!_isSetup) Setup();
            return;

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

        public void Setup()
        {
            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].Contacts') AND type in (N'U'))
                                                        BEGIN
                                                            CREATE TABLE Contacts(Id bigint NOT NULL IDENTITY(1,1), Name varchar(200) NOT NULL, CreatedAt datetime NOT NULL DEFAULT GETDATE(), UpdatedAt datetime, PRIMARY KEY (Id));
                                                        END;");

            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].FavoriteContacts') AND type in (N'U'))
                                                        BEGIN
                                                            CREATE TABLE FavoriteContacts(UserSid nvarchar(450) NOT NULL UNIQUE FOREIGN KEY REFERENCES AspNetUsers(Id), ContactId bigint NOT NULL UNIQUE FOREIGN KEY REFERENCES Contacts(Id));
                                                        END;");

            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].ContactInfos') AND type in (N'U'))
                                                        BEGIN
                                                            CREATE TABLE ContactInfos(ContactId bigint NOT NULL UNIQUE FOREIGN KEY REFERENCES Contacts(Id), InfoTypeId bigint NOT NULL UNIQUE, Value varchar(200) NOT NULL, CreatedAt datetime NOT NULL DEFAULT GETDATE(), PRIMARY KEY (ContactId,InfoTypeId));
                                                        END;");

            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].InfoTypes') AND type in (N'U'))
                                                        BEGIN
                                                            CREATE TABLE InfoTypes(Id bigint NOT NULL IDENTITY(1,1), Name varchar(200) NOT NULL, Formatting varchar(200), Example varchar(200), Link varchar(200), PRIMARY KEY (Id));
                                                        END;");

            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CreateContact') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE CreateContact @Name varchar(200)
                                                            AS
                                                            INSERT INTO Contacts (Name) VALUES (@Name);')
                                                        END;");

            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpdateContact') AND type in (N'P'))
                                                        BEGIN
                                                            EXEC ('CREATE PROCEDURE UpdateContact @Id bigint, @Name varchar(200)
                                                            AS
                                                            UPDATE Contacts set Name = @Name, UpdatedAt = GETDATE() WHERE Id = @Id;')
                                                        END;");

            SqlHelper.CreateCommand(connectionString, @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpdateContact') AND type in (N'P'))
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

            //SqlHelper.ExecuteNonQuery(connectionString, "CreateContact", System.Data.CommandType.StoredProcedure, new SqlParameter[] { new SqlParameter("@Name", "New test") });
            //SqlHelper.ExecuteNonQuery(connectionString, "UpdateContact", System.Data.CommandType.StoredProcedure, new SqlParameter[] { new SqlParameter("@Id",1), new SqlParameter("@Name", "New test 2") });

            SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, "ListContacts", System.Data.CommandType.StoredProcedure, new SqlParameter[] { });
            while (reader.Read())
            {
                System.Diagnostics.Debug.WriteLine(String.Format("{0}, {1}, {2}, {3}", reader[0], reader[1], reader[2], reader[3]));
            }

            _isSetup = true;
        }
    }
}
