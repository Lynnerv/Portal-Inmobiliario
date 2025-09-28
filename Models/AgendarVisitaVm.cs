using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Portal_Inmobiliario.Models
{
    public class AgendarVisitaVm
    {
        [Required] public int InmuebleId { get; set; }
        [Display(Name="Desde"), DataType(DataType.DateTime)]
        public DateTime FechaInicio { get; set; }
        [Display(Name="Hasta"), DataType(DataType.DateTime)]
        public DateTime FechaFin { get; set; }
        [StringLength(500)] public string? Notas { get; set; }
    }
}