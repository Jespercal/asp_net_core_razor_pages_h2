using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactListWebpage.Pages
{
    public class InfoTypesIndexModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;

        public List<InfoType> InfoTypes { get; set; }
        public InfoTypesIndexModel(ILogger<FilledPageModel> logger, IMyRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public void OnGet()
        {
            InfoTypes = _repository.ListInfoTypes();
        }

        public void OnPost()
        {

        }
    }
}