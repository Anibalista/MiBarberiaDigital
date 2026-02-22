using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Domicilios.
    /// 
    /// Responsabilidades:
    /// - Proveer operaciones CRUD y consultas específicas relacionadas con domicilios.
    /// - Gestionar la creación y resolución de entidades relacionadas (Localidades, Provincias)
    ///   cuando sea necesario para mantener la integridad referencial en la capa de datos.
    /// - Validar la disponibilidad del <see cref="Contexto"/> y de los <see cref="DbSet"/> usados
    ///   mediante <see cref="ComprobacionContexto"/> antes de ejecutar consultas o escrituras.
    /// - Devolver resultados uniformes usando el patrón <c>Resultado&lt;T&gt;</c> para transportar
    ///   tanto los datos como mensajes de error y facilitar el manejo en capas superiores.
    ///
    /// Diseño y buenas prácticas:
    /// - No contiene reglas de negocio; solo validaciones técnicas (nulls, ids válidos, existencia de DbSet).
    /// - Evita exponer excepciones crudas: registra detalles técnicos con <c>Logger</c> y devuelve mensajes
    ///   claros en <c>Resultado&lt;T&gt;</c>.
    /// - Cuando se actualizan entidades, recupera la entidad existente y asigna solo los campos permitidos
    ///   para evitar sobrescribir relaciones o provocar inserciones accidentales.
    /// - Usa <c>ComprobacionContexto.ComprobarEntidad</c> o <c>ComprobarEntidades</c> según corresponda,
    ///   para centralizar la comprobación del contexto y mantener mensajes consistentes.
    /// - No crea ni dispone contextos fuera de los métodos; cada método crea su propio <c>Contexto</c>
    ///   salvo que se diseñe explícitamente para recibir uno (por ejemplo, para transacciones).
    ///
    /// Consideraciones operativas:
    /// - Las operaciones que requieren atomicidad entre varias tablas deben orquestarse en la capa de negocio
    ///   con transacciones explícitas.
    /// - Para búsquedas sensibles a mayúsculas/minúsculas, preferir la collation de la base de datos o
    ///   normalizar con <c>ToLowerInvariant()</c> solo cuando sea necesario y no afecte el uso de índices.
    /// - Métodos auxiliares internos (p. ej. para obtener el siguiente Id) devuelven <c>Resultado&lt;int&gt;</c>
    ///   para un manejo de errores consistente.
    /// </summary>
    public class DomiciliosDatos
    {

        #region Listados

        /// <summary>
        /// Obtiene todas las provincias desde la base de datos.
        /// </summary>
        /// <returns>Resultado con la lista de provincias o mensaje de error.</returns>
        public static Resultado<List<Provincias>> GetProvincias()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Provincias, nameof(contexto.Provincias));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Provincias>>.Fail(rc.Mensaje);
                    }

                    var provincias = contexto.Provincias.ToList();
                    if (provincias == null || provincias.Count == 0)
                        return Resultado<List<Provincias>>.Fail("No se encontraron provincias.");

                    return Resultado<List<Provincias>>.Ok(provincias);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener Provincias:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Provincias>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene todas las localidades ordenadas por nombre.
        /// </summary>
        /// <returns>Resultado con la lista de localidades o mensaje de error.</returns>
        public static Resultado<List<Localidades>> GetLocalidades()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Localidades, nameof(contexto.Localidades));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Localidades>>.Fail(rc.Mensaje);
                    }

                    var localidades = contexto.Localidades
                        .OrderBy(l => l.Localidad)
                        .ToList();

                    if (localidades == null || localidades.Count == 0)
                        return Resultado<List<Localidades>>.Fail("No se encontraron localidades.");

                    return Resultado<List<Localidades>>.Ok(localidades);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener Localidades:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Localidades>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene las localidades pertenecientes a una provincia específica.
        /// </summary>
        /// <param name="provincia">Provincia seleccionada.</param>
        /// <returns>Resultado con la lista de localidades o mensaje de error.</returns>
        public static Resultado<List<Localidades>> GetLocalidadesPorProvincia(Provincias? provincia)
        {
            if (provincia == null || provincia.IdProvincia == null || provincia.IdProvincia <= 0)
                return Resultado<List<Localidades>>.Fail("Provincia inválida para la consulta de localidades.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Localidades, nameof(contexto.Localidades)),
                        (contexto.Provincias, nameof(contexto.Provincias))
                    );

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Localidades>>.Fail(rc.Mensaje);
                    }

                    var localidades = contexto.Localidades
                        .Where(l => l.IdProvincia == provincia.IdProvincia)
                        .OrderBy(l => l.Localidad)
                        .ToList();

                    if (localidades == null || localidades.Count == 0)
                        return Resultado<List<Localidades>>.Fail($"No se encontraron localidades para la provincia '{provincia.Provincia ?? "Sin Nombre"}'.");

                    return Resultado<List<Localidades>>.Ok(localidades);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener Localidades de la provincia {provincia?.Provincia ?? "Sin Nombre"}:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Localidades>>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene domicilios filtrados por calle, barrio o localidad.
        /// </summary>
        /// <param name="calle">Nombre de la calle (parcial o completo).</param>
        /// <param name="barrio">Nombre del barrio (parcial o completo).</param>
        /// <param name="localidad">Localidad asociada (objeto con IdLocalidad).</param>
        /// <returns>Resultado con la lista de domicilios o mensaje de error.</returns>
        public static Resultado<List<Domicilios>> GetDomiciliosPorCampos(string? calle, string? barrio, Localidades? localidad)
        {
            // Validación inicial: al menos un criterio debe estar presente
            if (string.IsNullOrWhiteSpace(calle) && string.IsNullOrWhiteSpace(barrio) && (localidad == null || localidad.IdLocalidad == null || localidad.IdLocalidad <= 0))
                return Resultado<List<Domicilios>>.Fail("No llegan los datos de búsqueda de domicilios a la capa datos.");

            int idLocalidad = (localidad != null && localidad.IdLocalidad != null && localidad.IdLocalidad > 0)
                ? localidad.IdLocalidad
                : 0;

            try
            {
                using (var contexto = new Contexto())
                {
                    // Comprobamos las entidades necesarias (Domicilios y Localidades; Provincias se carga por Include)
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Domicilios, nameof(contexto.Domicilios)),
                        (contexto.Localidades, nameof(contexto.Localidades)),
                        (contexto.Provincias, nameof(contexto.Provincias))
                    );

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Domicilios>>.Fail(rc.Mensaje);
                    }

                    // Construcción de la consulta de forma segura
                    var query = contexto.Domicilios
                                       .Include(d => d.Localidades)
                                           .ThenInclude(l => l.Provincias)
                                       .AsQueryable();

                    // Aplicar filtros; cada condición se evalúa solo si el criterio fue provisto
                    query = query.Where(d =>
                        (!string.IsNullOrWhiteSpace(calle) && d.Calle != null && d.Calle.Contains(calle!))
                        || (!string.IsNullOrWhiteSpace(barrio) && d.Barrio != null && d.Barrio.Contains(barrio!))
                        || (idLocalidad > 0 && d.IdLocalidad == idLocalidad)
                    );

                    var domicilios = query.OrderBy(d => d.Calle).ToList();

                    if (domicilios == null || domicilios.Count == 0)
                        return Resultado<List<Domicilios>>.Fail("No se encontraron domicilios con los criterios de búsqueda.");

                    return Resultado<List<Domicilios>>.Ok(domicilios);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener domicilios por campos:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Domicilios>>.Fail(msg);
            }
        }

        #endregion

        #region Consultas

        /// <summary>
        /// Obtiene el siguiente IdProvincia disponible a partir del contexto recibido.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos (no se crea ni se dispone aquí).</param>
        /// <returns>Resultado con el siguiente IdProvincia o Fail con el mensaje de error.</returns>
        static Resultado<int> GetSiguienteIdProvincia(Contexto contexto)
        {
            if (contexto == null)
                return Resultado<int>.Fail("Contexto nulo al obtener siguiente IdProvincia.");

            try
            {
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Provincias, nameof(contexto.Provincias));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<int>.Fail(rc.Mensaje);
                }

                // Si no hay registros, DefaultIfEmpty(0) asegura que Max() devuelva 0
                int idMaximo = contexto.Provincias
                                      .Select(p => p.IdProvincia)
                                      .DefaultIfEmpty(0)
                                      .Max();

                int siguiente = idMaximo + 1;
                return Resultado<int>.Ok(siguiente);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener siguiente IdProvincia:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene el siguiente IdLocalidad disponible a partir del contexto recibido.
        /// </summary>
        /// <param name="contexto">Instancia del contexto de base de datos (no se crea ni se dispone aquí).</param>
        /// <returns>Resultado con el siguiente IdLocalidad o Fail con el mensaje de error.</returns>
        static Resultado<int> GetSiguienteIdLocalidad(Contexto contexto)
        {
            if (contexto == null)
                return Resultado<int>.Fail("Contexto nulo al obtener siguiente IdLocalidad.");

            try
            {
                var comprobacion = new ComprobacionContexto(contexto);
                var rc = comprobacion.ComprobarEntidad(contexto.Localidades, nameof(contexto.Localidades));
                if (!rc.Success)
                {
                    Logger.LogError(rc.Mensaje);
                    return Resultado<int>.Fail(rc.Mensaje);
                }

                int idMaximo = contexto.Localidades
                                      .Select(l => l.IdLocalidad)
                                      .DefaultIfEmpty(0)
                                      .Max();

                int siguiente = idMaximo + 1;
                return Resultado<int>.Ok(siguiente);
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener siguiente IdLocalidad:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene una localidad por su nombre (búsqueda exacta, case-insensitive).
        /// </summary>
        /// <param name="nombre">Nombre de la localidad.</param>
        /// <returns>Resultado con la localidad encontrada o mensaje de error.</returns>
        public static Resultado<Localidades?> GetLocalidadPorNombre(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Resultado<Localidades?>.Fail("No viaja el nombre de la localidad a consultar (capa datos).");

            // Normalizar para comparación segura
            var nombreNormalized = nombre.Trim().ToLower();

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Localidades, nameof(contexto.Localidades));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<Localidades?>.Fail(rc.Mensaje);
                    }

                    var localidad = contexto.Localidades
                        .FirstOrDefault(l => l.Localidad != null && l.Localidad.Trim().ToLower() == nombreNormalized);

                    if (localidad == null)
                        return Resultado<Localidades?>.Fail($"No se encontró la localidad con el nombre '{nombre.Trim()}'.");

                    return Resultado<Localidades?>.Ok(localidad);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener la localidad con el nombre {nombre}:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Localidades?>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene el domicilio asociado a una persona.
        /// </summary>
        /// <param name="persona">Persona con IdDomicilio.</param>
        /// <returns>Resultado con el domicilio encontrado o mensaje de error.</returns>
        public static Resultado<Domicilios?> GetDomicilioPorPersona(Personas? persona)
        {
            if (persona == null || persona.IdDomicilio == null || persona.IdDomicilio <= 0)
                return Resultado<Domicilios?>.Fail("No llegan los datos de la persona a la capa datos o IdDomicilio inválido.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Domicilios, nameof(contexto.Domicilios)),
                        (contexto.Localidades, nameof(contexto.Localidades)),
                        (contexto.Provincias, nameof(contexto.Provincias))
                    );

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<Domicilios?>.Fail(rc.Mensaje);
                    }

                    var domicilio = contexto.Domicilios
                                           .Include(d => d.Localidades)
                                               .ThenInclude(l => l.Provincias)
                                           .FirstOrDefault(d => d.IdDomicilio == persona.IdDomicilio);

                    if (domicilio == null)
                        return Resultado<Domicilios?>.Fail($"No se encontró el domicilio con Id {persona.IdDomicilio}.");

                    return Resultado<Domicilios?>.Ok(domicilio);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener el domicilio por persona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Domicilios?>.Fail(msg);
            }
        }

        #endregion

        #region Interacciones

        /// <summary>
        /// Registra una nueva localidad en la base de datos.
        /// Si la provincia no existe, la crea antes de registrar la localidad.
        /// </summary>
        /// <param name="localidad">Localidad a registrar.</param>
        /// <returns>Resultado con el Id de la localidad registrada o mensaje de error.</returns>
        public static Resultado<int> RegistrarLocalidad(Localidades? localidad)
        {
            if (localidad == null || string.IsNullOrWhiteSpace(localidad.Localidad))
                return Resultado<int>.Fail("La localidad no puede ser nula ni vacía.");

            try
            {
                using (var contexto = new Contexto())
                {
                    // Comprobamos las entidades necesarias
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Localidades, nameof(contexto.Localidades)),
                        (contexto.Provincias, nameof(contexto.Provincias))
                    );
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<int>.Fail(rc.Mensaje);
                    }

                    // Resolver IdProvincia si no llega
                    if (localidad.IdProvincia == 0)
                    {
                        if (localidad.Provincias == null)
                            return Resultado<int>.Fail("La localidad llega sin provincia al registro.");

                        if (localidad.Provincias.IdProvincia > 0)
                        {
                            localidad.IdProvincia = localidad.Provincias.IdProvincia;
                        }
                        else
                        {
                            // Obtener siguiente IdProvincia (método devuelve Resultado<int>)
                            var rcProv = GetSiguienteIdProvincia(contexto);
                            if (!rcProv.Success)
                            {
                                Logger.LogError(rcProv.Mensaje);
                                return Resultado<int>.Fail("Problemas al obtener ids de provincia: " + rcProv.Mensaje);
                            }

                            localidad.Provincias.IdProvincia = rcProv.Data;
                            contexto.Provincias.Add(localidad.Provincias);
                            contexto.SaveChanges();

                            localidad.IdProvincia = localidad.Provincias.IdProvincia;
                        }

                        // Limpiar la navegación para evitar duplicados al persistir localidad
                        if (localidad.IdProvincia > 0)
                            localidad.Provincias = null;
                    }

                    // Si por alguna razón sigue habiendo navegación de Provincias, abortar
                    if (localidad.Provincias != null)
                        return Resultado<int>.Fail("Error al registrar provincias.");

                    // Obtener siguiente IdLocalidad
                    var rcLoc = GetSiguienteIdLocalidad(contexto);
                    if (!rcLoc.Success)
                    {
                        Logger.LogError(rcLoc.Mensaje);
                        return Resultado<int>.Fail("No se puede generar un Id nuevo para la localidad: " + rcLoc.Mensaje);
                    }

                    localidad.IdLocalidad = rcLoc.Data;
                    contexto.Localidades.Add(localidad);
                    contexto.SaveChanges();

                    return Resultado<int>.Ok(localidad.IdLocalidad);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar localidad:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Registra un nuevo domicilio en la base de datos.
        /// </summary>
        /// <param name="domicilio">Domicilio a registrar.</param>
        /// <returns>Resultado con el Id del domicilio registrado o mensaje de error.</returns>
        public static Resultado<int> RegistrarDomicilio(Domicilios? domicilio)
        {
            if (domicilio == null)
                return Resultado<int>.Fail("El domicilio no llega a la capa datos.");

            // Aseguramos que EF asigne el Id (si es autoincremental)
            domicilio.IdDomicilio = null;

            try
            {
                using (var contexto = new Contexto())
                {
                    // Comprobamos las entidades necesarias
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Domicilios, nameof(contexto.Domicilios)),
                        (contexto.Localidades, nameof(contexto.Localidades)),
                        (contexto.Provincias, nameof(contexto.Provincias))
                    );

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<int>.Fail(rc.Mensaje);
                    }

                    // Evitar inserciones accidentales de entidades relacionadas
                    domicilio.Localidades = null;
                    domicilio.Proveedores = null;

                    contexto.Domicilios.Add(domicilio);
                    contexto.SaveChanges();

                    if (domicilio.IdDomicilio == null || domicilio.IdDomicilio == 0)
                    {
                        var msg = "No se pudo obtener el Id del domicilio registrado.";
                        Logger.LogError(msg);
                        return Resultado<int>.Fail(msg);
                    }

                    return Resultado<int>.Ok(domicilio.IdDomicilio.Value);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar el domicilio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Modifica un domicilio existente.
        /// </summary>
        /// <param name="domicilio">Domicilio con datos actualizados.</param>
        /// <returns>Resultado indicando éxito o fallo en la modificación.</returns>
        public static Resultado<bool> ModificarDomicilio(Domicilios? domicilio)
        {
            if (domicilio == null || domicilio.IdDomicilio == null || domicilio.IdDomicilio <= 0)
                return Resultado<bool>.Fail("El domicilio o el Id no llega a la capa datos.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Domicilios, nameof(contexto.Domicilios)),
                        (contexto.Localidades, nameof(contexto.Localidades)),
                        (contexto.Provincias, nameof(contexto.Provincias))
                    );

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<bool>.Fail(rc.Mensaje);
                    }

                    // Recuperar la entidad existente para evitar sobrescribir relaciones no deseadas
                    var existente = contexto.Domicilios.FirstOrDefault(d => d.IdDomicilio == domicilio.IdDomicilio);
                    if (existente == null)
                        return Resultado<bool>.Fail($"No se encontró el domicilio con Id {domicilio.IdDomicilio}.");

                    // Actualizar solo los campos permitidos (evitar reemplazar navegaciones completas)
                    existente.Calle = domicilio.Calle;
                    existente.Altura = domicilio.Altura;
                    existente.Piso = domicilio.Piso;
                    existente.Depto = domicilio.Depto;
                    existente.Barrio = domicilio.Barrio;
                    existente.IdLocalidad = domicilio.IdLocalidad;
                    existente.IdProveedor = domicilio.IdProveedor;

                    int cambios = contexto.SaveChanges();
                    return cambios > 0
                        ? Resultado<bool>.Ok(true)
                        : Resultado<bool>.Fail("No se realizaron cambios al domicilio.");
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al modificar el domicilio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        /// <summary>
        /// Elimina un domicilio por su Id.
        /// </summary>
        /// <param name="id">Id del domicilio a eliminar.</param>
        /// <returns>Resultado indicando éxito o fallo en la eliminación.</returns>
        public static Resultado<bool> EliminarDomicilioPorId(int id)
        {
            if (id < 1)
                return Resultado<bool>.Fail("No llega el id del domicilio a la capa datos.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Domicilios, nameof(contexto.Domicilios));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<bool>.Fail(rc.Mensaje);
                    }

                    var domicilio = contexto.Domicilios.FirstOrDefault(d => d.IdDomicilio == id);
                    if (domicilio == null)
                        return Resultado<bool>.Fail("No se encuentra el domicilio con el Id proporcionado.");

                    contexto.Domicilios.Remove(domicilio);
                    int cambios = contexto.SaveChanges();

                    return cambios > 0
                        ? Resultado<bool>.Ok(true)
                        : Resultado<bool>.Fail("No se pudo eliminar el domicilio.");
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al eliminar domicilio:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        #endregion

    }
}
