using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace ContactListWebpage.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorsModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        public ErrorsModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet( string error )
        {
            if(error == "404")
            {
                TempData["text"] = "The page you are looking for doesn't exist...";
                TempData["text2"] = "Please return to the main menu.";
                return Page();
            }
            else if(error == "500")
            {
                TempData["text"] = "Sorry about that, something went wrong on our end...";
                TempData["text2"] = "Please return to the main menu.";
                return Page();
            }
            return Page();
        }
    }
}