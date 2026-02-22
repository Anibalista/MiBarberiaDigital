using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Productos.
    ///
    /// Responsabilidades:
    /// - Proveer operaciones de lectura y escritura relacionadas con productos (listar, buscar,
    ///   insertar, actualizar, cambiar estado, obtener códigos sugeridos, etc.).
    /// - Incluir relaciones de lectura necesarias (Categorias, UnidadesMedidas, Proveedores) solo
    ///   cuando el método lo requiera, evitando sobrescribir navegaciones inadvertidamente.
    /// - Validar la disponibilidad del <see cref="Contexto"/> y de los <see cref="DbSet"/> usados
    ///   mediante <see cref="ComprobacionContexto"/> antes de ejecutar consultas o escrituras.
    /// - Devolver resultados uniformes usando el patrón <c>Resultado&lt;T&gt;</c> para transportar
    ///   tanto los datos como mensajes de error y facilitar el manejo en capas superiores.
    ///
    /// Diseño y buenas prácticas:
    /// - No contiene reglas de negocio; solo validaciones técnicas como nulls, ids válidos y existencia de DbSet.
    /// - Al leer, usar Include solo cuando se necesiten las relaciones; al actualizar, recuperar la entidad
    ///   existente y asignar únicamente los campos escalares para evitar inserciones o modificaciones
    ///   accidentales de relaciones de navegación.
    /// - Normalizar criterios de búsqueda textuales con <c>ToLowerInvariant()</c> en el criterio (y en la columna
    ///   si EF lo traduce a SQL) para búsquedas case‑insensitive; **no** aplicar ToLower a códigos numéricos,
    ///   en cuyo caso usar <c>Trim()</c> y comparaciones exactas o normalización acordada (ceros a la izquierda, etc.).
    /// - Evitar pasar mensajes por referencia; devolver mensajes de error dentro de <c>Resultado&lt;T&gt;</c>.
    /// - Registrar errores técnicos con <c>Logger</c> y devolver mensajes claros y amigables en las respuestas.
    ///
    /// Consideraciones de rendimiento y operativas:
    /// - Aplicar funciones sobre columnas en WHERE (por ejemplo LOWER) puede impedir el uso de índices;
    ///   para búsquedas frecuentes considerar collation case‑insensitive, columnas computadas normalizadas e índices
    ///   sobre ellas, o usar <c>EF.Functions.Like</c> con collations apropiadas.
    /// - Para operaciones que requieren atomicidad entre varias tablas, orquestar transacciones en la capa de negocio
    ///   con un contexto compartido y transacción explícita.
    /// - Validaciones de formato (por ejemplo CodProducto, precios, cantidades) y control de concurrencia
    ///   (por ejemplo <c>RowVersion</c>) deben implementarse en la capa de negocio o en iteraciones posteriores.
    /// - Tener en cuenta las precisiones declaradas en el modelo (decimal(10,4), decimal(12,2), etc.) al validar
    ///   y mapear valores desde la UI o servicios externos.
    ///
    /// Seguridad y robustez:
    /// - No exponer excepciones crudas; capturar excepciones, registrar detalles técnicos y devolver mensajes
    ///   útiles en <c>Resultado&lt;T&gt;</c>.
    /// - Validar entradas (nulls, longitudes máximas, rangos numéricos) antes de persistir.
    /// - Evitar que entidades relacionadas entrantes provoquen inserciones no deseadas limpiando colecciones
    ///   o navegaciones cuando corresponda.
    ///
    /// Ejemplos de métodos esperados:
    /// - <c>ListarSimple()</c>, <c>ObtenerProductoPorCodigo(string)</c>, <c>ObtenerCodigoMayorSugerido()</c>,
    ///   <c>RegistrarProducto(Productos)</c>, <c>ModificarProducto(Productos)</c>, <c>CambiarEstadoProducto(Productos)</c>.
    /// </summary>
    public class ProductosDatos
    {
        #region Listados

        /// <summary>
        /// Obtiene un listado simple de productos incluyendo categorías y unidades de medida.
        /// </summary>
        /// <returns>Resultado con la lista de productos ordenados por descripción o mensaje de error.</returns>
        public static Resultado<List<Productos>> ListarSimple()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Productos, nameof(contexto.Productos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Productos>>.Fail(rc.Mensaje);
                    }

                    var lista = contexto.Productos
                                        .Include(p => p.Categorias)
                                        .Include(p => p.UnidadesMedidas)
                                        .OrderBy(p => p.Descripcion)
                                        .ToList();

                    if (lista == null || lista.Count == 0)
                        return Resultado<List<Productos>>.Ok(new List<Productos>(), "No se encontraron productos.");

                    return Resultado<List<Productos>>.Ok(lista);
                }
            }
            catch (Exception ex)
            {
                string msg = $"Error al obtener productos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Productos>>.Fail(msg);
            }
        }

        /// <summary>
        /// Lista todas las unidades de medida disponibles.
        /// </summary>
        /// <returns>Resultado con la lista de unidades de medida ordenadas por nombre o mensaje de error.</returns>
        public static Resultado<List<UnidadesMedidas>> ListarUnidadesMedida()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.UnidadesMedidas, nameof(contexto.UnidadesMedidas));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<UnidadesMedidas>>.Fail(rc.Mensaje);
                    }

                    var lista = contexto.UnidadesMedidas
                                        .OrderBy(u => u.Unidad)
                                        .ToList();

                    if (lista == null || lista.Count == 0)
                        return Resultado<List<UnidadesMedidas>>.Fail("No se encontraron unidades de medida.");

                    return Resultado<List<UnidadesMedidas>>.Ok(lista);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener Unidades de Medida:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<UnidadesMedidas>>.Fail(msg);
            }
        }

        #endregion

        #region Consultas con claúsulas

        /// <summary>
        /// Obtiene el código de producto más alto sugerido, filtrando por longitud.
        /// </summary>
        /// <returns>Resultado con el código máximo encontrado o mensaje de error.</returns>
        public static Resultado<string> ObtenerCodigoMayorSugerido()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Productos, nameof(contexto.Productos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<string>.Fail(rc.Mensaje);
                    }

                    var query = contexto.Productos
                                        .Where(p => p.CodProducto != null
                                                 && p.CodProducto.Length > 6
                                                 && p.CodProducto.Length < 11)
                                        .Select(p => p.CodProducto);

                    if (!query.Any())
                        return Resultado<string>.Fail("No se encontraron códigos de producto con la longitud esperada.");

                    var maxCodigo = query.Max();
                    return Resultado<string>.Ok(maxCodigo);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener códigos de productos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<string>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene un producto a partir de su código (código numérico).
        /// </summary>
        /// <param name="codigo">Código del producto a buscar.</param>
        /// <returns>Resultado con el producto encontrado o mensaje de error.</returns>
        public static Resultado<Productos?> ObtenerProductoPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return Resultado<Productos?>.Fail("El código de producto no puede estar vacío.");

            // Normalizar: eliminar espacios en los extremos (no aplicar ToLower porque es numérico)
            var codigoCrit = codigo.Trim();

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Productos, nameof(contexto.Productos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<Productos?>.Fail(rc.Mensaje);
                    }

                    var producto = contexto.Productos
                                           .Include(p => p.Categorias)
                                           .Include(p => p.UnidadesMedidas)
                                           .Include(p => p.Proveedores)
                                           .FirstOrDefault(p => p.CodProducto != null && p.CodProducto.Trim() == codigoCrit);

                    if (producto == null)
                        return Resultado<Productos?>.Fail($"No se encontró un producto con código '{codigoCrit}'.");

                    return Resultado<Productos?>.Ok(producto);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener producto por código:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Productos?>.Fail(msg);
            }
        }

        #endregion

        #region Interacción con BD (registros, modificación, etc)

        /// <summary>
        /// Cambia el estado (activo/inactivo) de un producto.
        /// </summary>
        /// <param name="producto">Producto con el Id y el nuevo estado Activo.</param>
        /// <returns>Resultado indicando éxito o mensaje de error.</returns>
        public static Resultado<bool> CambiarEstadoProducto(Productos producto)
        {
            if (producto == null || producto.IdProducto == null || producto.IdProducto <= 0)
                return Resultado<bool>.Fail("Debe indicar un producto válido con IdProducto.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Productos, nameof(contexto.Productos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<bool>.Fail(rc.Mensaje);
                    }

                    var prod = contexto.Productos.FirstOrDefault(p => p.IdProducto == producto.IdProducto);
                    if (prod == null)
                        return Resultado<bool>.Fail("Producto no encontrado.");

                    // Actualizar solo el campo de estado
                    prod.Activo = producto.Activo;

                    int cambios = contexto.SaveChanges();

                    return cambios > 0
                        ? Resultado<bool>.Ok(true)
                        : Resultado<bool>.Fail("No se pudo actualizar el estado del producto.");
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al cambiar el estado del producto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo producto en la base de datos.
        /// </summary>
        /// <param name="producto">Producto a registrar.</param>
        /// <returns>Resultado con el Id del producto registrado o mensaje de error.</returns>
        public static Resultado<int> RegistrarProducto(Productos producto)
        {
            if (producto == null)
                return Resultado<int>.Fail("El producto no puede ser nulo.");

            // Validaciones mínimas
            if (string.IsNullOrWhiteSpace(producto.CodProducto))
                return Resultado<int>.Fail("El campo CodProducto es obligatorio.");

            if (string.IsNullOrWhiteSpace(producto.Descripcion))
                return Resultado<int>.Fail("El campo Descripción es obligatorio.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Productos, nameof(contexto.Productos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<int>.Fail(rc.Mensaje);
                    }

                    // Normalizar campos
                    producto.CodProducto = producto.CodProducto.Trim();
                    producto.Descripcion = producto.Descripcion.Trim();

                    // Asegurar que EF genere el Id
                    producto.IdProducto = null;

                    contexto.Productos.Add(producto);
                    contexto.SaveChanges();

                    if (producto.IdProducto != null && producto.IdProducto > 0)
                        return Resultado<int>.Ok(producto.IdProducto.Value);

                    var msg = "No se pudo obtener el Id del producto registrado.";
                    Logger.LogError(msg);
                    return Resultado<int>.Fail(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar el producto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica los datos de un producto existente.
        /// </summary>
        /// <param name="producto">Producto con los datos actualizados (debe incluir IdProducto).</param>
        /// <returns>Resultado indicando éxito o mensaje de error.</returns>
        public static Resultado<bool> ModificarProducto(Productos producto)
        {
            if (producto == null || producto.IdProducto == null || producto.IdProducto <= 0)
                return Resultado<bool>.Fail("Debe indicar un producto válido con IdProducto.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Productos, nameof(contexto.Productos));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<bool>.Fail(rc.Mensaje);
                    }

                    // Recuperar la entidad existente para evitar sobrescribir navegaciones
                    var existente = contexto.Productos.FirstOrDefault(p => p.IdProducto == producto.IdProducto);
                    if (existente == null)
                        return Resultado<bool>.Fail($"No se encontró el producto con Id {producto.IdProducto}.");

                    // Actualizar únicamente los campos escalares permitidos
                    existente.CodProducto = string.IsNullOrWhiteSpace(producto.CodProducto) ? existente.CodProducto : producto.CodProducto.Trim();
                    existente.Descripcion = string.IsNullOrWhiteSpace(producto.Descripcion) ? existente.Descripcion : producto.Descripcion.Trim();
                    existente.Stock = producto.Stock;
                    existente.CantidadMedida = producto.CantidadMedida;
                    existente.Medida = producto.Medida;
                    existente.PrecioVenta = producto.PrecioVenta;
                    existente.Costo = producto.Costo;
                    existente.IdUnidadMedida = producto.IdUnidadMedida;
                    existente.IdProveedor = producto.IdProveedor;
                    existente.idCategoria = producto.idCategoria;
                    existente.Activo = producto.Activo;
                    existente.Comision = producto.Comision;

                    int cambios = contexto.SaveChanges();

                    return cambios > 0
                        ? Resultado<bool>.Ok(true)
                        : Resultado<bool>.Fail("No se realizaron cambios al producto.");
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al modificar el producto:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        #endregion

    }
}
