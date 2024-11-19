using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Servicios()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ProcederPago(string service)
    {
        if (string.IsNullOrEmpty(service))
        {
            ViewBag.ErrorMessage = "Debes seleccionar un servicio antes de proceder con el pago.";
            return View("Servicios");
        }

        TempData["SelectedService"] = service;
        return RedirectToAction("Pago");
    }

    [HttpGet]
    public IActionResult Pago()
    {
        if (TempData["SelectedService"] != null)
        {
            ViewBag.SelectedService = TempData["SelectedService"].ToString();
        }
        else
        {
            return RedirectToAction("Servicios");
        }

        return View();
    }
    public async Task<IActionResult> CerrarSesion()
    {
        //Borrar la autenticacion con SignOutAsync.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("IniciarSesion", "Inicio");
    }
}
