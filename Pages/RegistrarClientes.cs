using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CFAGestionClientes.Pages;

public class RegistrarClientesModel : PageModel
{
    private readonly ILogger<RegistrarClientesModel> _logger;

    public RegistrarClientesModel(ILogger<RegistrarClientesModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}

