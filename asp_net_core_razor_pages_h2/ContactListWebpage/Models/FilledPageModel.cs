using ContactListWebpage.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactListWebpage.Models
{
    public class FilledPageModel : PageModel
    {
        protected readonly ILogger<FilledPageModel> _logger;
        protected readonly IDataHandler _dataHandler;

        public FilledPageModel(ILogger<FilledPageModel> logger, IDataHandler dataHandler )
        {
            _logger = logger;
            _dataHandler = dataHandler;
        }
    }
}
