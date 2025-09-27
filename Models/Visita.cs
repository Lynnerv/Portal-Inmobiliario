using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Portal_Inmobiliario.Models
{
    public class Visita
    {
        public int Id { get; set; }

        [Required]
        public int InmuebleId { get; set; }
        public Inmueble? Inmueble { get; set; }

        [Required]
        public string UsuarioId { get; set; } = default!;
        public IdentityUser? Usuario { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaFin { get; set; }

        public EstadoVisita Estado { get; set; } = EstadoVisita.Solicitada;

        [StringLength(500)]
        public string? Notas { get; set; }
    }
}