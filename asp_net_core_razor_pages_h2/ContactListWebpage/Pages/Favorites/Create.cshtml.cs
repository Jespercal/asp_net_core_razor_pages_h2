using ContactListWebpage.DAL;
using ContactListWebpage.Models;
using ContactListWebpage.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactListWebpage.Pages
{
    public class FavoritesCreateModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IMyRepository _repository;
        protected readonly UserManager<IdentityUser> _userManager;


        public InfoType InfoType { get; set; }
        public FavoritesCreateModel(ILogger<FilledPageModel> logger, IMyRepository repository, UserManager<IdentityUser> userManager )
        {
            _logger = logger;
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPost( int contactId )
        {
            if(ModelState.IsValid)
            {
                IdentityUser? user = await _userManager.GetUserAsync(User);
                if(user != null)
                {
                    _repository.CreateFavorite(user, contactId);
                    _repository.SaveChanges();
                }

                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
}