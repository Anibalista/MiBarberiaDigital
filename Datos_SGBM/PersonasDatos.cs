using EF_SGBM;
using Entidades_SGBM;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Datos_SGBM
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Personas.
    /// 
    /// Responsabilidades:
    /// - Proveer operaciones CRUD y consultas específicas relacionadas con personas.
    /// - Incluir y resolver relaciones de lectura con Domicilios, Localidades y Provincias
    ///   cuando la operación lo requiera, sin sobrescribir navegaciones inadvertidamente.
    /// - Validar la disponibilidad del <see cref="Contexto"/> y de los <see cref="DbSet"/> usados
    ///   mediante <see cref="ComprobacionContexto"/> antes de ejecutar consultas o escrituras.
    /// - Devolver resultados uniformes usando el patrón <c>Resultado&lt;T&gt;</c> para transportar
    ///   tanto los datos como mensajes de error y facilitar el manejo en capas superiores.
    ///
    /// Diseño y buenas prácticas:
    /// - No contiene reglas de negocio; solo validaciones técnicas como nulls, ids válidos y existencia de DbSet.
    /// - Evita exponer excepciones crudas: registra detalles técnicos con <c>Logger</c> y devuelve mensajes
    ///   claros en <c>Resultado&lt;T&gt;</c>.
    /// - Al actualizar entidades, recupera la entidad existente y asigna únicamente los campos escalares
    ///   del modelo (por ejemplo Dni, Nombres, Apellidos, FechaNac, IdDomicilio) para evitar inserciones o
    ///   modificaciones accidentales de relaciones de navegación.
    /// - Limpia o ignora colecciones y navegaciones entrantes en operaciones de inserción cuando corresponda
    ///   para evitar que EF intente persistir entidades relacionadas no deseadas.
    /// - Usa <see cref="ComprobacionContexto.ComprobarEntidad"/> o <see cref="ComprobacionContexto.ComprobarEntidades"/>
    ///   según el método requiera uno o varios DbSet, centralizando la validación del contexto.
    ///
    /// Consideraciones operativas:
    /// - Las operaciones que requieren atomicidad entre varias tablas deben orquestarse en la capa de negocio
    ///   con transacciones explícitas y contextos compartidos por la operación transaccional.
    /// - Para búsquedas textuales, preferir collation case insensitive en la base de datos o columnas computadas
    ///   normalizadas para mantener el uso de índices; si se normaliza en C#, documentar el impacto en índices.
    /// - Validaciones de formato (DNI, email) y control de concurrencia (RowVersion) se implementan en la capa
    ///   de negocio o en iteraciones posteriores según necesidad.
    /// </summary>
    public class PersonasDatos
    {
        #region Listados

        /// <summary>
        /// Obtiene la lista completa de personas, incluyendo sus domicilios,
        /// localidades y provincias asociadas.
        /// </summary>
        /// <returns>
        /// Resultado con la lista de personas o mensaje de error.
        /// </returns>
        public static Resultado<List<Personas>> GetPersonas()
        {
            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Personas, nameof(contexto.Personas)),
                        (contexto.Domicilios, nameof(contexto.Domicilios)),
                        (contexto.Localidades, nameof(contexto.Localidades)),
                        (contexto.Provincias, nameof(contexto.Provincias))
                    );

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Personas>>.Fail(rc.Mensaje);
                    }

                    var query = contexto.Personas
                        .Include(p => p.Domicilios)
                            .ThenInclude(d => d.Localidades)
                                .ThenInclude(l => l.Provincias)
                        .OrderBy(p => p.Apellidos)
                        .ThenBy(p => p.Nombres);

                    var personas = query.ToList();
                    return Resultado<List<Personas>>.Ok(personas);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener el registro de personas:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Personas>>.Fail(msg);
            }
        }

        #endregion

        #region Búsquedas

        /// <summary>
        /// Obtiene una persona a partir de su DNI, incluyendo domicilio,
        /// localidad y provincia asociada.
        /// </summary>
        /// <param name="dni">Número de documento de la persona.</param>
        /// <returns>
        /// Resultado con el objeto Persona encontrado o mensaje de error.
        /// </returns>
        public static Resultado<Personas?> GetPersonaPorDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return Resultado<Personas?>.Fail("El DNI no llega a la consulta.");

            var dniNormalized = dni.Trim();

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Personas, nameof(contexto.Personas)),
                        (contexto.Domicilios, nameof(contexto.Domicilios)),
                        (contexto.Localidades, nameof(contexto.Localidades)),
                        (contexto.Provincias, nameof(contexto.Provincias))
                    );

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<Personas?>.Fail(rc.Mensaje);
                    }

                    var persona = contexto.Personas
                        .Include(p => p.Domicilios)
                            .ThenInclude(d => d.Localidades)
                                .ThenInclude(l => l.Provincias)
                        .FirstOrDefault(p => p.Dni == dniNormalized);

                    if (persona == null)
                        return Resultado<Personas?>.Fail($"No se encontró una persona con DNI: {dniNormalized}");

                    return Resultado<Personas?>.Ok(persona);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener a la persona con DNI {dniNormalized}:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<Personas?>.Fail(msg);
            }
        }

        /// <summary>
        /// Obtiene una lista de personas filtradas por DNI o nombres/apellidos.
        /// Incluye domicilio, localidad y provincia asociada.
        /// </summary>
        /// <param name="dni">Número de documento parcial o completo.</param>
        /// <param name="nombres">Texto a buscar en Nombres o Apellidos.</param>
        /// <returns>Resultado con la lista de personas encontradas o mensaje de error.</returns>
        public static Resultado<List<Personas>> GetPersonasPorDniNombres(string? dni, string? nombres)
        {
            if (string.IsNullOrWhiteSpace(dni) && string.IsNullOrWhiteSpace(nombres))
                return Resultado<List<Personas>>.Fail("No llegan los datos de búsqueda.");

            // Normalizar criterios en C#
            var dniCrit = string.IsNullOrWhiteSpace(dni) ? null : dni.Trim();
            var nombresCrit = string.IsNullOrWhiteSpace(nombres) ? null : nombres.Trim();

            // Convertir nombres a minúsculas invariante
            var nombresCritLower = nombresCrit?.ToLower();

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Personas, nameof(contexto.Personas)),
                        (contexto.Domicilios, nameof(contexto.Domicilios)),
                        (contexto.Localidades, nameof(contexto.Localidades)),
                        (contexto.Provincias, nameof(contexto.Provincias))
                    );

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<List<Personas>>.Fail(rc.Mensaje);
                    }

                    var query = contexto.Personas
                        .Include(p => p.Domicilios)
                            .ThenInclude(d => d.Localidades)
                                .ThenInclude(l => l.Provincias)
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(dniCrit) && !string.IsNullOrWhiteSpace(nombresCritLower))
                    {
                        query = query.Where(p =>
                            (p.Dni != null && p.Dni.Contains(dniCrit)) ||
                            (p.Nombres != null && p.Nombres.ToLower().Contains(nombresCritLower)) ||
                            (p.Apellidos != null && p.Apellidos.ToLower().Contains(nombresCritLower))
                        );
                    }
                    else if (!string.IsNullOrWhiteSpace(dniCrit))
                    {
                        query = query.Where(p => p.Dni != null && p.Dni.Contains(dniCrit));
                    }
                    else
                    {
                        query = query.Where(p =>
                            (p.Nombres != null && p.Nombres.ToLower().Contains(nombresCritLower)) ||
                            (p.Apellidos != null && p.Apellidos.ToLower().Contains(nombresCritLower))
                        );
                    }

                    query = query.OrderBy(p => p.Apellidos).ThenBy(p => p.Nombres);

                    var personas = query.ToList();

                    if (personas == null || personas.Count == 0)
                        return Resultado<List<Personas>>.Fail("No se encontraron personas con los criterios de búsqueda.");

                    return Resultado<List<Personas>>.Ok(personas);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al obtener a la persona buscada:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<List<Personas>>.Fail(msg);
            }
        }

        #endregion

        #region Edición (registro y modificación)

        /// <summary>
        /// Registra una nueva persona en la base de datos.
        /// </summary>
        /// <param name="persona">Objeto Persona a registrar.</param>
        /// <returns>Resultado con el Id de la persona registrada o mensaje de error.</returns>
        public static Resultado<int> RegistrarPersona(Personas? persona)
        {
            // Validación previa
            if (persona == null)
                return Resultado<int>.Fail("La persona no puede ser nula.");

            if (string.IsNullOrWhiteSpace(persona.Nombres))
                return Resultado<int>.Fail("El campo Nombres es obligatorio.");

            if (string.IsNullOrWhiteSpace(persona.Apellidos))
                return Resultado<int>.Fail("El campo Apellidos es obligatorio.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidades(
                        (contexto.Personas, nameof(contexto.Personas)),
                        (contexto.Domicilios, nameof(contexto.Domicilios)),
                        (contexto.Contactos, nameof(contexto.Contactos))
                    );

                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<int>.Fail(rc.Mensaje);
                    }

                    // Evitar inserciones accidentales de entidades relacionadas
                    persona.IdPersona = null;
                    persona.Domicilios = null;

                    contexto.Personas.Add(persona);
                    contexto.SaveChanges();

                    if (persona.IdPersona != null && persona.IdPersona > 0)
                        return Resultado<int>.Ok(persona.IdPersona.Value);

                    var msg = "No se pudo obtener el Id de la persona registrada.";
                    Logger.LogError(msg);
                    return Resultado<int>.Fail(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al registrar la persona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<int>.Fail(msg);
            }
        }

        /// <summary>
        /// Actualiza los datos de una persona existente.
        /// </summary>
        /// <param name="persona">Objeto Persona con los datos actualizados.</param>
        /// <returns>Resultado indicando éxito o fallo.</returns>
        public static Resultado<bool> ModificarPersona(Personas? persona)
        {
            if (persona == null || persona.IdPersona == null || persona.IdPersona <= 0)
                return Resultado<bool>.Fail("Debe indicar una persona válida con IdPersona.");

            try
            {
                using (var contexto = new Contexto())
                {
                    var comprobacion = new ComprobacionContexto(contexto);
                    var rc = comprobacion.ComprobarEntidad(contexto.Personas, nameof(contexto.Personas));
                    if (!rc.Success)
                    {
                        Logger.LogError(rc.Mensaje);
                        return Resultado<bool>.Fail(rc.Mensaje);
                    }

                    // Recuperar la entidad existente para evitar sobrescribir navegaciones
                    var existente = contexto.Personas.FirstOrDefault(p => p.IdPersona == persona.IdPersona);
                    if (existente == null)
                        return Resultado<bool>.Fail($"No se encontró la persona con Id {persona.IdPersona}.");

                    // Actualizar solo los campos del modelo Personas
                    existente.Dni = persona.Dni;
                    existente.Nombres = persona.Nombres;
                    existente.Apellidos = persona.Apellidos;
                    existente.FechaNac = persona.FechaNac;
                    existente.IdDomicilio = persona.IdDomicilio;
                    existente.Domicilios = persona.Domicilios;

                    int cambios = contexto.SaveChanges();

                    return cambios > 0
                        ? Resultado<bool>.Ok(true)
                        : Resultado<bool>.Fail("No se pudo actualizar la información de la persona.");
                }
            }
            catch (Exception ex)
            {
                var msg = $"Error al actualizar la persona:\n{ex.ToString()}";
                Logger.LogError(msg);
                return Resultado<bool>.Fail(msg);
            }
        }

        #endregion

    }
}
