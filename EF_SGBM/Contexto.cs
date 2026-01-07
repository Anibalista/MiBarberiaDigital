using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DbSet<FondosMembresias> FondosMembresias { get; set; }
        public DbSet<TiposMembresias> TiposMembresias { get; set; }
        public DbSet<TiposTransacciones> TiposTransacciones { get; set; }
        public DbSet<UnidadesMedidas> UnidadesMedidas { get; set; }
        public DbSet<Servicios> Servicios { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Transacciones> Transacciones { get; set; }
        public DbSet<CuotasMembresias> CuotasMembresias { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Insumos> Insumos { get; set; }
        public DbSet<Ventas> Ventas { get; set; }
        public DbSet<DetallesVentas> DetallesVentas { get; set; }
        public DbSet<Facturas> Facturas { get; set; }
        public DbSet<DetallesFacturas> DetallesFacturas { get; set; }
        public DbSet<CostosServicios> ServiciosInsumos { get; set; }
        public DbSet<Membresias> Membresias { get; set; }
        public DbSet<MembresiasServicios> MembresiasServicios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            optionsBuilder.UseSqlServer(connString);
        }
    }
}
