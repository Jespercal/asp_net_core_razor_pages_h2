using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactListWebpage.Pages
{
    public class ContactsIndexModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;

        public List<Contact> Contacts { get; set; }
        public ContactsIndexModel(ILogger<FilledPageModel> logger, IMyRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public void OnGet()
        {
            Contacts = _repository.ListContacts();
        }

        public void OnPost()
        {

        }
    }
}