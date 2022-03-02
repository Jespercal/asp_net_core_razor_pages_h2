using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace ContactListWebpage.Pages
{
    [BindProperties]
    public class ContactsEditModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;
        protected readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public Contact Contact { get; set; }
        public List<ContactInfoTemplate> Infos { get; set; }
        public ContactsEditModel(ILogger<FilledPageModel> logger, IMyRepository repository, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _logger = logger;
            _repository = repository;
            _sharedLocalizer = sharedLocalizer;
        }

        public IActionResult OnGet( int id )
        {
            List<Contact> contacts = _repository.ListContacts();
            Contact contact = _repository.GetContact(id);
            Contact = contact;
            if (Contact != null)
            {
                Infos = new List<ContactInfoTemplate>();
                Infos.AddRange(_repository.ListInfoTypes().Select(dat => ContactInfoTemplate.FromInfoType(dat, Contact.Infos)));

                return Page();
            }
            return NotFound();
        }

        public IActionResult OnPost( [FromRoute] int id )
        {
            int counter = 0;
            foreach (ContactInfoTemplate temp in Infos)
            {
                int result = temp.IsValid();
                if (temp.IsSelected && result != 0)
                {
                    ModelState.AddModelError($"Infos[{counter}].Value", result == -1 ? _sharedLocalizer["value_not_allowed"] : _sharedLocalizer["value_wrong_format"]);
                }
                else if(temp.IsSelected && result == 0)
                {
                    Contact.TryAddInfo(temp);
                }
                counter++;
            }
            if (Contact.Infos.Count <= 0)
            {
                ModelState.AddModelError("Overall", _sharedLocalizer["missing_one_or_more"]);
            }
            if (ModelState.IsValid)
            {
                Contact.UpdatedAt = DateTime.UtcNow;
                _repository.UpdateContact(id, Contact );
                _repository.SaveChanges();

                return RedirectToPage("/Contacts/Index");
            }
            return Page();
        }
    }
}