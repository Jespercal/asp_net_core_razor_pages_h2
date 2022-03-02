using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactListWebpage.Pages
{
    public class ContactsDeleteModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;
        public ContactsDeleteModel(ILogger<FilledPageModel> logger, IMyRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public IActionResult OnPost( int id )
        {
            if(id != null)
            {
                if(_repository.DeleteContact(id))
                {
                    return RedirectToPage("/Contacts/Index");
                }
            }
            return NotFound();
        }
    }
}