using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace EF_SGBM
{
    public class Contexto : DbContext
    {
        public DbSet<Cajas> Cajas { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<Estados> Estados { get; set; }
        public DbSet<Provincias> Provincias { get; set; }
        public DbSet<Localidades> Localidades { get; set; }
        public DbSet<Proveedores> Proveedores { get; set; }
        public DbSet<Domicilios> Domicilios { get; set; }
        public DbSet<Personas> Personas { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Contactos> Contactos { get; set; }
        public DbSet<Niveles> Niveles { get; set; }
        public DbSet<Credenciales> Credenciales { get; set; }
        public DbSet<Empleados> Empleados { get; set; }
        public DbSet<MediosPagos> MediosPagos { get; set; }
        public DbSet<TiposTransacciones> TiposTransacciones { get; set; }
        public DbSet<UnidadesMedidas> UnidadesMedidas { get; set; }
        public DbSet<Servicios> Servicios { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Transacciones> Transacciones { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Ventas> Ventas { get; set; }
        public DbSet<DetallesVentas> DetallesVentas { get; set; }
        public DbSet<Facturas> Facturas { get; set; }
        public DbSet<DetallesFacturas> DetallesFacturas { get; set; }
        public DbSet<CostosServicios> CostosServicios { get; set; }
        public DbSet<TiposCajas> TiposCajas { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 1. Intentamos leer del App.config (Esto funciona cuando el programa corre de verdad)
            var conexionConfig = ConfigurationManager.ConnectionStrings["Default"];

            // 2. Definimos la cadena. Si conexionConfig es null (como pasa en el Add-Migration),
            // usamos la cadena hardcodeada directamente como respaldo.
            string connString = conexionConfig != null
                ? conexionConfig.ConnectionString
                : "server=localhost;Integrated security=yes;Database=BarberiaMartinez;TrustServerCertificate=true;";

            // 3. Conectamos
            optionsBuilder.UseSqlServer(connString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. SOLUCIÓN GLOBAL DE CASCADAS (Mantenemos esto que ya funcionaba)
            var cascadas = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadas)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // 2. SEEDING (Datos Estáticos Iniciales)

            // --- TIPOS DE CAJAS ---
            modelBuilder.Entity<TiposCajas>().HasData(
                new TiposCajas { IdTipo = 1, Tipo = "Productos" },
                new TiposCajas { IdTipo = 2, Tipo = "Servicios" }
            );

            // --- ESTADOS ---
            modelBuilder.Entity<Estados>().HasData(
                new Estados { IdEstado = 1, Indole = "Empleados", Estado = "Activo" },
                new Estados { IdEstado = 2, Indole = "Empleados", Estado = "Inactivo" },
                new Estados { IdEstado = 3, Indole = "Empleados", Estado = "Bloqueado" },
                new Estados { IdEstado = 4, Indole = "Empleados", Estado = "Desvinculado" },
                new Estados { IdEstado = 5, Indole = "Ventas", Estado = "Finalizada" },
                new Estados { IdEstado = 6, Indole = "Ventas", Estado = "Anulada" },
                new Estados { IdEstado = 7, Indole = "Ventas", Estado = "Facturada" }
            );

            // --- UNIDADES DE MEDIDA ---
            modelBuilder.Entity<UnidadesMedidas>().HasData(
                new UnidadesMedidas { IdUnidadMedida = 1, Unidad = "gr", Descripcion = "Gramos" },
                new UnidadesMedidas { IdUnidadMedida = 2, Unidad = "ml", Descripcion = "Mililitros" },
                new UnidadesMedidas { IdUnidadMedida = 3, Unidad = "cc", Descripcion = "Centímetros Cúbicos" },
                new UnidadesMedidas { IdUnidadMedida = 4, Unidad = "lt", Descripcion = "Litros" }
            );

            // --- NIVELES (Roles) ---
            modelBuilder.Entity<Niveles>().HasData(
                new Niveles { IdNivel = 1, Nivel = "Admin" },
                new Niveles { IdNivel = 2, Nivel = "Propietario" },
                new Niveles { IdNivel = 3, Nivel = "Barbero Senior" },
                new Niveles { IdNivel = 4, Nivel = "Barbero Junior" },
                new Niveles { IdNivel = 5, Nivel = "Administrativo" }
            );

            // --- PROVINCIAS ---
            // Forzamos Entre Ríos al ID 1 para asegurarnos.
            modelBuilder.Entity<Provincias>().HasData(
                new Provincias { IdProvincia = 1, Provincia = "Entre Ríos" },
                new Provincias { IdProvincia = 2, Provincia = "Buenos Aires" },
                new Provincias { IdProvincia = 3, Provincia = "Catamarca" },
                new Provincias { IdProvincia = 4, Provincia = "Chaco" },
                new Provincias { IdProvincia = 5, Provincia = "Chubut" },
                new Provincias { IdProvincia = 6, Provincia = "Córdoba" },
                new Provincias { IdProvincia = 7, Provincia = "Corrientes" },
                new Provincias { IdProvincia = 8, Provincia = "Formosa" },
                new Provincias { IdProvincia = 9, Provincia = "Jujuy" },
                new Provincias { IdProvincia = 10, Provincia = "La Pampa" },
                new Provincias { IdProvincia = 11, Provincia = "La Rioja" },
                new Provincias { IdProvincia = 12, Provincia = "Mendoza" },
                new Provincias { IdProvincia = 13, Provincia = "Misiones" },
                new Provincias { IdProvincia = 14, Provincia = "Neuquén" },
                new Provincias { IdProvincia = 15, Provincia = "Río Negro" },
                new Provincias { IdProvincia = 16, Provincia = "Salta" },
                new Provincias { IdProvincia = 17, Provincia = "San Juan" },
                new Provincias { IdProvincia = 18, Provincia = "San Luis" },
                new Provincias { IdProvincia = 19, Provincia = "Santa Cruz" },
                new Provincias { IdProvincia = 20, Provincia = "Santa Fe" },
                new Provincias { IdProvincia = 21, Provincia = "Santiago del Estero" },
                new Provincias { IdProvincia = 22, Provincia = "Tierra del Fuego" },
                new Provincias { IdProvincia = 23, Provincia = "Tucumán" }
            );

            // --- LOCALIDADES ---
            // Aquí ocurre la magia: Usamos IdProvincia = 1 (que sabemos que es Entre Ríos)
            modelBuilder.Entity<Localidades>().HasData(
                new Localidades { IdLocalidad = 1, Localidad = "Gualeguaychú", IdProvincia = 1, CodPostal = "2820" }
            );

            //-- TIPOS DE TRANSACCIONES --
            modelBuilder.Entity<TiposTransacciones>().HasData(
                new TiposTransacciones { IdTipoTransaccion = 1, Tipo = "Venta Productos" },
                new TiposTransacciones { IdTipoTransaccion = 2, Tipo = "Venta Servicios" },
                new TiposTransacciones { IdTipoTransaccion = 3, Tipo = "Devolución Productos" },
                new TiposTransacciones { IdTipoTransaccion = 4, Tipo = "Devolución Servicios" },
                new TiposTransacciones { IdTipoTransaccion = 5, Tipo = "Compra" },
                new TiposTransacciones { IdTipoTransaccion = 6, Tipo = "Pago Comisión" },
                new TiposTransacciones { IdTipoTransaccion = 7, Tipo = "Pago Costos" },
                new TiposTransacciones { IdTipoTransaccion = 8, Tipo = "Pago Otros" },
                new TiposTransacciones { IdTipoTransaccion = 9, Tipo = "Retiro Propietario" },
                new TiposTransacciones { IdTipoTransaccion = 10, Tipo = "Apertura de Caja" }
            );

            //-- MEDIOS DE PAGO --
            modelBuilder.Entity<MediosPagos>().HasData(
                new MediosPagos { IdMedioPago = 1, Medio = "Efectivo", Observaciones = "Contado efectivo"},
                new MediosPagos { IdMedioPago = 2, Medio = "Mercado Pago (Transferencia)", Observaciones = "Transferencia a la cuenta de mercado pago" },
                new MediosPagos { IdMedioPago = 3, Medio = "Mercado Pago (QR)", Observaciones = "Pago con código Qr a mercado pago" }, 
                new MediosPagos { IdMedioPago = 4, Medio = "Tarjeta de Débito", Observaciones = "Pago con Tarjeta de débito" },
                new MediosPagos { IdMedioPago = 5, Medio = "Tarjeta de Crédito", Observaciones = "Pago con Tarjeta de crédito" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
