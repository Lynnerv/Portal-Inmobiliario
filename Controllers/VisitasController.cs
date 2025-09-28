using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portal_Inmobiliario.Data;
using Portal_Inmobiliario.Models;
using Portal_Inmobiliario.Services;

namespace Portal_Inmobiliario.Controllers
{
    [Authorize]
    public class VisitasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IAgendaService _agenda;
        private readonly UserManager<IdentityUser> _um;

        public VisitasController(ApplicationDbContext db, IAgendaService agenda, UserManager<IdentityUser> um)
        {
            _db = db;
            _agenda = agenda;
            _um = um;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agendar(AgendarVisitaVm vm)
        {
            if (vm.FechaInicio >= vm.FechaFin)
                ModelState.AddModelError("", "La fecha de inicio debe ser menor a la fecha fin.");

            var abre = new TimeSpan(8, 0, 0);
            var cierra = new TimeSpan(19, 0, 0);
            if (vm.FechaInicio.TimeOfDay < abre || vm.FechaFin.TimeOfDay > cierra)
                ModelState.AddModelError("", "Las visitas son entre 08:00 y 19:00.");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction("Detalle", "Catalogo", new { id = vm.InmuebleId });
            }

            if (await _agenda.HaySolapeVisita(vm.InmuebleId, vm.FechaInicio, vm.FechaFin))
            {
                TempData["Error"] = "Ya existe una visita en ese intervalo.";
                return RedirectToAction("Detalle", "Catalogo", new { id = vm.InmuebleId });
            }

            var user = await _um.GetUserAsync(User);
            _db.Visitas.Add(new Visita
            {
                InmuebleId = vm.InmuebleId,
                UsuarioId = user!.Id,
                FechaInicio = vm.FechaInicio,
                FechaFin = vm.FechaFin,
                Notas = vm.Notas,
                Estado = EstadoVisita.Solicitada
            });

            await _db.SaveChangesAsync();
            TempData["Ok"] = "Visita solicitada. Un broker la confirmará.";
            return RedirectToAction("Detalle", "Catalogo", new { id = vm.InmuebleId });
        }
    }
}
