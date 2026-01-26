using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Productos.
    /// Contiene métodos de consulta, registro y modificación de productos y unidades de medida.
    /// </summary>
    public class ProductosDatos
    {
        /// <summary>
        /// Comprueba que el contexto y sus entidades estén disponibles y correctas.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si todas las entidades están disponibles, False en caso contrario.</returns>
        static bool ComprobarContexto(Contexto contexto, ref string mensaje)
        {
            ComprobacionContexto comprobar = new ComprobacionContexto(contexto);

            // Verifica que la entidad UnidadesMedidas esté disponible
            if (!comprobar.ComprobarEntidad(contexto.UnidadesMedidas, ref mensaje))
                return false;

            // Verifica que la entidad Productos esté disponible
            if (!comprobar.ComprobarEntidad(contexto.Productos, ref mensaje))
                return false;

            // Verifica que la entidad Proveedores esté disponible
            return comprobar.ComprobarEntidad(contexto.Proveedores, ref mensaje);
        }

        /// <summary>
        /// Obtiene un listado simple de productos incluyendo categorías y unidades de medida.
        /// </summary>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Lista de productos ordenados por descripción, o null si ocurre un error.</returns>
        public static List<Productos>? ListarSimple(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    // Incluye relaciones con Categorías y UnidadesMedidas
                    return contexto.Productos.Include("Categorias").Include("UnidadesMedidas")
                                   .OrderBy(p => p.Descripcion)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener productos:\n{ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Obtiene el código de producto más alto sugerido, filtrando por longitud.
        /// </summary>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>El código de producto máximo encontrado, o null si ocurre un error.</returns>
        public static string? ObtenerCodigoMayorSugerido(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    // Filtra códigos con longitud entre 7 y 10 caracteres
                    return contexto.Productos.Where(p => p.CodProducto.Length > 6 && p.CodProducto.Length < 11)
                                             .Max(p => p.CodProducto);
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener códigos de productos:\n{ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Obtiene un producto a partir de su código.
        /// </summary>
        /// <param name="codigo">Código del producto a buscar.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>El producto encontrado o null si no existe.</returns>
        public static Productos? ObtenerProductoPorCodigo(string codigo, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.Productos.FirstOrDefault(p => p.CodProducto == codigo);
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener producto por código:\n{ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Lista todas las unidades de medida disponibles.
        /// </summary>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>Lista de unidades de medida ordenadas por nombre, o null si ocurre un error.</returns>
        public static List<UnidadesMedidas>? ListarUnidadesMedida(ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return null;

                    return contexto.UnidadesMedidas
                                   .OrderBy(u => u.Unidad)
                                   .ToList();
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener Unidades de Medida:\n{ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// Cambia el estado (activo/inactivo) de un producto.
        /// </summary>
        /// <param name="producto">Producto con el nuevo estado.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si se actualizó correctamente, False en caso contrario.</returns>
        public static bool CambiarEstadoProducto(Productos producto, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    // Busca el producto por Id
                    Productos? prod = contexto.Productos.FirstOrDefault(p => p.IdProducto == producto.IdProducto);
                    if (prod == null)
                    {
                        mensaje = "Producto no encontrado";
                        return false;
                    }

                    // Actualiza el estado
                    prod.Activo = producto.Activo;
                    contexto.Productos.Update(prod);
                    int exito = contexto.SaveChanges();

                    return exito > 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener producto por código:\n{ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Registra un nuevo producto en la base de datos.
        /// </summary>
        /// <param name="producto">Producto a registrar.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>El Id del producto registrado, o 0 si ocurre un error.</returns>
        public static int RegistrarProducto(Productos producto, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return 0;

                    // Se asegura de que el Id sea nulo para que EF lo genere
                    producto.IdProducto = null;
                    contexto.Productos.Add(producto);
                    contexto.SaveChanges();

                    return producto.IdProducto ?? 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al registrar el producto:\n{ex.Message}";
                return 0;
            }
        }

        /// <summary>
        /// Modifica los datos de un producto existente.
        /// </summary>
        /// <param name="producto">Producto con los datos actualizados.</param>
        /// <param name="mensaje">Mensaje de error en caso de fallo.</param>
        /// <returns>True si se modificó correctamente, False en caso contrario.</returns>
        public static bool ModificarProducto(Productos producto, ref string mensaje)
        {
            try
            {
                using (Contexto contexto = new Contexto())
                {
                    if (!ComprobarContexto(contexto, ref mensaje))
                        return false;

                    contexto.Productos.Update(producto);
                    int exito = contexto.SaveChanges();

                    return exito > 0;
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al modificar el producto:\n{ex.Message}";
                return false;
            }
        }
    }
}
