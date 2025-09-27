using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Portal_Inmobiliario.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int InmuebleId { get; set; }
        public Inmueble? Inmueble { get; set; }

        [Required]
        public string UsuarioId { get; set; } = default!;
        public IdentityUser? Usuario { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaExpiracion { get; set; } // (ahora + 48h en la lógica de negocio)
    }
}