using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ContactListWebpage.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessageResourceName = "missing_name", ErrorMessageResourceType = typeof(ContactListWebpage.Resources.SharedResource))]
        public string Name { get; set; }
        [XmlIgnore]
        public List<FavoriteContact> Favorites { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }

        public User()
        {
            Favorites = new List<FavoriteContact>();
        }
    }
}
