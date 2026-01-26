using Datos_SGBM;
using Entidades_SGBM;

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
        /// <param name="producto">Producto a validar.</param>
        /// <param name="registro">Indica si la validación es para registro (true) o modificación (false).</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si el producto es válido, False en caso contrario.</returns>
        static bool ComprobarProducto(Productos? producto, bool registro, ref string mensaje)
        {
            // Verifica que el objeto producto no sea nulo
            if (producto == null)
            {
                mensaje = "La información del producto no llega a la consulta";
                return false;
            }

            // Validación del código del producto
            if (string.IsNullOrWhiteSpace(producto.CodProducto))
            {
                mensaje = "El Código del producto no llega a la consulta";
                return false;
            }
            else if (producto.CodProducto.Length > 99)
            {
                mensaje = "El Código del producto supera la longitud máxima permitida";
                return false;
            }

            // Validación de la descripción
            if (string.IsNullOrWhiteSpace(producto.Descripcion))
            {
                mensaje = "La descripción del producto no llega a la consulta";
                return false;
            }
            else if (producto.Descripcion.Length > 99)
            {
                mensaje = "La descripción del producto supera la longitud máxima permitida";
                return false;
            }

            // Validación de precios
            if (producto.PrecioVenta < 0)
            {
                mensaje = $"El valor ${producto.PrecioVenta.ToString("0.00")} como precio de venta no está permitido";
                return false;
            }
            if (producto.Costo < 0)
            {
                mensaje = $"El valor ${producto.Costo.ToString("0.00")} como costo del producto no está permitido";
                return false;
            }

            // Si es registro, no se valida el Id
            if (registro)
                return true;

            // Validación del Id en caso de modificación
            if (producto.IdProducto == null || producto.IdProducto < 1)
            {
                mensaje = "Error con el Id del producto a manipular";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Prepara el producto para ser persistido en la base de datos.
        /// Convierte las relaciones en claves foráneas y normaliza valores.
        /// </summary>
        /// <param name="producto">Producto a preparar.</param>
        /// <returns>Producto listo para ser registrado o modificado.</returns>
        static Productos prepararProducto(Productos producto)
        {
            // Si existe categoría, asigna su Id y elimina la referencia
            if (producto.Categorias != null)
                producto.idCategoria = producto.Categorias.IdCategoria;
            if (producto.idCategoria != null)
                producto.Categorias = null;

            // Si existe unidad de medida, asigna su Id y elimina la referencia
            if (producto.UnidadesMedidas != null)
                producto.IdUnidadMedida = producto.UnidadesMedidas.IdUnidadMedida;
            if (producto.IdUnidadMedida != null)
                producto.UnidadesMedidas = null;

            // Si existe proveedor, asigna su Id y elimina la referencia
            if (producto.Proveedores != null)
                producto.IdProveedor = producto.Proveedores.IdProveedor;
            if (producto.IdProveedor != null)
                producto.Proveedores = null;

            // Normaliza la comisión: si es mayor a 1, se interpreta como porcentaje
            if (producto.Comision != null)
                producto.Comision = producto.Comision > 1 ? producto.Comision / 100 : producto.Comision;

            return producto;
        }

        /// <summary>
        /// Obtiene la lista simple de productos desde la capa de datos.
        /// </summary>
        public static List<Productos>? ListaSimple(ref string mensaje)
        {
            try
            {
                List<Productos>? lista = ProductosDatos.ListarSimple(ref mensaje);
                return lista;
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la búsqueda de Productos:\n{ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Sugiere un nuevo código de producto basado en el mayor existente.
        /// </summary>
        public static string SugerirCodigo(ref string mensaje)
        {
            string codigo = "1000000"; // Valor inicial por defecto
            try
            {
                string? mayor = ProductosDatos.ObtenerCodigoMayorSugerido(ref mensaje);

                // Si no hay código mayor, se devuelve el inicial
                if (string.IsNullOrWhiteSpace(mayor))
                    return codigo;

                int codNumerico = 0;

                // Intenta convertir el código mayor a número
                if (!int.TryParse(mayor, out codNumerico))
                {
                    mensaje += "Error al convertir código a número";
                    return codigo;
                }

                // Incrementa el código numérico y lo devuelve
                codNumerico++;
                return codNumerico.ToString();
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la búsqueda de Productos:\n{ex.Message}";
                return "1000000";
            }
        }

        /// <summary>
        /// Busca un producto por su código.
        /// </summary>
        public static Productos? BuscarPorCodigo(string codigo, ref string mensaje)
        {
            // Validación de entrada
            if (string.IsNullOrWhiteSpace(codigo))
            {
                mensaje = "El código llegó vacío a la capa negocio";
                return null;
            }

            try
            {
                Productos? producto = ProductosDatos.ObtenerProductoPorCodigo(codigo, ref mensaje);
                return producto;
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la consulta de disponibilidad del código:\n{ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Lista todas las unidades de medida disponibles.
        /// </summary>
        public static List<UnidadesMedidas>? ListarUnidadesMedida(ref string mensaje)
        {
            try
            {
                List<UnidadesMedidas>? lista = ProductosDatos.ListarUnidadesMedida(ref mensaje);
                return lista;
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la búsqueda de Unidades de Medida:\n{ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Cambia el estado (activo/inactivo) de un producto.
        /// </summary>
        public static bool CambiarEstadoProducto(Productos? producto, ref string mensaje)
        {
            // Valida el producto antes de modificar
            if (!ComprobarProducto(producto, false, ref mensaje))
                return false;

            try
            {
                // Invierte el estado actual
                producto.Activo = !producto.Activo;
                return ProductosDatos.CambiarEstadoProducto(producto, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la modificación del Productos:\n{ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Registra un nuevo producto en la base de datos.
        /// </summary>
        public static int RegistrarProducto(Productos? producto, ref string mensaje)
        {
            // Valida el producto antes de registrar
            if (!ComprobarProducto(producto, true, ref mensaje))
                return 0;

            try
            {
                int id = 0;
                // Prepara el producto para persistencia
                Productos p = prepararProducto(producto);
                p.IdProducto = null; // Se asegura que EF genere el Id

                id = ProductosDatos.RegistrarProducto(p, ref mensaje);
                return id;
            }
            catch (Exception ex)
            {
                mensaje = $"Error en el registro del Productos:\n{ex.Message}";
                return 0;
            }
        }

        /// <summary>
        /// Modifica un producto existente en la base de datos.
        /// </summary>
        public static bool ModificarProducto(Productos? producto, ref string mensaje)
        {
            // Valida el producto antes de modificar
            if (!ComprobarProducto(producto, false, ref mensaje))
                return false;

            try
            {
                // Prepara el producto para persistencia
                Productos p = prepararProducto(producto);
                p.IdProducto = producto.IdProducto; // Mantiene el Id original

                return ProductosDatos.ModificarProducto(p, ref mensaje);
            }
            catch (Exception ex)
            {
                mensaje = $"Error en la modificación del Productos:\n{ex.Message}";
                return false;
            }
        }
    }
}
