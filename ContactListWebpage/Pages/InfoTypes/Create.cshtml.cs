using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactListWebpage.Pages
{
    public class InfoTypesCreateModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;

        public InfoType InfoType { get; set; }
        public InfoTypesCreateModel(ILogger<FilledPageModel> logger, IMyRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(InfoType infoType )
        {
            if(ModelState.IsValid)
            {
                _repository.CreateInfoType(infoType);
                _repository.SaveChanges();

                return RedirectToPage("/InfoTypes/Index");
            }
            return Page();
        }
    }
}