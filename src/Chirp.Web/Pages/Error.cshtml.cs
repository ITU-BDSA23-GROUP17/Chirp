using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages
{
    public class ErrorPageModel : PageModel
    {
         public IActionResult OnGet()
        {
            return Page();
        }
    }
}