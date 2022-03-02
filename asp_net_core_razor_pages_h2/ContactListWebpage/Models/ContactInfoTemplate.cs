using System.Text.RegularExpressions;

namespace ContactListWebpage.Models
{
    public class ContactInfoTemplate
    {
        public InfoType InfoType { get; set; }
        public string? Value { get; set; }
        public bool IsSelected { get; set; }

        public int IsValid()
        {
            if(IsSelected)
            {
                if (Value != null && Value != "")
                {
                    if(InfoType != null && InfoType.Formatting != null && InfoType.Formatting != "")
                    {
                        if(new Regex(InfoType.Formatting).IsMatch(Value))
                        {
                            return 0;
                        }
                        return -2;
                    }
                    return 0;
                }
            }
            return -1;
        }

        public static ContactInfoTemplate FromInfoType( InfoType infoType, List<ContactInfo> infos = null )
        {
            if (infos != null)
            {
                ContactInfo info = infos.Count(dat => dat.InfoType.Id == infoType.Id) > 0 ? infos.Find(dat => dat.InfoType.Id == infoType.Id) : null;
                if (info != null)
                {
                    return new() { InfoType = infoType, IsSelected = true, Value = info.Value };
                }
            }
            return new() { InfoType = infoType, IsSelected = false };
        }
    }
}
