﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Web.HttpUtility;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }
    public int pageNr { get; set; }
    public int pages { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        pages = _service.getPagesHome(false, null);
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        Cheeps = _service.GetCheeps(pageNr);
        return Page();

    }
}