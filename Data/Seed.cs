using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portal_Inmobiliario.Models;

namespace Portal_Inmobiliario.Data
{
    public static class Seed
    {
        public static async Task RunAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Aplica migraciones si faltan
            await db.Database.MigrateAsync();

            // Semilla mínima de inmuebles (3–4 activos)
            if (!await db.Inmuebles.AnyAsync())
            {
                db.Inmuebles.AddRange(
                    new Inmueble{
                        Codigo="DEP-001", Titulo="Departamento céntrico 2D",
                        Tipo=TipoInmueble.Departamento, Ciudad="Bogotá",
                        Direccion="Calle 10 #123", Dormitorios=2, Banos=2,
                        MetrosCuadrados=58, Precio=255000000, Activo=true,
                        Imagen="/img/depa1.jpg"
                    },
                    new Inmueble{
                        Codigo="CAS-002", Titulo="Casa familiar con patio",
                        Tipo=TipoInmueble.Casa, Ciudad="Jamundí",
                        Direccion="Mz A Lt 5", Dormitorios=3, Banos=3,
                        MetrosCuadrados=100, Precio=350000000, Activo=true
                    },
                    new Inmueble{
                        Codigo="OFC-003", Titulo="Oficina minimalista",
                        Tipo=TipoInmueble.Oficina, Ciudad="Rionegro",
                        Direccion="Av. Centro 456", Dormitorios=0, Banos=2,
                        MetrosCuadrados=54, Precio=190000000, Activo=true
                    },
                    new Inmueble{
                        Codigo="LOC-004", Titulo="Local en esquina",
                        Tipo=TipoInmueble.Local, Ciudad="Barranquilla",
                        Direccion="Jr. Comercio 789", Dormitorios=0, Banos=1,
                        MetrosCuadrados=80, Precio=321100000, Activo=true
                    }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
