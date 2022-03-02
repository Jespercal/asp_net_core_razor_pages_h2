using System.ComponentModel.DataAnnotations;

namespace ContactListWebpage.Models
{
    public class InfoType
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "missing_name", ErrorMessageResourceType = typeof(ContactListWebpage.Resources.SharedResource))]
        public string Name { get; set; }
        public string? Formatting { get; set; }
        public string? Example { get; set; }
        public string? Link { get; set; }
    }
}
