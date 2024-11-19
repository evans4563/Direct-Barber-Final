using Microsoft.AspNetCore.Mvc;

namespace BarberiaApp.Controllers
{
    public class ServiciosController : Controller
    {
        public IActionResult Servicios()
        {
            var servicios = new List<Servicio>
            {
                new Servicio { Nombre = "Corte de Cabello", Imagen = "corte.jpg" },
                new Servicio { Nombre = "Afeitado Clásico", Imagen = "afeitado.jpg" },
                new Servicio { Nombre = "Corte de Barba", Imagen = "barba.jpg" },
                new Servicio { Nombre = "Tintura de Cabello", Imagen = "tintura.jpg" },
            };
            return View(servicios);
        }

        [HttpPost]
        public IActionResult ResumenServicios(List<string> serviciosSeleccionados)
        {
            if (serviciosSeleccionados == null || serviciosSeleccionados.Count == 0)
            {
                ViewBag.Message = "No has seleccionado ningún servicio.";
                return View("Index", new List<Servicio>());
            }

            ViewBag.ServiciosSeleccionados = serviciosSeleccionados;
            return View("ResumenServicios");
        }
        public IActionResult Pago()
        {
            return View();
        }
    }

    public class Servicio
    {
        public string Nombre { get; set; }
        public string Imagen { get; set; }
    }
}
