using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactListWebpage.Pages
{
    public class FavoritesIndexModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;
        protected readonly UserManager<IdentityUser> _userManager;
        public List<Contact> Favorites { get; set; }
        public List<InfoType> InfoTypes { get; set; }
        public int ShowInfoType { get; set; }
        public FavoritesIndexModel(ILogger<FilledPageModel> logger, IMyRepository repository, UserManager<IdentityUser> userManager )
        {
            _logger = logger;
            _repository = repository;
            _userManager = userManager;
        }

        public async Task OnGet( int? infotype)
        {
            IdentityUser? user = await _userManager.GetUserAsync(User);
            Favorites = (_repository.ListFavorites(user) ?? new List<Contact>()).ToList();

            InfoTypes = _repository.ListInfoTypes();

            ShowInfoType = infotype != null ? infotype.Value : (InfoTypes.Count > 0 ? InfoTypes[0].Id : 0);
        }
    }
}