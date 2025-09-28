using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portal_Inmobiliario.Data;
using Portal_Inmobiliario.Services;

namespace Portal_Inmobiliario.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IAgendaService _agenda;
        private readonly UserManager<IdentityUser> _um;

        public ReservasController(ApplicationDbContext db, IAgendaService agenda, UserManager<IdentityUser> um)
        {
            _db = db;
            _agenda = agenda;
            _um = um;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int inmuebleId)
        {
            if (await _agenda.TieneReservaActiva(inmuebleId))
            {
                TempData["Error"] = "Este inmueble ya tiene una reserva activa.";
                return RedirectToAction("Detalle", "Catalogo", new { id = inmuebleId });
            }

            var user = await _um.GetUserAsync(User);
            var now = DateTime.UtcNow;

            _db.Reservas.Add(new Models.Reserva
            {
                InmuebleId = inmuebleId,
                UsuarioId = user!.Id,
                FechaCreacion = now,
                FechaExpiracion = now.AddHours(48)
            });

            await _db.SaveChangesAsync();
            TempData["Ok"] = "Reserva creada por 48 horas.";
            return RedirectToAction("Detalle", "Catalogo", new { id = inmuebleId });
        }
    }
}
