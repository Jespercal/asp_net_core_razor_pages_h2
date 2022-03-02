using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactListWebpage.Pages
{
    public class InfoTypesEditModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;

        [BindProperty(SupportsGet = true)] public InfoType InfoType { get; set; }
        public InfoTypesEditModel(ILogger<FilledPageModel> logger, IMyRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult OnGet( int id )
        {
            List<InfoType> infoTypes = _repository.ListInfoTypes();
            InfoType infoType = _repository.GetInfoType(id);
            InfoType = infoType;
            if (InfoType != null)
            {
                return Page();
            }
            return NotFound();
        }

        public IActionResult OnPost( [FromRoute] int id, InfoType infoType)
        {
            if(ModelState.IsValid)
            {
                _repository.UpdateInfoType(id, infoType);
                _repository.SaveChanges();

                return RedirectToPage("/InfoTypes/Index");
            }
            return Page();
        }
    }
}