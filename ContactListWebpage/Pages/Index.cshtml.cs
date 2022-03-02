using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace ContactListWebpage.Pages
{
    public class IndexModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;
        protected readonly UserManager<IdentityUser> _userManager;

        public List<Contact> Contacts { get; set; }
        public List<Contact> Favorites { get; set; }
        public IndexModel(ILogger<FilledPageModel> logger, IMyRepository repository, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _repository = repository;
            _userManager = userManager;
        }

        public async Task OnGet( [FromQuery] string? sort )
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            Favorites = (_repository.ListFavorites(user) ?? new List<Contact>()).ToList();
            Contacts = Favorites.Count > 0 ? _repository.ListContacts().Where(dat => Favorites.Count(dat2 => dat2.Id == dat.Id) <= 0).ToList() : _repository.ListContacts();

            if(sort != null) TempData.Add("sort", sort);
        }
    }
}