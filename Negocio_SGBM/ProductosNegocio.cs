using Datos_SGBM;
using Entidades_SGBM;
using Utilidades;

namespace Negocio_SGBM
{
    /// <summary>
    /// Capa de negocio para la gestión de productos.
    /// Contiene validaciones y preparación de datos antes de acceder a la capa de datos.
    /// </summary>
    public class ProductosNegocio
    {
        /// <summary>
        /// Valida que la información del producto sea correcta antes de procesarla.
        /// </summary>
        private static Resultado<bool> ComprobarProducto(Productos? producto, bool registro)
        {
            if (producto == null)
                return Resultado<bool>.Fail("La información del producto no llega a la consulta.");

            if (string.IsNullOrWhiteSpace(producto.CodProducto))
                return Resultado<bool>.Fail("El código del producto no puede estar vacío.");
            if (producto.CodProducto.Length > 99)
                return Resultado<bool>.Fail("El código del producto supera la longitud máxima permitida.");

            if (string.IsNullOrWhiteSpace(producto.Descripcion))
                return Resultado<bool>.Fail("La descripción del producto no puede estar vacía.");
            if (producto.Descripcion.Length > 99)
                return Resultado<bool>.Fail("La descripción del producto supera la longitud máxima permitida.");

            if (producto.PrecioVenta < 0)
                return Resultado<bool>.Fail($"El precio de venta {producto.PrecioVenta:0.00} no está permitido.");
            if (producto.Costo < 0)
                return Resultado<bool>.Fail($"El costo {producto.Costo:0.00} no está permitido.");

            if (!registro && (producto.IdProducto == null || producto.IdProducto < 1))
                return Resultado<bool>.Fail("Error con el Id del producto a manipular.");

            return Resultado<bool>.Ok(true);
        }

        /// <summary>
        /// Prepara el producto para ser persistido en la base de datos.
        /// Convierte las relaciones en claves foráneas y normaliza valores.
        /// </summary>
        private static Productos PrepararProducto(Productos producto)
        {
            if (producto.Categorias != null)
                producto.idCategoria = producto.Categorias.IdCategoria;
            if (producto.idCategoria != null)
                producto.Categorias = null;

            if (producto.UnidadesMedidas != null)
                producto.IdUnidadMedida = producto.UnidadesMedidas.IdUnidadMedida;
            if (producto.IdUnidadMedida != null)
                producto.UnidadesMedidas = null;

            if (producto.Proveedores != null)
                producto.IdProveedor = producto.Proveedores.IdProveedor;
            if (producto.IdProveedor != null)
                producto.Proveedores = null;

            if (producto.Comision != null)
                producto.Comision = producto.Comision > 1 ? producto.Comision / 100 : producto.Comision;

            return producto;
        }

        /// <summary>
        /// Obtiene la lista simple de productos desde la capa de datos.
        /// </summary>
        public static Resultado<List<Productos>> ListaSimple()
        {
            try
            {
                return ProductosDatos.ListarSimple();
            }
            catch (Exception ex)
            {
                var msg = $"Error en la búsqueda de productos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Productos>>.Fail(msg);
            }
        }

        /// <summary>
        /// Sugiere un nuevo código de producto basado en el mayor existente.
        /// </summary>
        public static Resultado<string> SugerirCodigo()
        {
            const string codigoInicial = "1000000";

            try
            {
                var resultadoDatos = ProductosDatos.ObtenerCodigoMayorSugerido();
                if (!resultadoDatos.Success)
                    return Resultado<string>.Fail(resultadoDatos.Mensaje);

                var mayor = resultadoDatos.Data;
                if (string.IsNullOrWhiteSpace(mayor))
                    return Resultado<string>.Ok(codigoInicial);

                if (!int.TryParse(mayor, out int codNumerico))
                    return Resultado<string>.Fail("Error al convertir código a número.");

                codNumerico++;
                return Resultado<string>.Ok(codNumerico.ToString());
            }
            catch (Exception ex)
            {
                var msg = $"Error al sugerir código de producto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<string>.Fail(msg);
            }
        }

        /// <summary>
        /// Busca un producto por su código.
        /// </summary>
        public static Resultado<Productos?> BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return Resultado<Productos?>.Fail("El código llegó vacío a la capa negocio.");

            try
            {
                return ProductosDatos.ObtenerProductoPorCodigo(codigo);
            }
            catch (Exception ex)
            {
                var msg = $"Error en la consulta de disponibilidad del código:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Productos?>.Fail(msg);
            }
        }

        /// <summary>
        /// Lista todas las unidades de medida disponibles.
        /// </summary>
        public static Resultado<List<UnidadesMedidas>> ListarUnidadesMedida()
        {
            try
            {
                return ProductosDatos.ListarUnidadesMedida();
            }
            catch (Exception ex)
            {
                var msg = $"Error en la búsqueda de unidades de medida:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<UnidadesMedidas>>.Fail(msg);
            }
        }

        /// <summary>
        /// Cambia el estado (activo/inactivo) de un producto.
        /// </summary>
        public static Resultado<bool> CambiarEstadoProducto(Productos? producto)
        {
            var validacion = ComprobarProducto(producto, false);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            try
            {
                producto!.Activo = !producto.Activo;
                return ProductosDatos.CambiarEstadoProducto(producto);
            }
            catch (Exception ex)
            {
                var msg = $"Error al cambiar estado del producto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo producto en la base de datos.
        /// </summary>
        public static Resultado<int> RegistrarProducto(Productos? producto)
        {
            var validacion = ComprobarProducto(producto, true);
            if (!validacion.Success)
                return Resultado<int>.Fail(validacion.Mensaje);

            try
            {
                var p = PrepararProducto(producto!);
                p.IdProducto = null;

                return ProductosDatos.RegistrarProducto(p);
            }
            catch (Exception ex)
            {
                var msg = $"Error en el registro del producto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un producto existente en la base de datos.
        /// </summary>
        public static Resultado<bool> ModificarProducto(Productos? producto)
        {
            var validacion = ComprobarProducto(producto, false);
            if (!validacion.Success)
                return Resultado<bool>.Fail(validacion.Mensaje);

            try
            {
                var p = PrepararProducto(producto!);
                p.IdProducto = producto.IdProducto;

                return ProductosDatos.ModificarProducto(p);
            }
            catch (Exception ex)
            {
                var msg = $"Error en la modificación del producto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }
    }
}
