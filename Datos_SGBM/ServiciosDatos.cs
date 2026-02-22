using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Servicios.
    ///
    /// Responsabilidades:
    /// - Proveer operaciones de acceso y persistencia relacionadas con servicios (listar, buscar,
    ///   insertar, actualizar, obtener por Id/nombre, búsqueda avanzada, etc.).
    /// - Incluir relaciones de lectura necesarias (Categorias, CostosServicios u otras) únicamente
    ///   cuando el método lo requiera, evitando sobrescribir navegaciones inadvertidamente.
    /// - Validar la disponibilidad del <see cref="Contexto"/> y de los <see cref="DbSet"/> usados
    ///   mediante <see cref="ComprobacionContexto"/> antes de ejecutar consultas o escrituras.
    /// - Devolver resultados uniformes usando el patrón <c>Resultado&lt;T&gt;</c> para transportar
    ///   tanto los datos como mensajes de error y facilitar el manejo en capas superiores.
    ///
    /// Diseño y buenas prácticas:
    /// - Mantener la clase enfocada en acceso a datos; las reglas de negocio, validaciones complejas,
    ///   normalizaciones y políticas de autorización deben residir en la capa de negocio.
    /// - Al leer, usar <c>Include</c> y <c>ThenInclude</c> solo cuando se necesiten las relaciones;
    ///   al actualizar, recuperar la entidad existente y asignar únicamente los campos escalares para
    ///   evitar inserciones o modificaciones accidentales de relaciones de navegación.
    /// - Normalizar criterios de búsqueda textuales con <c>ToLowerInvariant()</c> en el criterio (y en la columna
    ///   si EF lo traduce a SQL) para búsquedas case‑insensitive; no aplicar ToLower a códigos numéricos,
    ///   en cuyo caso usar <c>Trim()</c> y comparaciones exactas o normalización acordada (ceros a la izquierda, etc.).
    /// - Evitar pasar mensajes por referencia; devolver mensajes de error dentro de <c>Resultado&lt;T&gt;</c>.
    /// - Registrar errores técnicos con <c>Logger</c> y devolver mensajes claros y amigables en las respuestas.
    ///
    /// Consideraciones de rendimiento y operativas:
    /// - Evitar aplicar funciones CLR sobre columnas en WHERE que impidan el uso de índices; para búsquedas frecuentes
    ///   considerar collations case‑insensitive, columnas computadas normalizadas e índices sobre ellas.
    /// - Para búsquedas complejas (por ejemplo búsqueda avanzada que cruza CostosServicios) modularizar la lógica
    ///   en métodos privados y/o delegar parte de la orquestación a la capa de negocio para facilitar pruebas y paginación.
    /// - Implementar paginación en la capa de negocio y aplicar <c>Skip/Take</c> en la capa de datos cuando proceda.
    /// - Tener en cuenta las precisiones declaradas en el modelo (decimal(12,2), etc.) al validar y mapear valores desde la UI o servicios externos.
    ///
    /// Seguridad y robustez:
    /// - No exponer excepciones crudas; capturar excepciones, registrar detalles técnicos y devolver mensajes
    ///   útiles en <c>Resultado&lt;T&gt;</c>.
    /// - Validar entradas (nulls, longitudes máximas, rangos numéricos) antes de persistir.
    /// - Evitar que entidades relacionadas entrantes provoquen inserciones no deseadas limpiando colecciones
    ///   o navegaciones cuando corresponda.
    ///
    /// Métodos esperados y su comportamiento:
    /// - <c>ListaServicios()</c>: listar servicios con las relaciones necesarias para lectura.
    /// - <c>RegistrarServicio(Servicios)</c>: validar campos mínimos, normalizar cadenas y devolver Id generado.
    /// - <c>ModificarServicio(Servicios)</c>: recuperar entidad existente y actualizar solo campos escalares.
    /// - <c>ObtenerServicioPorId(int)</c>, <c>ObtenerServicioPorNombre(string)</c>: búsquedas con inclusión de relaciones cuando se requiera.
    /// - <c>BuscarAvanzado(...)</c>: orquestar búsquedas complejas delegando filtros a helpers reutilizables y devolviendo resultados paginables.
    ///
    /// Extensibilidad:
    /// - Centralizar la traducción de criterios de búsqueda en un helper (por ejemplo <c>FiltrosHelper</c>) para mantener consistencia.
    /// - Considerar la introducción de DTOs o proyecciones para consultas que devuelvan grandes volúmenes de datos.
    /// - Añadir pruebas unitarias e integración para cada método público y para los helpers de filtrado.
    ///
    /// </summary>
    public class ServiciosDatos
    {
        #region Listados (sin criterios)
        /// <summary>
        /// Lista todos los servicios ordenados por nombre.
        /// </summary>
        /// <returns>Resultado con la lista de servicios ordenada o mensaje de error.</returns>
        public static Resultado<List<Servicios>> ListaServicios()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Servicios, nameof(contexto.Servicios));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Servicios>>.Fail(rc.Mensaje);
                    }

                    var lista = contexto.Servicios
                                        .Include(s => s.Categorias)
                                        .OrderBy(s => s.NombreServicio)
                                        .ToList();

                    if (lista == null || lista.Count == 0)
                        return Resultado<List<Servicios>>.Fail("No se encontraron servicios.");

                    return Resultado<List<Servicios>>.Ok(lista);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener servicios:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Servicios>>.Fail(msg);
            }
        }

        #endregion

        #region Interacciones con BD (Registros, modificaciones, etc)

        /// <summary>
        /// Registra un nuevo servicio en la base de datos.
        /// </summary>
        /// <param name="servicio">Objeto Servicios a registrar.</param>
        /// <returns>Resultado con el Id del servicio registrado o mensaje de error.</returns>
        public static Resultado<int> RegistrarServicio(Servicios? servicio)
        {
            if (servicio == null)
                return Resultado<int>.Fail("El servicio no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(servicio.NombreServicio))
                return Resultado<int>.Fail("El campo NombreServicio es obligatorio.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Servicios, nameof(contexto.Servicios));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<int>.Fail(rc.Mensaje);
                    }

                    // Normalizar y preparar entidad
                    servicio.IdServicio = null;
                    servicio.NombreServicio = servicio.NombreServicio.Trim();
                    servicio.Descripcion = string.IsNullOrWhiteSpace(servicio.Descripcion) ? null : servicio.Descripcion.Trim();

                    contexto.Servicios.Add(servicio);
                    contexto.SaveChanges();

                    if (servicio.IdServicio != null && servicio.IdServicio > 0)
                        return Resultado<int>.Ok(servicio.IdServicio.Value);

                    var msg = "No se pudo obtener el Id del servicio registrado.";
                    Logger.LogError(msg);
                    return Resultado<int>.Fail(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar servicio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica los datos de un servicio existente.
        /// </summary>
        /// <param name="servicio">Servicio con los datos actualizados (debe incluir IdServicio).</param>
        /// <returns>Resultado indicando éxito o mensaje de error.</returns>
        public static Resultado<bool> ModificarServicio(Servicios? servicio)
        {
            if (servicio == null || servicio.IdServicio == null || servicio.IdServicio <= 0)
                return Resultado<bool>.Fail("Debe indicar un servicio válido con IdServicio.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Servicios, nameof(contexto.Servicios));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<bool>.Fail(rc.Mensaje);
                    }

                    var existente = contexto.Servicios.FirstOrDefault(s => s.IdServicio == servicio.IdServicio);
                    if (existente == null)
                        return Resultado<bool>.Fail($"No se encontró el servicio con Id {servicio.IdServicio}.");

                    // Actualizar solo campos escalares para no sobrescribir la navegación Categorias
                    existente.NombreServicio = string.IsNullOrWhiteSpace(servicio.NombreServicio) ? existente.NombreServicio : servicio.NombreServicio.Trim();
                    existente.Descripcion = string.IsNullOrWhiteSpace(servicio.Descripcion) ? existente.Descripcion : servicio.Descripcion.Trim();
                    existente.PrecioVenta = servicio.PrecioVenta;
                    existente.Costos = servicio.Costos;
                    existente.Margen = servicio.Margen;
                    existente.Comision = servicio.Comision;
                    existente.DuracionMinutos = servicio.DuracionMinutos;
                    existente.Puntaje = servicio.Puntaje;
                    existente.IdCategoria = servicio.IdCategoria;
                    existente.Activo = servicio.Activo;

                    int cambios = contexto.SaveChanges();

                    return cambios > 0
                        ? Resultado<bool>.Ok(true)
                        : Resultado<bool>.Ok(true, "No se realizaron cambios al servicio.");
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al modificar el servicio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        #endregion

        #region Consultas

        /// <summary>
        /// Obtiene un servicio por su Id.
        /// </summary>
        /// <param name="idServicio">Id del servicio a buscar.</param>
        /// <returns>Resultado con el servicio encontrado o mensaje de error.</returns>
        public static Resultado<Servicios?> ObtenerServicioPorId(int idServicio)
        {
            if (idServicio <= 0)
                return Resultado<Servicios?>.Fail("Debe indicar un Id de servicio válido.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Servicios, nameof(contexto.Servicios));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<Servicios?>.Fail(rc.Mensaje);
                    }

                    var servicio = contexto.Servicios
                                           .Include(s => s.Categorias)
                                           .FirstOrDefault(s => s.IdServicio == idServicio);

                    if (servicio == null)
                        return Resultado<Servicios?>.Fail("No se encontró el servicio con el Id proporcionado.");

                    return Resultado<Servicios?>.Ok(servicio);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener servicio por Id:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Servicios?>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene un servicio por su nombre (búsqueda case-insensitive).
        /// </summary>
        /// <param name="nombreServicio">Nombre del servicio a buscar.</param>
        /// <returns>Resultado con el servicio encontrado o mensaje de error.</returns>
        public static Resultado<Servicios?> ObtenerServicioPorNombre(string nombreServicio)
        {
            if (string.IsNullOrWhiteSpace(nombreServicio))
                return Resultado<Servicios?>.Fail("El nombre del servicio no puede estar vacío.");

            var nombreCrit = nombreServicio.Trim().ToLower();

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Servicios, nameof(contexto.Servicios));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<Servicios?>.Fail(rc.Mensaje);
                    }

                    var servicio = contexto.Servicios
                                           .Include(s => s.Categorias)
                                           .FirstOrDefault(s => s.NombreServicio != null
                                                             && s.NombreServicio.ToLower() == nombreCrit);

                    if (servicio == null)
                        return Resultado<Servicios?>.Fail("No se encontró el servicio con el nombre proporcionado.");

                    return Resultado<Servicios?>.Ok(servicio);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener servicio por nombre:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Servicios?>.Fail(msg);
            }
        }

        /// <summary>
        /// Búsqueda avanzada pública en la capa de datos para la entidad <see cref="Servicios"/>.
        /// </summary>
        /// <param name="campo">Nombre del campo a filtrar; se normaliza en minúsculas en el método llamador.</param>
        /// <param name="criterio">Criterio de filtrado (por ejemplo: "contiene", "no contiene", "mayor que", "igual").</param>
        /// <param name="valor">Valor de comparación; para campos textuales se usa tal cual, para numéricos se intenta parsear a decimal.</param>
        /// <param name="idCategoria">Id de categoría para filtrar; si es mayor que 0 se aplica el filtro de categoría.</param>
        /// <returns>
        /// <see cref="Resultado{T}"/> que contiene la lista de <see cref="Servicios"/> que cumplen los criterios ordenada por <see cref="Servicios.NombreServicio"/>,
        /// o un <see cref="Resultado{T}"/> con el mensaje de error en caso de fallo.
        /// </returns>
        /// <remarks>
        /// - Este método orquesta la búsqueda avanzada y delega la lógica específica a métodos privados:
        ///   <see cref="BuscarPorNombreCosto"/> para la rama especial "nombre costo" y <see cref="BuscarPorCampoDirecto"/> para los demás campos.
        /// - El <see cref="Contexto"/> se crea y valida aquí; los métodos privados reciben el contexto ya inicializado para evitar abrir múltiples contextos.
        /// - **Normalización**: el parámetro <paramref name="campo"/> se normaliza con <c>Trim().ToLower())</c> para facilitar la detección de la rama a ejecutar.
        /// - **Política de errores**: las excepciones se capturan, se registran con <c>Logger</c> y se devuelven dentro de <see cref="Resultado{T}"/>; no se lanzan excepciones hacia capas superiores.
        /// - **Comportamiento ante campos no reconocidos**: si el campo no coincide con la rama especial, se delega a <c>BuscarPorCampoDirecto</c>, que por su política puede devolver todos los registros (sujeto a filtro de categoría) o aplicar los filtros reconocidos.
        /// - **Rendimiento**: para búsquedas que impliquen tablas relacionadas (por ejemplo CostosServicios) se usan <c>Include</c> y proyecciones con <c>Distinct()</c> para evitar duplicados y N+1.
        /// - **Extensibilidad**: nuevas reglas de negocio, validaciones o paginación deben implementarse en la capa de negocio; la capa de datos se mantiene enfocada en consultas y materialización.
        /// </remarks>
        public static Resultado<List<Servicios>> BuscarAvanzado(string campo, string criterio, string valor, int idCategoria)
        {
            try
            {
                using var contexto = new Contexto();

                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Servicios, nameof(contexto.Servicios));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<List<Servicios>>.Fail(rc.Mensaje);
                }

                if (string.IsNullOrWhiteSpace(campo))
                    return Resultado<List<Servicios>>.Fail("Debe indicar el campo a filtrar.");

                string campoLower = campo.Trim().ToLower();

                if (campoLower.Contains("nombre costo"))
                {
                    var resultado = BuscarPorNombreCosto(contexto, criterio, valor, idCategoria);
                    return Resultado<List<Servicios>>.Ok(resultado);
                }
                else
                {
                    var resultado = BuscarPorCampoDirecto(contexto, campoLower, criterio, valor, idCategoria);
                    return Resultado<List<Servicios>>.Ok(resultado);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error en búsqueda avanzada:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Servicios>>.Fail(msg);
            }
        }

        #region Métodos privados de busqueda avanzada

        /// <summary>
        /// Busca servicios cuyo/ cuyos costos asociados cumplan el criterio sobre la descripción de costo,
        /// o bien excluye servicios que tengan costos que contengan el texto cuando el criterio es "no contiene".
        /// </summary>
        /// <param name="contexto">Contexto de datos ya inicializado y validado por el llamador.</param>
        /// <param name="criterio">Criterio de filtrado (por ejemplo: "contiene", "no contiene", "igual", "mayor que").</param>
        /// <param name="valor">Valor de comparación para la descripción de costos; se usa tal cual para búsquedas de texto.</param>
        /// <param name="idCategoria">Id de categoría para filtrar; si es mayor que 0 se aplica el filtro de categoría sobre el servicio asociado.</param>
        /// <returns>
        /// Lista de <see cref="Servicios"/> que cumplen los criterios, ordenada por <see cref="Servicios.NombreServicio"/>.
        /// Devuelve servicios distintos aun cuando varios costos del mismo servicio cumplan el filtro.
        /// </returns>
        /// <remarks>
        /// - El método **no** gestiona el ciclo de vida del <see cref="Contexto"/>; el contexto debe ser creado y validado por el método público que lo invoque.
        /// - Se realiza una búsqueda inicial de IDs de servicios que tienen al menos un costo cuya descripción contiene <paramref name="valor"/>.
        /// — Esta lista se usa para la rama "no contiene" (exclusión).
        /// - Para la rama positiva (no "no contiene") se aplica el filtro sobre <c>CostosServicios</c> delegando la traducción del
        ///   <paramref name="criterio"/> a <c>FiltrosHelper.AplicarFiltroTexto</c>. Después se proyecta a los servicios asociados,
        ///   se eliminan duplicados con <c>Distinct()</c> y se ordena por nombre.
        /// - Se incluye la navegación a <c>Servicios</c> y a <c>Categorias</c> mediante <c>Include</c> y <c>ThenInclude</c> para evitar
        ///   consultas N+1 y permitir que la capa superior disponga de las relaciones necesarias.
        /// - Si <paramref name="idCategoria"/> es mayor que 0, se aplica el filtro de categoría sobre el servicio asociado
        ///   antes de materializar resultados para reducir el conjunto de datos procesado.
        /// - Política de errores y normalización:
        ///   * <c>valor</c> se usa tal cual para búsquedas de texto; la normalización (trim, case) debe decidirse en la capa de negocio.
        ///   * Si se requiere que la búsqueda sea case‑insensitive y eficiente, considerar usar collations SQL case‑insensitive
        ///     o columnas computadas normalizadas en lugar de aplicar funciones CLR en la consulta.
        /// - Rendimiento:
        ///   * La consulta que obtiene <c>idsConCosto</c> materializa una lista de IDs; esto es intencional para poder usarla
        ///     en una cláusula <c>Where(... !idsConCosto.Contains(...))</c> que EF traducirá a SQL con parámetros.
        ///   * Para conjuntos muy grandes, considerar paginación o limitar la búsqueda previa de IDs.
        /// - Extensibilidad:
        ///   * Si se añaden más campos en <c>CostosServicios</c> que deban filtrarse, centralizar la lógica en <c>FiltrosHelper</c>.
        ///   * Si la política de negocio cambia (por ejemplo, devolver error cuando <c>valor</c> está vacío), mover esa validación a la capa de negocio.
        /// </remarks>
        private static List<Servicios> BuscarPorNombreCosto(Contexto contexto, string criterio, string valor, int idCategoria)
        {
            // IDs de servicios que tienen al menos un costo cuya descripción contiene el texto buscado
            var idsConCosto = contexto.CostosServicios
                                      .Where(c => c.Descripcion != null && c.Descripcion.Contains(valor))
                                      .Select(c => c.IdServicio)
                                      .Distinct()
                                      .ToList();

            // Consulta base sobre CostosServicios incluyendo el servicio y su categoría
            var queryCostos = contexto.CostosServicios
                                     .Include(c => c.Servicios)
                                        .ThenInclude(s => s.Categorias)
                                     .Where(c => c.Servicios != null);

            // Aplicar filtro de categoría sobre el servicio asociado si corresponde
            if (idCategoria > 0)
                queryCostos = queryCostos.Where(c => c.Servicios!.IdCategoria == idCategoria);

            // Rama de exclusión: si el criterio es "no contiene", devolvemos servicios que NO están en idsConCosto
            if (string.Equals(criterio, "no contiene", StringComparison.OrdinalIgnoreCase))
            {
                return contexto.Servicios
                               .Where(s => !idsConCosto.Contains(s.IdServicio!.Value))
                               .OrderBy(s => s.NombreServicio)
                               .ToList();
            }
            else
            {
                // Para los demás criterios, aplicar el filtro sobre la descripción de costos y proyectar a servicios distintos
                queryCostos = FiltrosHelper.AplicarFiltroTexto(queryCostos, c => c.Descripcion!, criterio, valor);

                return queryCostos.Select(c => c.Servicios!)
                                  .Distinct()
                                  .OrderBy(s => s.NombreServicio)
                                  .ToList();
            }
        }

        /// <summary>
        /// Realiza la búsqueda directa sobre campos de la entidad <see cref="Servicios"/>.
        /// </summary>
        /// <param name="contexto">Contexto de datos ya inicializado y validado por el llamador.</param>
        /// <param name="campoLower">Nombre del campo a filtrar en minúsculas y ya normalizado.</param>
        /// <param name="criterio">Criterio de filtrado (por ejemplo: "contiene", "no contiene", "mayor que", "igual").</param>
        /// <param name="valor">Valor de comparación; para campos numéricos se intenta parsear a <see cref="decimal"/>.</param>
        /// <param name="idCategoria">Id de categoría para filtrar; si es mayor que 0 se aplica el filtro de categoría.</param>
        /// <returns>
        /// Lista de <see cref="Servicios"/> que cumplen los filtros, ordenada por <see cref="Servicios.NombreServicio"/>.
        /// Devuelve todos los registros que cumplan los filtros aplicables; si el campo no se reconoce, devuelve la lista completa (sujeta al filtro de categoría).
        /// </returns>
        /// <remarks>
        /// - Este método **no** abre ni cierra el <see cref="Contexto"/>; el contexto debe ser gestionado por el método público que lo invoque.
        /// - Para filtros de texto se delega en <c>FiltrosHelper.AplicarFiltroTexto</c>, que debe traducir el <paramref name="criterio"/> a expresiones LINQ compatibles con EF.
        /// - Para filtros numéricos se intenta parsear <paramref name="valor"/> a <see cref="decimal"/>; si el parseo falla, el filtro numérico se omite (se devuelve la consulta sin aplicar ese criterio).
        /// - Se incluye la navegación <c>Categorias</c> para permitir filtrado/ordenación y evitar consultas N+1 en capas superiores.
        /// - El método aplica el filtro de categoría antes de aplicar el filtro por campo para reducir el conjunto de datos procesado.
        /// - Si se requiere otra política (por ejemplo, devolver error cuando el campo no se reconoce o cuando el parseo numérico falla),
        ///   esa validación debe moverse a la capa de negocio o al método público que orquesta la búsqueda.
        /// </remarks>
        private static List<Servicios> BuscarPorCampoDirecto(Contexto contexto, string campoLower, string criterio, string valor, int idCategoria)
        {
            // Iniciamos la consulta incluyendo la navegación de Categorías
            IQueryable<Servicios> query = contexto.Servicios.Include(s => s.Categorias);

            // Aplicar filtro por categoría si corresponde
            if (idCategoria > 0)
                query = query.Where(s => s.IdCategoria == idCategoria);

            // Ramas por campo reconocidos (comparación sobre campoLower ya normalizado)
            if (campoLower.Contains("descrip"))
            {
                // Filtro de texto sobre Descripcion (delegado a helper)
                query = FiltrosHelper.AplicarFiltroTexto(query, s => s.Descripcion!, criterio, valor);
            }
            else if (campoLower.Contains("precio"))
            {
                // Filtro numérico sobre PrecioVenta
                if (decimal.TryParse(valor, out var numero))
                    query = FiltrosHelper.AplicarFiltroNumerico(query, s => s.PrecioVenta, criterio, numero);
                // Si no parsea, se omite el filtro numérico (política: no lanzar aquí)
            }
            else if (campoLower.Contains("puntaje"))
            {
                // Puntaje es entero en el modelo; se usa decimal para compatibilidad con el helper numérico
                if (decimal.TryParse(valor, out var numero))
                    query = FiltrosHelper.AplicarFiltroNumerico(query, s => s.Puntaje, criterio, numero);
            }
            else if (campoLower.Contains("duracion"))
            {
                // Duración en minutos (entero)
                if (decimal.TryParse(valor, out var numero))
                    query = FiltrosHelper.AplicarFiltroNumerico(query, s => s.DuracionMinutos, criterio, numero);
            }
            else if (campoLower.Contains("costo total") || campoLower.Contains("costos"))
            {
                // Costos totales del servicio
                if (decimal.TryParse(valor, out var numero))
                    query = FiltrosHelper.AplicarFiltroNumerico(query, s => s.Costos, criterio, numero);
            }

            // Ordenar por nombre de servicio y materializar la lista
            return query.OrderBy(s => s.NombreServicio).ToList();
        }

        #endregion

        #endregion

    }
}
