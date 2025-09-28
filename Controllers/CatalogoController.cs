using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portal_Inmobiliario.Data;
using Portal_Inmobiliario.Models;

namespace Portal_Inmobiliario.Controllers
{
    // ¡Ojo! Sin [Route] para usar el enrutado MVC convencional: /Catalogo/Index
    public class CatalogoController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CatalogoController(ApplicationDbContext db) => _db = db;

        // /Catalogo?Ciudad=...&Tipo=...&PrecioMin=...&PrecioMax=...&DormitoriosMin=...&Page=1
        public async Task<IActionResult> Index(CatalogoFiltroVm vm)
        {
            // Validaciones server-side
            if (vm.PrecioMin is not null && vm.PrecioMax is not null && vm.PrecioMin > vm.PrecioMax)
                ModelState.AddModelError(string.Empty, "El precio mínimo no puede ser mayor que el máximo.");

            // Combos
            var ciudades = await _db.Inmuebles.Where(i => i.Activo)
                                .Select(i => i.Ciudad).Distinct().OrderBy(x => x).ToListAsync();
            ViewBag.Ciudades = new SelectList(new[] { "" }.Concat(ciudades)); // "" = Todas

            ViewBag.Tipos = Enum.GetValues(typeof(TipoInmueble))
                .Cast<TipoInmueble>()
                .Select(t => new SelectListItem { Value = t.ToString(), Text = t.ToString(), Selected = vm.Tipo == t })
                .ToList();

            // Query base
            var q = _db.Inmuebles.AsNoTracking().Where(i => i.Activo);

            if (!string.IsNullOrWhiteSpace(vm.Ciudad))      q = q.Where(i => i.Ciudad == vm.Ciudad);
            if (vm.Tipo.HasValue)                           q = q.Where(i => i.Tipo == vm.Tipo.Value);
            if (vm.PrecioMin.HasValue)                      q = q.Where(i => i.Precio >= vm.PrecioMin.Value);
            if (vm.PrecioMax.HasValue)                      q = q.Where(i => i.Precio <= vm.PrecioMax.Value);
            if (vm.DormitoriosMin.HasValue)                 q = q.Where(i => i.Dormitorios >= vm.DormitoriosMin.Value);

            vm.Total = await q.CountAsync();
            vm.Page = Math.Max(1, vm.Page);
            var skip = (vm.Page - 1) * vm.PageSize;

            vm.Resultados = await q.OrderBy(i => i.Ciudad).ThenBy(i => i.Precio)
                                   .Skip(skip).Take(vm.PageSize).ToListAsync();

            return View(vm);
        }

        // /Catalogo/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var item = await _db.Inmuebles.AsNoTracking()
                              .FirstOrDefaultAsync(i => i.Id == id && i.Activo);
            if (item is null) return NotFound();
            return View(item);
        }
    }
}