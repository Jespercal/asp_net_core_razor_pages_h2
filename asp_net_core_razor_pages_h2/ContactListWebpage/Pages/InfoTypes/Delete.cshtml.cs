using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactListWebpage.Pages
{
    public class InfoTypesDeleteModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;
        public InfoTypesDeleteModel(ILogger<FilledPageModel> logger, IMyRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public IActionResult OnPost( int id )
        {
            if(id != null)
            {
                if(_repository.DeleteInfoType(id))
                {
                    return RedirectToPage("/InfoTypes/Index");
                }
            }
            return NotFound();
        }
    }
}