using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EF_SGBM.Migrations
{
    /// <inheritdoc />
    public partial class InicialSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Indole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.IdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Indole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.IdEstado);
                });

            migrationBuilder.CreateTable(
                name: "MediosPagos",
                columns: table => new
                {
                    IdMedioPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Medio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediosPagos", x => x.IdMedioPago);
                });

            migrationBuilder.CreateTable(
                name: "Niveles",
                columns: table => new
                {
                    IdNivel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nivel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Niveles", x => x.IdNivel);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    IdProveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cuit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RazonSocial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.IdProveedor);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    IdProvincia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Provincia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.IdProvincia);
                });

            migrationBuilder.CreateTable(
                name: "TiposCajas",
                columns: table => new
                {
                    IdTipo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCajas", x => x.IdTipo);
                });

            migrationBuilder.CreateTable(
                name: "TiposTransacciones",
                columns: table => new
                {
                    IdTipoTransaccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTransacciones", x => x.IdTipoTransaccion);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedidas",
                columns: table => new
                {
                    IdUnidadMedida = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unidad = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedidas", x => x.IdUnidadMedida);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    IdServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreServicio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PrecioVenta = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Costos = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Margen = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Comision = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: false),
                    Puntaje = table.Column<int>(type: "int", nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.IdServicio);
                    table.ForeignKey(
                        name: "FK_Servicios_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Credenciales",
                columns: table => new
                {
                    IdCredencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Accesos = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IdNivel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credenciales", x => x.IdCredencial);
                    table.ForeignKey(
                        name: "FK_Credenciales_Niveles_IdNivel",
                        column: x => x.IdNivel,
                        principalTable: "Niveles",
                        principalColumn: "IdNivel",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Localidades",
                columns: table => new
                {
                    IdLocalidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Localidad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CodPostal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IdProvincia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidades", x => x.IdLocalidad);
                    table.ForeignKey(
                        name: "FK_Localidades_Provincias_IdProvincia",
                        column: x => x.IdProvincia,
                        principalTable: "Provincias",
                        principalColumn: "IdProvincia",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cajas",
                columns: table => new
                {
                    IdCaja = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Abierta = table.Column<bool>(type: "bit", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalEfectivo = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalMP = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    IdTipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cajas", x => x.IdCaja);
                    table.ForeignKey(
                        name: "FK_Cajas_TiposCajas_IdTipo",
                        column: x => x.IdTipo,
                        principalTable: "TiposCajas",
                        principalColumn: "IdTipo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodProducto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    CantidadMedida = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    Medida = table.Column<int>(type: "int", nullable: true),
                    PrecioVenta = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    IdUnidadMedida = table.Column<int>(type: "int", nullable: true),
                    IdProveedor = table.Column<int>(type: "int", nullable: true),
                    idCategoria = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Comision = table.Column<decimal>(type: "decimal(4,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_idCategoria",
                        column: x => x.idCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria");
                    table.ForeignKey(
                        name: "FK_Productos_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "IdProveedor");
                    table.ForeignKey(
                        name: "FK_Productos_UnidadesMedidas_IdUnidadMedida",
                        column: x => x.IdUnidadMedida,
                        principalTable: "UnidadesMedidas",
                        principalColumn: "IdUnidadMedida");
                });

            migrationBuilder.CreateTable(
                name: "Domicilios",
                columns: table => new
                {
                    IdDomicilio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Calle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Altura = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Piso = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Depto = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Barrio = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: true),
                    IdLocalidad = table.Column<int>(type: "int", nullable: false),
                    IdProveedor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domicilios", x => x.IdDomicilio);
                    table.ForeignKey(
                        name: "FK_Domicilios_Localidades_IdLocalidad",
                        column: x => x.IdLocalidad,
                        principalTable: "Localidades",
                        principalColumn: "IdLocalidad",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Domicilios_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "IdProveedor");
                });

            migrationBuilder.CreateTable(
                name: "Transacciones",
                columns: table => new
                {
                    IdTransaccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoIngreso = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    MontoEgreso = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    IdTipoTransaccion = table.Column<int>(type: "int", nullable: false),
                    IdCaja = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacciones", x => x.IdTransaccion);
                    table.ForeignKey(
                        name: "FK_Transacciones_Cajas_IdCaja",
                        column: x => x.IdCaja,
                        principalTable: "Cajas",
                        principalColumn: "IdCaja",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacciones_TiposTransacciones_IdTipoTransaccion",
                        column: x => x.IdTipoTransaccion,
                        principalTable: "TiposTransacciones",
                        principalColumn: "IdTipoTransaccion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostosServicios",
                columns: table => new
                {
                    IdCostoServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Costo = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Unidades = table.Column<int>(type: "int", nullable: true),
                    CantidadMedida = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    IdProducto = table.Column<int>(type: "int", nullable: true),
                    IdServicio = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostosServicios", x => x.IdCostoServicio);
                    table.ForeignKey(
                        name: "FK_CostosServicios_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_CostosServicios_Servicios_IdServicio",
                        column: x => x.IdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dni = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaNac = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdDomicilio = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.IdPersona);
                    table.ForeignKey(
                        name: "FK_Personas_Domicilios_IdDomicilio",
                        column: x => x.IdDomicilio,
                        principalTable: "Domicilios",
                        principalColumn: "IdDomicilio");
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPersona = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    esMiembro = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                    table.ForeignKey(
                        name: "FK_Clientes_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estados",
                        principalColumn: "IdEstado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contactos",
                columns: table => new
                {
                    IdContacto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Whatsapp = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Instagram = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Facebook = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ExtranjeroWhatsapp = table.Column<bool>(type: "bit", nullable: false),
                    IdPersona = table.Column<int>(type: "int", nullable: true),
                    IdProveedor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contactos", x => x.IdContacto);
                    table.ForeignKey(
                        name: "FK_Contactos_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona");
                    table.ForeignKey(
                        name: "FK_Contactos_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "IdProveedor");
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    IdEmpleado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoEmpleado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdPersona = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.IdEmpleado);
                    table.ForeignKey(
                        name: "FK_Empleados_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estados",
                        principalColumn: "IdEstado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Empleados_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Imagen = table.Column<string>(type: "nvarchar(230)", maxLength: 230, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    IdNivel = table.Column<int>(type: "int", nullable: false),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empleados_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleados",
                        principalColumn: "IdEmpleado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Niveles_IdNivel",
                        column: x => x.IdNivel,
                        principalTable: "Niveles",
                        principalColumn: "IdNivel",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    IdVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NroVenta = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    FechaVenta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.IdVenta);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ventas_Empleados_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleados",
                        principalColumn: "IdEmpleado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ventas_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estados",
                        principalColumn: "IdEstado",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesVentas",
                columns: table => new
                {
                    IdDetalleVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    InteresDescuento = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    IdProducto = table.Column<int>(type: "int", nullable: true),
                    IdServicio = table.Column<int>(type: "int", nullable: true),
                    IdFondoMembresia = table.Column<int>(type: "int", nullable: true),
                    IdVenta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesVentas", x => x.IdDetalleVenta);
                    table.ForeignKey(
                        name: "FK_DetallesVentas_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_DetallesVentas_Servicios_IdServicio",
                        column: x => x.IdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio");
                    table.ForeignKey(
                        name: "FK_DetallesVentas_Ventas_IdVenta",
                        column: x => x.IdVenta,
                        principalTable: "Ventas",
                        principalColumn: "IdVenta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    IdFactura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NroFactura = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalAbonado = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    IdTransaccion = table.Column<int>(type: "int", nullable: true),
                    IdMedioPago = table.Column<int>(type: "int", nullable: false),
                    IdVenta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.IdFactura);
                    table.ForeignKey(
                        name: "FK_Facturas_MediosPagos_IdMedioPago",
                        column: x => x.IdMedioPago,
                        principalTable: "MediosPagos",
                        principalColumn: "IdMedioPago",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Facturas_Transacciones_IdTransaccion",
                        column: x => x.IdTransaccion,
                        principalTable: "Transacciones",
                        principalColumn: "IdTransaccion");
                    table.ForeignKey(
                        name: "FK_Facturas_Ventas_IdVenta",
                        column: x => x.IdVenta,
                        principalTable: "Ventas",
                        principalColumn: "IdVenta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesFacturas",
                columns: table => new
                {
                    IdDetalleFactura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDetalleVenta = table.Column<int>(type: "int", nullable: false),
                    IdFactura = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesFacturas", x => x.IdDetalleFactura);
                    table.ForeignKey(
                        name: "FK_DetallesFacturas_DetallesVentas_IdDetalleVenta",
                        column: x => x.IdDetalleVenta,
                        principalTable: "DetallesVentas",
                        principalColumn: "IdDetalleVenta",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesFacturas_Facturas_IdFactura",
                        column: x => x.IdFactura,
                        principalTable: "Facturas",
                        principalColumn: "IdFactura",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Estados",
                columns: new[] { "IdEstado", "Estado", "Indole" },
                values: new object[,]
                {
                    { 1, "Activo", "Empleados" },
                    { 2, "Inactivo", "Empleados" },
                    { 3, "Bloqueado", "Empleados" },
                    { 4, "Desvinculado", "Empleados" },
                    { 5, "Activo", "Clientes" },
                    { 6, "Inactivo", "Clientes" },
                    { 7, "Activo", "Servicios" },
                    { 8, "Inactivo", "Servicios" },
                    { 9, "En Curso", "Ventas" },
                    { 10, "Finalizada", "Ventas" },
                    { 11, "Anulada", "Ventas" }
                });

            migrationBuilder.InsertData(
                table: "Niveles",
                columns: new[] { "IdNivel", "Nivel" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Propietario" },
                    { 3, "Responsable" },
                    { 4, "Barbero" },
                    { 5, "Invitado" }
                });

            migrationBuilder.InsertData(
                table: "Provincias",
                columns: new[] { "IdProvincia", "Provincia" },
                values: new object[,]
                {
                    { 1, "Entre Ríos" },
                    { 2, "Buenos Aires" },
                    { 3, "Catamarca" },
                    { 4, "Chaco" },
                    { 5, "Chubut" },
                    { 6, "Córdoba" },
                    { 7, "Corrientes" },
                    { 8, "Formosa" },
                    { 9, "Jujuy" },
                    { 10, "La Pampa" },
                    { 11, "La Rioja" },
                    { 12, "Mendoza" },
                    { 13, "Misiones" },
                    { 14, "Neuquén" },
                    { 15, "Río Negro" },
                    { 16, "Salta" },
                    { 17, "San Juan" },
                    { 18, "San Luis" },
                    { 19, "Santa Cruz" },
                    { 20, "Santa Fe" },
                    { 21, "Santiago del Estero" },
                    { 22, "Tierra del Fuego" },
                    { 23, "Tucumán" }
                });

            migrationBuilder.InsertData(
                table: "TiposCajas",
                columns: new[] { "IdTipo", "Tipo" },
                values: new object[,]
                {
                    { 1, "Productos" },
                    { 2, "Servicios" }
                });

            migrationBuilder.InsertData(
                table: "UnidadesMedidas",
                columns: new[] { "IdUnidadMedida", "Descripcion", "Unidad" },
                values: new object[,]
                {
                    { 1, "Gramos", "gr" },
                    { 2, "Mililitros", "ml" },
                    { 3, "Centímetros Cúbicos", "cc" },
                    { 4, "Litros", "lt" }
                });

            migrationBuilder.InsertData(
                table: "Localidades",
                columns: new[] { "IdLocalidad", "CodPostal", "IdProvincia", "Localidad" },
                values: new object[] { 1, "2820", 1, "Gualeguaychú" });

            migrationBuilder.CreateIndex(
                name: "IX_Cajas_IdTipo",
                table: "Cajas",
                column: "IdTipo");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdEstado",
                table: "Clientes",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdPersona",
                table: "Clientes",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_IdPersona",
                table: "Contactos",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_IdProveedor",
                table: "Contactos",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_CostosServicios_IdProducto",
                table: "CostosServicios",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_CostosServicios_IdServicio",
                table: "CostosServicios",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Credenciales_IdNivel",
                table: "Credenciales",
                column: "IdNivel");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesFacturas_IdDetalleVenta",
                table: "DetallesFacturas",
                column: "IdDetalleVenta");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesFacturas_IdFactura",
                table: "DetallesFacturas",
                column: "IdFactura");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVentas_IdProducto",
                table: "DetallesVentas",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVentas_IdServicio",
                table: "DetallesVentas",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVentas_IdVenta",
                table: "DetallesVentas",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_Domicilios_IdLocalidad",
                table: "Domicilios",
                column: "IdLocalidad");

            migrationBuilder.CreateIndex(
                name: "IX_Domicilios_IdProveedor",
                table: "Domicilios",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_IdEstado",
                table: "Empleados",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_IdPersona",
                table: "Empleados",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdMedioPago",
                table: "Facturas",
                column: "IdMedioPago");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdTransaccion",
                table: "Facturas",
                column: "IdTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdVenta",
                table: "Facturas",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_Localidades_IdProvincia",
                table: "Localidades",
                column: "IdProvincia");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_IdDomicilio",
                table: "Personas",
                column: "IdDomicilio");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_idCategoria",
                table: "Productos",
                column: "idCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdProveedor",
                table: "Productos",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdUnidadMedida",
                table: "Productos",
                column: "IdUnidadMedida");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_IdCategoria",
                table: "Servicios",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_IdCaja",
                table: "Transacciones",
                column: "IdCaja");

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_IdTipoTransaccion",
                table: "Transacciones",
                column: "IdTipoTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdEmpleado",
                table: "Usuarios",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdNivel",
                table: "Usuarios",
                column: "IdNivel");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdCliente",
                table: "Ventas",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdEmpleado",
                table: "Ventas",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdEstado",
                table: "Ventas",
                column: "IdEstado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contactos");

            migrationBuilder.DropTable(
                name: "CostosServicios");

            migrationBuilder.DropTable(
                name: "Credenciales");

            migrationBuilder.DropTable(
                name: "DetallesFacturas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "DetallesVentas");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "Niveles");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "MediosPagos");

            migrationBuilder.DropTable(
                name: "Transacciones");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "UnidadesMedidas");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Cajas");

            migrationBuilder.DropTable(
                name: "TiposTransacciones");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "TiposCajas");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "Domicilios");

            migrationBuilder.DropTable(
                name: "Localidades");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Provincias");
        }
    }
}
