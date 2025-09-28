using Microsoft.EntityFrameworkCore;
using Portal_Inmobiliario.Data;
using Portal_Inmobiliario.Models;

namespace Portal_Inmobiliario.Services
{
    public interface IAgendaService
    {
        Task<bool> HaySolapeVisita(int inmuebleId, DateTime ini, DateTime fin);
        Task<bool> TieneReservaActiva(int inmuebleId);
    }

    public class AgendaService : IAgendaService
    {
        private readonly ApplicationDbContext _db;
        public AgendaService(ApplicationDbContext db) => _db = db;

        // Devuelve true si existe una visita (no cancelada) que se solapa con [ini, fin]
        public Task<bool> HaySolapeVisita(int inmuebleId, DateTime ini, DateTime fin)
        {
            return _db.Visitas
                .Where(v => v.InmuebleId == inmuebleId && v.Estado != EstadoVisita.Cancelada)
                .AnyAsync(v => ini < v.FechaFin && fin > v.FechaInicio);
        }

        // Devuelve true si hay una reserva activa (ahora < FechaExpiracion)
        public Task<bool> TieneReservaActiva(int inmuebleId)
        {
            var now = DateTime.UtcNow;
            return _db.Reservas.AnyAsync(r => r.InmuebleId == inmuebleId && now < r.FechaExpiracion);
        }
    }
}
